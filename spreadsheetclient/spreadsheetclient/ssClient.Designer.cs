namespace spreadsheetclient
{
    partial class ssClient
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
            this.ServerIPLabel = new System.Windows.Forms.Label();
            this.NameLabel = new System.Windows.Forms.Label();
            this.ServerIPTextBox = new System.Windows.Forms.TextBox();
            this.LoginNameTextBox = new System.Windows.Forms.TextBox();
            this.ConnectButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // ServerIPLabel
            // 
            this.ServerIPLabel.AutoSize = true;
            this.ServerIPLabel.Location = new System.Drawing.Point(44, 86);
            this.ServerIPLabel.Name = "ServerIPLabel";
            this.ServerIPLabel.Size = new System.Drawing.Size(54, 13);
            this.ServerIPLabel.TabIndex = 9;
            this.ServerIPLabel.Text = "Server IP:";
            // 
            // NameLabel
            // 
            this.NameLabel.AutoSize = true;
            this.NameLabel.Location = new System.Drawing.Point(28, 59);
            this.NameLabel.Name = "NameLabel";
            this.NameLabel.Size = new System.Drawing.Size(67, 13);
            this.NameLabel.TabIndex = 8;
            this.NameLabel.Text = "Login Name:";
            // 
            // ServerIPTextBox
            // 
            this.ServerIPTextBox.Location = new System.Drawing.Point(105, 83);
            this.ServerIPTextBox.Name = "ServerIPTextBox";
            this.ServerIPTextBox.Size = new System.Drawing.Size(98, 20);
            this.ServerIPTextBox.TabIndex = 7;
            this.ServerIPTextBox.Text = "localhost";
            this.ServerIPTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.ServerIPTextBox.TextChanged += new System.EventHandler(this.ServerIPTextBox_TextChanged);
            // 
            // LoginNameTextBox
            // 
            this.LoginNameTextBox.Location = new System.Drawing.Point(104, 56);
            this.LoginNameTextBox.Name = "LoginNameTextBox";
            this.LoginNameTextBox.Size = new System.Drawing.Size(100, 20);
            this.LoginNameTextBox.TabIndex = 6;
            this.LoginNameTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.LoginNameTextBox.TextChanged += new System.EventHandler(this.LoginNameTextBox_TextChanged);
            // 
            // ConnectButton
            // 
            this.ConnectButton.Location = new System.Drawing.Point(76, 114);
            this.ConnectButton.Margin = new System.Windows.Forms.Padding(2);
            this.ConnectButton.Name = "ConnectButton";
            this.ConnectButton.Size = new System.Drawing.Size(157, 22);
            this.ConnectButton.TabIndex = 5;
            this.ConnectButton.Text = "Connect";
            this.ConnectButton.UseVisualStyleBackColor = true;
            this.ConnectButton.Click += new System.EventHandler(this.ConnectButton_Click);
            // 
            // ssClient
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.ServerIPLabel);
            this.Controls.Add(this.NameLabel);
            this.Controls.Add(this.ServerIPTextBox);
            this.Controls.Add(this.LoginNameTextBox);
            this.Controls.Add(this.ConnectButton);
            this.Name = "ssClient";
            this.Text = "Spreadsheet Client";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label ServerIPLabel;
        private System.Windows.Forms.Label NameLabel;
        private System.Windows.Forms.TextBox ServerIPTextBox;
        private System.Windows.Forms.TextBox LoginNameTextBox;
        private System.Windows.Forms.Button ConnectButton;
    }
}

