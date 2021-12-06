using ProtoMessageClient;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewArmy : Controller
{
    [SerializeField] private Text lblPageTitle;
    [SerializeField] private Text lblMessageForUser;
    [SerializeField] private Text lblArmyInfo;

    [SerializeField] private Button btnAttackArmy;
    [SerializeField] private Button btnMaintainArmy;
    [SerializeField] private Button btnDisbandArmy;
    [SerializeField] private Button btnDropoffTroops;

    [Header("Dropoff Units")]
    public TMPro.TMP_InputField InputRabble;
    public TMPro.TMP_InputField InputFootmen;
    public TMPro.TMP_InputField InputCrossbowmen;
    public TMPro.TMP_InputField InputLongbowmen;
    public TMPro.TMP_InputField InputLightCavalry;
    public TMPro.TMP_InputField InputMenAtArms;
    public TMPro.TMP_InputField InputKnights;

    [Header("Pickup Panel")]
    public GameObject PickUpPanel;
    public GameObject PickUpPanelHolder;
    public GameObject DetachementPrefab;
    public Button btnClosePickUp;
    public Button btnListDetachments;


    private ProtoArmy currentlyViewedArmy;

    // Start is called before the first frame update
    public void Start()
    {
        btnAttackArmy.onClick.AddListener(BtnAttackArmy);
        btnMaintainArmy.onClick.AddListener(BtnMaintainArmy);
        btnDisbandArmy.onClick.AddListener(BtnDisbandArmy);
        btnListDetachments.onClick.AddListener(BtnListDetachments);
        btnDropoffTroops.onClick.AddListener(BtnDropoffTroops);
        btnClosePickUp.onClick.AddListener(SwitchOffPanel);
        lblMessageForUser.text = "";
        btnAttackArmy.interactable = false;
        PickUpPanel.SetActive(false);


        ProtoMessage reply = GetArmyDetails(armyToViewID, tclient);
        if(reply.ResponseType == DisplayMessages.Success) 
        {
            currentlyViewedArmy = (ProtoArmy)reply;
            
            DisplayArmyDetails();

            // Can't attack if you don't have an army.
            if(!string.IsNullOrWhiteSpace(protoClient.activeChar.armyID)) {
                // Can't attack your own armies.
                if (!currentlyViewedArmy.ownerID.Equals(protoClient.playerChar.charID)) {
                    if(currentlyViewedArmy.location.StartsWith(fiefNames[protoClient.playerChar.location])) {
                        btnAttackArmy.interactable = true;
                    }
                }
            }

            if (currentlyViewedArmy.isMaintained)
            {
                btnMaintainArmy.interactable = false;
            }

        }
        else {
            DisplayMessageToUser("ERROR: Response type: " + reply.ResponseType.ToString());
        }
        
    }

    private void SwitchOffPanel()
    {
        PickUpPanel.SetActive(false);
    }

    public void BtnControlArmy()
    {
        AppointLeader(armyToViewID, protoClient.activeChar.charID, tclient);
        GoToScene(SceneName.ViewArmy);
    }

    private void BtnListDetachments()
    {
        Debug.Log("Get detachements for: " + currentlyViewedArmy.armyID);
        ProtoMessage reply = ListDetachments(currentlyViewedArmy.armyID ,tclient);
        switch (reply.ResponseType)
        {
            case DisplayMessages.Success:
                {
                    ProtoGenericArray<ProtoDetachment> detachemts = (ProtoGenericArray<ProtoDetachment>)reply;
                    if(detachemts.fields == null)
                    {
                        DisplayMessageToUser("There are no detachements in " + currentlyViewedArmy.location);
                    }
                    else
                    {
                        if(PickUpPanelHolder.transform.childCount > 2)
                        {
                            for(int i = 2; i < PickUpPanelHolder.transform.childCount; i++)
                            {
                                Destroy(PickUpPanelHolder.transform.GetChild(i).gameObject);
                            }
                        }
                        PickUpPanelHolder.transform.GetChild(0).GetComponent<TMPro.TMP_Text>().SetText("Detachements in " + currentlyViewedArmy.location);
                        foreach (ProtoDetachment detachement in detachemts.fields)
                        {
                            GameObject gameObject = Instantiate(DetachementPrefab);
                            gameObject.transform.SetParent(PickUpPanelHolder.transform);
                            gameObject.transform.localScale = Vector3.one;
                            gameObject.transform.GetChild(0).GetComponent<TMPro.TMP_Text>().SetText(detachement.id);
                            gameObject.transform.GetChild(1).GetComponent<TMPro.TMP_Text>().SetText(detachement.troops[0].ToString());
                            gameObject.transform.GetChild(2).GetComponent<TMPro.TMP_Text>().SetText(detachement.troops[1].ToString());
                            gameObject.transform.GetChild(3).GetComponent<TMPro.TMP_Text>().SetText(detachement.troops[2].ToString());
                            gameObject.transform.GetChild(4).GetComponent<TMPro.TMP_Text>().SetText(detachement.troops[3].ToString());
                            gameObject.transform.GetChild(5).GetComponent<TMPro.TMP_Text>().SetText(detachement.troops[4].ToString());
                            gameObject.transform.GetChild(6).GetComponent<TMPro.TMP_Text>().SetText(detachement.troops[5].ToString());
                            gameObject.transform.GetChild(7).GetComponent<TMPro.TMP_Text>().SetText(detachement.troops[6].ToString());
                            gameObject.transform.GetChild(8).GetComponent<Button>().onClick.AddListener(PickupDetachement);
                            gameObject.transform.GetChild(8).GetComponent<Button>().name = detachement.id;
                        }
                        PickUpPanel.SetActive(true);
                    }
                    break;
                }
            case DisplayMessages.ErrorGenericUnauthorised:
                {
                    DisplayMessageToUser("ErrorGenericUnauthorised!");
                    break;
                }
            case DisplayMessages.ErrorGenericArmyUnidentified:
                {
                    DisplayMessageToUser("ErrorGenericArmyUnidentified!");
                    break;
                }
            case DisplayMessages.ErrorGenericMessageInvalid:
                {
                    DisplayMessageToUser("ErrorGenericMessageInvalid!");
                    break;
                }
            default:
                {
                    DisplayMessageToUser("default!");
                    break;
                }
        }
    }

    private void PickupDetachement()
    {
        string[] detachementID = new string[] { UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.name };
        Debug.Log($"ID: {currentlyViewedArmy.armyID} || name {detachementID[0]}------------------------------------");
        ProtoMessage reply = PickUpTroops(currentlyViewedArmy.armyID, detachementID, tclient);

        switch (reply.ResponseType)
        {
            case DisplayMessages.ErrorGenericMessageInvalid:
                {
                    DisplayMessageToUser("ErrorGenericMessageInvalid!");
                    break;
                }
            case DisplayMessages.ErrorGenericArmyUnidentified:
                {
                    DisplayMessageToUser("ErrorGenericArmyUnidentified!");
                    break;
                }
            case DisplayMessages.ErrorGenericUnauthorised:
                {
                    DisplayMessageToUser("ErrorGenericUnauthorised!");
                    break;
                }
            case DisplayMessages.ArmyPickupsNotEnoughDays:
                {
                    DisplayMessageToUser("ArmyPickupsNotEnoughDays!");
                    break;
                }
            case DisplayMessages.Success:
                {
                    GoToScene(SceneName.ViewArmy);
                    break;
                }
        }

    }

    private void BtnDropoffTroops()
    {
        uint[] troops = new uint[7]
        {
            InputKnights.text == "" ? 0 : uint.Parse(InputKnights.text),
            InputMenAtArms.text == "" ? 0 : uint.Parse(InputMenAtArms.text),
            InputLightCavalry.text == "" ? 0 : uint.Parse(InputLightCavalry.text),
            InputLongbowmen.text == "" ? 0 : uint.Parse(InputLongbowmen.text),
            InputCrossbowmen.text == "" ? 0 : uint.Parse(InputCrossbowmen.text),
            InputFootmen.text == "" ? 0 : uint.Parse(InputFootmen.text),
            InputRabble.text == "" ? 0 : uint.Parse(InputRabble.text),
        };

        uint totalTroops = 0;
        foreach (var troop in troops) totalTroops += troop;

        if(totalTroops > 0)
        {
            ProtoMessage reply = DropOffTroops(troops, tclient);

            switch (reply.ResponseType)
            {
                case DisplayMessages.ErrorGenericMessageInvalid:
                    {
                        DisplayMessageToUser("ErrorGenericMessageInvalid!");
                        break;
                    }
                case DisplayMessages.ErrorGenericArmyUnidentified:
                    {
                        DisplayMessageToUser("ErrorGenericArmyUnidentified!");
                        break;
                    }
                case DisplayMessages.ErrorGenericUnauthorised:
                    {
                        DisplayMessageToUser("ErrorGenericUnauthorised!");
                        break;
                    }
                case DisplayMessages.Success:
                    {
                        GoToScene(SceneName.ViewArmy);
                        break;
                    }
            }

            
        }

    }

    private void BtnDisbandArmy()
    {
        ProtoMessage reply = DisbandArmy(currentlyViewedArmy.armyID, tclient);

        switch (reply.ResponseType)
        {
            case DisplayMessages.ErrorGenericUnauthorised:
                {
                    DisplayMessageToUser("ErrorGenericUnauthorised!");
                    break;
                }
            case DisplayMessages.Success:
                {
                    GoToScene(SceneName.ViewArmiesList);
                    break;
                }
            default:
                {
                    DisplayMessageToUser("Default!");
                    break;
                }
        }

    }

    private void BtnAttackArmy() {
        ProtoMessage reply = Attack(protoClient.activeChar.armyID, currentlyViewedArmy.armyID, tclient);
        if(reply.ResponseType == DisplayMessages.BattleResults) {
            var battleResult = (ProtoBattle)reply;
            lblArmyInfo.text = ""
                + "Battle Took Place: " + battleResult.battleTookPlace.ToString()
                + "\nBattle Type: " + battleResult.circumstance.ToString()
                + "\nAttacker Victorious: " + battleResult.attackerVictorious.ToString()
                + "\n"
                + "\nAttacker Losses: " + battleResult.attackerCasualties.ToString()
                + "\nStature Change: " + battleResult.statureChangeAttacker.ToString()
                + "\n"
                + "\nDefender Losses: " + battleResult.defenderCasualties.ToString()
                + "\nStature Change: " + battleResult.statureChangeDefender.ToString()
                ;
        }
        else {
            DisplayMessageToUser("ERROR: Response type: " + reply.ResponseType.ToString());
        }
    }

    private void BtnMaintainArmy()
    {
        ProtoMessage reply = MaintainArmy(currentlyViewedArmy.armyID, tclient);

        switch (reply.ResponseType)
        {
            case DisplayMessages.ArmyMaintainInsufficientFunds:
                DisplayMessageToUser("Insufficient Funds to Maintain Army!");
                break;
            case DisplayMessages.ArmyMaintainConfirm:
                DisplayMessageToUser("Army Maintained Successfully!");
                currentlyViewedArmy.isMaintained = true;
                DisplayArmyDetails();
                btnMaintainArmy.interactable = false;
                break;
            case DisplayMessages.ArmyMaintainedAlready:
                DisplayMessageToUser("Army Is Already Maintained!");
                currentlyViewedArmy.isMaintained = true;
                DisplayArmyDetails();
                btnMaintainArmy.interactable = false;
                break;
            default:
                DisplayMessageToUser("Generic Error!");
                break;
        }


        if(reply.ResponseType == DisplayMessages.Success)
        {

        }
    }

    void DisplayMessageToUser(string message) {
        lblMessageForUser.text = "[" + DateTime.Now.ToString("h:mm:ss tt") + "] " + message;
    }

    private void DisplayArmyDetails() {
        //lblPageTitle.text = currentlyViewedCharacter.firstName + " " + currentlyViewedCharacter.familyName + "'s Army";
        lblPageTitle.text = currentlyViewedArmy.leader + "'s Army";

        uint totalTroops = 0;
        foreach(uint troops in currentlyViewedArmy.troops) {
            totalTroops += troops;
        }

        lblArmyInfo.text = ""
            + "\nOwner:\t\t\t\t" + currentlyViewedArmy.owner
            + "\nLeader:\t\t\t\t" + currentlyViewedArmy.leader
            + "\nLocation:\t\t\t" + currentlyViewedArmy.location
            + "\nNationality:\t\t" + currentlyViewedArmy.nationality
            + "\n"
            + "\nSiege Status:\t\t" + currentlyViewedArmy.siegeStatus
            + "\n"
            + "\nMaintained:\t\t" + currentlyViewedArmy.isMaintained
            + "\nMaintenance Cost:\t" + currentlyViewedArmy.maintCost
            + "\n"
            + "\nKnights:\t\t\t" + currentlyViewedArmy.troops[0].ToString()
            + "\nMen at Arms:\t\t" + currentlyViewedArmy.troops[1].ToString()
            + "\nLight Cavalry:\t\t" + currentlyViewedArmy.troops[2].ToString()
            + "\nLongbowmen:\t\t" + currentlyViewedArmy.troops[3].ToString()
            + "\nCrossbowmen:\t\t" + currentlyViewedArmy.troops[4].ToString()
            + "\nFootmen:\t\t\t" + currentlyViewedArmy.troops[5].ToString()
            + "\nRabble:\t\t\t\t" + currentlyViewedArmy.troops[6].ToString()
            + "\n"
            + "\nTotal Soldiers:\t" + totalTroops.ToString();
            ;
    }
}
