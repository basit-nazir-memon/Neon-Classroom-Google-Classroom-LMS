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

namespace Neon_Classroom
{
    public partial class SignUp : Form
    {
        SqlConnection cn = new SqlConnection();
        SqlCommand cmd = new SqlCommand();
        SqlDataReader reader;

        public SignUp()
        {
            InitializeComponent();
            cn = new SqlConnection(DB.connStr);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Login form = new Login();
            this.Hide();
            form.ShowDialog();
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            cn.Open();
            string firstName, lastName, email, pass, c_pass, username, dob, gender, role;
            bool tAndC;
            firstName = textBox1.Text;
            lastName = textBox3.Text;
            email = textBox5.Text;
            c_pass = textBox2.Text;
            pass = textBox6.Text;
            username = textBox1.Text;
            dob = dateTimePicker1.Value.Date.ToString("yyyy-mm-dd");
            gender = (radioButton1.Checked ? "Male" : radioButton2.Checked ? "Female" : "Other");
            role = comboBox1.Text;
            tAndC = checkBox1.Checked;

            cmd = new SqlCommand("select * from [User] where username = '" + username + "'", cn);
            reader = cmd.ExecuteReader();
            reader.Read();

            if (firstName == "" || lastName == "" || email == "" || c_pass == "" || pass == "" || username == "" || dob == "" || gender == "" || role == "")
            {
                MessageBox.Show("Please Fill all Fields", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (c_pass != pass)
            {
                MessageBox.Show("Password & Confirms Password must Match", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }else if (reader.HasRows)
            {
                reader.Close();
                MessageBox.Show("Username already exixts. Try using other username", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                MessageBox.Show("");
                reader.Close();
                cmd = new SqlCommand("EXEC RegisterUser @username, @password, @email, @role, @tAndC, @firstName, @lastName, @dob, @gender", cn);
                cmd.Parameters.AddWithValue("@username", username);
                cmd.Parameters.AddWithValue("@password", pass);
                cmd.Parameters.AddWithValue("@email", email);
                cmd.Parameters.AddWithValue("@role", role);
                cmd.Parameters.AddWithValue("@tAndC", tAndC);
                cmd.Parameters.AddWithValue("@firstName", firstName);
                cmd.Parameters.AddWithValue("@lastName", lastName);
                cmd.Parameters.AddWithValue("@dob", dateTimePicker1.Value.Date);
                cmd.Parameters.AddWithValue("@gender", gender);
                if (cmd.ExecuteNonQuery() > 0)
                {
                    MessageBox.Show("User Added Successfully", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    button2_Click(sender, e);

                }
                else
                {
                    MessageBox.Show("An Error Occurred. Please Try Again", "Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            reader.Close();
            cn.Close();

        }
    }
}
