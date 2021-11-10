using ProtoMessageClient;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewingList : Controller
{

    [SerializeField] private Text lblPageTitle;
    [SerializeField] private Text lblMessageForUser;

    public GameObject viewportContent;

    public GameObject panelTemplate;
    public GameObject lblTemplate;
    public GameObject btnTemplate;

    private enum ListType {
        Army,
        Character,
        Fief,
        Journal
    }

    private ListType listType;

    // Start is called before the first frame update
    public void Start()
    {
        lblMessageForUser.text = "";
        Initialise();
    }

    new void Initialise() {
        switch(viewingListSelectAction) {
            case "ArmiesInFief": {
                lblPageTitle.text = "Armies in " + fiefNames[fiefToViewID];
                listType = ListType.Army;
                ProtoMessage armiesInFief = ExamineArmiesInFief(fiefToViewID, tclient);
                if(armiesInFief.ResponseType == DisplayMessages.Success) {
                    armyList = (ProtoGenericArray<ProtoArmyOverview>)armiesInFief;
                }
                else {
                    DisplayMessageToUser("ERROR: Response type: " + armiesInFief.ResponseType);
                }
                break;
            }
            case "YourArmies": {
                    GoToScene(SceneName.ViewArmiesList);
                break;
            }
            case "Entourage": {
                lblPageTitle.text = "Your Current Entourage";
                listType = ListType.Character;
                break;
            }
            case "ListChars": {
                lblPageTitle.text = "Persons present in the " + globalString + " of " + fiefNames[fiefToViewID];
                listType = ListType.Character;
                ProtoMessage reply = ListCharsInMeetingPlace(globalString, protoClient.activeChar.charID, tclient);
                if(reply.ResponseType == DisplayMessages.Success) {
                    characterList = (ProtoGenericArray<ProtoCharacterOverview>)reply;
                }
                else {
                    DisplayMessageToUser("ERROR: Response type: " + reply.ResponseType.ToString());
                }
                break;
            }
            case "AssignBailiff": {
                lblPageTitle.text = "Assign Bailiff to " + fiefNames[fiefToViewID];
                listType = ListType.Character;
                break;
            }
            case "SwitchCharacter": {
                    GoToScene(SceneName.ChangeCharactersList);
                    break;
            }
            case "TransferFundsToFief": {
                lblPageTitle.text = "Transfer Money to Fief";
                listType = ListType.Fief;
                ProtoMessage reply = ViewMyFiefs(tclient);
                if(reply.ResponseType == DisplayMessages.Success) {
                    fiefList = (ProtoGenericArray<ProtoFief>)reply;
                }
                else {
                    DisplayMessageToUser("ERROR: Response type: " + reply.ResponseType.ToString());
                }
                break;
            }
            case "MyFiefs": {
                GoToScene(SceneName.ViewMyFiefsList);
                break;
            }
            case "Journal": {
                lblPageTitle.text = "Journal";
                listType = ListType.Journal;
                ProtoMessage reply = ViewJournalEntries("all", tclient);
                if(reply.ResponseType == DisplayMessages.JournalEntries) {
                    journalList = (ProtoGenericArray<ProtoJournalEntry>)reply;
                }
                else {
                    DisplayMessageToUser("ERROR: Response type: " + reply.ResponseType.ToString());
                }
                break;
            }
            default: {
                DisplayMessageToUser("ERROR: Invalid action encountered.");
                return;
            }
        }

        CreatePanel(out GameObject panel, out GameObject label, out GameObject button);
        Destroy(button);

        switch(listType) {
            case ListType.Character: {
                if(characterList.fields == null) {
                    DisplayMessageToUser("There are no valid characters.");
                    return;
                }

                label.GetComponent<Text>().text = string.Format("\t{0,-30}{1,-30}{2,-30}{3,-30}", "Name", "Sex", "Role", "Location");

                foreach(ProtoCharacterOverview character in characterList.fields) {
                    CreatePanel(out panel, out label, out button);

                    string location = string.IsNullOrWhiteSpace(character.locationID) ? "Unknown" : fiefNames[character.locationID];

                    string sex = character.isMale ? "Male" : "Female";
                    label.GetComponent<Text>().text = string.Format("\t{0,-30}{1,-30}{2,-30}{3,-30}", character.charName, sex, character.role, location);
                    //label.GetComponent<Text>().text = ""
                    //    + character.charName + "\t\t\t"
                    //    + sex + "\t\t\t"
                    //    + character.role + "\t\t\t"
                    //    + character.locationID
                    //    ;

                    if(viewingListSelectAction.Equals("Entourage") || viewingListSelectAction.Equals("ListChars")) {
                        button.GetComponent<Button>().onClick.AddListener( () => { BtnViewCharacter(character.charID); } );
                    }
                    else if(viewingListSelectAction.Equals("AssignBailiff")) {
                        button.GetComponent<Button>().onClick.AddListener( () => { BtnAppointBailiff(character.charID); } );
                    }
                    else if(viewingListSelectAction.Equals("SwitchCharacter")) {
                        button.GetComponent<Button>().onClick.AddListener( () => { BtnSwitchCharacter(character.charID); } );
                    }
                }
                break;
            }
            case ListType.Journal: {
                if(journalList.fields == null) {
                    DisplayMessageToUser("There are no valid journal entries.");
                    return;
                }

                label.GetComponent<Text>().text = string.Format("\t{0,-20}{1,-20}{2,-20}{3,-30}{4,-30}", "Entry ID", "Year", "Season", "Event", "Location");

                foreach(ProtoJournalEntry journalEntry in journalList.fields) {
                    CreatePanel(out panel, out label, out button);

                    string season = SeasonToString(journalEntry.season);
                    string location = string.IsNullOrWhiteSpace(journalEntry.location) ? "" : fiefNames[journalEntry.location];
                    if(string.IsNullOrWhiteSpace(journalEntry.location)) {

                    }
                    label.GetComponent<Text>().text = string.Format("{0,-20}{1,-20}{2,-20}{3,-30}{4,-30}", journalEntry.jEntryID, journalEntry.year, season, journalEntry.type, location);

                    //var lblContents = label.GetComponent<Text>();
                    //lblContents.text = ""
                    //    + journalEntry.jEntryID.ToString() + "\t\t\t"
                    //    + journalEntry.year.ToString() + "\t\t\t"
                    //    + SeasonToString(journalEntry.season) + "\t\t\t"
                    //    + journalEntry.type + "\t\t"
                    //    + journalEntry.location
                    //    ;
                    //if(journalEntry.type.Contains("death")) {
                    //    lblContents.text += journalEntry.eventDetails.MessageFields[0];
                    //}

                    if(viewingListSelectAction.Equals("Journal")) {
                        button.GetComponent<Button>().onClick.AddListener( () => { BtnViewJournalEntry(journalEntry.jEntryID); } );
                    }
                }
                break;
            }
        }
    }

    private void BtnTransferFundsToFief(string fiefToID, int funds) {
        ProtoMessage reply = TransferFunds(fiefToViewID, fiefToID, funds, tclient);
        if(reply.ResponseType == DisplayMessages.Success) {
            GoToScene(SceneName.ViewFief);
        }
        else {
            DisplayMessageToUser("ERROR: Response type: " + reply.ResponseType.ToString());
        }
    }

    private void BtnSwitchCharacter(string charID) {
        ProtoMessage reply = UseChar(charID, tclient);
        if(reply.ResponseType == DisplayMessages.Success) {
            characterToViewID = charID;
            GoToScene(SceneName.ViewCharacter);
        }
        else {
            DisplayMessageToUser("ERROR: Response type: " + reply.ResponseType.ToString());
        }
    }

    private void BtnViewCharacter(string charID) {
        characterToViewID = charID;
        GoToScene(SceneName.ViewCharacter);
    }


    private void BtnViewJournalEntry(uint jEntryID) {
        journalEntryToViewID = jEntryID;
        GoToScene(SceneName.ViewJournalEntry);
    }

    private void BtnAppointBailiff(string charID) {
        ProtoMessage reply = AppointBailiff(charID, fiefToViewID, tclient);
        if(reply.ResponseType == DisplayMessages.Success) {
            GoToScene(SceneName.ViewFief);
        }
        else {
            DisplayMessageToUser("ERROR: Response type: " + reply.ResponseType.ToString());
        }
    }

    private void CreatePanel(out GameObject panel, out GameObject label, out GameObject button) {
        panel = Instantiate(panelTemplate);
        label = Instantiate(lblTemplate);
        button = Instantiate(btnTemplate);
        
        panel.transform.SetParent(viewportContent.transform);
        label.transform.SetParent(panel.transform);
        button.transform.SetParent(panel.transform);

        label.transform.localPosition = new Vector3(-90f, 0f);
        button.transform.localPosition = new Vector3(660f, 0f);
    }

    void DisplayMessageToUser(string message) {
        lblMessageForUser.text = "[" + DateTime.Now.ToString("h:mm:ss tt") + "] " + message;
    }

    private void AddPanel() {

        GameObject panel = Instantiate(panelTemplate);
        GameObject label = Instantiate(lblTemplate);
        GameObject button = Instantiate(btnTemplate);
        
        panel.transform.SetParent(viewportContent.transform);
        label.transform.SetParent(panel.transform);
        button.transform.SetParent(panel.transform);

        label.transform.localPosition = new Vector3(-90f, 0f);
        button.transform.localPosition = new Vector3(660f, 0f);

        label.GetComponent<Text>().text = "I am a label in a panel. sahfdgweugegvehjvgberkjhvergverhgverjhvhergverjhvgerjhvgerjherkjvherkjvhj";
        //button.transform.GetChild(0).GetComponent<Text>().text = "Panel Button";

        //button.transform.SetParent(viewport.transform);
        //button.GetComponent<Button>().onClick.AddListener( () => { printTest(charID); } );
        //button.transform.GetChild(0).GetComponent<Text>().text = charID;
    }
}
