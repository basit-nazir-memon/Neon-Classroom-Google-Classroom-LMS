using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;
using Button = System.Windows.Forms.Button;

namespace Neon_Classroom
{
    public partial class Assignment : Form
    {
        SqlConnection cn = new SqlConnection();
        SqlCommand cmd = new SqlCommand();
        SqlDataReader reader;

        int classId;
        int currYPos = 0;
        int classItemId = 0;
        TabPage screen;
        ArrayList files = new ArrayList();
        string pageName;
        public Assignment(int id, TabPage scr, int classItemId, string pageName)
        {
            screen = scr;
            InitializeComponent();
            classId = id;
            cn = new SqlConnection(DB.connStr);
            this.classItemId = classItemId;
            updatePanel();
            this.pageName = pageName;
        }


        private void updatePanel()
        {
            classWorkPanel.Controls.Clear();
            panel2.Controls.Clear();
            panel3.Controls.Clear();

            currYPos = 5;
            cn.Open();
            cmd = new SqlCommand("EXEC getAssignment @classItemId", cn);
            cmd.Parameters.AddWithValue("@classItemId", classItemId);
            reader = cmd.ExecuteReader();
            reader.Read();
            label1.Text = reader["title"].ToString();
            label2.Text = reader["name"].ToString() + " ● " + reader["publishTime"].ToString();
            label4.Text = reader["points"] + " Points";
            label3.Text = reader["dueDateTime"].ToString();
            string instructions = reader["instructions"].ToString();
            reader.Close();
            int subCId = 0;
            int internalY = 5;

            if (instructions != "")
            {
                Label instr = new Label();
                instr.Text = instructions;
                instr.Font = new Font("Microsoft Sans Serif", 10);
                instr.Location = new Point(5, internalY);
                instr.Size = new Size(200, 50);
                internalY += 55;
                classWorkPanel.Controls.Add(instr);
            }

            cmd = new SqlCommand("SELECT * FROM AttachmentFiles WHERE classItemId = @classItemId", cn);
            cmd.Parameters.AddWithValue("@classItemId", classItemId);
            reader = cmd.ExecuteReader();

            for (int i = 0; reader.Read(); i++)
            {
                Button btn = new Button();
                btn.Text = "File";
                btn.Location = new Point(5, internalY);
                btn.Size = new Size(200, 30);
                btn.Font = new Font("Microsoft Sans Serif", 12, FontStyle.Bold);
                string n = reader["fileName"].ToString();
                btn.Click += (s, e) => {
                    PDF view = new PDF(n);
                    view.Show();
                };
                internalY += 35;
                classWorkPanel.Controls.Add(btn);
            }
            reader.Close();

            Panel sep = new Panel();
            sep.BackColor = Color.Black;
            sep.Size = new Size(575, 1);
            sep.BorderStyle = BorderStyle.FixedSingle;
            sep.Location = new Point(5, internalY);

            internalY += 5;
            classWorkPanel.Controls.Add(sep);

            Label name = new Label();
            name.Text = "Class Comments";
            name.Font = new Font("Microsoft Sans Serif", 12, FontStyle.Bold);
            name.Location = new Point(5, internalY);
            name.Size = new Size(200, 15);

            internalY += 25;
            classWorkPanel.Controls.Add(name);

            cmd = new SqlCommand("EXEC getComments @classItemId", cn);
            cmd.Parameters.AddWithValue("@classItemId", classItemId);
            reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                PictureBox pict = new PictureBox();
                pict.Image = Properties.Resources.P1;
                pict.Size = new Size(25, 25);
                pict.Location = new Point(5, internalY);
                pict.SizeMode = PictureBoxSizeMode.StretchImage;

                Label person = new Label();
                person.Text = reader["name"].ToString() + "   " + reader["time"].ToString();
                person.Font = new Font("Microsoft Sans Serif", 9);
                person.Location = new Point(35, internalY);
                person.AutoSize = true;
                person.MaximumSize = new Size(400, 30);

                internalY += 15;

                Label cmnt = new Label();
                cmnt.Text = reader["comments"].ToString();
                cmnt.Font = new Font("Microsoft Sans Serif", 10);
                cmnt.Location = new Point(35, internalY);
                cmnt.AutoSize = true;
                cmnt.MaximumSize = new Size(540, 80);

                internalY += 30;
                classWorkPanel.Controls.Add(pict);
                classWorkPanel.Controls.Add(person);
                classWorkPanel.Controls.Add(cmnt);
            }
            reader.Close();
            Panel seperator = new Panel();
            seperator.BackColor = Color.Black;
            seperator.Size = new Size(575, 1);
            seperator.BorderStyle = BorderStyle.FixedSingle;
            seperator.Location = new Point(0, internalY);

            internalY += 5;


            int cid = classItemId;

            PictureBox pic = new PictureBox();
            pic.Image = Properties.Resources.P1;
            pic.Size = new Size(25, 25);
            pic.Location = new Point(5, internalY);
            pic.SizeMode = PictureBoxSizeMode.StretchImage;

            System.Windows.Forms.TextBox comment = new System.Windows.Forms.TextBox();
            comment.Location = new Point(35, internalY + 5);
            comment.Size = new Size(classWorkPanel.Width - 105, 30);
            comment.Font = new Font("Microsoft Sans Serif", 10);

            Button pst = new Button();
            pst.Text = "Post";
            pst.Location = new Point(classWorkPanel.Width - 60, internalY);
            pst.Size = new Size(60, 25);
            pst.Click += (s, e) =>
            {
                if (comment.Text == "")
                {
                    MessageBox.Show("Enter Some Comments to Post", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    SqlConnection con = new SqlConnection(DB.connStr);
                    SqlCommand cmd1 = new SqlCommand();
                    con.Open();
                    cmd1 = new SqlCommand("INSERT INTO Comment (userId, classItemId, time, comments) VALUES (@userId, @classItemId, @time, @comments)", con);
                    cmd1.Parameters.AddWithValue("@userId", DB.userId);
                    cmd1.Parameters.AddWithValue("@classItemId", cid);
                    cmd1.Parameters.AddWithValue("@time", DateTime.Now);
                    cmd1.Parameters.AddWithValue("@comments", comment.Text);
                    if (cmd1.ExecuteNonQuery() <= 0)
                    {
                        MessageBox.Show("Failed to Post Comment", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    con.Close();
                    comment.Text = "";
                    updatePanel();
                }
            };

            currYPos += 180;
            classWorkPanel.Controls.Add(name);
            classWorkPanel.Controls.Add(seperator);
            classWorkPanel.Controls.Add(pic);
            classWorkPanel.Controls.Add(comment);
            classWorkPanel.Controls.Add(pst);

            classWorkPanel.Size = new Size(classWorkPanel.Width, internalY + 40);
           

            internalY = 5;

            Label sub = new Label();
            sub.Text = "Submissions";
            sub.Font = new Font("Microsoft Sans Serif", 12, FontStyle.Bold);
            sub.Location = new Point(5, internalY);
            sub.AutoSize = true;

            internalY += 20;

            
            panel2.Controls.Add(sub);
            
            if (DB.role == "Teacher")
            {

                cmd = new SqlCommand("EXEC getSubmissions @classItemId", cn);
                cmd.Parameters.AddWithValue("@classItemId", classItemId);
                reader = cmd.ExecuteReader();
                for (int i = 0; reader.Read(); i++)
                {
                    {
                        Label std = new Label();
                        std.Text = reader["name"].ToString();
                        std.Font = new Font("Microsoft Sans Serif", 10);
                        std.Location = new Point(5, internalY);
                        std.Size = new Size(200, 15);
                        panel2.Controls.Add(std);

                        internalY += 15;
                    }
                }
                reader.Close();
            }
            else
            {
                cmd = new SqlCommand("EXEC getSubmission @classItemId, @userId", cn);
                cmd.Parameters.AddWithValue("@classItemId", classItemId);
                cmd.Parameters.AddWithValue("@userId", DB.userId);
                reader = cmd.ExecuteReader();
                reader.Read();
                //DateTime date = DateTime.Parse(submission)
                if (reader.HasRows)
                {


                    bool isSubmitted = bool.Parse(reader["isTurnedIn"].ToString());
                    subCId = int.Parse(reader["classItemsId"].ToString());

                    if (reader["obtainedMarks"].ToString() != "")
                    {

                        Label std = new Label();
                        std.Text = reader["obtainedMarks"].ToString() + " pts";
                        std.Font = new Font("Microsoft Sans Serif", 9);
                        std.Location = new Point(120, 5);
                        std.Size = new Size(60, 15);
                        panel2.Controls.Add(std);
                    }
                    reader.Close();
                    cmd = new SqlCommand("EXEC getSubmittedFiles @classItemId, @userId", cn);
                    cmd.Parameters.AddWithValue("@classItemId", classItemId);
                    cmd.Parameters.AddWithValue("@userId", DB.userId);
                    reader = cmd.ExecuteReader();
                    for (int i = 0; reader.Read(); i++)
                    {
                        Button btn = new Button();
                        btn.Text = "File";
                        btn.Location = new Point(5, internalY);
                        btn.Size = new Size(120, 20);
                        btn.Font = new Font("Microsoft Sans Serif", 12, FontStyle.Bold);
                        internalY += 25;
                        panel2.Controls.Add(btn);
                    }
                    reader.Close();


                    if (isSubmitted)
                    {
                        internalY += 10;
                        Button btn = new Button();
                        btn.Text = "Unsubmit";
                        btn.Location = new Point(5, internalY);
                        btn.Size = new Size(panel2.Width - 10, 25);
                        btn.Font = new Font("Microsoft Sans Serif", 12, FontStyle.Bold);
                        internalY += 25;
                        btn.ForeColor = Color.FromArgb(137, 79, 157);
                        btn.BackColor = Color.White;
                        panel2.Controls.Add(btn);

                        btn.Click += (s, e) =>
                        {
                            SqlConnection conn = new SqlConnection(DB.connStr);
                            conn.Open();
                            SqlCommand cmdFile;
                            SqlDataReader r;
                            cmdFile = new SqlCommand("EXEC unSubmitSubmission @classItemId, @userId", conn);
                            cmdFile.Parameters.AddWithValue("@classItemId", cid);
                            cmdFile.Parameters.AddWithValue("@userId", DB.userId);
                            r = cmdFile.ExecuteReader();
                            r.Read();
                            int ClassItemID = (int)r[0];
                            r.Close();
                            conn.Close();
                            updatePanel();
                        };
                    }
                    else
                    {
                        internalY += 15;
                        Button btn = new Button();
                        btn.Text = "UploadFiles";
                        btn.Location = new Point(5, internalY);
                        btn.Size = new Size(panel2.Width - 10, 25);
                        btn.Font = new Font("Microsoft Sans Serif", 12, FontStyle.Bold);
                        btn.ForeColor = Color.FromArgb(137, 79, 157);
                        btn.BackColor = Color.White;
                        internalY += 25;
                        panel2.Controls.Add(btn);
                        btn.Click += (s, e) =>
                        {
                            OpenFileDialog filedailog = new OpenFileDialog();
                            filedailog.Filter = "pdf files (*.pdf) |*.pdf;";
                            string str;
                            filedailog.ShowDialog();
                            str = filedailog.FileName;
                            string Destination = "E:\\Files Upload\\";
                            var kl = str.Split('\\');
                            string fname = kl[kl.Length - 1];
                            if (str != "")
                            {
                                System.IO.File.Copy(str, Destination + fname, true);
                                files.Add(str);
                            }
                            updatePanel();
                        };

                        internalY += 3;
                        Button btn1 = new Button();
                        btn1.Text = "Turn In";
                        btn1.Location = new Point(5, internalY);
                        btn1.Size = new Size(panel2.Width - 10, 25);
                        btn1.Font = new Font("Microsoft Sans Serif", 12);
                        btn.ForeColor = Color.FromArgb(137, 79, 157);
                        btn.BackColor = Color.White;
                        internalY += 25;
                        panel2.Controls.Add(btn1);
                        btn1.Click += (s, e) =>
                        {
                            SqlConnection conn = new SqlConnection(DB.connStr);
                            conn.Open();
                            SqlCommand cmdFile;
                            SqlDataReader r;
                            cmdFile = new SqlCommand("EXEC turnInSubmission @classItemId, @userId", conn);
                            cmdFile.Parameters.AddWithValue("@classItemId", cid);
                            cmdFile.Parameters.AddWithValue("@userId", DB.userId);
                            r = cmdFile.ExecuteReader();
                            r.Read();
                            int ClassItemID = (int)r[0];
                            r.Close();
                            for (int i = 0; i < files.Count; i++)
                            {
                                cmdFile = new SqlCommand("EXEC insertFile @fileName, @classItemId", conn);
                                cmdFile.Parameters.AddWithValue("@fileName", files[i].ToString());
                                cmdFile.Parameters.AddWithValue("@classItemId", ClassItemID);
                                if (cmdFile.ExecuteNonQuery() <= 0)
                                {
                                    MessageBox.Show("Fail");
                                }
                            }
                            conn.Close();
                            updatePanel();
                        };
                    }
                }
                else
                {
                    reader.Close();
                }
            }


            panel2.Size = new Size(panel2.Width, internalY + 5);
            panel3.Visible = false;
            if (DB.role == "Student")
            {
                panel3.Visible = true;

                panel3.Location = new Point(panel3.Location.X, panel2.Location.Y + internalY + 10);

                internalY = 5;
                Label name1 = new Label();
                name1.Text = "Private Comments";
                name1.Font = new Font("Microsoft Sans Serif", 10, FontStyle.Bold);
                name1.Location = new Point(2, internalY);
                name1.Size = new Size(200, 15);

                internalY += 25;
                panel3.Controls.Add(name1);

                cmd = new SqlCommand("EXEC getComments @classItemId", cn);
                cmd.Parameters.AddWithValue("@classItemId", subCId);
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    PictureBox pict = new PictureBox();
                    pict.Image = Properties.Resources.P1;
                    pict.Size = new Size(20, 20);
                    pict.Location = new Point(2, internalY);
                    pict.SizeMode = PictureBoxSizeMode.StretchImage;

                    Label person = new Label();
                    person.Text = reader["name"].ToString() + "   \n" + reader["time"].ToString();
                    person.Font = new Font("Microsoft Sans Serif", 9);
                    person.Location = new Point(25, internalY-2);
                    person.AutoSize = true;
                    person.MaximumSize = new Size(400, 30);

                    internalY += 22;

                    Label cmnt = new Label();
                    cmnt.Text = reader["comments"].ToString();
                    cmnt.Font = new Font("Microsoft Sans Serif", 10);
                    cmnt.Location = new Point(25, internalY);
                    cmnt.AutoSize = true;
                    cmnt.MaximumSize = new Size(panel3.Width-27, 70);

                    internalY += 40;
                    panel3.Controls.Add(pict);
                    panel3.Controls.Add(person);
                    panel3.Controls.Add(cmnt);
                }
                reader.Close();
                Panel seperator1 = new Panel();
                seperator1.BackColor = Color.Black;
                seperator1.Size = new Size(panel3.Width, 1);
                seperator1.BorderStyle = BorderStyle.FixedSingle;
                seperator1.Location = new Point(0, internalY);

                internalY += 5;

                PictureBox pic1 = new PictureBox();
                pic1.Image = Properties.Resources.P1;
                pic1.Size = new Size(20, 20);
                pic1.Location = new Point(2, internalY);
                pic1.SizeMode = PictureBoxSizeMode.StretchImage;

                

                System.Windows.Forms.TextBox comment1 = new System.Windows.Forms.TextBox();
                comment1.Location = new Point(25, internalY + 2);
                comment1.Size = new Size(panel3.Width - 42-25, 30);
                comment1.Font = new Font("Microsoft Sans Serif", 10);

                Button pst1 = new Button();
                pst1.Text = "Post";
                pst1.Location = new Point(panel3.Width - 42, internalY);
                pst1.Size = new Size(40, 20);
                pst1.Click += (s, e) =>
                {
                    if (comment1.Text == "")
                    {
                        MessageBox.Show("Enter Some Comments to Post", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        SqlConnection con = new SqlConnection(DB.connStr);
                        SqlCommand cmd1 = new SqlCommand();
                        con.Open();
                        cmd1 = new SqlCommand("INSERT INTO Comment (userId, classItemId, time, comments) VALUES (@userId, @classItemId, @time, @comments)", con);
                        cmd1.Parameters.AddWithValue("@userId", DB.userId);
                        cmd1.Parameters.AddWithValue("@classItemId", subCId);
                        cmd1.Parameters.AddWithValue("@time", DateTime.Now);
                        cmd1.Parameters.AddWithValue("@comments", comment1.Text);
                        if (cmd1.ExecuteNonQuery() <= 0)
                        {
                            MessageBox.Show("Failed to Post Comment", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        con.Close();
                        comment1.Text = "";
                        updatePanel();
                    }
                };

                internalY += 25;

                panel3.Controls.Add(pst1);
                panel3.Controls.Add(comment1);
                panel3.Controls.Add(pic1);
                panel3.Controls.Add(seperator1);
                panel3.Size = new Size(panel3.Width, internalY+5); 
            }
            cn.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (pageName == "Stream")
            {
                Stream a = new Stream(classId, screen);
                a.TopLevel = false;
                a.AutoScroll = true;
                screen.Controls.Add(a);
                a.FormBorderStyle = FormBorderStyle.None;
                a.Show();
                this.Close();
            }
            else if (pageName == "Classwork")
            {
                Classwork a = new Classwork(classId, screen);
                a.TopLevel = false;
                a.AutoScroll = true;
                screen.Controls.Add(a);
                a.FormBorderStyle = FormBorderStyle.None;
                a.Show();
                this.Close();
            }
        }
    }
}
