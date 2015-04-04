using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace BB
{
    /// <summary>
    /// Given two player connections, a new Boggle game created and manages rules and communications between the players.
    /// </summary>
    class BoggleGame
    {
        /// <summary>
        /// The starting game time in seconds.
        /// </summary>
        private int gameTime;
        /// <summary>
        /// The custom configuration of the boggle board (if any)
        /// </summary>
        private string customConfig;
        /// <summary>
        /// A reference to the dictionary of loaded legal words.
        /// </summary>
        private HashSet<string> dictionary;
        /// <summary>
        /// Player 1's connection
        /// </summary>
        private PlayerConnection playerConnection1;
        /// <summary>
        /// Player 2's connection
        /// </summary>
        private PlayerConnection playerConnection2;
        /// <summary>
        /// The boggle board used for this game.
        /// </summary>
        private BoggleBoard board;
        /// <summary>
        /// The amount of time left in seconds.
        /// </summary>
        private int timeLeft;
        /// <summary>
        /// Used to make sure the time doesn't get messed up by 
        /// parallel threads.
        /// </summary>
        private Object time_Lock = new Object();
        /// <summary>
        /// when set the main run method continues as if the game is over.
        /// </summary>
        ManualResetEvent gameOverNotification;
        /// <summary>
        /// The timer.
        /// </summary>
        System.Timers.Timer timer;
        /// <summary>
        /// The list of legal common words that both players have played in this boggle game.
        /// </summary>
        private HashSet<String> InCommonList;
        /// <summary>
        /// A reference to the database manager for this server which adds database functionality.
        /// </summary>
        private BoggleDatabaseManager dbManager;

        /// <summary>
        /// Makes sure that only one word is scored at a time.
        /// </summary>
        private Object scoreWordLock = new Object();

        /// <summary>
        /// Sets up a boggle game (use .run() to start the game)
        /// </summary>
        /// <param name="gameTime">The starting game time in seconds</param>
        /// <param name="customConfig">The custom configuration of the boggle board (if any, otherwise it's set to null)</param>
        /// <param name="dictionary">A reference to the dictionary of loaded legal words.</param>
        /// <param name="playerConnection1">Player 1's connection</param>
        /// <param name="playerConnection2">Player 2's connection</param>
        /// <param name="dbManager">A reference to the database manager for this server which adds database functionality.</param>
        public BoggleGame(int gameTime, string customConfig, HashSet<string> dictionary, PlayerConnection playerConnection1, PlayerConnection playerConnection2, BoggleDatabaseManager dbManager)
        {
            //Save all of the necessary game information.
            this.gameTime = gameTime;
            this.customConfig = customConfig;
            this.dictionary = dictionary;
            this.playerConnection1 = playerConnection1;
            this.playerConnection2 = playerConnection2;
            this.InCommonList = new HashSet<String>();
            this.dbManager = dbManager;

            //Generate the board, use the custom config if we have one.
            if (customConfig == null)
                this.board = new BoggleBoard();
            else
            {
                //Attempt to use the custom board configuration provided
                try
                {
                    this.board = new BoggleBoard(customConfig);
                }
                //The configuration was not valid, create a new random board and report it
                catch (Exception)
                {
                    this.board = new BoggleBoard();
                    Console.WriteLine("Invalid boggle board. Using " + board.ToString());
                }
            }
            timeLeft = gameTime;
            //Set the initial state to "Non-signaled" 
            //When the gameOverNotification is set that means the timer has run out.
            gameOverNotification = new ManualResetEvent(false);

            //Catches the disconnect messages
            playerConnection1.setDirectOutputTo(PlayerHasDisconnected, null);
            playerConnection2.setDirectOutputTo(PlayerHasDisconnected, null);

        }

        /// <summary>
        /// This runs the boggle game between 2 clients
        /// using the thread that called it.
        /// </summary>
        public void run(Object ThisIsIrrelevant)
        {
            Console.WriteLine("Starting game using: "+board);

            //Set up the timer
            timer = new System.Timers.Timer(1000);
            //When 1000 milliseconds has passed then run the timer_Elapsed code
            timer.Elapsed += timer_Elapsed;

            //Assuring that BOTH previous locks have been released before proceeding.
            lock (playerConnection1.GagLock)
            {
                lock (playerConnection2.GagLock)
                {
                }
            }

            //Assigns where to send received messages now that the game is going to start
            playerConnection1.setDirectOutputTo(PlayerHasDisconnected, receivedFromPlayer);
            playerConnection2.setDirectOutputTo(PlayerHasDisconnected, receivedFromPlayer);

            //Announce the start of the game
            playerConnection1.SendMessage("START " + board + " " + gameTime + " " + playerConnection2.nameOfPlayer);
            playerConnection2.SendMessage("START " + board + " " + gameTime + " " + playerConnection1.nameOfPlayer);

            //Start the timer
            timer.Start();

            //Hold this thread until the timer runs out or a player disconnects
            gameOverNotification.WaitOne();

            //close both playersockets if they're still open
            playerConnection1.CloseSocketConnection();
            playerConnection2.CloseSocketConnection();

            //Done with this game and thread.
            Console.WriteLine("Game is done.");
        }

        private void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            //Just in case the time threads stalled for some reason
            lock (time_Lock)
            {
                //One less second has passed
                timeLeft -= 1;

                //Send the current time to each player
                if (timeLeft >= 0)
                    BroadCastToBoth("TIME " + timeLeft);

                //Check if game is over
                if (timeLeft == 0)
                {
                    timer.Stop();
                    //Time has run out
                    StopAcceptingInput();

                    //transmits final scores
                    ScoreBroadcast();

                    //transmit game summary
                    endGameAndSendSummary();

                    //record game results in database using DatabaseManager
                    if(dbManager!=null)
                        ThreadPool.QueueUserWorkItem(
                            o=>dbManager.recordGameResults(
                                playerConnection1.nameOfPlayer,
                                playerConnection2.nameOfPlayer,
                                board.ToString(),
                                gameTime,
                                playerConnection1.score,
                                playerConnection2.score,
                                playerConnection1.LegalList,
                                playerConnection2.LegalList,
                                InCommonList,
                                playerConnection1.IllegalList,
                                playerConnection2.IllegalList));


                    //Continue the run() method
                    gameOverNotification.Set();
                }
            }
        }

        /// <summary>
        /// Tells the clients that the game is over and sends them the summary of the game.
        /// </summary>
        private void endGameAndSendSummary()
        {
            //Get the formatted lists from all the lists
            String stopFormat = "STOP{0}{1}{2}{3}{4}";
            String p1Legal = spaceCountWords(playerConnection1.LegalList);
            String p2Legal = spaceCountWords(playerConnection2.LegalList);
            String inCom = spaceCountWords(InCommonList);
            String p1Ill = spaceCountWords(playerConnection1.IllegalList);
            String p2Ill = spaceCountWords(playerConnection2.IllegalList);

            //Transmit the game summaries respectively
            playerConnection1.SendMessage(String.Format(stopFormat,
                                                                 p1Legal,
                                                                 p2Legal,
                                                                 inCom,
                                                                 p1Ill,
                                                                 p2Ill));
            playerConnection2.SendMessage(String.Format(stopFormat,
                                                                 p2Legal,
                                                                 p1Legal,
                                                                 inCom,
                                                                 p2Ill,
                                                                 p1Ill));
        }

        /// <summary>
        /// Helper method.
        /// Returns " a #1" where 'a' is the count of the words and '#1' is the list 
        /// of words separated by spaces (no trailing spaces).
        /// </summary>
        /// <param name="list">The list of words to format</param>
        /// <returns></returns>
        private String spaceCountWords(HashSet<String> list)
        {
            //start with a space
            StringBuilder toReturn = new StringBuilder(" ");
            //Add the count 
            toReturn.Append(list.Count);
            //loop through all the words in the list and append them with a preceeding space
            foreach (String s in list){
                toReturn.Append(" ");
                toReturn.Append(FormatWord(s));
            }
            //return the result
            return toReturn.ToString();
        }

        /// <summary>
        /// Encodes any spaces that the player put in a word guess.
        /// Turns all '%' into '%%' then any spaces into '%s'
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        private String FormatWord(String word)
        {
            //first, format any special slashes that they put in there
            word = word.Replace(@"%", @"%%");
            //next encode any spaces they put in
            word = word.Replace(" ", @"%s");

            return word;
        }

        /// <summary>
        /// Sets the player connections to refuse any further messages from the players and
        /// sets the gameOverNotification which declares that the time is up (game is over).
        /// </summary>
        private void StopAcceptingInput() 
        {
            //Ignore any more commands sent by the players 
            playerConnection1.setDirectOutputTo(null,null);
            playerConnection2.setDirectOutputTo(null,null);

        }

        /// <summary>
        /// Method that is called when a player disconnects.
        /// </summary>
        /// <param name="player">The player that disconnected</param>
        private void PlayerHasDisconnected(PlayerConnection player)
        {
            //Don't need timer anymore
            timer.Stop();
            //Stop accepting messages from either player
            StopAcceptingInput();
            //Tell them they were terminated
            BroadCastToBoth("TERMINATED");
            //Finish the run method
            gameOverNotification.Set();
            
        }


        /// <summary>
        /// Function called when the client sends a message while in game.
        /// </summary>
        /// <param name="messageInfo">Information associated with the message received.</param>
        public void receivedFromPlayer(MessageReceivedFrom messageInfo)
        {
            //This is the sender of the word
            PlayerConnection client = messageInfo.playerconnection;
            //This is figuring out and referencing the opponent of the word sender
            PlayerConnection opponent = messageInfo.playerconnection == playerConnection1 ? playerConnection2 : playerConnection1;

            //when scoring a word, make sure no other words are being scored
            lock (scoreWordLock)
            {
                //Makes sure that the proper command has been received.
                if (messageInfo.message.Length > 5 
                    && messageInfo.message.Substring(0, 5).ToUpper().Equals("WORD "))
                {

                    int wordPoints = 0;
                    String word = messageInfo.message.Substring(5).ToUpper();

                    //Word must be longer than 2 letters
                    if (word.Length < 3)
                        return; //simply do nothing, don't even send an ignore message

                    //Check the dictionary to see if word is a valid word and check if it can be made on the board
                    if (dictionary.Contains(word) && board.CanBeFormed(word))
                    {
                        //calculate points scored for using word
                        switch (word.Length)
                        {
                            case 3:
                            case 4:
                                wordPoints = 1;
                                break;
                            case 5:
                                wordPoints = 2;
                                break;
                            case 6:
                                wordPoints = 3;
                                break;
                            case 7:
                                wordPoints = 5;
                                break;
                            default:
                                wordPoints = 11;
                                break;
                        }

                        //check if in opponents legallist
                        if (opponent.LegalList.Contains(word))
                        {
                            //yes? remove word and points from opponents score/list and move word to incommon
                            opponent.LegalList.Remove(word);
                            opponent.score -= wordPoints;
                            InCommonList.Add(word);
                            ScoreBroadcast();
                        }
                        //if in incommon list or in own legallist
                        else if (InCommonList.Contains(word) || client.LegalList.Contains(word))
                        {
                            //do nothing
                            return;
                            //messageInfo.playerconnection.SendIgnoreMessage(messageInfo.message);
                        }
                        else
                        {
                            //YAY points!
                            client.score += wordPoints;
                            //add the word to your own list
                            client.LegalList.Add(word);
                            //Notify players of the score change
                            ScoreBroadcast();
                        }
                    }
                    else //if the word is either not in the dictionary or can't be formed on the board
                    {
                        //Ignore the word if it has been tried already
                        if (!client.IllegalList.Contains(word))
                        {   //otherwise, lose points
                            client.score--;
                            client.IllegalList.Add(word);
                            ScoreBroadcast();
                        }
                    }
                }
                else //if they didn't send a proper "WORD" command
                {
                    //ignore their message
                    messageInfo.playerconnection.SendIgnoreMessage(messageInfo.message);
                }
            }
        }

        /// <summary>
        /// Sends a message to the two players in the game
        /// </summary>
        /// <param name="s">The message to be sent</param>
        private void BroadCastToBoth(string s)
        {
            playerConnection1.SendMessage(s);
            playerConnection2.SendMessage(s);
        }

        /// <summary>
        /// Sends the score to each player 
        /// </summary>
        private void ScoreBroadcast()
        {
            //Make sure a word isn't being scored
            lock (scoreWordLock)
            {
                //Send score with their score first, then their opponents
                playerConnection1.SendMessage("SCORE " + playerConnection1.score + " " + playerConnection2.score);
                playerConnection2.SendMessage("SCORE " + playerConnection2.score + " " + playerConnection1.score);
            }
        }


    }
}
