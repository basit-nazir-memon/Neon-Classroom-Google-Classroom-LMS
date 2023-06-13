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
    public partial class Login : Form
    {
        SqlConnection cn = new SqlConnection();
        SqlCommand cmd = new SqlCommand();
        SqlDataReader reader;
        public Login()
        {
            InitializeComponent();
            cn = new SqlConnection(DB.connStr);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SignUp signUp = new SignUp();
            this.Hide();
            signUp.ShowDialog();
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            cn.Open();
            cmd = new SqlCommand("EXEC SelectUserByCredentials @usernameOrEmail, @password;", cn);
            cmd.Parameters.AddWithValue("@usernameOrEmail", textBox1.Text);
            cmd.Parameters.AddWithValue("@password", textBox2.Text);
            reader = cmd.ExecuteReader();
            reader.Read();
            if (reader.HasRows)
            {
                if (bool.Parse(reader["isActive"].ToString()) == false)
                {
                    MessageBox.Show("Account is inactive, Unable to Login", "Inactive Account", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else if (reader["role"].ToString() == "Student")
                {
                    DB.userId = int.Parse(reader["id"].ToString());
                    DB.role = "Student";
                    reader.Close();
                    cmd = new SqlCommand("EXEC getName @userId;", cn);
                    cmd.Parameters.AddWithValue("@userId", DB.userId);
                    reader = cmd.ExecuteReader();
                    reader.Read();
                    DB.userName = reader["name"].ToString();
                    reader.Close();
                    cmd = new SqlCommand("INSERT INTO LoginHistory(userId, dateTime) VALUES (@userId, @dateTime)", cn);
                    cmd.Parameters.AddWithValue("@userId", DB.userId);
                    cmd.Parameters.AddWithValue("@dateTime", DateTime.Now);
                    cmd.ExecuteNonQuery();
                    StudentPanel tp = new StudentPanel();
                    this.Hide();
                    tp.ShowDialog();
                    this.Close();
                }
                else
                {
                    DB.userId = int.Parse(reader["id"].ToString());
                    DB.role = "Teacher";
                    reader.Close();
                    cmd = new SqlCommand("EXEC getName @userId;", cn);
                    cmd.Parameters.AddWithValue("@userId", DB.userId);
                    reader = cmd.ExecuteReader();
                    reader.Read();
                    DB.userName = reader["name"].ToString();
                    reader.Close();
                    cmd = new SqlCommand("INSERT INTO LoginHistory(userId, dateTime) VALUES (@userId, @dateTime)", cn);
                    cmd.Parameters.AddWithValue("@userId", DB.userId);
                    cmd.Parameters.AddWithValue("@dateTime", DateTime.Now);
                    cmd.ExecuteNonQuery();
                    TeacherPanel tp = new TeacherPanel();
                    this.Hide();
                    tp.ShowDialog();
                    this.Close();
                }
                textBox1.Clear();
                textBox2.Clear();
            }
            else
            {
                MessageBox.Show("Enter Credentials", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            reader.Close();
            cn.Close();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ForgotPassword signUp = new ForgotPassword();
            this.Hide();
            signUp.ShowDialog();
            this.Close();
        }
    }
}
