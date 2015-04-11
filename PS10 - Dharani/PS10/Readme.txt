kAuthor: Dharani Adhikari and Taylor Morris
Date: 12/11/2014

Time spent on googling: 1 hours
Time spent on coding: 12 hours
Time spent on debugging: 2 hours

Database Description:

Our database consists of 3 tables: Games, Players, and Words

Players has columns for auto-incrementing Player ID, player name, # wins, # losses, and # ties

Games has columns for auto-incrementing Game ID, player 1 ID, player 2 ID, game Date/Time, the 16 letters of the board, the game duration time, and each of the player's scores.

Words has columns for auto-incrementing wordID, the actual word, the ID of the game it was played in, the ID of the player who played it (zero is recorded for a word played by both players), and a binary field indicating whether the word was legal or not.



Database Calls:

Database calls from the Game class are used to record game information into the database:

"SELECT PlayerID from Players where Players.Name = '" + player + "'" - returns the player ID number from the database of a player, given their name

"INSERT INTO Players (Name, Win, Loss, Tie) VALUES (@name, 0, 0, 0)" - inserts a new player into the database if they do not already exist

"SELECT Win, Loss, Tie FROM Players WHERE PlayerID = " + PID - pulls down a player's win/loss record previous to the currently finishing game

"UPDATE Players SET Win = " + win + ", Loss = " + loss + ", Tie = " + tie + " WHERE PlayerID = " + PID - Updates the player's win/loss record as of the currently finishing game

"INSERT INTO Games (P1ID, P2ID, Date, Board, GameTime, P1Score, P2Score) VALUES (" + p1ID +", " + p2ID + ", " + "Now(), '" + boardString + "', " + timeOfGame + ", " + p1.score + ", " +p2.score +")" - Adds all the information of a finished game to the database

"SELECT GameID FROM Games ORDER BY GameID DESC LIMIT 1" - returns the ID of the just recorded game

"INSERT INTO Words (Word, GameID, PlayerID, Valid) VALUES ('" + s + "', " +currentGameID + ", " + PID + ", " + valid + ")" - records the words of the just finished game into the database



Database calls from the BoggleServer class are used to query the database and build relevant websites from the information:

"SELECT * from Players" - returns the entire player table, which contains player's names, and their win/loss records

"SELECT PlayerID from Players where Players.Name = '" + pName + "'" - returns the playerID of a player, given their name

"select * FROM Games JOIN Players ON (Games.P2ID = Players.PlayerID AND Games.P1ID = " + playerID + ") OR (Games.P1ID = Players.PlayerID AND Games.P2ID = " + playerID + ")" - selects all games that a player participated in (whether player 1 or 2) and joins it with the players table so that their names will also be available

"SELECT GameID FROM Games ORDER BY GameID DESC LIMIT 1" - returns the ID of the last game in the games table

"select * FROM Games,Words where Words.GameID = Games.GameID AND Games.GameID = " - returns all information from the games table and the words table where the gameID matches what is given, this returns all the game stats and the words that were played for a given game

"SELECT Name from Players where Players.PlayerID = " + p1ID - returns a player's name, given their playerID


