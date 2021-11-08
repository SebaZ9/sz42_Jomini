

using System.Threading;
using System.Collections.Generic;
using System.Diagnostics;
using System;
using ProtoMessageClient;

namespace JominiAI
{
    /// <summary>
    ///     These are the possible directions to travel to an adjacent fief
    /// </summary>
    public enum MoveDirections
    {
        E, W, SE, SW, NE, NW
    }

    /// <summary>
    /// This class gives all the necessary functions to communicate with the server.
    /// Though it is quite similar to the class I imported it from, I did modify/add/correct a lot of things
    /// and I only kept what was needed (I removed all the unity part).
    /// </summary>
    public class Player
    {
        private TextTestClient tclient;
        protected string currentPCid = null; // Contains the current PlayerCharacter ID of the player
        public List<double> waitingForServerTimes = new List<double>();

        /// <summary>
        ///    Log in with an existing account. The available accounts are:
        ///    - "helen" => "potato"
        ///    - "test" => "tomato"
        ///    For all the PCs not already linked to an account, if the paramater 'createAccountForAllPCs' is true in the initialise() function on the server side, 
        ///    then accounts are automatically created with the following format: 
        ///    username: charID + "_username"
        ///    password: charID + "_password"
        /// </summary>
        public Player(string username, string password)
        {
            tclient = new TextTestClient(); // Create a new client
            Login(username, password); //Log in with an existing account
        }

        private void Login(string username, string password)
        {
            //tclient.LogInAndConnect(username, password, "localhost");
            tclient.LogInAndConnect(username, password, "192.168.0.16");
            while (!tclient.IsConnectedAndLoggedIn())
            {
                Thread.Sleep(0);
            }
            if (tclient.IsConnectedAndLoggedIn())
            {
                Console.WriteLine($"\n\nSuccessfully Logged In: {tclient.playerID}\n\n");
                updateCurrentPCid();
            }
        }

        protected void updateCurrentPCid()
        {
            foreach (ProtoPlayer player in ((ProtoGenericArray<ProtoPlayer>)GetPlayers()).fields) 
            {

                if (player.playerID.Equals(tclient.playerID))
                {
                    currentPCid = player.pcID; // Update currentPCid
                    break;
                }
            }
        }

        private ProtoMessage GetActionReply(Actions action)
        {
            ProtoMessage responseTask = tclient.GetReply();
            while (responseTask.ActionType != action)
            {
                responseTask = tclient.GetReply();
            }
            tclient.ClearMessageQueues();
            return responseTask;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Returns a ProtoGenericArray<ProtoPlayer> if succeeds</returns>
        public ProtoMessage GetPlayers()
        {
            return SendRequestToServer(Actions.GetPlayers, currentPCid, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Returns a ProtoGenericArray<ProtoSiegeOverview> if succeeds</returns>
        public ProtoMessage SiegeList()
        {
            return SendRequestToServer(Actions.SiegeList, currentPCid, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Returns a ProtoPlayerCharacter if succeeds</returns>
        public ProtoMessage viewCurrentPC()
        {
            return SendRequestToServer(Actions.ViewChar, currentPCid, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Returns a ProtoGenericArray<ProtoCharacterOverview> if succeeds</returns>
        public ProtoMessage GetNPCList(String typeOfNpc)
        {
            return SendRequestToServer(Actions.GetNPCList, typeOfNpc, null);
        }
        /// <summary>
        ///     returns details about the current fief
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public ProtoMessage viewCurrentFief()
        {
            ProtoPlayerCharacter currentPC = (ProtoPlayerCharacter)ViewCharacter(currentPCid);
            return ViewFief(currentPC.location);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Returns a ProtoGenericArray<ProtoFief> if succeeds</returns>
        public ProtoMessage ViewMyFiefs()
        {
            return SendRequestToServer(Actions.ViewMyFiefs, null, null);
        }

        /// <summary>
        /// army function
        /// </summary>
        /// <returns>Returns a ProtoGenericArray<ProtoArmyOverview> if succeeds</returns>
        public ProtoMessage ListArmies()
        {
            return SendRequestToServer(Actions.ListArmies, null, null);
        }

        public ProtoMessage MaintainArmy(string armyID)
        {
            return SendRequestToServer(Actions.MaintainArmy, armyID, null);
        }
        public ProtoMessage AppointLeader(string armyID, string charID)
        {
            return SendRequestToServer(Actions.AppointLeader, armyID, new string[] { charID });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Returns a ProtoGenericArray<ProtoDetachment> if succeeds</returns>
        public ProtoMessage ListDetachments(String armyID)
        {
            return SendRequestToServer(Actions.ListDetachments, armyID, null);
        }
        public ProtoMessage DropOffTroops(string armyID, uint[] Troops, String leftForCharID)
        {
            ProtoDetachment protoDetachment = new ProtoDetachment();
            protoDetachment.armyID = armyID;
            protoDetachment.troops = Troops;
            protoDetachment.leftFor = leftForCharID;
            return SendRequestToServer(Actions.DropOffTroops, null, null, protoDetachment);
        }
        public ProtoMessage PickUpTroops(string armyID, string[] detachmentIDs)
        {
            return SendRequestToServer(Actions.PickUpTroops, armyID, detachmentIDs);
        }
        public ProtoMessage RecruitTroops(string armyID, int amount)
        {
            ProtoRecruit protoRecruit = new ProtoRecruit();
            if (amount > 0)
            {
                protoRecruit.amount = (uint)amount;
            }
            protoRecruit.armyID = armyID;
            protoRecruit.isConfirm = true;
            return SendRequestToServer(Actions.RecruitTroops, null, null, protoRecruit);
        }

        public ProtoMessage PillageFief(string armyID)
        {
            return SendRequestToServer(Actions.PillageFief, armyID, null);
        }
        public ProtoMessage DisbandArmy(string armyID)
        {
            return SendRequestToServer(Actions.DisbandArmy, armyID, null);
        }
        /// <summary>
        /// family functions
        /// </summary>
        /// <param name="brideID"></param>
        /// <returns></returns>
        public ProtoMessage Marry(string groomID, string brideID)
        {
            return SendRequestToServer(Actions.ProposeMarriage, groomID, new string[] { brideID });
        }

        public ProtoMessage AcceptRejectProposal(bool AcceptOrReject)
        {
            return SendRequestToServer(Actions.AcceptRejectProposal, currentPCid, new string[] { Convert.ToString(AcceptOrReject) });
        }

        public ProtoMessage TryForChild()
        {
            return SendRequestToServer(Actions.TryForChild, currentPCid, null);
        }

        /// <summary>
        /// siege functions
        /// </summary>
        /// <returns></returns>
        public ProtoMessage BesiegeCurrentFief(string armyID)
        {
            return SendRequestToServer(Actions.BesiegeFief, armyID, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Returns a ProtoSiegeDisplay if succeeds</returns>
        public ProtoMessage ViewSiege(string siegeID)
        {
            return SendRequestToServer(Actions.ViewSiege, siegeID, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Returns a ProtoSiegeDisplay if succeeds</returns>
        public ProtoMessage SiegeRoundStorm(string siegeID)
        {
            return SendRequestToServer(Actions.SiegeRoundStorm, siegeID, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Returns a ProtoSiegeDisplay if succeeds</returns>
        public ProtoMessage SiegeRoundReduction(string siegeID)
        {
            return SendRequestToServer(Actions.SiegeRoundReduction, siegeID, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Returns a ProtoSiegeDisplay if succeeds</returns>
        public ProtoMessage SiegeRoundNegotiate(string siegeID)
        {
            return SendRequestToServer(Actions.SiegeRoundNegotiate, siegeID, null);
        }
        public ProtoMessage EndSiege(string siegeID)
        {
            return SendRequestToServer(Actions.EndSiege, siegeID, null);
        }
        /// <summary>
        /// spy funcations
        /// </summary>
        /// <param name="fiefID"></param>
        /// <returns></returns>
        public ProtoMessage SpyFief(string fiefID, string charID)
        {
            return SendRequestToServer(Actions.SpyFief, charID, new string[] { fiefID });
        }
        public ProtoMessage SpyCharacter(string targetID, string charID)
        {
            return SendRequestToServer(Actions.SpyCharacter, charID, new string[] { targetID });
        }

        public ProtoMessage SpyArmy(string armyID, string charID)
        {
            return SendRequestToServer(Actions.SpyArmy, charID, new string[] { armyID });
        }
        /// <summary>
        ///  
        /// </summary>
        /// <param name="scope">"year" || "season" || "unread" => By default, returns events from all years and seasons</param>
        /// <returns></returns>
        protected ProtoGenericArray<ProtoJournalEntry> ViewJournalEntries(string scope = null)
        {
            return (ProtoGenericArray<ProtoJournalEntry>)SendRequestToServer(Actions.ViewJournalEntries, currentPCid, new string[] { scope });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Returns a ProtoJournalEntry if succeeds</returns>
        protected ProtoMessage ViewJournalEntry(uint journalID)
        {
            return SendRequestToServer(Actions.ViewJournalEntry, currentPCid, new string[] { Convert.ToString(journalID) });
        }
        /// <summary>
        /// kidnapping functions
        /// </summary>
        /// <param name="targetID"></param>
        /// <param name="kidnapperID"></param>
        /// <returns></returns>
        public ProtoMessage Kidnap(string targetID, string kidnapperID)
        {
            return SendRequestToServer(Actions.Kidnap, targetID, new string[] { kidnapperID });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Returns a ProtoGenericArray<ProtoCharacterOverview> if succeeds</returns>
        public ProtoMessage ViewCaptives(string captiveLocation)
        {
            return SendRequestToServer(Actions.ViewCaptives, captiveLocation, null);
        }
        public ProtoMessage ViewCaptive(string charID)
        {
            return SendRequestToServer(Actions.ViewCaptive, charID, null);
        }
        public ProtoMessage RansomCaptive(string charID)
        {
            return SendRequestToServer(Actions.RansomCaptive, charID, null);
        }
        public ProtoMessage ReleaseCaptive(string charID)
        {
            return SendRequestToServer(Actions.ReleaseCaptive, charID, null);
        }
        public ProtoMessage ExecuteCaptive(string charID)
        {
            return SendRequestToServer(Actions.ExecuteCaptive, charID, null);
        }
        public ProtoMessage RespondRansom(uint jEntryID, bool pay)
        {
            return SendRequestToServer(Actions.ExecuteCaptive, Convert.ToString(jEntryID), new string[] { Convert.ToString(pay) });
        }
        protected ProtoMessage UseChar()
        {
            return SendRequestToServer(Actions.UseChar, currentPCid, null);
        }
        /// <summary>
        /// NPC functions
        /// </summary>
        /// <param name="NPCID"></param>
        /// <returns></returns>
        public ProtoMessage hireNPC(string NPCID, uint bid)
        {
            return SendRequestToServer(Actions.HireNPC, NPCID, new String[] { bid.ToString() });
        }
        public ProtoMessage fireNPC(string NPCID)
        {
            return SendRequestToServer(Actions.FireNPC, NPCID, null);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fiefID"></param>
        /// <returns></returns>
        public ProtoMessage AppointBailiff(string fiefID, string charID)
        {
            return SendRequestToServer(Actions.AppointBailiff, fiefID, new string[] { charID });
        }
        public ProtoMessage RemoveBailiff(string fiefID)
        {
            return SendRequestToServer(Actions.RemoveBailiff, fiefID, null);
        }
        protected ProtoMessage BarCharacters(string fiefID, string[] charIDs)
        {
            return SendRequestToServer(Actions.BarCharacters, fiefID, charIDs);
        }
        protected ProtoMessage UnbarCharacters(string fiefID, string[] charIDs)
        {
            return SendRequestToServer(Actions.UnbarCharacters, fiefID, charIDs);
        }
        protected ProtoMessage BarNationalities(string fiefID, string[] netIDs)
        {
            return SendRequestToServer(Actions.BarNationalities, fiefID, netIDs);
        }
        protected ProtoMessage UnbarNationalities(string fiefID, string[] netIDs)
        {
            return SendRequestToServer(Actions.UnbarNationalities, fiefID, netIDs);
        }
        public ProtoMessage GrantFiefTitle(string fiefID, string charID)
        {
            return SendRequestToServer(Actions.GrantFiefTitle, fiefID, new string[] { charID });
        }
        public ProtoMessage AdjustExpenditure(string fiefID, double[] adjustedValues)
        {
            ProtoGenericArray<double> newExpenses = new ProtoGenericArray<double>();
            newExpenses.Message = fiefID;
            newExpenses.fields = adjustedValues;
            return SendRequestToServer(Actions.AdjustExpenditure, null, null, newExpenses);
        }
        protected ProtoMessage AutoAdjustExpenditure(string fiefID)
        {
            return SendRequestToServer(Actions.AdjustExpenditure, fiefID, null);
        }
        public ProtoMessage TransferFunds(string fiefFromID, string fiefToID, int amount)
        {
            ProtoTransfer ProtoTransfer = new ProtoTransfer();
            ProtoTransfer.fiefFrom = fiefFromID;
            ProtoTransfer.fiefTo = fiefToID;
            ProtoTransfer.amount = amount;
            return SendRequestToServer(Actions.TransferFunds, null, null, ProtoTransfer);
        }
        public ProtoMessage TransferFundsToPlayer(string playerTo, int amount)
        {
            ProtoTransferPlayer ProtoTransferPlayer = new ProtoTransferPlayer();
            ProtoTransferPlayer.playerTo = playerTo;
            ProtoTransferPlayer.amount = amount;
            return SendRequestToServer(Actions.TransferFundsToPlayer, null, null, ProtoTransferPlayer);
        }
        protected ProtoMessage AdjustCombatValues(string armyID, byte aggression, byte odds)
        {
            ProtoCombatValues ProtoCombatValues = new ProtoCombatValues();
            ProtoCombatValues.armyID = armyID;
            ProtoCombatValues.aggression = aggression;
            ProtoCombatValues.odds = odds;
            return SendRequestToServer(Actions.AdjustCombatValues, null, null, ProtoCombatValues);
        }
        public ProtoMessage Attack(string armyID, string targetID)
        {
            return SendRequestToServer(Actions.Attack, armyID, new string[] { targetID });
        }
        public ProtoMessage EnterExitKeep(string charID)
        {
            return SendRequestToServer(Actions.EnterExitKeep, charID, null);
        }
        public ProtoMessage ListCharsInMeetingPlace(string placeType, string charID)
        {
            return SendRequestToServer(Actions.ListCharsInMeetingPlace, placeType, new string[] { "charID" });
        }
        public ProtoMessage Camp(string charID, byte days)
        {
            return SendRequestToServer(Actions.Camp, charID, new string[] { days.ToString() });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Returns a ProtoGenericArray<ProtoCharacterOverview> if succeeds</returns>
        public ProtoMessage AddRemoveEntourage(string charID)
        {
            return SendRequestToServer(Actions.AddRemoveEntourage, charID, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Returns a ProtoFief if succeeds</returns>
        public ProtoMessage Move(MoveDirections directions)
        {
            ProtoTravelTo protoTravel = new ProtoTravelTo();
            protoTravel.travelVia = new[] { directions.ToString() };
            protoTravel.characterID = currentPCid;
            return SendRequestToServer(Actions.TravelTo, null, null, protoTravel);
        }

        /// <summary>
        ///     Contrarely to the previous function this one can be used to directly travel to a fief, even if
        ///     it's not an adjacent one.
        /// </summary>
        /// <param name="fiefID">Fief to travel to</param>
        /// <returns>Returns a ProtoFief if succeeds</returns>
        public ProtoMessage MoveToFief(String fiefID)
        {
            ProtoTravelTo protoTravel = new ProtoTravelTo();
            protoTravel.travelTo = fiefID;
            protoTravel.characterID = currentPCid;
            return SendRequestToServer(Actions.TravelTo, null, null, protoTravel);
        }

        /*public void initFiefID()
        {
            mf = viewCurrentFief();
        }*/

        /// <summary>
        ///     Request the details about a particular army
        /// </summary>
        /// <param name="armyID">Army to get details from</param>
        /// <returns>Returns a ProtoArmy if succeeds</returns>
        public ProtoMessage ViewArmy(string armyID)
        {
            return SendRequestToServer(Actions.ViewArmy, armyID, null);
        }

        /// <summary>
        ///     Request the details about a particular character
        /// </summary>
        /// /// <param name="charID">Character to get details from</param>
        /// <returns>Returns a ProtoPlayerCharacter or a ProtoNPC if succeeds</returns>
        public ProtoMessage ViewCharacter(string charID)
        {
            ProtoMessage msg = SendRequestToServer(Actions.ViewChar, charID, null);
            Console.WriteLine($"Action type: {msg.ActionType}.Message: {msg.Message}.Fields {msg.MessageFields}. Response Type: {msg.ResponseType}");
            return msg;
        }

        /// <summary>
        ///     Get details from a fief
        /// </summary>
        /// <param name="fiefId">Id of the fief to get details from</param>
        /// <returns>Returns a ProtoFief if succeeds</returns>
        public ProtoMessage ViewFief(string fiefId)
        {
            return SendRequestToServer(Actions.ViewFief, fiefId, null);
        }

        /// <summary>
        ///     
        /// </summary>
        /// <param name="fiefId"></param>
        /// <returns></returns>
        public ProtoMessage getTravelDayCost(string fiefId, string direction = null)
        {
            if(direction == null)
                return SendRequestToServer(Actions.GetTravelCost, currentPCid, new string[] { fiefId });
            else
                return SendRequestToServer(Actions.GetTravelCost, currentPCid, new string[] { fiefId, direction });
            /*if (reply.ResponseType != DisplayMessages.Success)
                throw new Exception("Couldn't get travel cost");*/
        }

        /// <summary>
        ///     
        /// </summary>
        /// <returns></returns>
        public ProtoMessage GetAvailableTravelDirections()
        {
            return SendRequestToServer(Actions.GetAvailableTravelDirections, currentPCid, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Returns a ProtoCharacter if succeeds</returns>
        public ProtoMessage AppointHeir(string heirID)
        {
            return SendRequestToServer(Actions.AppointHeir, heirID, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Returns a ProtoGenericArray<ProtoArmyOverview> if succeeds</returns>
        protected ProtoMessage ExamineArmiesInFief(string fiefID)
        {
            return SendRequestToServer(Actions.ExamineArmiesInFief, fiefID, null);
        }

        /// <summary>
        ///     Send a request to the server and wait for its reply.
        ///     The request is built with the parameters.
        ///     Will update the current PlayerCharacter ID after receiving the answer from the server
        /// </summary>
        /// <param name="actionType">Type of action requesting by the client to the server</param>
        /// <param name="message">paramater associated with the actionType</param>
        /// <param name="messageFields">paramaters associated with the actionType</param>
        /// <param name="pProtoMessage">If not null, takes precedence to all the other parameters and send directly this protoMessage to the server</param>
        /// <returns>The ProtoMessage received from the server</returns>
        private ProtoMessage SendRequestToServer(Actions actionType, string message, string[] messageFields, ProtoMessage pProtoMessage = null)
        {
            ProtoMessage protoMessageToSend;
            if (pProtoMessage == null)
            {
                protoMessageToSend = new ProtoMessage();
                protoMessageToSend.Message = message;
                protoMessageToSend.MessageFields = messageFields;
            }
            else
                protoMessageToSend = pProtoMessage;
            protoMessageToSend.ActionType = actionType;
            tclient.net.Send(protoMessageToSend);

            Stopwatch timer = new Stopwatch();
            timer.Start();
            ProtoMessage reply = GetActionReply(actionType);
            timer.Stop();
            waitingForServerTimes.Add(timer.Elapsed.TotalSeconds);

            if(actionType != Actions.GetPlayers) // To avoid getting in an infinite loop
                updateCurrentPCid();
            return reply;
        }

        /// <summary>
        ///     !!! ONLY FOR TESTING PURPOSE !!!
        ///     DOESN'T WORK ON THE SERVER SIDE
        ///     Request to end the current season and pass to the next one. 
        ///     This function will probably only be used for testing purpose as later it will probably be done automatically on the server side.
        /// </summary>
        /// <returns></returns>
        protected ProtoMessage SeasonUpdate()
        {
            return SendRequestToServer(Actions.SeasonUpdate, null, null);
        }

        /// <summary>
        ///     !!! ONLY FOR TESTING PURPOSE !!!
        ///     Asks the server to load a preconfigured scenario
        ///     Contrarely to all the others functions of this class, for this one the server needs to be started before running it as the client won't wait for the server to send an answer
        /// </summary>
        public void LoadScenario(string scenarioID)
        {
            Thread.Sleep(500); // The server often crash if there's no break.
            ProtoMessage protoMessageToSend = new ProtoMessage();
            protoMessageToSend.Message = scenarioID;
            protoMessageToSend.ActionType = Actions.LoadScenario;
            tclient.net.Send(protoMessageToSend);
        }
    }
}
