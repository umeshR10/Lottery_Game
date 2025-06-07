namespace FinalTask
{
    partial class AccountsForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.HeadLable = new System.Windows.Forms.Label();
            this.HeadPanel = new System.Windows.Forms.Panel();
            this.LinePanel = new System.Windows.Forms.Panel();
            this.filterPanel = new System.Windows.Forms.Panel();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnPrint = new System.Windows.Forms.Button();
            this.btnSubmit = new System.Windows.Forms.Button();
            this.toDatePicker = new System.Windows.Forms.DateTimePicker();
            this.label2 = new System.Windows.Forms.Label();
            this.fromDatePicker = new System.Windows.Forms.DateTimePicker();
            this.label1 = new System.Windows.Forms.Label();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.HeadPanel.SuspendLayout();
            this.filterPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // HeadLable
            // 
            this.HeadLable.AutoSize = true;
            this.HeadLable.Font = new System.Drawing.Font("Times New Roman", 16.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.HeadLable.ForeColor = System.Drawing.Color.Maroon;
            this.HeadLable.Location = new System.Drawing.Point(518, 40);
            this.HeadLable.Name = "HeadLable";
            this.HeadLable.Size = new System.Drawing.Size(124, 32);
            this.HeadLable.TabIndex = 0;
            this.HeadLable.Text = "Accounts";
            // 
            // HeadPanel
            // 
            this.HeadPanel.BackColor = System.Drawing.Color.DarkSeaGreen;
            this.HeadPanel.Controls.Add(this.HeadLable);
            this.HeadPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.HeadPanel.Location = new System.Drawing.Point(0, 0);
            this.HeadPanel.Name = "HeadPanel";
            this.HeadPanel.Size = new System.Drawing.Size(1262, 103);
            this.HeadPanel.TabIndex = 1;
            // 
            // LinePanel
            // 
            this.LinePanel.BackColor = System.Drawing.Color.Maroon;
            this.LinePanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.LinePanel.Location = new System.Drawing.Point(0, 103);
            this.LinePanel.Name = "LinePanel";
            this.LinePanel.Size = new System.Drawing.Size(1262, 10);
            this.LinePanel.TabIndex = 2;
            // 
            // filterPanel
            // 
            this.filterPanel.BackColor = System.Drawing.Color.DarkSeaGreen;
            this.filterPanel.Controls.Add(this.btnClose);
            this.filterPanel.Controls.Add(this.btnPrint);
            this.filterPanel.Controls.Add(this.btnSubmit);
            this.filterPanel.Controls.Add(this.toDatePicker);
            this.filterPanel.Controls.Add(this.label2);
            this.filterPanel.Controls.Add(this.fromDatePicker);
            this.filterPanel.Controls.Add(this.label1);
            this.filterPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.filterPanel.Location = new System.Drawing.Point(0, 113);
            this.filterPanel.Name = "filterPanel";
            this.filterPanel.Size = new System.Drawing.Size(1262, 68);
            this.filterPanel.TabIndex = 3;
            // 
            // btnClose
            // 
            this.btnClose.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClose.Location = new System.Drawing.Point(1130, 23);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(87, 36);
            this.btnClose.TabIndex = 6;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnPrint
            // 
            this.btnPrint.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPrint.Location = new System.Drawing.Point(1013, 23);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(87, 36);
            this.btnPrint.TabIndex = 5;
            this.btnPrint.Text = "Print";
            this.btnPrint.UseVisualStyleBackColor = true;
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // btnSubmit
            // 
            this.btnSubmit.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSubmit.Location = new System.Drawing.Point(895, 23);
            this.btnSubmit.Name = "btnSubmit";
            this.btnSubmit.Size = new System.Drawing.Size(87, 36);
            this.btnSubmit.TabIndex = 4;
            this.btnSubmit.Text = "Submit";
            this.btnSubmit.UseVisualStyleBackColor = true;
            this.btnSubmit.Click += new System.EventHandler(this.btnSubmit_Click);
            // 
            // toDatePicker
            // 
            this.toDatePicker.CalendarFont = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toDatePicker.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toDatePicker.Location = new System.Drawing.Point(506, 24);
            this.toDatePicker.Name = "toDatePicker";
            this.toDatePicker.Size = new System.Drawing.Size(200, 30);
            this.toDatePicker.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(398, 28);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(102, 22);
            this.label2.TabIndex = 2;
            this.label2.Text = "TO DATE :";
            // 
            // fromDatePicker
            // 
            this.fromDatePicker.CalendarFont = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.fromDatePicker.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.fromDatePicker.Location = new System.Drawing.Point(172, 24);
            this.fromDatePicker.Name = "fromDatePicker";
            this.fromDatePicker.Size = new System.Drawing.Size(200, 30);
            this.fromDatePicker.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(35, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(131, 22);
            this.label1.TabIndex = 0;
            this.label1.Text = "FROM DATE :";
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToResizeColumns = false;
            this.dataGridView1.AllowUserToResizeRows = false;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Top;
            this.dataGridView1.Location = new System.Drawing.Point(0, 181);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersWidth = 51;
            this.dataGridView1.RowTemplate.Height = 24;
            this.dataGridView1.Size = new System.Drawing.Size(1262, 540);
            this.dataGridView1.TabIndex = 4;
            // 
            // AccountsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.RosyBrown;
            this.ClientSize = new System.Drawing.Size(1262, 721);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.filterPanel);
            this.Controls.Add(this.LinePanel);
            this.Controls.Add(this.HeadPanel);
            this.Name = "AccountsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "AccountsForm";
            this.Load += new System.EventHandler(this.AccountsForm_Load);
            this.HeadPanel.ResumeLayout(false);
            this.HeadPanel.PerformLayout();
            this.filterPanel.ResumeLayout(false);
            this.filterPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label HeadLable;
        private System.Windows.Forms.Panel HeadPanel;
        private System.Windows.Forms.Panel LinePanel;
        private System.Windows.Forms.Panel filterPanel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DateTimePicker fromDatePicker;
        private System.Windows.Forms.DateTimePicker toDatePicker;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnPrint;
        private System.Windows.Forms.Button btnSubmit;
        private System.Windows.Forms.DataGridView dataGridView1;
    }
}