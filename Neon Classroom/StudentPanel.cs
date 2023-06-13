using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Neon_Classroom
{
    public partial class StudentPanel : Form
    {
        public static SplitContainer SplitContainer;
        public StudentPanel()
        {
            InitializeComponent();
            Classess c = new Classess(splitContainer2);
            c.TopLevel = false;
            c.AutoScroll = true;
            c.Dock = DockStyle.Fill;
            splitContainer2.Panel2.Controls.Add(c);
            c.FormBorderStyle = FormBorderStyle.None;
            c.Show();
            SplitContainer = splitContainer2;
        }

        
        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {

        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            logOutToolStripMenuItem_Click(sender, e);
        }

        private void TeacherPanel_Load(object sender, EventArgs e)
        {

        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            _ = (panel1.Visible == false ? panel1.Visible = true : panel1.Visible = false);
            splitContainer2.Panel2.Controls.Add(panel1);
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            splitContainer2.Panel2.Controls.Clear();
            JoinClass c = new JoinClass();
            c.TopLevel = false;
            c.AutoScroll = true;
            c.Dock = DockStyle.Fill;
            splitContainer2.Panel2.Controls.Add(c);
            c.FormBorderStyle = FormBorderStyle.None;
            c.Show();
        }

        private void classessToolStripMenuItem_Click(object sender, EventArgs e)
        {
            splitContainer2.Panel2.Controls.Clear();
            Classess c = new Classess(splitContainer2);
            c.TopLevel = false;
            c.AutoScroll = true;
            c.Dock = DockStyle.Fill;
            splitContainer2.Panel2.Controls.Add(c);
            c.FormBorderStyle = FormBorderStyle.None;
            c.Show();
        }

        private void archivedClassToolStripMenuItem_Click(object sender, EventArgs e)
        {
            splitContainer2.Panel2.Controls.Clear();
            ArchievedClassess c = new ArchievedClassess(splitContainer2);
            c.TopLevel = false;
            c.AutoScroll = true;
            c.Dock = DockStyle.Fill;
            splitContainer2.Panel2.Controls.Add(c);
            c.FormBorderStyle = FormBorderStyle.None;
            c.Show();
        }

        private void logOutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Login tp = new Login();
            this.Hide();
            tp.ShowDialog();
            this.Close();
        }

        private void profileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            splitContainer2.Panel2.Controls.Clear();
            Profile c = new Profile();
            c.TopLevel = false;
            c.AutoScroll = true;
            c.Dock = DockStyle.Fill;
            splitContainer2.Panel2.Controls.Add(c);
            c.FormBorderStyle = FormBorderStyle.None;
            c.Show();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            profileToolStripMenuItem_Click(sender, e);
        }
    }
}
