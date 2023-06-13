using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace Neon_Classroom
{
    public partial class Class : Form
    {
        SqlConnection cn = new SqlConnection();
        SqlCommand cmd = new SqlCommand();
        SqlDataReader reader;

        int classId;
        string classCode, className, subject;

        static bool isAnnouncedTabOpen = false;

        private void button3_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "" || textBox2.Text == "")
            {
                MessageBox.Show("Fill the Field to Update", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                cn.Open();
                cmd = new SqlCommand("UPDATE Class SET className = @className, subject = @subject, section = @section, room = @room WHERE id = @classId;", cn);
                cmd.Parameters.AddWithValue("@classId", classId);
                cmd.Parameters.AddWithValue("@className", textBox1.Text);
                cmd.Parameters.AddWithValue("@subject", textBox2.Text);
                cmd.Parameters.AddWithValue("@section", textBox3.Text);
                cmd.Parameters.AddWithValue("@room", textBox4.Text);
                if (cmd.ExecuteNonQuery() <= 0)
                {
                    MessageBox.Show("Unable to Update Class", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show("Changes Applied Successfully", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                reader.Close();
                cn.Close();
                update();
            }
        }

        SplitContainer screen;

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you Sure You Want to Delete Class", "Delete Class", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                cn.Open();
                cmd = new SqlCommand("UPDATE Class SET isActive = 0 WHERE id = @classId;", cn);
                cmd.Parameters.AddWithValue("@classId", classId);
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

        private void button2_Click(object sender, EventArgs e)
        {
            
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            StudentReport s = new StudentReport(classId, tabPage6);
            s.TopLevel = false;
            s.AutoScroll = true;
            tabPage6.Controls.Clear();
            tabPage6.Controls.Add(s);
            s.FormBorderStyle = FormBorderStyle.None;
            s.Show();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            AssignmentReport s = new AssignmentReport(classId, tabPage6);
            s.TopLevel = false;
            s.AutoScroll = true;
            tabPage6.Controls.Clear();
            tabPage6.Controls.Add(s);
            s.FormBorderStyle = FormBorderStyle.None;
            s.Show();
        }

        public Class(int id, SplitContainer scr)
        {
            screen = scr;
            InitializeComponent();
            classId = id;
            if (DB.role != "Teacher")
            {
                tabControl1.TabPages.Remove(tabPage4);
                tabControl1.TabPages.Remove(tabPage5);
                tabControl1.TabPages.Remove(tabPage6);
            }


            update();

            Stream s = new Stream(classId, tabPage1);
            s.TopLevel = false;
            s.AutoScroll = true;
            tabPage1.Controls.Add(s);
            s.FormBorderStyle = FormBorderStyle.None;
            s.Show();

            Classwork cw = new Classwork(classId, tabPage2);
            cw.TopLevel = false;
            cw.AutoScroll = true;
            tabPage2.Controls.Add(cw);
            cw.FormBorderStyle = FormBorderStyle.None;
            cw.Show();

            Person per = new Person(classId, screen);
            per.TopLevel = false;
            per.AutoScroll = true;
            tabPage3.Controls.Add(per);
            per.FormBorderStyle = FormBorderStyle.None;
            per.Show();

            Grade grade = new Grade(classId, tabPage4);
            grade.TopLevel = false;
            grade.AutoScroll = true;
            tabPage4.Controls.Add(grade);
            grade.FormBorderStyle = FormBorderStyle.None;
            grade.Show();

            

        }

        private void update()
        {
            cn = new SqlConnection(DB.connStr);
            cn.Open();
            cmd = new SqlCommand("EXEC getClass @classId;", cn);
            cmd.Parameters.AddWithValue("@classId", classId);
            reader = cmd.ExecuteReader();
            reader.Read();
            classCode = reader["code"].ToString();
            className = reader["className"].ToString();
            subject = reader["subject"].ToString();

            textBox1.Text = className = reader["className"].ToString();
            textBox2.Text = className = reader["subject"].ToString();
            textBox3.Text = className = reader["section"].ToString();
            textBox4.Text = className = reader["room"].ToString();

            bool isActive = bool.Parse(reader["isActive"].ToString());
            if (isActive)
            {
                label7.Text = "Active";
            }
            else
            {
                label7.Text = "Archieved";
                label8.Visible = false;
                button1.Visible = false;
            }

            reader.Close();
            cn.Close();
        }



    }
}
