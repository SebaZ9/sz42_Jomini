#define TESTSUITE
using System;
using System.Collections.Generic;
using System.Threading;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace JominiEngine
{

    static class Program
    {

        /// <summary>
        /// The Game object for this test (contains and handles all game data)
        /// </summary>
        public static Game game;
        /// <summary>
        /// The Server object used for this test (contains connected client information
        /// </summary>
        public static Server server;
        /// <summary>
        /// The dummy Client to be used for this test
        /// </summary>
        public static TestClient client;
        /// <summary>
        /// The location of the log file
        /// </summary>
        public static string logFilePath;
        /// <summary>
        /// Store the max memory consumption
        /// </summary>
        public static long maxMemoryUseage;

        /******Objects for use during testing **/
        public static Army OwnedArmy;
        public static Army NotOwnedArmy;
        public static Fief OwnedFief;
        public static Fief NotOwnedFief;
        public static string Username;
        public static string Pass;
        public static PlayerCharacter MyPlayerCharacter;
        public static PlayerCharacter NotMyPlayerCharacter;
         
        /// <summary>
        /// Set up the data and game state for the test run
        /// </summary>
        public static void InitialiseGameState()
        {
            game = new Game();
            server = new Server();
        }

        /// <summary>
        /// Clean up the server and close the log file
        /// </summary>
        public static void FinaliseGameState()
        {

            client.LogOut();
            server.Shutdown();
#if DEBUG
            Console.WriteLine("A log file was written to "+logFilePath);
#endif
        }

        /// <summary>
        /// Set up the game, run the TestRun (both for encrypted and unencrypted messages) and end
        /// </summary>
        public static void Main()
        {
            var encryptString = "_encrypted_";
            string datePatern = "MM_dd_H_mm";
            logFilePath = "TestRun_NoSessions"+encryptString + DateTime.Now.ToString(datePatern) + ".txt";
            Console.WriteLine("Program launch");
            Globals_Server.LogFile = new System.IO.StreamWriter(logFilePath); 
            InitialiseGameState();
            SetUpForDemo();
            //TestRun(true);                
        }
        
        /// <summary>
        /// Run through a sequence of actions, recording the time taken and memory consumption
        /// </summary>
        /// <param name="encrypt">Whether or not to use encryption</param>
        [STAThread]
        public static void TestRun(bool encrypt = true)
        {
            Process currentProcess = Process.GetCurrentProcess();
            if (encrypt)
            {
                Globals_Server.logEvent("Running test with encryption");
            }
            else
            {
                Globals_Server.logEvent("Running test without encryption");
            }
            double LoginTime, RecruitTime, MoveTime, SpyTime;
            double start = DateTime.Now.TimeOfDay.TotalMilliseconds;
            byte[] encryptionKey = null;
            if (encrypt)
            {
                encryptionKey = LogInManager.GetRandomSalt(32);
            }
            client.LogInAndConnect(Username, Pass, encryptionKey);
            while (!client.IsConnectedAndLoggedIn())
            {
                Thread.Sleep(0);
            }
            LoginTime = DateTime.Now.TimeOfDay.TotalMilliseconds - start;
            client.RecruitTroops(OwnedArmy.armyID, 70, true);
            RecruitTime = ProcessNextAction(Actions.RecruitTroops,currentProcess);
            // Move to another fief
            client.Move(MyPlayerCharacter.charID, NotOwnedFief.id, null);
            MoveTime = ProcessNextAction(Actions.TravelTo,currentProcess);
            // Spy
            client.SpyOnFief(MyPlayerCharacter.charID, MyPlayerCharacter.location.id);
            SpyTime = ProcessNextAction(Actions.SpyFief,currentProcess);
            // Confirm spy
            Globals_Server.logEvent("Time taken to run test (ms): " + (DateTime.Now.TimeOfDay.TotalMilliseconds - start));
            Globals_Server.logEvent("LogIn time: " + LoginTime);
            Globals_Server.logEvent("Recruit time: " + (RecruitTime));
            Globals_Server.logEvent("Travel time: " + MoveTime);
            Globals_Server.logEvent("Spy time: " + SpyTime);
            Globals_Server.logEvent("Max memory measured: " + maxMemoryUseage);
        }

        /// <summary>
        /// Logs the memory useage using GC.GetTotalMemory and returns the memory useage
        /// </summary>
        /// <param name="p">Process to use to calculate memory</param>
        public static void LogMemory(Process p)
        {
            long mem = +GC.GetTotalMemory(false);
            if (mem > maxMemoryUseage) maxMemoryUseage = mem;
            Globals_Server.logEvent("GC memory: " + mem);
        }

        /// <summary>
        /// Waits for the response to a client's action, gets the time taken to receive reply, and logs memory
        /// </summary>
        /// <param name="action">Action which was taken- will wait until a response with the same action has been received</param>
        /// <param name="p">Process used to get memory</param>
        /// <returns>Time taken (milliseconds)</returns>
        private static double ProcessNextAction(Actions action, Process p)
        {
            double start = DateTime.Now.TimeOfDay.TotalMilliseconds;
            Task<ProtoMessage> responseTask = client.GetReply();
            responseTask.Wait();
            while (responseTask.Result.ActionType != action)
            {
                responseTask = client.GetReply();
                responseTask.Wait();
            }
            double end = DateTime.Now.TimeOfDay.TotalMilliseconds;
            client.ClearMessageQueues();
            LogMemory(p);
            return end - start;
        }

        ///// <summary>
        ///// The main entry point for the application.
        ///// </summary>
        //static void Main()
        //{

        //    try
        //    {
        //        using (Globals_Server.LogFile = new System.IO.StreamWriter("LogFile.txt"))
        //        {

        //            //Globals_Server.rCluster = RiakCluster.FromConfig("riakConfig","app.config");
        //            //Globals_Server.rClient = Globals_Server.rCluster.CreateClient();
        //            Globals_Server.LogFile.AutoFlush = true;
        //            Globals_Server.logEvent("Server start");

        //            Game game = new Game();
        //            SetUpForDemo();
        //            /*
        //            //DatabaseWrite.DatabaseWriteAll ("testBucket");

        //            /*if (Globals_Server.rClient.Ping ().IsSuccess) {
        //                Console.WriteLine ("Database connection successful");
        //                string gameID = "testBucket";
        //                foreach (string trait in Globals_Game.traitKeys) {
        //                    Console.WriteLine (trait);
        //                }

        //                // Test can read from database
        //                var newClient = Globals_Server.rCluster.CreateClient();
        //                RiakObject newObj = new RiakObject (gameID, "superawesome3", Globals_Game.traitKeys.ToArray ());
        //                newClient.Put (newObj);
        //                Thread.Sleep (5000);
        //                var testRead =newClient.Get (gameID, "superawesome3");
        //                if (!testRead.IsSuccess) {
        //                    Console.WriteLine ("FAIL :(" + testRead.ErrorMessage);
        //                } else {
        //                    Console.WriteLine ("Got traitkeys:");
        //                }
        //                //DatabaseRead.DatabaseReadAll (gameID);
        //            } else {
        //                Console.WriteLine ("Could not connect to database :( ");
        //            } */

        //            //testCaptives();



        //            Server server = new Server();
        //            try
        //            {
        //                //TestSuite testSuite = new TestSuite();
        //                TestClient client = new TestClient();
        //                client.LogInAndConnect("helen", "potato");
        //            }
        //            catch (Exception e)
        //            {
        //                Console.WriteLine(e.Message);
        //            }
        //            //client.LogIn("helen", "potato");
        //            String s = Console.ReadLine();
        //            if (s != null && s.Equals("exit"))
        //            {
        //                Globals_Server.logEvent("Server exits");
        //                server.isListening = false;
        //                Globals_Server.server.Shutdown("Server exits");
        //            }

        //            //testArmy();
        //            //testSpying();
        //            /*
        //                    while (true)
        //                    {

        //                        if (s != null && s.Equals("exit"))
        //                        {
        //                            Globals_Server.logEvent("Server exits");
        //                            server.isListening = false;
        //                            Globals_Server.server.Shutdown("Server exits");
        //                            break;
        //                        }

        //                    }

        //                    * */
        //            Globals_Server.LogFile.Close();

        //        }

        //    }
        //    catch (Exception e)
        //    {
        //        Globals_Server.LogFile.Close();
        //        Console.WriteLine("Encountered an error:" + e.StackTrace);
        //        Console.ReadLine();
        //    }


        //}

        /// <summary>
        /// Code which was used in the 2015 demo- sets up a few armies, adds funds and sets a few traits to demonstrate trait effects
        /// </summary>
        public static void SetUpForDemo()
        {
            // Make Anselm Marshal very sneaky
            Character Anselm = Globals_Game.getCharFromID("Char_390");
            Character Bishop = Globals_Game.getCharFromID("Char_391");
            Tuple<Trait, int>[] newTraits = new Tuple<Trait, int>[2];
            newTraits[0] = new Tuple<Trait, int>(Globals_Game.traitMasterList["trait_9"], 9);
            newTraits[1] = new Tuple<Trait, int>(Globals_Game.traitMasterList["trait_8"], 9);
            Anselm.traits = newTraits;
            // Make Bishop Henry Marshal not sneaky
            Tuple<Trait, int>[] newTraits2 = new Tuple<Trait, int>[1];
            newTraits2[0] = new Tuple<Trait, int>(Globals_Game.traitMasterList["trait_5"], 2);
            Bishop.traits = newTraits2;
            // Add funds to home treasury
            (Globals_Game.getCharFromID("Char_158") as PlayerCharacter).GetHomeFief().AdjustTreasury(100000);

            // create enemy character in home fief
            NonPlayerCharacter enemyGeneral = new NonPlayerCharacter("Char_164459", "John", "Smith", new Tuple<uint, byte>(1142, 3), true, Globals_Game.nationalityMasterList["Sco"], true, 9, 9, new Queue<Fief>(), Globals_Game.languageMasterList["lang_C1"], 90, 9, 9, 9, new Tuple<Trait, int>[0], true, false, "Char_126", null, "Char_126", null, 0, false, false, new List<string>(), null, null, Globals_Game.fiefMasterList["EPM02"]);
            PlayerCharacter factionLeader = Globals_Game.pcMasterList["Char_126"];
            factionLeader.myNPCs.Add(enemyGeneral);
            enemyGeneral.inKeep = false;
            // create enemy army for above enemy character
            uint[] enemyArmyTroops = new uint[] { 3, 7, 0, 20, 30, 65, 190 };
            Army enemyArmy = new Army(Globals_Game.GetNextArmyID(), Globals_Game.npcMasterList["Char_164459"].charID, Globals_Game.pcMasterList["Char_196"].charID, Globals_Game.npcMasterList["Char_164459"].days, Globals_Game.npcMasterList["Char_164459"].location.id, trp: enemyArmyTroops);
            enemyArmy.AddArmy();

            // create and add army
            uint[] myArmyTroops1 = new uint[] { 8, 10, 0, 30, 60, 100, 220 };
            Army myArmy1 = new Army(Globals_Game.GetNextArmyID(), Globals_Game.pcMasterList["Char_196"].charID, Globals_Game.pcMasterList["Char_196"].charID, Globals_Game.pcMasterList["Char_196"].days, Globals_Game.pcMasterList["Char_196"].location.id, trp: myArmyTroops1);
            myArmy1.AddArmy();
            // create and add army
            uint[] myArmyTroops2 = new uint[] { 5, 10, 0, 30, 40, 80, 220 };
            Army myArmy2 = new Army(Globals_Game.GetNextArmyID(), Globals_Game.pcMasterList["Char_158"].charID, Globals_Game.pcMasterList["Char_158"].charID, Globals_Game.pcMasterList["Char_158"].days, Globals_Game.pcMasterList["Char_158"].location.id, trp: myArmyTroops2, aggr: 1, odds: 2);
            myArmy2.AddArmy();

            // Add single lady appropriate for marriage
            //Nationality nat = Globals_Game.nationalityMasterList["Sco"];
            NonPlayerCharacter proposalChar = new NonPlayerCharacter("Char_626", "Mairi", "Meah", new Tuple<uint, byte>(1162, 3), false, Globals_Game.nationalityMasterList["Sco"], true, 9, 9, new Queue<Fief>(), Globals_Game.languageMasterList["lang_C1"], 90, 9, 9, 9, new Tuple<Trait, int>[0], true, false, "Char_126", null, "Char_126", null, 0, false, false, new List<string>(), null, null, Globals_Game.fiefMasterList["ESW05"]);
            PlayerCharacter pc = Globals_Game.pcMasterList["Char_126"];
            pc.myNPCs.Add(proposalChar);
            proposalChar.inKeep = false;
        }

        /// <summary>
        ///     NEW FUNCTION
        ///     Initialise the account by giving the PlayerCharacter the default ressources he will start the game with
        /// </summary>
        /// <param name="username">Username of the account</param>
        private static void setUpPCofThisAccount(string username) {
            PlayerCharacter pc = Globals_Game.ownedPlayerCharacters[username];

            // Add funds to home treasury
            pc.GetHomeFief().AdjustTreasury(100000);

            // create and add army
            Army myArmy1 = new Army(Globals_Game.GetNextArmyID(), pc.charID, pc.charID, pc.days, pc.location.id, trp: new uint[] { 1000, 0, 0, 0, 0, 0, 0 });
            myArmy1.AddArmy();
        }

    }
}
