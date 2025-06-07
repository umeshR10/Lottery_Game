using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FinalTask
{
    public partial class RePrintForm : Form
    {
        private int _userId;
        private string _username;
        private decimal _balance;
        private string printBarcode = "";
        string connectionString = ConfigurationManager.ConnectionStrings["myConstr"].ConnectionString;
        public RePrintForm(int id, string username, decimal balance)
        {
            InitializeComponent();

            _userId = id;
            _username = username;
            _balance = balance;

            createEmptyColumns();
            dateTimePicker1.ValueChanged += dateTimePicker1_ValueChanged;

            // Fetch for today on load
            LoadTicketData(dateTimePicker1.Value.Date);
        }

        private void btnHome_Click(object sender, EventArgs e)
        {
            this.Hide();
            Dashboard db = new Dashboard(_userId,_username,_balance);
            db.Show();
        }

        private void createEmptyColumns()
        {
            dataGridView.Rows.Clear();
            dataGridView.Columns.Clear();

            dataGridView.Columns.Add("Username", "Username");
            dataGridView.Columns.Add("DrawTime", "Draw Time");
            dataGridView.Columns.Add("DrawDate", "Date");
            dataGridView.Columns.Add("BarcodeNum", "BarcodeNum");
            dataGridView.Columns.Add("QTY", "QTY");
            dataGridView.Columns.Add("Points", "Points");
            dataGridView.Columns.Add("WinAmount", "Win Amount");
            dataGridView.Columns.Add("ClaimStatus", "Claim Status");

            DataGridViewButtonColumn viewBtn = new DataGridViewButtonColumn();
            viewBtn.Name = "View";
            viewBtn.HeaderText = "View";
            viewBtn.Text = "View";
            viewBtn.UseColumnTextForButtonValue = true;
            dataGridView.Columns.Add(viewBtn);

            DataGridViewButtonColumn rePrintBtn = new DataGridViewButtonColumn();
            rePrintBtn.Name = "RePrint";
            rePrintBtn.HeaderText = "Re Print";
            rePrintBtn.Text = "Re Print";
            rePrintBtn.UseColumnTextForButtonValue = true;
            dataGridView.Columns.Add(rePrintBtn);

            dataGridView.CellClick -= dataGridView_CellClick;
            dataGridView.CellClick += dataGridView_CellClick;
        }
        private void LoadTicketData(DateTime selectedDate)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT * FROM TicketPurchases
                    WHERE UserID = @userId AND CAST(DrawDate AS DATE) = @selectedDate
                    ORDER BY DrawDate DESC, TicketTime DESC";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@userId", _userId);
                cmd.Parameters.AddWithValue("@selectedDate", selectedDate);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                dataGridView.Rows.Clear();

                foreach (DataRow row in dt.Rows)
                {
                    dataGridView.Rows.Add(
                        row["Username"].ToString(),
                        Convert.ToDateTime(row["NextDraw"]).ToString("hh:mm tt"),
                        Convert.ToDateTime(row["DrawDate"]).ToString("dd-MM-yyyy"),
                        row["Barcode"].ToString(),
                        row["TotalQuantity"].ToString(),
                        row["TotalAmount"].ToString(),
                        row["WinningTotalAmount"] != DBNull.Value ? row["WinningTotalAmount"].ToString() : "0.00",
                        row["WinningResult"] != DBNull.Value ? "Claimed" : "Not Claimed",
                        "View",
                        "Re Print"
                    );
                }
            }
        }
        private void PrintDoc_PrintPage(object sender, PrintPageEventArgs e)
        {
            Font headerFont = new Font("Arial", 14, FontStyle.Bold);
            Font labelFont = new Font("Arial", 11, FontStyle.Regular);
            float y = 100;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM TicketPurchases WHERE Barcode = @barcode";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@barcode", printBarcode);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    // Header
                    e.Graphics.DrawString("Reprint", headerFont, Brushes.Black, 100, y); y += 30;
                    e.Graphics.DrawString("Malamaal Daily", headerFont, Brushes.Black, 100, y); y += 40;

                    // User info
                    e.Graphics.DrawString("UserName : " + reader["Username"], labelFont, Brushes.Black, 100, y); y += 25;
                    e.Graphics.DrawString("UserID   : " + reader["Username"], labelFont, Brushes.Black, 100, y); y += 25;

                    // Date & Time
                    e.Graphics.DrawString("DATE     : " + Convert.ToDateTime(reader["DrawDate"]).ToString("yyyy-MM-dd"), labelFont, Brushes.Black, 100, y); y += 25;
                    e.Graphics.DrawString("TIME     : " + DateTime.Now.ToString("HH:mm:ss"), labelFont, Brushes.Black, 100, y); y += 25;

                    // Draw Time
                    e.Graphics.DrawString("Draw Time: " + Convert.ToDateTime(reader["NextDraw"]).ToString("hh:mm tt"), labelFont, Brushes.Black, 100, y); y += 30;

                    // Divider
                    e.Graphics.DrawString("----------------------------------------", labelFont, Brushes.Black, 100, y); y += 25;

                    // Ticket Numbers with quantity
                    string[] tickets = reader["TicketResult"].ToString().Split(',');
                    string[] qty = reader["Quantity"].ToString().Split(',');

                    for (int i = 0; i < tickets.Length; i++)
                    {
                        string ticketText = $"{tickets[i]}-{(i < qty.Length ? qty[i] : "1")},";
                        e.Graphics.DrawString(ticketText, labelFont, Brushes.Black, 100, y); y += 25;
                    }

                    // Divider
                    e.Graphics.DrawString("----------------------------------------", labelFont, Brushes.Black, 100, y); y += 25;

                    // Summary
                    e.Graphics.DrawString("Quantity      :- " + reader["TotalQuantity"], labelFont, Brushes.Black, 100, y); y += 25;
                    e.Graphics.DrawString("Total Amount  :- Rs" + reader["TotalAmount"], labelFont, Brushes.Black, 100, y); y += 25;
                }
                con.Close();
            }
        }
        private void dataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                string barcode = dataGridView.Rows[e.RowIndex].Cells["BarcodeNum"].Value.ToString();

                if (dataGridView.Columns[e.ColumnIndex].Name == "View")
                {
                    TicketDetailForm detailForm = new TicketDetailForm(barcode);
                    detailForm.ShowDialog();
                }
                else if (dataGridView.Columns[e.ColumnIndex].Name == "RePrint")
                {
                    printBarcode = barcode;
                    PrintDocument printDoc = new PrintDocument();
                    printDoc.PrintPage += PrintDoc_PrintPage;

                    PrintPreviewDialog previewDialog = new PrintPreviewDialog();
                    previewDialog.Document = printDoc;
                    previewDialog.ShowDialog();
                }
            }
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            LoadTicketData(dateTimePicker1.Value.Date);
        }
    }
}
