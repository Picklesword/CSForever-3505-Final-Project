// Form1.cs by Jonathan Warner cs3500 
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SS;
using System.Text.RegularExpressions;


namespace SpreadsheetGUI
{
    /// <summary>
    /// This is the gui for the spreadsheet.
    /// </summary>
    public partial class Form1 : Form
    {
        private Spreadsheet actual_spreadsheet;// the logic of the spreadsheet
        private Communicator communicator;
        private bool cancel_from_overwrite;// checks to see if canceled when dealing with potential overwrites 
        const string form_error = @"(FormulaError)";// regex used to help detect formula error values in spreadsheet 
        private string file_name;// will be used to look back at last file saved to 
        private bool sendToServer;
        private int numberOfCellsReceived;
        /// <summary>
        /// Generates a new gui for the spreadsheet 
        /// </summary>
        public Form1()
        {
            InitializeComponent();
            sendToServer = false;
            // following components handle communication with server
            communicator = new Communicator();
            communicator.IncomingErrorEvent += ErrorReceived;
            communicator.IncomingConnectEvent += ConnectErrorReceived;
            communicator.IncomingConnectedEvent += ConnectedMsgReceived;
            communicator.IncomingCellEvent += CellUpdateReceived;
            communicator.ServerEvent += ServerStatusReceived;
            numberOfCellsReceived = 0;
            //Internal spreadsheet events
            this.FormClosing += new FormClosingEventHandler(Form_Close_Check);
            GUICells.SelectionChanged += displaySelection;
            GUICells.SetSelection(0, 0);
            this.Resize += new EventHandler(redraw);
            SaveOnMenu.Click += new EventHandler(saveToolStripMenuItem_Click);
            OpenOnMenu.Click += new EventHandler(openToolStripMenuItem_Click);
            actual_spreadsheet = new Spreadsheet(x => true, x => x.ToUpper(), "ps6");
            Cell_Contents.KeyDown += new KeyEventHandler(Edit_Cell_Contents);
            Cell_Contents.Text = "";
            Cell_Name.Text = "A1";
            Cell_Value.Text = "";
            Go_to_Cell.KeyDown += new KeyEventHandler(GoToCell);
        }

        /// <summary>
        /// This method pops up a message box to display the message sent by server about it's status
        /// </summary>
        /// <param name="obj"></param>
        private void ServerStatusReceived(string obj)
        {
          ServertextBox.Text = obj;
            MessageBox.Show(obj);
        }
        /// <summary>
        /// This invent updates the content of a cell
        /// </summary>
        /// <param name="msg"></param>
        private void CellUpdateReceived(string[] msg)
        {
            //outputs what we see from the server for debug and possible to allow the user to see what the server is sending
            /*
            for (int i = 0; i < msg.Length; i++)
            {
                ServertextBox.Text = ServertextBox.Text + msg[i];

            }
             */
            string cellContents = "";
            //Update the cell locally.
            for (int i = 2; i < msg.Length; i++)
            {
                
                //creates a string from the message received from the server
                if (i == msg.Length - 1)
                    cellContents = cellContents + msg[i];
                else
                    cellContents = cellContents + msg + " ";

                
            }

            UpdateCell(msg[1], cellContents);
            //UpdateCell(msg[1], msg[2]); 
            //msg[0] contains the word cell, the following array locations should contain cell name and
            //contents
            
        }

        /// <summary>
        /// This method updates the status of connection with the server, when it is connected.
        /// </summary>
        /// <param name="obj"></param>
        private void ConnectedMsgReceived(string[] msg)
        {
            //when connected message is received from the server it sends the number of cells 
            //contained in the spreadsheet
            if (InvokeRequired)
            {
                for (int i = 0; i < msg.Length; i++)
                {
                    ServertextBox.Invoke(new MethodInvoker(delegate { ServertextBox.Text = ServertextBox.Text + msg[i]; }));
                }
            }
            else
            {
                for (int i = 0; i < msg.Length; i++)
                {
                    ServertextBox.Text = ServertextBox.Text + msg[i];
                }
            }
            numberOfCellsReceived = Convert.ToInt32(msg[1]);//not sure what use this is but it is required in the spec
            sendToServer = true;
            if (InvokeRequired)
            {
                ConnectButton.Invoke(new MethodInvoker(delegate { ConnectButton.Text = "Register User"; }));
            }
            else
            {
                ConnectButton.Text = "Register User";
            }
             
            //if message that user has connected is received change the text on the Connect button to Register User
            //to allow a person that is logged in to register a user.
        }

        /// <summary>
        /// This method is invoked when a connection could not be established either because the server
        /// is not running or the IP address and port are incorrect
        /// </summary>
        /// <param name="msg"></param>
        private void ConnectErrorReceived(string msg)
        {
            if (InvokeRequired)
            {
                ServertextBox.Invoke(new MethodInvoker(delegate { ServertextBox.Text = msg; }));
            }
            else
            {
                ServertextBox.Text = msg;
            }
            
            //MessageBox.Show(msg);
        }

        /// <summary>
        /// This method is invoked when an error message is received from the server
        /// </summary>
        /// <param name="msg"></param>
        private void ErrorReceived(string msg)
        {
            if (InvokeRequired)
            {
                ServertextBox.Invoke(new MethodInvoker(delegate { ServertextBox.Text = msg; }));
            }
            else
            {
                ServertextBox.Text = msg;
            }
            //MessageBox.Show(msg);
        }


        /// <summary>
        /// Causes the appropriate information to be displayed by cells 
        /// and also the textboxes for name value and content 
        /// </summary>
        /// <param name="ss"></param>
        private void displaySelection(SpreadsheetPanel ss)
        {
            int row, col;
            String value;
            ss.GetSelection(out col, out row);
            ss.GetValue(col, row, out value);
            Cell_Name.Text = ((char)('A' + col)).ToString() + (row + 1).ToString();
            if(InvokeRequired)
            {
                Cell_Value.Invoke(new MethodInvoker(delegate { Cell_Value.Text = actual_spreadsheet.GetCellValue(Cell_Name.Text).ToString(); }));
            }
            Cell_Value.Text = actual_spreadsheet.GetCellValue(Cell_Name.Text).ToString();
            
            if (actual_spreadsheet.GetCellContents(Cell_Name.Text).GetType() != typeof(Double) &&
                actual_spreadsheet.GetCellContents(Cell_Name.Text).GetType() != typeof(string))
            {
                Cell_Contents.Text = "=" + actual_spreadsheet.GetCellContents(Cell_Name.Text).ToString();
            }
            else
            {
                if (InvokeRequired)
                {
                    Cell_Contents.Invoke(new MethodInvoker(delegate { Cell_Contents.Text = actual_spreadsheet.GetCellContents(Cell_Name.Text).ToString(); }));
                }
                Cell_Contents.Text = actual_spreadsheet.GetCellContents(Cell_Name.Text).ToString();
            }
        }

        /// <summary>
        /// Causes the spreadsheet panel to refresh itself 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void redraw(object sender, EventArgs e)
        {
            GUICells.Invalidate();
        }

        /// <summary>
        /// Updates the spreadsheet logic and causes the 
        /// cells to be refreshed. Also determines 
        /// if cells should show content in case of 
        /// formula that can't eval or circular dependency 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Edit_Cell_Contents(object sender, KeyEventArgs e)
        {
            if (Cell_Contents.Focused == true && e.KeyCode == Keys.Return)
            {
                UpdateCell(Cell_Name.Text, Cell_Contents.Text); 

                //sendToServer = true; //not needed
                /* Tuple<int, int> cell_adress;
                 IEnumerable<string> updates = null;
                 try
                 {
                     updates = actual_spreadsheet.SetContentsOfCell(Cell_Name.Text, Cell_Contents.Text);
                 }
                 catch
                 {
                     StatusText.Text = " A Circular Dependency has occured.";
                     statusStrip1.Invalidate();
                     GUICells.GetSelection(out col, out row);
                     GUICells.SetValue(col, row, "");
                     return;
                 }

                 // added by Dharani
                 try
                 {
                     string cellInfo = Cell_Name.Text + "|" + Cell_Contents.Text;
                     communicator.SendMessage(cellInfo);
                 }
                 catch(Exception ex)
                 {
                     StatusText.Text = " Could not send the cell information to Server!\n" + ex.Message;
                     return;
                 }


                 StatusText.Text = " All Formulas Able to Evaluate";
                 GUICells.GetSelection(out col, out row);
                 if (Regex.IsMatch(actual_spreadsheet.GetCellValue(Cell_Name.Text).ToString(), form_error))
                 {
                     GUICells.SetValue(col, row, "");
                     StatusText.Text = " Not All Formulas Able to Evaluate";
                 }
                 else
                 {
                     GUICells.SetValue(col, row, actual_spreadsheet.GetCellValue(Cell_Name.Text).ToString());
                 }


                 foreach (string s in updates)
                 {
                     cell_adress = Reverse_Cell_Name(s);
                     if (Regex.IsMatch(actual_spreadsheet.GetCellValue(s).ToString(), form_error))
                     {
                         GUICells.SetValue(cell_adress.Item1, cell_adress.Item2, "");
                         StatusText.Text = "Not All Formulas Able to Evaluate";

                     }
                     else
                     {
                         GUICells.SetValue(cell_adress.Item1, cell_adress.Item2, actual_spreadsheet.GetCellValue(s).ToString());
                     }
                 }
                 IEnumerable<string> temp_it = actual_spreadsheet.GetNamesOfAllNonemptyCells();
                 foreach (string s in temp_it)
                 {
                     if (Regex.IsMatch(actual_spreadsheet.GetCellValue(s).ToString(), form_error))
                     {
                         StatusText.Text = "Not All Formulas Able to Evaluate";

                     }
                 } 
                 displaySelection(GUICells);
             }*/
                // added by Dharani, edited by Scott
            }
            if (sendToServer) //if statement is not needed but doesn't hurt anything
            {
                try
                {
                    string cellInfo = "cell " + Cell_Name.Text + " " + Cell_Contents.Text;
                    communicator.SendMessage(cellInfo);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Could not send the cell information to Server!\n" + ex.Message);
                    return;
                }
                //sendToServer = false;
            }
        }

        /// <summary>
        /// This method updates the cell contents to spreadsheet and refreshes GUI
        /// </summary>
        /// <param name="cellName"></param>
        /// <param name="content"></param>

        private void UpdateCell(string cellName, string content)
        {
            int row, col;
            //cellName = cellName.ToUpper(); //makes the cell name uppercase
            Tuple<int, int> cell_adress;
            IEnumerable<string> updates = null;
            try
            {
                updates = actual_spreadsheet.SetContentsOfCell(cellName, content);
            }
            catch
            {//need to update this section when a circular dependency occurs to revert the cells back to previous value in cell
                //currently it is just setting the cell to equal ""
                StatusText.Text = " A Circular Dependency has occured.";
                statusStrip1.Invalidate();
                sendToServer = false;
                //GUICells.GetSelection(out col, out row);
                //GUICells.SetValue(col, row, "");
                return;
            }

            StatusText.Text = " All Formulas Able to Evaluate";
            GUICells.GetSelection(out col, out row);
            if (Regex.IsMatch(actual_spreadsheet.GetCellValue(Cell_Name.Text).ToString(), form_error))
            {
                GUICells.SetValue(col, row, "");
                StatusText.Text = " Not All Formulas Able to Evaluate";
            }
            else
            {
                GUICells.SetValue(col, row, actual_spreadsheet.GetCellValue(Cell_Name.Text).ToString());
            }


            foreach (string s in updates)
            {
                cell_adress = Reverse_Cell_Name(s);
                if (Regex.IsMatch(actual_spreadsheet.GetCellValue(s).ToString(), form_error))
                {
                    GUICells.SetValue(cell_adress.Item1, cell_adress.Item2, "");
                    StatusText.Text = "Not All Formulas Able to Evaluate";

                }
                else
                {
                    GUICells.SetValue(cell_adress.Item1, cell_adress.Item2, actual_spreadsheet.GetCellValue(s).ToString());
                }
            }
            IEnumerable<string> temp_it = actual_spreadsheet.GetNamesOfAllNonemptyCells();
            foreach (string s in temp_it)
            {
                if (Regex.IsMatch(actual_spreadsheet.GetCellValue(s).ToString(), form_error))
                {
                    StatusText.Text = "Not All Formulas Able to Evaluate";

                }
            }
            sendToServer = true;
            displaySelection(GUICells);
        }

        /// <summary>
        /// This get two ints corresponding to the col and row 
        /// based on given string version of the cell name 
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private Tuple<int, int> Reverse_Cell_Name(string s)
        {
            int col, row;

            row = 0;
            col = s[0] - 'A';
            string letter = s[0].ToString();
            string temp = s.Replace(letter, "");
            int add = 0;
            Int32.TryParse(temp, out add);


            row = add - 1;
            return new Tuple<int, int>(col, row);
        }

        /// <summary>
        /// This sets up the save dialog box 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveToolStripMenuItem_Click(object sender, System.EventArgs e)
        {

            Save.FileName = file_name;
            Save.FileOk += new CancelEventHandler(saveFileDialog1_FileOk);
            Save.Filter = "Spreadsheet File (*.sprd)|*.sprd|All Files (*.*)|*.*";
            Save.Disposed += new EventHandler(saveFileDialog1_FileCancel);
            Save.ShowDialog();
        }

        /// <summary>
        /// Sets up the open/load dialog box when open is clicked on from menu 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void openToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            Open.FileName = "";
            cancel_from_overwrite = false;
            if (actual_spreadsheet.Changed && !possible_overide(Open))
            {
                cancel_from_overwrite = true;
                return;
            }
            Open.FileOk += new CancelEventHandler(opendFileDialog1_FileOk);
            Open.Filter = "Spreadsheet File (*.sprd)|*.sprd|All Files (*.*)|*.*";
            Open.ShowDialog();
        }

        /// <summary>
        /// Will check if a file exist and give warning 
        /// and try to save to choosen file address 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            string spread_ext = @"(.sprd)$";
            string temp_name = Save.FileName;
            if (temp_name != file_name)
            {
                Save.CheckFileExists = true;
            }
            Save.CheckFileExists = false;
            file_name = temp_name;
            if (Save.FilterIndex == 0 && !Regex.IsMatch(temp_name, spread_ext))
            {
                temp_name = temp_name + ".sprd";

            }
            try
            {
                actual_spreadsheet.Save(temp_name);

            }
            catch (Exception execpt)
            {
                file_name = null;
                string message = "Could not save file\r\n" + execpt.Message;
                string caption = "Save failed";
                MessageBoxButtons button = MessageBoxButtons.OK;
                DialogResult error_mes = MessageBox.Show(message, caption, button);

            }
        }

        /// <summary>
        /// makes note that 
        /// after choosing to save 
        /// to prevent data loss
        /// cancel was pressed 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveFileDialog1_FileCancel(object sender, EventArgs e)
        {
            cancel_from_overwrite = false;
        }

        /// <summary>
        /// Attempts to open from file given 
        /// also warns if current data is unsaved
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void opendFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            IEnumerable<string> temp_iem;
            Tuple<int, int> temp_address;
            IEnumerable<string> old_vars;
            old_vars = actual_spreadsheet.GetNamesOfAllNonemptyCells();
            try
            {
                actual_spreadsheet = new Spreadsheet(Open.FileName, x => true, x => x.ToUpper(), "ps6");
            }
            catch (Exception except)
            {
                string message = "Could not open file\r\n" + except.Message;
                string caption = "Open Failed";
                MessageBoxButtons button = MessageBoxButtons.OK;
                DialogResult error_mes = MessageBox.Show(message, caption, button);
                return;
            }
            temp_iem = actual_spreadsheet.GetNamesOfAllNonemptyCells();
            foreach (string s in old_vars)
            {
                temp_address = Reverse_Cell_Name(s);
                GUICells.SetValue(temp_address.Item1, temp_address.Item2, "");
            }
            foreach (string s in temp_iem)
            {
                temp_address = Reverse_Cell_Name(s);
                if (Regex.IsMatch(actual_spreadsheet.GetCellValue(s).ToString(), form_error))
                {
                    GUICells.SetValue(temp_address.Item1, temp_address.Item2, "");

                }
                else
                {
                    GUICells.SetValue(temp_address.Item1, temp_address.Item2, actual_spreadsheet.GetCellValue(s).ToString());
                }
            }

            GUICells.Invalidate();
            GUICells.SetSelection(0, 0);
            Cell_Name.Text = "A1";
            if (actual_spreadsheet.GetCellContents("A1").GetType() != typeof(double) &&
                actual_spreadsheet.GetCellContents("A1").GetType() != typeof(string))
            {
                Cell_Contents.Text = "=" + actual_spreadsheet.GetCellContents("A1").ToString();
            }
            else
            {
                Cell_Contents.Text = actual_spreadsheet.GetCellContents("A1").ToString();
            }
            Cell_Value.Text = actual_spreadsheet.GetCellValue("A1").ToString();
        }

        /// <summary>
        /// Creates the message box 
        /// giving the option to save so data 
        /// is not lost 
        /// </summary>
        /// <param name="sender"></param>
        /// <returns></returns>
        private bool possible_overide(object sender)
        {
            string message = "Your work has not been saved and will be lost. \r\n Would you like to save?";
            string caption = "Possible Dataloss";
            MessageBoxButtons button = MessageBoxButtons.YesNoCancel;
            DialogResult overwrite_mes = MessageBox.Show(message, caption, button);
            if (overwrite_mes == DialogResult.Yes)
            {

                saveToolStripMenuItem_Click(SaveOnMenu, new EventArgs());
                return cancel_from_overwrite;
            }
            else if (overwrite_mes == DialogResult.No)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// nothing accidentally generated it
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void label1_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Starts a new instance of the spreadsheet gui
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SpreadsheetGUIApplicationContext.getSpreadAppContext().RunInstance(new Form1());

        }

        /// <summary>
        /// starts to close the current gui app 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Checks to make sure not overriding data 
        /// and give the option to save if you are 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form_Close_Check(object sender, FormClosingEventArgs e)
        {
            cancel_from_overwrite = false;
            if (actual_spreadsheet.Changed && !possible_overide(Open))
            {
                cancel_from_overwrite = true;
                e.Cancel = true;
                return;
            }

        }

        /// <summary>
        /// Will set up so you can input to desired cell 
        /// but will not take you there on the grid 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GoToCell(object sender, KeyEventArgs e)
        {
            if (Go_to_Cell.Focused == true && e.KeyCode == Keys.Return)
            {
                Tuple<int, int> cell_address;

                Go_to_Cell.Text = Go_to_Cell.Text.ToUpper();
                cell_address = Reverse_Cell_Name(Go_to_Cell.Text);

                try
                {

                    string cellnames = @"^[A-Z]\d{1,2}$";
                    if (!Regex.IsMatch(Go_to_Cell.Text, cellnames))
                    {
                        throw new Exception();
                    }
                    GUICells.SetSelection(cell_address.Item1, cell_address.Item2);
                    Cell_Name.Text = Go_to_Cell.Text;
                    if (actual_spreadsheet.GetCellContents(Go_to_Cell.Text).GetType() != typeof(string)
                        && actual_spreadsheet.GetCellContents(Go_to_Cell.Text).GetType() != typeof(Double))
                    {
                        Cell_Contents.Text = "=" + actual_spreadsheet.GetCellContents(Go_to_Cell.Text).ToString();
                    }
                    else
                    {
                        Cell_Contents.Text = actual_spreadsheet.GetCellContents(Go_to_Cell.Text).ToString();
                    }
                    Cell_Value.Text = actual_spreadsheet.GetCellValue(Go_to_Cell.Text).ToString();
                    Go_to_Cell.Text = "";
                }
                catch
                {
                    Go_to_Cell.Text = "";
                    string message = "Cell entered is not valid on the grid";
                    string caption = "Possible Dataloss";
                    MessageBoxButtons button = MessageBoxButtons.OK;
                    DialogResult non_validCell = MessageBox.Show(message, caption, button);
                }
            }
        }


        /// <summary>
        /// Launches the help message box 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HelpOnMenu_Click(object sender, EventArgs e)
        {
            string message = "Use the mouse to select cells on the grid.\r\nType in valid cell with lower or capital letters to be able to alter the cells contents.\r\nYou must scroll to the desired cell to view it, so do not click on any other cells before typing in contents after using the GoToCell box.\r\nPress enter while in the GoToCell box or contents box to enter data.\r\nThere will be an annoying beep each time enter is pressed in those boxes sorry about that.\r\nUse the menu tabs to open new spreadsheet, save current spreadsheet, or open another spreadsheet.\r\n My extra feature is the GoToCell box and I spent a good deal of time making sure close and cancel buttons behaved how I thought they should.";
            string caption = "Help";
            MessageBoxButtons button = MessageBoxButtons.OK;
            DialogResult non_validCell = MessageBox.Show(message, caption, button);
        }

        private void ConnectButton_Click(object sender, EventArgs e)
        {
            //if ConnectButton.Text == Connect then the user hasn't connected to the server. once it has the text will change to register user
            if (ConnectButton.Text == "Connect")
            {
                actual_spreadsheet = new Spreadsheet(x => true, x => x.ToUpper(), "ps6"); //make a new clean spreadsheet
                if (LoginNameTextBox.Text == "")
                {
                    MessageBox.Show("Please enter your username!");
                    LoginNameTextBox.Focus();
                    return;
                }
                else if (fileNameTextBox.Text == "")
                {
                    MessageBox.Show("Please enter the filename you want to open or create!");
                    fileNameTextBox.Focus();
                    return;
                }
                else if (ServerIPTextBox.Text == "")
                {
                    MessageBox.Show("Please enter the IP address of the server you want to connect to.");
                    ServerIPTextBox.Focus();
                    return;
                }
                else
                    communicator.Connect(ServerIPTextBox.Text, LoginNameTextBox.Text, fileNameTextBox.Text, portTextBox.Text);
            }
            else if(ConnectButton.Text == "Register User")
            {
                if (LoginNameTextBox.Text == "")
                {
                    MessageBox.Show("Please enter a name to be registered");
                    LoginNameTextBox.Focus();
                    return;
                }
                else
                    communicator.RegisterUser(LoginNameTextBox.Text);

            }
        }

        /// <summary>
        /// Sends the message undo to the server.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            communicator.SendMessage("undo");
        }//end ConnectButton Method
    }
}
