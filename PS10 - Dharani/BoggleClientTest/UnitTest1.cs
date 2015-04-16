using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Sockets;
using System.Net;
using BoggleClient;
using System.Text;
using System.Threading;
using Boggle;
using System.Collections.Generic;
using System.IO;

namespace BoggleClientTest
{
    [TestClass]
    public class UnitTest1
    {
        // Data that is shared across threads

        private string message;
        private string[] startgame;
        private string[] score;
        private string[] stop;

        BoggleServer server = null;



        // Timeout used in test case
        //private static int timeout = 4000;

        [TestMethod]
        public void ClientModelTest()
        {
            // Create and start a server and client.

            BoggleModel client = null;
            BoggleModel client2 = null;

            // build the dictionary to look up valid words
            HashSet<string> dictionary = new HashSet<string>();
            string filePath = "dictionary.txt";
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

            try
            {
                
                server = new BoggleServer(10, dictionary, "SERSPATGLINESERS");

                client = new BoggleModel();

                client.IncomingStartEvent += StartGameReceived;
                client.IncomingStopEvent += StopGameReceived;
                client.IncomingTimeEvent += TimeReceived;
                client.IncomingScoreEvent += ScoreReceived;
                client.IncomingTerminatedEvent += TerminatedReceived;
                client.ServerEvent += SeverReceived;

                client.Connect("localhost", "taylor", 2000);

                client2 = new BoggleModel();
                client2.Connect("localhost", "dharani", 2000);




                //Thread.Sleep(1000);
                // Now send the data.  Hope those receive requests didn't block!



                //sendSocket.BeginSend(msg, (e, o) => { }, null);

                Thread.Sleep(1000);

                client.SendMessage("WORD line");
                client.SendMessage("WORD line");
                client2.SendMessage("WORD sears");
                client.SendMessage("WORD pat");
                client2.SendMessage("WORD pat");

              
                Thread.Sleep(1000);

                Thread.Sleep(12000);

                // Make sure the lines were received properly.
                Assert.AreEqual(startgame[1], "SERSPATGLINESERS");
                Assert.AreEqual(startgame[2], "10");
                Assert.AreEqual(startgame[3], "dharani");
                
                Assert.AreEqual(score[1], "1");
                Assert.AreEqual(score[2], "2");
                                
                Assert.AreEqual("1", stop[1]);
                Assert.AreEqual("LINE", stop[2]);
                Assert.AreEqual("1", stop[3]);
                Assert.AreEqual("SEARS", stop[4]);
                Assert.AreEqual("1", stop[5]);
                Assert.AreEqual("PAT", stop[6]);
                Assert.AreEqual("0", stop[7]);
                Assert.AreEqual("0", stop[8]);

             /*   
                Assert.AreEqual("no words", stop[1]);
                Assert.AreEqual("no words", stop[2]);
                Assert.AreEqual("no words", stop[3]);
                Assert.AreEqual("no words", stop[4]);
                /*
                endMessage[0] = player1 + "taylor played the following valid word/s: LINE";                
                endMessage[1] = player2 + " played the following valid word/s:";
                endMessage[2] = "You both played the following word/s:";
                endMessage[3] = player1 + " played the following invalid word/s:";
                endMessage[4] = player2 + " played the following invalid word/s:";
                */


            }
            finally
            {

            }
        }


        private void SeverReceived(string obj)
        {
            //throw new NotImplementedException();
        }

        private void TerminatedReceived(string obj)
        {
            //throw new NotImplementedException();
        }

        private void ScoreReceived(string[] obj)
        {
            score = obj;
        }

        private void TimeReceived(string obj)
        {
            //throw new NotImplementedException();
        }

        private void StopGameReceived(string[] obj)
        {
            stop = obj;
        }

        private void StartGameReceived(string[] obj)
        {
            startgame = obj;
        }

        private void MessageReceived(string obj)
        {
            message = obj;
        }


    }
}
