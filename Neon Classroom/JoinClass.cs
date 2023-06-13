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
    public partial class JoinClass : Form
    {
        SqlConnection cn = new SqlConnection();
        SqlDataReader reader;
        SqlCommand cmd = new SqlCommand();
        public JoinClass()
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
            string code = textBox1.Text;

            if (code == "")
            {
                MessageBox.Show("Enter the Code to Join", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                cn.Open();
                cmd = new SqlCommand("SELECT *  FROM Class WHERE code = @classCode", cn);
                cmd.Parameters.AddWithValue("@classCode", code);
                reader = cmd.ExecuteReader();
                reader.Read();
                if (reader.HasRows)
                {
                    reader.Close();
                    cmd = new SqlCommand("EXEC joinClass @classCode, @userId", cn);
                    cmd.Parameters.AddWithValue("@classCode", code);
                    cmd.Parameters.AddWithValue("@userId", DB.userId);
                    if (cmd.ExecuteNonQuery() > 0)
                    {
                        MessageBox.Show("Class Joined", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        button2_Click(sender, e);
                    }
                    else
                    {
                        MessageBox.Show("Class Not Joined. Please Try Again", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    cn.Close();
                }
                else
                {
                    MessageBox.Show("InValid Class Code", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                reader.Close();
                cn.Close();
            }
        }
    }
}
