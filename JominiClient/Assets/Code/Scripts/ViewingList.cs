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
            case "ArmiesInFief":
                {
                    GoToScene(SceneName.ArmiesInFief);
                    break;
            }
            case "YourArmies":
                {
                    GoToScene(SceneName.ViewArmiesList);
                    break;
            }
            case "Entourage": {
                lblPageTitle.text = "Your Current Entourage";
                listType = ListType.Character;
                break;
            }
            case "ListChars": {
                    GoToScene(SceneName.ViewCharacters);
                break;
            }
            case "AssignBailiff": {
                    GoToScene(SceneName.AssignBailiff);
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
                GoToScene(SceneName.ViewJournalEntries);
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
                    if(viewingListSelectAction.Equals("Entourage") || viewingListSelectAction.Equals("ListChars")) {
                        button.GetComponent<Button>().onClick.AddListener( () => { BtnViewCharacter(character.charID); } );
                    }
                    else if(viewingListSelectAction.Equals("SwitchCharacter")) {
                        button.GetComponent<Button>().onClick.AddListener( () => { BtnSwitchCharacter(character.charID); } );
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
