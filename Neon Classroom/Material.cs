using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;
using Button = System.Windows.Forms.Button;
using Label = System.Windows.Forms.Label;

namespace Neon_Classroom
{
    public partial class Material : Form
    {
        SqlConnection cn = new SqlConnection();
        SqlCommand cmd = new SqlCommand();
        SqlDataReader reader;

        int classId;
        int currYPos = 0;
        int classItemId = 0;
        TabPage screen;
        string pageName;
        public Material(int id, TabPage scr, int classItemId, string pageName)
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
            currYPos = 5;
            cn.Open();

            cmd = new SqlCommand("EXEC getMaterial @classItemId", cn);
            cmd.Parameters.AddWithValue("@classItemId", classItemId);
            reader = cmd.ExecuteReader();
            reader.Read();
            label1.Text = reader["title"].ToString();
            label2.Text = reader["name"].ToString() + " ● " + reader["publishTime"].ToString();
            string description = reader["description"].ToString();
            reader.Close();

            int internalY = 5;

            if (description != "")
            {
                Label instr = new Label();
                instr.Text = description;
                instr.Font = new Font("Microsoft Sans Serif", 10);
                instr.Location = new Point(5, internalY);
                instr.Size = new Size(500, 40);
                internalY += 45;
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
                btn.Size = new Size(200, 25);
                btn.Font = new Font("Microsoft Sans Serif", 12, FontStyle.Bold);
                internalY += 30;
                classWorkPanel.Controls.Add(btn);
            }
            reader.Close();

            Panel sep = new Panel();
            sep.BackColor = Color.Black;
            sep.Size = new Size(575, 1);
            sep.BorderStyle = BorderStyle.FixedSingle;
            sep.Location = new Point(0, internalY);

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
            comment.Location = new Point(35, internalY +5);
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
