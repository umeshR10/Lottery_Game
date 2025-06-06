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
    public partial class AccountsForm : Form
    {
        private int _userId;
        private string _username;
        private decimal _balance;
        string connectionString = ConfigurationManager.ConnectionStrings["myConstr"].ConnectionString;
        public AccountsForm(int id, string username, decimal balance)
        {
            InitializeComponent();
            _userId = id;
            _username = username;
            _balance = balance;
            createEmptyColumns();
        }

        private void createEmptyColumns()
        {
            // Clear any existing columns
            dataGridView1.Columns.Clear();

            DataGridViewTextBoxColumn idColumn = new DataGridViewTextBoxColumn();
            idColumn.Name = "Amt";
            idColumn.HeaderText = "Amt";
            dataGridView1.Columns.Add(idColumn);

            DataGridViewTextBoxColumn winAmtColumn = new DataGridViewTextBoxColumn();
            winAmtColumn.Name = "Win Amount";
            winAmtColumn.HeaderText = "Win Amount";
            dataGridView1.Columns.Add(winAmtColumn);

            DataGridViewTextBoxColumn commissiontColumn = new DataGridViewTextBoxColumn();
            commissiontColumn.Name = "Commission";
            commissiontColumn.HeaderText = "Commission";
            dataGridView1.Columns.Add(commissiontColumn);

            DataGridViewTextBoxColumn payColumn = new DataGridViewTextBoxColumn();
            payColumn.Name = "Net To Pay";
            payColumn.HeaderText = "Net To Pay";
            dataGridView1.Columns.Add(payColumn);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Hide();
            Dashboard dashboard = new Dashboard(_userId, _username, _balance);
            dashboard.Show();
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            DateTime fromDate = fromDatePicker.Value.Date;
            DateTime toDate = toDatePicker.Value.Date.AddDays(1).AddSeconds(-1); // Include end of day

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"SELECT ISNULL(SUM(TotalAmount), 0) AS TotalAmount,ISNULL(SUM(WinningTotalAmount), 0) AS WinningAmount
                FROM TicketPurchases WHERE UserID = @UserID AND DrawDate BETWEEN @FromDate AND @ToDate";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@UserID", _userId);
                cmd.Parameters.AddWithValue("@FromDate", fromDate);
                cmd.Parameters.AddWithValue("@ToDate", toDate);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    decimal amt = reader.GetDecimal(0);
                    decimal winAmt = reader.GetDecimal(1);
                    decimal commission = 0;
                    decimal netToPay = amt - winAmt - commission;

                    dataGridView1.Rows.Clear(); // Keep headers
                    dataGridView1.Rows.Add(
                        amt.ToString("0.00"),
                        winAmt.ToString("0.00"),
                        commission.ToString("0.00"),
                        netToPay.ToString("0.00")
                    );
                }

                conn.Close();
            }
        }
    }
}
