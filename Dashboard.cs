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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using System.Drawing.Printing;
using System.IO;

namespace FinalTask
{
    public partial class Dashboard : Form
    {
        private int _userId;
        private string _username;
        private decimal _balance;
        private DateTime _lastGeneratedDrawTime = DateTime.MinValue;
        private HashSet<int> userEnteredNumbers = new HashSet<int>();
        string connectionString = ConfigurationManager.ConnectionStrings["myConstr"].ConnectionString;
        private bool isLabelViewMode = false;
        private int labelOffset = 0;  // 0 for default 0–999, 1000 for 1000–1999, ..., 9000 for 9000–9999
        private bool isUpdatingAllCheckBox = false;
        private List<(string number, int qty)> _printTickets = new List<(string number, int qty)>();
        private int _printTotalQty = 0;
        private decimal _printTotalAmt = 0;
        private string _printUsername = "";
        private int _printUserId;
        private string _printDrawTime = "";
        private Dictionary<int, HashSet<int>> cpPatternMap = new Dictionary<int, HashSet<int>>();
        private int? lastCPSource = null;

        public Dashboard(int id, string username, decimal balance)
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            _userId = id;
            _username = username;
            _balance = balance;
            InitValues();
            LoadTextBoxes(0); // default load
        }
        private void Dashboard_Load(object sender, EventArgs e)
        {
            userNameLable.Text = $"UserName : {_username}";
            userIDLable.Text = $"UserID : {_userId}";
            balanceLable.Text = $"{_balance}";
            inputValidation();
            headlineLable.Left = this.Width;
            clockTimer.Start();
            currentDateLable.Text = DateTime.Now.ToShortDateString();
            GenerateAndSaveAllDrawsForToday(); // this will auto-check if the slot already has results
            LoadLatestResults();
            checkBox0_99.Checked = true;

            // Attach to F0–F9 and B0–B9
            for (int i = 0; i < 10; i++)
            {
                TextBox txtF = this.Controls.Find("txtF" + i, true).FirstOrDefault() as TextBox;
                if (txtF != null)
                    txtF.TextChanged += txtF_TextChanged;

                TextBox txtB = this.Controls.Find("txtB" + i, true).FirstOrDefault() as TextBox;
                if (txtB != null)
                    txtB.TextChanged += txtB_TextChanged;
            }

            InitTicketGrid();

            checkBox0_99.CheckedChanged += RangeCheckBox_CheckedChanged;
            checkBox100_199.CheckedChanged += RangeCheckBox_CheckedChanged;
            checkBox200_299.CheckedChanged += RangeCheckBox_CheckedChanged;
            checkBox300_399.CheckedChanged += RangeCheckBox_CheckedChanged;
            checkBox400_499.CheckedChanged += RangeCheckBox_CheckedChanged;
            checkBox500_599.CheckedChanged += RangeCheckBox_CheckedChanged;
            checkBox600_699.CheckedChanged += RangeCheckBox_CheckedChanged;
            checkBox700_799.CheckedChanged += RangeCheckBox_CheckedChanged;
            checkBox800_899.CheckedChanged += RangeCheckBox_CheckedChanged;
            checkBox900_999.CheckedChanged += RangeCheckBox_CheckedChanged;

            labelCheck0_99.Click += (s, ev) => HandleLabelClick(0);
            labelCheck100_199.Click += (s, ev) => HandleLabelClick(100);
            labelCheck200_299.Click += (s, ev) => HandleLabelClick(200);
            labelCheck300_399.Click += (s, ev) => HandleLabelClick(300);
            labelCheck400_499.Click += (s, ev) => HandleLabelClick(400);
            labelCheck500_599.Click += (s, ev) => HandleLabelClick(500);
            labelCheck600_699.Click += (s, ev) => HandleLabelClick(600);
            labelCheck700_799.Click += (s, ev) => HandleLabelClick(700);
            labelCheck800_899.Click += (s, ev) => HandleLabelClick(800);
            labelCheck900_999.Click += (s, ev) => HandleLabelClick(900);

            label0_9.Click += (s, ev) => HandleHorizontalLabelClick(0);
            label10_19.Click += (s, ev) => HandleHorizontalLabelClick(1);
            label20_29.Click += (s, ev) => HandleHorizontalLabelClick(2);
            label30_39.Click += (s, ev) => HandleHorizontalLabelClick(3);
            label40_49.Click += (s, ev) => HandleHorizontalLabelClick(4);
            label50_59.Click += (s, ev) => HandleHorizontalLabelClick(5);
            label60_69.Click += (s, ev) => HandleHorizontalLabelClick(6);
            label70_79.Click += (s, ev) => HandleHorizontalLabelClick(7);
            label80_89.Click += (s, ev) => HandleHorizontalLabelClick(8);
            label90_99.Click += (s, ev) => HandleHorizontalLabelClick(9);

            checkBox0_9.CheckedChanged += LabelMode_CheckedChanged;
            checkBox10_19.CheckedChanged += LabelMode_CheckedChanged;
            checkBox20_29.CheckedChanged += LabelMode_CheckedChanged;
            checkBox30_39.CheckedChanged += LabelMode_CheckedChanged;
            checkBox40_49.CheckedChanged += LabelMode_CheckedChanged;
            checkBox50_59.CheckedChanged += LabelMode_CheckedChanged;
            checkBox60_69.CheckedChanged += LabelMode_CheckedChanged;
            checkBox70_79.CheckedChanged += LabelMode_CheckedChanged;
            checkBox80_89.CheckedChanged += LabelMode_CheckedChanged;
            checkBox90_99.CheckedChanged += LabelMode_CheckedChanged;
            AllCheckBox.CheckedChanged += AllCheckBox_CheckedChanged;

            // Add Cross Mode checkbox event
            checkBoxCP.CheckedChanged += checkBoxCP_CheckedChanged;
        }
        // Handle Cross Mode toggle
        private void checkBoxCP_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxCP.Checked)
            {
                // Uncheck other modes to avoid conflicts
                checkBoxFP.Checked = false;
                checkBoxEVEN.Checked = false;
                checkBoxODD.Checked = false;
            }
        }

        private void HandleHorizontalLabelClick(int horizontalIndex)
        {
            // Set labelOffset based on clicked label (e.g., 3 → 3000)
            labelOffset = horizontalIndex * 1000;

            // Update vertical range checkbox labels to reflect offset
            UpdateVerticalCheckboxLabels();

            // Optionally: visually deselect old label and highlight new one
            if (_lastClickedLabel != null)
                _lastClickedLabel.BackColor = Color.Transparent;

            Label clickedLabel = this.Controls.Find($"label{horizontalIndex * 10}_{horizontalIndex * 10 + 9}", true).FirstOrDefault() as Label;
            if (clickedLabel != null)
            {
                clickedLabel.BackColor = Color.LightGreen;
                _lastClickedLabel = clickedLabel;
            }

            // If no vertical checkbox is checked, default to 0–99
            if (!checkBox0_99.Checked &&
                !checkBox100_199.Checked &&
                !checkBox200_299.Checked &&
                !checkBox300_399.Checked &&
                !checkBox400_499.Checked &&
                !checkBox500_599.Checked &&
                !checkBox600_699.Checked &&
                !checkBox700_799.Checked &&
                !checkBox800_899.Checked &&
                !checkBox900_999.Checked)
            {
                checkBox0_99.Checked = true;
            }

            ReloadTextBoxesWithOffset();
        }


        // This method gets the current draw slot
        private DateTime GetCurrentDrawSlot(DateTime now)
        {
            DateTime start = now.Date.AddHours(8);    // 08:00 AM
            DateTime end = now.Date.AddHours(22);     // 10:00 PM

            if (now < start)
                return start;

            if (now >= end)
                return end;

            int elapsedMinutes = (int)(now - start).TotalMinutes;
            int completedSlots = elapsedMinutes / 15;

            return start.AddMinutes(completedSlots * 15);
        }

        // This method gets the next draw slot
        private DateTime GetNextDrawSlot(DateTime now)
        {
            DateTime start = now.Date.AddHours(8);    // 08:00 AM
            DateTime end = now.Date.AddHours(22);     // 10:00 PM

            if (now < start)
                return start;

            if (now >= end)
                return start.AddDays(1); // Tomorrow 08:00 AM

            int elapsedMinutes = (int)(now - start).TotalMinutes;
            int nextSlotIndex = (elapsedMinutes / 15) + 1;

            return start.AddMinutes(nextSlotIndex * 15);
        }

        // This method restricts input to digits only
        private void textBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Allow only digits and control keys
            e.Handled = !char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar);
        }
        private void inputValidation()
        {
            for (int i = 0; i < 100; i++)
            {
                // Attach TextChanged event for all 100 grid boxes
                TextBox tb = this.Controls.Find("textBox" + i, true).FirstOrDefault() as TextBox;
                if (tb != null)
                {
                    tb.TextChanged += textBox_TextChanged;
                    tb.KeyPress += textBox_KeyPress; // restrict input
                }
            }
        }

        private Label _lastClickedLabel = null;
        private void HandleLabelClick(int startIndex)
        {
            isLabelViewMode = true;
            isAllModeEnabled = false;
            SaveCurrentValues(); // Save before load
            LoadTextBoxes(labelOffset + startIndex);  // Ensure correct offset before saving/loading

            if (_lastClickedLabel != null)
                _lastClickedLabel.BackColor = Color.Transparent;

            Label clicked = this.Controls.Find($"labelCheck{startIndex}_{startIndex + 99}", true).FirstOrDefault() as Label;
            if (clicked != null)
            {
                clicked.BackColor = Color.LightBlue;
                _lastClickedLabel = clicked;
            }
        }
        private DateTime GetNextDrawTime(DateTime currentTime)
        {
            // Set start and end draw times
            DateTime start = currentTime.Date.AddHours(8);   // 08:00 AM
            DateTime end = currentTime.Date.AddHours(22);    // 10:00 PM
            // Check if current time is before start or after end
            if (currentTime < start)
                return start;
            // Check if current time is after end
            if (currentTime >= end)
                return start.AddDays(1); // next day
            // Calculate next 15-minute interval
            int minutes = currentTime.Minute;
            int nextInterval = ((minutes / 15) + 1) * 15;
            // Check if next interval exceeds 60 minutes
            if (nextInterval == 60)
                return new DateTime(currentTime.Year, currentTime.Month, currentTime.Day, currentTime.Hour + 1, 0, 0);
            else
                return new DateTime(currentTime.Year, currentTime.Month, currentTime.Day, currentTime.Hour, nextInterval, 0);
        }
        private DateTime _lastProcessedDrawTime = DateTime.MinValue;

        private void clockTimer_Tick(object sender, EventArgs e)
        {
            DateTime now = DateTime.Now;
            timeResult.Text = "Current Time: " + now.ToString("hh:mm:ss tt");

            DateTime nextDraw = GetNextDrawTime(now);
            nextDrawLable.Text = "Next Draw: " + nextDraw.ToString("hh:mm tt");

            TimeSpan remaining = nextDraw - now;
            if (remaining.TotalSeconds < 0) remaining = TimeSpan.Zero;
            RTimeResult.Text = "Remaining Time: " + remaining.ToString(@"hh\:mm\:ss");

            // Get the draw slot that JUST completed (15 minutes before the next draw)
            DateTime currentSlot = GetCurrentDrawSlot(DateTime.Now);

            // Trigger result declaration when countdown reaches 00:00:00
            if (_lastProcessedDrawTime != currentSlot && remaining.TotalSeconds <= 1)
            {
                Console.WriteLine($"[DEBUG] Processing draw for slot: {currentSlot:HH:mm}");

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string query = "SELECT ResultList FROM DrawResults WHERE DrawDate = @date AND DrawTime = @time";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@date", currentSlot.Date);
                        cmd.Parameters.AddWithValue("@time", currentSlot.TimeOfDay);

                        object resultObj = cmd.ExecuteScalar();
                        if (resultObj != null)
                        {
                            string resultCSV = resultObj.ToString();
                            List<string> drawnNumbers = resultCSV.Split(',').ToList();

                            // Process and evaluate winners
                            DateTime targetNextDraw = currentSlot.AddMinutes(15);
                            ProcessWinners(targetNextDraw, drawnNumbers);
                            EvaluateWinners(targetNextDraw, drawnNumbers);
                            LoadLatestResults(); // Refresh UI

                            Console.WriteLine($"[INFO] Results processed for {currentSlot:HH:mm}");
                        }
                        else
                        {
                            MessageBox.Show($"No results found for draw at {currentSlot:HH:mm}.", "Missing Results", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                }

                _lastProcessedDrawTime = currentSlot; // Prevent reprocessing same slot
            }
            headlineLable.Left -= 30; // Move label to the left

            // If the label has completely moved out of view, reset it to the right
            if (headlineLable.Right < 0)
            {
                headlineLable.Left = this.Width;
            }
        }

        private void logoutButton_Click(object sender, EventArgs e)
        {
            this.Hide();
            LoginForm loginForm = new LoginForm();
            loginForm.Show();
        }
        private void GenerateAndSaveAllDrawsForToday()
        {
            DateTime today = DateTime.Today;
            TimeSpan start = TimeSpan.FromHours(8);   // NEW: Start at 08:00 
            TimeSpan end = TimeSpan.FromHours(22);    // End at 10:00 PM
            int totalDraws = 56; // number of draws between 08:15 and 22:00 (inclusive)

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // Check if today's results already exist
                string checkQuery = "SELECT COUNT(*) FROM DrawResults WHERE DrawDate = @date";
                using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn))
                {
                    checkCmd.Parameters.AddWithValue("@date", today);
                    int existingCount = (int)checkCmd.ExecuteScalar();

                    if (existingCount >= totalDraws)
                        return; // Already generated
                }

                Random rnd = new Random();

                // Step 1: Pre-generate 56 unique numbers for each of the 100 blocks
                // Dictionary: blockIndex -> List of unique numbers (strings)
                Dictionary<int, List<string>> blockNumbers = new Dictionary<int, List<string>>();

                for (int block = 0; block < 100; block++)
                {
                    int rangeStart = block * 100;
                    int rangeEnd = rangeStart + 99;

                    HashSet<int> uniqueNums = new HashSet<int>();
                    List<string> blockList = new List<string>();

                    while (uniqueNums.Count < totalDraws)
                    {
                        int candidate = rnd.Next(rangeStart, rangeEnd + 1);
                        if (!uniqueNums.Contains(candidate))
                        {
                            uniqueNums.Add(candidate);
                            blockList.Add(candidate.ToString("D4"));
                        }
                    }

                    // Shuffle blockList so draws get random distribution
                    blockList = blockList.OrderBy(x => rnd.Next()).ToList();

                    blockNumbers[block] = blockList;
                }

                // Step 2: Construct draws - each draw gets the i-th number from every block
                List<(DateTime drawTime, string resultCSV)> allResults = new List<(DateTime, string)>();
                TimeSpan drawTime = start;

                for (int drawIndex = 0; drawIndex < totalDraws; drawIndex++)
                {
                    List<string> drawResults = new List<string>();

                    for (int block = 0; block < 100; block++)
                    {
                        drawResults.Add(blockNumbers[block][drawIndex]);
                    }

                    string resultCSV = string.Join(",", drawResults);
                    DateTime drawDateTime = today.Add(drawTime);
                    allResults.Add((drawDateTime, resultCSV));
                    drawTime = drawTime.Add(TimeSpan.FromMinutes(15));
                }

                // Step 3: Insert all draws into DB
                foreach (var (dt, csv) in allResults)
                {
                    string insertQuery = @"INSERT INTO DrawResults (DrawDate, DrawTime, ResultList)
                                   VALUES (@date, @time, @results)";
                    using (SqlCommand insertCmd = new SqlCommand(insertQuery, conn))
                    {
                        insertCmd.Parameters.AddWithValue("@date", dt.Date);
                        insertCmd.Parameters.AddWithValue("@time", dt.TimeOfDay);
                        insertCmd.Parameters.AddWithValue("@results", csv);
                        insertCmd.ExecuteNonQuery();
                    }
                }
            }
        }
        private void LoadLatestResults()
        {
            DateTime now = DateTime.Now;

            // Step back 15 mins to get last completed draw
            DateTime slot = now.AddMinutes(-15); // look back to last draw
            int roundedMinutes = (slot.Minute / 15) * 15;
            DateTime drawSlot = new DateTime(slot.Year, slot.Month, slot.Day, slot.Hour, roundedMinutes, 0);

            // Only allow draw times between 08:00 AM and 10:00 PM
            if (drawSlot.TimeOfDay < TimeSpan.FromHours(8) || drawSlot.TimeOfDay > TimeSpan.FromHours(22))
            {
                ClearResultTextBoxes();
                return;
            }

            DateTime drawDate = drawSlot.Date;
            TimeSpan drawTime = drawSlot.TimeOfDay;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT ResultList FROM DrawResults WHERE DrawDate = @date AND DrawTime = @time";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@date", drawDate);
                    cmd.Parameters.AddWithValue("@time", drawTime);

                    object resultObj = cmd.ExecuteScalar();

                    if (resultObj != null)
                    {
                        string resultCSV = resultObj.ToString();
                        string[] results = resultCSV.Split(',');

                        if (results.Length >= 10)
                        {
                            textBoxFResult1.Text = results[0];
                            textBoxFResult2.Text = results[1];
                            textBoxFResult3.Text = results[2];
                            textBoxFResult4.Text = results[3];
                            textBoxFResult5.Text = results[4];
                            textBoxFResult6.Text = results[5];
                            textBoxFResult7.Text = results[6];
                            textBoxFResult8.Text = results[7];
                            textBoxFResult9.Text = results[8];
                            textBoxFResult10.Text = results[9];
                        }
                        else
                        {
                            ClearResultTextBoxes();
                        }
                    }
                    else
                    {
                        ClearResultTextBoxes();
                    }
                }
            }
        }

        // Clear the result text boxes
        private void ClearResultTextBoxes()
        {
            textBoxFResult1.Text = textBoxFResult2.Text = textBoxFResult3.Text =
            textBoxFResult4.Text = textBoxFResult5.Text = textBoxFResult6.Text =
            textBoxFResult7.Text = textBoxFResult8.Text = textBoxFResult9.Text =
            textBoxFResult10.Text = "";
        }
        private void SetTextBoxesEvenOdd(bool enableEven, bool enableOdd)
        {
            // Step 1: Update logic for allValues (0–999)
            for (int i = 0; i < 1000; i++)
            {
                bool isEven = i % 2 == 0;
                bool shouldEnable = (enableEven && isEven) || (enableOdd && !isEven);

                if (!shouldEnable)
                {
                    allValues[i] = ""; // Clear disabled values
                }
            }

            // Step 2: Apply changes only to currently visible 100 textBoxes
            for (int i = 0; i < 100; i++)
            {
                int realNumber = currentStart + i;

                TextBox textBox = panelContainer.Controls.Find("textBox" + i, true).FirstOrDefault() as TextBox;
                if (textBox != null)
                {
                    bool isEven = realNumber % 2 == 0;
                    bool shouldEnable = (enableEven && isEven) || (enableOdd && !isEven);

                    textBox.ReadOnly = !shouldEnable;
                    textBox.BackColor = shouldEnable ? Color.White : Color.Gray;

                    if (!shouldEnable)
                    {
                        textBox.TextChanged -= textBox_TextChanged;
                        textBox.Clear(); // Clear the textbox visually
                        textBox.TextChanged += textBox_TextChanged;
                    }
                }
            }

            // Step 3: Recalculate totals
            UpdateRowSummaries();
            UpdateTotalSummary();
        }
        private void checkBoxEVEN_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxEVEN.Checked)
            {
                checkBoxODD.Checked = false;
                SetTextBoxesEvenOdd(true, false);
            }
            else
            {
                SetTextBoxesEvenOdd(true, true); // Enable all if neither is checked
            }
        }

        private void checkBoxODD_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxODD.Checked)
            {
                checkBoxEVEN.Checked = false;
                SetTextBoxesEvenOdd(false, true);
            }
            else
            {
                SetTextBoxesEvenOdd(true, true); // Enable all if neither is checked
            }
        }
        private bool isAllModeEnabled = false;
        private void checkBoxAll_CheckedChanged(object sender, EventArgs e)
        {
            isLabelViewMode = false;
            if (checkBoxAll.Checked)
            {
                isAllModeEnabled = true;
                //UncheckOthers(checkBoxAll);

                // Check all range checkboxes
                checkBox0_99.Checked = true;
                checkBox100_199.Checked = true;
                checkBox200_299.Checked = true;
                checkBox300_399.Checked = true;
                checkBox400_499.Checked = true;
                checkBox500_599.Checked = true;
                checkBox600_699.Checked = true;
                checkBox700_799.Checked = true;
                checkBox800_899.Checked = true;
                checkBox900_999.Checked = true;

                LoadTextBoxes(0); // load the first 0–99 block
            }
            else
            {
                isAllModeEnabled = false;

                // Uncheck all range checkboxes
                checkBox0_99.Checked = false;
                checkBox100_199.Checked = false;
                checkBox200_299.Checked = false;
                checkBox300_399.Checked = false;
                checkBox400_499.Checked = false;
                checkBox500_599.Checked = false;
                checkBox600_699.Checked = false;
                checkBox700_799.Checked = false;
                checkBox800_899.Checked = false;
                checkBox900_999.Checked = false;
            }

            UpdateRowSummaries();  // Recalculate with new multiplier
            UpdateTotalSummary();  // In case any changes happened
        }
        // This method is used to handle the check changed event for the range checkboxes
        private void RangeCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (!(sender as CheckBox).Checked)
            {
                checkBoxAll.CheckedChanged -= checkBoxAll_CheckedChanged;
                checkBoxAll.Checked = false;
                checkBoxAll.CheckedChanged += checkBoxAll_CheckedChanged;
            }
            else
            {
                if (checkBox0_99.Checked &&
                    checkBox100_199.Checked &&
                    checkBox200_299.Checked &&
                    checkBox300_399.Checked &&
                    checkBox400_499.Checked &&
                    checkBox500_599.Checked &&
                    checkBox600_699.Checked &&
                    checkBox700_799.Checked &&
                    checkBox800_899.Checked &&
                    checkBox900_999.Checked)
                {
                    checkBoxAll.CheckedChanged -= checkBoxAll_CheckedChanged;
                    checkBoxAll.Checked = true;
                    checkBoxAll.CheckedChanged += checkBoxAll_CheckedChanged;
                }
            }

            RecalculateSummaries();
        }
        private void RecalculateSummaries()
        {
            UpdateRowSummaries();
            UpdateTotalSummary();
        }
        private void checkBox0_99_CheckedChanged(object sender, EventArgs e)
        {
            if (((CheckBox)sender).Checked)
            {
                LoadTextBoxes(0);
            }
            RecalculateSummaries();
            isLabelViewMode = false;
        }
        private void checkBox100_199_CheckedChanged(object sender, EventArgs e)
        {
            if (((CheckBox)sender).Checked)
            {
                LoadTextBoxes(100);
            }
            RecalculateSummaries();
            isLabelViewMode = false;
        }
        private void checkBox200_299_CheckedChanged(object sender, EventArgs e)
        {
            if (((CheckBox)sender).Checked)
            {
                LoadTextBoxes(200);
            }
            RecalculateSummaries();
            isLabelViewMode = false;
        }
        private void checkBox300_399_CheckedChanged(object sender, EventArgs e)
        {
            if (((CheckBox)sender).Checked)
            {
                LoadTextBoxes(300);
            }
            RecalculateSummaries();
            isLabelViewMode = false;
        }
        private void checkBox400_499_CheckedChanged(object sender, EventArgs e)
        {
            if (((CheckBox)sender).Checked)
            {
                LoadTextBoxes(400);
            }
            RecalculateSummaries();
            isLabelViewMode = false;
        }
        private void checkBox500_599_CheckedChanged(object sender, EventArgs e)
        {
            if (((CheckBox)sender).Checked)
            {
                LoadTextBoxes(500);
            }
            RecalculateSummaries();
            isLabelViewMode = false;
        }
        private void checkBox600_699_CheckedChanged(object sender, EventArgs e)
        {
            if (((CheckBox)sender).Checked)
            {
                LoadTextBoxes(600);
            }
            RecalculateSummaries();
            isLabelViewMode = false;
        }
        private void checkBox700_799_CheckedChanged(object sender, EventArgs e)
        {
            if (((CheckBox)sender).Checked)
            {
                LoadTextBoxes(700);
            }
            RecalculateSummaries();
            isLabelViewMode = false;
        }
        private void checkBox800_899_CheckedChanged(object sender, EventArgs e)
        {
            if (((CheckBox)sender).Checked)
            {
                LoadTextBoxes(800);
            }
            RecalculateSummaries();
            isLabelViewMode = false;
        }
        private void checkBox900_999_CheckedChanged(object sender, EventArgs e)
        {
            if (((CheckBox)sender).Checked)
            {
                LoadTextBoxes(900);
            }
            RecalculateSummaries();
            isLabelViewMode = false;
        }

        private Dictionary<int, string> allValues = new Dictionary<int, string>(); // to store values for text boxes
        private int currentStart = 0; // to store the current starting index for text boxes

        // This method is used to initialize the dictionary with empty strings
        private void InitValues()
        {
            for (int i = 0; i < 1000; i++)
            {
                allValues[i] = "";
            }
        }

        // This method saves the current values of the text boxes into the dictionary
        private void SaveCurrentValues()
        {
            for (int i = 0; i < 100; i++)
            {
                TextBox tb = panelContainer.Controls.Find($"textBox{i}", true).FirstOrDefault() as TextBox;
                if (tb != null)
                {
                    int realNumber = labelOffset + currentStart + i;
                    allValues[realNumber] = tb.Text;
                    if (!string.IsNullOrWhiteSpace(tb.Text))
                        userEnteredNumbers.Add(realNumber);
                    else
                        userEnteredNumbers.Remove(realNumber);
                }
            }
        }
        // This method is used to load the values from the dictionary into the text boxes
        private void LoadTextBoxes(int newStart)
        {
            SaveCurrentValues();
            currentStart = newStart;

            for (int i = 0; i < 100; i++)
            {
                Label label = panelContainer.Controls.Find($"label{i}", true).FirstOrDefault() as Label;
                TextBox textbox = panelContainer.Controls.Find($"textBox{i}", true).FirstOrDefault() as TextBox;

                if (label != null && textbox != null)
                {
                    int realNumber = labelOffset + currentStart + i;
                    label.Text = realNumber.ToString("D4");

                    textbox.TextChanged -= textBox_TextChanged;

                    if (allValues.TryGetValue(realNumber, out string val))
                        textbox.Text = val;
                    else
                        textbox.Text = "";

                    // Set read-only for F0–F9 and B0–B9 in Cross Mode
                    if (checkBoxCP.Checked)
                    {
                        int row = i / 10;
                        int col = i % 10;
                        if (row == 0 || col == 0) // Assuming F0–F9 are row 0, B0–B9 are col 0
                            textbox.ReadOnly = true;
                        else
                            textbox.ReadOnly = false;
                    }
                    else
                    {
                        textbox.ReadOnly = false;
                    }

                    textbox.TextChanged += textBox_TextChanged;
                }
            }

            UpdateRowSummaries();
            UpdateTotalSummary();
        }
        // This method is used to update the row summaries based on the values in the text boxes
        private void UpdateRowSummaries()
        {
            for (int block = 0; block < 10; block++)
            {
                int start = labelOffset + (block * 100);
                int blockQty = 0;

                for (int i = 0; i < 100; i++)
                {
                    int number = start + i;

                    if (IsNumberInSelectedRanges(number))
                    {
                        if (allValues.TryGetValue(number, out string val) && int.TryParse(val, out int qty) && qty > 0)
                        {
                            blockQty += qty;
                        }
                    }
                }

                TextBox qtyBox = this.Controls.Find("txtQtyRow" + block, true).FirstOrDefault() as TextBox;
                TextBox amtBox = this.Controls.Find("txtAmtRow" + block, true).FirstOrDefault() as TextBox;

                if (qtyBox != null) qtyBox.Text = blockQty.ToString();
                if (amtBox != null) amtBox.Text = (blockQty * 2).ToString("N0");
            }
        }

        private bool IsNumberInSelectedRanges(int number)
        {
            if (number < 0 || number > 9999) return false;

            if (isAllModeEnabled)
            {
                // Only include if that range's checkbox is checked
                int rangeStart = ((number - labelOffset) / 100) * 100;
                switch (rangeStart)
                {
                    case 0: return checkBox0_99.Checked;
                    case 100: return checkBox100_199.Checked;
                    case 200: return checkBox200_299.Checked;
                    case 300: return checkBox300_399.Checked;
                    case 400: return checkBox400_499.Checked;
                    case 500: return checkBox500_599.Checked;
                    case 600: return checkBox600_699.Checked;
                    case 700: return checkBox700_799.Checked;
                    case 800: return checkBox800_899.Checked;
                    case 900: return checkBox900_999.Checked;
                    default: return false;
                }
            }

            // If not in All mode, only include manually entered values
            return userEnteredNumbers.Contains(number);
        }

        // This method is used to update the total summary based on the values in the text boxes
        private void UpdateTotalSummary()
        {
            int totalQty = 0;

            for (int i = labelOffset; i < labelOffset + 1000; i++)
            {
                if (IsNumberInSelectedRanges(i))
                {
                    if (allValues.TryGetValue(i, out string val) && int.TryParse(val, out int qty) && qty > 0)
                    {
                        totalQty += qty;
                    }
                }
            }

            TextBox totalQtyBox = this.Controls.Find("txtTotalQty", true).FirstOrDefault() as TextBox;
            TextBox totalAmtBox = this.Controls.Find("txtTotalAmt", true).FirstOrDefault() as TextBox;

            int multiplier = _advanceDrawTimes != null && _advanceDrawTimes.Count > 0 ? _advanceDrawTimes.Count : 1;

            if (totalQtyBox != null) totalQtyBox.Text = (totalQty * multiplier).ToString();
            if (totalAmtBox != null) totalAmtBox.Text = ((totalQty * 2) * multiplier).ToString("N0");

        }
        private bool IsVerticalRangeChecked(int index)
        {
            switch (index)
            {
                case 0: return checkBox0_99.Checked;
                case 1: return checkBox100_199.Checked;
                case 2: return checkBox200_299.Checked;
                case 3: return checkBox300_399.Checked;
                case 4: return checkBox400_499.Checked;
                case 5: return checkBox500_599.Checked;
                case 6: return checkBox600_699.Checked;
                case 7: return checkBox700_799.Checked;
                case 8: return checkBox800_899.Checked;
                case 9: return checkBox900_999.Checked;
                default: return false;
            }
        }
        // This method fills a diagonal cross pattern (X shape) based on the input number
        private void ApplyXCrossPattern(int inputNumber, string value)
        {
            // Clear previous pattern if any
            if (lastCPSource.HasValue && lastCPSource.Value != inputNumber)
            {
                ClearCPPattern(lastCPSource.Value);
            }

            // Calculate the position within the 100-number block
            int offset = inputNumber % 100;
            int centerRow = offset / 10;
            int centerCol = offset % 10;

            // Determine which ranges to apply the pattern to
            List<int> ranges = new List<int>();
            if (checkBoxAll.Checked)
            {
                // Add all ranges that are checked
                for (int i = 0; i < 10; i++)
                {
                    if (IsVerticalRangeChecked(i))
                        ranges.Add(i * 100);
                }
            }
            else
            {
                // Apply only to the current range
                int base100 = (inputNumber / 100) * 100;
                ranges.Add(base100);
            }

            // Apply the "x" pattern to each range
            foreach (int base100 in ranges)
            {
                for (int r = 0; r < 10; r++)
                {
                    for (int c = 0; c < 10; c++)
                    {
                        if ((r - c == centerRow - centerCol) || (r + c == centerRow + centerCol))
                        {
                            int num = base100 + r * 10 + c;
                            ApplyToTextBox(num, value);
                        }
                    }
                }
            }

            // Store current source
            if (!string.IsNullOrEmpty(value))
                lastCPSource = inputNumber;
            else if (lastCPSource == inputNumber)
                lastCPSource = null;
        }
        private void ClearCPPattern(int inputNumber)
        {
            int offset = inputNumber % 100;
            int centerRow = offset / 10;
            int centerCol = offset % 10;

            // Determine which ranges to clear
            List<int> ranges = new List<int>();
            if (checkBoxAll.Checked)
            {
                for (int i = 0; i < 10; i++)
                {
                    if (IsVerticalRangeChecked(i))
                        ranges.Add(i * 100);
                }
            }
            else
            {
                int base100 = (inputNumber / 100) * 100;
                ranges.Add(base100);
            }

            // Clear the "x" pattern in each range
            foreach (int base100 in ranges)
            {
                for (int r = 0; r < 10; r++)
                {
                    for (int c = 0; c < 10; c++)
                    {
                        if ((r - c == centerRow - centerCol) || (r + c == centerRow + centerCol))
                        {
                            int num = base100 + r * 10 + c;
                            ApplyToTextBox(num, "");
                        }
                    }
                }
            }
        }
        // This method applies the value to a specific textbox and updates the backing dictionary
        private void ApplyToTextBox(int number, string value)
        {
            // Always update the backing dictionary
            if (allValues.ContainsKey(number))
                allValues[number] = value;
            else
                allValues.Add(number, value);

            // Track whether this was manually entered
            if (!string.IsNullOrWhiteSpace(value))
                userEnteredNumbers.Add(number);
            else
                userEnteredNumbers.Remove(number);

            // Only update the visible textbox if it's on screen
            int relativeIndex = number - (labelOffset + currentStart);
            if (relativeIndex >= 0 && relativeIndex < 100)
            {
                TextBox tb = panelContainer.Controls.Find($"textBox{relativeIndex}", true).FirstOrDefault() as TextBox;
                if (tb != null)
                {
                    tb.TextChanged -= textBox_TextChanged;
                    tb.Text = value;
                    tb.TextChanged += textBox_TextChanged;
                }
            }
        }

        // This method is used to handle the text changed event for the text boxes
        private void textBox_TextChanged(object sender, EventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (tb == null || !tb.Name.StartsWith("textBox")) return;
            if (!int.TryParse(tb.Name.Substring(7), out int index)) return;

            int realNumber = labelOffset + currentStart + index;
            string val = tb.Text;

            allValues[realNumber] = val;
            userEnteredNumbers.Add(realNumber);

            if (!int.TryParse(val, out int inputQty))
                inputQty = 0;

            // ✅ FAMILY POINT MODE
            if (checkBoxFP.Checked && !isLabelViewMode && inputQty > 0)
            {
                int familyBase = realNumber % 100;
                if (familyBase >= 0 && familyBase < 100)
                    FamilyFillingByInput(familyBase, val);
                return;
            }

            // ✅ CROSS MODE
            if (checkBoxCP.Checked && tb.Focused)
            {
                ApplyXCrossPattern(realNumber, val);
                UpdateRowSummaries();
                UpdateTotalSummary();
                return;
            }

            // ✅ NORMAL SPREAD MODE
            int lastTwoDigits = realNumber % 100;

            if (!isLabelViewMode)
            {
                for (int i = 0; i < 10; i++)
                {
                    if (!IsVerticalRangeChecked(i)) continue;

                    int targetNumber = labelOffset + (i * 100) + lastTwoDigits;

                    int relativeIndex = targetNumber - (labelOffset + currentStart);
                    TextBox targetBox = (relativeIndex >= 0 && relativeIndex < 100)
                        ? this.Controls.Find("textBox" + relativeIndex, true).FirstOrDefault() as TextBox
                        : null;

                    if (inputQty > 0)
                    {
                        allValues[targetNumber] = val;
                        userEnteredNumbers.Add(targetNumber);

                        if (targetBox != null && targetBox != tb)
                        {
                            targetBox.TextChanged -= textBox_TextChanged;
                            targetBox.Text = val;
                            targetBox.TextChanged += textBox_TextChanged;
                        }
                    }
                    else // inputQty == 0 → clear spread
                    {
                        allValues[targetNumber] = "";
                        userEnteredNumbers.Remove(targetNumber);

                        if (targetBox != null && targetBox != tb)
                        {
                            targetBox.TextChanged -= textBox_TextChanged;
                            targetBox.Text = "";
                            targetBox.TextChanged += textBox_TextChanged;
                        }
                    }
                }
            }

            UpdateRowSummaries();
            UpdateTotalSummary();
        }

        // This method is used to handle the text changed event for the F0–F9 and B0–B9 text boxes
        private void txtF_TextChanged(object sender, EventArgs e)
        {
            TextBox source = sender as TextBox;
            if (source == null) return;

            int row = int.Parse(source.Name.Replace("txtF", ""));
            string val = source.Text;

            for (int col = 0; col < 10; col++)
            {
                int idx = row * 10 + col;
                TextBox tb = this.Controls.Find("textBox" + idx, true).FirstOrDefault() as TextBox;
                if (tb != null && !tb.ReadOnly)  //  Only update if it's editable
                {
                    tb.Text = val;
                }
            }
        }
        // This method is used to handle the text changed event for the B0–B9 text boxes
        private void txtB_TextChanged(object sender, EventArgs e)
        {
            TextBox source = sender as TextBox;
            if (source == null) return;

            int col = int.Parse(source.Name.Replace("txtB", ""));
            string val = source.Text;

            for (int row = 0; row < 10; row++)
            {
                int idx = row * 10 + col;
                TextBox tb = this.Controls.Find("textBox" + idx, true).FirstOrDefault() as TextBox;
                if (tb != null && !tb.ReadOnly)  //  Only update if it's editable
                {
                    tb.Text = val;
                }
            }
        }
        private void ClearAllInputs()
        {
            userEnteredNumbers.Clear();
            checkBox0_99.Checked = true;
            _advanceDrawTimes.Clear();

            // Clear backing dictionary values
            for (int i = 0; i < 1000; i++)
            {
                allValues[i] = "";
            }
            // Clear textBoxes from textBox0 to textBox999
            for (int i = 0; i <= 999; i++)
            {
                var textBox = this.Controls.Find("textBox" + i, true).FirstOrDefault() as TextBox;
                if (textBox != null)
                {
                    textBox.Clear();
                }
            }

            // Clear quantityTextBox0 to quantityTextBox9
            for (int i = 0; i <= 9; i++)
            {
                var quantityBox = this.Controls.Find("txtQtyRow" + i, true).FirstOrDefault() as TextBox;
                if (quantityBox != null)
                {
                    quantityBox.Clear();
                }

                var amountBox = this.Controls.Find("txtAmtRow" + i, true).FirstOrDefault() as TextBox;
                if (amountBox != null)
                {
                    amountBox.Clear();
                }
            }

            // Clear f0 to f9 and b0 to b9
            for (int i = 0; i <= 9; i++)
            {
                var fBox = this.Controls.Find("txtF" + i, true).FirstOrDefault() as TextBox;
                if (fBox != null)
                {
                    fBox.Clear();
                }

                var bBox = this.Controls.Find("txtB" + i, true).FirstOrDefault() as TextBox;
                if (bBox != null)
                {
                    bBox.Clear();
                }
            }

            // Clear total quantity and amount
            var totalQtyBox = this.Controls.Find("txtTotalQty", true).FirstOrDefault() as TextBox;
            var totalAmtBox = this.Controls.Find("txtTotalAmt", true).FirstOrDefault() as TextBox;

            if (totalQtyBox != null) totalQtyBox.Text = "";
            if (totalAmtBox != null) totalAmtBox.Text = "";

            // Reset selected label (visual)
            if (_lastClickedLabel != null)
            {
                _lastClickedLabel.BackColor = Color.Transparent;
                _lastClickedLabel = null;
            }

            // Reset mode
            isLabelViewMode = false;

            UncheckAllCheckBoxes(this);
            labelOffset = 0;                     // Reset to base range
            UpdateVerticalCheckboxLabels();     // Reflect labels from 0–99 to 900–999
            LoadTextBoxes(0);                   // Load the default 0–99 range

            checkBoxAll.Checked = true;
            checkBoxAll.Checked = false;
        }
        private void UncheckAllCheckBoxes(Control parent)
        {
            foreach (Control ctrl in parent.Controls)
            {
                if (ctrl is CheckBox cb)
                    cb.Checked = false;

                else if (ctrl.HasChildren)
                    UncheckAllCheckBoxes(ctrl); // Recursive for nested containers
            }
        }
        private void refreshButton_Click(object sender, EventArgs e)
        {
            ClearAllInputs();
        }

        private TextBox _selectedTicketBox = null;
        private const decimal PayoutPerMatch = 180m;   // win 180 Rs per matching ticket

        // This method initializes the ticket grid
        private void InitTicketGrid()
        {
            foreach (Control ctrl in panelContainer.Controls)
            {
                if (ctrl is TextBox tb && tb.Name.StartsWith("textBox"))
                {
                    tb.Click += TicketTextBox_Click;
                }
            }
        }

        // This method handles the click event for the ticket text boxes
        private void TicketTextBox_Click(object sender, EventArgs e)
        {
            //// Deselect previous
            if (_selectedTicketBox != null)
                _selectedTicketBox.BackColor = Color.White;

            // Select new
            _selectedTicketBox = (TextBox)sender;
            _selectedTicketBox.BackColor = Color.LightGreen;
        }

        // This method handles the click event for the Buy Now button
        private void buyNowButton_Click(object sender, EventArgs e)
        {
            var ticketsToBuy = new List<(string number, int qty, decimal amt)>();

            foreach (var entry in allValues.Where(kvp => userEnteredNumbers.Contains(kvp.Key)))
            {
                if (int.TryParse(entry.Value, out int qty) && qty > 0)
                {
                    string ticketNumber = entry.Key.ToString("D4");
                    decimal amount = qty * 2;
                    ticketsToBuy.Add((ticketNumber, qty, amount));
                }
            }

            if (ticketsToBuy.Count == 0)
            {
                MessageBox.Show("Please enter at least one valid ticket quantity.", "Input Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int totalQuantity = ticketsToBuy.Sum(t => t.qty);
            int drawSlotCount = _advanceDrawTimes.Any() ? _advanceDrawTimes.Count : 1;
            decimal totalAmount = ticketsToBuy.Sum(t => t.amt) * drawSlotCount;
            decimal perDrawAmount = ticketsToBuy.Sum(t => t.amt);

            if (totalAmount > _balance)
            {
                MessageBox.Show($"Insufficient balance. Required: {totalAmount:N2}, Available: {_balance:N2}", "Insufficient Balance", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string ticketResults = string.Join(",", ticketsToBuy.Select(t => t.number));
            string quantities = string.Join(",", ticketsToBuy.Select(t => t.qty.ToString()));
            string amounts = string.Join(",", ticketsToBuy.Select(t => t.amt.ToString("N0")));

            DateTime nextDraw = GetNextDrawSlot(DateTime.Now);
            TimeSpan ticketTime = DateTime.Now.TimeOfDay;

            //StringBuilder summary = new StringBuilder();
            //summary.AppendLine("Ticket Purchase Summary:");
            //summary.AppendLine("Number\tQty\tAmount");

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlTransaction transaction = conn.BeginTransaction();

                try
                {
                    HashSet<string> existingBarcodes = GetExistingBarcodes(conn, transaction);
                    Random rnd = new Random();

                    _advanceDrawTimes = _advanceDrawTimes
                    .Where(t => DateTime.Today.Add(t) >= DateTime.Now)
                    .ToList();

                    foreach (var drawTime in _advanceDrawTimes.DefaultIfEmpty(nextDraw.TimeOfDay))
                    {
                        DateTime baseDate = DateTime.Today;
                        if (drawTime < DateTime.Now.TimeOfDay && DateTime.Now.Hour >= 22)
                        {
                            baseDate = baseDate.AddDays(1);
                        }

                        DateTime fullDrawDateTime = baseDate.Add(drawTime);
                        string barcode = GenerateUniqueBarcode(existingBarcodes, rnd);

                        using (SqlCommand cmd = conn.CreateCommand())
                        {
                            cmd.Transaction = transaction;
                            cmd.CommandText = @"INSERT INTO TicketPurchases 
                            (UserID, Username, NextDraw, DrawDate, TicketResult, Quantity, Amount, TotalQuantity, TotalAmount, TicketTime, Barcode)
                            VALUES (@uid, @uname, @nd, @dd, @results, @qtys, @amts, @totalQty, @totalAmt, @tt, @bc);";

                            cmd.Parameters.AddWithValue("@uid", _userId);
                            cmd.Parameters.AddWithValue("@uname", _username);
                            cmd.Parameters.AddWithValue("@nd", fullDrawDateTime);
                            cmd.Parameters.AddWithValue("@dd", fullDrawDateTime.Date);
                            cmd.Parameters.AddWithValue("@results", ticketResults);
                            cmd.Parameters.AddWithValue("@qtys", quantities);
                            cmd.Parameters.AddWithValue("@amts", amounts);
                            cmd.Parameters.AddWithValue("@totalQty", totalQuantity);
                            cmd.Parameters.AddWithValue("@totalAmt", perDrawAmount);
                            cmd.Parameters.AddWithValue("@tt", ticketTime);
                            cmd.Parameters.AddWithValue("@bc", barcode);

                            cmd.ExecuteNonQuery();
                        }
                    }

                    using (SqlCommand updateCmd = conn.CreateCommand())
                    {
                        updateCmd.Transaction = transaction;
                        updateCmd.CommandText = "UPDATE users SET balance = balance - @amt WHERE id = @uid;";
                        updateCmd.Parameters.AddWithValue("@amt", totalAmount);
                        updateCmd.Parameters.AddWithValue("@uid", _userId);
                        updateCmd.ExecuteNonQuery();
                    }

                    transaction.Commit();
                    _balance -= totalAmount;
                    balanceLable.Text = _balance.ToString("N2");

                    //foreach (var (number, qty, amt) in ticketsToBuy)
                    //{
                    //    summary.AppendLine($"{number}\t{qty}\t₹{amt:N2}");
                    //}

                    //summary.AppendLine("-----------------------------");
                    //summary.AppendLine($"Total Qty: {totalQuantity}");
                    //summary.AppendLine($"Total Amount: ₹{totalAmount:N2}");
                    //summary.AppendLine($"× {drawSlotCount} Draw Slot(s)");

                    //if (_advanceDrawTimes.Count > 0)
                    //{
                    //    summary.AppendLine("Advance Draw Times:");
                    //    foreach (var dt in _advanceDrawTimes)
                    //    {
                    //        summary.AppendLine($"→ {dt:hh\\:mm}");
                    //    }
                    //}

                    //MessageBox.Show(summary.ToString(), "Purchase Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    // Prepare data for print
                    _printTickets = ticketsToBuy.Select(t => (t.number, t.qty)).ToList();
                    _printTotalQty = totalQuantity;
                    _printTotalAmt = totalAmount;
                    _printUsername = _username;
                    _printUserId = _userId;
                    _printDrawTime = nextDraw.ToString("hh:mmtt");

                    // Trigger print
                    List<TimeSpan> drawSlotsToPrint = _advanceDrawTimes.Any() ? _advanceDrawTimes : new List<TimeSpan> { nextDraw.TimeOfDay };

                    foreach (TimeSpan drawSlot in drawSlotsToPrint)
                    {
                        _printTickets = ticketsToBuy.Select(t => (t.number, t.qty)).ToList();
                        _printTotalQty = totalQuantity;
                        _printTotalAmt = ticketsToBuy.Sum(t => t.amt); // Per-draw amount
                        _printUsername = _username;
                        _printUserId = _userId;
                        _printDrawTime = DateTime.Today.Add(drawSlot).ToString("hh:mmtt");

                        PrintDocument pd = new PrintDocument();
                        pd.PrintPage += new PrintPageEventHandler(PrintReceipt);

                        PrintDialog dlg = new PrintDialog();
                        dlg.Document = pd;

                        if (dlg.ShowDialog() == DialogResult.OK)
                        {
                            pd.Print();
                        }
                    }

                    ClearAllInputs();
                    _advanceDrawTimes.Clear();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    MessageBox.Show("Error while purchasing tickets: " + ex.Message, "Transaction Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // This method retrieves existing barcodes from the database
        private HashSet<string> GetExistingBarcodes(SqlConnection conn, SqlTransaction transaction)
        {
            var existing = new HashSet<string>();
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.Transaction = transaction;
                cmd.CommandTimeout = 60;
                cmd.CommandText = "SELECT Barcode FROM TicketPurchases";

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        existing.Add(reader.GetString(0));
                    }
                }
            }
            return existing;
        }
        // This method generates a unique barcode
        private string GenerateUniqueBarcode(HashSet<string> existingBarcodes, Random rnd)
        {
            string code;
            do
            {
                code = rnd.Next(0, 1000000000).ToString("D10");
            } while (existingBarcodes.Contains(code));
            existingBarcodes.Add(code);
            return code;
        }
        public void ProcessWinners(DateTime drawDateTime, List<string> drawnNumbers)
        {
            decimal userWinningAmount = 0m;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();


                // Step 1: Read all purchases ONLY for this draw time
                var purchases = new List<(int pid, int uid, string[] tickets, string[] qtys, DateTime nextDraw)>();

                DateTime slotStart = drawDateTime;
                DateTime slotEnd = drawDateTime.AddSeconds(59); // precision within 1 minute

                using (SqlCommand selectCmd = new SqlCommand(@"
                        SELECT PurchaseID, UserID, TicketResult, Quantity, NextDraw 
                        FROM TicketPurchases 
                        WHERE NextDraw BETWEEN @slotStart AND @slotEnd
                        AND (WinningResult IS NULL OR WinningResult = '')", conn))
                {
                    selectCmd.Parameters.AddWithValue("@slotStart", slotStart);
                    selectCmd.Parameters.AddWithValue("@slotEnd", slotEnd);

                    using (SqlDataReader reader = selectCmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int pid = reader.GetInt32(0);
                            int uid = reader.GetInt32(1);
                            string[] tickets = reader.GetString(2).Split(',');
                            string[] qtys = reader.GetString(3).Split(',');
                            DateTime nextDraw = reader.GetDateTime(4);

                            // Safety check: skip if draw time mismatches
                            if (nextDraw != drawDateTime)
                                continue;

                            purchases.Add((pid, uid, tickets, qtys, nextDraw));
                        }
                    }
                }

                // Step 2: Process each valid purchase
                foreach (var (pid, uid, tickets, qtys, nextDraw) in purchases)
                {
                    List<string> winResults = new List<string>();
                    List<string> winQuantities = new List<string>();
                    List<string> winAmounts = new List<string>();

                    int totalWinningQty = 0;
                    decimal totalWinningAmt = 0m;

                    for (int i = 0; i < tickets.Length; i++)
                    {
                        string ticket = tickets[i].PadLeft(4, '0');
                        int qty = int.Parse(qtys[i]);

                        if (drawnNumbers.Contains(ticket) && qty > 0)
                        {
                            decimal winAmt = qty * PayoutPerMatch;

                            winResults.Add(ticket);
                            winQuantities.Add(qty.ToString());
                            winAmounts.Add(winAmt.ToString("N0"));

                            totalWinningQty += qty;
                            totalWinningAmt += winAmt;

                        }
                    }

                    // Step 3: Update TicketPurchases if any win
                    using (SqlCommand updateCmd = new SqlCommand(@"
                        UPDATE TicketPurchases 
                        SET WinningResult = @wr, WinningQuantity = @wq, WinningAmount = @wa, 
                            WinningTotalQuantity = @wtq, WinningTotalAmount = @wta 
                        WHERE PurchaseID = @pid", conn))
                    {
                        updateCmd.Parameters.AddWithValue("@wr", string.Join(",", winResults));
                        updateCmd.Parameters.AddWithValue("@wq", string.Join(",", winQuantities));
                        updateCmd.Parameters.AddWithValue("@wa", string.Join(",", winAmounts));
                        updateCmd.Parameters.AddWithValue("@wtq", totalWinningQty);
                        updateCmd.Parameters.AddWithValue("@wta", totalWinningAmt);
                        updateCmd.Parameters.AddWithValue("@pid", pid);

                        updateCmd.ExecuteNonQuery();
                    }

                    // Step 4: Credit user if they won
                    if (totalWinningAmt > 0)
                    {
                        using (SqlCommand creditCmd = new SqlCommand(
                            "UPDATE Users SET Balance = Balance + @amt WHERE Id = @uid", conn))
                        {
                            creditCmd.Parameters.AddWithValue("@amt", totalWinningAmt);
                            creditCmd.Parameters.AddWithValue("@uid", uid);
                            creditCmd.ExecuteNonQuery();
                        }

                        if (uid == _userId)
                        {
                            userWinningAmount += totalWinningAmt;
                        }
                    }
                }

                // Step 5: Show popup for current user if they won
                if (userWinningAmount > 0)
                {
                    _balance = GetUserBalanceFromDb();
                    balanceLable.Text = _balance.ToString("N2");
                }

            }
        }

        private void EvaluateWinners(DateTime drawDateTime, List<string> drawnResults)
        {
            List<string> winningResultList = new List<string>();
            int totalWinningTickets = 0;
            decimal totalWinningAmount = 0m;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                //  Store summary under the slot's starting time (to match DrawResults)
                DateTime resultTime = drawDateTime.AddMinutes(-15);

                // Check if summary already exists for this draw slot
                SqlCommand checkCmd = new SqlCommand("SELECT COUNT(*) FROM WinningSummary WHERE DrawDate = @dd AND DrawTime = @dt", conn);
                checkCmd.Parameters.AddWithValue("@dd", resultTime.Date);
                checkCmd.Parameters.AddWithValue("@dt", resultTime.TimeOfDay);

                int count = (int)checkCmd.ExecuteScalar();
                if (count > 0)
                {
                    // Summary already exists for this slot — skip
                    return;
                }

                // Read ticket purchases for this draw time (NextDraw = drawDateTime)
                DateTime slotStart = drawDateTime;
                DateTime slotEnd = drawDateTime.AddSeconds(59);

                SqlCommand selectCmd = new SqlCommand(@"
            SELECT TicketResult, Quantity 
            FROM TicketPurchases 
            WHERE NextDraw BETWEEN @slotStart AND @slotEnd", conn);
                selectCmd.Parameters.AddWithValue("@slotStart", slotStart);
                selectCmd.Parameters.AddWithValue("@slotEnd", slotEnd);

                using (SqlDataReader reader = selectCmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string[] ticketNumbers = reader.GetString(0).Split(',');  // TicketResult
                        string[] quantities = reader.GetString(1).Split(',');     // Quantity

                        for (int i = 0; i < ticketNumbers.Length; i++)
                        {
                            string ticket = ticketNumbers[i].PadLeft(4, '0');
                            int qty = int.Parse(quantities[i]);

                            if (drawnResults.Contains(ticket))
                            {
                                if (!winningResultList.Contains(ticket))
                                    winningResultList.Add(ticket);

                                totalWinningTickets += qty;
                                totalWinningAmount += qty * 180m;
                            }
                        }
                    }
                }

                //  Insert summary using result slot time (10:00 for a 10:00–10:15 draw)
                SqlCommand insertCmd = new SqlCommand(@"INSERT INTO WinningSummary (DrawDate, DrawTime, WinningResult, TotalWinningTickets, TotalWinningAmount)
                                                        VALUES (@dd, @dt, @res, @tq, @ta)", conn);

                insertCmd.Parameters.AddWithValue("@dd", resultTime.Date);
                insertCmd.Parameters.AddWithValue("@dt", resultTime.TimeOfDay);
                insertCmd.Parameters.AddWithValue("@res", string.Join(",", winningResultList));
                insertCmd.Parameters.AddWithValue("@tq", totalWinningTickets);
                insertCmd.Parameters.AddWithValue("@ta", totalWinningAmount);

                int inserted = insertCmd.ExecuteNonQuery();
                if (inserted == 0)
                {
                    MessageBox.Show("Failed to insert WinningSummary", "Insert Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        // This method retrieves the user's balance from the database
        private decimal GetUserBalanceFromDb()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                conn.Open();
                cmd.CommandText = "SELECT Balance FROM Users WHERE Id = @uid";
                cmd.Parameters.AddWithValue("@uid", _userId);
                return (decimal)cmd.ExecuteScalar();
            }
        }

        private void btnChangePass_Click(object sender, EventArgs e)
        {
            this.Hide();
            ChangePassword changePassword = new ChangePassword(_userId, _username, _balance);
            changePassword.Show();
        }

        private void LabelMode_CheckedChanged(object sender, EventArgs e)
        {
            if (!(sender is CheckBox clickedBox)) return;

            int index = -1;

            if (clickedBox == checkBox0_9) index = 0;
            else if (clickedBox == checkBox10_19) index = 1;
            else if (clickedBox == checkBox20_29) index = 2;
            else if (clickedBox == checkBox30_39) index = 3;
            else if (clickedBox == checkBox40_49) index = 4;
            else if (clickedBox == checkBox50_59) index = 5;
            else if (clickedBox == checkBox60_69) index = 6;
            else if (clickedBox == checkBox70_79) index = 7;
            else if (clickedBox == checkBox80_89) index = 8;
            else if (clickedBox == checkBox90_99) index = 9;

            if (index == -1 || !clickedBox.Checked)
                return;

            // Step 1: Set offset
            labelOffset = index * 1000;

            // Step 2: Update vertical checkbox labels
            UpdateVerticalCheckboxLabels();

            // Step 3: Trigger textbox update
            ReloadTextBoxesWithOffset();

            // Step 4: If no vertical checkbox is checked, select default
            if (!checkBox0_99.Checked &&
                !checkBox100_199.Checked &&
                !checkBox200_299.Checked &&
                !checkBox300_399.Checked &&
                !checkBox400_499.Checked &&
                !checkBox500_599.Checked &&
                !checkBox600_699.Checked &&
                !checkBox700_799.Checked &&
                !checkBox800_899.Checked &&
                !checkBox900_999.Checked)
            {
                checkBox0_99.Checked = true;  // trigger default display
            }
            // Update AllCheckBox based on individual checkbox state
            isUpdatingAllCheckBox = true;
            AllCheckBox.CheckedChanged -= AllCheckBox_CheckedChanged;

            AllCheckBox.Checked = checkBox0_9.Checked &&
                                  checkBox10_19.Checked &&
                                  checkBox20_29.Checked &&
                                  checkBox30_39.Checked &&
                                  checkBox40_49.Checked &&
                                  checkBox50_59.Checked &&
                                  checkBox60_69.Checked &&
                                  checkBox70_79.Checked &&
                                  checkBox80_89.Checked &&
                                  checkBox90_99.Checked;

            AllCheckBox.CheckedChanged += AllCheckBox_CheckedChanged;
            isUpdatingAllCheckBox = false;
        }

        private void UpdateVerticalCheckboxLabels()
        {
            labelCheck0_99.Text = $"{labelOffset + 0}-{labelOffset + 99}";
            labelCheck100_199.Text = $"{labelOffset + 100}-{labelOffset + 199}";
            labelCheck200_299.Text = $"{labelOffset + 200}-{labelOffset + 299}";
            labelCheck300_399.Text = $"{labelOffset + 300}-{labelOffset + 399}";
            labelCheck400_499.Text = $"{labelOffset + 400}-{labelOffset + 499}";
            labelCheck500_599.Text = $"{labelOffset + 500}-{labelOffset + 599}";
            labelCheck600_699.Text = $"{labelOffset + 600}-{labelOffset + 699}";
            labelCheck700_799.Text = $"{labelOffset + 700}-{labelOffset + 799}";
            labelCheck800_899.Text = $"{labelOffset + 800}-{labelOffset + 899}";
            labelCheck900_999.Text = $"{labelOffset + 900}-{labelOffset + 999}";
        }

        private void ReloadTextBoxesWithOffset()
        {
            SaveCurrentValues();

            if (checkBoxAll.Checked)
            {
                LoadTextBoxes(0); // Default to 0–99 when All is checked
                return;
            }

            // Load the first checked range
            if (checkBox0_99.Checked) LoadTextBoxes(0);
            else if (checkBox100_199.Checked) LoadTextBoxes(100);
            else if (checkBox200_299.Checked) LoadTextBoxes(200);
            else if (checkBox300_399.Checked) LoadTextBoxes(300);
            else if (checkBox400_499.Checked) LoadTextBoxes(400);
            else if (checkBox500_599.Checked) LoadTextBoxes(500);
            else if (checkBox600_699.Checked) LoadTextBoxes(600);
            else if (checkBox700_799.Checked) LoadTextBoxes(700);
            else if (checkBox800_899.Checked) LoadTextBoxes(800);
            else if (checkBox900_999.Checked) LoadTextBoxes(900);
            else
            {
                // If no range is checked, default to 0–99
                checkBox0_99.Checked = true;
                LoadTextBoxes(0);
            }
        }
        private void resultButton_Click(object sender, EventArgs e)
        {
            this.Hide();
            Results result = new Results(_userId, _username, _balance);
            result.Show();
        }

        private void accountButton_Click(object sender, EventArgs e)
        {
            this.Hide();
            AccountsForm accountsForm = new AccountsForm(_userId, _username, _balance);
            accountsForm.Show();
        }
        private void AllCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (isUpdatingAllCheckBox) return;

            isUpdatingAllCheckBox = true;

            bool isChecked = AllCheckBox.Checked;

            // 1. Toggle all horizontal checkboxes
            checkBox0_9.Checked = isChecked;
            checkBox10_19.Checked = isChecked;
            checkBox20_29.Checked = isChecked;
            checkBox30_39.Checked = isChecked;
            checkBox40_49.Checked = isChecked;
            checkBox50_59.Checked = isChecked;
            checkBox60_69.Checked = isChecked;
            checkBox70_79.Checked = isChecked;
            checkBox80_89.Checked = isChecked;
            checkBox90_99.Checked = isChecked;

            // 2. If turned ON, apply behavior of first horizontal checkbox (checkBox0_9)
            if (isChecked)
            {
                labelOffset = 0; // because checkBox0_9 means 0–999
                UpdateVerticalCheckboxLabels();

                // If no vertical checkbox is checked, default to checkBox0_99
                if (!checkBox0_99.Checked &&
                    !checkBox100_199.Checked &&
                    !checkBox200_299.Checked &&
                    !checkBox300_399.Checked &&
                    !checkBox400_499.Checked &&
                    !checkBox500_599.Checked &&
                    !checkBox600_699.Checked &&
                    !checkBox700_799.Checked &&
                    !checkBox800_899.Checked &&
                    !checkBox900_999.Checked)
                {
                    checkBox0_99.Checked = true;
                }

                ReloadTextBoxesWithOffset();
            }

            isUpdatingAllCheckBox = false;
        }

        private List<TimeSpan> _advanceDrawTimes = new List<TimeSpan>();

        private void btnAdvanceDraw_Click(object sender, EventArgs e)
        {
            AdvanceDrawFrom adf = new AdvanceDrawFrom();
            if (adf.ShowDialog() == DialogResult.OK)
            {
                _advanceDrawTimes = adf.GetSelectedDrawTimes();

                //if (_advanceDrawTimes.Count == 0)
                //{
                //    MessageBox.Show("No draw time selected!", "Advance Draw", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //}
                //else
                //{
                //    // OPTIONAL: show on screen
                //    lblSelectedDraws.Text = "Selected Slots: " +
                //        string.Join(", ", _advanceDrawTimes.Select(t => t.ToString(@"hh\:mm")));

                //    MessageBox.Show($"{_advanceDrawTimes.Count} draw(s) selected.", "Advance Draw Selected", MessageBoxButtons.OK, MessageBoxIcon.Information);
                //}
            }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Hide();
            CancelForm cf = new CancelForm(_userId, _username, _balance);
            cf.Show();
        }

        private void reprintButton_Click(object sender, EventArgs e)
        {
            this.Hide();
            RePrintForm rp = new RePrintForm(_userId, _username, _balance);
            rp.Show();
        }
        private void PrintReceipt(object sender, PrintPageEventArgs e)
        {
            Font headerFont = new Font("Consolas", 14, FontStyle.Bold);
            Font bodyFont = new Font("Consolas", 10);
            float y = 20;
            float left = 20;
            float lineHeight = bodyFont.GetHeight(e.Graphics) + 4;

            e.Graphics.DrawString("Malamal Dally", headerFont, Brushes.Black, left, y);
            y += lineHeight * 2;

            e.Graphics.DrawString($"DATE : {DateTime.Now:yyyy-MM-dd}", bodyFont, Brushes.Black, left, y); y += lineHeight;
            e.Graphics.DrawString($"TIME : {DateTime.Now:hh:mm:ss tt}", bodyFont, Brushes.Black, left, y); y += lineHeight;
            e.Graphics.DrawString($"Draw Time : {_printDrawTime}", bodyFont, Brushes.Black, left, y); y += lineHeight;
            e.Graphics.DrawString($"UserName : {_printUsername}", bodyFont, Brushes.Black, left, y); y += lineHeight;
            e.Graphics.DrawString($"UserID : {_printUserId}", bodyFont, Brushes.Black, left, y); y += lineHeight;

            y += lineHeight;
            e.Graphics.DrawString(new string('-', 40), bodyFont, Brushes.Black, left, y); y += lineHeight;

            // Print ticket entries: 3 per line
            int count = 0;
            StringBuilder line = new StringBuilder();
            foreach (var (number, qty) in _printTickets)
            {
                line.Append($"{number}-{qty},");
                count++;

                if (count % 3 == 0)
                {
                    e.Graphics.DrawString(line.ToString(), bodyFont, Brushes.Black, left, y);
                    y += lineHeight;
                    line.Clear();
                }
            }

            // Print the remaining items
            if (line.Length > 0)
            {
                e.Graphics.DrawString(line.ToString(), bodyFont, Brushes.Black, left, y);
                y += lineHeight;
            }

            y += 2;
            e.Graphics.DrawString(new string('-', 40), bodyFont, Brushes.Black, left, y); y += lineHeight;

            e.Graphics.DrawString($"Quantity :- {_printTotalQty}", bodyFont, Brushes.Black, left, y); y += lineHeight;
            e.Graphics.DrawString($"Total Amount :- Rs{_printTotalAmt}", bodyFont, Brushes.Black, left, y);
        }
        private void FamilyFillingByInput(int familyNumber, string qty)
        {
            string[] farr = new string[]
            {
                "00,50,05,55", "01,10,60,06,56,65,51,15", "02,20,07,57,75,25,52,70", "03,30,80,08,58,85,35,53",
                "04,40,90,09,59,95,45,54", "05,50,00,55", "06,60,51,56,65,10,01,15", "07,70,57,75,20,02,25,52",
                "08,80,53,58,85,30,03,35", "09,90,59,95,40,04,45,54", "10,56,65,01,60,06,51,15", "11,16,61,66",
                "12,67,76,21,17,71,62,26", "13,68,86,31,18,81,36,63", "14,69,96,41,19,91,46,64", "15,60,06,51,65,56,01,10",
                "16,66,61,11", "17,67,76,21,12,71,62,26", "18,13,31,86,68,63,36,81", "19,14,41,91,46,64,96,69",
                "20,02,70,07,25,52,75,57", "21,12,67,76,17,71,26,62", "22,72,27,77", "23,78,87,32,73,28,82,37",
                "24,79,97,42,92,29,47,74", "25,70,07,52,02,20,57,75", "26,71,17,62,21,12,67,76", "27,72,77,22",
                "28,78,87,82,37,73,32,23", "29,74,47,79,97,92,42,24", "30,85,58,03,35,53,80,08", "31,86,68,13,63,36,18,81",
                "32,87,78,23,37,73,28,82", "33,88,38,83", "34,89,98,43,48,84,39,93", "35,80,08,53,85,58,30,03",
                "36,81,18,63,68,86,13,31", "37,82,28,73,78,87,32,23", "38,88,38,83", "39,84,48,89,98,43,34,93",
                "40,95,59,04,90,09,45,54", "41,96,69,14,91,19,46,64", "42,79,97,24,92,29,47,74", "43,98,89,34,93,39,48,84",
                "44,99,94,49", "45,90,09,54,95,59,40,04", "46,91,19,64,96,69,41,14", "47,92,29,74,97,79,42,24",
                "48,93,39,84,98,89,43,34", "49,99,49,94", "50,05,55,00", "51,06,60,15,10,01,65,56", "52,07,70,25,20,02,75,57",
                "53,08,80,35,30,03,30,58", "54,09,90,45,40,04,95,59", "55,00,05,50", "56,01,10,65,06,60,15,51",
                "57,02,20,75,70,07,25,52", "58,03,30,85,80,08,35,53", "59,04,40,95,90,09,45,54", "60,15,51,06,65,56,10,01",
                "61,16,11,66", "62,17,71,26,67,76,21,12", "63,18,81,36,68,86,31,13", "64,19,91,46,69,96,41,14",
                "65,10,01,56,60,06,51,15", "66,11,61,16", "67,12,21,17,71,26,62,76", "68,13,31,18,81,36,63,86",
                "69,14,41,19,91,46,64,96", "70,25,52,07,75,57,20,02", "71,26,62,17,76,62,21,12", "72,27,77,22",
                "73,28,82,37,78,87,23,32", "74,29,92,47,79,97,24,42", "75,20,02,57,70,07,25,52", "76,21,12,62,26,17,71,67",
                "77,22,72,27", "78,23,32,37,73,28,82,87", "79,24,42,47,74,29,92,97", "80,35,53,08,85,58,30,03",
                "81,36,63,18,86,68,31,13", "82,37,73,28,87,78,32,23", "83,33,38,88", "84,39,93,48,89,98,34,43",
                "85,30,03,35,53,80,08,58", "86,31,13,68,63,36,18,81", "87,32,23,78,73,37,28,82", "88,33,83,38",
                "89,34,43,98,93,39,48,84", "90,45,54,09,95,59,04,40", "91,46,64,19,96,69,14,41", "92,47,74,29,97,79,24,42",
                "93,98,89,39,48,84,34,43", "94,44,99,49", "95,40,04,59,90,09,45,54", "96,41,14,69,96,19,46,64",
                "97,42,24,79,92,29,47,74", "98,43,34,89,93,39,48,84", "99,44,94,49"
            };
            if (familyNumber < 0 || familyNumber >= farr.Length || string.IsNullOrEmpty(qty)) return;

            string[] members = farr[familyNumber].Split(',');

            for (int vertical = 0; vertical <= 9; vertical++)
            {
                if (!IsVerticalRangeChecked(vertical)) continue;

                foreach (string raw in members)
                {
                    if (!int.TryParse(raw, out int num)) continue;

                    int fullNumber = labelOffset + (vertical * 100) + num;
                    allValues[fullNumber] = qty;
                    userEnteredNumbers.Add(fullNumber);

                    int relIndex = fullNumber - (labelOffset + currentStart);
                    if (relIndex >= 0 && relIndex < 100)
                    {
                        var tb = panelContainer.Controls.Find("textBox" + relIndex, true).FirstOrDefault() as TextBox;
                        if (tb != null)
                        {
                            tb.TextChanged -= textBox_TextChanged;
                            tb.Text = qty;
                            tb.TextChanged += textBox_TextChanged;
                        }
                    }
                }
            }

            UpdateRowSummaries();
            UpdateTotalSummary();
        }
    }
}