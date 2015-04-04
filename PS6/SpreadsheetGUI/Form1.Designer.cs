﻿namespace SpreadsheetGUI
{
    partial class Form1
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
            this.MenuBar = new System.Windows.Forms.MenuStrip();
            this.FileOnMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.NewOnMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.SaveOnMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.OpenOnMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.CloseOnMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.HelpOnMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.StatusText = new System.Windows.Forms.ToolStripStatusLabel();
            this.Save = new System.Windows.Forms.SaveFileDialog();
            this.GUICells = new SS.SpreadsheetPanel();
            this.Cell_Name = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.Cell_Value = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.Cell_Contents = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.Open = new System.Windows.Forms.OpenFileDialog();
            this.panel1 = new System.Windows.Forms.Panel();
            this.Go_to_Cell = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.MenuBar.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // MenuBar
            // 
            this.MenuBar.BackColor = System.Drawing.SystemColors.ControlLight;
            this.MenuBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.FileOnMenu,
            this.HelpOnMenu});
            this.MenuBar.Location = new System.Drawing.Point(0, 0);
            this.MenuBar.Name = "MenuBar";
            this.MenuBar.Size = new System.Drawing.Size(284, 24);
            this.MenuBar.TabIndex = 0;
            this.MenuBar.Text = "menuStrip1";
            // 
            // FileOnMenu
            // 
            this.FileOnMenu.BackColor = System.Drawing.SystemColors.Control;
            this.FileOnMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.NewOnMenu,
            this.SaveOnMenu,
            this.OpenOnMenu,
            this.CloseOnMenu});
            this.FileOnMenu.Name = "FileOnMenu";
            this.FileOnMenu.Size = new System.Drawing.Size(35, 20);
            this.FileOnMenu.Text = "file";
            // 
            // NewOnMenu
            // 
            this.NewOnMenu.Name = "NewOnMenu";
            this.NewOnMenu.Size = new System.Drawing.Size(103, 22);
            this.NewOnMenu.Text = "New";
            this.NewOnMenu.Click += new System.EventHandler(this.newToolStripMenuItem_Click);
            // 
            // SaveOnMenu
            // 
            this.SaveOnMenu.Name = "SaveOnMenu";
            this.SaveOnMenu.Size = new System.Drawing.Size(103, 22);
            this.SaveOnMenu.Text = "Save";
            // 
            // OpenOnMenu
            // 
            this.OpenOnMenu.Name = "OpenOnMenu";
            this.OpenOnMenu.Size = new System.Drawing.Size(103, 22);
            this.OpenOnMenu.Text = "Open";
            // 
            // CloseOnMenu
            // 
            this.CloseOnMenu.Name = "CloseOnMenu";
            this.CloseOnMenu.Size = new System.Drawing.Size(103, 22);
            this.CloseOnMenu.Text = "Close";
            this.CloseOnMenu.Click += new System.EventHandler(this.closeToolStripMenuItem_Click);
            // 
            // HelpOnMenu
            // 
            this.HelpOnMenu.Name = "HelpOnMenu";
            this.HelpOnMenu.Size = new System.Drawing.Size(42, 20);
            this.HelpOnMenu.Text = "help";
            this.HelpOnMenu.Click += new System.EventHandler(this.HelpOnMenu_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.StatusText});
            this.statusStrip1.Location = new System.Drawing.Point(0, 239);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(284, 22);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(39, 17);
            this.toolStripStatusLabel1.Text = "Status";
            // 
            // StatusText
            // 
            this.StatusText.Name = "StatusText";
            this.StatusText.Size = new System.Drawing.Size(164, 17);
            this.StatusText.Text = " All Formulas Able to Evaluate";
            // 
            // Save
            // 
            this.Save.FileOk += new System.ComponentModel.CancelEventHandler(this.saveFileDialog1_FileOk);
            // 
            // GUICells
            // 
            this.GUICells.AutoSize = true;
            this.GUICells.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GUICells.Location = new System.Drawing.Point(0, 0);
            this.GUICells.MinimumSize = new System.Drawing.Size(284, 102);
            this.GUICells.Name = "GUICells";
            this.GUICells.Size = new System.Drawing.Size(284, 125);
            this.GUICells.TabIndex = 2;
            // 
            // Cell_Name
            // 
            this.Cell_Name.Location = new System.Drawing.Point(76, 10);
            this.Cell_Name.Name = "Cell_Name";
            this.Cell_Name.ReadOnly = true;
            this.Cell_Name.Size = new System.Drawing.Size(58, 20);
            this.Cell_Name.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(58, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Cell_Name";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // Cell_Value
            // 
            this.Cell_Value.Location = new System.Drawing.Point(76, 35);
            this.Cell_Value.Name = "Cell_Value";
            this.Cell_Value.ReadOnly = true;
            this.Cell_Value.Size = new System.Drawing.Size(196, 20);
            this.Cell_Value.TabIndex = 5;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 38);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(57, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Cell_Value";
            // 
            // Cell_Contents
            // 
            this.Cell_Contents.AcceptsReturn = true;
            this.Cell_Contents.Location = new System.Drawing.Point(91, 64);
            this.Cell_Contents.Name = "Cell_Contents";
            this.Cell_Contents.Size = new System.Drawing.Size(181, 20);
            this.Cell_Contents.TabIndex = 7;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 67);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(72, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Cell_Contents";
            // 
            // Open
            // 
            this.Open.FileName = "openFileDialog1";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.Go_to_Cell);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.Cell_Name);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.Cell_Value);
            this.panel1.Controls.Add(this.Cell_Contents);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(284, 86);
            this.panel1.TabIndex = 9;
            // 
            // Go_to_Cell
            // 
            this.Go_to_Cell.Location = new System.Drawing.Point(223, 9);
            this.Go_to_Cell.Name = "Go_to_Cell";
            this.Go_to_Cell.Size = new System.Drawing.Size(49, 20);
            this.Go_to_Cell.TabIndex = 10;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(163, 18);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "Go to Cell";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.IsSplitterFixed = true;
            this.splitContainer1.Location = new System.Drawing.Point(0, 24);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.panel1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.GUICells);
            this.splitContainer1.Size = new System.Drawing.Size(284, 215);
            this.splitContainer1.SplitterDistance = 86;
            this.splitContainer1.TabIndex = 10;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.MenuBar);
            this.MainMenuStrip = this.MenuBar;
            this.MinimumSize = new System.Drawing.Size(300, 300);
            this.Name = "Form1";
            this.Text = "Spreadsheet App";
            this.MenuBar.ResumeLayout(false);
            this.MenuBar.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip MenuBar;
        private System.Windows.Forms.ToolStripMenuItem FileOnMenu;
        private System.Windows.Forms.ToolStripMenuItem NewOnMenu;
        private System.Windows.Forms.ToolStripMenuItem SaveOnMenu;
        private System.Windows.Forms.ToolStripMenuItem OpenOnMenu;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.SaveFileDialog Save;
        private SS.SpreadsheetPanel GUICells;
        private System.Windows.Forms.TextBox Cell_Name;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox Cell_Value;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox Cell_Contents;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.OpenFileDialog Open;
        private System.Windows.Forms.ToolStripMenuItem HelpOnMenu;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ToolStripMenuItem CloseOnMenu;
        private System.Windows.Forms.ToolStripStatusLabel StatusText;
        private System.Windows.Forms.TextBox Go_to_Cell;
        private System.Windows.Forms.Label label4;

    }
}

