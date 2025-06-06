namespace FinalTask
{
    partial class LoginForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LoginForm));
            this.loginLable = new System.Windows.Forms.Label();
            this.userLable = new System.Windows.Forms.Label();
            this.passwordLable = new System.Windows.Forms.Label();
            this.userName_textBox = new System.Windows.Forms.TextBox();
            this.password_textBox = new System.Windows.Forms.TextBox();
            this.loginButton = new System.Windows.Forms.Button();
            this.CharShowButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // loginLable
            // 
            this.loginLable.AutoSize = true;
            this.loginLable.BackColor = System.Drawing.Color.Transparent;
            this.loginLable.Font = new System.Drawing.Font("Times New Roman", 16.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.loginLable.ForeColor = System.Drawing.Color.White;
            this.loginLable.Location = new System.Drawing.Point(466, 223);
            this.loginLable.Name = "loginLable";
            this.loginLable.Size = new System.Drawing.Size(85, 32);
            this.loginLable.TabIndex = 0;
            this.loginLable.Text = "Login";
            // 
            // userLable
            // 
            this.userLable.AutoSize = true;
            this.userLable.BackColor = System.Drawing.Color.Transparent;
            this.userLable.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.userLable.ForeColor = System.Drawing.Color.White;
            this.userLable.Location = new System.Drawing.Point(296, 308);
            this.userLable.Name = "userLable";
            this.userLable.Size = new System.Drawing.Size(99, 22);
            this.userLable.TabIndex = 1;
            this.userLable.Text = "Username :";
            // 
            // passwordLable
            // 
            this.passwordLable.AutoSize = true;
            this.passwordLable.BackColor = System.Drawing.Color.Transparent;
            this.passwordLable.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.passwordLable.ForeColor = System.Drawing.Color.White;
            this.passwordLable.Location = new System.Drawing.Point(296, 362);
            this.passwordLable.Name = "passwordLable";
            this.passwordLable.Size = new System.Drawing.Size(99, 22);
            this.passwordLable.TabIndex = 2;
            this.passwordLable.Text = "Password :";
            // 
            // userName_textBox
            // 
            this.userName_textBox.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.userName_textBox.Location = new System.Drawing.Point(424, 300);
            this.userName_textBox.Name = "userName_textBox";
            this.userName_textBox.Size = new System.Drawing.Size(257, 30);
            this.userName_textBox.TabIndex = 3;
            // 
            // password_textBox
            // 
            this.password_textBox.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.password_textBox.Location = new System.Drawing.Point(424, 354);
            this.password_textBox.Name = "password_textBox";
            this.password_textBox.Size = new System.Drawing.Size(225, 30);
            this.password_textBox.TabIndex = 4;
            this.password_textBox.UseSystemPasswordChar = true;
            // 
            // loginButton
            // 
            this.loginButton.BackColor = System.Drawing.Color.Coral;
            this.loginButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.loginButton.Font = new System.Drawing.Font("Times New Roman", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.loginButton.Location = new System.Drawing.Point(300, 425);
            this.loginButton.Name = "loginButton";
            this.loginButton.Size = new System.Drawing.Size(95, 42);
            this.loginButton.TabIndex = 5;
            this.loginButton.Text = "Login";
            this.loginButton.UseVisualStyleBackColor = false;
            this.loginButton.Click += new System.EventHandler(this.loginButton_Click);
            // 
            // CharShowButton
            // 
            this.CharShowButton.Image = global::FinalTask.Properties.Resources.open_eye;
            this.CharShowButton.Location = new System.Drawing.Point(645, 354);
            this.CharShowButton.Name = "CharShowButton";
            this.CharShowButton.Size = new System.Drawing.Size(36, 30);
            this.CharShowButton.TabIndex = 6;
            this.CharShowButton.UseVisualStyleBackColor = true;
            this.CharShowButton.Click += new System.EventHandler(this.CharShowButton_Click);
            // 
            // LoginForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(1006, 721);
            this.Controls.Add(this.CharShowButton);
            this.Controls.Add(this.loginButton);
            this.Controls.Add(this.password_textBox);
            this.Controls.Add(this.userName_textBox);
            this.Controls.Add(this.passwordLable);
            this.Controls.Add(this.userLable);
            this.Controls.Add(this.loginLable);
            this.Name = "LoginForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "LoginForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label loginLable;
        private System.Windows.Forms.Label userLable;
        private System.Windows.Forms.Label passwordLable;
        private System.Windows.Forms.TextBox userName_textBox;
        private System.Windows.Forms.TextBox password_textBox;
        private System.Windows.Forms.Button loginButton;
        private System.Windows.Forms.Button CharShowButton;
    }
}