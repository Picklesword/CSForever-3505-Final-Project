// Author: Taylor Morris and Dharani Adhikari
// Fall 2014

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Text;
using CustomNetworking;
using System.Collections.Generic;
using Boggle;


namespace BoggleServerTest
{

    [TestClass]
    public class BoggleServerTest
    {
        /// <summary>
        ///Tests all of the methods associated with normal gameplay, when the users behave in 
        ///an ideal way.
        ///</summary>
        [TestMethod()]
        public void normalGameplay()
        {
            
            new normalGameplayClass().run();
        }

        public class normalGameplayClass
        {

            // Data that is shared across threads
            private ManualResetEvent mre1;
            private ManualResetEvent mre2;

            private List<String> client1Messages = new List<string>();

            private List<String> client2Messages = new List<string>();


            // Timeout used in test case
            private static int timeout = 6000;

            public void run()
            {
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
                // Create and start a server and client.
                BoggleServer server = null;

                TcpClient client1 = null;
                TcpClient client2 = null;

                try
                {
                    server = new BoggleServer(15, dictionary, "SERSPATGLINESERS");

                    client1 = new TcpClient("localhost", 2000);
                    client2 = new TcpClient("localhost", 2000);

                    // Obtain the sockets from the two ends of the connection.  We are using the blocking AcceptSocket()
                    // method here, which is OK for a test case.
                    //Socket serverSocket = server.AcceptSocket();
                    StringSocket clientSocket1 = new StringSocket(client1.Client, UTF8Encoding.Default);
                    StringSocket clientSocket2 = new StringSocket(client2.Client, UTF8Encoding.Default);

                    // This will coordinate communication between the threads of the test cases
                    mre1 = new ManualResetEvent(false);
                    mre2 = new ManualResetEvent(false);

                    // Make two receive requests
                    clientSocket1.BeginReceive(CompletedReceive1, clientSocket1);
                    clientSocket2.BeginReceive(CompletedReceive2, clientSocket2);

                    // Now send the data.  Hope those receive requests didn't block!
                    clientSocket1.BeginSend("play dharani\n", (ee, pp) => { }, clientSocket1);
                    clientSocket2.BeginSend("play taylor\n", (ee, pp) => { }, clientSocket2);

                    // client 1 submits a 1 point word
                    clientSocket1.BeginSend("word rent\n", (ee, pp) => { }, clientSocket1);
                    Thread.Sleep(500);

                    // client 1 submits a 2 point word
                    clientSocket1.BeginSend("word slips\n", (ee, pp) => { }, clientSocket1);
                    Thread.Sleep(500);

                    // client 1 submits a 3 point word
                    clientSocket1.BeginSend("word renter\n", (ee, pp) => { }, clientSocket1);
                    Thread.Sleep(500);

                    // client 1 submits a 5 point word
                    clientSocket1.BeginSend("word slaters\n", (ee, pp) => { }, clientSocket1);
                    Thread.Sleep(500);

                    // client 1 submits a 11 point word
                    clientSocket1.BeginSend("word stainers\n", (ee, pp) => { }, clientSocket1);
                    Thread.Sleep(500);

                    // client 1 sends a word that is too short
                    clientSocket1.BeginSend("word to\n", (ee, pp) => { }, clientSocket1);
                    Thread.Sleep(500);

                    // client 2 submits a 3 point word client 1 already has
                    clientSocket2.BeginSend("word renter\n", (ee, pp) => { }, clientSocket2);
                    Thread.Sleep(500);

                    // client 2 submits an invalid word that can't be formed
                    clientSocket2.BeginSend("word slapp\n", (ee, pp) => { }, clientSocket2);
                    Thread.Sleep(500);

                    // client 2 submits an invalid that can be formed
                    clientSocket2.BeginSend("word replanters\n", (ee, pp) => { }, clientSocket2);
                    Thread.Sleep(500);                    

                    // client 2 submits an original 3 length word for 1 point
                    clientSocket2.BeginSend("word tap\n", (ee, pp) => { }, clientSocket2);
                    Thread.Sleep(500);

                    // client 1 submits the same 3 length word
                    clientSocket1.BeginSend("word tap\n", (ee, pp) => { }, clientSocket1);
                    Thread.Sleep(500);

                    // client 2 submits an original 4 length word for 1 point
                    clientSocket2.BeginSend("word line\n", (ee, pp) => { }, clientSocket2);
                    Thread.Sleep(500);

                    // client 1 submits the same 4 length word
                    clientSocket1.BeginSend("word line\n", (ee, pp) => { }, clientSocket1);
                    Thread.Sleep(500);

                    // client 1 submits the same 4 length word again
                    clientSocket1.BeginSend("word line\n", (ee, pp) => { }, clientSocket1);
                    Thread.Sleep(500);

                    

                    Thread.Sleep(10000);

                    // Make sure the lines were received properly.
                    Assert.AreEqual(true, mre1.WaitOne(timeout), "Timed out waiting 1");
                    Assert.AreEqual("START SERSPATGLINESERS 15 taylor", client1Messages[0]);

                    Assert.AreEqual(true, mre2.WaitOne(timeout), "Timed out waiting 2");
                    Assert.AreEqual("START SERSPATGLINESERS 15 dharani", client2Messages[0]);

                    // assert the program works in its entirety by checking score updates
                    Assert.AreEqual("SCORE 1 0", client1Messages[1]);
                    Assert.AreEqual("SCORE 0 1", client2Messages[1]);

                    Assert.AreEqual("SCORE 3 0", client1Messages[2]);
                    Assert.AreEqual("SCORE 0 3", client2Messages[2]);

                    Assert.AreEqual("SCORE 6 0", client1Messages[3]);
                    Assert.AreEqual("SCORE 0 6", client2Messages[3]);

                    Assert.AreEqual("SCORE 11 0", client1Messages[4]);
                    Assert.AreEqual("SCORE 0 11", client2Messages[4]);

                    Assert.AreEqual("SCORE 22 0", client1Messages[5]);
                    Assert.AreEqual("SCORE 0 22", client2Messages[5]);

                    Assert.AreEqual("SCORE 19 0", client1Messages[6]);
                    Assert.AreEqual("SCORE 0 19", client2Messages[6]);

                    Assert.AreEqual("SCORE 19 -1", client1Messages[7]);
                    Assert.AreEqual("SCORE -1 19", client2Messages[7]);

                    Assert.AreEqual("SCORE 19 -2", client1Messages[8]);
                    Assert.AreEqual("SCORE -2 19", client2Messages[8]);

                    Assert.AreEqual("SCORE 19 -1", client1Messages[9]);
                    Assert.AreEqual("SCORE -1 19", client2Messages[9]);

                    Assert.AreEqual("SCORE 19 -2", client1Messages[10]);
                    Assert.AreEqual("SCORE -2 19", client2Messages[10]);

                    Assert.AreEqual("SCORE 19 -1", client1Messages[11]);
                    Assert.AreEqual("SCORE -1 19", client2Messages[11]);

                    Assert.AreEqual("SCORE 19 -2", client1Messages[12]);
                    Assert.AreEqual("SCORE -2 19", client2Messages[12]);

                    // assert end of game report is correct
                    Assert.AreEqual("SCORE 19 -2", client1Messages[13]);
                    Assert.AreEqual("SCORE -2 19", client2Messages[13]);

                    Assert.AreEqual("STOP 4 RENT SLIPS SLATERS STAINERS 0 3 RENTER TAP LINE 0 2 SLAPP REPLANTERS", client1Messages[14]);
                    Assert.AreEqual("STOP 0 4 RENT SLIPS SLATERS STAINERS 3 RENTER TAP LINE 2 SLAPP REPLANTERS 0", client2Messages[14]);

                }
                finally
                {
                    client1.Close();
                    client2.Close();
                    server.Close();
                }
            }


            // This is the callback for the first receive request.  We can't make assertions anywhere
            // but the main thread, so we write the values to member variables so they can be tested
            // on the main thread.
            private void CompletedReceive1(String s, Exception o, object payload)
            {
                client1Messages.Add(s);
                mre1.Set();

                StringSocket temp = (StringSocket)payload;
                temp.BeginReceive(receivedMessage1, temp);
            }

            // This is the callback for the second receive request.
            private void CompletedReceive2(String s, Exception o, object payload)
            {
                client2Messages.Add(s);
                mre2.Set();

                StringSocket temp = (StringSocket)payload;
                temp.BeginReceive(receivedMessage2, temp);
            }

            private void receivedMessage1(string s, Exception e, object payload)
            {
                if (!s.StartsWith("TIME"))
                {
                    client1Messages.Add(s);
                }

                StringSocket temp = (StringSocket)payload;
                temp.BeginReceive(receivedMessage1, temp);
            }

            private void receivedMessage2(string s, Exception e, object payload)
            {
                if (!s.StartsWith("TIME"))
                {
                    client2Messages.Add(s);
                }

                StringSocket temp = (StringSocket)payload;
                temp.BeginReceive(receivedMessage2, temp);
            }
        }

        /// <summary>
        ///Tests all of the methods associated with normal gameplay, when the users behave in 
        ///an ideal way.
        ///</summary>
        [TestMethod()]
        public void unexpectedGameplay1()
        {
            new unexpectedGameplay1Class().run();
        }

        public class unexpectedGameplay1Class
        {
            // Data that is shared across threads
            private ManualResetEvent mre1;
            private ManualResetEvent mre2;

            private List<String> client1Messages = new List<string>();

            private List<String> client2Messages = new List<string>();


            // Timeout used in test case
            private static int timeout = 6000;

            public void run()
            {
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
                // Create and start a server and client.
                BoggleServer server = null;
                TcpClient client1 = null;
                TcpClient client2 = null;

                try
                {
                    server = new BoggleServer(5, dictionary, "quequeqeqeqeeeeu");

                    client1 = new TcpClient("localhost", 2000);
                    client2 = new TcpClient("localhost", 2000);

                    // Obtain the sockets from the two ends of the connection.  We are using the blocking AcceptSocket()
                    // method here, which is OK for a test case.
                    //Socket serverSocket = server.AcceptSocket();
                    StringSocket clientSocket1 = new StringSocket(client1.Client, UTF8Encoding.Default);
                    StringSocket clientSocket2 = new StringSocket(client2.Client, UTF8Encoding.Default);

                    // This will coordinate communication between the threads of the test cases
                    mre1 = new ManualResetEvent(false);
                    mre2 = new ManualResetEvent(false);

                    // Make two receive requests
                    clientSocket1.BeginReceive(CompletedReceive1, clientSocket1);
                    clientSocket2.BeginReceive(CompletedReceive2, clientSocket2);

                    // send invalid game request message
                    clientSocket1.BeginSend("harley\n", (ee, pp) => { }, clientSocket1);
                    clientSocket2.BeginSend("taylor\n", (ee, pp) => { }, clientSocket2);
                    Thread.Sleep(500);

                    // send valid game request message
                    clientSocket1.BeginSend("play harley\n", (ee, pp) => { }, clientSocket1);
                    clientSocket2.BeginSend("play taylor\n", (ee, pp) => { }, clientSocket2);
                    Thread.Sleep(500);

                    // send invalid word message
                    clientSocket1.BeginSend("hello\n", (ee, pp) => { }, clientSocket1);
                    clientSocket2.BeginSend("world\n", (ee, pp) => { }, clientSocket2);
                    Thread.Sleep(500);

                    // send valid word message
                    clientSocket1.BeginSend("word queue\n", (ee, pp) => { }, clientSocket1);
                    clientSocket2.BeginSend("word qeeeq\n", (ee, pp) => { }, clientSocket2);
                    Thread.Sleep(500);

                    client2.Close();                    

                    Thread.Sleep(3000);

                    // Make sure the lines were received properly.
                    Assert.AreEqual(true, mre1.WaitOne(timeout), "Timed out waiting 1");
                    Assert.AreEqual("IGNORING harley", client1Messages[0]);

                    Assert.AreEqual(true, mre2.WaitOne(timeout), "Timed out waiting 2");
                    Assert.AreEqual("IGNORING taylor", client2Messages[0]);

                    // Make sure the lines were received properly and the random board generator functions properly
                    Assert.IsTrue(client1Messages[1].StartsWith("START "));
                    Assert.IsTrue(client1Messages[1].EndsWith(" 5 taylor"));
                    Assert.AreEqual(client1Messages[1].Length, 31);

                    Assert.IsTrue(client2Messages[1].StartsWith("START "));
                    Assert.IsTrue(client2Messages[1].EndsWith(" 5 harley"));
                    Assert.AreEqual(client2Messages[1].Length, 31);

                    // make sure we ignore things properly
                    Assert.AreEqual("IGNORING hello", client1Messages[2]);

                    Assert.AreEqual("IGNORING world", client2Messages[2]);

                    // assert the other player got the message that the connection was terminated
                    Assert.AreEqual("TERMINATED", client1Messages[5]);
                }

                finally
                {
                    client1.Close();
                    client2.Close();
                    server.Close();
                }
            }


            // This is the callback for the first receive request.  We can't make assertions anywhere
            // but the main thread, so we write the values to member variables so they can be tested
            // on the main thread.
            private void CompletedReceive1(String s, Exception o, object payload)
            {
                client1Messages.Add(s);
                mre1.Set();

                StringSocket temp = (StringSocket)payload;
                temp.BeginReceive(receivedMessage1, temp);
            }

            // This is the callback for the second receive request.
            private void CompletedReceive2(String s, Exception o, object payload)
            {
                client2Messages.Add(s);
                mre2.Set();

                StringSocket temp = (StringSocket)payload;
                temp.BeginReceive(receivedMessage2, temp);
            }

            private void receivedMessage1(string s, Exception e, object payload)
            {
                if (!s.StartsWith("TIME"))
                {
                    client1Messages.Add(s);
                }

                StringSocket temp = (StringSocket)payload;
                temp.BeginReceive(receivedMessage1, temp);
            }

            private void receivedMessage2(string s, Exception e, object payload)
            {
                if (!s.StartsWith("TIME"))
                {
                    client2Messages.Add(s);
                }

                StringSocket temp = (StringSocket)payload;
                temp.BeginReceive(receivedMessage2, temp);
            }
        }

        /// <summary>
        ///Tests all of the methods associated with normal gameplay, when the users behave in 
        ///an ideal way.
        ///</summary>
        [TestMethod()]
        public void unexpectedGameplay2()
        {
            new unexpectedGameplay2Class().run();
        }

        public class unexpectedGameplay2Class
        {
            // Data that is shared across threads
            private ManualResetEvent mre1;
            private ManualResetEvent mre2;

            private List<String> client1Messages = new List<string>();

            private List<String> client2Messages = new List<string>();


            // Timeout used in test case
            private static int timeout = 6000;

            public void run()
            {
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
                // Create and start a server and client.
                BoggleServer server = null;
                TcpClient client1 = null;
                TcpClient client2 = null;

                try
                {
                    server = new BoggleServer(5, dictionary);

                    client1 = new TcpClient("localhost", 2000);
                    client2 = new TcpClient("localhost", 2000);

                    // Obtain the sockets from the two ends of the connection.  We are using the blocking AcceptSocket()
                    // method here, which is OK for a test case.
                    //Socket serverSocket = server.AcceptSocket();
                    StringSocket clientSocket1 = new StringSocket(client1.Client, UTF8Encoding.Default);
                    StringSocket clientSocket2 = new StringSocket(client2.Client, UTF8Encoding.Default);

                    // This will coordinate communication between the threads of the test cases
                    mre1 = new ManualResetEvent(false);
                    mre2 = new ManualResetEvent(false);

                    // Make two receive requests
                    clientSocket1.BeginReceive(CompletedReceive1, clientSocket1);
                    clientSocket2.BeginReceive(CompletedReceive2, clientSocket2);

                    // Now send the data.  Hope those receive requests didn't block!
                    clientSocket1.BeginSend("play harley\n", (ee, pp) => { }, clientSocket1);
                    clientSocket2.BeginSend("play taylor\n", (ee, pp) => { }, clientSocket2);

                    Thread.Sleep(500);

                    client1.Close();
                    
                    Thread.Sleep(3000);

                    // Make sure the lines were received properly.
                    Assert.AreEqual(true, mre1.WaitOne(timeout), "Timed out waiting 1");
                    //Assert.AreEqual("START SERSPATGLINESERS 5 taylor", client1Messages[0]);

                    Assert.AreEqual(true, mre2.WaitOne(timeout), "Timed out waiting 2");
                    //Assert.AreEqual("START SERSPATGLINESERS 5 harley", client2Messages[0]);

                    // assert the other player got the message that the connection was terminated
                    Assert.AreEqual("TERMINATED", client2Messages[1]);

                }

                finally
                {
                    client1.Close();
                    client2.Close();
                    server.Close();
                }
            }


            // This is the callback for the first receive request.  We can't make assertions anywhere
            // but the main thread, so we write the values to member variables so they can be tested
            // on the main thread.
            private void CompletedReceive1(String s, Exception o, object payload)
            {
                client1Messages.Add(s);
                mre1.Set();

                StringSocket temp = (StringSocket)payload;
                temp.BeginReceive(receivedMessage1, temp);
            }

            // This is the callback for the second receive request.
            private void CompletedReceive2(String s, Exception o, object payload)
            {
                client2Messages.Add(s);
                mre2.Set();

                StringSocket temp = (StringSocket)payload;
                temp.BeginReceive(receivedMessage2, temp);
            }

            private void receivedMessage1(string s, Exception e, object payload)
            {
                if (!s.StartsWith("TIME"))
                {
                    client1Messages.Add(s);
                }

                StringSocket temp = (StringSocket)payload;
                temp.BeginReceive(receivedMessage1, temp);
            }

            private void receivedMessage2(string s, Exception e, object payload)
            {
                if (!s.StartsWith("TIME"))
                {
                    client2Messages.Add(s);
                }

                StringSocket temp = (StringSocket)payload;
                temp.BeginReceive(receivedMessage2, temp);
            }
        }

    }
}
