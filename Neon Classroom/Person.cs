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
    public partial class Person : Form
    {
        SqlConnection cn = new SqlConnection();
        SqlCommand cmd = new SqlCommand();
        SqlDataReader reader;

        int classId;
        int currYPos = 0;
        SplitContainer screen;

        public Person(int id, SplitContainer scr)
        {
            screen = scr;
            InitializeComponent();
            classId = id;
            cn = new SqlConnection(DB.connStr);
            updatePanel();
        }


        private void updatePanel()
        {
            this.Controls.Clear();
            currYPos = 5;
            cn.Open();

            Label ann = new Label();
            ann.Text = "Teacher";
            ann.Font = new Font("Microsoft Sans Serif", 18);
            ann.Location = new Point(2, currYPos);
            ann.AutoSize = true;
            ann.MaximumSize = new Size(520, 25);
            this.Controls.Add(ann);

            currYPos += 25;
            
            Panel sep = new Panel();
            sep.BackColor = Color.Black;
            sep.Size = new Size(532, 1);
            sep.BorderStyle = BorderStyle.FixedSingle;
            sep.Location = new Point(5, currYPos);
            currYPos += 5;
            this.Controls.Add(sep);


            cmd = new SqlCommand("EXEC getTeacher @classId", cn);
            cmd.Parameters.AddWithValue("@classId", classId);
            reader = cmd.ExecuteReader();
            reader.Read();

            Panel tech = new Panel();
            tech.Size = new Size(532, 40);
            tech.BorderStyle = BorderStyle.FixedSingle;
            tech.Location = new Point(5, currYPos);

            currYPos += 45;

            PictureBox pic = new PictureBox();
            pic.Image = Properties.Resources.P1;
            pic.Size = new Size(30, 30);
            pic.Location = new Point(7, 5);
            pic.SizeMode = PictureBoxSizeMode.StretchImage;

            Label nam = new Label();
            nam.Text = reader["name"].ToString();
            nam.Font = new Font("Microsoft Sans Serif", 12, FontStyle.Bold);
            nam.Location = new Point(45, 11);
            nam.Size = new Size(200, 16);

            tech.Controls.Add(pic);
            tech.Controls.Add(nam);
            this.Controls.Add(tech);

            reader.Close();

            currYPos += 5;

            Label material = new Label();
            material.Text = "Students";
            material.Font = new Font("Microsoft Sans Serif", 18);
            material.Location = new Point(2, currYPos);
            material.AutoSize = true;
            material.MaximumSize = new Size(520, 25);
            this.Controls.Add(material);

            currYPos += 25;

            Panel seper = new Panel();
            seper.BackColor = Color.Black;
            seper.Size = new Size(532, 1);
            seper.BorderStyle = BorderStyle.FixedSingle;
            seper.Location = new Point(5, currYPos);
            currYPos += 5;
            this.Controls.Add(seper);

            cmd = new SqlCommand("EXEC getStudents @classId", cn);
            cmd.Parameters.AddWithValue("@classId", classId);
            reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Panel assignmentInfo = new Panel();
                assignmentInfo.Size = new Size(532, 40);
                assignmentInfo.BorderStyle = BorderStyle.FixedSingle;
                assignmentInfo.Location = new Point(5, currYPos);

                currYPos += 45;

                PictureBox pict = new PictureBox();
                pict.Image = Properties.Resources.P1;
                pict.Size = new Size(30, 30);
                pict.Location = new Point(7, 5);
                pict.SizeMode = PictureBoxSizeMode.StretchImage;

                Label name = new Label();
                name.Text = reader["name"].ToString();
                name.Font = new Font("Microsoft Sans Serif", 12, FontStyle.Bold);
                name.Location = new Point(45, 11);
                name.Size = new Size(200, 16);

                assignmentInfo.Controls.Add(pict);
                assignmentInfo.Controls.Add(name);

                this.Controls.Add(assignmentInfo);
            }

            reader.Close();
            cn.Close();
        }
    }
}
