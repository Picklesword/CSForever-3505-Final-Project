﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace spreadsheetclient
{
    /// <summary>
    /// Used to describe what the client is doing right now.
    /// </summary>
    public enum clientIs { notConnectedToServer, waitingForPartner, playingGame };

    public enum disconnectReason {Unknown, serverUnexpected, serverTerminated, serverStop, clientLeftWhileWaiting, clientLeftGame , serverLeftWhileWaiting, serverLeftGame};

    /// <summary>
    /// Describes a zero-parameter an void returning method.
    /// </summary>
    public delegate void Action();

    /// <summary>
    /// Maintains all the data and underlying logic.
    /// </summary>
    public class ClientModel
    {
        /// <summary>
        /// Describes what the client is doing and what kind of messages it expects to receive.
        /// </summary>
        private clientIs status;

        /// <summary>
        /// Holds the current connection to a server if any, otherwise reference is null.
        /// </summary>
        SocketConnection connection;

        public bool weDisconnected = false;
        public disconnectReason whyWeDisconnected = disconnectReason.Unknown;
        public disconnectReason whyServerDisconnected = disconnectReason.serverUnexpected;

            /*
             * server disconnect when waiting for partner
             * server disconnect while playing game
             * 
             * serverUnexpectedDisconnect
             * serverTerminatedConnection
             * serverStoppedConnection
             * weDisconnectedWhileWaiting
             * weLeftTheGame
             * 
             */

        /// <summary>
        /// Called when we get disconnected from the server. Can happen at any time.
        /// Passes the exception that occured when the connection was lost (null if there wasn't an
        /// exception, just a regular disconnect.)
        /// </summary>
        public Action<disconnectReason> ConnectionClosed;

        /// <summary>
        /// Called when the boggle board is updated/changed.
        /// Passes the 16 character string describing the boggle board.
        /// </summary>
        public Action<String> UpdateBoggleBoard;

        /// <summary>
        /// Called when we get the opponents name sent to us at the beginning of a game.
        /// Passes the name of the opponent.
        /// </summary>
        public Action<String> UpdateOpponentName;

        /// <summary>
        /// Called when the boggle game starts.
        /// Doesn't pass any parameters.
        /// </summary>
        public Action GameStarted;

        /// <summary>
        /// Called when the time gets updated.
        /// Passes the time left in the game.
        /// </summary>
        public Action<int> UpdateTimeLeftInGame;

        /// <summary>
        /// Called when the score gets updated.
        /// First parameter is our score, second parameter is opponent's score.
        /// </summary>
        public Action<int,int> UpdateGameScore;

        /// <summary>
        /// Called when another word has been guessed.
        /// Passes the Set of all the words we have guessed.
        /// </summary>
        public Action<HashSet<String>> UpdateGuessedWordsList;

        /// <summary>
        /// Called when the game gets terminated early by the server (other player disconnects).
        /// Doesn't pass anything.
        /// </summary>
        public Action GameTerminated;

        /// <summary>
        /// Called when the game finishes.
        /// Passes a List of 5 HashSets:
        /// [0]: Our legal words not played by opponent
        /// [1]: Opponent's legalwords that weren't played by the client
        /// [2]: Legal words played by both players
        /// [3]: Our illegal words played
        /// [4]: Opponent's illegal words played
        /// </summary>
        public Action<List<HashSet<String>>> GameFinished;

        /// <summary>
        /// Called when we sent a word but it was rejected.
        /// Passes the Exception that occured and the command 
        /// that we had attempted to send.
        /// </summary>
        public Action<Exception,String> FailedToSendWord;

        public const string CONNECTED_MSG = "Successfully connected to server";
        public const string DISCONNECTED_MSG = "Lost connection with server";

        /// <summary>
        /// Holds the string of 16 letters that describe the boggle board.
        /// </summary>
        public string BoggleBoard
        {
            get;
            private set;
        }

        /// <summary>
        /// Holds the amount of time left in the current or previous game.
        /// </summary>
        private int timeLeftInGame;

        /// <summary>
        /// Holds the name of the opponent in the current or previous game.
        /// </summary>
        private string opponentsName;

        /// <summary>
        /// Set to track which words have been guessed by the client
        /// </summary>
        private HashSet<String> clientGuessedWords;

        /// <summary>
        /// Variables to keep track of the score
        /// </summary>
        private int scoreUs, scoreThem;

        /// <summary>
        /// Should only contain 5 HashSets:
        /// [0]: Our legal words not played by opponent
        /// [1]: Opponent's legalwords that weren't played by the client
        /// [2]: Legal words played by both players
        /// [3]: Our illegal words played
        /// [4]: Opponent's illegal words played
        /// </summary>
        List<HashSet<String>> endGameStats;



        /// <summary>
        /// The constructor which starts off the client with a "notConnectedToserver" status.
        /// </summary>
        public ClientModel()
        {
            this.status = clientIs.notConnectedToServer;
        }

        /// <summary>
        /// A non-stopping function that attempts to connect to the given server.
        /// If you want to use the stopping version use ConnectToServer(string s)
        /// </summary>
        /// <param name="s">The server to connect to (like "localhost" or "192.168.1.3")</param>
        /// <param name="callWhenConnected">The function to call when we have successfully connected to the server.</param>
        /// <returns>Either the CONNECTED_MSG or an "Error Connecting: " message stating why we couldn't connect.</returns>
        public void ConnectToServer(string s,Action callWhenConnected, Action<String> failedToConnect)
        {
            ThreadPool.QueueUserWorkItem((o) => { _connectToServer(s,callWhenConnected,failedToConnect); });
        }

        /// <summary>
        /// A stopping function that attempts to connect to the given server.
        /// If you want to use the non-stopping version use ConnectToServer(string s,Action string callWhenConnected)
        /// </summary>
        /// <param name="s">The server to connect to (like "localhost" or "192.168.1.3")</param>
        /// <returns>Either the CONNECTED_MSG or an "Error Connecting: " message stating why we couldn't connect.</returns>
        private void _connectToServer(string s,Action callWhenConnected,Action<String> failedToConnect)
        {
            //if we have not connected to a server or the connection is not active
            if (!weAreConnected())
            {
                try
                {
                    //Setup the connection
                    TcpClient client = new TcpClient(s, 2112); //Connect on port 2000 by default

                    //Grab the client socket and pass it to the SocketConnection manager
                    connection = new SocketConnection(client.Client, WhenDisconnected, MessageReceived);

                    //success
                    callWhenConnected();
                    return;
                }
                catch (Exception e)
                {
                    //return the error message
                    failedToConnect(e.Message);
                    return;
                }

            }
            else //if we do have a connection
            {
                failedToConnect("Error Connecting: Already connected to a server.");
            }
        }

        /// <summary>
        /// Determines if we have a connection stored and if that connection is active.
        /// </summary>
        /// <returns>True if we are connected to a server.</returns>
        private bool weAreConnected()
        {
            return connection != null && connection.isConnected();
        }


        /// <summary>
        /// Client sends a command to the server.
        /// </summary>
        /// <param name="command">The string that is the command</param>
        /// <param name="failedToSend">Delegate called if the command failed to send even though we were connected</param>
        /// <returns>True if we have a connection established and tried sending the command.</returns>
        public bool sendCommand(string command, Action<Exception,String> failedToSend)
        {
            if (weAreConnected())
            {
                //Use the proper Protocol for sending a word
                connection.SendMessage(command,
                    (e, o) => { 
                        if (e != null && failedToSend != null) 
                            failedToSend(e, o.ToString()); 
                    }); //if we recieved an error message sending the work
                return true;
            }
            return false;
        }

        /// <summary>
        /// Client attempts to send a name to the server.
        /// </summary>
        /// <param name="name">The name sent to identify the client in the boggle game</param>
        /// <returns></returns>
        public bool SendName(string name)
        {
            status = clientIs.waitingForPartner;
            whyServerDisconnected = disconnectReason.serverLeftWhileWaiting;
            return sendCommand("play " + name, (e, s) => { this.WeClosedConnection(disconnectReason.clientLeftWhileWaiting); });
        }

        /// <summary>
        /// Sends a word-guess to the server.
        /// </summary>
        /// <param name="wordToGuess">The word the client is guessing for the boggle game.</param>
        /// <returns>True if we have a connection established and tried sending the word.</returns>
        public bool GuessWord(string wordToGuess)
        {
            clientGuessedWords.Add(wordToGuess);
            UpdateGuessedWordsList(clientGuessedWords);
            return sendCommand("word " + wordToGuess, FailedToSendWord);
        }

        /// <summary>
        /// Called when connection to server is lost.
        /// This method is activated when we close the connection.
        /// </summary>
        /// <param name="sc">Socket that lost connection</param>
        /// <param name="e">Exception associated with the disconnect. Null if disconnected on purpose</param>
        private void WhenDisconnected(SocketConnection sc, Exception e)
        {
            //Notify the view of dicsconnect
            if (ConnectionClosed != null)
            {
                if (weDisconnected)
                {
                    ConnectionClosed(whyWeDisconnected);
                }
                else
                {
                    ConnectionClosed(whyServerDisconnected);
                }
            }
        }

        private void MessageReceived(MessageReceivedFrom m)
        {
            //grab the message that was received
            string message = m.message.ToUpper();
            string[] splinters = message.Split(' ');
           
            //Check on status to determine what to do with the received message
            if (status == clientIs.waitingForPartner)
            {
                if (splinters[0] =="ONE")
                {
                    Console.WriteLine("we made it");
                }
                //Process only the commands that we expect
                if (splinters[0]=="START")
                {
                    //if the message has more than 4 splinters then we assume the extra splinters are part of the opponent's name.
                    if (splinters.Length >= 4)
                    {
                        //validate each splinter
                        //check to make sure the boggle board is valid
                        if (!isValidBoggleBoard(splinters[1]))
                            return; //IGNORED
                        else
                            BoggleBoard = splinters[1];

                        //check to make sure the time is valid
                        int time;
                        if (int.TryParse(splinters[2], out time))
                            timeLeftInGame = time;
                        else //if the time was invalid
                            return; //IGNORED

                        //process opponents name
                        opponentsName ="";
                        for (int i = 3; i < splinters.Length; i++)
                            opponentsName += splinters[i];

                        //***Now start the game!***
                        status = clientIs.playingGame;
                        whyServerDisconnected = disconnectReason.serverLeftGame;

                        //Update the views boggleboard
                        if(UpdateBoggleBoard != null)
                            UpdateBoggleBoard(BoggleBoard);

                        //Reset the scores for initial display
                        scoreUs = 0;
                        scoreThem = 0;
                        if(UpdateGameScore != null)
                            UpdateGameScore(scoreUs,scoreThem);

                        //Reset list of guessed words
                        clientGuessedWords = new HashSet<String>();
                        if(UpdateGuessedWordsList != null)
                            UpdateGuessedWordsList(clientGuessedWords);

                        //Reset the time
                        if(UpdateTimeLeftInGame != null)
                            UpdateTimeLeftInGame(timeLeftInGame);

                        //Update opponents name for the new game
                        if(UpdateOpponentName != null)
                            UpdateOpponentName(opponentsName);

                        //Notify client of game start
                        if(GameStarted != null)
                            GameStarted();

                    } //otherwise the start command has too few argruments with it. Ignored.
                }
            }
            else if (status == clientIs.playingGame)
            {
                //deal with time
                if (splinters[0]=="TIME")
                {
                    //Check for valid format
                    if (splinters.Length != 2)
                        return; //ignore this message

                    //Try extracting the time from the message
                    int timeParse = 0;
                    if (int.TryParse(splinters[1], out timeParse))
                    {
                        //Success, update the time
                        timeLeftInGame = timeParse;
                        if(UpdateTimeLeftInGame != null)
                            UpdateTimeLeftInGame(timeLeftInGame);
                    }
                    else //if the time didn't parse correctly
                        return; //ignore this message
                } //deal with score
                else if (splinters[0]=="SCORE")
                {
                    //check format
                    if (splinters.Length != 3)
                        return; //invalid format

                    //try extracting the score
                    int s1;
                    int s2;
                    //Both must parse correctly
                    if (int.TryParse(splinters[1], out s1) && int.TryParse(splinters[2], out s2))
                    {
                        //update the scores and notify the View
                        scoreUs = s1;
                        scoreThem = s2;
                        if(UpdateGameScore != null)
                            UpdateGameScore(scoreUs, scoreThem);
                    }
                    else //at least one of the scores didn't parse correctly
                        return; //ignore the message

                } //deal with stop
                else if (splinters[0] == "STOP")
                {
                    List<HashSet<String>> tmpWordList = new List<HashSet<string>>();
                    HashSet<String> ExtractedList = new HashSet<String>();
                    //start the cursor at the first number
                    int cursor = 1;
                    for (int i = 0; i < 5; i++)
                    {
                        //try to extract the words
                        ExtractedList = wordExtractor(ref cursor, splinters);

                        //check if there was a problem extracting list
                        if (ExtractedList == null)
                            return; //we're done here

                        //Add the extracted list.
                        tmpWordList.Add(ExtractedList);
                    }

                    //check if we ended up at the end of the word list like we should
                    if (cursor != splinters.Length)
                        return; 

                    //otherwise, we've extracted the whole list correctly

                    whyServerDisconnected = disconnectReason.serverStop;
                    //Make sure the connection is closed
                    this.MakeSureConnectionClosed();

                    endGameStats = tmpWordList;
                    if (GameFinished != null)
                        GameFinished(endGameStats);

                } //deal with terminated
                else if (splinters[0] == "TERMINATED")
                {
                    //make sure connection is closed
                    whyServerDisconnected = disconnectReason.serverTerminated;
                    this.MakeSureConnectionClosed();

                    //notify the view
                    if(GameTerminated != null)
                        GameTerminated();
                }
                //otherwise ignore the command

            }

        }

        /// <summary>
        /// Helper method for extracting lists of words from the STOP command.
        /// </summary>
        /// <param name="cursor">Place in the splinters array that has the number of following words</param>
        /// <returns>Either the hashset of extracted words or null if invalid format</returns>
        private HashSet<string> wordExtractor(ref int cursor, string[] splinters)
        {
            //check if cursor is out of bounds
            if (cursor >= splinters.Length)
                return null;

            //Figure out how many words to extract
            int count=0;
            if (!int.TryParse(splinters[cursor], out count))
                return null;

            //Increment cursor and get the upper bound
            count += ++cursor;

            //check if upper bound is out of bounds
            if (count > splinters.Length)
                return null;

            //extract words
            HashSet<String> extracted = new HashSet<String>();
            for (; cursor < count; cursor++)
            {
                extracted.Add(splinters[cursor]);
            }
            
            //return the list
            return extracted;
        }


        /// <summary>
        /// Determines if the given board is a valid boggle board.
        /// </summary>
        /// <param name="suspectBoggleBoard">The string of characters to analyze</param>
        /// <returns></returns>
        private bool isValidBoggleBoard(string suspectBoggleBoard)
        {
            //Make sure there is something to analyze
            if (suspectBoggleBoard == null)
                return false;

            // Use upper case
            suspectBoggleBoard = suspectBoggleBoard.ToUpper();
            // Make sure letters are legal
            if (suspectBoggleBoard.Length != 16)
            {
                //then ignore message
                return false;
            }
            //check to make sure each character is a letter
            foreach (char c in suspectBoggleBoard)
            {
                if (!Char.IsLetter(c))
                {
                    //ignore
                    return false;
                }
            }

            //Otherwise it is valid
            return true;
        }

        private void MakeSureConnectionClosed()
        {
            if (connection != null)
            {
                //close the connection
                connection.CloseSocketConnection();
            }
                //Update status
                status = clientIs.notConnectedToServer;
        }


        /// <summary>
        /// Terminates the connection with the server.
        /// </summary>
        public void WeClosedConnection(disconnectReason reason)
        {
            whyWeDisconnected = reason;
            MakeSureConnectionClosed();
        }
    }
}
