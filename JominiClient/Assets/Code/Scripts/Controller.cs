
using UnityEngine;
using UnityEngine.UI;
using Lidgren.Network;
using System.Net;
using UnityEngine.SceneManagement;
using ProtoMessageClient;
using System.Threading.Tasks;
using System.Threading;
using System;
using System.Collections.Generic;
using System.Collections;

public class Controller : MonoBehaviour { 

    ////Other value
    public static TextTestClient tclient;
    public static ProtoFief mf;
    public static ProtoGenericArray<ProtoArmyOverview> ma;
    public static ProtoGenericArray<ProtoDetachment> dl;
    public static ProtoPlayerCharacter c;
    public static ProtoMessage hr;
    public static ProtoMessage sd;
    public static ProtoMessage knp;
    public static ProtoMessage sm;
    public static ProtoSiegeDisplay sc;
    public static ProtoGenericArray<ProtoSiegeOverview> sl;

    public static string ipAddress = "192.168.1.154";

    public enum SceneName {
        FiefDetails, LogIn, MainMenu, Map, ViewArmy, ViewCharacter, ViewFief, ViewingList, ViewJournalEntry, ViewArmiesList, ViewMyFiefsList, ChangeCharactersList, ViewJournalEntries,
        AssignBailiff, ViewCharacters, ArmiesInFief, ViewSiege
    }
//
    public static ProtoClient protoClient;

    //public static ProtoFief currentlyViewedFief;
    public static string fiefToViewID;
    //public static ProtoCharacter currentlyViewedCharacter;
    public static string characterToViewID;
    //public static ProtoArmy currentlyViewedArmy;
    public static string armyToViewID;
    public static uint journalEntryToViewID;
    public static ProtoGenericArray<ProtoArmyOverview> armyList;
    public static ProtoGenericArray<ProtoCharacterOverview> characterList;
    public static ProtoGenericArray<ProtoFief> fiefList;
    public static ProtoGenericArray<ProtoJournalEntry> journalList;
    public static string viewingListSelectAction;
    public static string globalString;
    public static int globalInt;
    public static string userMessageOnSceneLoad;

    public static Dictionary<string, string> fiefNames = new Dictionary<string, string>();
    public static Dictionary<string, string> fiefOwners = new Dictionary<string, string>();
    public static Dictionary<string, Color> ownerColours = new Dictionary<string, Color>();
    public static Dictionary<string, string> charIdNames = new Dictionary<string, string>();
    public static List<Color> colours = new List<Color>();


    private static float TimeOutLimit = 5000f;

    public static bool EventUpdateSeason = false;
    public static ProtoGameEvent gameEvent;

    public void Update()
    {
        if (EventUpdateSeason)
        {
            GameObject UI = GameObject.Find("NavigationUI");
            if (UI != null)
            {
                UI.transform.Find("EventBox").gameObject.SetActive(true);
                UI.transform.Find("EventBox").transform.GetChild(1).GetComponent<Text>().text = "The season has been updated!.\n Next season update will be on: " + gameEvent.nextUpdate.ToString();
            }
            EventUpdateSeason = false;
        }
    }

    public static void Initialise()
    {
        tclient = new TextTestClient();
        GenerateColourList();
        string reas;
        //Login("helen", "potato", out reas);
    }

    public static void GoToScene(SceneName sceneName) {
        SceneManager.LoadScene(sceneName.ToString());
    }

    public static bool Login(string username, string password, out string reason)
    {
        DateTime current = DateTime.Now;
        tclient.LogInAndConnect(username, password, ipAddress);
        while (!tclient.IsConnectedAndLoggedIn())
        {
            Thread.Sleep(0);
            if ((DateTime.Now - current).TotalMilliseconds > TimeOutLimit) break;
        }
        
        if (tclient.IsConnectedAndLoggedIn())
        {
            //mf = (ProtoFief)GetFiefDetails("Char_158", tclient);
            GoToScene(SceneName.Map);
        }
        reason = "test";
        return true;
    }

    public static string GetCharName(string charID) {
        if(charIdNames.ContainsKey(charID)) {
            return charIdNames[charID];
        } else {
            ProtoCharacter character = GetCharacterDetails(charID, tclient);
            charIdNames[charID] = character.firstName + " " + character.familyName;
            return charIdNames[charID];
        }
    }

    public static IEnumerator SleepFunction(float t) {
        yield return new WaitForSeconds(t);
    }

    public static ProtoMessage GetActionReply(Actions action, TextTestClient client)
    {
        bool receivedActionReply = false;
        bool receivedUpdateReply = false;
        ProtoMessage reply;
        ProtoMessage actionReply = null;

        if(action == Actions.LogIn) {
            receivedUpdateReply = true;
        }

        do {
            reply = client.CheckForProtobufMessage();
            if(reply == null) { // wait time expired.
                globalString = "You have been disconnected.";
                tclient.LogOut();
                GoToScene(SceneName.LogIn);
                return null;
            }

            if(reply.ActionType == action) {
                actionReply = reply;
                receivedActionReply = true;
            }
            else if(reply.ActionType == Actions.Update) {
                if(reply.ResponseType == DisplayMessages.ErrorGenericMessageInvalid) {
                    actionReply = reply; // When the server found something wrong with the action.
                    receivedActionReply = true;
                }
                else if(reply.ResponseType == DisplayMessages.Success) {
                    protoClient = (ProtoClient)reply;
                    receivedUpdateReply = true;
                }
            }
            else {
                Debug.Log("Mismatching action found: " + reply.ActionType.ToString() + " /w " + reply.ResponseType.ToString());
            }

        } while(!receivedActionReply || !receivedUpdateReply);


        return actionReply;
    }

    public static string SeasonToString(byte seasonCode) {
        string season = "Unknown";
        switch(seasonCode) {
            case 0: season = "Spring"; break;
            case 1: season = "Summer"; break;
            case 2: season = "Autumn"; break;
            case 3: season = "Winter"; break;
        }
        return season;
    }

    public static ProtoMessage AddRemoveEntourage(string charID, TextTestClient client)
    {
        ProtoMessage request = new ProtoMessage {
            ActionType = Actions.AddRemoveEntourage,
            Message = charID
        };
        client.net.Send(request);
        return GetActionReply(Actions.AddRemoveEntourage, client);
    }

    public static ProtoMessage AdjustExpenditure(string fiefID, double[] adjustedValues, TextTestClient client)
    {
        ProtoGenericArray<double> request = new ProtoGenericArray<double> {
            ActionType = Actions.AdjustExpenditure,
            Message = fiefID,
            fields = adjustedValues
        };
        client.net.Send(request);
        return GetActionReply(Actions.AdjustExpenditure, client);
    }
    
    public static ProtoMessage AppointBailiff(string charID, string fiefID, TextTestClient client)
    {
        ProtoMessage request = new ProtoMessage {
            ActionType = Actions.AppointBailiff,
            Message = fiefID,
            MessageFields = new string[] { charID }
        };
        Debug.Log("Try assign bailiff with character: " + request.MessageFields[0]);
        client.net.Send(request);
        return GetActionReply(Actions.AppointBailiff, client);
    }

    public static ProtoMessage Attack(string armyID, string defenderID, TextTestClient client) {
        ProtoMessage request = new ProtoMessage {
            ActionType = Actions.Attack,
            Message = armyID,
            MessageFields = new string[] { defenderID }
        };
        client.net.Send(request);
        return GetActionReply(Actions.Attack, client);
    }
    
    public static ProtoMessage AutoAdjustExpenditure(string fiefID, TextTestClient client)
    {
        ProtoGenericArray<double> request = new ProtoGenericArray<double> {
            Message = fiefID,
            ActionType = Actions.AdjustExpenditure
        };
        client.net.Send(request);
        return GetActionReply(Actions.AdjustExpenditure, client);
    }

    public static ProtoMessage ExamineArmiesInFief(string fiefID, TextTestClient client) {
        ProtoMessage request = new ProtoMessage {
            ActionType = Actions.ExamineArmiesInFief,
            Message = fiefID
        };
        client.net.Send(request);
        return GetActionReply(Actions.ExamineArmiesInFief, client);
    }

    public static ProtoMessage FireNPC(string npcID, TextTestClient client)
    {
        ProtoMessage request = new ProtoMessage {
            ActionType = Actions.FireNPC,
            Message = npcID
        };
        client.net.Send(request);
        return GetActionReply(Actions.FireNPC, client);
    }

    public static ProtoMessage GetArmyDetails(string armyID, TextTestClient client)
    {
        ProtoArmy request = new ProtoArmy {
            Message = armyID,
            ActionType = Actions.ViewArmy
        };
        client.net.Send(request);
        return GetActionReply(Actions.ViewArmy, client);
    }

    public static ProtoCharacter GetCharacterDetails(string charID, TextTestClient client)
    {
        ProtoCharacter request = new ProtoCharacter {
            Message = charID,
            ActionType = Actions.ViewChar
        };
        client.net.Send(request);
        return (ProtoCharacter)GetActionReply(Actions.ViewChar, client);
    }

    public static ProtoMessage GetFiefDetails(string fiefID, TextTestClient client)
    {
        ProtoFief request = new ProtoFief {
            Message = fiefID,
            ActionType = Actions.ViewFief
        };
        client.net.Send(request);
        return GetActionReply(Actions.ViewFief, client);
    }

    public static ProtoGenericArray<ProtoCharacterOverview> GetNPCList(string categories, TextTestClient client)
    {
        ProtoMessage request = new ProtoMessage {
            Message = categories,
            ActionType = Actions.GetNPCList
        };
        client.net.Send(request);
        return (ProtoGenericArray<ProtoCharacterOverview>)GetActionReply(Actions.GetNPCList, client);
    }

    public static ProtoMessage HireNPC(string npcID, string bid, TextTestClient client)
    {
        ProtoMessage request = new ProtoMessage {
            ActionType = Actions.HireNPC,
            Message = npcID,
            MessageFields = new string[] { bid }
        };
        client.net.Send(request);
        return GetActionReply(Actions.HireNPC, client);
    }

    // This method only lists the requesters OWN armies.
    public static ProtoMessage ListArmies(TextTestClient client)
    {
        ProtoMessage request = new ProtoMessage {
            //ownerID = "Char_158",
            ActionType = Actions.ListArmies
        };
        client.net.Send(request);
        return GetActionReply(Actions.ListArmies, client);
    }

    public ProtoMessage SeasonUpdate(TextTestClient client) {
        ProtoMessage request = new ProtoMessage {
            ActionType = Actions.SeasonUpdate
        };
        client.net.Send(request);
        return GetActionReply(Actions.SeasonUpdate, client);
    }

    public static ProtoMessage TravelTo(string charID, string destinationFiefID, TextTestClient client) 
    {
        ProtoMessage request = new ProtoTravelTo {
            ActionType = Actions.TravelTo,
            characterID = charID,
            travelTo = destinationFiefID,
            travelVia = null
        };
        client.net.Send(request);
        return GetActionReply(Actions.TravelTo, client);
    }

    public static ProtoMessage UseChar(string charID, TextTestClient client)
    {
        ProtoMessage protoMessage = new ProtoMessage {
            ActionType = Actions.UseChar,
            Message = charID
        };
        client.net.Send(protoMessage);
        return GetActionReply(Actions.UseChar, client);
    }

    public static ProtoMessage ViewJournalEntries(string scope, TextTestClient client)
    {
        ProtoMessage request = new ProtoMessage {
            ActionType = Actions.ViewJournalEntries,
            Message = scope
        };
        client.net.Send(request);
        return GetActionReply(Actions.ViewJournalEntries, client); // ProtoGenericArray<ProtoJournalEntry>
    }

    public static ProtoGenericArray<ProtoFief> ViewMyFiefs(TextTestClient client)
    {
        ProtoMessage request = new ProtoMessage {
            ActionType = Actions.ViewMyFiefs
        };
        client.net.Send(request);
        return (ProtoGenericArray<ProtoFief>)GetActionReply(Actions.ViewMyFiefs, tclient);
    }

    public static ProtoMessage RemoveBailiff(string fiefID, TextTestClient client)
    {
        ProtoMessage request = new ProtoMessage {
            ActionType = Actions.RemoveBailiff,
            Message = fiefID
        };
        client.net.Send(request);
        return GetActionReply(Actions.RemoveBailiff, client);
    }


    public static ProtoMessage TransferFunds(string fiefFromID, string fiefToID, int amount, TextTestClient client)
    {
        ProtoTransfer request = new ProtoTransfer{
            fiefFrom = fiefFromID,
            fiefTo = fiefToID,
            amount = amount,
            ActionType = Actions.TransferFunds
        };
        client.net.Send(request);
        return GetActionReply(Actions.TransferFunds, client);
    }

    public static ProtoMessage ListCharsInMeetingPlace(string placeType, string charID, TextTestClient client) {
        ProtoMessage message = new ProtoMessage {
            ActionType = Actions.ListCharsInMeetingPlace,
            MessageFields = new string[] { charID },
            Message = placeType
        };
        client.net.Send(message);
        return GetActionReply(Actions.ListCharsInMeetingPlace, client);
    }

    public static ProtoMessage GetWorldMap(TextTestClient client) {
        ProtoMessage request = new ProtoMessage {
            ActionType = Actions.ViewWorldMap
        };
        client.net.Send(request);
        var response = GetActionReply(Actions.ViewWorldMap, client);
        return response;
    }

    // #####################################################
    // TODO: Methods
    // #####################################################

    public static ProtoMessage GetPlayers(TextTestClient client)
    {
        ProtoMessage protoMessage = new ProtoMessage();
        protoMessage.Message = "Char_158";
        protoMessage.ActionType = Actions.GetPlayers;
        client.net.Send(protoMessage);
        ProtoMessage reply = GetActionReply(Actions.GetPlayers, client);
        return reply;
    }
    public static ProtoGenericArray<ProtoSiegeOverview> SiegeList(TextTestClient client)
    {
        ProtoMessage protoMessage = new ProtoMessage();
        protoMessage.ActionType = Actions.SiegeList;
        protoMessage.Message = "Char_158";
        client.net.Send(protoMessage);
        var reply = GetActionReply(Actions.SiegeList, client);
        return (ProtoGenericArray<ProtoSiegeOverview>)reply;
    }
    public static ProtoPlayerCharacter Profile(TextTestClient client)
    {
        ProtoPlayerCharacter protoMessage = new ProtoPlayerCharacter();
        protoMessage.Message = "Char_158";
        protoMessage.ActionType = Actions.ViewChar;
        client.net.Send(protoMessage);
        var reply = GetActionReply(Actions.ViewChar, client);
        return (ProtoPlayerCharacter)reply;
    }

    


    /// <summary>
    /// army function
    /// </summary>
    /// <param name="client"></param>
    /// <returns></returns>

    public static ProtoMessage MaintainArmy(string armyID, TextTestClient client)                                   // Complete
    {
       // ProtoPlayerCharacter armyResult = GetArmyID(client);
        ProtoMessage protoMessage = new ProtoMessage();
        protoMessage.ActionType = Actions.MaintainArmy;
        protoMessage.Message = armyID;
        client.net.Send(protoMessage);
        var reply = GetActionReply(Actions.MaintainArmy, client);
        return reply;
    }
    public static ProtoMessage AppointLeader(string armyID,string charID, TextTestClient client)                    // Complete
    {
        //ProtoPlayerCharacter armyResult = GetArmyID(client);
        ProtoMessage protoMessage = new ProtoMessage();
        protoMessage.ActionType = Actions.AppointLeader;
        protoMessage.Message = armyID;
        protoMessage.MessageFields = new string[] { charID };
        client.net.Send(protoMessage);
        var reply = GetActionReply(Actions.AppointLeader, client);
        return reply;
    }

    public static ProtoGenericArray<ProtoDetachment> ListDetachments(string army, TextTestClient client)            // Complete
    {
        ProtoMessage protoMessage = new ProtoMessage();
        protoMessage.ActionType = Actions.ListDetachments;
        protoMessage.Message = army;
        client.net.Send(protoMessage);
        var reply = GetActionReply(Actions.ListDetachments, client);
        var Detachments = (ProtoGenericArray<ProtoDetachment>)reply;
        return Detachments;
    }
    public static ProtoMessage DropOffTroops(uint[] Troops, TextTestClient client)                                  // Complete
    {
        ProtoCharacter armyResult = GetCharacterDetails(protoClient.activeChar.charID, client);
        ProtoDetachment protoDetachment = new ProtoDetachment();
        protoDetachment.ActionType = Actions.DropOffTroops;
        protoDetachment.troops = Troops;
        protoDetachment.armyID = armyToViewID;
        protoDetachment.leftFor = "Char_158";
        client.net.Send(protoDetachment);
        var reply = GetActionReply(Actions.DropOffTroops, client);
        return reply;
    }
    public static ProtoMessage PickUpTroops(string armyID, string[] detachmentIDs, TextTestClient client)         // 1/2 Complete. Need to do some UI work and fix server crashes
    {
        ProtoMessage protoMessage = new ProtoMessage();
        protoMessage.ActionType = Actions.PickUpTroops;
        protoMessage.Message = armyID;
        protoMessage.MessageFields = detachmentIDs;
        client.net.Send(protoMessage);
        var reply = GetActionReply(Actions.PickUpTroops, client);
        return reply;
    }
    public static ProtoMessage HireTroops(int amount, TextTestClient client)                                    // Complete. Need to do some error handling
    {
        ProtoCharacter armyResult = GetCharacterDetails(protoClient.activeChar.charID, client);
        ProtoRecruit protoRecruit = new ProtoRecruit();
        protoRecruit.ActionType = Actions.RecruitTroops;
        if (amount > 0)
        {
            protoRecruit.amount = (uint)amount;
        }
        protoRecruit.armyID = armyResult.armyID;
        protoRecruit.isConfirm = true;
        client.net.Send(protoRecruit);
        var reply = GetActionReply(Actions.RecruitTroops, client);
        return reply;
    }

    public static ProtoMessage PillageFief(TextTestClient client)                                           // Complete
    {
        ProtoCharacter armyResult = GetCharacterDetails(protoClient.activeChar.charID,client);
        ProtoMessage protoMessage = new ProtoMessage();
        protoMessage.ActionType = Actions.PillageFief;
        protoMessage.Message = armyResult.armyID;
        
        client.net.Send(protoMessage);
        var reply = GetActionReply(Actions.PillageFief, client);
        return reply;
    }
    public static ProtoMessage DisbandArmy(string armyID, TextTestClient client)                            // Complete
    {
        ProtoMessage protoMessage = new ProtoMessage();
        protoMessage.ActionType = Actions.DisbandArmy;
        protoMessage.Message = armyID;
        client.net.Send(protoMessage);
        var reply = GetActionReply(Actions.DisbandArmy, client);
        return reply;
    }
    /// <summary>
    /// family functions
    /// </summary>
    /// <param name="client"></param>
    /// <param name="brideID"></param>
    /// <returns></returns>
    public static ProtoMessage Marry(TextTestClient client, string brideID)
    {
        ProtoMessage protoMessage = new ProtoMessage();
        protoMessage.ActionType = Actions.ProposeMarriage;
        protoMessage.Message = protoClient.activeChar.armyID;
        protoMessage.MessageFields = new string[] { brideID };
        client.net.Send(protoMessage);
        var reply = GetActionReply(Actions.ProposeMarriage, client);
        return reply;
    }

    public static ProtoMessage AcceptRejectProposal(TextTestClient client, bool AcceptOrReject)
    {
        ProtoMessage protoMessage = new ProtoMessage();
        protoMessage.ActionType = Actions.AcceptRejectProposal;
        protoMessage.Message = protoClient.activeChar.armyID;
        protoMessage.MessageFields = new string[] { Convert.ToString(AcceptOrReject) };
        client.net.Send(protoMessage);
        var reply = GetActionReply(Actions.AcceptRejectProposal, client);
        return reply;
    }

    public static ProtoMessage TryForChild(string charid, TextTestClient client)
    {
        ProtoMessage protoMessage = new ProtoMessage();
        protoMessage.ActionType = Actions.TryForChild;
        protoMessage.Message = charid;
        client.net.Send(protoMessage);
        var reply = GetActionReply(Actions.TryForChild, client);
        return reply;
    }

    /// <summary>
    /// siege functions
    /// </summary>
    /// <param name="client"></param>
    /// <returns></returns>
    public static ProtoMessage SiegeCurrentFief(TextTestClient client)                                              // Complete
    {
        ProtoCharacter armyResult = GetCharacterDetails(protoClient.activeChar.charID, client);
        ProtoMessage protoSiegeStart = new ProtoMessage();
        protoSiegeStart.ActionType = Actions.BesiegeFief;
        protoSiegeStart.Message = armyResult.armyID;
        client.net.Send(protoSiegeStart);
        var reply = GetActionReply(Actions.BesiegeFief, client);
        return reply;
    }
    public static ProtoMessage ViewSiege(string siegeID, TextTestClient client)                                     // Complete
    {

        ProtoMessage ViewSiege = new ProtoMessage();
        ViewSiege.ActionType = Actions.ViewSiege;
        ViewSiege.Message = siegeID;
        client.net.Send(ViewSiege);
        var reply = GetActionReply(Actions.ViewSiege, client);
        return reply;
    }

    public static ProtoMessage SiegeRoundStorm(string siegeID, TextTestClient client)                               // Complete
    {
       
        ProtoMessage protoMessage = new ProtoMessage();
        protoMessage.ActionType = Actions.SiegeRoundStorm;
        protoMessage.Message = siegeID;
        client.net.Send(protoMessage);
        var reply = GetActionReply(Actions.SiegeRoundStorm, client);
        return reply;
    }

    public static ProtoMessage SiegeRoundReduction(string siegeID, TextTestClient client)                           // Complete
    {
       
        ProtoMessage protoMessage = new ProtoMessage();
        protoMessage.ActionType = Actions.SiegeRoundReduction;
        protoMessage.Message = siegeID;
        client.net.Send(protoMessage);
        var reply = GetActionReply(Actions.SiegeRoundReduction, client);
        return reply;
    }
    public static ProtoMessage SiegeRoundNegotiate(string siegeID, TextTestClient client)                           // Complete
    {
       
        ProtoMessage protoMessage = new ProtoMessage();
        protoMessage.ActionType = Actions.SiegeRoundNegotiate;
        protoMessage.Message = siegeID;
        client.net.Send(protoMessage);
        var reply = GetActionReply(Actions.SiegeRoundNegotiate, client);
        return reply;
    }
    public static ProtoMessage EndSiege(string siegeID,TextTestClient client)                                       // Complete
    {  
        ProtoMessage protoMessage = new ProtoMessage();
        protoMessage.ActionType = Actions.EndSiege;
        protoMessage.Message = siegeID;
        client.net.Send(protoMessage);
        var reply = GetActionReply(Actions.EndSiege, client);
        return  reply;
    }
    /// <summary>
    /// spy funcations
    /// </summary>
    /// <param name="fiefID"></param>
    /// <param name="client"></param>
    /// <returns></returns>
    public static ProtoMessage SpyFief(string fiefID, string charID, TextTestClient client)                           // Complete
    {
        ProtoMessage protoMessage = new ProtoMessage();
        protoMessage.ActionType = Actions.SpyFief;
        protoMessage.Message = charID;
        protoMessage.MessageFields = new string[] { fiefID };
        client.net.Send(protoMessage);
        var reply = GetActionReply(Actions.SpyFief, client);
        return reply;
    }
    public static ProtoMessage SpyCharacter(string targetID, string charID, TextTestClient client)
    {
        ProtoMessage protoMessage = new ProtoMessage();
        protoMessage.ActionType = Actions.SpyCharacter;
        protoMessage.Message = charID;
        protoMessage.MessageFields = new string[] { targetID };
        client.net.Send(protoMessage);
        var reply = GetActionReply(Actions.SpyCharacter, client);
        return reply;
    }

    public static ProtoMessage SpyArmy(string armyID, string charID, TextTestClient client)
    {
        ProtoMessage protoMessage = new ProtoMessage();
        protoMessage.ActionType = Actions.SpyArmy;
        protoMessage.Message = charID;
        protoMessage.MessageFields = new string[] { armyID };
        client.net.Send(protoMessage);
        var reply = GetActionReply(Actions.SpyArmy, client);
        return reply;
    }

    public static ProtoJournalEntry ViewJournalEntry(uint journalID, TextTestClient client)
    {
        ProtoMessage protoMessage = new ProtoMessage();
        protoMessage.ActionType = Actions.ViewJournalEntry;
        protoMessage.Message = protoClient.activeChar.armyID;
        protoMessage.MessageFields = new string[] { Convert.ToString(journalID) };
        client.net.Send(protoMessage);
        var reply = GetActionReply(Actions.ViewJournalEntry, client);
        return (ProtoJournalEntry)reply;
    }
    /// <summary>
    /// kidnapping functions
    /// </summary>
    /// <param name="targetID"></param>
    /// <param name="client"></param>
    /// <returns></returns>
    public static ProtoMessage Kidnap(string targetID,string kidnapperID, TextTestClient client)
    {
        ProtoMessage protoMessage = new ProtoMessage();
        protoMessage.ActionType = Actions.Kidnap;
        protoMessage.Message = targetID;
        protoMessage.MessageFields = new string[] { kidnapperID };
        client.net.Send(protoMessage);

        var reply = GetActionReply(Actions.Kidnap, client);
        return reply;
    }
    public static ProtoMessage ViewCaptives(string captiveLocation, TextTestClient client)
    {
        ProtoMessage protoMessage = new ProtoMessage();
        protoMessage.ActionType = Actions.ViewCaptives;
        protoMessage.Message = captiveLocation;   
        client.net.Send(protoMessage);
        var reply = GetActionReply(Actions.ViewCaptives, client);
        return reply;
    }
    public static ProtoMessage ViewCaptive(string charID, TextTestClient client)
    {
        ProtoMessage protoMessage = new ProtoMessage();
        protoMessage.ActionType = Actions.ViewCaptive;
        protoMessage.Message = charID;
        client.net.Send(protoMessage);
        var reply = GetActionReply(Actions.ViewCaptive, client);
        return reply;
    }
    public static ProtoMessage RansomCaptive(string charID, TextTestClient client)
    {
        ProtoMessage protoMessage = new ProtoMessage();
        protoMessage.ActionType = Actions.RansomCaptive;
        protoMessage.Message = charID;
        client.net.Send(protoMessage);
        var reply = GetActionReply(Actions.RansomCaptive, client);
        return reply;
    }
    public static ProtoMessage ReleaseCaptive(string charID, TextTestClient client)
    {
        ProtoMessage protoMessage = new ProtoMessage();
        protoMessage.ActionType = Actions.ReleaseCaptive;
        protoMessage.Message = charID;
        client.net.Send(protoMessage);
        var reply = GetActionReply(Actions.ReleaseCaptive, client); 
        return reply;
    }
    public static ProtoMessage ExecuteCaptive(string charID, TextTestClient client)
    {
        ProtoMessage protoMessage = new ProtoMessage();
        protoMessage.ActionType = Actions.ExecuteCaptive;
        protoMessage.Message = charID;
        client.net.Send(protoMessage);
        var reply = GetActionReply(Actions.ExecuteCaptive, client);
        return reply;
    }
    public static ProtoMessage RespondRansom(uint jEntryID, bool pay, TextTestClient client)
    {
        ProtoMessage protoMessage = new ProtoMessage();
        protoMessage.ActionType = Actions.ExecuteCaptive;
        protoMessage.Message = Convert.ToString(jEntryID);
        protoMessage.MessageFields = new string[] { Convert.ToString(pay)};
        client.net.Send(protoMessage);
        var reply = GetActionReply(Actions.ExecuteCaptive, client);
        return reply;
    }

    /// <summary>
    /// NPC functions
    /// </summary>
    /// <param name="npcID"></param>
    /// <param name="client"></param>
    /// <returns></returns>


    public static ProtoMessage BarCharacters(string fiefID, string[] charIDs, TextTestClient client)
    {
        ProtoMessage protoMessage = new ProtoMessage();
        protoMessage.ActionType = Actions.BarCharacters;
        protoMessage.Message = fiefID;
        protoMessage.MessageFields = charIDs;
        client.net.Send(protoMessage);
        var reply = GetActionReply(Actions.BarCharacters, client);
        return reply;
    }
    public static ProtoMessage UnbarCharacters(string fiefID, string[] charIDs, TextTestClient client)
    {
        ProtoMessage protoMessage = new ProtoMessage();
        protoMessage.ActionType = Actions.UnbarCharacters;
        protoMessage.Message = fiefID;
        protoMessage.MessageFields = charIDs;
        client.net.Send(protoMessage);
        var reply = GetActionReply(Actions.UnbarCharacters, client);
        return reply;
    }
    public static ProtoMessage BarNationalities(string fiefID, string[] netIDs, TextTestClient client)
    {
        ProtoMessage protoMessage = new ProtoMessage();
        protoMessage.ActionType = Actions.BarNationalities;
        protoMessage.Message = fiefID;
        protoMessage.MessageFields = netIDs;
        client.net.Send(protoMessage);
        var reply = GetActionReply(Actions.BarNationalities, client);
        return reply;
    }
    public static ProtoMessage UnbarNationalities(string fiefID, string[] netIDs, TextTestClient client)
    {
        ProtoMessage protoMessage = new ProtoMessage();
        protoMessage.ActionType = Actions.UnbarNationalities;
        protoMessage.Message = fiefID;
        protoMessage.MessageFields = netIDs;
        client.net.Send(protoMessage);
        var reply = GetActionReply(Actions.UnbarNationalities, client);
        return reply;
    }
    public static ProtoMessage GrantFiefTitle(string fiefID, TextTestClient client)
    {
        ProtoMessage protoMessage = new ProtoMessage();
        protoMessage.ActionType = Actions.GrantFiefTitle;
        protoMessage.Message = fiefID;
        protoMessage.MessageFields = new string[] { protoClient.activeChar.armyID };
        client.net.Send(protoMessage);
        var reply = GetActionReply(Actions.GrantFiefTitle, client);
        return reply;
    }

    public static ProtoMessage TransferFundsToPlayer(string playerTo, int amount, TextTestClient client)
    {
        ProtoTransferPlayer ProtoTransferPlayer = new ProtoTransferPlayer();
        ProtoTransferPlayer.playerTo = playerTo;
        ProtoTransferPlayer.amount = amount;
        ProtoTransferPlayer.ActionType = Actions.AdjustExpenditure;
        client.net.Send(ProtoTransferPlayer);
        var reply = GetActionReply(Actions.AdjustExpenditure, client);
        return reply;
    }
    public static ProtoMessage AdjustCombatValues(string armyID, byte aggression, byte odds, TextTestClient client)
    {
        ProtoCombatValues ProtoCombatValues = new ProtoCombatValues();
        ProtoCombatValues.armyID = armyID;
        ProtoCombatValues.aggression = aggression;
        ProtoCombatValues.odds = odds;
        ProtoCombatValues.ActionType = Actions.AdjustCombatValues;
        client.net.Send(ProtoCombatValues);
        var reply = GetActionReply(Actions.AdjustCombatValues, client);
        return reply;
    }

    public static ProtoMessage EnterExitKeep(string charID, TextTestClient client)
    {
        ProtoMessage protoMessage = new ProtoMessage();
        protoMessage.ActionType = Actions.EnterExitKeep;
        protoMessage.Message = charID;
        client.net.Send(protoMessage);
        var reply = GetActionReply(Actions.EnterExitKeep, client);
        return reply;
    }

    public static ProtoMessage Camp(string charID, byte days, TextTestClient client)
    {
        ProtoMessage protoMessage = new ProtoMessage();
        protoMessage.ActionType = Actions.GrantFiefTitle;
        protoMessage.Message = charID;
        protoMessage.MessageFields = new string[] { "days" };
        client.net.Send(protoMessage);
        var reply = GetActionReply(Actions.GrantFiefTitle, client);
        return reply;
    }

    public static ProtoMessage GetProvince(string pID, TextTestClient client)
    {
        ProtoMessage msg = new ProtoMessage();
        msg.ActionType = Actions.GetProvince;
        msg.Message = pID;
        client.net.Send(msg);
        var reply = GetActionReply(Actions.GetProvince, client);
        return reply;
    }


    private static void GenerateColourList() {
        int i;
        byte r,g,b,a;
        r=g=b=0;
        a = 255;

        // Reds
        r = 102;
        for(i = 0; i < 3; i++, r+=51) {
            colours.Add(new Color32(r,g,b,a));
        }
        r = 255;
        for(i = 0; i < 5; i++, g+=51, b+=51) {
            colours.Add(new Color32(r,g,b,a));
        }

        // Oranges
        r = 102; g = 51; b = 0;
        for(i = 0; i < 3; i++, r+=51, g+=25) {
            colours.Add(new Color32(r,g,b,a));
        }
        r = 255; g = 128; b = 0;
        for(i = 0; i < 5; i++, g+=25, b+=51) {
            colours.Add(new Color32(r,g,b,a));
        }

        // Yellows
        r = 102; g = 102; b = 0;
        for(i = 0; i < 3; i++, r+=51, g+=51) {
            colours.Add(new Color32(r,g,b,a));
        }
        r = 255; g = 255; b = 0;
        for(i = 0; i < 5; i++, b+=51) {
            colours.Add(new Color32(r,g,b,a));
        }

        // Greeny-yellows
        r = 51; g = 102; b = 0;
        for(i = 0; i < 3; i++, g+=25, b+=51) {
            colours.Add(new Color32(r,g,b,a));
        }
        r = 128; g = 255; b = 0;
        for(i = 0; i < 5; i++, r+=25, b+=51) {
            colours.Add(new Color32(r,g,b,a));
        }

        // Greens
        r = 0; g = 102; b = 0;
        for(i = 0; i < 3; i++, g+=51) {
            colours.Add(new Color32(r,g,b,a));
        }
        r = 0; g = 255; b = 0;
        for(i = 0; i < 5; i++, r+=51, b+=51) {
            colours.Add(new Color32(r,g,b,a));
        }

        // Greeny-blues
        r = 0; g = 102; b = 51;
        for(i = 0; i < 3; i++, g+=51, b+=25) {
            colours.Add(new Color32(r,g,b,a));
        }
        r = 0; g = 255; b = 128;
        for(i = 0; i < 5; i++, r+=51, b+=25) {
            colours.Add(new Color32(r,g,b,a));
        }

        // Teals/Cyans
        r = 0; g = 102; b = 102;
        for(i = 0; i < 3; i++, g+=51, b+=51) {
            colours.Add(new Color32(r,g,b,a));
        }
        r = 0; g = 255; b = 255;
        for(i = 0; i < 5; i++, r+=51) {
            colours.Add(new Color32(r,g,b,a));
        }

        // Light Blues
        r = 0; g = 51; b = 102;
        for(i = 0; i < 3; i++, g+=25, b+=51) {
            colours.Add(new Color32(r,g,b,a));
        }
        r = 0; g = 128; b = 255;
        for(i = 0; i < 5; i++, r+=51, g+=25) {
            colours.Add(new Color32(r,g,b,a));
        }

        // Purples
        r = 51; g = 0; b = 102;
        for(i = 0; i < 3; i++, r+=25, b+=51) {
            colours.Add(new Color32(r,g,b,a));
        }
        r = 127; g = 0; b = 255;
        for(i = 0; i < 5; i++, r+=25, g+=51) {
            colours.Add(new Color32(r,g,b,a));
        }

        // Pinky-purples
        r = 102; g = 0; b = 102;
        for(i = 0; i < 3; i++, r+=51, b+=51) {
            colours.Add(new Color32(r,g,b,a));
        }
        r = 255; g = 0; b = 255;
        for(i = 0; i < 5; i++, g+=51) {
            colours.Add(new Color32(r,g,b,a));
        }

        // Hot-pinks
        r = 102; g = 0; b = 51;
        for(i = 0; i < 3; i++, r+=51, b+=25) {
            colours.Add(new Color32(r,g,b,a));
        }
        r = 255; g = 0; b = 127;
        for(i = 0; i < 5; i++, g+=51, b+=25) {
            colours.Add(new Color32(r,g,b,a));
        }
    }


}
