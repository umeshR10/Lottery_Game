﻿using System;
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
using System.Drawing.Printing;

namespace FinalTask
{
    public partial class Results : Form
    {
        private int _userId;
        private string _username;
        private decimal _balance;
        // Fields for printing
        private PrintDocument printDocument = new PrintDocument();
        private string printText = "";
        string connectionString = ConfigurationManager.ConnectionStrings["myConstr"].ConnectionString;
        public Results(int id, string username, decimal balance)
        {
            InitializeComponent();
            _userId = id;
            _username = username;
            _balance = balance;
            comboBox1.MaxDropDownItems = 5;
            fetchDrawTime();
        }
        private void fetchDrawTime()
        {
            string query = "select DrawTime from DrawResults WHERE DrawDate = CAST(GETDATE() AS DATE) AND CAST(DrawDate AS DATETIME) + CAST(DrawTime AS DATETIME) < DATEADD(MINUTE, -15, GETDATE())";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    try
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                comboBox1.Items.Add(reader["DrawTime"].ToString());
                            }
                            reader.Close();
                        }

                        // Sort times in descending order and auto-select latest
                        var sorted = comboBox1.Items.Cast<string>().OrderByDescending(t => TimeSpan.Parse(t)).ToList();
                        comboBox1.Items.Clear();
                        comboBox1.Items.AddRange(sorted.ToArray());

                        if (comboBox1.Items.Count > 0)
                        {
                            comboBox1.SelectedIndex = 0; // Automatically select the latest
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: " + ex.Message);
                    }
                }
            }
        }
        private void fetchResults()
        {
            string query = "select ResultList from DrawResults where DrawTime = @DrawTime and DrawDate = @DrawDate";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    try
                    {
                        cmd.Parameters.AddWithValue("@DrawTime", comboBox1.SelectedItem.ToString());
                        cmd.Parameters.AddWithValue("@DrawDate", dateTimePicker1.Value.Date);

                        SqlDataReader reader = cmd.ExecuteReader();

                        dataGridView1.Columns.Clear();
                        dataGridView1.Rows.Clear();
                        dataGridView1.ColumnHeadersVisible = false; // Hide headers

                        if (reader.Read())
                        {
                            string resultList = reader.GetString(0);
                            string[] results = resultList.Split(',');

                            int columns = 10;
                            int rows = 10;

                            // Add 10 columns without headers
                            for (int i = 0; i < columns; i++)
                            {
                                dataGridView1.Columns.Add($"col{i}", "");
                            }

                            // Adjust cell sizes and style
                            dataGridView1.RowTemplate.Height = 30;
                            foreach (DataGridViewColumn col in dataGridView1.Columns)
                            {
                                col.Width = 50;
                                col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                            }

                            // Fill rows with results
                            for (int r = 0; r < rows; r++)
                            {
                                var rowValues = new List<string>();
                                for (int c = 0; c < columns; c++)
                                {
                                    int index = r * columns + c;
                                    rowValues.Add(index < results.Length ? results[index] : "");
                                }
                                dataGridView1.Rows.Add(rowValues.ToArray());
                            }
                            ResizeDataGridViewHeight();
                        }
                        else
                        {
                            MessageBox.Show("No results found for selected draw.");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: " + ex.Message);
                    }
                }
            }
        }
        private void ResizeDataGridViewHeight()
        {
            int totalRowHeight = dataGridView1.Rows.GetRowsHeight(DataGridViewElementStates.Visible);
            int columnHeaderHeight = dataGridView1.ColumnHeadersVisible ? dataGridView1.ColumnHeadersHeight : 0;
            int borderHeight = dataGridView1.Height - dataGridView1.ClientSize.Height;

            dataGridView1.Height = totalRowHeight + columnHeaderHeight + borderHeight;
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            fetchResults();
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            this.Hide();
            Dashboard dashboard = new Dashboard(_userId, _username, _balance);
            dashboard.Show();
        }
        private void LoadDrawTimesForSelectedDate()
        {
            comboBox1.Items.Clear();

            bool isToday = dateTimePicker1.Value.Date == DateTime.Today;
            string query;

            if (isToday)
            {
                query = "select DrawTime from DrawResults where DrawDate = CAST(GETDATE() as DATE) " +
                    "and CAST(DrawDate as DATETIME) + CAST(DrawTime as DATETIME) < DATEADD(MINUTE, - 15, GETDATE())" +
                    "ORDER BY DrawTime";
            }
            else
            {
                query = "SELECT DrawTime FROM DrawResults WHERE DrawDate = @SelectedDate ORDER BY DrawTime";
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    if (!isToday)
                    {
                        cmd.Parameters.AddWithValue("@SelectedDate", dateTimePicker1.Value.Date);
                    }

                    try
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                comboBox1.Items.Add(reader["DrawTime"].ToString());
                            }
                        }

                        if (comboBox1.Items.Count > 0)
                        {
                            // Optionally select the latest or first item
                            comboBox1.SelectedIndex = 0;
                        }
                        else
                        {
                            MessageBox.Show("No draw times available for selected date.");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error loading draw times: " + ex.Message);
                    }
                }
            }
        }
        private void btnGo_Click(object sender, EventArgs e)
        {
            LoadDrawTimesForSelectedDate();
        }
        private void PreparePrintText()
        {
            StringBuilder sb = new StringBuilder();

            // Add header info: Date and Draw Time
            string selectedDrawTime = comboBox1.SelectedItem?.ToString() ?? "N/A";
            string selectedDate = dateTimePicker1.Value.ToShortDateString();

            sb.AppendLine($"Draw Results - {selectedDate} at {selectedDrawTime}");
            sb.AppendLine(new string('-', 50)); // separator line

            // Grid data
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                for (int j = 0; j < dataGridView1.Columns.Count; j++)
                {
                    var value = dataGridView1.Rows[i].Cells[j].Value?.ToString();
                    sb.Append(string.IsNullOrWhiteSpace(value) ? "   " : $"{value,3}");
                    sb.Append("  "); // spacing between cells
                }
                sb.AppendLine();
            }

            printText = sb.ToString();
        }
        private void printDocument_PrintPage(object sender, PrintPageEventArgs e)
        {
            Font headerFont = new Font("Arial", 12, FontStyle.Bold);
            Font bodyFont = new Font("Courier New", 10);
            float yPos = e.MarginBounds.Top;

            // Draw header (first line)
            string selectedDrawTime = comboBox1.SelectedItem?.ToString() ?? "N/A";
            string selectedDate = dateTimePicker1.Value.ToShortDateString();
            string header = $"Draw Results - {selectedDate} at {selectedDrawTime}";

            e.Graphics.DrawString(header, headerFont, Brushes.Black, e.MarginBounds.Left, yPos);
            yPos += headerFont.GetHeight(e.Graphics) + 10; // add space after header

            // Now draw the rest of the content
            string[] lines = printText.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            for (int i = 2; i < lines.Length; i++) // skip first 2 lines (header & separator already printed)
            {
                e.Graphics.DrawString(lines[i], bodyFont, Brushes.Black, e.MarginBounds.Left, yPos);
                yPos += bodyFont.GetHeight(e.Graphics);
            }

            e.HasMorePages = false;
        }
        private void btnPrint_Click(object sender, EventArgs e)
        {
            if (dataGridView1.Rows.Count == 0)
            {
                MessageBox.Show("There is nothing to print.");
                return;
            }

            PreparePrintText();

            PrintDialog printDialog = new PrintDialog();
            printDialog.Document = printDocument;

            printDocument.PrintPage += new PrintPageEventHandler(printDocument_PrintPage);

            if (printDialog.ShowDialog() == DialogResult.OK)
            {
                printDocument.Print();
            }
        }
    }
}
