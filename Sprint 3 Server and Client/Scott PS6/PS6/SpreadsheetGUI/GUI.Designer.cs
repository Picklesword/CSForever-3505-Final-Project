namespace SpreadsheetGUI
{
    partial class GUI
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.undoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.findToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.howToUseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.CellNameTB = new System.Windows.Forms.TextBox();
            this.CellContentTB = new System.Windows.Forms.TextBox();
            this.CellValueTB = new System.Windows.Forms.TextBox();
            this.CellNameLabel = new System.Windows.Forms.Label();
            this.CellContentLabel = new System.Windows.Forms.Label();
            this.CellValueLabel = new System.Windows.Forms.Label();
            this.UpdateButton = new System.Windows.Forms.Button();
            this.spreadsheetPanel1 = new SS.SpreadsheetPanel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.SConsollabel = new System.Windows.Forms.Label();
            this.ServertextBox = new System.Windows.Forms.TextBox();
            this.portLabel = new System.Windows.Forms.Label();
            this.portTextBox = new System.Windows.Forms.TextBox();
            this.filenamelabel = new System.Windows.Forms.Label();
            this.fileNameTextBox = new System.Windows.Forms.TextBox();
            this.ServerIPLabel = new System.Windows.Forms.Label();
            this.NameLabel = new System.Windows.Forms.Label();
            this.ServerIPTextBox = new System.Windows.Forms.TextBox();
            this.LoginNameTextBox = new System.Windows.Forms.TextBox();
            this.ConnectButton = new System.Windows.Forms.Button();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.menuStrip1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.findToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(986, 24);
            this.menuStrip1.TabIndex = 3;
            this.menuStrip1.Text = "menuStrip1";
            this.menuStrip1.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.menuStrip1_ItemClicked);
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.openToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.saveAsToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.newToolStripMenuItem.Size = new System.Drawing.Size(146, 22);
            this.newToolStripMenuItem.Text = "New";
            this.newToolStripMenuItem.ToolTipText = "Open a new Spreadsheet";
            this.newToolStripMenuItem.Click += new System.EventHandler(this.newToolStripMenuItem_Click_1);
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.openToolStripMenuItem.Size = new System.Drawing.Size(146, 22);
            this.openToolStripMenuItem.Text = "Open";
            this.openToolStripMenuItem.ToolTipText = "Open a file from a file location";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(146, 22);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.ToolTipText = "Save file to current location or to a file location if one is not designated";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(146, 22);
            this.saveAsToolStripMenuItem.Text = "Save As";
            this.saveAsToolStripMenuItem.ToolTipText = "Save a file to a file location of your choice";
            this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.saveAsToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(146, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.ToolTipText = "Close the program";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.undoToolStripMenuItem});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
            this.editToolStripMenuItem.Text = "Edit";
            // 
            // undoToolStripMenuItem
            // 
            this.undoToolStripMenuItem.Name = "undoToolStripMenuItem";
            this.undoToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.undoToolStripMenuItem.Text = "Undo";
            this.undoToolStripMenuItem.Click += new System.EventHandler(this.undoToolStripMenuItem_Click);
            // 
            // findToolStripMenuItem
            // 
            this.findToolStripMenuItem.Name = "findToolStripMenuItem";
            this.findToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F)));
            this.findToolStripMenuItem.Size = new System.Drawing.Size(42, 20);
            this.findToolStripMenuItem.Text = "Find";
            this.findToolStripMenuItem.ToolTipText = "Opens a Find popup to allow user to find contents of cells and where they are loc" +
    "ated";
            this.findToolStripMenuItem.Click += new System.EventHandler(this.findToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.howToUseToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // howToUseToolStripMenuItem
            // 
            this.howToUseToolStripMenuItem.Name = "howToUseToolStripMenuItem";
            this.howToUseToolStripMenuItem.Size = new System.Drawing.Size(138, 22);
            this.howToUseToolStripMenuItem.Text = "How To Use";
            this.howToUseToolStripMenuItem.Click += new System.EventHandler(this.howToUseToolStripMenuItem_Click);
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.DefaultExt = "ss";
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // CellNameTB
            // 
            this.CellNameTB.Location = new System.Drawing.Point(6, 17);
            this.CellNameTB.Name = "CellNameTB";
            this.CellNameTB.ReadOnly = true;
            this.CellNameTB.Size = new System.Drawing.Size(52, 20);
            this.CellNameTB.TabIndex = 4;
            this.CellNameTB.TabStop = false;
            this.CellNameTB.TextChanged += new System.EventHandler(this.CellNameTB_TextChanged);
            // 
            // CellContentTB
            // 
            this.CellContentTB.Location = new System.Drawing.Point(6, 98);
            this.CellContentTB.Name = "CellContentTB";
            this.CellContentTB.Size = new System.Drawing.Size(303, 20);
            this.CellContentTB.TabIndex = 5;
            // 
            // CellValueTB
            // 
            this.CellValueTB.Location = new System.Drawing.Point(6, 59);
            this.CellValueTB.Name = "CellValueTB";
            this.CellValueTB.ReadOnly = true;
            this.CellValueTB.Size = new System.Drawing.Size(349, 20);
            this.CellValueTB.TabIndex = 6;
            this.CellValueTB.TabStop = false;
            this.CellValueTB.TextChanged += new System.EventHandler(this.CellValueTB_TextChanged);
            // 
            // CellNameLabel
            // 
            this.CellNameLabel.AutoSize = true;
            this.CellNameLabel.Location = new System.Drawing.Point(6, 3);
            this.CellNameLabel.Name = "CellNameLabel";
            this.CellNameLabel.Size = new System.Drawing.Size(55, 13);
            this.CellNameLabel.TabIndex = 7;
            this.CellNameLabel.Text = "Cell Name";
            this.CellNameLabel.Click += new System.EventHandler(this.CellNameLabel_Click);
            // 
            // CellContentLabel
            // 
            this.CellContentLabel.AutoSize = true;
            this.CellContentLabel.Location = new System.Drawing.Point(3, 84);
            this.CellContentLabel.Name = "CellContentLabel";
            this.CellContentLabel.Size = new System.Drawing.Size(64, 13);
            this.CellContentLabel.TabIndex = 8;
            this.CellContentLabel.Text = "Cell Content";
            // 
            // CellValueLabel
            // 
            this.CellValueLabel.AutoSize = true;
            this.CellValueLabel.Location = new System.Drawing.Point(3, 42);
            this.CellValueLabel.Name = "CellValueLabel";
            this.CellValueLabel.Size = new System.Drawing.Size(54, 13);
            this.CellValueLabel.TabIndex = 9;
            this.CellValueLabel.Text = "Cell Value";
            // 
            // UpdateButton
            // 
            this.UpdateButton.Location = new System.Drawing.Point(324, 95);
            this.UpdateButton.Name = "UpdateButton";
            this.UpdateButton.Size = new System.Drawing.Size(75, 23);
            this.UpdateButton.TabIndex = 6;
            this.UpdateButton.Text = "Update Cell";
            this.UpdateButton.UseVisualStyleBackColor = true;
            this.UpdateButton.Click += new System.EventHandler(this.UpdateButton_Click);
            // 
            // spreadsheetPanel1
            // 
            this.spreadsheetPanel1.AutoScroll = true;
            this.spreadsheetPanel1.AutoSize = true;
            this.spreadsheetPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.spreadsheetPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.spreadsheetPanel1.Location = new System.Drawing.Point(0, 142);
            this.spreadsheetPanel1.MinimumSize = new System.Drawing.Size(10, 10);
            this.spreadsheetPanel1.Name = "spreadsheetPanel1";
            this.spreadsheetPanel1.Size = new System.Drawing.Size(986, 279);
            this.spreadsheetPanel1.TabIndex = 11;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.SConsollabel);
            this.groupBox1.Controls.Add(this.ServertextBox);
            this.groupBox1.Controls.Add(this.portLabel);
            this.groupBox1.Controls.Add(this.portTextBox);
            this.groupBox1.Controls.Add(this.filenamelabel);
            this.groupBox1.Controls.Add(this.fileNameTextBox);
            this.groupBox1.Controls.Add(this.ServerIPLabel);
            this.groupBox1.Controls.Add(this.NameLabel);
            this.groupBox1.Controls.Add(this.ServerIPTextBox);
            this.groupBox1.Controls.Add(this.LoginNameTextBox);
            this.groupBox1.Controls.Add(this.ConnectButton);
            this.groupBox1.Controls.Add(this.UpdateButton);
            this.groupBox1.Controls.Add(this.CellValueLabel);
            this.groupBox1.Controls.Add(this.CellContentLabel);
            this.groupBox1.Controls.Add(this.CellNameLabel);
            this.groupBox1.Controls.Add(this.CellContentTB);
            this.groupBox1.Controls.Add(this.CellValueTB);
            this.groupBox1.Controls.Add(this.CellNameTB);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox1.Location = new System.Drawing.Point(0, 24);
            this.groupBox1.MinimumSize = new System.Drawing.Size(1, 1);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(986, 118);
            this.groupBox1.TabIndex = 12;
            this.groupBox1.TabStop = false;
            // 
            // SConsollabel
            // 
            this.SConsollabel.AutoSize = true;
            this.SConsollabel.Location = new System.Drawing.Point(379, 11);
            this.SConsollabel.Name = "SConsollabel";
            this.SConsollabel.Size = new System.Drawing.Size(82, 13);
            this.SConsollabel.TabIndex = 31;
            this.SConsollabel.Text = "Server Console:";
            // 
            // ServertextBox
            // 
            this.ServertextBox.Location = new System.Drawing.Point(467, 8);
            this.ServertextBox.Multiline = true;
            this.ServertextBox.Name = "ServertextBox";
            this.ServertextBox.Size = new System.Drawing.Size(197, 77);
            this.ServertextBox.TabIndex = 30;
            this.ServertextBox.TabStop = false;
            // 
            // portLabel
            // 
            this.portLabel.AutoSize = true;
            this.portLabel.Location = new System.Drawing.Point(698, 72);
            this.portLabel.Name = "portLabel";
            this.portLabel.Size = new System.Drawing.Size(39, 13);
            this.portLabel.TabIndex = 29;
            this.portLabel.Text = "Port #:";
            // 
            // portTextBox
            // 
            this.portTextBox.Location = new System.Drawing.Point(743, 69);
            this.portTextBox.Name = "portTextBox";
            this.portTextBox.Size = new System.Drawing.Size(100, 20);
            this.portTextBox.TabIndex = 3;
            this.portTextBox.Text = "2112";
            this.portTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // filenamelabel
            // 
            this.filenamelabel.AutoSize = true;
            this.filenamelabel.Location = new System.Drawing.Point(680, 32);
            this.filenamelabel.Name = "filenamelabel";
            this.filenamelabel.Size = new System.Drawing.Size(57, 13);
            this.filenamelabel.TabIndex = 27;
            this.filenamelabel.Text = "File Name:";
            // 
            // fileNameTextBox
            // 
            this.fileNameTextBox.Location = new System.Drawing.Point(743, 29);
            this.fileNameTextBox.Name = "fileNameTextBox";
            this.fileNameTextBox.Size = new System.Drawing.Size(100, 20);
            this.fileNameTextBox.TabIndex = 1;
            this.fileNameTextBox.Text = "test";
            this.fileNameTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // ServerIPLabel
            // 
            this.ServerIPLabel.AutoSize = true;
            this.ServerIPLabel.Location = new System.Drawing.Point(683, 52);
            this.ServerIPLabel.Name = "ServerIPLabel";
            this.ServerIPLabel.Size = new System.Drawing.Size(54, 13);
            this.ServerIPLabel.TabIndex = 26;
            this.ServerIPLabel.Text = "Server IP:";
            // 
            // NameLabel
            // 
            this.NameLabel.AutoSize = true;
            this.NameLabel.Location = new System.Drawing.Point(670, 11);
            this.NameLabel.Name = "NameLabel";
            this.NameLabel.Size = new System.Drawing.Size(63, 13);
            this.NameLabel.TabIndex = 24;
            this.NameLabel.Text = "User Name:";
            // 
            // ServerIPTextBox
            // 
            this.ServerIPTextBox.Location = new System.Drawing.Point(743, 49);
            this.ServerIPTextBox.Name = "ServerIPTextBox";
            this.ServerIPTextBox.Size = new System.Drawing.Size(100, 20);
            this.ServerIPTextBox.TabIndex = 2;
            this.ServerIPTextBox.Text = "155.98.111.67";
            this.ServerIPTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // LoginNameTextBox
            // 
            this.LoginNameTextBox.Location = new System.Drawing.Point(743, 8);
            this.LoginNameTextBox.Name = "LoginNameTextBox";
            this.LoginNameTextBox.Size = new System.Drawing.Size(100, 20);
            this.LoginNameTextBox.TabIndex = 0;
            this.LoginNameTextBox.Text = "sysadmin";
            this.LoginNameTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // ConnectButton
            // 
            this.ConnectButton.Location = new System.Drawing.Point(848, 8);
            this.ConnectButton.Margin = new System.Windows.Forms.Padding(2);
            this.ConnectButton.Name = "ConnectButton";
            this.ConnectButton.Size = new System.Drawing.Size(100, 81);
            this.ConnectButton.TabIndex = 4;
            this.ConnectButton.Text = "Connect";
            this.ConnectButton.UseVisualStyleBackColor = true;
            this.ConnectButton.Click += new System.EventHandler(this.ConnectButton_Click);
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            this.backgroundWorker1.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker1_ProgressChanged);
            this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
            // 
            // GUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(986, 421);
            this.Controls.Add(this.spreadsheetPanel1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.MinimumSize = new System.Drawing.Size(100, 100);
            this.Name = "GUI";
            this.Text = "Spreadsheet GUI";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.TextBox CellNameTB;
        private System.Windows.Forms.TextBox CellContentTB;
        private System.Windows.Forms.TextBox CellValueTB;
        private System.Windows.Forms.Label CellNameLabel;
        private System.Windows.Forms.Label CellContentLabel;
        private System.Windows.Forms.Label CellValueLabel;
        private System.Windows.Forms.Button UpdateButton;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem howToUseToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private SS.SpreadsheetPanel spreadsheetPanel1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.ToolStripMenuItem findToolStripMenuItem;
        private System.Windows.Forms.Label SConsollabel;
        private System.Windows.Forms.TextBox ServertextBox;
        private System.Windows.Forms.Label portLabel;
        private System.Windows.Forms.TextBox portTextBox;
        private System.Windows.Forms.Label filenamelabel;
        private System.Windows.Forms.TextBox fileNameTextBox;
        private System.Windows.Forms.Label ServerIPLabel;
        private System.Windows.Forms.Label NameLabel;
        private System.Windows.Forms.TextBox ServerIPTextBox;
        private System.Windows.Forms.TextBox LoginNameTextBox;
        private System.Windows.Forms.Button ConnectButton;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem undoToolStripMenuItem;
    }
}

