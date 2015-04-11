using CustomNetworking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SS
{
    /// <summary>
    /// 
    /// </summary>
    public class Communicator
    {
        // The socket used to communicate with the server
        private StringSocket socket;

        // This is when we receive a start string and need to start all the stuff
        /// <summary>
        /// 
        /// </summary>
        public event Action<String[]> IncomingStartEvent;

        /// <summary>
        /// Action when error received
        /// </summary>
        public event Action<String[]> IncomingErrorEvent;

        /// <summary>
        /// 
        /// </summary>
        public event Action<String[]> IncomingConnectedEvent;


        public event Action<String[]> IncomingCellEvent;
            
            
            
        // This is when we receive a stop string and need to stop all the stuff
        /// <summary>
        /// 
        /// </summary>
        public event Action<String[]> IncomingStopEvent;

        // This is when we receive a time and need to update it
        /// <summary>
        /// 
        /// </summary>
        public event Action<String> IncomingTimeEvent;

        // This is when we receive the score and need to update it
        /// <summary>
        /// 
        /// </summary>
        public event Action<String[]> IncomingScoreEvent;

        // This is when we receive the game has been terminated
        /// <summary>
        /// 
        /// </summary>
        public event Action<String> IncomingTerminatedEvent;

        // This is when we receive the game has been terminated
        /// <summary>
        /// 
        /// </summary>
        public event Action<String> ServerEvent;

        // This is when we receive the game has been terminated
        //public event Action<String> AnythingEvent;
        /// <summary>
        /// 
        /// </summary>
        public string tempString = "";

        /// <summary>
        /// 
        /// </summary>
        public Communicator()
        {
            socket = null;
        }

        /// <summary>
        /// Connect to the server at the given hostname and port and with the given name.
        /// </summary>
        /// <param name="hostname">IP addresses for game server</param>
        /// <param name="name">user's name</param>
        /// <param name="spreadsheetName">file name</param>
        /// <param name="portS">Connection port</param>
        public void Connect(string hostname, String name, String spreadsheetName, String portS)
        {
            int port = Convert.ToInt32(portS, 10);
            TcpClient client = new TcpClient(hostname, port);
            socket = new StringSocket(client.Client, ASCIIEncoding.Default);
            socket.BeginSend("connect|" + name + "|" + spreadsheetName + "\n", (e, p) => { }, null);
            socket.BeginReceive(LineReceived, null);
        }

        /// <summary>
        /// Sends a message from the client to the server.  The message is passed in from the GUI
        /// in the line parameter, gets a newline character appended to it, and is sent off.
        /// </summary>
        /// <param name="line">Message from GUI</param>
        public void SendMessage(String line)
        {
            if (socket != null)
            {
                try
                {
                    socket.BeginSend(line + "\n", (e, p) => { }, null);
                }
                catch (Exception e)
                {
                    //server died
                    Disconnect();
                    ServerEvent("The server has crashed");
                }
            }
        }

        /// <summary>
        /// Helper method used to disconnect clients under any condition that ends the game
        /// </summary>
        public void Disconnect()
        {
            socket.Close();
        }

        /// <summary>
        /// Deal with an arriving line of text. Determine what to do with each message by reading the first word, then 
        /// remove that word from the message, trim the message to remove unwanted blank space, and then pass the message
        /// to the GUI
        /// </summary>
        /// <param name="s">Message</param>
        /// <param name="e">Exception</param>
        /// <param name="p">Payload</param>
        private void LineReceived(String s, Exception e, object p)
        {
            // handles null message
            if (s == null)
            {
                ServerEvent("The server has crashed");
                Disconnect();
                return;
            }

            string temp = s.ToUpper();
            s = s.Trim();
            string[] words = s.Split('|');


            // handles reception of start game message, the letters of the board, populates client timer, 
            // and the oponent name.  It also updates the MainButton State

            if(temp.StartsWith("ERROR"))
            {
                if(words[1].CompareTo("0") == 0)
                {

                }
                else if(words[1].CompareTo("1") == 0)
                {

                }
                else if (words[1].CompareTo("2") == 0)
                {

                }
                else if (words[1].CompareTo("3") == 0)
                {

                }
                else //error 4 is an unregistered username. we disconnect the socket and send a message to user to use a registered name
                {
                    IncomingErrorEvent(words);
                    Disconnect(); //disconnect socket
                }

            }
            else if (temp.StartsWith("CONNECTED"))
            {
                IncomingConnectedEvent(words);
                socket.BeginReceive(LineReceived, null);
            }
            else if (temp.StartsWith("CELL"))
            {
                if(IncomingCellEvent != null)
                {
                    IncomingCellEvent(words);
                    socket.BeginReceive(LineReceived, null);
                }
            }

            if (temp.StartsWith("START"))
            {
                if (IncomingStartEvent != null)
                {
                    IncomingStartEvent(words);
                    socket.BeginReceive(LineReceived, null);
                }
            }

            // handles reception of Score update message, updates both scores, the format is own score first.
            else if (temp.StartsWith("SCORE"))
            {
                if (IncomingScoreEvent != null)
                {
                    IncomingScoreEvent(words);
                    socket.BeginReceive(LineReceived, null);
                }
            }

            // handles reception of game timer update message, updates the game timer every second.
            else if (temp.StartsWith("TIME"))
            {
                if (IncomingTimeEvent != null)
                {
                    IncomingTimeEvent(words[1]);
                    socket.BeginReceive(LineReceived, null);
                }
            }

            // handles reception of game over message, this string comes as a long list of 5 types of words:
            // own valid words, opponent valid words, common valid words, own invalid words, and opponent invalid words, 
            // and updates the MainButton State

            else if (temp.StartsWith("STOP"))
            {
                if (IncomingStopEvent != null)
                {
                    IncomingStopEvent(words);
                    socket.BeginReceive(LineReceived, null);
                }
            }

            // handles reception of game disconnect message, Displays a terminated message to the player, and updates
            // The MainButton state.

            else if (temp.StartsWith("TERMINATED"))
            {
                if (IncomingTerminatedEvent != null)
                {
                    IncomingTerminatedEvent(words[0]);
                }
            }

            else
            {
                // should never happen, here for debugging
                socket.BeginReceive(LineReceived, null);
            }
        }
    }
}
