using CustomNetworking;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BoggleClient
{
    public partial class BoggleForm : Form
    {
        BoggleModel model;

        public BoggleForm()
        {
            InitializeComponent();
            model = new BoggleModel();
            model.IncomingScoreEvent += ScoreReceived;
            model.IncomingStartEvent += StartReceived;
            model.IncomingStopEvent += StopReceived;
            model.IncomingTerminatedEvent += TerminatedReceived;
            model.IncomingTimeEvent += TimeReceived;
            model.ServerEvent += ServerReceived;
            
            this.AcceptButton = buttonMain;
        }

        private void StartReceived(string[] msg)
        {
            // update game status field
            labelGameStatus.Invoke(new Action(() => { labelGameStatus.Text = "Game in progress"; }));

            // display the board
            letter1.Invoke(new Action(() => { letter1.Text = msg[1][0] + "\r\n"; }));
            letter2.Invoke(new Action(() => { letter2.Text = msg[1][1] + "\r\n"; }));
            letter3.Invoke(new Action(() => { letter3.Text = msg[1][2] + "\r\n"; }));
            letter4.Invoke(new Action(() => { letter4.Text = msg[1][3] + "\r\n"; }));
            letter5.Invoke(new Action(() => { letter5.Text = msg[1][4] + "\r\n"; }));
            letter6.Invoke(new Action(() => { letter6.Text = msg[1][5] + "\r\n"; }));
            letter7.Invoke(new Action(() => { letter7.Text = msg[1][6] + "\r\n"; }));
            letter8.Invoke(new Action(() => { letter8.Text = msg[1][7] + "\r\n"; }));
            letter9.Invoke(new Action(() => { letter9.Text = msg[1][8] + "\r\n"; }));
            letter10.Invoke(new Action(() => { letter10.Text = msg[1][9] + "\r\n"; }));
            letter11.Invoke(new Action(() => { letter11.Text = msg[1][10] + "\r\n"; }));
            letter12.Invoke(new Action(() => { letter12.Text = msg[1][11] + "\r\n"; }));
            letter13.Invoke(new Action(() => { letter13.Text = msg[1][12] + "\r\n"; }));
            letter14.Invoke(new Action(() => { letter14.Text = msg[1][13] + "\r\n"; }));
            letter15.Invoke(new Action(() => { letter15.Text = msg[1][14] + "\r\n"; }));
            letter16.Invoke(new Action(() => { letter16.Text = msg[1][15] + "\r\n"; }));

            // Set the game timer to max
            labelRemainingTime.Invoke(new Action(() => { labelRemainingTime.Text = msg[2] + "\r\n"; }));

            // Display the opponent's name and score 
            labelOpponentName.Invoke(new Action(() => { labelOpponentName.Text = msg[3] + "\r\n"; }));
            labelOpponentScore.Invoke(new Action(() => { labelOpponentScore.Text = "0" + "\r\n"; }));
        }

        private void TimeReceived(string msg)
        {
            labelRemainingTime.Invoke(new Action(() => { labelRemainingTime.Text = msg + "\r\n"; }));
        }

        private void ScoreReceived(string[] msg)
        {
            labelOwnScore.Invoke(new Action(() => { labelOwnScore.Text = msg[1] + "\r\n"; }));
            labelOpponentScore.Invoke(new Action(() => { labelOpponentScore.Text = msg[2] + "\r\n"; }));            
        }

        private void StopReceived(string[] msg)
        {
            // update game status field
            labelGameStatus.Invoke(new Action(() => { labelGameStatus.Text = "Game time over"; }));

            int[] numbers = new int[5];
            string[] endMessage = new string[5];
            string player1 = labelOwnName.Text.Trim();
            string player2 = labelOpponentName.Text.Trim();
            endMessage[0] = player1 + " played the following valid word/s:";
            endMessage[1] = player2 + " played the following valid word/s:";
            endMessage[2] = "You both played the following word/s:";
            endMessage[3] = player1 + " played the following invalid word/s:";
            endMessage[4] = player2 + " played the following invalid word/s:";
            int j = 1;
            for (int i = 0 ; i < 5 ; i++)
            {
                int counter;
                int.TryParse(msg[j], out numbers[i]);
                counter = numbers[i];
                j++;
                while(counter > 0)
                {
                    endMessage[i] += " " + msg[j];
                    counter--;
                    j++;
                }
            }

            // display game stats in pop up window
            MessageBox.Show(endMessage[0] + "\n\n" + endMessage[1] + "\n\n" + endMessage[2] + "\n\n" + endMessage[3] + "\n\n" + endMessage[4]);
                        
            buttonMain.Invoke(new Action(() => { buttonMain.Text = "Play Again" + "\r\n"; }));
        }

        private void TerminatedReceived(string msg)
        {
            // update game status field
            labelGameStatus.Invoke(new Action(() => { labelGameStatus.Text = "Game terminated"; }));

            MessageBox.Show("Opponent has disconnected!");
            buttonMain.Invoke(new Action(() => { buttonMain.Text = "Play Again" + "\r\n"; }));
        }

        private void buttonMain_Click(object sender, EventArgs e)
        {           

            if(buttonMain.Text == "Quit")
            {
                this.AcceptButton = buttonMain;
                //model.socket.Close();
                model.Disconnect();
                buttonMain.Invoke(new Action(() => { buttonMain.Text = "Connect"; }));
                ResetBoard();

                // update game status field
                labelGameStatus.Invoke(new Action(() => { labelGameStatus.Text = "Game terminated"; }));

                return;                
            }

            this.AcceptButton = buttonSend;

            ResetBoard();
                        
            // if name field is blank
            if(textBoxName.Text == "")
            {
                MessageBox.Show("Please enter a name.");
                return;
            }
            // if IP field is blank
            if (textBoxIP.Text == "")
            {
                MessageBox.Show("Please enter an IP address.");
                return;
            }

            try
            {
                model.Connect(textBoxIP.Text, textBoxName.Text, 2000);

                // Display own name and score
                labelOwnName.Invoke(new Action(() => { labelOwnName.Text = textBoxName.Text; }));
                labelOwnScore.Invoke(new Action(() => { labelOwnScore.Text = "0"; }));
                buttonMain.Invoke(new Action(() => { buttonMain.Text = "Quit"; }));

                // update game status field
                labelGameStatus.Invoke(new Action(() => { labelGameStatus.Text = "Searching for game"; }));
            }            

            catch (Exception)
            {
                labelGameStatus.Invoke(new Action(() => { labelGameStatus.Text = "Server could not be reached"; }));
            }
            
        }

         private void buttonSend_Click(object sender, EventArgs e)
        {
            string word = textBoxWord.Text.Trim();
            word = "WORD " + word;
            model.SendMessage(word);            
            textBoxWord.Text = "";
        }

         private void ServerReceived(string msg)
         {
             // update game status field
             labelGameStatus.Invoke(new Action(() => { labelGameStatus.Text = "The server has crashed"; }));
             buttonMain.Invoke(new Action(() => { buttonMain.Text = "Connect" + "\r\n"; }));
             
         }
         
        
        private void ResetBoard()
        {
            // reset the 16 dice to read "-"
            letter1.Invoke(new Action(() => { letter1.Text = "-"; }));
            letter2.Invoke(new Action(() => { letter2.Text = "-"; }));
            letter3.Invoke(new Action(() => { letter3.Text = "-"; }));
            letter4.Invoke(new Action(() => { letter4.Text = "-"; }));
            letter5.Invoke(new Action(() => { letter5.Text = "-"; }));
            letter6.Invoke(new Action(() => { letter6.Text = "-"; }));
            letter7.Invoke(new Action(() => { letter7.Text = "-"; }));
            letter8.Invoke(new Action(() => { letter8.Text = "-"; }));
            letter9.Invoke(new Action(() => { letter9.Text = "-"; }));
            letter10.Invoke(new Action(() => { letter10.Text = "-"; }));
            letter11.Invoke(new Action(() => { letter11.Text = "-"; }));
            letter12.Invoke(new Action(() => { letter12.Text = "-"; }));
            letter13.Invoke(new Action(() => { letter13.Text = "-"; }));
            letter14.Invoke(new Action(() => { letter14.Text = "-"; }));
            letter15.Invoke(new Action(() => { letter15.Text = "-"; }));
            letter16.Invoke(new Action(() => { letter16.Text = "-"; }));

            // set other GUI elements back to a default state
            labelOwnName.Invoke(new Action(() => { labelOwnName.Text = "You"; }));
            labelOwnScore.Invoke(new Action(() => { labelOwnScore.Text = "0"; }));
            labelOpponentName.Invoke(new Action(() => { labelOpponentName.Text = "Opponent"; }));
            labelOpponentScore.Invoke(new Action(() => { labelOpponentScore.Text = "0"; }));
            labelRemainingTime.Invoke(new Action(() => { labelRemainingTime.Text = "0"; }));
            labelGameStatus.Invoke(new Action(() => { labelGameStatus.Text = ""; }));
        }

        private void BoggleForm_Load(object sender, EventArgs e)
        {

        }  
                
    }
}
