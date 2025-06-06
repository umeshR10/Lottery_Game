using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FinalTask
{
    public partial class LoginForm : Form
    {
        string connectionString = ConfigurationManager.ConnectionStrings["myConstr"].ConnectionString;
      
        public LoginForm()
        {
            InitializeComponent();
        }
        private void userLogin()
        {
            string username = userName_textBox.Text;
            string password = password_textBox.Text;
            string query = "select * from users where username = @username and password = @password";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query,conn))
                {
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@password", password);
                    try
                    {
                        conn.Open();
                        SqlDataReader reader = cmd.ExecuteReader();
                        if (reader.HasRows)
                        {   
                            reader.Read();
                            int id = (Int32)reader["id"];
                            string user = reader["username"].ToString();
                            decimal balance = Convert.ToDecimal(reader["balance"]);

                            //MessageBox.Show("Login successful!");
                            this.Hide();
                            Dashboard dashboard = new Dashboard(id,user,balance);
                            ChangePassword changePassword = new ChangePassword(id, user, balance);
                            dashboard.Show();
                        }
                        else
                        {
                            MessageBox.Show("Invalid username or password.");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: " + ex.Message);
                    }
                }
            }
        }
        private void clearLoginFeilds()
        {
            userName_textBox.Clear();
            password_textBox.Clear();
        }
        private void CharShowButton_Click(object sender, EventArgs e)
        {
            if (password_textBox.UseSystemPasswordChar == true)
            {
                password_textBox.UseSystemPasswordChar = false;
                CharShowButton.Image = Properties.Resources.open_eye;
            }
            else
            {
                password_textBox.UseSystemPasswordChar = true;
                CharShowButton.Image = Properties.Resources.closed_eye;
            }
        }

        private void loginButton_Click(object sender, EventArgs e)
        {
            userLogin();
            clearLoginFeilds();
        }
    }
}
