//Scott Young
//U0824833
//PS6
//11/4/2013

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpreadsheetGUI
{
    /// <summary>
    /// Class that creates a Form to control all of the find methods and replace methods.
    /// </summary>
    public partial class Find : Form
    {
        //Long list of global controls and variables
        private SS.SpreadsheetPanel spreadsheetPanel1;
        private SS.Spreadsheet sheet;
        //private GUI gui;
        private Dictionary<string, List<string>> Cell;
        private List<string> cellList;
        private GoToCell navigate;
        //private UpdateCell updateCell;
        private bool boolFind;
        private bool FindNextLoop;
        //private bool replaced;//if the cell is replaced
        private SetContentPanel setContent;
        public delegate void GoToCell(string name);
        public delegate void UpdateCell(string cellContent);
        public delegate void SetContentPanel(bool update, string cellContent);

        /// <summary>
        /// default constructor that is not used
        /// </summary>
        //public Find()
        //{
        //    InitializeComponent();

        //}

        /// <summary>
        /// Constructor that sets up all of the values that control all of the methods in this class
        /// </summary>
        /// <param name="_spreadsheetPanel1"></param>
        /// <param name="_sheet"></param>
        /// <param name="_Cell"></param>
        /// <param name="_navigate"></param>
        /// <param name="_setContent"></param>
        public Find(SS.SpreadsheetPanel _spreadsheetPanel1, SS.Spreadsheet _sheet, Dictionary<string, List<string>> _Cell, GoToCell _navigate, SetContentPanel _setContent)
        {
            InitializeComponent();
            this.spreadsheetPanel1 = _spreadsheetPanel1; //connects to the spreadsheet from the GUI
            this.sheet = _sheet; //connects to the sheet from the GUI class
            this.navigate = _navigate; //method that when the find is set it will navigate to a cell
            this.setContent = _setContent; //uses the method from GUI class the change the values if the replace is set
            cellList = new List<string>();  //gets the list of cells that contain a certain value
            backgroundWorker1.WorkerSupportsCancellation = true;
            backgroundWorker2.WorkerSupportsCancellation = true;
            boolFind = false;
            //this.gui = _gui;
            FindNextLoop = true;
            //replaced = false;
            this.Cell = _Cell;
            textBoxFind.KeyUp += new KeyEventHandler(textBoxFind_KeyUp);
            textBoxReplace.KeyUp += new KeyEventHandler(textBoxReplace_KeyUp);
        }


        /// <summary>
        /// Sets up the textBoxFind so that it will accept if you push the enter key
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxFind_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                string FindValue = textBoxFind.Text;
                if (FindValue.Equals("") || FindValue == null || !Cell.ContainsKey(FindValue))
                {
                    String message = "Cell containing requested value could not be found";
                    String title = "Find";
                    MessageBox.Show(message, title);
                }
                else if (Cell.ContainsKey(FindValue))
                {
                    //if (FindValue.Equals("") || FindValue == null || !Cell.ContainsKey(FindValue))
                    //{
                    //    String message = "Cell containing requested value could not be found";
                    //    String title = "Find";
                    //    MessageBox.Show(message, title);
                    //    ButtonFindNext.Visible = false;
                    //}
                    //else
                    //{
                    //IEnumerable<string> tempList = sheet.GetNamesOfAllNonemptyCells();
                    cellList = new List<string>();
                    Cell.TryGetValue(FindValue, out cellList);
                    cellList.Sort();
                    navigateToCellInSpreadsheet(cellList[0]);
                    if (cellList.Count() > 1)
                    {
                        ButtonFindNext.Visible = true;
                        buttonFind.Enabled = false;
                    }
                    else
                        ButtonFindNext.Visible = false;
                    boolFind = true;
                    //}
                    //foreach (string n in tempList)
                    //{
                    //    string value = sheet.GetCellValue(n).ToString();
                    //    if (value.Equals(FindValue))
                    //    {
                    //        string cellName = n;
                    //        navigateToCellInSpreadsheet(cellName);


                    //    }
                    //}
                }
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="charCellName"></param>
        /// <param name="row"></param>
        /// <param name="col"></param>
        private void calculateRowCol(char[] charCellName, out int row, out int col)
        {
            int tempRow = 0;
            int tempCol = 0;
            String stringRow = "";

            for (int ix = 0; ix < charCellName.Length; ix++)
            {
                if (int.TryParse(charCellName.ElementAt(ix).ToString(), out tempRow))
                {
                    stringRow = stringRow + charCellName.ElementAt(ix);
                }
                else
                    tempCol += getColValue(charCellName.ElementAt(ix));
            }

            row = (Convert.ToInt32(stringRow)) - 1;
            col = tempCol;
        }

        /// <summary>
        /// 
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
        /// 
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
        /// 
        /// </summary>
        /// <param name="cellName"></param>
        private void navigateToCellInSpreadsheet(string cellName)
        {
            int row, col;

            calculateRowCol(cellName.ToCharArray(), out row, out col);

            spreadsheetPanel1.SetSelection(col, row);
            navigate(cellName);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Replace_CheckedChanged(object sender, EventArgs e)
        {
            if (Replace.Checked == true)
            {
                textBoxReplace.Visible = true;
                buttonReplace.Visible = true;
            }
            else if (Replace.Checked == false)
            {
                textBoxReplace.Visible = false;
                buttonReplace.Visible = false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxFind_TextChanged(object sender, EventArgs e)
        {
            buttonFind.Enabled = true;
            ButtonFindNext.Visible = false;
            backgroundWorker2.CancelAsync();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxReplace_TextChanged(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxReplace_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                //string replaceContent = textBoxReplace.Text;
                //setContent(true, replaceContent);
                if (backgroundWorker1.IsBusy != true)
                {
                    // Start the asynchronous operation.
                    backgroundWorker1.RunWorkerAsync();
                }

            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonReplace_Click(object sender, EventArgs e)
        {
            if (backgroundWorker1.IsBusy != true)
            {
                // Start the asynchronous operation.
                backgroundWorker1.RunWorkerAsync();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Find_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 
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
                string FindValue = textBoxReplace.Text;
                string replaceContent = textBoxReplace.Text;
                setContent(true, replaceContent);
                EnableFindButton();
                //buttonFind.Enabled = true;
                DisableFindNextButton();
                backgroundWorker2.CancelAsync();
                boolFind = true;
                //ButtonFindNext.Visible = false; //fix with method
                backgroundWorker1.CancelAsync();
            }
        }

        /// <summary>
        /// Holds the loop for findNextLoop so that it won't change the location until the button is pressed
        /// </summary>
        private void HoldFindNext()
        {
            FindNextLoop = true;
            while (FindNextLoop)
            {
            }
            FindNextLoop = false;
        }

        private void DisableFindButton()
        {
            if (InvokeRequired)
                Invoke(new Action(DisableFindButton));
            else
                buttonFind.Enabled = false;

        }

        private void DisableFindNextButton()
        {
            if (InvokeRequired)
                Invoke(new Action(DisableFindNextButton));
            else
                ButtonFindNext.Visible = false;

        }

        private void EnableFindNextButton()
        {
            if (InvokeRequired)
                Invoke(new Action(EnableFindNextButton));
            else
                ButtonFindNext.Visible = true;

        }

        private void EnableFindButton()
        {
            if (InvokeRequired)
                Invoke(new Action(EnableFindButton));
            else
                buttonFind.Enabled = true;

        }

        /// <summary>
        /// 
        /// </summary>
        private void FindNext()
        {
            if (cellList.Count() == 1)
            {
                MessageBox.Show("There are no more cells containing this value.", "Find Next");
                ButtonFindNext.Visible = false;
            }
            else
            {
                for (int i = 0; i < cellList.Count(); i++)
                {
                    DisableFindButton();
                    if (boolFind)
                    {
                        i++;
                        boolFind = false;
                    }

                    //if (replaced)
                    //{
                    //    i = 1;
                    //    replaced = false;
                        
                    //}
                    if (i > cellList.Count() - 1)
                        i = 0;

                    navigateToCellInSpreadsheet(cellList[i]);

                    HoldFindNext();
                    //if(cellList.Count() == 0)

                    if (i == cellList.Count() - 1)
                    {
                        //var result = MessageBox.Show("There are no more cells containing this value. \n Do you want to return to the first cell?", "Find Next", MessageBoxButtons.YesNo);
                        //if (result == DialogResult.No)
                        //    continue;
                        //else
                        //{
                            i = 0;
                            navigateToCellInSpreadsheet(cellList[0]);
                            HoldFindNext();
                        //}
                    }
                }

            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonFindNext_Click(object sender, EventArgs e)
        {
            if (backgroundWorker2.IsBusy != true)
            {
                // Start the asynchronous operation.
                backgroundWorker2.RunWorkerAsync();
            }

            FindNextLoop = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            if ((worker.CancellationPending == true))
            {
                e.Cancel = true;
                //break;
            }
            else
            {

                FindNext();
                backgroundWorker2.CancelAsync();
                DisableFindNextButton();
                EnableFindButton();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonFind_Click(object sender, EventArgs e)
        {
            string FindValue = textBoxFind.Text;
            if (FindValue.Equals("") || FindValue == null || !Cell.ContainsKey(FindValue))
            {
                String message = "Cell containing requested value could not be found";
                String title = "Find";
                MessageBox.Show(message, title);
            }
            else if (Cell.ContainsKey(FindValue))
            {
                //if (FindValue.Equals("") || FindValue == null || !Cell.ContainsKey(FindValue))
                //{
                //    String message = "Cell containing requested value could not be found";
                //    String title = "Find";
                //    MessageBox.Show(message, title);
                //    ButtonFindNext.Visible = false;
                //}
                //else
                //{
                //IEnumerable<string> tempList = sheet.GetNamesOfAllNonemptyCells();
                cellList = new List<string>();
                Cell.TryGetValue(FindValue, out cellList);
                cellList.Sort();
                navigateToCellInSpreadsheet(cellList[0]);
                if (cellList.Count() > 1)
                {
                    ButtonFindNext.Visible = true;
                    buttonFind.Enabled = false;
                }
                else
                    ButtonFindNext.Visible = false;
                boolFind = true;
                //}
                //foreach (string n in tempList)
                //{
                //    string value = sheet.GetCellValue(n).ToString();
                //    if (value.Equals(FindValue))
                //    {
                //        string cellName = n;
                //        navigateToCellInSpreadsheet(cellName);


                //    }
                //}
            }

        }


    }
}
