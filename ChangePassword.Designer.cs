namespace FinalTask
{
    partial class ChangePassword
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ChangePassword));
            this.HeadLable = new System.Windows.Forms.Label();
            this.labelUsername = new System.Windows.Forms.Label();
            this.textBoxUsername = new System.Windows.Forms.TextBox();
            this.labelPass1 = new System.Windows.Forms.Label();
            this.textBoxPass = new System.Windows.Forms.TextBox();
            this.labelPass2 = new System.Windows.Forms.Label();
            this.textBoxPassCon = new System.Windows.Forms.TextBox();
            this.ChangePassBtn = new System.Windows.Forms.Button();
            this.btnBack = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // HeadLable
            // 
            this.HeadLable.AutoSize = true;
            this.HeadLable.BackColor = System.Drawing.Color.Transparent;
            this.HeadLable.Font = new System.Drawing.Font("Times New Roman", 16.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.HeadLable.ForeColor = System.Drawing.Color.White;
            this.HeadLable.Location = new System.Drawing.Point(401, 232);
            this.HeadLable.Name = "HeadLable";
            this.HeadLable.Size = new System.Drawing.Size(228, 32);
            this.HeadLable.TabIndex = 0;
            this.HeadLable.Text = "Change Password";
            // 
            // labelUsername
            // 
            this.labelUsername.AutoSize = true;
            this.labelUsername.BackColor = System.Drawing.Color.Transparent;
            this.labelUsername.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelUsername.ForeColor = System.Drawing.Color.White;
            this.labelUsername.Location = new System.Drawing.Point(299, 290);
            this.labelUsername.Name = "labelUsername";
            this.labelUsername.Size = new System.Drawing.Size(146, 22);
            this.labelUsername.TabIndex = 1;
            this.labelUsername.Text = "Enter Username :";
            // 
            // textBoxUsername
            // 
            this.textBoxUsername.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxUsername.Location = new System.Drawing.Point(474, 287);
            this.textBoxUsername.Name = "textBoxUsername";
            this.textBoxUsername.Size = new System.Drawing.Size(233, 30);
            this.textBoxUsername.TabIndex = 2;
            // 
            // labelPass1
            // 
            this.labelPass1.AutoSize = true;
            this.labelPass1.BackColor = System.Drawing.Color.Transparent;
            this.labelPass1.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelPass1.ForeColor = System.Drawing.Color.White;
            this.labelPass1.Location = new System.Drawing.Point(299, 342);
            this.labelPass1.Name = "labelPass1";
            this.labelPass1.Size = new System.Drawing.Size(146, 22);
            this.labelPass1.TabIndex = 3;
            this.labelPass1.Text = "Enter Password :";
            // 
            // textBoxPass
            // 
            this.textBoxPass.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxPass.Location = new System.Drawing.Point(474, 339);
            this.textBoxPass.Name = "textBoxPass";
            this.textBoxPass.Size = new System.Drawing.Size(233, 30);
            this.textBoxPass.TabIndex = 4;
            // 
            // labelPass2
            // 
            this.labelPass2.AutoSize = true;
            this.labelPass2.BackColor = System.Drawing.Color.Transparent;
            this.labelPass2.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelPass2.ForeColor = System.Drawing.Color.White;
            this.labelPass2.Location = new System.Drawing.Point(299, 394);
            this.labelPass2.Name = "labelPass2";
            this.labelPass2.Size = new System.Drawing.Size(169, 22);
            this.labelPass2.TabIndex = 5;
            this.labelPass2.Text = "Confirm Password :";
            // 
            // textBoxPassCon
            // 
            this.textBoxPassCon.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxPassCon.Location = new System.Drawing.Point(474, 391);
            this.textBoxPassCon.Name = "textBoxPassCon";
            this.textBoxPassCon.Size = new System.Drawing.Size(233, 30);
            this.textBoxPassCon.TabIndex = 6;
            // 
            // ChangePassBtn
            // 
            this.ChangePassBtn.BackColor = System.Drawing.Color.RosyBrown;
            this.ChangePassBtn.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ChangePassBtn.Location = new System.Drawing.Point(328, 454);
            this.ChangePassBtn.Name = "ChangePassBtn";
            this.ChangePassBtn.Size = new System.Drawing.Size(196, 48);
            this.ChangePassBtn.TabIndex = 7;
            this.ChangePassBtn.Text = "Change Password";
            this.ChangePassBtn.UseVisualStyleBackColor = false;
            this.ChangePassBtn.Click += new System.EventHandler(this.ChangePassBtn_Click);
            // 
            // btnBack
            // 
            this.btnBack.BackColor = System.Drawing.Color.Khaki;
            this.btnBack.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnBack.Location = new System.Drawing.Point(890, 37);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(75, 37);
            this.btnBack.TabIndex = 8;
            this.btnBack.Text = "Back";
            this.btnBack.UseVisualStyleBackColor = false;
            this.btnBack.Click += new System.EventHandler(this.btnBack_Click);
            // 
            // ChangePassword
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(1006, 721);
            this.Controls.Add(this.btnBack);
            this.Controls.Add(this.ChangePassBtn);
            this.Controls.Add(this.textBoxPassCon);
            this.Controls.Add(this.labelPass2);
            this.Controls.Add(this.textBoxPass);
            this.Controls.Add(this.labelPass1);
            this.Controls.Add(this.textBoxUsername);
            this.Controls.Add(this.labelUsername);
            this.Controls.Add(this.HeadLable);
            this.Name = "ChangePassword";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ChangePassword";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label HeadLable;
        private System.Windows.Forms.Label labelUsername;
        private System.Windows.Forms.TextBox textBoxUsername;
        private System.Windows.Forms.Label labelPass1;
        private System.Windows.Forms.TextBox textBoxPass;
        private System.Windows.Forms.Label labelPass2;
        private System.Windows.Forms.TextBox textBoxPassCon;
        private System.Windows.Forms.Button ChangePassBtn;
        private System.Windows.Forms.Button btnBack;
    }
}