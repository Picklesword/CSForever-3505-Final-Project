using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BB
{
    class BoggleDatabaseManager
    {
        /// <summary>
        /// The string that contains the server, database name, uid, and 
        /// password necessary to connect to the database we are to use.
        /// </summary>
        //private string connectionString;

        //need to change to jasteck instead of syoung for final revision
        private MySqlConnection connection;
        //private const string connectionString = "SERVER=atr.eng.utah.edu;DATABASE=cs3500_syoung;UID=library;PASSWORD=071985431";
        private string connectionString;
        /// <summary>
        /// Sets up the Boggle Database Manager.
        /// </summary>
        /// <param name="dbConnectionString">The string that contains the server, database name, uid, and password necessary to connect to the database we are to use.</param>
        public BoggleDatabaseManager(string dbConnectionString)
        {
            this.connectionString = dbConnectionString;
            Initialize();


        }


        private void Initialize()
        {
            connection = new MySqlConnection(connectionString);
        }




        /// <summary>
        /// Records the results of a boggle game into the database.
        /// </summary>
        /// <param name="nameP1">The name of Player 1</param>
        /// <param name="nameP2">the name of Player 2</param>
        /// <param name="theBoggleBoardUsed">The 16-character string of the boggle board that was used.</param>
        /// <param name="gameTime">The length of the game in seconds.</param>
        /// <param name="p1Score">Player 1's Score at the end of the game</param>
        /// <param name="p2Score">Player 2's Score at the end of the game</param>
        /// <param name="p1LegalWords">Legal words played by Player 1</param>
        /// <param name="p2LegalWords">Legal words played by Player 2</param>
        /// <param name="validWordsPlayedByBothPlayers">Legal Words played by both players</param>
        /// <param name="p1IllegalWords">Illegal words played by Player 1</param>
        /// <param name="p2IllegalWords">Illegal words played by Player 2</param>
        /// <returns></returns>
        internal void recordGameResults(string nameP1, string nameP2, string theBoggleBoardUsed, int gameTime, int p1Score, int p2Score, IEnumerable<string> p1LegalWords, IEnumerable<string> p2LegalWords, IEnumerable<string> validWordsPlayedByBothPlayers, IEnumerable<string> p1IllegalWords, IEnumerable<string> p2IllegalWords)
        {

            PlayerInfoInsert(nameP1, nameP2);
            GameOutcomeInsert(nameP1, nameP2, theBoggleBoardUsed, gameTime, p1Score, p2Score);
            WordInsert(nameP1, nameP2, p1LegalWords, p2LegalWords, validWordsPlayedByBothPlayers, p1IllegalWords, p2IllegalWords);


          


        }

        /// <summary>
        /// gets the name in the playerinfo table
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        private bool NameInTable(string Name)
        {
            string PlayerName = "SELECT name from PlayerInfo WHERE Name ='" + Name + "';";
            string Output = "";
            if (this.OpenConnection() == true)
            {
                using (MySqlCommand cmd = new MySqlCommand(PlayerName, connection))
                {
                    
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Output = reader["Name"].ToString();
                        }


                    }
                }

            }
            CloseConnection();
            if (Output == "" || Output == " " || Output == null)
                return false;
            else
                return true;

        }

        /// <summary>
        /// insert into WordsPlayed table in database
        /// </summary>
        /// <param name="Player1Name"></param>
        /// <param name="Player2Name"></param>
        /// <param name="p1LegalWords"></param>
        /// <param name="p2LegalWords"></param>
        /// <param name="validWordsPlayedByBothPlayers"></param>
        /// <param name="p1IllegalWords"></param>
        /// <param name="p2IllegalWords"></param>
        public void WordInsert(string Player1Name, string Player2Name, IEnumerable<string> p1LegalWords, IEnumerable<string> p2LegalWords, IEnumerable<string> validWordsPlayedByBothPlayers, IEnumerable<string> p1IllegalWords, IEnumerable<string> p2IllegalWords)
        {
            int Player1ID = GetPlayerID(Player1Name);
            int Player2ID = GetPlayerID(Player2Name);
            int GameID = GetGameID(Player1ID);
            string input;
            foreach (string n in p1LegalWords)
            {
                input = "WordsPlayed (theWord, gameIDPlayedIn, playerIDThatPlayedIt, LegalWord) VALUES('" + n + "', '" + GameID + "', '" + Player1ID + "', '1');";
                Insert(input);
            }

            foreach (string n in p2LegalWords)
            {
                input = "WordsPlayed (theWord, gameIDPlayedIn, playerIDThatPlayedIt, LegalWord) VALUES('" + n + "', '" + GameID + "', '" + Player2ID + "', '1');";
                Insert(input);
            }

            foreach (string n in validWordsPlayedByBothPlayers)
            {
                input = "WordsPlayed (theWord, gameIDPlayedIn, playerIDThatPlayedIt, LegalWord) VALUES('" + n + "', '" + GameID + "', '" + Player1ID + "', '1');";
                Insert(input);
                input = "WordsPlayed (theWord, gameIDPlayedIn, playerIDThatPlayedIt, LegalWord) VALUES('" + n + "', '" + GameID + "', '" + Player2ID + "', '1');";
                Insert(input);
            }

            foreach (string n in p1IllegalWords)
            {
                input = "WordsPlayed (theWord, gameIDPlayedIn, playerIDThatPlayedIt, LegalWord) VALUES('" + n + "', '" + GameID + "', '" + Player1ID + "', '0');";
                Insert(input);
            }

            foreach (string n in p2IllegalWords)
            {
                input = "WordsPlayed (theWord, gameIDPlayedIn, playerIDThatPlayedIt, LegalWord) VALUES('" + n + "', '" + GameID + "', '" + Player2ID + "', '0');";
                Insert(input);
            }
        }

        /// <summary>
        /// helper method to get game ID from the GameResults
        /// </summary>
        /// <param name="PlayerID"></param>
        /// <returns></returns>
        private int GetGameID(int PlayerID)
        {
            string GameID = "SELECT gameID from GameResults WHERE (player1_ID ='" + PlayerID + "') OR (player2_Id = '" + PlayerID + "');";
            int ID = 0;
            if (this.OpenConnection() == true)
            {
                using (MySqlCommand cmd = new MySqlCommand(GameID, connection))
                {
                    
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ID = (int)reader["gameID"];
                        }
                    }
                }

            }
            CloseConnection();
            return ID;
        }

        /// <summary>
        /// Method that creates the string to be inserted for player names.
        /// </summary>
        /// <param name="nameP1"></param>
        /// <param name="nameP2"></param>
        public void PlayerInfoInsert(string nameP1, String nameP2)
        {
            string input = "";
            for (int i = 0; i < 2; i++)
            {
                switch (i)
                {
                    case 0:
                        if (NameInTable(nameP1))
                            return;
                        input = "PlayerInfo (Name) VALUES('" + nameP1 + "');";
                        Insert(input);
                        break;
                    case 1:
                        if (NameInTable(nameP2))
                            return;
                        input = "PlayerInfo (Name) VALUES('" + nameP2 + "');";
                        Insert(input);
                        break;

                }
            }
        }

        /// <summary>
        /// Method to get the Player ID for each player
        /// </summary>
        /// <param name="playerName"></param>
        /// <returns></returns>
        public int GetPlayerID(string playerName)
        {
            string PlayerID = "SELECT playerID from PlayerInfo WHERE Name ='" + playerName + "';";
            int ID = 0;
            if (this.OpenConnection() == true)
            {
                using (MySqlCommand cmd = new MySqlCommand(PlayerID, connection))
                {
                    //cmd.Parameters.AddWithValue("@pname", playerName);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ID = (int)reader["playerID"];
                        }


                    }
                }

            }
            CloseConnection();
            return ID;


        }


        /// <summary>
        /// Insert into database table GameResults
        /// </summary>
        /// <param name="player1Name"></param>
        /// <param name="player2Name"></param>
        /// <param name="boardConfig"></param>
        /// <param name="gameTime"></param>
        /// <param name="p1Score"></param>
        /// <param name="p2Score"></param>
        public void GameOutcomeInsert(string player1Name, string player2Name, string boardConfig, int gameTime, int p1Score, int p2Score)
        {
            string input;
            int player1ID, player2ID;
            player1ID = GetPlayerID(player1Name);
            player2ID = GetPlayerID(player2Name);

            string dateTime = System.DateTime.Now.ToString();

            input = "GameResults (player1_ID, player2_ID, dateTime, boardConfig, timeLimit, player1_Score, player2_Score) VALUES('" + player1ID + "', '" + player2ID + "', '" + dateTime + "', '" +
                boardConfig + "', '" + gameTime + "', '" + p1Score + "', '" + p2Score + "');";
            Insert(input);
        }

        /// <summary>
        /// helper method for inserting into table
        /// </summary>
        /// <param name="input"></param>
        private void Insert(string input)
        {
            string insert = "INSERT INTO " + input;
            

            //open connection
            if (this.OpenConnection() == true)
            {
                //create command and assign the query and connection from the constructor
                MySqlCommand command = new MySqlCommand(insert, connection);
                

                //Execute command for insertion
                command.ExecuteNonQuery();

                //close connection
                this.CloseConnection();
            }
        }

        /// <summary>
        /// Opens Connection to mySQL database
        /// </summary>
        /// <returns></returns>
        private bool OpenConnection()
        {
            try
            {
                connection.Open();
                return true;
            }
            catch (MySqlException fail)
            {
                
                switch (fail.Number)
                {
                    case 0:
                        break;

                    
                }
                return false;
            }
        }

        //Close connection
        private bool CloseConnection()
        {
            try
            {
                connection.Close();
                return true;
            }
            catch (MySqlException ex)//not implementedS
            {
                return false;
            }
        }

        /// <summary>
        /// Populates the required lists from the database.
        /// </summary>
        /// <param name="playerName"></param>
        /// <param name="numGames_Won"></param>
        /// <param name="numGames_Lost"></param>
        /// <param name="numGames_Tied"></param>
        /// <returns></returns>
        internal bool getPlayersStatTable(out List<string> playerName, out List<int> numGames_Won, out List<int> numGames_Lost, out List<int> numGames_Tied)
        {
            playerName = new List<string>();
            numGames_Won = new List<int>();
            numGames_Tied = new List<int>();
            numGames_Lost = new List<int>();
            string PlayerID = "SELECT * from PlayerInfo";
            List<int> ID = new List<int>();
            if (this.OpenConnection() == true)
            {
                using (MySqlCommand cmd = new MySqlCommand(PlayerID, connection))
                {

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ID.Add((int)reader["playerID"]);
                            string name = (string)reader["Name"];
                            playerName.Add(name);



                        }

                        //reader.Close();
                    }

                    for (int i = 0; i < ID.Count; i++)
                    {
                        numGames_Won.Add(GamesWon(ID.ElementAt(i)));
                        numGames_Lost.Add(GamesLost(ID.ElementAt(i)));
                        numGames_Tied.Add(GamesTied(ID.ElementAt(i)));
                    }



                }

            }
            CloseConnection();
            return true;
        }

        /// <summary>
        /// Gets the number of games lost per ID
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        private int GamesLost(int ID)
        {
            int number = 0;
            string count = "SELECT COUNT(*) FROM GameResults WHERE(" + ID + "= player1_ID AND player1_Score < player2_Score) OR (" + ID + "= player2_ID AND player2_Score < player1_Score);";
            using (MySqlCommand cmd = new MySqlCommand(count, connection))
            {

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {

                        int.TryParse(reader["COUNT(*)"].ToString(), out number);



                    }


                }
            }
            return number;
        }

        /// <summary>
        /// Gets the number of games tied per ID
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        private int GamesTied(int ID)
        {
            int number = 0;
            string count = "SELECT COUNT(*) FROM GameResults WHERE(" + ID + "= player1_ID AND player1_Score = player2_Score) OR (" + ID + "= player2_ID AND player2_Score = player1_Score);";
            using (MySqlCommand cmd = new MySqlCommand(count, connection))
            {

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int.TryParse(reader["COUNT(*)"].ToString(), out number);
                    }
                }
            }
            return number;
        }

        /// <summary>
        /// Gets the number of games won per ID
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        private int GamesWon(int ID)
        {
            int number = 0;
            string count = "SELECT COUNT(*) FROM GameResults WHERE(" + ID + "= player1_ID AND player1_Score > player2_Score) OR (" + ID + "= player2_ID AND player2_Score > player1_Score);";
            using (MySqlCommand cmd = new MySqlCommand(count, connection))
            {

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int.TryParse(reader["COUNT(*)"].ToString(), out number);
                    }
                }
            }
            return number;
        }

        /// <summary>
        /// Populates the required lists from the database.
        /// </summary>
        /// <param name="playerName">Name of the player who we want to get statistics on.</param>
        /// <param name="GameID"></param>
        /// <param name="DateTime"></param>
        /// <param name="OpponentsName"></param>
        /// <param name="PlayersScore"></param>
        /// <param name="OpponentsScore"></param>
        /// <returns></returns>
        internal bool getGamesStatTable(string playerName, out List<int> GameID, out List<string> DateTime, out List<string> OpponentsName, out List<int> PlayersScore, out List<int> OpponentsScore)
        {
            // TODO:
            GameID = new List<int>();
            DateTime = new List<string>();
            OpponentsName = new List<string>();
            PlayersScore = new List<int>();
            OpponentsScore = new List<int>();

            return false;
        }

        /// <summary>
        /// Populates the required lists from the database.
        /// </summary>
        /// <param name="GameID">The ID of the game we wish to view data for.</param>
        /// <param name="p1Name"></param>
        /// <param name="p2Name"></param>
        /// <param name="p1Score"></param>
        /// <param name="p2Score"></param>
        /// <param name="dateTime"></param>
        /// <param name="boggleBoard"></param>
        /// <param name="timeLimit"></param>
        /// <param name="p1Legal"></param>
        /// <param name="p2Legal"></param>
        /// <param name="inCommon"></param>
        /// <param name="p1Ilegal"></param>
        /// <param name="p2Ilegal"></param>
        /// <returns></returns>
        internal bool getGameInfo(string GameID, out string p1Name, out string p2Name, out int p1Score, out int p2Score, out string dateTime, out string boggleBoard, out int timeLimit, out List<string> p1Legal, out List<string> p2Legal, out List<string> inCommon, out List<string> p1Ilegal, out List<string> p2Ilegal)
        {

            p1Name = getPlayer1ID(GameID);
            p2Name = getPlayer2ID(GameID);
            int p1ID = GetPlayerID(p1Name);
            int p2ID = GetPlayerID(p2Name);
            p1Score = GetPlayer1Score(GameID);
            p2Score = GetPlayer2Score(GameID);
            dateTime = GetDateTime(GameID);
            boggleBoard = GetBoggleBoard(GameID);
            timeLimit = GetTimeLimit(GameID);
            p1Legal = GetP1Legal(p1ID, p2ID);
            p2Legal = GetP1Legal(p2ID, p1ID);
            inCommon = null;
            p1Ilegal = null;
            p2Ilegal = null;
            return true;
            // TODO:
            //throw new NotImplementedException();
        }

        private List<string> GetP1Legal(int p1ID, int p2ID)
        {
            string PlayerWord = "SELECT theWord from WordsPlayed WHERE playerIDThatPlayedIt ='" + p1ID + "' AND playerIDThatPlayedIt != '" + p2ID + "'  AND legalWord = '1';";
            List<String> words = new List<string>();

            if (this.OpenConnection() == true)
            {
                using (MySqlCommand cmd = new MySqlCommand(PlayerWord, connection))
                {
                    //cmd.Parameters.AddWithValue("@pname", playerName);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            words.Add(reader["theWord"].ToString());
                        }


                    }
                }

                //Name = GetPlayerName(ID);
            }
            CloseConnection();
            return words;
        }

        private int GetTimeLimit(string GameID)
        {
            string PlayerScore = "SELECT timeLimit from GameResults WHERE gameID ='" + GameID + "';";
            int time = 0;

            if (this.OpenConnection() == true)
            {
                using (MySqlCommand cmd = new MySqlCommand(PlayerScore, connection))
                {
                    //cmd.Parameters.AddWithValue("@pname", playerName);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            time = (int)reader["timeLimit"];
                        }


                    }
                }

                //Name = GetPlayerName(ID);
            }
            CloseConnection();
            return time;
        }

        private string GetBoggleBoard(string GameID)
        {
            string GameBoard = "SELECT boardConfig from GameResults WHERE gameID ='" + GameID + "';";
            String Board = "";

            if (this.OpenConnection() == true)
            {
                using (MySqlCommand cmd = new MySqlCommand(GameBoard, connection))
                {
                    //cmd.Parameters.AddWithValue("@pname", playerName);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Board = reader["boardConfig"].ToString();
                        }


                    }
                }

                //Name = GetPlayerName(ID);
            }
            CloseConnection();
            return Board;
        }

        /// <summary>
        /// gets the date from the gameresults table
        /// </summary>
        /// <param name="GameID"></param>
        /// <returns></returns>
        private String GetDateTime(string GameID)
        {
            
            string PlayerScore = "SELECT dateTime from GameResults WHERE gameID ='" + GameID + "';";
            String Date = "";
            
            if (this.OpenConnection() == true)
            {
                using (MySqlCommand cmd = new MySqlCommand(PlayerScore, connection))
                {
                    //cmd.Parameters.AddWithValue("@pname", playerName);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Date = reader["dateTime"].ToString();
                        }


                    }
                }

                //Name = GetPlayerName(ID);
            }
            CloseConnection();
            return Date;
        }

        /// <summary>
        /// Gets player 2 score
        /// 
        /// </summary>
        /// <param name="GameID"></param>
        /// <returns></returns>
        private int GetPlayer2Score(string GameID)
        {

            string PlayerScore = "SELECT player2_Score from GameResults WHERE gameID ='" + GameID + "';";
            int score = 0;

            if (this.OpenConnection() == true)
            {
                using (MySqlCommand cmd = new MySqlCommand(PlayerScore, connection))
                {
                    //cmd.Parameters.AddWithValue("@pname", playerName);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            score = (int)reader["player2_Score"];
                        }


                    }
                }

                //Name = GetPlayerName(ID);
            }
            CloseConnection();
            return score;
        }


        /// <summary>
        /// get player 1 score
        /// </summary>
        /// <param name="GameID"></param>
        /// <returns></returns>
        private int GetPlayer1Score(string GameID)
        {
            string PlayerScore = "SELECT player1_Score from GameResults WHERE gameID ='" + GameID + "';";
            int score = 0;

            if (this.OpenConnection() == true)
            {
                using (MySqlCommand cmd = new MySqlCommand(PlayerScore, connection))
                {
                    //cmd.Parameters.AddWithValue("@pname", playerName);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            score = (int)reader["player1_Score"];
                        }


                    }
                }

                //Name = GetPlayerName(ID);
            }
            CloseConnection();
            return score;

        }

        /// <summary>
        /// get player 1 ID then name
        /// 
        /// </summary>
        /// <param name="GameID"></param>
        /// <returns></returns>
        private string getPlayer1ID(string GameID)
        {
            string PlayerID = "SELECT player1_ID from GameResults WHERE gameID ='" + GameID + "';";
            int ID = 0;
            String Name = "";
            if (this.OpenConnection() == true)
            {
                using (MySqlCommand cmd = new MySqlCommand(PlayerID, connection))
                {
                    //cmd.Parameters.AddWithValue("@pname", playerName);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ID = (int)reader["player1_ID"];
                        }


                    }
                }

                Name = GetPlayerName(ID);
            }
            CloseConnection();
            return Name;


        }

        /// <summary>
        /// gets the player name using the player ID
        /// </summary>
        /// <param name="PID"></param>
        /// <returns></returns>
        private string GetPlayerName(int PID)
        {
            string PlayerID = "SELECT Name from PlayerInfo WHERE PlayerID ='" + PID + "';";
            int ID = 0;
            String Name = "";
            if (this.OpenConnection() == true)
            {
                using (MySqlCommand cmd = new MySqlCommand(PlayerID, connection))
                {
                    //cmd.Parameters.AddWithValue("@pname", playerName);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Name = reader["Name"].ToString();
                        }


                    }
                }


            }
            CloseConnection();
            return Name;
        }

        /// <summary>
        /// Gets the player Name using the ID
        /// </summary>
        /// <param name="GameID"></param>
        /// <returns></returns>
        private string getPlayer2ID(string GameID)
        {
            string PlayerID = "SELECT player2_ID from GameResults WHERE gameID ='" + GameID + "';";
            int ID = 0;
            String Name = "";
            if (this.OpenConnection() == true)
            {
                using (MySqlCommand cmd = new MySqlCommand(PlayerID, connection))
                {
                    //cmd.Parameters.AddWithValue("@pname", playerName);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ID = (int)reader["player2_ID"];
                        }


                    }
                }

                Name = GetPlayerName(ID);
            }
            CloseConnection();
            return Name;


        }





    }
}
