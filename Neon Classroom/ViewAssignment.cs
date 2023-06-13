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
    public partial class ViewAssignment : Form
    {
        SqlConnection cn = new SqlConnection();
        SqlCommand cmd = new SqlCommand();
        SqlDataReader reader;

        int classId;
        int currYPos = 0;
        int subId = 0;
        TabPage screen;
        ArrayList files = new ArrayList();
        string name = "";
        int maxMarks;
        int ass_id;
        public ViewAssignment(int id, TabPage scr, int ass_id, string name, int maxMarks, int subId)
        {
            screen = scr;
            InitializeComponent();
            classId = id;
            cn = new SqlConnection(DB.connStr);
            this.subId = subId;
            this.ass_id = ass_id;
            this.maxMarks = maxMarks;
            this.name = name;
            updatePanel();
        }


        private void updatePanel()
        {
            classWorkPanel.Controls.Clear();
            currYPos = 5;
            cn.Open();

            int internalY = 5;

            cmd = new SqlCommand("EXEC assignmentSubmission @subId, @userId", cn);
            cmd.Parameters.AddWithValue("@subId", subId);
            cmd.Parameters.AddWithValue("@userId", DB.userId);
            reader = cmd.ExecuteReader();
            reader.Read();
            numericUpDown1.Value = int.Parse(reader["obtainedMarks"].ToString());
            label3.Text = reader["name"].ToString();
            int subcId = int.Parse(reader["classItemsId"].ToString());
            reader.Close();

            cmd = new SqlCommand("SELECT * FROM AttachmentFiles WHERE classItemId = @classItemId;", cn);
            cmd.Parameters.AddWithValue("@classItemId", subcId);
            reader = cmd.ExecuteReader();
            for (int i = 0; reader.Read(); i++)
            {
                Button btn = new Button();
                btn.Text = "File";
                btn.Location = new Point(5, internalY);
                btn.Size = new Size(120, 20);
                string n = reader["fileName"].ToString();
                btn.Font = new Font("Microsoft Sans Serif", 12, FontStyle.Bold);
                btn.Click += (s, e) => {
                    PDF view = new PDF(n);
                    view.Show();
                };
                internalY += 25;
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
            name.Text = "Private Comments";
            name.Font = new Font("Microsoft Sans Serif", 12, FontStyle.Bold);
            name.Location = new Point(0, internalY);
            name.Size = new Size(200, 15);

            internalY += 25;
            classWorkPanel.Controls.Add(name);

            cmd = new SqlCommand("EXEC getComments @classItemId", cn);
            cmd.Parameters.AddWithValue("@classItemId", subcId);
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
                    cmd1.Parameters.AddWithValue("@classItemId", subcId);
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
            cn.Close();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            ManageGrade a = new ManageGrade(classId, screen, ass_id, name, maxMarks);
            a.TopLevel = false;
            a.AutoScroll = true;
            screen.Controls.Add(a);
            a.FormBorderStyle = FormBorderStyle.None;
            a.Show();
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (numericUpDown1.Value >= 0)
            {
                SqlConnection c = new SqlConnection(DB.connStr);
                c.Open();
                SqlCommand cm = new SqlCommand("UPDATE Submission SET obtainedMarks = @obtainedMarks WHERE id = @subId", c);
                cm.Parameters.AddWithValue("@obtainedMarks", numericUpDown1.Value);
                cm.Parameters.AddWithValue("@subId", subId);
                if (cm.ExecuteNonQuery() <= 0) { MessageBox.Show("Unable to Update Submission", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                c.Close();
                MessageBox.Show("Marks Updated Successfully", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                updatePanel();
            }
            else
            {
                MessageBox.Show("Marks Should not be negative", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
        }
    }
}
