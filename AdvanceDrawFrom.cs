using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ZXing;

namespace FinalTask
{
    public partial class AdvanceDrawFrom : Form
    {
        string connectionString = ConfigurationManager.ConnectionStrings["myConstr"].ConnectionString;
        private const int ColumnsPerRow = 6;
        private List<TimeSpan> _allDrawTimes = new List<TimeSpan>();
        private HashSet<string> selectedDrawStrings = new HashSet<string>();

        public List<TimeSpan> SelectedDrawTimes { get; private set; } = new List<TimeSpan>();

        public AdvanceDrawFrom()
        {
            InitializeComponent();

            dataGridView1.ReadOnly = true;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.CellSelect;
            dataGridView1.MultiSelect = false;
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.ColumnHeadersVisible = false;
            dataGridView1.AllowUserToResizeRows = false;
            dataGridView1.AllowUserToResizeColumns = false;

            dataGridView1.DefaultCellStyle.SelectionBackColor = Color.White;
            dataGridView1.DefaultCellStyle.SelectionForeColor = Color.Black;

            dataGridView1.CellClick += dataGridView1_CellClick;

            textBoxSelectMultiple.TextChanged += textBoxSelectMultiple_TextChanged;
        }

        private void AdvanceDrawFrom_Load(object sender, EventArgs e)
        {
            LoadUpcomingDraws();

            Timer timer = new Timer();
            timer.Interval = 60000; // refresh every minute
            timer.Tick += (s, ev) => LoadUpcomingDraws();
            timer.Start();
        }

        private void LoadUpcomingDraws()
        {
            string query = @"SELECT DrawTime FROM DrawResults WHERE DrawDate = CAST(GETDATE() AS DATE) ORDER BY DrawTime";
            _allDrawTimes.Clear();

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        TimeSpan drawTime = (TimeSpan)reader["DrawTime"];
                        if (drawTime > DateTime.Now.TimeOfDay)
                            _allDrawTimes.Add(drawTime);
                    }
                }
            }

            DataTable table = new DataTable();
            for (int i = 0; i < ColumnsPerRow; i++)
                table.Columns.Add();

            for (int i = 0; i < _allDrawTimes.Count; i += ColumnsPerRow)
            {
                DataRow row = table.NewRow();
                for (int j = 0; j < ColumnsPerRow; j++)
                {
                    int index = i + j;
                    row[j] = index < _allDrawTimes.Count ?
                        DateTime.Today.Add(_allDrawTimes[index]).ToString("hh:mm tt") : "";
                }
                table.Rows.Add(row);
            }

            dataGridView1.DataSource = table;
            dataGridView1.ClearSelection(); // Clear default selection

            if (_allDrawTimes.Count > 0)
            {
                string firstDrawTimeStr = DateTime.Today.Add(_allDrawTimes[0]).ToString("hh:mm tt");

                selectedDrawStrings.Clear(); // clear existing
                selectedDrawStrings.Add(firstDrawTimeStr); // add only the first draw

                // Refresh DataGridView cell backgrounds
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    foreach (DataGridViewCell cell in row.Cells)
                    {
                        string val = cell.Value?.ToString()?.Trim();

                        if (!string.IsNullOrEmpty(val) && selectedDrawStrings.Contains(val))
                        {
                            cell.Style.BackColor = Color.LightGreen;
                        }
                        else
                        {
                            cell.Style.BackColor = Color.White;
                        }
                    }
                }
            }

            // Repaint cells based on `selectedDrawStrings`
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                foreach (DataGridViewCell cell in row.Cells)
                {
                    string val = cell.Value?.ToString()?.Trim();

                    if (!string.IsNullOrEmpty(val) && selectedDrawStrings.Contains(val))
                    {
                        cell.Style.BackColor = Color.LightGreen;
                    }
                    else
                    {
                        cell.Style.BackColor = Color.White;
                    }
                }
            }
            lblDrawRemaining.Text = $"Remaining Draws: {_allDrawTimes.Count}";
        }

        public List<TimeSpan> GetSelectedDrawTimes()
        {
            var selectedTimes = new List<TimeSpan>();

            foreach (string timeStr in selectedDrawStrings)
            {
                if (DateTime.TryParse(timeStr, out DateTime dt))
                {
                    selectedTimes.Add(dt.TimeOfDay);
                }
            }

            return selectedTimes;
        }

        private DateTime GetCurrentDrawSlot(DateTime now)
        {
            DateTime start = now.Date.AddHours(8); // 08:00 AM
            DateTime end = now.Date.AddHours(22);  // 10:00 PM

            if (now < start)
                return start;
            if (now >= end)
                return end;

            int elapsedMinutes = (int)(now - start).TotalMinutes;
            int completedSlots = elapsedMinutes / 15;

            return start.AddMinutes(completedSlots * 15);
        }

        private void btnOkay_Click(object sender, EventArgs e)
        {
            SelectedDrawTimes = GetSelectedDrawTimes();

            //if (SelectedDrawTimes.Count == 0)
            //{
            //    MessageBox.Show("No draw time selected.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    return;
            //}

            //string message = "You selected the following draw slots:\n" +
            //    string.Join("\n", SelectedDrawTimes.Select(t => DateTime.Today.Add(t).ToString("hh:mm tt")));

            //MessageBox.Show(message, "Draw Slots Selected", MessageBoxButtons.OK, MessageBoxIcon.Information);

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            var cell = dataGridView1[e.ColumnIndex, e.RowIndex];
            string val = cell.Value?.ToString()?.Trim();

            if (string.IsNullOrEmpty(val)) return;

            if (selectedDrawStrings.Contains(val))
            {
                selectedDrawStrings.Remove(val);
                cell.Style.BackColor = Color.White;
            }
            else
            {
                selectedDrawStrings.Add(val);
                cell.Style.BackColor = Color.LightGreen;
            }

            dataGridView1.ClearSelection(); // Prevents built-in blue highlight
        }

        private void textBoxSelectMultiple_TextChanged(object sender, EventArgs e)
        {
            if (int.TryParse(textBoxSelectMultiple.Text, out int numberOfDraws) && numberOfDraws > 0)
            {
                // Select the next 'numberOfDraws' available draw times
                SelectNextDraws(numberOfDraws);
            }
            else
            {
                // Clear any selections if input is invalid
                selectedDrawStrings.Clear();
                dataGridView1.Refresh();
            }
        }
        private void SelectNextDraws(int numberOfDraws)
        {
            // Clear previous selection to reset the grid
            selectedDrawStrings.Clear();

            // Select 'numberOfDraws' future draw times
            int selectedCount = 0;
            foreach (var drawTime in _allDrawTimes)
            {
                if (selectedCount >= numberOfDraws)
                    break;

                // Add the formatted draw time to the selected set
                string formattedTime = DateTime.Today.Add(drawTime).ToString("hh:mm tt");
                selectedDrawStrings.Add(formattedTime);
                selectedCount++;
            }

            // Update DataGridView to reflect new selection
            UpdateDataGridViewSelection();
        }

        private void UpdateDataGridViewSelection()
        {
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                foreach (DataGridViewCell cell in row.Cells)
                {
                    string val = cell.Value?.ToString()?.Trim();

                    if (!string.IsNullOrEmpty(val) && selectedDrawStrings.Contains(val))
                    {
                        cell.Style.BackColor = Color.LightGreen;
                    }
                    else
                    {
                        cell.Style.BackColor = Color.White;
                    }
                }
            }
        }


    }
}
