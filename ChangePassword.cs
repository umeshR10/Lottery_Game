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
    public partial class ChangePassword : Form
    {
        private int _userId;
        private string _username;
        private decimal _balance;
        string connectionString = ConfigurationManager.ConnectionStrings["myConstr"].ConnectionString;
        public ChangePassword(int id, string username, decimal balance)
        {
            InitializeComponent();
            _userId = id;
            _username = username;
            _balance = balance;
        }
        private void changePassword()
        {
            if(textBoxPass.Text == textBoxPassCon.Text)
            {
                try
                {
                    using(SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();
                        string query = "UPDATE Users SET Password = @NewPassword WHERE username = @Username";
                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@NewPassword", textBoxPass.Text);
                            cmd.Parameters.AddWithValue("@Username", textBoxUsername.Text); 
                            cmd.ExecuteNonQuery();
                        }
                    }
                    MessageBox.Show("Password changed successfully.");
                    textBoxUsername.Clear();
                    textBoxPass.Clear();
                    textBoxPassCon.Clear();
                }
                catch (Exception e )
                {
                    MessageBox.Show("Error changing password: " + e.Message);
                }
            }
            else
            {
                MessageBox.Show("Passwords do not match.");
            }
        }
        private void ChangePassBtn_Click(object sender, EventArgs e) => changePassword();

        private void btnBack_Click(object sender, EventArgs e)
        {
            this.Hide();
            Dashboard dashboard = new Dashboard(_userId, _username, _balance);
            dashboard.Show();
        }
    }
}
