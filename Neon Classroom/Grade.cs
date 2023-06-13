using System;
using System.Collections;
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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;
using Button = System.Windows.Forms.Button;

namespace Neon_Classroom
{
    public partial class Grade : Form
    {
        SqlConnection cn = new SqlConnection();
        SqlCommand cmd = new SqlCommand();
        SqlDataReader reader;

        int classId;
        int currYPos = 0;
        TabPage screen;

        public Grade(int id, TabPage scr)
        {
            screen = scr;
            InitializeComponent();
            classId = id;
            cn = new SqlConnection(DB.connStr);
            classWorkPanel.Location = new Point(5, 5);

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
                Panel assignmentInfo = new Panel();
                assignmentInfo.Size = new Size(532, 40);
                assignmentInfo.BorderStyle = BorderStyle.FixedSingle;
                assignmentInfo.Location = new Point(5, currYPos);

                currYPos += 45;

                Label name = new Label();
                name.Text = reader["title"].ToString();
                name.Font = new Font("Microsoft Sans Serif", 12, FontStyle.Bold);
                name.Location = new Point(10, 11);
                name.Size = new Size(200, 16);

                assignmentInfo.Controls.Add(name);

                int maxMarks = int.Parse(reader["points"].ToString());

                int assignmentId = int.Parse(reader["id"].ToString());
                assignmentInfo.Click += (s, e) =>
                {
                    ManageGrade mg = new ManageGrade(classId, screen, assignmentId, name.Text, maxMarks);
                    mg.TopLevel = false;
                    mg.AutoScroll = true;
                    mg.Dock = DockStyle.Fill;
                    screen.Controls.Add(mg);
                    mg.FormBorderStyle = FormBorderStyle.None;
                    mg.Show();
                    this.Close();
                };
                classWorkPanel.Controls.Add(assignmentInfo);
            }

            reader.Close();
            cn.Close();
        }
    }
}
