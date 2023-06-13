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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace Neon_Classroom
{
    public partial class CreateClass : Form
    {
        SqlConnection cn = new SqlConnection();
        SqlCommand cmd = new SqlCommand();
        public CreateClass()
        {
            InitializeComponent();
            cn = new SqlConnection(DB.connStr);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string cName = textBox1.Text;
            string sec = textBox2.Text;
            string subject = textBox3.Text;
            string room = textBox4.Text;

            if (cName == "" || sec == "" || subject == "" || room == "")
            {
                MessageBox.Show("Enter All Fields", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                cn.Open();
                cmd = new SqlCommand("EXEC CreateClass @subject, @className, @room, @section, @userId", cn);
                cmd.Parameters.AddWithValue("@subject", subject);
                cmd.Parameters.AddWithValue("@className", cName);
                cmd.Parameters.AddWithValue("@room", room);
                cmd.Parameters.AddWithValue("@section", sec);
                cmd.Parameters.AddWithValue("@userId", DB.userId);
                if (cmd.ExecuteNonQuery() > 0)
                {
                    MessageBox.Show("Class Created", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    button2_Click(sender, e);
                }
                else
                {
                    MessageBox.Show("Class Not Created. Please Try Again", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                cn.Close();
            }
        }
    }
}
