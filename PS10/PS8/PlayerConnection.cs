using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using CustomNetworking;

namespace BB
{

    /// <summary>
    /// A PlayerConnection is created for each client that attempts to connect to the server. It will handle communications between server and client
    /// </summary>
    public class PlayerConnection
    {
        /// <summary>
        /// Holds the name of the player on this connection.
        /// </summary>
        public string nameOfPlayer = "";

        /// <summary>
        /// Only to be used by playerIsConnected
        /// </summary>
        private bool _playerIsConnected;
        private Object _playerIsConnected_Lock = new Object();
        /// <summary>
        /// This is pretty much only used by the ConnectionManager when adding a second player to the queue
        /// to see if this player (who was the one on deck) disconnected while waiting.
        /// Thread safe. Set this to false if you've received word that the player has disconnected while in the queue.
        /// </summary>
        public bool playerLeftWhileInQueue
        {
            get
            {
                //ensures thread safety
                lock (_playerIsConnected_Lock)
                {
                    return _playerIsConnected;
                }
            }

            set
            {
                //Ensures thead safety
                lock (_playerIsConnected_Lock)
                {
                    _playerIsConnected = value;
                }
            }
        }

        //private waitingFor state;
        /// <summary>
        /// As long as this object is locked, PlayerConnection will not process any more received messages.
        /// </summary>
        public Object GagLock = new Object();
        /// <summary>
        /// The underlying socket, a connection specific to each client
        /// </summary>
        private Socket sock;
        /// <summary>
        /// Socket that sends and receives strings from the client
        /// </summary>
        private StringSocket ss;
        /// <summary>
        /// This delegate directs the incoming message to the appropriate place for processing
        /// The PlayerConnection cannot process any more incoming messages (but can received them) until this function has finished
        /// </summary>
        private Action<MessageReceivedFrom> directOutputTo;

        /// <summary>
        /// The function that is called when this player disconnects.
        /// </summary>
        private Action<PlayerConnection> WhenPlayerDisconnects;

        //These 3 objects are to be used by the BoggleGame that the player is in.
        //They are here for convenience.

            /// <summary>
            /// A list of the valid words that the player has played.
            /// </summary>
            public HashSet<String> LegalList;
            /// <summary>
            /// List of illegal words that the player has played.
            /// </summary>
            public HashSet<String> IllegalList;
            /// <summary>
            /// The score of this player in the current boggle game.
            /// </summary>
            public int score;
        


        /// <summary>
        /// Initializes and begins recieving messages on the underlying socket.
        /// </summary>
        /// <param name="sock">Socket that sends and receives strings from the client</param>
        /// <param name="callback">Function that is called when a message is received</param>
        /// <param name="whenPlayerDisconnects">Event called when a player disconnects</param>
        public PlayerConnection(Socket sock, Action<PlayerConnection> whenPlayerDisconnects, Action<MessageReceivedFrom> callback)
        {
            //Stores parameters for use within the object
            this.WhenPlayerDisconnects = whenPlayerDisconnects;
            this.playerLeftWhileInQueue = false;
            this.sock = sock; 
            this.ss = new StringSocket(sock, new UTF8Encoding());
            this.directOutputTo = callback;
            LegalList = new HashSet<string>();
            IllegalList = new HashSet<string>();
            score = 0;

            //Start receiving and processing commands
            ss.BeginReceive(receivedSomething, null);
        }

        /// <summary>
        /// Changes what the PlayerConnection does with the stuff it receives from the player/client.
        /// </summary>
        /// <param name="callback">Directs the incoming message to the appropriate place for processing</param>
        /// <param name="_whenThePlayerDisconnects">Event called when a player disconnects</param>
        public void setDirectOutputTo(Action<PlayerConnection> _whenThePlayerDisconnects, Action<MessageReceivedFrom> callback)
        {
            //May already have the GagLock in this thread if we received a message
            //  that resulted in this player connection getting it's output re-directed.
            lock (GagLock)
            {
                this.WhenPlayerDisconnects = _whenThePlayerDisconnects;
                //Remembering where to forward the incoming messages (the callback)
                this.directOutputTo = callback;
            }
        }

        /// <summary>
        /// Function called when we receive a message
        /// </summary>
        /// <param name="s">The message received.</param>
        /// <param name="e">Arbitrary exception object that may be sent by the string socket</param>
        /// <param name="payload">Identifying object sent by the string socket</param>
        private void receivedSomething(String s, Exception e, Object payload)
        {
            lock (GagLock)
            {
                //If the message is null, either the player has disconnected or an exception was thrown
                if (s == null)
                {
                    //Make sure we have somewhere to send this information and send it
                    if (WhenPlayerDisconnects != null)
                    {
                        WhenPlayerDisconnects(this);
                    }
                }
                else //if we received a message
                {
                    //start another receive
                    ss.BeginReceive(receivedSomething, null);

                    //remove whitespace from the front and back of message.
                    s = s.Trim();

                    //Tell the server what was received.
                    //Lock the console for thread safety.
                    lock (consoleLock)
                    {
                        Console.WriteLine("From " + nameOfPlayer + ":\"" + s + "\"");
                    }

                    //Detects whether we have been given somewhere to send the received messages
                    if (directOutputTo == null)
                    {
                        //sends message informing the client that its message has been ignored
                        SendIgnoreMessage(s);
                    }
                    else //Send the message where it need to go
                    {
                        this.directOutputTo(new MessageReceivedFrom(this, s));
                    }


                }
            }


        }

        /// <summary>
        /// Sends message informing the client that its message has been ignored
        /// </summary>
        /// <param name="s">The command being ignored</param>
        public void SendIgnoreMessage(string s)
        {
            SendMessage("IGNORING " + s);
        }

        /// <summary>
        /// Sends a message back to the client
        /// </summary>
        /// <param name="p">Sends whatever message back to the client</param>
        public bool SendMessage(string p)
        {
            //Make sure this connection is still live
            if (sock.Connected)
            {
                //Send the message with the terminating character attached
                ss.BeginSend(p + "\n", sent, p);
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Used to make any Console.Write* calls threadsafe
        /// </summary>
        private static Object consoleLock = new object();

        /// <summary>
        /// This function is called when a message has been sent.
        /// </summary>
        /// <param name="e">The exception if the message failed to send.</param>
        /// <param name="payload">The identifying object to the message sent.</param>
        private void sent(Exception e, object payload)
        {
            //Ensure thread safety
            lock (consoleLock)
            {
                //If there wasn't an error sending, then show the server what was sent
                if (e == null)
                    Console.WriteLine("To " + nameOfPlayer + ":\"" + payload.ToString() + "\"");
            }
        }

        /// <summary>
        /// Makes sure the connection to the player is closed.
        /// </summary>
        public void CloseSocketConnection()
        {
                //First check if it's connected
                if (sock.Connected)
                {
                    sock.Shutdown(SocketShutdown.Both);
                    sock.Close();
                }
        }


    }

    /// <summary>
    /// Only used when an actual message has been received. Contains the PlayerConnection and Message
    /// </summary>
    public class MessageReceivedFrom
    {
        /// <summary>
        /// The PlayerConnection that received the message.
        /// </summary>
        public PlayerConnection playerconnection;
        /// <summary>
        /// The message received.
        /// </summary>
        public String message;
        /// <summary>
        /// Stores the following parameters
        /// </summary>
        /// <param name="playerConnection">The PlayerConnection that received the message.</param>
        /// <param name="s">The message received.</param>
        public MessageReceivedFrom(PlayerConnection playerConnection, String s)
        {
            this.playerconnection = playerConnection;
            this.message = s;
        }

    }

}
