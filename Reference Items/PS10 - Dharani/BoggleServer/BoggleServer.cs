// Author: Dharani Adhikari and Taylor Morris
// CS 3500, Fall 2014


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net.Sockets;
using CustomNetworking;
using System.Net;
using MySql.Data.MySqlClient;


namespace Boggle
{
    /// <summary>
    /// This is our Boggle Server, it will implement all the requirements of PS8.
    /// 
    /// The Server
    /// Your server should be a console project whose main method takes two required and one optional command line parameter:

    /// Running from the Command Line
    /// Remember, a command line parameter is one that is passed to the main function by the Operating System when the program is run on the command line, such as:

    /// C: boggle_server 200 dictionary
    /// C: boggle_server 200 dictionary jimiergsatnesaps

    /// The Command Line Arguments
    /// When running the program the following values should be used:

    /// The number of seconds that each Boggle game should last. This should be a positive integer.
    /// The pathname of a file that contains all the legal words. The file should contain one word per line.
    /// An optional string consisting of exactly 16 letters. If provided, this will be used to initialize each Boggle board.
    /// </summary>
    public class BoggleServer
    {
        // Member Variables

        // TCPListerner to listen to all incoming connections
        private TcpListener bServer;   
        private Queue<Player> waitingPlayers = new Queue<Player>();
        private Dictionary<Object, Player> pendingPlayers = new Dictionary<Object, Player>();
        int port = 2000;
        int timeInSeconds;
        string gameSeed;
        Object SuperLock = new Object();
        HashSet<string> dictionary = new HashSet<string>();

        private TcpListener webListener;

        private string WebStyling = "<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Strict//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd\"><html xmlns=\"http://www.w3.org/1999/xhtml\"><head><title>Boggle Database</title><meta name=\"AUTHOR\" content=\"Dharani Adhikari and Taylor Morris\"/><style type=\"text/css\">body {width: 800px;background-color: #5f5f5f;margin-left: auto;margin-right: auto;font-family: Arial, Helvetica, sans-serif;position:relative;top:20px;color:#eeeeee;}table {border-collapse: collapse;box-shadow: 10px 10px 15px #424242; }table, th, td{border-style:solid;border-width:3px;border-color:white;}td{padding:15px;}</style></head>";

        /// <summary>
        /// The connection string.
        /// Your CADE login name serves as both your database name and your uid
        /// Your uNID serves as your password
        /// </summary>
        private const string connectionString = "server=atr.eng.utah.edu;database=cs3500_tmorris;uid=cs3500_tmorris;password=183014191";
        

        static void Main(string[] args)
        {
            int timeInSeconds;
            bool threeArgs = false;
           
            string filePath = args[1];
            string gameSeed = "";
            HashSet<string> dictionary = new HashSet<string>();

                        
            // check if the first argument is valid
            if (args.Length < 2 || args.Length > 3)
                Console.WriteLine("(Invalid number of arguments!! Press enter to quit." + "\n");
            if(!int.TryParse(args[0], out timeInSeconds))
            {
                Console.WriteLine("Invalid argument! Please enter a valid integer as the first argument" + "\n");
                Console.ReadLine();
                return;              
            }
            else
            {
                if(timeInSeconds < 0)
                {
                    Console.WriteLine("Invalid argument! Time cannot be negative!" + "\n");
                    Console.ReadLine();
                    return; 
                }
            }

           // check if the file name provided is a valid file
            if(args[1] == "" || !File.Exists(args[1]) )
            {
                Console.WriteLine("Invalid filename!! Press enter to quit" + "\n");
                Console.ReadLine();
                return;               
            }

            if(args.Length == 3)
            {
                if (args[2].ToString().Length != 16)
                {
                    Console.WriteLine("The third parapater is invalid! Press enter to quit" + "\n");
                    Console.ReadLine();
                    return;
                }
                foreach (char c in args[2])
                {
                    if (!Char.IsLetter(c))
                    {
                        Console.WriteLine("Invalid character in the third argument!" + "\n");
                        Console.ReadLine();
                        return;
                    }
                }
                gameSeed = args[2];
                threeArgs = true;
            }

            
            // read the file into a list of words
            try
            {
                using (StreamReader reader = new StreamReader(filePath))
                {
                    string line = reader.ReadLine();
                    while (!ReferenceEquals(line, null))
                    {
                        if (line.Contains(" ") || line.Equals("\n"))
                        {
                            Console.WriteLine("The file is not in a valid format. Press enter to quit." + "\n");
                            Console.ReadLine();
                            return;
                        }
                        dictionary.Add(line);
                        line = reader.ReadLine();
                    }
                }
            }
            catch (Exception)
            {
               throw new FileNotFoundException();
            }     

            // Create the boggle server by calling the constructor with all the input from command line above

            if (threeArgs)
            {
                BoggleServer bs = new BoggleServer(timeInSeconds, dictionary, gameSeed);
            }
            else
            {
                BoggleServer bs = new BoggleServer(timeInSeconds, dictionary);
            }
            
            Console.WriteLine("Server is running" + "\n");

            // wait
            Console.ReadLine();
        }

        /// <summary>
        /// this is the boggleserver constructor
        /// 
        /// it will read in the dictionary and setup the tcplistener,
        /// it will also set member vars for game length and board layout
        /// it will finally begin accepting sockets
        /// </summary>
        /// <param name="p">the port for the server</param>
        /// <param name="s">the ammount of time a game will last</param>
        /// <param name="path">the path of the dictionary</param>
        /// <param name="seed">the optional board set</param>
        public BoggleServer(int s, HashSet<string>listOfwords, String seed = null)
        {
            timeInSeconds = s;
            gameSeed = seed;
            dictionary = listOfwords;
            bServer = new TcpListener(IPAddress.Any, port);
            bServer.Start();
            bServer.BeginAcceptSocket(ConnectionReceived, null);

            webListener = new TcpListener(IPAddress.Any, 2500);
            webListener.Start();
            webListener.BeginAcceptSocket(WebConnectionReceived, null); 
        }

        /// <summary>
        /// once a connection has been received, we create a player and add
        /// them to pending, we then wait for their name and continue accepting
        /// sockets
        /// </summary>
        /// <param name="ar">the result of the asynchronous method</param>
        private void ConnectionReceived(IAsyncResult ar)
        {
            Socket playerSocket = bServer.EndAcceptSocket(ar);
            Player newPlayer = new Player(playerSocket);
            // start receiving messages from player socket
            newPlayer.stringSocket.BeginReceive(NameReceived, newPlayer.stringSocket);
            pendingPlayers.Add(newPlayer.stringSocket, newPlayer);
            Console.WriteLine("Connection Received." + "\n");

            bServer.BeginAcceptSocket(ConnectionReceived, null);
        }

        /// <summary>
        /// this handles when a web browser attempts to connect to the server
        /// it will start listening for a message on a stringsocket
        /// </summary>
        /// <param name="ar">the result for the request</param>
        private void WebConnectionReceived(IAsyncResult ar)
        {
            Socket socket = webListener.EndAcceptSocket(ar);

            StringSocket webSocket = new StringSocket(socket, UTF8Encoding.Default);

            webSocket.BeginReceive(WebRequestReceived, webSocket);

            webListener.BeginAcceptSocket(WebConnectionReceived, null);
        }


        /// <summary>
        /// we received a message from the server, and are now going to send them a webpage
        /// based on what they wanted
        /// </summary>
        /// <param name="s">the message we received</param>
        /// <param name="e">an exception that should be null</param>
        /// <param name="payload">payload for the connection</param>
        private void WebRequestReceived(string s, Exception e, object payload)
        {
            StringSocket webSocket = (StringSocket)payload;

            //tell the browser we will give it a webpage
            webSocket.BeginSend("HTTP/1.1 200 OK\r\nConnection: close\r\nContent-Type: text/html; charset=UTF-8\r\n\r\n", (ee, pp) => { }, null);
            if (s == null)
            {
                return;
            }

            //a list of all players was requested
            if (s.StartsWith("GET /players"))
            {

                List<string> ServerStat = new List<string>();

                // request the players list
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    
                    try
                    {
                        // Open a connection
                        conn.Open();

                        // Create a command
                        MySqlCommand command = conn.CreateCommand();
                        command.CommandText = "SELECT * from Players";  // SQL command goes here

                        // Execute the command and cycle through the DataReader object
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ServerStat.Add("href=\"/games?player=" + reader["Name"] + "\">" + reader["Name"] + "</a>" + " " + reader["Win"] + " " + reader["Loss"] + " " + reader["Tie"]);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    finally
                    {
                        //close the connection
                        conn.Close();
                    }
                }

                //create the html for the table of players
                string table = @"<table>";
                table += "<tr><th>Name</th><th>Times Won</th><th>Times Lost</th><th>Times Tied</th></tr>";

                foreach (string str in ServerStat)
                {
                    table += @"<tr>";
                    String[] tableRow = str.Split(null);
                    foreach (string cell in tableRow)
                    {
                        if (cell.StartsWith("href"))
                        {
                            //string temp = "<a style=\"color: #CCFFFF\" " + cell;
                            table += @"<td>" + "<a style=\"color: #CCFFFF\" " + cell + @"</td>";
                        }
                        else
                        {
                            table += @"<td>" + cell + @"</td>";
                        }
                    }
                    table += @"</tr>";

                }
                table += @"</table>";

                //send the webpage to the browser
                webSocket.BeginSend(WebStyling + @"<body><h1>All boggle players</h1>" + table + @"</body></html>", (ee, pp) => { }, null);
            }
            //a list of a single players stats was requested
            else if (s.StartsWith("GET /games?player="))
            {
                List<string> PlayerStat = new List<string>();

                String[] subString = s.Substring(18).Split(null);
                //get the player name
                string pName = subString[0];
                int playerID = 0;

                // first get the player's ID
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    try
                    {
                        // Open a connection
                        conn.Open();

                        // Create a command
                        MySqlCommand command = conn.CreateCommand();
                        command.CommandText = "SELECT PlayerID from Players where Players.Name = '" + pName + "'";

                        // Execute the command and cycle through the DataReader object
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                playerID = (int)reader["PlayerID"];
                            }
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

                // Fetch all games by the player
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {

                    try
                    {
                        // Open a connection
                        conn.Open();

                        // Create a command
                        MySqlCommand command = conn.CreateCommand();
                        command.CommandText = "select * FROM Games JOIN Players ON (Games.P2ID = Players.PlayerID AND Games.P1ID = " + playerID + ") OR (Games.P1ID = Players.PlayerID AND Games.P2ID = " + playerID + ")";

                        // Execute the command and cycle through the DataReader object
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                //add to our table based on what player they were in this particular game
                                if ((int)reader["P1ID"] == playerID)
                                {
                                    PlayerStat.Add("href=\"/game?id=" + reader["GameID"] + "\">" + reader["GameID"] + " " + reader["Date"] + " " + "href=\"/games?player=" + reader["Name"] + "\">" + reader["Name"] + " " + reader["P1Score"] + " " + reader["P2Score"]);
                                }
                                else
                                {
                                    PlayerStat.Add("href=\"/game?id=" + reader["GameID"] + "\">" + reader["GameID"] + " " + reader["Date"] + " " + "href=\"/games?player=" + reader["Name"] + "\">" + reader["Name"] + " " + reader["P2Score"] + " " + reader["P1Score"]);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    finally
                    {
                        //close the connection
                        conn.Close();
                    }
                }

                //create the html for the table of players
                string table = @"<table>";
                table += "<tr><th>Game ID</th><th>Date</th><th>Time</th><th>AM/PM</th><th>Opponent Name</th><th>Own Score</th><th>Opponent Score</th></tr>";

                foreach (string str in PlayerStat)
                {
                    table += @"<tr>";
                    String[] tableRow = str.Split(null);
                    foreach (string cell in tableRow)
                    {
                        if (cell.StartsWith("href"))
                        {
                            table += @"<td>" + "<a style=\"color: #CCFFFF\" " + cell + @"</td>";
                        }
                        else
                        {
                            table += @"<td>" + cell + @"</td>";
                        }
                    }
                    table += @"</tr>";

                }
                table += @"</table>";

                //send the webpage to the browser
                webSocket.BeginSend(WebStyling + @"<body><h1>Games by " + pName + "</h1>" + table + @"</body></html>", (ee, pp) => { }, null);
            }

            //info for a specific game was requested
            else if (s.StartsWith("GET /game?id="))
            {
                //get the game id
                String[] subString = s.Substring(13).Split(null);
                string gameID = subString[0];

                string gameInfo = "";
                Boolean gameinfoSet = false;

                int p1ID = 0;
                int p2ID = 0;

                string p1Name = "";
                string p2Name = "";

                string p1Valid = "";
                string p1Invalid = "";
                string common = "";
                string p2Valid = "";
                string p2Invalid = "";

                int maxGameID;
                int gameIDAsInt;

                // get the needed info from the game
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    try
                    {
                        // Open a connection
                        conn.Open();

                        // Create a command
                        MySqlCommand command = conn.CreateCommand();


                        // make sure the game they're asking for exists

                        command.CommandText = "SELECT GameID FROM Games ORDER BY GameID DESC LIMIT 1";
                        command.ExecuteNonQuery();

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            reader.Read();
                            maxGameID = (int)reader["GameID"];

                            int.TryParse(gameID, out gameIDAsInt);

                            if (gameIDAsInt > maxGameID)
                            {
                                // Send them to the invalid request page
                                webSocket.BeginSend(WebStyling + @"<body><h1>Boggle Database</h1><p>Game ID Number invalid. Please try using a valid request such as:</p>
                                <ul><li>/players</li><li>/games?player=*nameofplayer*</li><li>/game?id=*gameID*</li></body></html>", (ee, pp) => { }, null);

                                //sleep to allow all the messages to get through
                                System.Threading.Thread.Sleep(300);

                                webSocket.Close();
                                return;
                            }
                        }

                        
                        command.CommandText = "select * FROM Games,Words where Words.GameID = Games.GameID AND Games.GameID = " + gameID;

                        // Execute the command and cycle through the DataReader object
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {

                            while (reader.Read())
                            {
                                //first time running through the reader get all the game stats
                                if (!gameinfoSet)
                                {
                                    p1ID = (int)reader["P1ID"];
                                    p2ID = (int)reader["P2ID"];
                                    gameInfo += reader["P1Score"] + " " + reader["P2Score"] + " " + reader["Date"] + " " + reader["Board"] + " " + reader["GameTime"];
                                    gameinfoSet = true;
                                }
                                //put the word in the corresponding string
                                if ((int)reader["P1ID"] == (int)reader["PlayerID"])
                                {
                                    if ((int)reader["Valid"] == 1)
                                    {
                                        p1Valid += reader["Word"] + "<br/>";
                                    }
                                    else
                                    {
                                        p1Invalid += reader["Word"] + "<br/>";
                                    }
                                }
                                else if ((int)reader["P2ID"] == (int)reader["PlayerID"])
                                {
                                    if ((int)reader["Valid"] == 1)
                                    {
                                        p2Valid += reader["Word"] + "<br/>";
                                    }
                                    else
                                    {
                                        p2Invalid += reader["Word"] + "<br/>";
                                    }
                                }
                                else
                                {
                                    common += reader["Word"] + "<br/>";
                                }
                            }
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

                //get player 1 name
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    try
                    {
                        // Open a connection
                        conn.Open();

                        // Create a command
                        MySqlCommand command = conn.CreateCommand();
                        command.CommandText = "SELECT Name from Players where Players.PlayerID = " + p1ID;

                        // Execute the command and cycle through the DataReader object
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                p1Name += "<a style=\"color: #CCFFFF\" href=\"/games?player=" + reader["Name"] + "\">" + reader["Name"] + "</a>";
                            }
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

                //get player 2 name
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    try
                    {
                        // Open a connection
                        conn.Open();

                        // Create a command
                        MySqlCommand command = conn.CreateCommand();
                        command.CommandText = "SELECT Name from Players where Players.PlayerID = " + p2ID;

                        // Execute the command and cycle through the DataReader object
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                p2Name += "<a style=\"color: #CCFFFF\" href=\"/games?player=" + reader["Name"] + "\">" + reader["Name"] + "</a>";
                            }
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

                String[] gameInfoSub = gameInfo.Split(null);
                //setup table for game info
                string table = "<table align=\"center\">";
                table += "<tr><td>Player 1 Name</td><td>Player 2 Name</td><td>Player 1 Score</td><td>Player 2 Score</td><td>Game End Date</td><td>Duration</td></tr>";
                try
                {
                    table += "<tr><td>" + p1Name + "</td><td>" + p2Name + "</td><td>" + gameInfoSub[0] + "</td><td>" + gameInfoSub[1] + "</td><td>" + gameInfoSub[2] + " " + gameInfoSub[3] + " " + gameInfoSub[4] + "</td><td>" + gameInfoSub[6] + "</td></tr>";
                }
                catch (Exception exe)
                {
                }
                table += "</table><br/><br/>";

                //set up another table for the board

                table += "<table align=\"center\">";
                table += "<tr><td>" + gameInfoSub[5].Substring(0, 1) + "</td><td>" + gameInfoSub[5].Substring(1, 1) + "</td><td>" + gameInfoSub[5].Substring(2, 1) + "</td><td>" + gameInfoSub[5].Substring(3, 1) + "</td></tr>";
                table += "<tr><td>" + gameInfoSub[5].Substring(4, 1) + "</td><td>" + gameInfoSub[5].Substring(5, 1) + "</td><td>" + gameInfoSub[5].Substring(6, 1) + "</td><td>" + gameInfoSub[5].Substring(7, 1) + "</td></tr>";
                table += "<tr><td>" + gameInfoSub[5].Substring(8, 1) + "</td><td>" + gameInfoSub[5].Substring(9, 1) + "</td><td>" + gameInfoSub[5].Substring(10, 1) + "</td><td>" + gameInfoSub[5].Substring(11, 1) + "</td></tr>";
                table += "<tr><td>" + gameInfoSub[5].Substring(12, 1) + "</td><td>" + gameInfoSub[5].Substring(13, 1) + "</td><td>" + gameInfoSub[5].Substring(14, 1) + "</td><td>" + gameInfoSub[5].Substring(15, 1) + "</td></tr>";
                table += "</table><br/><br/>";

                //sets up another table for words
                table += "<table align=\"center\">";
                table += "<tr><td>Player 1 Valid</td><td>Player 1 Invalid</td><td>Common Words</td><td>Player 2 Valid</td><td>Player 2 Invalid</td></tr>";
                table += "<tr><td>" + p1Valid + "</td><td>" + p1Invalid + "</td><td>" + common + "</td><td>" + p2Valid + "</td><td>" + p2Invalid + "</td></tr>";
                table += "</table>";

                //send the tables to the browser
                webSocket.BeginSend(WebStyling + @"<body><h1>Info for game " + gameID + "</h1>" + table + @"</body></html>", (ee, pp) => { }, null);
            }
            //they asked for something we didn't understand
            else
            {
                webSocket.BeginSend(WebStyling + @"<body><h1>Boggle Database</h1><p>The web request you have given us is not recognized. Please try using a valid request such as:</p>
                        <ul><li>/players</li><li>/games?player=*nameofplayer*</li><li>/game?id=*gameID*</li></body></html>", (ee, pp) => { }, null);
            }

            //sleep to allow all the messages to get through
            System.Threading.Thread.Sleep(300);

            webSocket.Close();
        }

        /// <summary>
        /// Method to validate a new player and assign them a game.  a player must type "play" followed by their
        /// name to be accepted as an actual player, else we ignore their command and wait for a proper one.  If a 
        /// null message is received the client has disconnected and we no longer listen to them.
        /// 
        /// once two valid players are available, they will be paired and sent to a new game.
        /// </summary>
        /// <param name="msg">message from a player = "play (name)" expected</param>
        /// <param name="e">exception</param>
        /// <param name="p">payload</param>
        private void NameReceived(String msg, Exception e, Object p)
        {
            // Apply lock to avoid race condition
            lock (SuperLock)
            {
                Game newGame;

                // When no message is received, return 
                if (msg == null)
                {
                    if(waitingPlayers.Count>0)
                        waitingPlayers.Dequeue();
                    return;
                }
                if (msg == "")
                {
                    if (waitingPlayers.Count > 0)
                        waitingPlayers.Dequeue();
                    return;
                }

                // save the first four characters in a temporary variable to check the protocol
                string protocol = msg.Substring(0, 4);
                protocol = protocol.ToUpper();
                
                if (protocol.StartsWith("PLAY"))
                {
                    // Get the name of the player and trim leading and trailling spaces
                    pendingPlayers[p].name = msg.Substring(4).Trim();

                    waitingPlayers.Enqueue(pendingPlayers[p]);
                    pendingPlayers.Remove(pendingPlayers[p]);
                    Console.WriteLine("New Player Waiting." + "\n");
                    
                    //if(waitingPlayers.Count == 1)
                        //pendingPlayers[p].stringSocket.BeginReceive(NameReceived, pendingPlayers[p].stringSocket);

                    // start the game when there are two  players in the queue
                    if (waitingPlayers.Count > 1)
                    {
                        Console.WriteLine("New Game Started." + "\n");
                        newGame = new Game(waitingPlayers.Dequeue(), waitingPlayers.Dequeue(), timeInSeconds, dictionary, gameSeed);
                    }
                }                
               
                else
                {
                    // for invalid input, send "Ignoring" message
                    pendingPlayers[p].stringSocket.BeginSend("IGNORING " + msg + "\n", (ee, pp) => { }, null);
                    //if(waitingPlayers.Count == 1)
                        //pendingPlayers[p].stringSocket.BeginReceive(NameReceived, pendingPlayers[p].stringSocket);
                }
            }
        }


        /// <summary>
        /// this simply closes the servers tcplistener when we are done
        /// </summary>
        public void Close()
        {
            bServer.Stop();
        }
    }
}
