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
    public partial class CancelForm : Form
    {
        private int _userId;
        private string _username;
        private decimal _balance;
        string connectionString = ConfigurationManager.ConnectionStrings["myConstr"].ConnectionString;
        public CancelForm(int id, string username, decimal balance)
        {
            InitializeComponent();
            _userId = id;
            _username = username;
            _balance = balance;
            createEmptyColumns();
            this.Load += CancelForm_Load;
            dataGridView1.CellClick += dataGridView1_CellClick;
        }
        private void CancelForm_Load(object sender, EventArgs e)
        {
            LoadUpcomingTickets();
        }
        private void createEmptyColumns()
        {
            // Clear any existing columns
            dataGridView1.Columns.Clear();

            DataGridViewTextBoxColumn ticketDate = new DataGridViewTextBoxColumn();
            ticketDate.Name = "Ticket_Date";
            ticketDate.HeaderText = "Ticket_Date";
            dataGridView1.Columns.Add(ticketDate);

            DataGridViewTextBoxColumn tckTime = new DataGridViewTextBoxColumn();
            tckTime.Name = "Tck Time";
            tckTime.HeaderText = "Tck Time";
            dataGridView1.Columns.Add(tckTime);

            DataGridViewTextBoxColumn name = new DataGridViewTextBoxColumn();
            name.Name = "Name";
            name.HeaderText = "Name";
            dataGridView1.Columns.Add(name);

            DataGridViewTextBoxColumn drawTime = new DataGridViewTextBoxColumn();
            drawTime.Name = "Draw Time";
            drawTime.HeaderText = "Draw Time";
            dataGridView1.Columns.Add(drawTime);

            DataGridViewTextBoxColumn qty = new DataGridViewTextBoxColumn();
            qty.Name = "QTY";
            qty.HeaderText = "QTY";
            dataGridView1.Columns.Add(qty);

            DataGridViewTextBoxColumn points = new DataGridViewTextBoxColumn();
            points.Name = "Points";
            points.HeaderText = "Points";
            dataGridView1.Columns.Add(points);

            DataGridViewTextBoxColumn barcode = new DataGridViewTextBoxColumn();
            barcode.Name = "BarcodeNum";
            barcode.HeaderText = "BarcodeNum";
            dataGridView1.Columns.Add(barcode);

            DataGridViewTextBoxColumn lotteryName = new DataGridViewTextBoxColumn();
            lotteryName.Name = "Lottery_Name";
            lotteryName.HeaderText = "Lottery_Name";
            dataGridView1.Columns.Add(lotteryName);

            DataGridViewButtonColumn cancelBtn = new DataGridViewButtonColumn();
            cancelBtn.Name = "Cancel";
            cancelBtn.HeaderText = "Cancel";
            cancelBtn.Text = "Cancel";
            cancelBtn.UseColumnTextForButtonValue = true;
            dataGridView1.Columns.Add(cancelBtn);
        }
        private void LoadUpcomingTickets()
        {
            string query = @" SELECT PurchaseID, RecordDate AS Ticket_Date,TicketTime AS TckTime, Username AS Name, NextDraw AS DrawTime,
            TotalQuantity AS QTY,TotalAmount AS Points, Barcode AS BarcodeNum, 'Daily Draw' AS Lottery_Name
            FROM TicketPurchases WHERE UserID = @UserID AND NextDraw >= GETDATE()
            ORDER BY NextDraw";

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@UserID", _userId);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                dataGridView1.Rows.Clear();

                while (reader.Read())
                {
                    dataGridView1.Rows.Add(
                        reader["Ticket_Date"],
                        reader["TckTime"],
                        reader["Name"],
                        reader["DrawTime"],
                        reader["QTY"],
                        reader["Points"],
                        reader["BarcodeNum"],
                        reader["Lottery_Name"],
                        "Cancel" // Button label
                    );
                }
            }
        }
        private decimal GetUserBalance(int userId)
        {
            string query = "SELECT Balance FROM Users WHERE Id = @UserID";
            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@UserID", userId);
                conn.Open();
                //return (decimal)cmd.ExecuteScalar();
                object result = cmd.ExecuteScalar();

                if (result == null || result == DBNull.Value)
                {
                    return _balance;
                }

                return Convert.ToDecimal(result);
            }
        }
        private void btnBack_Click(object sender, EventArgs e)
        {
            _balance = GetUserBalance(_userId);
            this.Hide();
            Dashboard db = new Dashboard(_userId, _username, _balance);
            db.Show();
        }
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= dataGridView1.Rows.Count)
                return;

            if (dataGridView1.Rows[e.RowIndex].IsNewRow)
                return;

            if (dataGridView1.Columns[e.ColumnIndex].Name == "Cancel")
            {
                var barcodeCell = dataGridView1.Rows[e.RowIndex].Cells["BarcodeNum"].Value;
                if (barcodeCell == null || barcodeCell == DBNull.Value)
                {
                    MessageBox.Show("Barcode is missing for this ticket.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string barcode = barcodeCell.ToString();

                var confirm = MessageBox.Show("Are you sure you want to cancel this ticket?", "Confirm Cancel", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (confirm == DialogResult.Yes)
                {
                    CancelTicketByBarcode(barcode);

                    // Refresh balance from DB
                    _balance = GetUserBalance(_userId);

                    LoadUpcomingTickets();
                }
            }
        }
        private void CancelTicketByBarcode(string barcode)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("CancelTicketPurchaseByBarcode", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserID", _userId);
                cmd.Parameters.AddWithValue("@Barcode", barcode);

                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Ticket canceled successfully. Balance refunded.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error canceling ticket: " + ex.Message);
                }
            }
        }

    }
}
