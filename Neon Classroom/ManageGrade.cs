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
    public partial class ManageGrade : Form
    {
        SqlConnection cn = new SqlConnection();
        SqlCommand cmd = new SqlCommand();
        SqlDataReader reader;

        int classId;
        int currYPos = 0;
        int assignmentId; 
        string name;
        int maxMarks;
        TabPage screen;

        ArrayList marks = new ArrayList();

        public ManageGrade(int id, TabPage scr, int assignmentId, string name, int maxMarks)
        {
            screen = scr;
            InitializeComponent();
            classId = id;
            cn = new SqlConnection(DB.connStr);
            classWorkPanel.Location = new Point(5, 5);
            this.assignmentId = assignmentId;
            this.name = name;
            this.maxMarks = maxMarks;
            label1.Text = name;
            updatePanel();
        }


        private void updatePanel()
        {
            classWorkPanel.Controls.Clear();
            marks.Clear();
            currYPos = 25;
            cn.Open();
            Label material = new Label();
            material.Text = "Students";
            material.Font = new Font("Microsoft Sans Serif", 14);
            material.Location = new Point(5, currYPos);
            material.AutoSize = true;
            material.MaximumSize = new Size(520, 25);
            classWorkPanel.Controls.Add(material);

            Label material1 = new Label();
            material1.Text = "Points";
            material1.Font = new Font("Microsoft Sans Serif", 14);
            material1.Location = new Point(265, currYPos);
            material1.AutoSize = true;
            material1.MaximumSize = new Size(520, 25);
            classWorkPanel.Controls.Add(material1);

            currYPos += 25;

            Panel seper = new Panel();
            seper.BackColor = Color.Black;
            seper.Size = new Size(532, 1);
            seper.BorderStyle = BorderStyle.FixedSingle;
            seper.Location = new Point(5, currYPos);
            currYPos += 5;
            classWorkPanel.Controls.Add(seper);



            cmd = new SqlCommand("EXEC getGrades @assignmentId, @classId", cn);
            cmd.Parameters.AddWithValue("@assignmentId", assignmentId);
            cmd.Parameters.AddWithValue("@classId", classId);
            reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Label name = new Label();
                name.Text = reader["name"].ToString();
                name.Font = new Font("Microsoft Sans Serif", 12, FontStyle.Bold);
                name.Location = new Point(10, currYPos);
                name.Size = new Size(200, 16);

                int subId = int.Parse(reader["id"].ToString());

                if (bool.Parse(reader["isTurnedIn"].ToString()))
                {
                    name.Click += (s, e) => {
                        ViewAssignment mg = new ViewAssignment(classId, screen, assignmentId, this.name, maxMarks, subId);
                        mg.TopLevel = false;
                        mg.AutoScroll = true;
                        mg.Dock = DockStyle.Fill;
                        screen.Controls.Add(mg);
                        mg.FormBorderStyle = FormBorderStyle.None;
                        mg.Show();
                        this.Close();
                    };
                    NumericUpDown mrks = new NumericUpDown();
                    mrks.Location = new Point(265, currYPos);
                    mrks.Size = new Size(100, 15);
                    if (reader["obtainedMarks"].ToString() == "")
                    {
                        mrks.Value = 0;
                    }
                    else
                    {
                        mrks.Value = int.Parse(reader["obtainedMarks"].ToString());
                    }
                    mrks.Maximum = maxMarks;
                    classWorkPanel.Controls.Add(mrks);

                    marks.Add(subId);
                    marks.Add(mrks);
                }
                else
                {
                    Label status = new Label();
                    status.Text = "Missing";
                    status.Font = new Font("Microsoft Sans Serif", 11, FontStyle.Bold);
                    status.Location = new Point(265, currYPos);
                    status.Size = new Size(200, 16);
                    classWorkPanel.Controls.Add(status);
                }
                classWorkPanel.Controls.Add(name);



                currYPos += 20;

                
            }
            
            reader.Close();
            cn.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Grade mg = new Grade(classId, screen);
            mg.TopLevel = false;
            mg.AutoScroll = true;
            mg.Dock = DockStyle.Fill;
            screen.Controls.Add(mg);
            mg.FormBorderStyle = FormBorderStyle.None;
            mg.Show();
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int num = marks.Count / 2;
            SqlConnection c = new SqlConnection(DB.connStr);
            c.Open();
            for (int i = 0; i<num; i++)
            {
                SqlCommand cm = new SqlCommand("UPDATE Submission SET obtainedMarks = @obtainedMarks WHERE id = @subId", c);
                cm.Parameters.AddWithValue("@obtainedMarks", ((NumericUpDown)marks[i*2 + 1]).Value);
                cm.Parameters.AddWithValue("@subId", marks[i*2]);
                if (cm.ExecuteNonQuery() <= 0) { MessageBox.Show("Unable to Update Submission", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            }
            c.Close();
            MessageBox.Show("Marks Updated Successfully", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            updatePanel();
        }
    }
}
