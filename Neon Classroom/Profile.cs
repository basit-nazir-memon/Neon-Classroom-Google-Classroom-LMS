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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace Neon_Classroom
{
    public partial class Profile : Form
    {
        SqlConnection cn = new SqlConnection();
        SqlCommand cmd = new SqlCommand();
        SqlDataReader reader;

        public Profile()
        {
            InitializeComponent();
            cn = new SqlConnection(DB.connStr);
            cn.Open();
            cmd = new SqlCommand("EXEC getUserInfo @userId", cn);
            cmd.Parameters.AddWithValue("@userId", DB.userId);
            reader = cmd.ExecuteReader();
            reader.Read();
            label1.Text = reader["first_name"].ToString();
            label8.Text = reader["last_name"].ToString();
            label11.Text = reader["email"].ToString();
            label9.Text = reader["username"].ToString();
            label12.Text = reader["dob"].ToString();
            label13.Text = reader["gender"].ToString();
            label14.Text = reader["role"].ToString();
            reader.Close();
            cn.Close();

        }
       
    }
}
