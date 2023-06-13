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
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;
using Button = System.Windows.Forms.Button;

namespace Neon_Classroom
{
    public partial class EditAssignment : Form
    {
        SqlConnection cn = new SqlConnection();
        SqlCommand cmd = new SqlCommand();
        SqlDataReader reader;

        int classId;
        int currYPos = 0;
        int classItemId;
        string pageName;
        TabPage screen;
        ArrayList filename = new ArrayList();
        public EditAssignment(int id, TabPage scr, int classItemId, string pageName)
        {
            //MessageBox.Show("start");
            screen = scr;
            InitializeComponent();
            classId = id;
            cn = new SqlConnection(DB.connStr);
            this.classItemId = classItemId;
            updatePanel();
            this.pageName = pageName;
            //MessageBox.Show("done");
        }


        private void updatePanel()
        {
            panel1.Controls.Clear();
            cn.Open();
            cmd = new SqlCommand("EXEC getAssignment @classItemId", cn);
            cmd.Parameters.AddWithValue("@classItemId", classItemId);
            reader = cmd.ExecuteReader();
            reader.Read();
            textBox2.Text = reader["title"].ToString();
            richTextBox2.Text = reader["instructions"].ToString();
            richTextBox2.Text = reader["instructions"].ToString();
            dateTimePicker1.Value = DateTime.Parse(reader["dueDateTime"].ToString());
            numericUpDown1.Value = int.Parse(reader["points"].ToString());
            reader.Close();

            int internalY = 2;

            cmd = new SqlCommand("SELECT * FROM AttachmentFiles WHERE classItemId = @classItemId", cn);
            cmd.Parameters.AddWithValue("@classItemId", classItemId);
            reader = cmd.ExecuteReader();
            for (int i = 0; reader.Read(); i++)
            {
                Button btn = new Button();
                btn.Text = "File";
                btn.Location = new Point(5, internalY);
                btn.Size = new Size(120, 20);
                internalY += 22;
                string n = reader["fileName"].ToString();
                btn.Click += (s, e) => {
                    PDF view = new PDF(n);
                    view.Show();
                };
                panel1.Controls.Add(btn);
            }
            reader.Close();
            cn.Close();
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

        private void button7_Click(object sender, EventArgs e)
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

        private void button8_Click(object sender, EventArgs e)
        {
            if (textBox2.Text == "" || richTextBox2.Text == "" || numericUpDown1.Value < 0)
            {
                MessageBox.Show("Enter the fields correctly", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                SqlConnection c = new SqlConnection(DB.connStr);
                c.Open();
                SqlCommand cm = new SqlCommand("EXEC updateAssignment  @classItemId, @points, @title, @instructions, @dueDateTime", c);
                cm.Parameters.AddWithValue("@title", textBox2.Text);
                cm.Parameters.AddWithValue("@classItemId", classItemId);
                cm.Parameters.AddWithValue("@instructions", richTextBox2.Text);
                cm.Parameters.AddWithValue("@dueDateTime", dateTimePicker1.Value);
                cm.Parameters.AddWithValue("@points", numericUpDown1.Value);
                if (cm.ExecuteNonQuery() <= 0) { MessageBox.Show("Unable to Update Announcement", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }

                for (int i = 0; i < filename.Count; i++)
                {
                    cm = new SqlCommand("EXEC insertFile @fileName, @classItemId", c);
                    cm.Parameters.AddWithValue("@fileName", filename[i].ToString());
                    cm.Parameters.AddWithValue("@classItemId", classItemId);
                    if (cm.ExecuteNonQuery() <= 0)
                    {
                        MessageBox.Show("Fail");
                    }
                }
                c.Close();
                button7_Click(sender, e);
            }
        }
    }
}
