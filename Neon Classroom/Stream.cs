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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace Neon_Classroom
{
    public partial class Stream : Form
    {
        SqlConnection cn = new SqlConnection();
        SqlCommand cmd = new SqlCommand();
        SqlDataReader reader;

        int classId;
        string classCode, className, subject;

        int currYPos = 0;

        static bool isAnnouncedTabOpen = false;

        TabPage screen;

        public Stream(int id, TabPage scr)
        {
            screen = scr;
            InitializeComponent();
            classId = id;
            cn = new SqlConnection(DB.connStr);
            cn.Open();
            cmd = new SqlCommand("EXEC getClass @classId;", cn);
            cmd.Parameters.AddWithValue("@classId", classId);
            reader = cmd.ExecuteReader();
            reader.Read();
            classCode = reader["code"].ToString();
            className = reader["className"].ToString();
            subject = reader["subject"].ToString();
            label1.Text = className;
            label3.Text = classCode;
            reader.Close();
            cn.Close();
            if (DB.role != "Teacher")
            {
                button1.Visible = false;
            }

            updatePanel();
        }


        private void updatePanel()
        {
            currYPos = 0;
            mainPanel.Controls.Clear();
            cn.Open();
            if (!isAnnouncedTabOpen)
            {
                Panel ann_Closed = new Panel();
                ann_Closed.Size = new Size(584, 40);
                ann_Closed.BorderStyle = BorderStyle.FixedSingle;
                ann_Closed.Location = new Point(0, currYPos);
                ann_Closed.Click += (s, e) => { isAnnouncedTabOpen = true; updatePanel(); };

                PictureBox picB = new PictureBox();
                picB.Image = Properties.Resources.P1;
                picB.Size = new Size(34, 34);
                picB.Location = new Point(3, 2);
                picB.SizeMode = PictureBoxSizeMode.StretchImage;

                Label ann = new Label();
                ann.Text = "Announce something to your class...";
                ann.Font = new Font("Microsoft Sans Serif", 8);
                ann.Location = new Point(52, 14);
                ann.AutoSize = true;
                ann.MaximumSize = new Size(520, 20);
                ann_Closed.Controls.Add(picB);
                ann_Closed.Controls.Add(ann);
                mainPanel.Controls.Add(ann_Closed);

                currYPos += (40 + 5);
            }
            else
            {
                ArrayList filePath = new ArrayList();

                Panel ann_Open = new Panel();
                ann_Open.Size = new Size(584, 156);
                ann_Open.BorderStyle = BorderStyle.FixedSingle;
                ann_Open.Location = new Point(0, currYPos);

                Label ann = new Label();
                ann.Text = "Announce something to your class...";
                ann.Font = new Font("Microsoft Sans Serif", 8);
                ann.Location = new Point(5, 5);
                ann.AutoSize = true;
                ann.MaximumSize = new Size(579, 15);

                RichTextBox txtBox = new RichTextBox();
                txtBox.Size = new Size(574, 100);
                txtBox.Location = new Point(5, 23);

                Button upload = new Button();
                upload.Text = "Upload Files";
                upload.Size = new Size(100, 25);
                upload.Location = new Point(5, 126);
                upload.Font = new Font("Microsoft Sans Serif", 10);
                upload.BackColor = Color.FromArgb(174, 137, 201);
                upload.ForeColor = Color.White;
                upload.Click += (s, e) =>
                {
                    OpenFileDialog filedailog = new OpenFileDialog();
                    string str;
                    filedailog.Filter = "pdf files (*.pdf) |*.pdf;";
                    filedailog.ShowDialog();
                    str = filedailog.FileName;
                    string Destination = "E:\\Files Upload\\";
                    var kl = str.Split('\\');
                    string fname = kl[kl.Length - 1];
                    if (str != "")
                    {
                        System.IO.File.Copy(str, Destination + fname, true);
                        filePath.Add(str);
                    }
                };

                Button sub = new Button();
                sub.Text = "Post";
                sub.Size = new Size(80, 25);
                sub.Location = new Point(499, 126);
                sub.Font = new Font("Microsoft Sans Serif", 10, FontStyle.Bold);
                sub.BackColor = Color.FromArgb(112, 79, 137);
                sub.ForeColor = Color.White;
                sub.Click += (s, e) =>
                {
                    if (txtBox.Text == "")
                    {
                        MessageBox.Show("Enter Some Announcement", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        SqlConnection conn = new SqlConnection(DB.connStr);
                        conn.Open();
                        SqlCommand cmdFile;
                        SqlDataReader r;
                        cmdFile = new SqlCommand("EXEC createAnnouncement @type, @annText, @userId, @classId", conn);
                        cmdFile.Parameters.AddWithValue("@type", "Announcement");
                        cmdFile.Parameters.AddWithValue("@annText", txtBox.Text);
                        cmdFile.Parameters.AddWithValue("@userId", DB.userId);
                        cmdFile.Parameters.AddWithValue("@classId", classId);
                        r = cmdFile.ExecuteReader();
                        r.Read();
                        int ClassItemID = (int)r[0];
                        r.Close();
                        for (int i = 0; i < filePath.Count; i++)
                        {
                            cmdFile = new SqlCommand("EXEC insertFile @fileName, @classItemId", conn);
                            cmdFile.Parameters.AddWithValue("@fileName", filePath[i].ToString());
                            cmdFile.Parameters.AddWithValue("@classItemId", ClassItemID);
                            if (cmdFile.ExecuteNonQuery() > 0)
                            {
                                MessageBox.Show("Success");
                            }
                            else
                            {
                                MessageBox.Show("Fail");
                            }
                        }
                        conn.Close();
                        isAnnouncedTabOpen = false; updatePanel();
                    }
                };


                Button cancel = new Button();
                cancel.Text = "Cancel";
                cancel.Size = new Size(80, 25);
                cancel.Location = new Point(414, 126);
                cancel.Font = new Font("Microsoft Sans Serif", 10);
                cancel.BackColor = Color.White;
                cancel.ForeColor = Color.FromArgb(112, 79, 137);
                cancel.Click += (s, e) => { isAnnouncedTabOpen = false; updatePanel(); };

                ann_Open.Controls.Add(upload);
                ann_Open.Controls.Add(sub);
                ann_Open.Controls.Add(cancel);
                ann_Open.Controls.Add(txtBox);
                ann_Open.Controls.Add(ann);

                mainPanel.Controls.Add(ann_Open);

                currYPos += (156 + 5);
            }



            cmd = new SqlCommand("EXEC getClassItems @classId;", cn);
            cmd.Parameters.AddWithValue("@classId", classId);
            reader = cmd.ExecuteReader();

            SqlDataReader reader1;
            SqlCommand cmd1;
            SqlDataReader reader2;
            SqlCommand cmd2;
            SqlDataReader reader3;
            SqlCommand cmd3;
            SqlConnection con = new SqlConnection(DB.connStr);
            SqlConnection con1 = new SqlConnection(DB.connStr);
            SqlConnection con2 = new SqlConnection(DB.connStr);
            SqlConnection con3 = new SqlConnection(DB.connStr);
            con.Open();
            con1.Open();
            con2.Open();
            con3.Open();
            int classItemId = 0;
            while (reader.Read())
            {
                classItemId = int.Parse(reader["id"].ToString());
                if (reader["type"].ToString() == "Announcement")
                {
                    cmd1 = new SqlCommand("SELECT * FROM Announcement WHERE classItemId = @classItemId", con);
                    cmd1.Parameters.AddWithValue("@classItemId", int.Parse(reader["id"].ToString()));
                    reader1 = cmd1.ExecuteReader();
                    reader1.Read();

                    cmd2 = new SqlCommand("EXEC getName @userId", con1);
                    cmd2.Parameters.AddWithValue("@userId", int.Parse(reader1["userId"].ToString()));
                    reader2 = cmd2.ExecuteReader();
                    reader2.Read();
                    string User = reader2[0].ToString();
                    reader2.Close();

                    int cid = classItemId;

                    Panel p = new Panel();
                    p.Size = new Size(585, 175);
                    p.BorderStyle = BorderStyle.FixedSingle;
                    p.Location = new Point(0, currYPos);

                    PictureBox pictureBox = new PictureBox();
                    pictureBox.Image = Properties.Resources.P1;
                    pictureBox.Size = new Size(35, 35);
                    pictureBox.Location = new Point(5, 7);
                    pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;

                    Label name = new Label();
                    name.Text = User;
                    name.Font = new Font("Microsoft Sans Serif", 10);
                    name.Location = new Point(52, 7);
                    name.Size = new Size(200, 15);

                    Label time = new Label();
                    time.Text = reader["publishTime"].ToString();
                    time.Font = new Font("Microsoft Sans Serif", 9);
                    time.Location = new Point(52, 26);
                    time.Size = new Size(200, 15);

                    if (DB.role == "Teacher" || DB.userId == int.Parse(reader1["userId"].ToString()))
                    {
                        Panel moreOptions = new Panel();
                        moreOptions.Size = new Size(70, 50);
                        moreOptions.BorderStyle = BorderStyle.FixedSingle;
                        moreOptions.Location = new Point(490, 27);

                        Button edit = new Button();
                        edit.Text = "Edit";
                        edit.Location = new Point(0, 0);
                        edit.Size = new Size(69, 23);
                        edit.Click += (s, e) =>
                        {
                            EditAnnouncement a = new EditAnnouncement(classId, screen, cid, "Stream");
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
                            DialogResult result = MessageBox.Show("Are you sure you want to delete this announcement?", "Delete Announcement", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                            if (result == DialogResult.Yes)
                            {
                                SqlConnection c = new SqlConnection(DB.connStr);
                                c.Open();
                                SqlCommand cm = new SqlCommand("DELETE FROM ClassItems WHERE id = @classItemId", c);
                                cm.Parameters.AddWithValue("@classItemId", cid);
                                if (cm.ExecuteNonQuery() <= 0) { MessageBox.Show("Unable to Delete Announcement", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                                c.Close();
                                updatePanel();
                            }
                        };

                        moreOptions.Controls.Add(edit);
                        moreOptions.Controls.Add(del);

                        moreOptions.Visible = false;

                        PictureBox pBox = new PictureBox();
                        pBox.Image = Properties.Resources.more;
                        pBox.Size = new Size(7, 20);
                        pBox.Location = new Point(567, 7);
                        pBox.SizeMode = PictureBoxSizeMode.StretchImage;
                        pBox.Click += (s, e) =>
                        {
                            _ = (moreOptions.Visible) ? (moreOptions.Visible = false) : (moreOptions.Visible = true);
                        };



                        p.Controls.Add(moreOptions);
                        p.Controls.Add(pBox);
                    }
                    Label post = new Label();
                    post.Text = reader1["announcementText"].ToString();
                    post.Font = new Font("Microsoft Sans Serif", 10);
                    post.Location = new Point(7, 50);
                    post.AutoSize = true;
                    post.MaximumSize = new Size(585, 80);

                    int internalY = 140;

                    cmd2 = new SqlCommand("SELECT * FROM AttachmentFiles WHERE classItemId = @classItemId", con2);
                    cmd2.Parameters.AddWithValue("@classItemId", int.Parse(reader["id"].ToString()));
                    reader2 = cmd2.ExecuteReader();

                    for (int i = 0; reader2.Read(); i++)
                    {
                        if (i == 0)
                        {
                            Panel sep = new Panel();
                            sep.BackColor = Color.Black;
                            sep.Size = new Size(575, 1);
                            sep.BorderStyle = BorderStyle.FixedSingle;
                            sep.Location = new Point(5, internalY);

                            internalY += 5;
                            p.Controls.Add(sep);
                        }
                        string n = reader2["fileName"].ToString();
                        Button btn = new Button();
                        btn.Text = "File";
                        btn.Location = new Point(5, internalY);
                        btn.Size = new Size(150, 25);
                        btn.Click += (s, e) => {
                            PDF view = new PDF(n);
                            view.Show();
                        };
                        internalY += 30;

                        p.Controls.Add(btn);
                    }
                    reader2.Close();



                    cmd2 = new SqlCommand("EXEC getCommentCount @classItemId", con2);
                    cmd2.Parameters.AddWithValue("@classItemId", int.Parse(reader["id"].ToString()));
                    reader2 = cmd2.ExecuteReader();
                    reader2.Read();
                    int count = (int)reader2[0];
                    reader2.Close();

                    if (count > 0)
                    {
                        Panel sep = new Panel();
                        sep.BackColor = Color.Black;
                        sep.Size = new Size(575, 1);
                        sep.BorderStyle = BorderStyle.FixedSingle;
                        sep.Location = new Point(5, internalY);

                        internalY += 5;

                        if (count > 1)
                        {
                            Button btn = new Button();
                            btn.Text = "View " + count.ToString() + " comments";
                            btn.Location = new Point(5, internalY);
                            btn.Size = new Size(150, 25);
                            internalY += 30;
                            btn.Click += (s, e) =>
                            {
                                Comments c = new Comments(classId, screen, cid);
                                c.TopLevel = false;
                                c.AutoScroll = true;
                                screen.Controls.Add(c);
                                c.FormBorderStyle = FormBorderStyle.None;
                                c.Show();
                                this.Close();
                            };
                            p.Controls.Add(btn);
                        }

                        cmd2 = new SqlCommand("EXEC getComments @classItemId", con2);
                        cmd2.Parameters.AddWithValue("@classItemId", int.Parse(reader["id"].ToString()));
                        reader2 = cmd2.ExecuteReader();
                        reader2.Read();

                        PictureBox pict = new PictureBox();
                        pict.Image = Properties.Resources.P1;
                        pict.Size = new Size(25, 25);
                        pict.Location = new Point(5, internalY);
                        pict.SizeMode = PictureBoxSizeMode.StretchImage;

                        Label person = new Label();
                        person.Text = reader2["name"].ToString() + "   " + reader2["time"].ToString();
                        person.Font = new Font("Microsoft Sans Serif", 9);
                        person.Location = new Point(35, internalY);
                        person.AutoSize = true;
                        person.MaximumSize = new Size(400, 30);

                        internalY += 15;

                        Label cmnt = new Label();
                        cmnt.Text = reader2["comments"].ToString();
                        cmnt.Font = new Font("Microsoft Sans Serif", 10);
                        cmnt.Location = new Point(35, internalY);
                        cmnt.AutoSize = true;
                        cmnt.MaximumSize = new Size(540, 80);
                        reader2.Close();

                        internalY += 30;
                        p.Controls.Add(sep);
                        p.Controls.Add(pict);
                        p.Controls.Add(person);
                        p.Controls.Add(cmnt);

                    }
                    p.Size = new Size(p.Width, p.Height + (internalY - 140));
                    currYPos += (internalY - 140);


                    Panel seperator = new Panel();
                    seperator.BackColor = Color.Black;
                    seperator.Size = new Size(575, 1);
                    seperator.BorderStyle = BorderStyle.FixedSingle;
                    seperator.Location = new Point(5, internalY);

                    internalY += 5;

                    PictureBox pic = new PictureBox();
                    pic.Image = Properties.Resources.P1;
                    pic.Size = new Size(25, 25);
                    pic.Location = new Point(7, internalY);
                    pic.SizeMode = PictureBoxSizeMode.StretchImage;

                    TextBox comment = new TextBox();
                    comment.Location = new Point(42, internalY);
                    comment.Size = new Size(473, 30);
                    comment.Font = new Font("Microsoft Sans Serif", 10);

                    Button pst = new Button();
                    pst.Text = "Post";
                    pst.Location = new Point(520, internalY);
                    pst.Size = new Size(60, 25);
                    pst.Click += (s, e) =>
                    {
                        if (comment.Text == "")
                        {
                            MessageBox.Show("Enter Some Comments to Post", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                        {
                            con3 = new SqlConnection(DB.connStr);
                            con3.Open();
                            cmd3 = new SqlCommand("INSERT INTO Comment (userId, classItemId, time, comments) VALUES (@userId, @classItemId, @time, @comments)", con3);
                            cmd3.Parameters.AddWithValue("@userId", DB.userId);
                            cmd3.Parameters.AddWithValue("@classItemId", cid);
                            cmd3.Parameters.AddWithValue("@time", DateTime.Now);
                            cmd3.Parameters.AddWithValue("@comments", comment.Text);
                            if (cmd3.ExecuteNonQuery() <= 0)
                            {
                                MessageBox.Show("Failed to Post Comment", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            con3.Close();
                        }
                    };

                    currYPos += 180;
                    p.Controls.Add(pictureBox);
                    p.Controls.Add(name);
                    p.Controls.Add(time);
                    p.Controls.Add(post);
                    p.Controls.Add(seperator);
                    p.Controls.Add(pic);
                    p.Controls.Add(comment);
                    p.Controls.Add(pst);


                    mainPanel.Controls.Add(p);

                    reader1.Close();
                    //    }
                    //    else if (reader["type"].ToString() == "Assignment")
                    //    {

                    //    }
                    //    else if (reader["type"].ToString() == "Material")
                    //    {

                    //    }
                    //}
                }
                else if (reader["type"].ToString() == "Assignment")
                {
                    cmd1 = new SqlCommand("SELECT * FROM Assignment WHERE classItemId = @classItemId", con);
                    cmd1.Parameters.AddWithValue("@classItemId", int.Parse(reader["id"].ToString()));
                    reader1 = cmd1.ExecuteReader();
                    reader1.Read();

                    int cid = classItemId;

                    Panel assignmentInfo = new Panel();
                    assignmentInfo.Size = new Size(585, 40);
                    assignmentInfo.BorderStyle = BorderStyle.FixedSingle;
                    assignmentInfo.Location = new Point(0, currYPos);
                    assignmentInfo.Click += (s, e) => {
                        Assignment a = new Assignment(classId, screen, cid, "Stream");
                        a.TopLevel = false;
                        a.AutoScroll = true;
                        screen.Controls.Add(a);
                        a.FormBorderStyle = FormBorderStyle.None;
                        a.Show();
                        this.Close();
                    };

                    currYPos += 45;

                    Label name = new Label();
                    name.Text = "Assignment Posted : " + reader1["title"].ToString();
                    name.Font = new Font("Microsoft Sans Serif", 11);
                    name.Location = new Point(10, 11);
                    name.Size = new Size(350, 20);

                    Label dateDue = new Label();
                    dateDue.Text = reader1["dueDateTime"].ToString();
                    dateDue.Font = new Font("Microsoft Sans Serif", 9);
                    dateDue.Location = new Point(420, 12);
                    dateDue.Size = new Size(200, 15);

                    if (DB.role == "Teacher")
                    {
                        Panel moreOptions = new Panel();
                        moreOptions.Size = new Size(70, 50);
                        moreOptions.BorderStyle = BorderStyle.FixedSingle;
                        moreOptions.Location = new Point(500, currYPos - 5);

                        Button edit = new Button();
                        edit.Text = "Edit";
                        edit.Location = new Point(0, 0);
                        edit.Size = new Size(69, 23);
                        edit.Click += (s, e) =>
                        {
                            EditAssignment a = new EditAssignment(classId, screen, cid, "Stream");
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
                        pictureBox.Location = new Point(565, 10);
                        pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
                        pictureBox.Click += (s, e) =>
                        {
                            _ = (moreOptions.Visible) ? (moreOptions.Visible = false) : (moreOptions.Visible = true);
                        };

                        assignmentInfo.Controls.Add(pictureBox);
                        mainPanel.Controls.Add(moreOptions);

                    }

                    assignmentInfo.Controls.Add(dateDue);
                    assignmentInfo.Controls.Add(name);

                    cmd2 = new SqlCommand("EXEC getCommentCount @classItemId", con2);
                    cmd2.Parameters.AddWithValue("@classItemId", int.Parse(reader["id"].ToString()));
                    reader2 = cmd2.ExecuteReader();
                    reader2.Read();
                    int count = (int)reader2[0];
                    reader2.Close();

                    int internalY = 40;

                    if (count > 0)
                    {
                        Panel sep = new Panel();
                        sep.BackColor = Color.Black;
                        sep.Size = new Size(575, 1);
                        sep.BorderStyle = BorderStyle.FixedSingle;
                        sep.Location = new Point(5, internalY);

                        internalY += 5;

                        if (count > 1)
                        {
                            Button btn = new Button();
                            btn.Text = "View " + count.ToString() + " comments";
                            btn.Location = new Point(5, internalY);
                            btn.Size = new Size(150, 25);
                            internalY += 30;
                            btn.Click += (s, e) =>
                            {
                                Comments c = new Comments(classId, screen, cid);
                                c.TopLevel = false;
                                c.AutoScroll = true;
                                screen.Controls.Add(c);
                                c.FormBorderStyle = FormBorderStyle.None;
                                c.Show();
                                this.Close();
                            };
                            assignmentInfo.Controls.Add(btn);
                        }

                        cmd2 = new SqlCommand("EXEC getComments @classItemId", con2);
                        cmd2.Parameters.AddWithValue("@classItemId", int.Parse(reader["id"].ToString()));
                        reader2 = cmd2.ExecuteReader();
                        reader2.Read();

                        PictureBox pict = new PictureBox();
                        pict.Image = Properties.Resources.P1;
                        pict.Size = new Size(25, 25);
                        pict.Location = new Point(5, internalY);
                        pict.SizeMode = PictureBoxSizeMode.StretchImage;

                        Label person = new Label();
                        person.Text = reader2["name"].ToString() + "   " + reader2["time"].ToString();
                        person.Font = new Font("Microsoft Sans Serif", 9);
                        person.Location = new Point(35, internalY);
                        person.AutoSize = true;
                        person.MaximumSize = new Size(400, 30);

                        internalY += 15;

                        Label cmnt = new Label();
                        cmnt.Text = reader2["comments"].ToString();
                        cmnt.Font = new Font("Microsoft Sans Serif", 10);
                        cmnt.Location = new Point(35, internalY);
                        cmnt.AutoSize = true;
                        cmnt.MaximumSize = new Size(540, 80);
                        reader2.Close();

                        internalY += 30;
                        assignmentInfo.Controls.Add(sep);
                        assignmentInfo.Controls.Add(pict);
                        assignmentInfo.Controls.Add(person);
                        assignmentInfo.Controls.Add(cmnt);

                        currYPos += (internalY - 40);
                        assignmentInfo.Size = new Size(585, internalY);
                    }

                    
                    mainPanel.Controls.Add(assignmentInfo);
                    reader1.Close();
                }
                else if (reader["type"].ToString() == "Material")
                {
                    cmd1 = new SqlCommand("SELECT * FROM Material WHERE classItemId = @classItemId", con);
                    cmd1.Parameters.AddWithValue("@classItemId", int.Parse(reader["id"].ToString()));
                    reader1 = cmd1.ExecuteReader();
                    reader1.Read();

                    int cid = classItemId;
                    Panel assignmentInfo = new Panel();
                    assignmentInfo.Size = new Size(585, 40);
                    assignmentInfo.BorderStyle = BorderStyle.FixedSingle;
                    assignmentInfo.Location = new Point(0, currYPos);
                    assignmentInfo.Click += (s, e) => {
                        Material a = new Material(classId, screen, cid, "Stream");
                        a.TopLevel = false;
                        a.AutoScroll = true;
                        screen.Controls.Add(a);
                        a.FormBorderStyle = FormBorderStyle.None;
                        a.Show();
                        this.Close();
                    };
                    currYPos += 45;

                    Label name = new Label();
                    name.Text = "Material Posted : " + reader1["title"].ToString();
                    name.Font = new Font("Microsoft Sans Serif", 11);
                    name.Location = new Point(10, 11);
                    name.Size = new Size(350, 20);

                    if (DB.role == "Teacher")
                    {

                        Panel moreOptions = new Panel();
                        moreOptions.Size = new Size(70, 50);
                        moreOptions.BorderStyle = BorderStyle.FixedSingle;
                        moreOptions.Location = new Point(500, currYPos - 5);

                        Button edit = new Button();
                        edit.Text = "Edit";
                        edit.Location = new Point(0, 0);
                        edit.Size = new Size(69, 23);
                        edit.Click += (s, e) =>
                        {
                            EditMaterial a = new EditMaterial(classId, screen, cid, "Stream");
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
                        pictureBox.Location = new Point(565, 10);
                        pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
                        pictureBox.Click += (s, e) =>
                        {
                            _ = (moreOptions.Visible) ? (moreOptions.Visible = false) : (moreOptions.Visible = true);
                        };

                        assignmentInfo.Controls.Add(pictureBox);


                        mainPanel.Controls.Add(moreOptions);
                    }
                    assignmentInfo.Controls.Add(name);

                    cmd2 = new SqlCommand("EXEC getCommentCount @classItemId", con2);
                    cmd2.Parameters.AddWithValue("@classItemId", int.Parse(reader["id"].ToString()));
                    reader2 = cmd2.ExecuteReader();
                    reader2.Read();
                    int count = (int)reader2[0];
                    reader2.Close();
                    int internalY = 40;

                    if (count > 0)
                    {
                        Panel sep = new Panel();
                        sep.BackColor = Color.Black;
                        sep.Size = new Size(575, 1);
                        sep.BorderStyle = BorderStyle.FixedSingle;
                        sep.Location = new Point(5, internalY);

                        internalY += 5;
                        
                        if (count > 1)
                        {
                            Button btn = new Button();
                            btn.Text = "View " + count.ToString() + " comments";
                            btn.Location = new Point(5, internalY);
                            btn.Size = new Size(150, 25);
                            internalY += 30;
                            btn.Click += (s, e) =>
                            {
                                Comments c = new Comments(classId, screen, cid);
                                c.TopLevel = false;
                                c.AutoScroll = true;
                                screen.Controls.Add(c);
                                c.FormBorderStyle = FormBorderStyle.None;
                                c.Show();
                                this.Close();
                            };
                            assignmentInfo.Controls.Add(btn);
                        }

                        cmd2 = new SqlCommand("EXEC getComments @classItemId", con2);
                        cmd2.Parameters.AddWithValue("@classItemId", int.Parse(reader["id"].ToString()));
                        reader2 = cmd2.ExecuteReader();
                        reader2.Read();

                        PictureBox pict = new PictureBox();
                        pict.Image = Properties.Resources.P1;
                        pict.Size = new Size(25, 25);
                        pict.Location = new Point(5, internalY);
                        pict.SizeMode = PictureBoxSizeMode.StretchImage;

                        Label person = new Label();
                        person.Text = reader2["name"].ToString() + "   " + reader2["time"].ToString();
                        person.Font = new Font("Microsoft Sans Serif", 9);
                        person.Location = new Point(35, internalY);
                        person.AutoSize = true;
                        person.MaximumSize = new Size(400, 30);

                        internalY += 15;

                        Label cmnt = new Label();
                        cmnt.Text = reader2["comments"].ToString();
                        cmnt.Font = new Font("Microsoft Sans Serif", 10);
                        cmnt.Location = new Point(35, internalY);
                        cmnt.AutoSize = true;
                        cmnt.MaximumSize = new Size(540, 80);
                        reader2.Close();

                        internalY += 30;
                        assignmentInfo.Controls.Add(sep);
                        assignmentInfo.Controls.Add(pict);
                        assignmentInfo.Controls.Add(person);
                        assignmentInfo.Controls.Add(cmnt);

                        currYPos += (internalY - 40);
                        assignmentInfo.Size = new Size(585, internalY);
                    }
                    mainPanel.Controls.Add(assignmentInfo);
                    reader1.Close();
                }
            }        
            con3.Close();
            con2.Close();
            con1.Close();
            con.Close();
            // finalizing DO NOT TOUCH
            mainPanel.Size = new Size(mainPanel.Size.Width, currYPos + 2);
            this.Controls.Add(mainPanel);
            reader.Close();
            cn.Close();
        }
    }
}
