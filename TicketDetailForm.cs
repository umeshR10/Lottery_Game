using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace FinalTask
{
    public partial class TicketDetailForm : Form
    {
        private string _barcode;
        string connectionString = ConfigurationManager.ConnectionStrings["myConstr"].ConnectionString;

        public TicketDetailForm(string barcode)
        {
            InitializeComponent();
            _barcode = barcode;

            LoadTicketDetails();
        }

        private void LoadTicketDetails()
        {
            // Add only required columns
            if (dataGridView1.Columns.Count == 0)
            {
                dataGridView1.Columns.Add("Ticket", "Ticket");
                dataGridView1.Columns.Add("Quantity", "Quantity");
            }

            dataGridView1.Rows.Clear(); // Clear previous rows

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT TicketResult, Quantity FROM TicketPurchases WHERE Barcode = @barcode";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@barcode", _barcode);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    string[] tickets = reader["TicketResult"].ToString().Split(',');
                    string[] qty = reader["Quantity"].ToString().Split(',');

                    for (int i = 0; i < tickets.Length; i++)
                    {
                        dataGridView1.Rows.Add(
                            tickets[i],
                            i < qty.Length ? qty[i] : "0"
                        );
                    }
                }
                con.Close();
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
