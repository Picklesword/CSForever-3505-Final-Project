using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BB;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Sockets;
using CustomNetworking;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Net;
using System.IO;

namespace BB.Tests
{
    [ExcludeFromCodeCoverage()]
    [TestClass()]
    public class BoggleServerTests
    {
        /// <summary>
        /// The common path to the dictionary file.
        /// </summary>
        private static string dictionaryPath = @"..\..\..\dictionary.txt";

        /// <summary>
        /// A O H U 
        /// I N H R 
        /// T S O A 
        /// F H T E
        /// ---------------
        /// Some valid words:
        /// ethos
        /// horah
        /// horahs
        /// instar
        /// torahs
        /// host
        /// rat
        /// rot
        /// hot
        /// shoe
        /// tin
        /// sin
        /// </summary>
        
        //These two objects are only used in the getOpenPort()
        private static object portlock = new object();
        private static int _openPort = 4001;
        /// <summary>
        /// Returns a port number that hasn't 
        /// been used by the tests yet (naive).
        /// </summary>
        /// <returns>An open port number.</returns>
        public static int getOpenPort()
        {
            //make sure port doesn't get all jacked up
            lock (portlock)
            {
                return _openPort++;
            }
        }

        /// <summary>
        /// Used to enforce which unit tests can add new connections to the CommonServer.
        /// </summary>
        private Object CommonServer_Lock = new Object();
        /// <summary>
        /// A server that is started before all the unit tests are run.
        /// It is used for many sets of players connecting simultaneously.
        /// </summary>
        static BoggleServer CommonServer;
        static int csGameLength = 8;


        //this is the first method that is run before all of the unit tests
        [ClassInitialize()]
        public static void SetupTheCommonServer(TestContext testContext)
        {
            //Create a boggle server to be used by many clients
            CommonServer = new BoggleServer(csGameLength, dictionaryPath, "AOHUINHRTSOAFHTE", getOpenPort());
            CommonServer.RunServer();
        }


        [TestMethod()]
        public void csPair1()
        {
            //*
            int use = CommonServer.playPort;
            /*/
            int use = 2000;
            //*/

            LinearClient p1 = new LinearClient(use);
            LinearClient p2 = new LinearClient(use);
            
            

            lock (CommonServer_Lock)
            {
                p1.play("p1");
                p2.play("p2");

                //wait until game has started
                p1.waitUntil_Start.WaitOne();
                p2.waitUntil_Start.WaitOne();
            }

            //Check the start message for accuracy
            Assert.AreEqual("START AOHUINHRTSOAFHTE "+csGameLength+" p2", p1.startMessage);
            Assert.AreEqual("START AOHUINHRTSOAFHTE " + csGameLength + " p1", p2.startMessage);

            //Play the game
            p1.guess("rot");
            Assert.AreEqual("SCORE 1 0", p1.getResponse(true));
            Assert.AreEqual("SCORE 0 1", p2.getResponse(true));
            p1.guess("lksdf");
            Assert.AreEqual("SCORE 0 0", p1.getResponse(true));
            Assert.AreEqual("SCORE 0 0", p2.getResponse(true));
            p1.guess("rot");
            Assert.AreEqual(null, p1.getResponse(false));
            Assert.AreEqual(null, p2.getResponse(false));
            p1.guess("ethos");
            Assert.AreEqual("SCORE 2 0", p1.getResponse(true));
            Assert.AreEqual("SCORE 0 2", p2.getResponse(true));
            p1.guess("horah");
            Assert.AreEqual("SCORE 4 0", p1.getResponse(true));
            Assert.AreEqual("SCORE 0 4", p2.getResponse(true));
            p1.guess("lksdf");
            Assert.AreEqual(null, p1.getResponse(false));
            Assert.AreEqual(null, p2.getResponse(false));


            p2.guess("rot");
            Assert.AreEqual("SCORE 0 3", p2.getResponse(true));
            Assert.AreEqual("SCORE 3 0", p1.getResponse(true));
            p2.guess("rot");
            Assert.AreEqual(null, p2.getResponse(false));
            Assert.AreEqual(null, p1.getResponse(false));
            p2.guess("horahs");
            Assert.AreEqual("SCORE 3 3", p2.getResponse(true));
            Assert.AreEqual("SCORE 3 3", p1.getResponse(true));
            p2.guess("lksdf");
            Assert.AreEqual("SCORE 2 3", p2.getResponse(true));
            Assert.AreEqual("SCORE 3 2", p1.getResponse(true));

            //wait until game ends
            p1.waitUntil_End.WaitOne();
            p2.waitUntil_End.WaitOne();

            //check the stop messges for accuracy
            Assert.AreEqual("STOP 2 ETHOS HORAH 1 HORAHS 1 ROT 1 LKSDF 1 LKSDF", p1.gameOverMessage);
            Assert.AreEqual("STOP 1 HORAHS 2 ETHOS HORAH 1 ROT 1 LKSDF 1 LKSDF", p2.gameOverMessage);

        }

        [TestMethod()]
        public void testRandomThings()
        {
            //EQATMSROSLADEHBR
            //Equator
            //Equators
            BoggleServer bs = new BoggleServer(8, dictionaryPath, "EQATMSROSLADEHBR", getOpenPort());
            bs.RunServer();
            LinearClient p1 = new LinearClient(bs.playPort);
            LinearClient p2 = new LinearClient(bs.playPort);

            //Person sends invalid name
            p1.send("hi");
            Assert.AreEqual("IGNORING hi", p1.getResponse(true));

            p1.play("p1");
            p2.play("p2");

            //wait until game has started
            p1.waitUntil_Start.WaitOne();
            p2.waitUntil_Start.WaitOne();


            //Check the start message for accuracy
            Assert.AreEqual("START EQATMSROSLADEHBR " + 8 + " p2", p1.startMessage);
            Assert.AreEqual("START EQATMSROSLADEHBR " + 8 + " p1", p2.startMessage);

            //Letter Q
            
            p1.guess("equator");
            p1.checkResponse("SCORE 5 0");
            p2.checkResponse("SCORE 0 5");
            
            //long words
            p2.guess("equators");
            p1.checkResponse("SCORE 5 11");
            p2.checkResponse("SCORE 11 5");

            //letters less than 3
            p1.guess("hi");
            p1.checkResponse(null);
            p2.checkResponse(null);

            //person sends invalid commands during game
            p2.send("Youre a jerk");
            p1.checkResponse(null);
            p2.checkResponse("IGNORING Youre a jerk");

            //wait until game ends
            p1.waitUntil_End.WaitOne();
            p2.waitUntil_End.WaitOne();

            //check the stop messges for accuracy
            Assert.AreEqual("STOP 1 EQUATOR 1 EQUATORS 0 0 0", p1.gameOverMessage);
            Assert.AreEqual("STOP 1 EQUATORS 1 EQUATOR 0 0 0", p2.gameOverMessage);

        }


        [TestMethod()]
        public void randomBoard()
        {
            //Invalid board
            BoggleServer bs = new BoggleServer(8, dictionaryPath, "EQAT", getOpenPort());
            bs.RunServer();
            int port = bs.playPort;

            LinearClient p1 = new LinearClient(port);
            LinearClient p2 = new LinearClient(port);
            LinearClient leaver = new LinearClient(port);
            //Person disconnects when waiting for name
            leaver.Disconnect();
            //First person in queue says something and gets ignored
            p1.play("p1");
            p1.sendAndWait("hi");
            p1.waitUntil_DoneSending.WaitOne();

            Console.WriteLine("/////");
            p1.checkResponse("IGNORING hi");
            //First person in queue left
            p1.Disconnect();
            //disconnected during game
            p1 = new LinearClient(port);

            p1.play("p1");
            p2.play("p2");

            //wait until game has started
            p1.waitUntil_Start.WaitOne();
            p2.waitUntil_Start.WaitOne();

            p1.Disconnect();
            p2.checkResponse("TERMINATED");


        }


        [TestMethod()]
        public void dictionaryDoesntExist()
        {
            BoggleServer server1 = null;

            server1 = new BoggleServer(5, "file.txt", "EQATMSROSLADEHBR", getOpenPort());

            server1.RunServer();

            LinearClient p1 = new LinearClient(server1.playPort);
            LinearClient p2 = new LinearClient(server1.playPort);


            p1.play("p1");
            p2.play("p2");

            //wait until game has started
            p1.waitUntil_Start.WaitOne();
            p2.waitUntil_Start.WaitOne();


            //Check the start message for accuracy
            Assert.AreEqual("START EQATMSROSLADEHBR " + 5 + " p2", p1.startMessage);
            Assert.AreEqual("START EQATMSROSLADEHBR " + 5 + " p1", p2.startMessage);

            //Letter Q

            p1.guess("equator");
            p1.checkResponse("SCORE -1 0");
            p2.checkResponse("SCORE 0 -1");

        }

        [TestMethod()]
        public void exceptionLoadingDictionary()
        {
            BoggleServer server1 = null;
            File.Open(@"..\..\..\oneWord.txt", FileMode.Open, FileAccess.Read, FileShare.None);
            server1 = new BoggleServer(5, @"..\..\..\oneWord.txt", "EQATMSROSLADEHBR", getOpenPort());
            
            server1.RunServer();

            LinearClient p1 = new LinearClient(server1.playPort);
            LinearClient p2 = new LinearClient(server1.playPort);


            p1.play("p1");
            p2.play("p2");

            //wait until game has started
            p1.waitUntil_Start.WaitOne();
            p2.waitUntil_Start.WaitOne();


            //Check the start message for accuracy
            Assert.AreEqual("START EQATMSROSLADEHBR " + 5 + " p2", p1.startMessage);
            Assert.AreEqual("START EQATMSROSLADEHBR " + 5 + " p1", p2.startMessage);

            //Letter Q

            p1.guess("equator");
            p1.checkResponse("SCORE -1 0");
            p2.checkResponse("SCORE 0 -1");

        }



        class LinearClient
        {
            Queue<string> received;
            ManualResetEvent waitUntil_validMessage;

            public string startMessage = null;
            public ManualResetEvent waitUntil_Start;

            public string gameOverMessage = null;
            public ManualResetEvent waitUntil_End; 

            StringSocket ss;
            Socket sock;

            public ManualResetEvent waitUntil_DoneSending;

            public LinearClient(int port)
            {
                TcpClient client = new TcpClient("localhost", port);
                sock = client.Client;

                received = new Queue<string>();
                waitUntil_validMessage = new ManualResetEvent(false);

                startMessage = null;
                waitUntil_Start = new ManualResetEvent(false);

                gameOverMessage = null;
                waitUntil_End = new ManualResetEvent(false);

                ss = new StringSocket(sock, new UTF8Encoding());

                ss.BeginReceive((s, e, o) => { FilterMessages(s, aScoreMessage); }, null);

                waitUntil_DoneSending = new ManualResetEvent(false);
            }

            public void send(string message)
            {
                ss.BeginSend(message + "\n", (e, o) => {  }, message);
            }

            public void sendAndWait(string s)
            {
                waitUntil_DoneSending.Reset();
                ss.BeginSend(s + "\n", (e, o) => { waitUntil_DoneSending.Set(); }, s);
                waitUntil_DoneSending.WaitOne();
            }

            public void play(String name)
            {
                send("play " + name);
            }

            public void guess(string word)
            {
                send("word " + word);

                //Assert.IsTrue(waitUntil_validMessage.WaitOne(1000), "Waiting for response to " + word);
            }

            public void checkResponse(string s)
            {
                if (s == null)
                    Assert.AreEqual(null, getResponse(false));
                else
                    Assert.AreEqual(s, getResponse(true));
            }

            public string getResponse(bool expectingAResponse)
            {
                Assert.AreEqual(expectingAResponse, waitUntil_validMessage.WaitOne(1000), 
                    "Was "+(expectingAResponse?"":"not ")+"expecting a response.");

                waitUntil_validMessage.Reset();

                if (received.Count == 0)
                    return null;
                else
                    return received.Dequeue();
            }

            //private void received(MessageReceivedFrom m)
            //{
            //    FilterMessages(m.message, usefulM);
            //}

            private void aScoreMessage(string s)
            {
                received.Enqueue(s);
                waitUntil_validMessage.Set();
            }


            private void FilterMessages(String Message, Action<String> scoreMessage)
            {
                ss.BeginReceive((s, e, o) => { FilterMessages(s, aScoreMessage); }, null);

                if (!Message.ToUpper().StartsWith("TIME "))
                {
                    if (Message.ToUpper().StartsWith("START "))
                    {
                        startMessage = Message;
                        waitUntil_Start.Set();
                    }
                    else if (Message.ToUpper().StartsWith("STOP "))
                    {
                        gameOverMessage = Message;
                        waitUntil_End.Set();
                    }
                    else
                        scoreMessage(Message);
                }
            }


            public void Disconnect()
            {
                //First check if it's connected
                if (sock.Connected)
                {
                    sock.Shutdown(SocketShutdown.Both);
                    sock.Close();
                }
            }

        }


        
        //class LinearClient
        //{
        //    Stack<string> p1Received;
        //    ManualResetEvent waitUntil_validMessage;

        //    public string startMessage = null;
        //    public ManualResetEvent waitUntil_Start;

        //    public string gameOverMessage = null;
        //    public ManualResetEvent waitUntil_End;

        //    PlayerConnection pc;
        //    bool expectingScore;

        //    public LinearClient(int port)
        //    {
        //        TcpClient client = new TcpClient("localhost", port);

        //        p1Received = new Stack<string>();
        //        waitUntil_validMessage = new ManualResetEvent(false);

        //        startMessage = null;
        //        waitUntil_Start = new ManualResetEvent(false);

        //        gameOverMessage = null;
        //        waitUntil_End = new ManualResetEvent(false);

        //        pc = new PlayerConnection(
        //            client.Client,
        //            null,
        //            received);

        //        expectingScore = false;
        //    }

        //    public void send(string message)
        //    {
        //        pc.SendMessage(message + "\r\n");
        //    }

        //    public void play(String name)
        //    {
        //        send("play " + name);
        //    }

        //    public string guess(string word)
        //    {
        //        expectingScore = true;
        //        send("word " + word);
        //        waitUntil_validMessage.WaitOne();
        //        waitUntil_validMessage.Reset();
        //        expectingScore = false;
        //        return p1Received.Pop();
        //    }

        //    private void received(MessageReceivedFrom m)
        //    {
        //        FilterMessages(m.message, usefulM);
        //    }

        //    private void usefulM(string s)
        //    {
        //        p1Received.Push(s);
        //        waitUntil_validMessage.Set();
        //    }


        //    private void FilterMessages(String Message, Action<String> usefulMessage)
        //    {
        //        if (!Message.ToUpper().StartsWith("TIME "))
        //        {
        //            if (Message.ToUpper().StartsWith("START "))
        //            {
        //                startMessage = Message;
        //                waitUntil_Start.Set();
        //            }
        //            else if (Message.ToUpper().StartsWith("STOP "))
        //            {
        //                gameOverMessage = Message;
        //                waitUntil_End.Set();
        //            }
        //            else
        //                if (expectingScore)
        //                    usefulMessage(Message);
        //        }
        //    }


        //}
        

    }
}
