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
    /// This class handles the communication between the server and the spreadsheet client and provides
    /// appropriate information to the view (spreadsheet GUI).
    /// </summary>
    public class Communicator
    {
        // The socket used to communicate with the server
        private StringSocket socket;
       
        /// <summary>
        /// Action when error received
        /// </summary>
        public event Action<String[]> IncomingErrorEvent;

        /// <summary>
        /// This event triggeres the view to display the status that the client is connected to the server
        /// </summary>
        public event Action<String[]> IncomingConnectedEvent;

        /// <summary>
        /// This event triggers the cells to he updated after receiving update signal from the server
        /// </summary>
        public event Action<String[]> IncomingCellEvent;

        /// <summary>
        /// This event provides the status of server to the view
        /// </summary>
        public event Action<String> ServerEvent;

        /// <summary>
        /// This event is used to provide message to the client when the server is not runnig or if 
        /// the server info is incorrect
        /// </summary>
        public event Action<String> IncomingConnectEvent; 
      
        /// <summary>
        /// This is the default constructor. It just initializes the socket to null.
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
            try
            {
                int port = Convert.ToInt32(portS, 10);
                TcpClient client = new TcpClient(hostname, port);
                socket = new StringSocket(client.Client, ASCIIEncoding.Default);
                socket.BeginSend("connect " + name + " " + spreadsheetName + "\n", (e, p) => { }, null);
                socket.BeginReceive(LineReceived, null);
            }
            catch(SocketException)
            {
                Disconnect();
                IncomingConnectEvent("Could not connect to the server! \nEither server is not running or the server IP address and port number are not correct. \n\nPlease try again.");
            }
        }

        /// <summary>
        /// //method to send to server to register a new user
        /// </summary>
        /// <param name="name">String</param>
        public void RegisterUser(String name)
        {
            socket.BeginSend("register " + name + "\n", (e, p) => { }, null);
            socket.BeginReceive(LineReceived, null);
        }

        
        

        /// <summary>
        /// Sends a message from the client to the server.  The message is passed in from the GUI
        /// in the line parameter, gets a newline character appended to it, and is sent off.
        /// </summary>
        /// <param name="line">Message from GUI</param>
        public void SendMessage(String line)
        {
            //If the client is not connected to the server, The spreadsheet will still update the local copy of spreadsheet.
            // but it won't be saved to the server. -Dharani
            
            if (socket != null)
            {
                try
                {
                    socket.BeginSend(line + "\n", (e, p) => { }, null);
                    socket.BeginReceive(LineReceived, null); //Scott added this to make sure that this is listening after it sends its message
                }
                catch (Exception e)
                {
                    //server died
                    Disconnect();
                    ServerEvent("The server has crashed");
                }
            }
            else
            {
                throw new Exception("Could not update the server. You are not connected!");
            }
        }

        /// <summary>
        /// Helper method used to disconnect clients under any condition that ends the game
        /// </summary>
        public void Disconnect()
        {
            if(socket != null)
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
            string[] words = s.Split(' '); //delimit by space
            int wordsCount = words.Count();

            // remove any white spaces between words
            for( int i = 0; i < wordsCount; i ++)
            {
                words[i] = words[i].Trim();
            }


            // handles reception of start game message, the letters of the board, populates client timer, 
            // and the oponent name.  It also updates the MainButton State

            if(temp.StartsWith("ERROR"))
            {

                // Dharani: 
                // I  was not sure if we need to disconnect on every occurance of error. I think this should not be the case
                // but would like to change it after team discussion
                //Also, protocol says the error command and the ID are separated by an space only. We might
                // need to parse it differently just for handling error. It should be a very easy fix if we need to change.
                //Note added by Scott: Server determines the message being sent to the client.  Client does not determine the error msg.
                if(words[1].CompareTo("0") == 0)
                {
                    IncomingErrorEvent(words);
                    Disconnect(); //disconnect socket
                }
                else if(words[1].CompareTo("1") == 0)
                {
                    IncomingErrorEvent(words);
                    Disconnect(); //disconnect socket
                }
                else if (words[1].CompareTo("2") == 0)
                {
                    IncomingErrorEvent(words);
                    Disconnect(); //disconnect socket
                }
                else if (words[1].CompareTo("3") == 0)
                {
                    IncomingErrorEvent(words);
                    Disconnect(); //disconnect socket
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

            
            else
            {
                // should never happen, here for debugging
                socket.BeginReceive(LineReceived, null);
            }
        }
    }
}
