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
    /// Holds a socket connection and manages the sending and reciving of messages as well as disconnects.
    /// </summary>
    public class SocketConnection
    {
        /// <summary>
        /// The function that is called when the connection is lost.
        /// </summary>
        private Action<SocketConnection,Exception> CallWhenDisconnected;
        /// <summary>
        /// This delegate directs the incoming message to the appropriate place for processing.
        /// The SocketConnection cannot process any more incoming messages (but can received them) until this function has finished.
        /// </summary>
        private Action<MessageReceivedFrom> directOutputTo;
        /// <summary>
        /// The underlying socket.
        /// </summary>
        private Socket sock;
        /// <summary>
        /// Class used for low-level sending and recieving of messages.
        /// </summary>
        private StringSocket ss;

        //private waitingFor state;
        /// <summary>
        /// As long as this object is locked, this SocketConnection will not process any more received messages (but it can still recieve them).
        /// </summary>
        public Object GagLock = new Object();


        /// <summary>
        /// Constructor. Begins listening for messages.
        /// </summary>
        /// <param name="sock">The underlying connection used for communication.</param>
        /// <param name="whenMessageIsReceived">Function that is called when a message is received</param>
        /// <param name="whenDisconnected">Event called when the connection is lost.</param>
        public SocketConnection(Socket sock, Action<SocketConnection,Exception> whenDisconnected, Action<MessageReceivedFrom> whenMessageIsReceived)
        {
            //Stores parameters for use within the object
            this.CallWhenDisconnected = whenDisconnected;
            this.directOutputTo = whenMessageIsReceived;
            this.sock = sock;
            this.ss = new StringSocket(sock, new UTF8Encoding());

            //Start receiving and processing commands
            ss.BeginReceive(receivedSomething, null);
        }

        /// <summary>
        /// Changes where the SocketConnection sends further output to.
        /// </summary>
        /// <param name="whenMessageIsReceived">Directs the incoming message to the appropriate place for processing</param>
        /// <param name="whenDisconnected">Event called when the connection is lost.</param>
        public void setDirectOutputTo(Action<SocketConnection,Exception> whenDisconnected, Action<MessageReceivedFrom> whenMessageIsReceived)
        {
            //May already have the GagLock in this thread if we received a message
            // that resulted in this SocketConnection getting its output re-directed.
            lock (GagLock)
            {
                this.CallWhenDisconnected = whenDisconnected;
                //Remembering where to forward the incoming messages (the callback)
                this.directOutputTo = whenMessageIsReceived;
            }
        }

        /// <summary>
        /// Function called when we receive a message.
        /// </summary>
        /// <param name="s">The message received.</param>
        /// <param name="e">Arbitrary exception object that may be sent by the string socket</param>
        /// <param name="payload">Expected to always be null.</param>
        private void receivedSomething(String s, Exception e, Object payload)
        {

            lock (GagLock)
            {
                //If the message is null, either we have been disconnected or an exception was thrown
                if (s == null)
                {
                    //Make sure we have somewhere to send this information and send it
                    if (CallWhenDisconnected != null)
                    {
                        CallWhenDisconnected(this, e);
                    }
                }
                else //if we received a message
                {
                    //start another receive
                    ss.BeginReceive(receivedSomething, null);

                    //remove whitespace from the front and back of message.
                    s = s.Trim();

                    //Detects whether we have been given somewhere to send the received messages
                    if (directOutputTo != null)
                    {
                        this.directOutputTo(new MessageReceivedFrom(this, s));
                    }
                    //otherwise ignore what we received
                }
            }


        }

        /// <summary>
        /// Sends a message through the connection.
        /// </summary>
        /// <param name="p">Message to be sent. The '\n' character is automatically attached to the end of the message.</param>
        /// <param name="theSendCallback">The function to call when we have a result of sending the message.</param>
        public bool SendMessage(string p, Action<Exception,object> theSendCallback)
        {
            //Make sure this connection is still live
            if (sock.Connected)
            {
                //Send the message with the terminating character attached
                ss.BeginSend(p + "\n", (e, o) => { theSendCallback(e, o); }, p);
                
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Determines if the underlying socket is still connected.
        /// </summary>
        /// <returns>True if the socket is still connected.</returns>
        public bool isConnected()
        {
            return sock.Connected;
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
    /// Only used when an actual message has been received. Contains a SocketConnection that recieved the message along with the message recieved.
    /// </summary>
    public class MessageReceivedFrom
    {
        /// <summary>
        /// The SocketConnection that received the message.
        /// </summary>
        public SocketConnection connection;
        /// <summary>
        /// The message received.
        /// </summary>
        public String message;

        /// <summary>
        /// Stores the following parameters
        /// </summary>
        /// <param name="connection">The SocketConnection that received the message.</param>
        /// <param name="s">The message received.</param>
        public MessageReceivedFrom(SocketConnection connection, String s)
        {
            this.connection = connection;
            this.message = s;
        }
    }
}
