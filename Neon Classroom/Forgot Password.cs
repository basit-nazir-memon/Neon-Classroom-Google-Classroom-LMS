using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Neon_Classroom
{
    public partial class ForgotPassword: Form
    {
        SqlConnection cn = new SqlConnection();
        SqlCommand cmd = new SqlCommand();
        SqlDataReader reader;
        public ForgotPassword()
        {
            InitializeComponent();
            cn = new SqlConnection(DB.connStr);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Login signUp = new Login();
            this.Hide();
            signUp.ShowDialog();
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            cn.Open();
            cmd = new SqlCommand("SELECT * FROM [User] u INNER JOIN UserInfo ui on u.id = ui.userId WHERE (Username = @usernameOrEmail OR email = @usernameOrEmail) AND dob = @dob;", cn);
            cmd.Parameters.AddWithValue("@usernameOrEmail", textBox1.Text);
            cmd.Parameters.AddWithValue("@dob", dateTimePicker1.Value.Date);
            reader = cmd.ExecuteReader();
            reader.Read();
            if (reader.HasRows)
            {
                if (bool.Parse(reader["isActive"].ToString()) == false)
                {
                    MessageBox.Show("Account is inactive, Unable to Change Password", "Inactive Account", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    label4.Visible = true;
                    label5.Visible = true;
                    label5.Text = reader["password"].ToString();
                    button1.Visible = false;
                    label1.Visible = false;
                    button2.Size = button1.Size;
                    button2.Location = button1.Location;
                }
                textBox1.Clear();
                
            }
            else
            {
                MessageBox.Show("Wrong Credentials", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            reader.Close();
            cn.Close();
        }

        private void getOtp_Click(object sender, EventArgs e)
        {
            
        }
    }
}
