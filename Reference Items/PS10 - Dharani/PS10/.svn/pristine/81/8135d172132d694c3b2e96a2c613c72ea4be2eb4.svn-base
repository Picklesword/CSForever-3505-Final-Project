// Author: Taylor Morris and Dharani Adhikari
// Fall 2014

using CustomNetworking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Boggle
{
    /// <summary>
    /// All the information needed to describe a Boggle player.  For example, name, connecting socket information,
    /// score, words played, etc.
    /// </summary>
    class Player
    {
        // Member Variables
        public String name;

        public Socket socket;
        public StringSocket stringSocket;

        public HashSet<String> validWords = new HashSet<string>();
        public HashSet<String> invalidWords = new HashSet<string>();

        public int score = 0;

        /// <summary>
        /// Constructor for the player class, assigns the player a new socket
        /// </summary>
        /// <param name="s">Socket</param>
        public Player(Socket s)
        {
            socket = s;
            stringSocket = new StringSocket(s, UTF8Encoding.Default);
        }
    }
}
