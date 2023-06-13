using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;
using Button = System.Windows.Forms.Button;

namespace Neon_Classroom
{
    public partial class Classwork : Form
    {
        SqlConnection cn = new SqlConnection();
        SqlCommand cmd = new SqlCommand();
        SqlDataReader reader;

        int classId;
        int currYPos = 0;
        TabPage screen;
        ArrayList filename = new ArrayList();
        public Classwork(int id, TabPage scr)
        {
            screen = scr;
            InitializeComponent();
            classId = id;
            cn = new SqlConnection(DB.connStr);
            classWorkPanel.Location = new Point(5, 42);
            panel1.Location = new Point(3, 5);
            panel2.Location = new Point(3, 5);
            
            button1.Click += (s, e) => { 
                _ = panel1.Visible ? panel1.Visible = false : panel1.Visible = true;
                _ = classWorkPanel.Visible ? classWorkPanel.Visible = false : classWorkPanel.Visible = true;
                filename.Clear();
            };
            button2.Click += (s, e) => { 
                _ = panel2.Visible ? panel2.Visible = false : panel2.Visible = true;
                _ = classWorkPanel.Visible ? classWorkPanel.Visible = false : classWorkPanel.Visible = true;
                filename.Clear();
            };
            button4.Click += (s, e) => {
                _ = panel1.Visible ? panel1.Visible = false : panel1.Visible = true;
                _ = classWorkPanel.Visible ? classWorkPanel.Visible = false : classWorkPanel.Visible = true;
                filename.Clear();
            };
            button7.Click += (s, e) => {
                _ = panel2.Visible ? panel2.Visible = false : panel2.Visible = true;
                _ = classWorkPanel.Visible ? classWorkPanel.Visible = false : classWorkPanel.Visible = true;
                filename.Clear();
            };

            if (DB.role != "Teacher")
            {
                button1.Visible = false;
                button2.Visible = false;
                classWorkPanel.Location = new Point(5, 5);
            }

            updatePanel();
        }


        private void updatePanel()
        {
            classWorkPanel.Controls.Clear();
            currYPos = 5;
            cn.Open();

            Label ann = new Label();
            ann.Text = "Assignments";
            ann.Font = new Font("Microsoft Sans Serif", 18);
            ann.Location = new Point(2, currYPos);
            ann.AutoSize = true;
            ann.MaximumSize = new Size(520, 25);
            classWorkPanel.Controls.Add(ann);

            currYPos += 25;
            
            Panel sep = new Panel();
            sep.BackColor = Color.Black;
            sep.Size = new Size(575, 1);
            sep.BorderStyle = BorderStyle.FixedSingle;
            sep.Location = new Point(5, currYPos);
            currYPos += 5;
            classWorkPanel.Controls.Add(sep);


            cmd = new SqlCommand("EXEC getAssignments @classId", cn);
            cmd.Parameters.AddWithValue("@classId", classId);
            reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                int cid = int.Parse(reader["classItemId"].ToString());
                Panel assignmentInfo = new Panel();
                assignmentInfo.Size = new Size(532, 40);
                assignmentInfo.BorderStyle = BorderStyle.FixedSingle;
                assignmentInfo.Location = new Point(5, currYPos);
                assignmentInfo.Click += (s, e) => {
                    Assignment a = new Assignment(classId, screen, cid, "Classwork");
                    a.TopLevel = false;
                    a.AutoScroll = true;
                    screen.Controls.Add(a);
                    a.FormBorderStyle = FormBorderStyle.None;
                    a.Show();
                    this.Close();
                };
                currYPos += 45;

                Label name = new Label();
                name.Text = reader["title"].ToString();
                name.Font = new Font("Microsoft Sans Serif", 12, FontStyle.Bold);
                name.Location = new Point(10, 11);
                name.Size = new Size(200, 16);

                Label dateDue = new Label();
                dateDue.Text = reader["dueDateTime"].ToString();
                dateDue.Font = new Font("Microsoft Sans Serif", 10);
                dateDue.Location = new Point(370, 13);
                dateDue.Size = new Size(200, 15);

                if (DB.role == "Teacher")
                {

                    Panel moreOptions = new Panel();
                    moreOptions.Size = new Size(70, 50);
                    moreOptions.BorderStyle = BorderStyle.FixedSingle;
                    moreOptions.Location = new Point(465, currYPos - 5);


                    Button edit = new Button();
                    edit.Text = "Edit";
                    edit.Location = new Point(0, 0);
                    edit.Size = new Size(69, 23);
                    edit.Click += (s, e) =>
                    {
                        EditAssignment a = new EditAssignment(classId, screen, cid, "Classwork");
                        screen.Controls.Clear();
                        a.TopLevel = false;
                        a.AutoScroll = true;
                        screen.Controls.Add(a);
                        a.FormBorderStyle = FormBorderStyle.None;
                        a.Show();
                        this.Close();
                    };

                    Button del = new Button();
                    del.Text = "Delete";
                    del.Location = new Point(0, 25);
                    del.Size = new Size(69, 23);
                    del.Click += (s, e) =>
                    {
                        DialogResult result = MessageBox.Show("Are you sure you want to delete this Assignment?", "Delete Assignment", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result == DialogResult.Yes)
                        {
                            SqlConnection c = new SqlConnection(DB.connStr);
                            c.Open();
                            SqlCommand cm = new SqlCommand("DELETE FROM ClassItems WHERE id = @classItemId", c);
                            cm.Parameters.AddWithValue("@classItemId", cid);
                            if (cm.ExecuteNonQuery() <= 0) { MessageBox.Show("Unable to Delete Assignment", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                            c.Close();
                            updatePanel();
                        }
                    };


                    moreOptions.Controls.Add(edit);
                    moreOptions.Controls.Add(del);

                    moreOptions.Visible = false;

                    PictureBox pictureBox = new PictureBox();
                    pictureBox.Image = Properties.Resources.more;
                    pictureBox.Size = new Size(7, 20);
                    pictureBox.Location = new Point(515, 10);
                    pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
                    pictureBox.Click += (s, e) =>
                    {
                        _ = (moreOptions.Visible) ? (moreOptions.Visible = false) : (moreOptions.Visible = true);
                    };
                    classWorkPanel.Controls.Add(moreOptions);
                    assignmentInfo.Controls.Add(pictureBox);
                }
                assignmentInfo.Controls.Add(dateDue);
                assignmentInfo.Controls.Add(name);

                
                classWorkPanel.Controls.Add(assignmentInfo);
            }

            reader.Close();


            currYPos += 5;

            Label material = new Label();
            material.Text = "Course Material";
            material.Font = new Font("Microsoft Sans Serif", 18);
            material.Location = new Point(2, currYPos);
            material.AutoSize = true;
            material.MaximumSize = new Size(520, 25);
            classWorkPanel.Controls.Add(material);

            currYPos += 25;

            Panel seper = new Panel();
            seper.BackColor = Color.Black;
            seper.Size = new Size(575, 1);
            seper.BorderStyle = BorderStyle.FixedSingle;
            seper.Location = new Point(5, currYPos);
            currYPos += 5;
            classWorkPanel.Controls.Add(seper);

            cmd = new SqlCommand("EXEC getMaterials @classId", cn);
            cmd.Parameters.AddWithValue("@classId", classId);
            reader = cmd.ExecuteReader();
            while (reader.Read())
            {

                int cid = int.Parse(reader["classItemId"].ToString());
                Panel assignmentInfo = new Panel();
                assignmentInfo.Size = new Size(532, 40);
                assignmentInfo.BorderStyle = BorderStyle.FixedSingle;
                assignmentInfo.Location = new Point(5, currYPos);
                assignmentInfo.Click += (s, e) => {
                    Material a = new Material(classId, screen, cid, "Classwork");
                    a.TopLevel = false;
                    a.AutoScroll = true;
                    screen.Controls.Add(a);
                    a.FormBorderStyle = FormBorderStyle.None;
                    a.Show();
                    this.Close();
                };
                currYPos += 45;

                

                Label name = new Label();
                name.Text = reader["title"].ToString();
                name.Font = new Font("Microsoft Sans Serif", 12, FontStyle.Bold);
                name.Location = new Point(10, 11);
                name.Size = new Size(200, 16);

                if (DB.role == "Teacher")
                {
                    Panel moreOptions = new Panel();
                    moreOptions.Size = new Size(70, 50);
                    moreOptions.BorderStyle = BorderStyle.FixedSingle;
                    moreOptions.Location = new Point(465, currYPos - 5);

                    Button edit = new Button();
                    edit.Text = "Edit";
                    edit.Location = new Point(0, 0);
                    edit.Size = new Size(69, 23);
                    edit.Click += (s, e) =>
                    {
                        EditMaterial a = new EditMaterial(classId, screen, cid, "Classwork");
                        screen.Controls.Clear();
                        a.TopLevel = false;
                        a.AutoScroll = true;
                        screen.Controls.Add(a);
                        a.FormBorderStyle = FormBorderStyle.None;
                        a.Show();
                        this.Close();
                    };

                    Button del = new Button();
                    del.Text = "Delete";
                    del.Location = new Point(0, 25);
                    del.Size = new Size(69, 23);
                    del.Click += (s, e) =>
                    {
                        DialogResult result = MessageBox.Show("Are you sure you want to delete this Material?", "Delete Material", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result == DialogResult.Yes)
                        {
                            SqlConnection c = new SqlConnection(DB.connStr);
                            c.Open();
                            SqlCommand cm = new SqlCommand("DELETE FROM ClassItems WHERE id = @classItemId", c);
                            cm.Parameters.AddWithValue("@classItemId", cid);
                            if (cm.ExecuteNonQuery() <= 0) { MessageBox.Show("Unable to Delete Material", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                            c.Close();
                            updatePanel();
                        }
                    };

                    moreOptions.Controls.Add(edit);
                    moreOptions.Controls.Add(del);

                    moreOptions.Visible = false;

                    PictureBox pictureBox = new PictureBox();
                    pictureBox.Image = Properties.Resources.more;
                    pictureBox.Size = new Size(7, 20);
                    pictureBox.Location = new Point(515, 10);
                    pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
                    pictureBox.Click += (s, e) =>
                    {
                        _ = (moreOptions.Visible) ? (moreOptions.Visible = false) : (moreOptions.Visible = true);
                    };

                    assignmentInfo.Controls.Add(pictureBox);
                    classWorkPanel.Controls.Add(moreOptions);
                }
                assignmentInfo.Controls.Add(name);

               
                classWorkPanel.Controls.Add(assignmentInfo);
            }

            reader.Close();


            cn.Close();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            string title = textBox2.Text;
            string instr = richTextBox2.Text;
            int pts = int.Parse(numericUpDown1.Value.ToString());
            DateTime dt = dateTimePicker1.Value;

            if ((title == "") || (instr == "") || (pts < 0))
            {
                MessageBox.Show("Fill All Fields", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                SqlConnection conn = new SqlConnection(DB.connStr);
                conn.Open();
                SqlCommand cmdFile;
                SqlDataReader r;
                cmdFile = new SqlCommand("EXEC createAssignment @classId, @points, @title, @instructions, @dueDate", conn);
                cmdFile.Parameters.AddWithValue("@classId", classId);
                cmdFile.Parameters.AddWithValue("@points", pts);
                cmdFile.Parameters.AddWithValue("@title", title);
                cmdFile.Parameters.AddWithValue("@instructions", instr);
                cmdFile.Parameters.AddWithValue("@dueDate", dt);
                r = cmdFile.ExecuteReader();
                r.Read();
                int ClassItemID = (int)r[0];
                r.Close();
                for (int i = 0; i < filename.Count; i++)
                {
                    cmdFile = new SqlCommand("EXEC insertFile @fileName, @classItemId", conn);
                    cmdFile.Parameters.AddWithValue("@fileName", filename[i].ToString());
                    cmdFile.Parameters.AddWithValue("@classItemId", ClassItemID);
                    if (cmdFile.ExecuteNonQuery() <= 0)
                    {
                        MessageBox.Show("Fail");
                    }
                }
                SqlConnection conn2 = new SqlConnection(DB.connStr);
                conn2.Open();
                SqlCommand cmdFile2;
                cmdFile = new SqlCommand("SELECT * FROM Enrollment WHERE classId = @classId", conn);
                cmdFile.Parameters.AddWithValue("@classId", classId);
                r = cmdFile.ExecuteReader();
                while (r.Read())
                {
                    cmdFile2 = new SqlCommand("EXEC createSubmission @classId, @classItemId, @studentId", conn2);
                    cmdFile2.Parameters.AddWithValue("@classId", classId);
                    cmdFile2.Parameters.AddWithValue("@classItemId", ClassItemID);
                    cmdFile2.Parameters.AddWithValue("@studentId", r["studentId"]);
                    if (cmdFile2.ExecuteNonQuery() <= 0)
                    {
                        MessageBox.Show("Failed to Create Submissions");
                        break;
                    }
                    
                }
                r.Close();
                conn2.Close();
                conn.Close();


                _ = panel2.Visible ? panel2.Visible = false : panel2.Visible = true;
                _ = classWorkPanel.Visible ? classWorkPanel.Visible = false : classWorkPanel.Visible = true;
                textBox2.Text = "";
                richTextBox2.Text = "";
                filename.Clear();
                updatePanel();
            }
        }

        private void button6_Click(object sender, EventArgs e)
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
                filename.Add(str);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string title = textBox1.Text;
            string desc = richTextBox1.Text;

            if ((title == "") || (desc == ""))
            {
                MessageBox.Show("Fill All Fields", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                SqlConnection conn = new SqlConnection(DB.connStr);
                conn.Open();
                SqlCommand cmdFile;
                SqlDataReader r;
                cmdFile = new SqlCommand("EXEC createMaterial @classId,@title,@desc", conn);
                cmdFile.Parameters.AddWithValue("@classId", classId);
                cmdFile.Parameters.AddWithValue("@title", title);
                cmdFile.Parameters.AddWithValue("@desc", desc);
                r = cmdFile.ExecuteReader();
                r.Read();
                int ClassItemID = (int)r[0];
                r.Close();
                for (int i = 0; i < filename.Count; i++)
                {
                    cmdFile = new SqlCommand("EXEC insertFile @fileName, @classItemId", conn);
                    cmdFile.Parameters.AddWithValue("@fileName", filename[i].ToString());
                    cmdFile.Parameters.AddWithValue("@classItemId", ClassItemID);
                    if (cmdFile.ExecuteNonQuery() <= 0)
                    {
                        MessageBox.Show("Fail");
                    }
                }
                conn.Close();
                _ = panel1.Visible ? panel1.Visible = false : panel1.Visible = true;
                _ = classWorkPanel.Visible ? classWorkPanel.Visible = false : classWorkPanel.Visible = true;
                textBox1.Text = "";
                richTextBox1.Text = "";
                filename.Clear();
                updatePanel();
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            button6_Click(sender, e);
        }
    }
}
