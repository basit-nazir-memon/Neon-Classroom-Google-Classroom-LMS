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
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;
using Button = System.Windows.Forms.Button;
using Label = System.Windows.Forms.Label;

namespace Neon_Classroom
{
    public partial class Comments : Form
    {
        SqlConnection cn = new SqlConnection();
        SqlCommand cmd = new SqlCommand();
        SqlDataReader reader;

        int classId;
        int classItemId = 0;
        TabPage screen;

        public Comments(int id, TabPage scr, int classItemId)
        {
            screen = scr;
            InitializeComponent();
            classId = id;
            cn = new SqlConnection(DB.connStr);
            this.classItemId = classItemId;
            updatePanel();
        }


        private void updatePanel()
        {
            classWorkPanel.Controls.Clear();
            int internalY = 5;
            cn.Open();

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

                int cmtId = int.Parse(reader["userId"].ToString());

                if (DB.role == "Teacher" || DB.userId == cmtId)
                {


                    Panel moreOptions = new Panel();
                    moreOptions.Size = new Size(70, 25);
                    moreOptions.BorderStyle = BorderStyle.FixedSingle;
                    moreOptions.Location = new Point(classWorkPanel.Width - 85, internalY + 15);
                    int cid = int.Parse(reader["classItemId"].ToString());
                    string comments = reader["comments"].ToString();

                    Button del = new Button();
                    del.Text = "Delete";
                    del.Location = new Point(0, 0);
                    del.Size = new Size(69, 23);
                    del.Click += (s, e) =>
                    {
                        DialogResult result = MessageBox.Show("Are you sure you want to delete this comment?", "Delete Comment", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result == DialogResult.Yes)
                        {
                            //MessageBox.Show(d.ToString() + "    " + cmtId.ToString());
                            SqlConnection c = new SqlConnection(DB.connStr);
                            c.Open();
                            SqlCommand cm = new SqlCommand("DELETE FROM Comment WHERE userId = @userId AND classItemId = @d AND comments = @comments", c);
                            cm.Parameters.AddWithValue("@userId", cmtId);
                            cm.Parameters.AddWithValue("@d", cid);
                            cm.Parameters.AddWithValue("@comments", comments);
                            if (cm.ExecuteNonQuery() <= 0) { MessageBox.Show("Unable to Delete Comment", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                            c.Close();

                            updatePanel();
                        }
                    };

                    moreOptions.Controls.Add(del);

                    moreOptions.Visible = false;

                    PictureBox pBox = new PictureBox();
                    pBox.Image = Properties.Resources.more;
                    pBox.Size = new Size(7, 20);
                    pBox.Location = new Point(classWorkPanel.Width - 10, internalY);
                    pBox.SizeMode = PictureBoxSizeMode.StretchImage;
                    pBox.Click += (s, e) =>
                    {
                        _ = (moreOptions.Visible) ? (moreOptions.Visible = false) : (moreOptions.Visible = true);
                    };



                    classWorkPanel.Controls.Add(moreOptions);
                    classWorkPanel.Controls.Add(pBox);
                }
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
            classWorkPanel.Size = new Size(classWorkPanel.Width, internalY + 5);
            reader.Close();

            cn.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Stream s = new Stream(classId, screen);
            s.TopLevel = false;
            s.AutoScroll = true;
            screen.Controls.Add(s);
            s.FormBorderStyle = FormBorderStyle.None;
            s.Show();
            this.Close();
        }
    }
}
