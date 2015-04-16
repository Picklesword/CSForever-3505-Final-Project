using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CustomNetworking;
using System.IO;

namespace BB
{
    /// <summary>
    /// Manages a Boggle server where players can connect and play against each other.
    /// </summary>
    public class BoggleServer
    {
        /// <summary>
        /// The amount of time each game runs for.
        /// </summary>
        private int gameTime;
        /// <summary>
        /// The (optional) board configuration used in all games.
        /// </summary>
        private string customConfig;
        /// <summary>
        /// Manages all the clients that connect to the server to play boggle.
        /// </summary>
        private PlayConnectionManager cManager;

        /// <summary>
        /// Manages the Web-Page side of the server.
        /// </summary>
        private BoggleWebPageManager wpManager;

        /// <summary>
        /// Manages all the database functionality.
        /// </summary>
        private BoggleDatabaseManager dbManager;

        /// <summary>
        /// Set that contains the loaded words in the dictionary file, cAse sensitIve
        /// </summary>
        private HashSet<String> dictionary;
        /// <summary>
        /// The port that boggle games run on.
        /// </summary>
        public int playPort;


        /// <summary>
        /// Starts a Boggle server with the given options.
        /// </summary>
        /// <param name="gameTime">The amount of time each game runs for.</param>
        /// <param name="dictPath">The path to the dictionary used in all boggle games</param>
        /// <param name="customConfig">The (optional) board configuration used in all games.</param>
        /// <param name="port">The port on which this server accepts connections</param>
        public BoggleServer(int gameTime, string dictPath, string customConfig, int port)
        {
            this.gameTime = gameTime;
            //Check if the dictionary exists
            LoadDictionary(dictPath);
            this.customConfig = customConfig;
            //Create the manager that handles the connections
            dbManager = new BoggleDatabaseManager("server=atr.eng.utah.edu;database=cs3500_jasteck;uid=cs3500_jasteck;password=450300138");
            cManager = new PlayConnectionManager(gameTime, dictionary,customConfig,dbManager);
            wpManager = new BoggleWebPageManager(dbManager);
            this.playPort = port;
        }

        /// <summary>
        /// Loads the dictionary into memory for use in the games.
        /// </summary>
        /// <param name="dictPath">File path to the dictionary</param>
        private void LoadDictionary(string dictPath)
        {
            //Start with an empty dictionary
            dictionary = new HashSet<string>();
            //If the file exists, load it. Else, display error and continue with an empty dictionary
            if (File.Exists(dictPath))
            {
                try
                {
                    string line;
                    StreamReader reader = new StreamReader(dictPath);
                    //Read from the file until there are no more lines
                    while ((line = reader.ReadLine()) != null)
                    {
                        //Add each line to the dictionary
                        dictionary.Add(line.ToUpper());
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error: " + e.Message);
                    //Clear out the dictionary in case something goes wrong while loading the file
                    dictionary = new HashSet<string>();
                }
            }
            else
                Console.WriteLine("ERROR: Could not find dictionary file.");
        }

        /// <summary>
        /// Starts the server listening for game and webpage requests.
        /// This method is Non-blocking.
        /// </summary>
        public void RunServer()
        {
            //Listen for play requests
            ThreadPool.QueueUserWorkItem(o=> listenForPlayRequests());

            //listen for webpage requests
            ThreadPool.QueueUserWorkItem(o => wpManager.startListener(2500));
        }

        /// <summary>
        /// Listens for requests for playing boggle.
        /// </summary>
        private void listenForPlayRequests()
        {
            TcpListener server = null;
            try
            {
                //Accept connections on the specified port from any IP address
                server = new TcpListener(IPAddress.Any, playPort);
                //Start the server
                server.Start();
                Socket theSocket = null;

                //Continuously accepts connections and passes them on to the connection manager
                while (true)
                {
                    theSocket = server.AcceptSocket(); //Halts the thread until a connection is made
                    ThreadPool.QueueUserWorkItem(CallConnectionManager, theSocket); //Start a new thread to handle the connection
                }
            }
            catch (Exception e) //If something goes wrong, show the error and close the server
            {
                Console.WriteLine("ERROR:"+e.Message);
            }
            finally
            {
                //Close the server
                server.Stop();
            }
        }

        /// <summary>
        /// Adds the given socket to the connection manager.
        /// </summary>
        /// <param name="socket">The socket connected to a client that wants to play.</param>
        private void CallConnectionManager(object socket)
        {
            cManager.AddConnection((Socket)socket);
            Console.WriteLine("New connection made.");
        }
        
    }
}
