// Author: Dharani Adhikari and taylor Morris
// CS 3500 fall 2014


using CustomNetworking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;



namespace BoggleClient
{
    /// <summary>
    /// Boggle Model primarily serves as a way to receive and properly route messages between
    /// the GUI and the server.  Initially the client, via the GUI will provide information needed
    /// to connect to the server, and then all the messages passed between them are received here, 
    /// their proper destination is determined based on the content of the message according to protocol, 
    /// and then proper events are triggered to handle the messages.
    /// </summary>
    public class BoggleModel
    {
        // The socket used to communicate with the server
        private StringSocket socket;
      
        // This is when we receive a start string and need to start all the stuff
        public event Action<String[]> IncomingStartEvent;

        // This is when we receive a stop string and need to stop all the stuff
        public event Action<String[]> IncomingStopEvent;

        // This is when we receive a time and need to update it
        public event Action<String> IncomingTimeEvent;

        // This is when we receive the score and need to update it
        public event Action<String[]> IncomingScoreEvent;

        // This is when we receive the game has been terminated
        public event Action<String> IncomingTerminatedEvent;

        // This is when we receive the game has been terminated
        public event Action<String> ServerEvent;

        // This is when we receive the game has been terminated
        //public event Action<String> AnythingEvent;

        public string tempString = "";

        public BoggleModel()
        {
            socket = null;
        }

        /// <summary>
        /// Connect to the server at the given hostname and port and with the given name.
        /// </summary>
        /// <param name="hostname">IP addresses for game server</param>
        /// <param name="name">Player's name</param>
        /// <param name="port">Connection port</param>
        public void Connect(string hostname, String name, int port)
        {
            TcpClient client = new TcpClient(hostname, port);
            socket = new StringSocket(client.Client, UTF8Encoding.Default);
            socket.BeginSend("PLAY " + name + "\n", (e, p) => { }, null);
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
            string[] words = s.Split(' ');


            // handles reception of start game message, the letters of the board, populates client timer, 
            // and the oponent name.  It also updates the MainButton State
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
