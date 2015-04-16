// Author: Taylor Morris and Dharani Adhikari
// CS 3500, Fall 2014

using CustomNetworking;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Boggle
{
    /// <summary>
    /// The Game class holds all information needed to run one game of boggle.  Data included are 2 players, 
    /// a dictionary of valid words, game timer, game board, and a list of common words played by both players
    /// for scoring purposes.
    /// 
    /// Each second the game sends a message to both players decrementing the game timer, also, as often as the
    /// game score changes a score update message is sent to each player.C:\Users\dharaniadhikari\Documents\Visual Studio 2013\Projects\PS8(1)\BoggleServer\Game.cs
    /// 
    /// When the timer runs out a final score message is sent as well as a game summary, detailing what words
    /// were played by each player.
    /// </summary>
    class Game
    {
        //Member Variables test
        private Player p1;
        private Player p2;

        private HashSet<String> commonWords = new HashSet<string>();
        private HashSet<String> wordDictionary;

        private int counter;
        private Timer timer;
        private Object gameLock = new Object();

        private BoggleBoard board;
        private bool isGameActive = true;
        private int timeOfGame = 0;
        private int currentGameID;

        /// <summary>
        /// The connection string.
        /// Your CADE login name serves as both your database name and your uid
        /// Your uNID serves as your password
        /// </summary>
        private const string connectionString = "server=atr.eng.utah.edu;database=cs3500_tmorris;uid=cs3500_tmorris;password=183014191";

        /// <summary>
        /// THe constructor for our game class, it contains everything related to one game of boggle:  the players, 
        /// the timer, the board, and a couple other odds and ends.
        /// </summary>
        /// <param name="player1">First player</param>
        /// <param name="player2">Second player</param>
        /// <param name="time">The duration of the game in seconds</param>
        /// <param name="words">The dictionary for the game</param>
        /// <param name="s">The 16 characters the comprise the board, randomly generated if blank</param>
        public Game(Player player1, Player player2, int time, HashSet<String> words, String s = null)
        {
            // set initial values
            wordDictionary = words;

            p1 = player1;
            p2 = player2;
            timeOfGame = time;
            counter = time;

            if (s == null)
            {
                board = new BoggleBoard();
            }
            else
            {
                board = new BoggleBoard(s);
            }

            // Send a message to each player's client indicating the game has begun.  This message contains each player's name and
            // the board that will be used.
            p1.stringSocket.BeginSend("START " + board.ToString() + " " + counter + " " + p2.name + "\n", (ee, pp) => { }, null);
            p2.stringSocket.BeginSend("START " + board.ToString() + " " + counter + " " + p1.name + "\n", (ee, pp) => { }, null);

            // Start listening for words from the players
            p1.stringSocket.BeginReceive(wordReceived, p1);
            p2.stringSocket.BeginReceive(wordReceived, p2);

            // Create a timer with a one second interval
            timer = new System.Timers.Timer(1000);

            // Hook up the Elapsed event for the timer
            timer.Elapsed += new ElapsedEventHandler(updateTime);

            // begin the game.
            timer.Start();
        }

        /// <summary>
        /// Event listener, runs when the internal timer ticks (each second).  Updates the game time and sends 
        /// the new value to the players each second.  Also sends final score and end of game report once the 
        /// timer hits 0
        /// </summary>
        /// <param name="sender">The timer this method is listening to</param>
        /// <param name="e">the timer event, "tick"</param>
        private void updateTime(object sender, ElapsedEventArgs e)
        {
            // decrement the game time
            counter--;

            // send the updated time to each player
            p1.stringSocket.BeginSend("TIME " + counter + "\n", (ee, pp) => { }, null);
            p2.stringSocket.BeginSend("TIME " + counter + "\n", (ee, pp) => { }, null);

            // if the game is over

            if (counter < 1)
            {
                timer.Stop();
                isGameActive = false;

                // Sends the score to each player
                displayScore();

                // Sends the game summary to each player
                p1.stringSocket.BeginSend("STOP " + GroupWords(p1.validWords) + GroupWords(p2.validWords) + 
                                                    GroupWords(commonWords) + GroupWords(p1.invalidWords) +
                                                    GroupWords(p2.invalidWords).Trim() + "\n", closeSocket, p1.stringSocket);

                p2.stringSocket.BeginSend("STOP " + GroupWords(p2.validWords) + GroupWords(p1.validWords) +
                                                    GroupWords(commonWords) + GroupWords(p2.invalidWords) +
                                                    GroupWords(p1.invalidWords).Trim() + "\n", closeSocket, p2.stringSocket);
                // Create connection
                //using Connection conn = new Connection 
                sendToDatabase();

            }

        }

        /// <summary>
        /// this function sends all of the game, player, and word information to the database
        /// </summary>
        private void sendToDatabase()
        {
            //get the player id, if they do not exist create them
            int p1ID = getPlayerID(p1.name);
            int p2ID = getPlayerID(p2.name);

            if (p1ID == 0)
            {
                //player isnt in database, add them
                insertPlayer(p1.name);
                p1ID = getPlayerID(p1.name);
            }

            if (p2ID == 0)
            {
                //player isnt in database, add them
                insertPlayer(p2.name);
                p2ID = getPlayerID(p2.name);
            }

            //update the players win/lose/ties
            updatePlayerRecord(p1ID, p1.score, p2.score);
            updatePlayerRecord(p2ID, p2.score, p1.score);

            //put the game into the database
            insertGame(p1ID, p2ID);

            //get the new games id
            

            //insert all the words into the database
            insertWords(p1ID, p1.validWords, 1);
            insertWords(p1ID, p1.invalidWords, 0);
            insertWords(0, commonWords, 1);
            insertWords(p2ID, p2.validWords, 1);
            insertWords(p2ID, p2.invalidWords, 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        private int getPlayerID(string player)
        {
            int playerID = 0;
            // first, we need a player ID
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    // Open a connection
                    conn.Open();

                    // Create a command
                    MySqlCommand command = conn.CreateCommand();
                    command.CommandText = "SELECT PlayerID from Players where Players.Name = '" + player + "'";

                    // Execute the command and cycle through the DataReader object
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            playerID = (int)reader["PlayerID"];
                        }
                    }

                    return playerID;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return 0;
                }
                finally
                {
                    conn.Close();
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="player"></param>
        private void insertPlayer(string name)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    // Open a connection
                    conn.Open();

                    // Create a command
                    MySqlCommand command = conn.CreateCommand();
                    command.CommandText = "INSERT INTO Players (Name, Win, Loss, Tie) VALUES (@name, 0, 0, 0)";
                    command.Parameters.AddWithValue("@name", name);
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        private void updatePlayerRecord(int PID, int myScore, int oppScore)
        {
            int win = 0;
            int loss = 0;
            int tie = 0;
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    // Open a connection
                    conn.Open();

                    // Create a command
                    MySqlCommand command = conn.CreateCommand();
                    command.CommandText = "SELECT Win, Loss, Tie FROM Players WHERE PlayerID = " + PID;

                    command.ExecuteNonQuery();

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            win = (int)reader["Win"];
                            loss = (int)reader["Loss"];
                            tie = (int)reader["Tie"];
                        }
                    }

                    if(myScore > oppScore)
                    {
                        win++;
                    }
                    else if (myScore == oppScore)
                    {
                        tie++;
                    }
                    else
                        loss++;

                    // update the values in the player table
                    command.CommandText = "UPDATE Players SET Win = " + win + ", Loss = " + loss + ", Tie = " + tie + " WHERE PlayerID = " + PID;
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    conn.Close();
                }

            }
        }

        private void insertGame(int p1ID, int p2ID)
        {
            string boardString = board.ToString();
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    // Open a connection
                    conn.Open();

                    // Create a command
                    MySqlCommand command = conn.CreateCommand();
                    command.CommandText = "INSERT INTO Games (P1ID, P2ID, Date, Board, GameTime, P1Score, P2Score) VALUES (" + p1ID +", " + p2ID + ", " + "Now(), '" + boardString + "', " + timeOfGame + ", " + p1.score + ", " +p2.score +")";
                    // execute the query to insert the values in to the game table
                    command.ExecuteNonQuery();


                    command.CommandText = "SELECT GameID FROM Games ORDER BY GameID DESC LIMIT 1";
                    command.ExecuteNonQuery();

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        reader.Read();
                        currentGameID = (int)reader["GameID"];
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    conn.Close();
                }
            }
        }

       
        private void insertWords(int PID, HashSet<string> wordList, int valid)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    // Open a connection
                    conn.Open();
                    
                    foreach (string s in wordList)
                    {
                        // Create a command
                        MySqlCommand command = conn.CreateCommand();
                        command.CommandText = "INSERT INTO Words (Word, GameID, PlayerID, Valid) VALUES ('" + s + "', " +currentGameID + ", " + PID + ", " + valid + ")";
                        // execute the query to insert the values in to the game table
                        command.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    conn.Close();
                }
            }
        }


        /// <summary>
        /// Private callback method which is invoked when STOP message is finished being transmitted
        /// </summary>
        /// <param name="e"></param>
        /// <param name="payload"></param>
        private void closeSocket(Exception e, Object payload)
        {
            // Cast the object to StringSocket and close the socket
            StringSocket ss = (StringSocket)payload;
            ss.Close();
        }

        /// <summary>
        /// Method for handling messages received from players.  First, checks whether the message is null, indicating the client has disconnected and closes their
        /// socket in that event.  
        /// </summary>
        /// <param name="msg">the message being received from the client</param>
        /// <param name="e"> callback exception</param>
        /// <param name="payload">callback payload - the player</param>
        private void wordReceived(string msg, Exception e, object payload)
        {
            lock (gameLock)
            {
                if (isGameActive)
                {
                    // Identifies the player who sent the message by the payload
                    Player caller = (Player)payload;
                    // if no messages are received, the connection is closed
                    if(msg == null)
                    {
                        if (caller == p1)
                        {
                            p2.stringSocket.BeginSend("TERMINATED" + "\n", (ee, pp) => { }, null);
                            p2.stringSocket.Close();
                        }
                        else
                        {
                            p1.stringSocket.BeginSend("TERMINATED" + "\n", (ee, pp) => { }, null);
                            p1.stringSocket.Close();
                        }
                        Console.WriteLine("TERMINATED" + "\n");
                        isGameActive = false;
                        return;
                    }

                  //  msg = msg.ToUpper();
                    string temp = msg;
                    msg = msg.ToUpper();
                    if (msg.StartsWith("WORD"))
                    {
                        msg = msg.Substring(4);
                        // Remove leading white spaces if they exist
                        msg = msg.Trim();

                        // if the word is only 1 or 2 letters long we ignore it
                        if (msg.Length < 3)
                        {
                            caller.stringSocket.BeginReceive(wordReceived, caller);
                            return;
                        }

                        if (board.CanBeFormed(msg))
                        {
                            // the word can be formed is in the dictionary
                            if (wordDictionary.Contains(msg))
                            {
                                // we have a valid word, we need to update the score.
                                if (commonWords.Contains(msg))
                                {
                                    caller.stringSocket.BeginReceive(wordReceived, caller);
                                    return;
                                }

                                if (!caller.validWords.Contains(msg))
                                {
                                    updateScore(msg, caller);
                                    Console.WriteLine("updateScore 1\n");
                                }
                            }
                            // if the word does not exist in the dictionary add it to the Invalid words list of the player
                            else
                            {
                                if (caller.invalidWords.Add(msg))
                                    caller.score--;
                                displayScore();
                                Console.WriteLine("updateScore 2\n");
                            }


                        }
                        // Add invalid word to the player's invalid words list
                        else
                        {
                            if (caller.invalidWords.Add(msg))
                                caller.score--;
                            displayScore();
                            Console.WriteLine("displayScore 1\n");
                        }
                    }
                    else
                    {
                        caller.stringSocket.BeginSend("IGNORING " + temp + "\n", (ee, pp) => { }, null);
                        Console.WriteLine("IGNORING " + temp + "\n");
                    }

                    caller.stringSocket.BeginReceive(wordReceived, caller);
                }
            }
        }

        /// <summary>
        /// This method first sets the worth of the word being played based on its length, then checks whether it already exists in
        /// any of the games various word lists and updates player scores appropriately.
        /// </summary>
        /// <param name="word">the word that has been played</param>
        /// <param name="p">the player who send the word</param>
        private void updateScore(String word, Player p)
        {
            int worth = 0;
            // update the scores based on the protocol
            if (word.Length == 3 || word.Length == 4)
                worth = 1;
            else if (word.Length == 5)
                worth = 2;
            else if (word.Length == 6)
                worth = 3;
            else if (word.Length == 7)
                worth = 5;
            else
                worth = 11;

            // check words against lists and handle score updates
            if (p == p1)
            {
                // if the word is common in p2
                if (p2.validWords.Contains(word))
                {
                    p2.score -= worth;
                    commonWords.Add(word);
                    p2.validWords.Remove(word);
                }
                else
                {
                    p1.validWords.Add(word);
                    p1.score += worth;
                }
            }
            if(p == p2)
            {
                // if the word is common in p1
                if (p1.validWords.Contains(word))
                {
                    p1.score -= worth;
                    commonWords.Add(word);
                    p1.validWords.Remove(word);
                }
                else
                {
                    p2.validWords.Add(word);
                    p2.score += worth;
                }
            }
            // display the updated score to each players board 
            displayScore();
            Console.WriteLine("displayScore 2\n");

        }

        /// <summary>
        /// A private method to display the score for both players
        /// </summary>
        private void displayScore()
        {
            p1.stringSocket.BeginSend("SCORE " + p1.score + " " + p2.score + "\n", (ee, pp) => { }, null);
            p2.stringSocket.BeginSend("SCORE " + p2.score + " " + p1.score + "\n", (ee, pp) => { }, null);
            Console.Write("SCORE P1: " + p1.score + " P2: " + p2.score + "\n");
        }

        /// <summary>
        /// Private method to group all strings together for display purpose
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private string GroupWords(HashSet<string> list)
        {
            string msgString = list.Count.ToString() + " ";
            // combine all strings to make a long string for display purpose
            foreach(string s in list)
            {
                msgString += s + " ";
            }
            return msgString;
        }
    }
}
