﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace BoggleClient
{
    /// <summary>
    /// Each value in the enum refers to a tab in the tabControl1.
    /// </summary>
    enum view {welcome=0,play,gameSummary};

    public partial class BoggleClientForm : Form
    {

        private Action SetupInv(Action a)
        {
            return () => { try { this.Invoke((MethodInvoker)delegate { a(); }); } catch { } };
        }

        private Action<T> SetupInv<T>(Action<T> a)
        {
            return (t) => { try { this.Invoke((MethodInvoker)delegate { a(t); }); } catch { } };
        }

        private Action<Exception, string> SetupInv(Action<Exception, string> a)
        {
            return (e, s) => { try { this.Invoke((MethodInvoker)delegate { a(e, s); }); } catch { } };
        }

        private Action<int, int> SetupInv(Action<int, int> a)
        {
            return (one, two) => { try { this.Invoke((MethodInvoker)delegate { a(one, two); }); } catch { } };
        }

        TextBox[] resultWordTextBoxes;
        Label[] LetterBoxes;
        public ClientModel model;

        public BoggleClientForm()
        {
            InitializeComponent();

            //Create the model part of the MVC and give it the ability to change the view
            model = new ClientModel();

            //Make the tabs invisible to the user
            tabControl1.Top = tabControl1.Top - tabControl1.ItemSize.Height;
            tabControl1.Height = tabControl1.Height + tabControl1.ItemSize.Height;
            tabControl1.Region = new Region(new RectangleF(StartMenu.Left, StartMenu.Top, StartMenu.Width, StartMenu.Height + tabControl1.ItemSize.Height));
            
            //Setup events
            model.GameStarted = SetupInv(this.GameStarted);
            model.GameFinished = SetupInv<List<HashSet<string>>>(this.GameFinished);
            model.GameTerminated = SetupInv(this.GameTerminated);
            model.UpdateBoggleBoard = SetupInv<string>(this.UpdateBoggleBoard);
            model.UpdateGameScore = SetupInv(this.UpdateGameScore);
            model.UpdateGuessedWordsList = SetupInv<HashSet<string>>(this.UpdateGuessedWordsList);
            model.UpdateOpponentName = SetupInv<string>(this.UpdateOpponentName);
            model.UpdateTimeLeftInGame = SetupInv<int>(this.UpdateTimeLeftInGame);
            model.ConnectionClosed = SetupInv<disconnectReason>(this.ConnectionClosed);
            model.FailedToSendWord = SetupInv(this.FailedToSendWord);
           
            //group all of the resultWordTextBoxes
            resultWordTextBoxes = new TextBox[5]{MyLegalWordTextBox,OpponentsLegalWordsTextBox,WordsInCommonTextBox,MyIllegalWordsTextBox,OpponentsIllegalWordsTextBox};
            LetterBoxes = new Label[16] { LetterSquare1, LetterSquare2, LetterSquare3, LetterSquare4, LetterSquare5, LetterSquare6, LetterSquare7, LetterSquare8, LetterSquare9, LetterSquare10, LetterSquare11, LetterSquare12, LetterSquare13, LetterSquare14, LetterSquare15, LetterSquare16 };
        }



        /// <summary>
        /// Changes which view the player is looking at.
        /// </summary>
        /// <param name="changeViewTo"></param>
        /// <returns>True if successfully changed view.</returns>
        internal bool changeView(view changeViewTo)
        {
            //If the changeViewTo is within the range of tabs that exist
            if ((int)changeViewTo <= tabControl1.TabCount)
            {
                //Then switch to that tab
                tabControl1.SelectedIndex = (int)changeViewTo;
                return true;
            }
            else //changeViewTo index did not refer to an existing tab.
                return false;
        }

        /// <summary>
        /// Is called when the game has started.
        /// </summary>
        private void GameStarted()
        {
            changeView(view.play);
        }


        private void FailedToSendWord(Exception arg1, string arg2)
        {
            //Do nothing
        }

        private void ConnectionClosed(disconnectReason obj)
        {
            PlayConnectButton.Text = "Play";
            PlayConnectButton.Enabled = true;
            if (obj == disconnectReason.clientLeftGame)
            {
                GameTerminated();
            }
            else if (obj == disconnectReason.serverLeftGame)
            {
                GameTerminated();
            }
        }

        private void GameTerminated()
        {
            foreach (TextBox t in resultWordTextBoxes)
                t.Visible = false;

            changeView(view.gameSummary);
        }

        private void GameFinished(List<HashSet<string>> obj)
        {
            
            //make sure all the boxes are displayed
            foreach (TextBox t in resultWordTextBoxes)
                t.Visible = true;

            //clear then repopulate the text boxes with the correct data
            foreach (TextBox t in resultWordTextBoxes)
                t.Clear();

            //If we have at least 5 HashSets of words then continue
            if (obj.Count >= 5)
            {
                string[] words;
                //Loop though each hashSet
                for (int i = 0; i < 5; i++)
                {
                    words = obj[i].ToArray<string>();

                    for (int j = 0; j < words.Length; j++)
                    {
                        string s = words[j];
                        s = s.Replace("%%", "~^%&");
                        s = s.Replace("%S", " ");
                        s = s.Replace("~^%&", "%");
                        //Add each word in the current hashSet to the correct box
                        resultWordTextBoxes[i].AppendText(s + '\n');
                    }
                }

            }

            //show the game over screen
            changeView(view.gameSummary);
        }

        private void UpdateTimeLeftInGame(int obj)
        {
            GameTimeDisplay.Text = obj.ToString();
            TimeRemainingDisplay.Text = obj.ToString();
        }

        private void UpdateOpponentName(string obj)
        {
            TheirScoreLabel.Text = ":"+obj; //odd quirk of displaying the name backwards
            OpponentsScoreLabel.Text = obj;
        }

        private void UpdateGuessedWordsList(HashSet<string> obj)
        {
            ListOfGuessedWords.Clear();
            foreach (String s in obj)
            {
                ListOfGuessedWords.AppendText(s+'\n');
            }
        }

        private void UpdateGameScore(int arg1, int arg2)
        {
            //Update our score
            OurScoreDisplay.Text = arg1.ToString();
            MyScoreDisplay.Text = arg1.ToString();
            //update their score
            TheirScoreDisplay.Text = arg2.ToString();
            OpponentsScoreDisplay.Text = arg2.ToString();

        }

        private void UpdateBoggleBoard(string obj)
        {
            //Make sure the board given is of valid length
            if (obj!=null && obj.Length == 16)
            {
                //Apply the letters to each box in the table
                for (int i = 0; i < 16; i++)
                {
                    LetterBoxes[i].Text = obj[i].ToString();
                }
            }
        }


        private void PlayerNameTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 13)
                PlayConnectButton_Click(sender, e);
        }

        private void PlayConnectButton_Click(object sender, EventArgs e)
        {
            //  MessageBox.Show(model.ConnectToServer("localhost"));
            //changeView(view.play);
            if (PlayerNameTextBox.Text == "")
            {
                MessageBox.Show("Please enter a name.");
                return;
            }
            if(ServerIPTextBox.Text == "")
            {
                MessageBox.Show("Please enter the server's IP Address.");
                return;
            }
                
            PlayConnectButton.Enabled = false;
            PlayConnectButton.Text = "Connecting...";
            model.ConnectToServer(ServerIPTextBox.Text, 
                SetupInv(connectedToServer), 
                SetupInv<string>(failedToConnectToServer));
        }


        private void connectedToServer()
        {
            model.SendName(PlayerNameTextBox.Text);
            PlayConnectButton.Text = "Waiting for partner...";
        }

        private void failedToConnectToServer(String errorMessage)
        {
            PlayConnectButton.Enabled = true;
            MessageBox.Show("Could not connect to Server!\n" + errorMessage.ToString());
            PlayConnectButton.Text = "Play";
        }


        private void SendButton_Click(object sender, EventArgs e)
        {
            //Send the text in the box
            model.GuessWord(GuessWordTextBox.Text);
            //clear the previous word
            GuessWordTextBox.Clear();
        }

        private void GuessWordTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 13)
                SendButton_Click(sender, e);
        }

        private void returnToWelcomeScreenButton_Click(object sender, EventArgs e)
        {
            changeView(view.welcome);
        }

        private void welcomeDisconnectFromServerButton_Click(object sender, EventArgs e)
        {
            model.WeClosedConnection(disconnectReason.clientLeftWhileWaiting);
        }

        private void playDisconnectFromServerButton_Click(object sender, EventArgs e)
        {
            model.WeClosedConnection(disconnectReason.clientLeftGame);
        }

        private void tabControl1_Selected(object sender, TabControlEventArgs e)
        {
            PlayerNameTextBox.Focus();
        }

        private void BoggleClientForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            model.WeClosedConnection(disconnectReason.clientLeftGame);
        }





    }
}
