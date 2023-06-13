using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Neon_Classroom
{
    public partial class Classess : Form
    {
        SplitContainer screen;
        SqlConnection cn = new SqlConnection();
        SqlCommand cmd = new SqlCommand();
        SqlDataReader reader;
        public Classess(SplitContainer s)
        {
            screen = s;
            InitializeComponent();
            cn = new SqlConnection(DB.connStr);

            cn.Open();
            if (DB.role == "Teacher")
            {
                cmd = new SqlCommand("EXEC ClassForTeacher @userId, @isActive;", cn);
            }
            else
            {
                cmd = new SqlCommand("EXEC getClassForStudent @userId, @isActive;", cn);
            }
            cmd.Parameters.AddWithValue("@userId", DB.userId);
            cmd.Parameters.AddWithValue("@isActive", true);
            reader = cmd.ExecuteReader();

            Panel panel2;
            for (int i = 0; reader.Read(); i++)
            {
                if (bool.Parse(reader["isActive"].ToString()) == true)
                {

                    panel2 = new Panel();
                    panel2.Tag = reader["id"].ToString();
                    PictureBox pictureBox6 = new PictureBox();
                    PictureBox pictureBox7 = new PictureBox();
                    PictureBox pictureBox5 = new PictureBox();
                    Label label6 = new Label();
                    Label label5 = new Label();
                    PictureBox pictureBox4 = new PictureBox();
                    Label label4 = new Label();

                    // intializing Panel
                    panel2.Location = new Point(9 * (i % 3 + 1) + (240 * (i % 3)), 11 * (i / 3 + 1) + (252 * (i / 3)));
                    panel2.Size = new Size(240, 252);
                    panel2.BorderStyle = BorderStyle.FixedSingle;

                    panel2.Click += new EventHandler(viewClass);


                    // initalizing Picture Box
                    pictureBox4.Image = Properties.Resources.BackDrop;
                    pictureBox4.Size = new Size(240, 105);
                    pictureBox4.Location = new Point(0, 0);
                    pictureBox4.SizeMode = PictureBoxSizeMode.StretchImage;

                    pictureBox5.Image = Properties.Resources.P1;
                    pictureBox5.Size = new Size(70, 70);
                    pictureBox5.Location = new Point(165, 70);
                    pictureBox5.SizeMode = PictureBoxSizeMode.StretchImage;

                    pictureBox6.Image = Properties.Resources.Folder_Icon;
                    pictureBox6.Size = new Size(30, 25);
                    pictureBox6.Location = new Point(203, 220);
                    pictureBox6.SizeMode = PictureBoxSizeMode.StretchImage;


                    // initializing labels
                    label4.Text = reader["className"].ToString();
                    label4.Location = new Point(2, 12);
                    label4.Font = new Font("Microsoft Sans Serif", 12, FontStyle.Bold);
                    label4.ForeColor = Color.White;
                    label4.Image = Properties.Resources.BackDrop;
                    label4.Size = new Size(240, 25);

                    label5.Text = reader["section"].ToString();
                    label5.Location = new Point(2, 40);
                    label5.Font = new Font("Microsoft Sans Serif", 9);
                    label5.Image = Properties.Resources.BackDrop;
                    label5.ForeColor = Color.White;

                    label6.Text = (DB.role == "Teacher" ? DB.userName : reader["name"].ToString());
                    label6.Location = new Point(2, 65);
                    label6.Font = new Font("Microsoft Sans Serif", 9);
                    label6.Image = Properties.Resources.BackDrop;
                    label6.ForeColor = Color.White;

                    if (DB.role == "Teacher") { 
                        pictureBox7.Image = Properties.Resources.bin_image;
                        pictureBox7.Size = new Size(20, 25);
                        pictureBox7.Location = new Point(2, 222);
                        pictureBox7.SizeMode = PictureBoxSizeMode.StretchImage;
                        pictureBox7.Click += new EventHandler(deleteClass);
                        pictureBox7.Tag = reader["id"].ToString();
                        panel2.Controls.Add(pictureBox7);
                    }

                    // adding controls
                    panel2.Controls.Add(label6);
                    panel2.Controls.Add(label5);
                    panel2.Controls.Add(label4);
                    panel2.Controls.Add(pictureBox6);
                    panel2.Controls.Add(pictureBox5);
                    panel2.Controls.Add(pictureBox4);

                    this.Controls.Add(panel2);
                }
            }

            reader.Close();
            cn.Close();

        }

        private void viewClass(object sender, EventArgs e)
        {
            Class c = new Class(int.Parse(((Panel)sender).Tag.ToString()), screen);
            c.TopLevel = false;
            c.AutoScroll = true;
            screen.Panel2.Controls.Add(c);
            c.FormBorderStyle = FormBorderStyle.None;
            this.Close();
            c.Show();
        }

       private void deleteClass(object sender, EventArgs e)
       {
            DialogResult result = MessageBox.Show("Are you Sure You Want to Delete Class", "Delete Class", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                cn.Open();
                cmd = new SqlCommand("UPDATE Class SET isActive = 0 WHERE id = @classId;", cn);
                cmd.Parameters.AddWithValue("@classId", int.Parse(((PictureBox)sender).Tag.ToString()));
                if (cmd.ExecuteNonQuery() <= 0)
                {
                    MessageBox.Show("Unable to DELETE Class", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show("Class DELETED Successfully", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                reader.Close();
                cn.Close();
                this.Close();
            }
        }
       
    }
}
