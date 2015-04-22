//Scott Young
//U0824833
//11-2-2013
//this program create a spreadsheet that allows for calculations, strings and doubles, it also allows you to use the find method to locate values
//and manipulate them.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SpreadsheetUtilities;
using SS;
using System.Text.RegularExpressions;

namespace SpreadsheetGUI
{
    public partial class GUI : Form
    {
        private Spreadsheet sheet;
        private string filePath;//global variable to store the file path for use in save and open etc.
        private Dictionary<string, List<string>> Cell; //dictionary that stores values and all the cells that contain it.
        private GUI gui; //used to create a new gui when the open method is used
        private Communicator communicator;
        private bool sendToServer;
        private int numberOfCellsReceived;

        /// <summary>
        /// default constructor that initializes all of the values
        /// </summary>
        public GUI()
        {
            InitializeComponent();
            filePath = "";
            Cell = new Dictionary<string, List<string>>();
            //this.KeyPreview = true;
            sheet = new Spreadsheet(isValid, Normalize, "ps6"); //need to implement an constructor that will normalize the values and get the version ps6
            sendToServer = false;
            // following components handle communication with server
            communicator = new Communicator();
            communicator.IncomingErrorEvent += ErrorReceived;
            communicator.IncomingConnectEvent += ConnectErrorReceived;
            communicator.IncomingConnectedEvent += ConnectedMsgReceived;
            communicator.IncomingCellEvent += CellUpdateReceived;
            communicator.ServerEvent += ServerStatusReceived;
            numberOfCellsReceived = 0;
            //spreadsheetPanel1.Size = System.Drawing.Size.Round(new SizeF(600, 600));
            spreadsheetPanel1.SelectionChanged += displaySelection; //this is what displays the cells contents when you click on it
            CellContentTB.KeyUp += new KeyEventHandler(CellContentTB_KeyUp);
            backgroundWorker1.DoWork += new DoWorkEventHandler(backgroundWorker1_DoWork);
            backgroundWorker1.ProgressChanged += new ProgressChangedEventHandler(backgroundWorker1_ProgressChanged);
            //spreadsheetPanel1.SetSelection(0, 0);//sets the start locating in A1
            backgroundWorker1.WorkerSupportsCancellation = true;
            displaySelection(spreadsheetPanel1);
            this.FormClosing += new FormClosingEventHandler(this.Form1_FormClosing);
            //String cellName = calculateCellName(0) + (0 + 1);//gets the cell name 
            //CellNameTB.Text = cellName; //stores cell name in TextBox



        }

        /// <summary>
        /// outputs the message received from the server for errors
        /// </summary>
        /// <param name="msg">string[]</param>
        private void ErrorReceived(string msg)
        {
            if (InvokeRequired)
            {
                ServertextBox.Invoke(new MethodInvoker(delegate { ServertextBox.Text = ""; }));
                for (int i = 2; i < msg.Length; i++)
                    ServertextBox.Invoke(new MethodInvoker(delegate { ServertextBox.Text = msg[i] + " "; }));
            }
            else
            {
                ServertextBox.Text = "";
                for (int i = 2; i < msg.Length; i++)
                    ServertextBox.Text = msg[i] + " ";
            }
        }

        /// <summary>
        /// This method is used when you open a file with a given file path to initialize all of the values and creates the new sheet.
        /// </summary>
        /// <param name="filePath"></param>
        public GUI(string filePath)
        {
            InitializeComponent();
            this.filePath = filePath;
            sheet = new Spreadsheet(this.filePath, isValid, Normalize, "ps6");
            updateAllDependentCells(sheet.GetNamesOfAllNonemptyCells().ToList());
            backgroundWorker1.WorkerSupportsCancellation = true;
            spreadsheetPanel1.SelectionChanged += displaySelection; //this is what updates the cells need to create text             
            CellContentTB.KeyUp += new KeyEventHandler(CellContentTB_KeyUp);
            displaySelection(spreadsheetPanel1);
            this.FormClosing += new FormClosingEventHandler(this.Form1_FormClosing);
        }


        /// <summary>
        /// This method displays a message sent by server about it's status
        /// </summary>
        /// <param name="obj">string</param>
        private void ServerStatusReceived(string obj)
        {
            if (obj.CompareTo("The server has crashed") == 0)
            {
                spreadsheetPanel1.Clear();
                if (InvokeRequired)
                {
                    spreadsheetPanel1.Invoke(new MethodInvoker(delegate { spreadsheetPanel1.Enabled = false; }));
                }

                if (InvokeRequired)
                {
                    ConnectButton.Invoke(new MethodInvoker(delegate { ConnectButton.Text = "Connect"; }));
                }
                else
                {
                    ConnectButton.Text = "Connect";
                }

            }

            if (InvokeRequired)
            {
                ServertextBox.Invoke(new MethodInvoker(delegate { ServertextBox.Text = ""; }));
                ServertextBox.Invoke(new MethodInvoker(delegate { ServertextBox.Text = obj; }));
            }
            else
            {
                ServertextBox.Text = "";
                ServertextBox.Text = obj;
            }

            //MessageBox.Show(obj);
        }
        /// <summary>
        /// This invent updates the content of a cell
        /// </summary>
        /// <param name="msg"></param>
        private void CellUpdateReceived(string[] msg)
        {
            //outputs what we see from the server for debug and possible to allow the user to see what the server is sending
            if (InvokeRequired)
            {
                ServertextBox.Invoke(new MethodInvoker(delegate { ServertextBox.Text = ""; }));
            }
            else
                ServertextBox.Text = "";

            for (int i = 0; i < msg.Length; i++)
            {

                if (InvokeRequired)
                {
                    ServertextBox.Invoke(new MethodInvoker(delegate { ServertextBox.Text = ServertextBox.Text + msg[i] + " "; }));
                }
                else
                    ServertextBox.Text = ServertextBox.Text + msg[i] + " ";

            }

            string cellContents = "";
            //Update the cell locally.
            for (int i = 2; i < msg.Length; i++)
            {

                //creates a string from the message received from the server
                if (i == msg.Length - 1)
                    cellContents = cellContents + msg[i];
                else
                    cellContents = cellContents + msg[i] + " ";


            }
            UpdateCellContents(msg[0], cellContents);//updated the cell contents
            //UpdateCell(msg[1], cellContents); //need to change to current setup
            
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
                ServertextBox.Invoke(new MethodInvoker(delegate { ServertextBox.Text = ""; }));
                for (int i = 0; i < msg.Length; i++)
                {
                    ServertextBox.Invoke(new MethodInvoker(delegate { ServertextBox.Text = ServertextBox.Text + msg[i] + " "; }));
                }
            }
            else
            {
                ServertextBox.Text = "";
                for (int i = 0; i < msg.Length; i++)
                {
                    ServertextBox.Text = ServertextBox.Text + msg[i] + " ";
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
                ServertextBox.Invoke(new MethodInvoker(delegate { ServertextBox.Text = ""; }));
                ServertextBox.Invoke(new MethodInvoker(delegate { ServertextBox.Text = msg; }));
            }
            else
            {
                ServertextBox.Text = "";
                ServertextBox.Text = msg;
            }

            //MessageBox.Show(msg);
        }

        /// <summary>
        /// when the form is closing it askes the user if they are sure if they haven't saved the spreadsheet.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            string message = "Are you sure you would like to exit without saving?";
            string caption = "Closing spreadsheet";

            if (sheet.Changed)
            {
                var result = MessageBox.Show(message, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.No)
                {
                    e.Cancel = true;
                }
            }
        }

        /// <summary>
        /// When enter is pressed while the cursor is in the cellContentTB it will put that value into
        /// the spreadsheet. This is all done using a backgroundworker thread
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CellContentTB_KeyUp(object sender, KeyEventArgs e)
        {
            sendToServer = true;
            if (e.KeyCode == Keys.Enter)
            {
                if (backgroundWorker1.IsBusy != true)
                {
                    // Start the asynchronous operation.
                    backgroundWorker1.RunWorkerAsync();
                }
                //setCellContentsUpdateCellContents();
                //if (sheet.Changed)
                //    saveToolStripMenuItem.Enabled = true;
            }
        }

        /// <summary>
        /// normalizes cell names
        /// </summary>
        /// <param name="st"></param>
        /// <returns></returns>
        public String Normalize(String st)
        {
            //String inputChanger;
            st = st.ToUpper();
            //inputChanger = "_" + st;

            return st;
        }

        /// <summary>
        /// checks to make sure that the input name is valid
        /// </summary>
        /// <param name="st"></param>
        /// <returns></returns>
        public bool isValid(String st)
        {
            //Regex check = new Regex(@"(^[A-Z]");
            Regex check = new Regex(@"(^[A-Z]+)([1-9]+)([0-9]*$)");
            if (check.IsMatch(st))
                return true;
            else
                return false;
        }


        /// <summary>
        /// Every time the selection changes, this method is called with the
        /// Spreadsheet as its parameter. this displays the contents, cell name and cell value in the text boxes
        /// </summary>
        /// <param name="ss"></param>
        private void displaySelection(SpreadsheetPanel ss)
        {
            int row, col;
            String value;
            ss.GetSelection(out col, out row);
            ss.GetValue(col, row, out value);

            ClearCellValueTB();
            //CellValueTB.Clear();
            ClearCellContentTB();
            //CellContentTB.Clear();

            if (value == "")
            {
                getCellName(col, row);
            }
            else
            {

                String cellName = calculateCellName(col) + (row + 1);

                UpdateCellNameTB(cellName);
                //CellNameTB.Text = cellName;
                UpdateCellContentTB(sheet.GetCellContents(cellName).ToString());
                //CellContentTB.Text = sheet.GetCellContents(cellName).ToString();
                UpdateCellValueTB(sheet.GetCellValue(cellName).ToString());
                //CellValueTB.Text = sheet.GetCellValue(cellName).ToString();

                ss.SetValue(col, row, sheet.GetCellValue(cellName).ToString());
            }
        }

        /// <summary>
        /// method to stop crossthreading errors
        /// </summary>
        /// <param name="text"></param>
        private void UpdateCellNameTB(string text)
        {
            if (InvokeRequired)
                Invoke(new Action<string>(UpdateCellNameTB), text);
            else
                CellNameTB.Text = text;
        }

        /// <summary>
        /// method to stop crossthreading errors
        /// </summary>
        private void ClearCellValueTB()
        {
            if (InvokeRequired)
                Invoke(new Action(ClearCellValueTB));
            else
                CellValueTB.Clear();

        }

        /// <summary>
        /// method that clears the contents and stops crossthreading errors
        /// </summary>
        private void ClearCellContentTB()
        {
            if (InvokeRequired)
                Invoke(new Action(ClearCellContentTB));
            else
                CellContentTB.Clear();

        }



        /// <summary>
        /// Override method for Update Cells when 2 strings are input from server
        /// </summary>
        /// <param name="cellName"></param>
        /// <param name="cellValue"></param>
        private void UpdateCellContents(string cellName, string cellValue)
        {
            List<String> cellsToUpdate = new List<string>();
            int row, col;
            col = getColValue(cellName[0]);
            row = cellName[1] - 49;
            //String value;
            //spreadsheetPanel1.GetSelection(out col, out row);
            //spreadsheetPanel1.GetValue(col, row, out value);

            //cellName = calculateCellName(col) + (row + 1);
            //cellValue = "";
            //CellNameTB.Text = cellName;
            string contentsOfCell = "";

            
            contentsOfCell = cellValue;

            List<string> currentCellName = new List<string>();
            currentCellName.Add(cellName);

            string tmpValue = "";
            List<string> tempList = sheet.GetNamesOfAllNonemptyCells().ToList();
            if (tempList.Contains(cellName))
                tmpValue = sheet.GetCellValue(cellName).ToString();

            try
            {
                cellsToUpdate = sheet.SetContentsOfCell(cellName, contentsOfCell).ToList(); //gets the list of cells to update
                if (sheet.Changed)
                    saveToolStripMenuItem.Enabled = true;

                UpdateCellContentTB(sheet.GetCellContents(cellName).ToString());//updates the cell content TB
                //CellContentTB.Text = sheet.GetCellContents(cellName).ToString();
                UpdateCellValueTB(sheet.GetCellValue(cellName).ToString()); //update the cell value tb
                //CellValueTB.Text = sheet.GetCellValue(cellName).ToString();
                cellValue = sheet.GetCellValue(cellName).ToString();

                FindListModifier(tmpValue, cellValue, cellName, cellsToUpdate);//creates a cell location for the cell List

                spreadsheetPanel1.SetValue(col, row, sheet.GetCellValue(cellName).ToString());

                if (cellsToUpdate.Count > 1)//updates all the cells that need to be
                {
                    cellsToUpdate.RemoveAt(0);
                    updateAllDependentCells(cellsToUpdate);
                }

            }
            catch (CircularException exception)
            {
                handleCircularException(exception, contentsOfCell);
                cellValue = sheet.GetCellValue(cellName).ToString();
                FindListModifier(tmpValue, cellValue, cellName, cellsToUpdate);
            }
            catch (FormulaFormatException formulaEx)
            {
                handleInvalidNameException(formulaEx, contentsOfCell);
                cellValue = sheet.GetCellValue(cellName).ToString();
                FindListModifier(tmpValue, cellValue, cellName, cellsToUpdate);
            }



        }

        /// <summary>
        /// Method that Sets and updates the cells that need updating
        /// </summary>
        private void UpdateCellContents(bool fromFindCell, string contentsFromFindAndReplace)
        {
            List<String> cellsToUpdate = new List<string>();
            int row, col;
            String value;
            spreadsheetPanel1.GetSelection(out col, out row);
            spreadsheetPanel1.GetValue(col, row, out value);

            String cellName = calculateCellName(col) + (row + 1);
            string cellValue = "";
            CellNameTB.Text = cellName;
            string contentsOfCell = "";

            if (fromFindCell)
                contentsOfCell = contentsFromFindAndReplace;
            else
                contentsOfCell = CellContentTB.Text;

            List<string> currentCellName = new List<string>();
            currentCellName.Add(cellName);

            string tmpValue = "";
            List<string> tempList = sheet.GetNamesOfAllNonemptyCells().ToList();
            if (tempList.Contains(cellName))
                tmpValue = sheet.GetCellValue(cellName).ToString();

            try
            {
                cellsToUpdate = sheet.SetContentsOfCell(cellName, contentsOfCell).ToList(); //gets the list of cells to update
                if (sheet.Changed)
                    saveToolStripMenuItem.Enabled = true;

                UpdateCellContentTB(sheet.GetCellContents(cellName).ToString());//updates the cell content TB
                //CellContentTB.Text = sheet.GetCellContents(cellName).ToString();
                UpdateCellValueTB(sheet.GetCellValue(cellName).ToString()); //update the cell value tb
                //CellValueTB.Text = sheet.GetCellValue(cellName).ToString();
                cellValue = sheet.GetCellValue(cellName).ToString();

                FindListModifier(tmpValue, cellValue, cellName, cellsToUpdate);//creates a cell location for the cell List

                spreadsheetPanel1.SetValue(col, row, sheet.GetCellValue(cellName).ToString());

                if (cellsToUpdate.Count > 1)//updates all the cells that need to be
                {
                    cellsToUpdate.RemoveAt(0);
                    updateAllDependentCells(cellsToUpdate);
                }

            }
            catch (CircularException exception)
            {
                handleCircularException(exception, contentsOfCell);
                cellValue = sheet.GetCellValue(cellName).ToString();
                FindListModifier(tmpValue, cellValue, cellName, cellsToUpdate);
            }
            catch (FormulaFormatException formulaEx)
            {
                handleInvalidNameException(formulaEx, contentsOfCell);
                cellValue = sheet.GetCellValue(cellName).ToString();
                FindListModifier(tmpValue, cellValue, cellName, cellsToUpdate);
            }

            //the following does the checks and send a message to the server for the contents that were updated

            string invalidCharTest = "";
            invalidCharTest = contentsOfCell;

            if (invalidCharTest.Contains('\n'))
            {
                //ServertextBox.Invoke(new MethodInvoker(delegate { ServertextBox.Text = ""; }));
                ServertextBox.Invoke(new MethodInvoker(delegate { ServertextBox.Text = "Error, cannot send '\n' in cell text please send a new value"; }));
                sendToServer = false;
                return; //exit function
            }

            if (sendToServer)
            {
                try
                {
                    string cellInfo = "cell " + cellName + " " + contentsOfCell;
                    communicator.SendMessage(cellInfo);
                }
                catch (Exception ex)
                {
                    ServertextBox.Invoke(new MethodInvoker(delegate { ServertextBox.Text = "Could not send the cell information to Server!\n" + ex.Message; }));
                    //MessageBox.Show("Could not send the cell information to Server!\n" + ex.Message);
                    return;
                }
                sendToServer = false;
            }


        }

        /// <summary>
        /// method that creates a list and updates the list if things are removed for the find and replace class
        /// </summary>
        /// <param name="tmpValue"></param>
        /// <param name="cellValue"></param>
        private void FindListModifier(string tmpValue, string cellValue, string cellName, List<String> cellsToUpdate)
        {
            if ((Cell.ContainsKey(tmpValue) && tmpValue != "") || Cell.ContainsKey(cellValue))
            {
                List<string> listName = new List<string>();
                List<string> tmpList = new List<string>();
                //if a value is alread contained in the Cell Dictionary it will go to this if statement
                if (tmpValue != "")
                {
                    Cell.TryGetValue(tmpValue, out listName); //gets a list from the dictionary of all of the cells that contain the tmpValue
                    
                    foreach (string n in listName)
                    {
                        if (!tmpList.Contains(n))
                           tmpList.Add(n); //adds the values into tmpList for manipulation
                        
                    }
                    listName = new List<string>();
                    foreach (string n in tmpList)
                    {
                        listName.Add(n);
                    }
                    int iterator = 0;
                    //iterates through all of the values that are connected toe the tmpList that are broken from the original after a change
                    foreach (string st in cellsToUpdate)
                    {
                        if (iterator == 0)
                        {
                            iterator++;
                            continue;
                        }
                        string temp = sheet.GetCellValue(st).ToString();
                        if (temp == tmpValue)
                        {
                            tmpList.Add(st);
                            listName.Add(st);
                        }
                    }

                    //for each loop that gets the new cell value and compares it to the old and removes it from the tmpList
                    //if it's new value is not the same as the old
                    foreach (string n in listName)
                    {
                        string temp = sheet.GetCellValue(n).ToString();
                        if (temp != tmpValue)
                            tmpList.Remove(n);
                    }

                    listName = tmpList;
                    //if the list is empty it is destroyed
                    if (listName.Count() == 0)
                        Cell.Remove(tmpValue);
                    else
                    {
                        //listName.Add(cellName);
                        Cell.Remove(tmpValue);//remove the list 
                        Cell.Add(tmpValue, listName); //create a new list
                    }
                }
                //if the tmpValue is not the same as the new value it will create a new dictionary set
                if (Cell.ContainsKey(cellValue) && tmpValue != cellValue)
                {

                    listName = new List<string>();
                    //tmpList = new List<string>();
                    Cell.TryGetValue(cellValue, out listName); //puts the current list into the set

                    //destroys the dictionary location if the list is empty
                    if (listName.Count() == 0)
                    {
                        Cell.Remove(cellValue);
                    }
                    else //creates a new dictionary set for the new value
                    {

                        //foreach (string n in listName)
                        //tmpList.Add(n);
                        listName.Add(cellName);

                        if (cellsToUpdate.Count > 1)
                        {
                            int iterator = 0;
                            foreach (string n in cellsToUpdate)
                            {
                                if (iterator == 0)
                                {
                                    iterator++;
                                    continue;
                                }
                                string tmpVal = sheet.GetCellValue(n).ToString();
                                if (cellValue == tmpVal)
                                    listName.Add(n);
                            }
                        }
                        listName.Sort();//sorts the list
                        Cell.Remove(cellValue); //deletes the set
                        Cell.Add(cellValue, listName); //creates a new set
                    }

                }
                else if (!Cell.ContainsKey(cellValue) && tmpValue != cellValue) //if the Dictionary does not contain the value it creates a new one
                {
                    List<string> listCellName = new List<string>();
                    if (cellsToUpdate.Count > 1)
                    {
                        int iterator = 0;
                        foreach (string n in cellsToUpdate)
                        {
                            if (iterator == 0)
                            {
                                iterator++;
                                continue;
                            }
                            string mainVal = sheet.GetCellValue(cellName).ToString();
                            string tmpVal = sheet.GetCellValue(n).ToString();
                            if (tmpVal == mainVal)
                                listCellName.Add(n);
                        }
                    }

                    listCellName.Add(cellName);
                    Cell.Add(cellValue, listCellName); //adding to dictionary for find gui.
                }
                //Cell.Remove();
                //Cell.Add(cellName, cellValue);
            }
            else //creates a Cell set if none of the above sets are true.
            {

                List<string> listName = new List<string>();
                listName.Add(cellName);
                if (cellsToUpdate.Count > 1)
                {
                    int iterator = 0;
                    foreach (string n in cellsToUpdate)
                    {
                        if (iterator == 0)
                        {
                            iterator++;
                            continue;
                        }
                        listName.Add(n);
                    }
                }
                Cell.Add(cellValue, listName); //adding to dictionary for find gui.
            }
        }//end method


        /// <summary>
        /// method created the handle cross threading when updated the Text Boxes
        /// </summary>
        /// <param name="text"></param>
        private void UpdateCellContentTB(string text)
        {
            if (InvokeRequired)
                Invoke(new Action<string>(UpdateCellContentTB), text);
            else
                CellContentTB.Text = text;
        }

        /// <summary>
        /// method created the handle cross threading when updated the Text Boxes
        /// </summary>
        /// <param name="text"></param>
        private void UpdateCellValueTB(string text)
        {
            if (InvokeRequired)
                Invoke(new Action<string>(UpdateCellValueTB), text);
            else
                CellValueTB.Text = text;
        }

        /// <summary>
        /// this method is used to update all of the cells that need to be updated after a cell has been changed
        /// </summary>
        /// <param name="cellsToUpdate"></param>
        private void updateAllDependentCells(List<string> cellsToUpdate)
        {
            for (int i = 0; i < cellsToUpdate.Count; i++)
            {
                String tempCellName = cellsToUpdate.ElementAt(i);
                
                char[] cellNameChar = tempCellName.ToCharArray();

                int row = 0;
                int tempRow = 0;
                int col = 0;
                for (int j = 0; j < cellNameChar.Length; j++)
                {

                    if (int.TryParse(cellNameChar.ElementAt(j).ToString(), out tempRow))
                    {
                        tempRow = tempRow - 1;
                        row += tempRow;
                    }
                    else
                        col += getColValue(cellNameChar.ElementAt(j));

                }
                string tempUpdateCellValue = "";
                spreadsheetPanel1.GetValue(col, row, out tempUpdateCellValue);
                spreadsheetPanel1.SetValue(col, row, sheet.GetCellValue(tempCellName).ToString());
                string currentCellValue = "";
                spreadsheetPanel1.GetValue(col, row, out currentCellValue);
                FindListModifier(tempUpdateCellValue, currentCellValue, tempCellName, cellsToUpdate);//creates a cell location for the cell List
            }
        }

        /// <summary>
        /// method to handle circular exceptions and put the error in the cell
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="formula"></param>
        private void handleCircularException(CircularException exception, string formula)
        {
            int row, col;
            //string value;
            spreadsheetPanel1.GetSelection(out col, out row);

            spreadsheetPanel1.SetValue(col, row, exception.Message);

            string cellName = calculateCellName(col) + (row + 1);
            ServertextBox.Invoke(new MethodInvoker(delegate { ServertextBox.Text = "Circular Dependancy has occurred."; }));
           // sheet.SetContentsOfCell(cellName, (exception.Message + " \"" + formula + "\""));

            //UpdateCellValueTB((exception.Message) + sheet.GetCellValue(cellName).ToString());
            //UpdateCellContentTB((exception.Message) + sheet.GetCellContents(cellName).ToString());
            //CellValueTB.Text = (exception.Message) + sheet.GetCellValue(cellName).ToString();
            //CellContentTB.Text = (exception.Message) + sheet.GetCellContents(cellName).ToString();

        }

        /// <summary>
        /// helper method that gets the value of a cell for reverse of creating a cell.  This is used when setting contents in the cells
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        private int getColValue(char p)
        {
            switch (p)
            {
                case 'A':
                    return 0;
                case 'B':
                    return 1;
                case 'C':
                    return 2;
                case 'D':
                    return 3;
                case 'E':
                    return 4;
                case 'F':
                    return 5;
                case 'G':
                    return 6;
                case 'H':
                    return 7;
                case 'I':
                    return 8;
                case 'J':
                    return 9;
                case 'K':
                    return 10;
                case 'L':
                    return 11;
                case 'M':
                    return 12;
                case 'N':
                    return 13;
                case 'O':
                    return 14;
                case 'P':
                    return 15;
                case 'Q':
                    return 16;
                case 'R':
                    return 17;
                case 'S':
                    return 18;
                case 'T':
                    return 19;
                case 'U':
                    return 20;
                case 'V':
                    return 21;
                case 'W':
                    return 22;
                case 'X':
                    return 23;
                case 'Y':
                    return 24;
                case 'Z':
                    return 25;
                default:
                    return -1;
            }
        }


        /// <summary>
        /// method that uses the calculateCellName to create a cell for the CellNameTB and other functions
        /// </summary>
        /// <param name="col"></param>
        /// <param name="row"></param>
        private void getCellName(int col, int row)
        {
            String cellName = calculateCellName(col) + (row + 1);
            CellNameTB.Text = cellName;
        }

        /// <summary>
        /// switch statement that allows you to get the cell letter to ensure they are updated or selecting the correct cell.
        /// </summary>
        /// <param name="col"></param>
        /// <returns></returns>
        private String calculateCellName(int col)
        {
            switch (col)
            {
                case 0:
                    return "A";
                case 1:
                    return "B";
                case 2:
                    return "C";
                case 3:
                    return "D";
                case 4:
                    return "E";
                case 5:
                    return "F";
                case 6:
                    return "G";
                case 7:
                    return "H";
                case 8:
                    return "I";
                case 9:
                    return "J";
                case 10:
                    return "K";
                case 11:
                    return "L";
                case 12:
                    return "M";
                case 13:
                    return "N";
                case 14:
                    return "O";
                case 15:
                    return "P";
                case 16:
                    return "Q";
                case 17:
                    return "R";
                case 18:
                    return "S";
                case 19:
                    return "T";
                case 20:
                    return "U";
                case 21:
                    return "V";
                case 22:
                    return "W";
                case 23:
                    return "X";
                case 24:
                    return "Y";
                case 25:
                    return "Z";
                default:
                    return "Error on finding cell name";
            }
        }

        /// <summary>
        /// not currently used
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// not currently used
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }


        // Deals with the New menu
        /// <summary>
        /// contained in the file menu creates a new spreadsheet
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void newToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            // Tell the application context to run the form on the same
            // thread as the other forms.
            NewApplicationContext.getAppContext().RunForm(new GUI());
        }

        // Deals with the Close menu
        /// <summary>
        /// contained in the file menu that closes the program
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close(); //closes the program
        }

        /// <summary>
        /// helper method for the save menu item that allows user to save to a file or a location like save as
        /// </summary>
        private void Save()
        {
            if (filePath.Equals("") || filePath != null)
            {
                SaveAs();
            }
            else
            {
                sheet.Save(filePath);
                saveToolStripMenuItem.Enabled = false;
            }

        }

        /// <summary>
        /// contained in the file menu that allows the user to save an existing file or save it to a location.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Save();
        }

        /// <summary>
        /// helper method that controls the saveAs menu item
        /// </summary>
        private void SaveAs()
        {
            //SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            //SaveFileDialog1.CheckFileExists = true;
            //SaveFileDialog1.CheckPathExists = true;
            saveFileDialog1.Title = "Save As";
            saveFileDialog1.InitialDirectory = @"C:\";
            //saveFileDialog1.DefaultExt = "ss";
            saveFileDialog1.Filter = "ss files (*.ss)|*.ss|All files (*.*)|*.*";
            saveFileDialog1.FilterIndex = 1;
            //SaveFileDialog1.ShowDialog();

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {

                if (saveFileDialog1.Filter == "ss files")
                    saveFileDialog1.DefaultExt = "ss";
                string file = saveFileDialog1.FileName;
                sheet.Save(file);
                filePath = file;
                saveToolStripMenuItem.Enabled = false;
            }

        }

        /// <summary>
        /// item contained in the file menu that allows the user to save a file to a location using the file save menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveAs();
        }

        /// <summary>
        /// Creates an open file menu that allows the user to navigate to a file and open it in a new spreadsheet.
        /// this is contained in the file menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Title = "Open";
            openFileDialog1.InitialDirectory = @"C:\";
            //OpenFileDialog1.DefaultExt = "ss";
            openFileDialog1.Filter = "ss files (*.ss)|*.ss|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 1;
            //OpenFileDialog1.ShowDialog();
            //need to implement the spreadsheet xml stuff into this method
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {

                if (openFileDialog1.Filter == "ss files")
                    openFileDialog1.DefaultExt = "ss";


                string file = openFileDialog1.FileName;
                gui = new GUI(file);
                NewApplicationContext.getAppContext().RunForm(gui);

                //string file = openFileDialog1.FileName;
                //sheet = new Spreadsheet(file, isValid, Normalize, "ps6");
                //updateAllDependentCells(sheet.GetNamesOfAllNonemptyCells().ToList());
                //filePath = file;
            }
            saveToolStripMenuItem.Enabled = false;
        }

        /// <summary>
        /// inputs an error message if an invalid name is detected by some chance.
        /// Error handling method nothing more
        /// </summary>
        /// <param name="formulaEx"></param>
        /// <param name="ContentsOfCell"></param>
        private void handleInvalidNameException(FormulaFormatException formulaEx, string ContentsOfCell)
        {
            int row, col;
            //string value;

            spreadsheetPanel1.GetSelection(out col, out row);

            spreadsheetPanel1.SetValue(col, row, formulaEx.Message);

            string cellName = calculateCellName(col) + (row + 1);

            sheet.SetContentsOfCell(cellName, (formulaEx.Message + " \"" + ContentsOfCell + "\""));

            CellValueTB.Text = sheet.GetCellValue(cellName).ToString();
            CellContentTB.Text = sheet.GetCellContents(cellName).ToString();
        }

        /// <summary>
        /// not currently used
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CellNameTB_TextChanged(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// not currently used
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CellContentTB_TextChanged(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// not currently used
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CellValueTB_TextChanged(object sender, EventArgs e)
        {

        }



        /// <summary>
        /// This is a button controlled by a background thread that updates a cell depending on what is contained inside the
        /// CellContentTB.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpdateButton_Click(object sender, EventArgs e)
        {
            sendToServer = true; //when update is pressed set to true to allow message to be sent to server
            if (backgroundWorker1.IsBusy != true)
            {
                // Start the asynchronous operation.
                backgroundWorker1.RunWorkerAsync();
            }

            //setCellContentsUpdateCellContents();
            //if (sheet.Changed)
            //    saveToolStripMenuItem.Enabled = true;
        }

        /// <summary>
        /// menu item that allows the user to get a popup window that displays how to use this spreadsheet
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void howToUseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RichTextBox richTextBox1 = new RichTextBox();

            float height = 500;
            float width = 800;
            Font Title = new Font("Times New Roman", 20f, FontStyle.Bold);
            Font subTitle = new Font("Times New Roman", 16f, FontStyle.Bold);
            Font subText = new Font("Times New Roman", 12f, FontStyle.Regular);
            richTextBox1.Multiline = true;
            richTextBox1.WordWrap = true;
            richTextBox1.SelectionFont = Title;
            richTextBox1.AppendText("How To Use Spreadheet\n");

            richTextBox1.SelectionFont = subTitle;
            richTextBox1.AppendText("\n\tCreating A Cell:");

            richTextBox1.SelectionFont = subText;
            richTextBox1.AppendText("\n\t\tTo create a cell select the cell to create and then in the text" +
             " box field \"Cell Content\" enter the contents you would \t\tlike to be in the cell and either select" +
             " the enter key, or select the button next to \"Cell Content\" labeled \"Update \t\tCell Contents\" ");

            richTextBox1.AppendText("\n\n\t\tThe contents of each cell may contain a string for example a name, a number," +
                " or a formula represented with an =. \t\tTo create a formula select a cell, for example A1 and put a value in it. Then select B1, in B1 " +
                "type \"=A1\", then \t\t\tselect enter. This creates a formula and B1 will now contain the value of A1.");

            richTextBox1.SelectionFont = subTitle;
            richTextBox1.AppendText("\n\n\tLoading A Spreadsheet:\n");

            richTextBox1.SelectionFont = subText;
            richTextBox1.AppendText("\t\tTo load a spreadsheet from a file of type .ss by selecting the option in the open popup for ss files, or you can"
            + " \t\t\tchoose all files and get any file type.  Do this by selecting  \"file\" \"Open\" or use Hotkey \"ctrl+o\"\n");

            richTextBox1.SelectionFont = subTitle;
            richTextBox1.AppendText("\n\tSaving A Spreadsheet:\n");

            richTextBox1.SelectionFont = subText;
            richTextBox1.AppendText("\t\tTo save a spreadsheet to a specific file location select \"file\" \"Save As\" and select destination" +
                " of where you would \t\tlike to save." + "\n\t\tTo save a file to its current file location select \"file\" \"Save\" or use Hotkey \"ctrl+s\"\n");

            richTextBox1.SelectionFont = subTitle;
            richTextBox1.AppendText("\n\tFind & Replace:\n");
            richTextBox1.SelectionFont = subText;
            richTextBox1.AppendText("\t\tTo use the find function you click the button on the menu that says \"Find\" or by pressing \"ctrl + f\".  A popup will \t\tappear that allows you to search for "
                + "values contained in the cell.  You can also replace the values contained in the \t\tcell selected using this method.   ");

            richTextBox1.Size = System.Drawing.Size.Round(new SizeF(width, height));
            richTextBox1.ReadOnly = true;

            Form RichMessageBox = new Form();

            RichMessageBox.Controls.Add(richTextBox1);
            RichMessageBox.StartPosition = FormStartPosition.CenterScreen;
            RichMessageBox.Size = System.Drawing.Size.Round(new SizeF(width, height));
            RichMessageBox.Name = "How to use spreadsheet";
            RichMessageBox.ShowDialog();
        }

        private void spreadsheetPanel1_Load(object sender, EventArgs e)
        {

        }

        private void CellNameLabel_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// This method creates a new thread to take care of calculations and inputs in the the Text Boxes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            if ((worker.CancellationPending == true))
            {
                e.Cancel = true;
                //break;
            }
            else
            {
                UpdateCellContents(false, "");
                backgroundWorker1.CancelAsync();
                //if (sheet.Changed)
                //saveToolStripMenuItem.Enabled = true;
            }
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {

        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

        }

        /// <summary>
        /// creates the find tool popup
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void findToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Find findLocation = new Find(spreadsheetPanel1, sheet, Cell, navigateToCell, UpdateCellContents);
            findLocation.Visible = true;
        }

        /// <summary>
        /// Navigates the the cell when you click on it
        /// </summary>
        /// <param name="cellName"></param>
        public void navigateToCell(string cellName)
        {
            displaySelection(spreadsheetPanel1);
        }

        private void ConnectButton_Click(object sender, EventArgs e)
        {
            //if ConnectButton.Text == Connect then the user hasn't connected to the server. once it has the text will change to register user
            if (ConnectButton.Text == "Connect")
            {
                //GUICells.Clear(); //clear the cells so that it only contains what from the server
                spreadsheetPanel1.Invalidate();
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
            else if (ConnectButton.Text == "Register User")
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

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            communicator.SendMessage("undo");
        }









    }
}
