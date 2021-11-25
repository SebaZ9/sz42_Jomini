using ProtoMessageClient;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewFief : Controller
{
    [SerializeField] private Text lblDetails1;
    [SerializeField] private Text lblDetails2;
    //[SerializeField] private Text lblFinancialInformation;
    [SerializeField] private Text lblThisSeason;
    [SerializeField] private Text lblNextSeason;
    [SerializeField] private Text lblMessageForUser;
    [SerializeField] private Text lblPageTitle;

    [SerializeField] private Button btnAdjustExpenditure;
    //[SerializeField] private Button btnAutoAdjustExpenditure;
    [SerializeField] private Button btnAppointBailiff;
    [SerializeField] private Button btnRemoveBailiff;
    [SerializeField] private Button btnViewArmies;
    [SerializeField] private Button btnListCharsInMeetingPlace;
    [SerializeField] private Button btnTransferFunds;
    [SerializeField] private Button btnTravelTo;

    [SerializeField] private InputField txtTaxRate;
    [SerializeField] private InputField txtOfficials;
    [SerializeField] private InputField txtGarrison;
    [SerializeField] private InputField txtInfrastructure;
    [SerializeField] private InputField txtKeep;
    [SerializeField] private InputField txtTransferFunds;

    [SerializeField] private Dropdown ddMeetingPlaceType;

    public Transform PillageResultOutput;
    public GameObject PillagePanel;
    public Button btnPillageFief;
    public Button btnSpyFief;
    public Button btnSiegeFief;

    private ProtoFief currentlyViewedFief;

    // Start is called before the first frame update
    public void Start()
    {
        PillagePanel.SetActive(false);
        btnAdjustExpenditure.onClick.AddListener(BtnAdjustExpenditure);
        //btnAutoAdjustExpenditure.onClick.AddListener(BtnAutoAdjustExpenditures);
        btnAppointBailiff.onClick.AddListener(BtnViewPossibleBailiffs);
        btnRemoveBailiff.onClick.AddListener(BtnRemoveBailiff);
        btnViewArmies.onClick.AddListener(BtnViewArmies);
        btnListCharsInMeetingPlace.onClick.AddListener(BtnListCharsInMeetingPlace);
        btnTransferFunds.onClick.AddListener(BtnTransferFunds);
        btnTravelTo.onClick.AddListener(BtnTravelTo);
        btnSpyFief.onClick.AddListener(BtnSpyOnFief);
        btnSiegeFief.onClick.AddListener(BtnSiegeFief);
        lblMessageForUser.text = "";
        btnAdjustExpenditure.interactable = false;
        btnAppointBailiff.interactable = false;
        btnRemoveBailiff.interactable = false;
        btnViewArmies.interactable = false;
        btnListCharsInMeetingPlace.interactable = false;
        btnTransferFunds.interactable = false;

        txtTaxRate.interactable = false;
        txtOfficials.interactable = false;
        txtGarrison.interactable = false;
        txtInfrastructure.interactable = false;
        txtKeep.interactable = false;
        txtTransferFunds.interactable = false;

        ddMeetingPlaceType.interactable = false;


        ProtoMessage reply = GetFiefDetails(fiefToViewID, tclient);
        if(reply.ResponseType == DisplayMessages.Success) {
            currentlyViewedFief = (ProtoFief)reply;
            btnViewArmies.interactable = true;
        }
        else if(reply.ResponseType == DisplayMessages.ErrorGenericTooFarFromFief) {
            lblMessageForUser.text = "You have no information on this fief.";
            currentlyViewedFief = null;
            return;
        }
        else {
            DisplayMessageToUser("ERROR: Response type: " + reply.ResponseType.ToString());
            currentlyViewedFief = null;
            return;
        }

        if(currentlyViewedFief.ownerID.Equals(protoClient.playerChar.charID)) { // If player's own fief.
            txtTaxRate.interactable = true;
            txtOfficials.interactable = true;
            txtGarrison.interactable = true;
            txtInfrastructure.interactable = true;
            txtKeep.interactable = true;
            btnAdjustExpenditure.interactable = true;
            txtTransferFunds.interactable = true;
            btnTransferFunds.interactable = true;
            btnPillageFief.gameObject.SetActive(false);
            btnSpyFief.gameObject.SetActive(false);
            btnSiegeFief.gameObject.SetActive(false);

            if (currentlyViewedFief.bailiff == null) {
                btnAppointBailiff.interactable = true;
            }
            else {
                btnRemoveBailiff.interactable = true;
            }

        }

        if(protoClient.activeChar.location.Equals(fiefToViewID)) {
            ddMeetingPlaceType.interactable = true;
            btnListCharsInMeetingPlace.interactable = true;
        }

        lblPageTitle.text = currentlyViewedFief.FiefName;

        if(currentlyViewedFief != null) {
            DisplayFiefDetails();
        }

        if(userMessageOnSceneLoad != null) {
            DisplayMessageToUser(userMessageOnSceneLoad);
            userMessageOnSceneLoad = null;
        }

        //lblDetails2.text += "\n" + protoClient.activeChar.days.ToString();
    }

    private void BtnSiegeFief()
    {
        ProtoMessage reply = SiegeCurrentFief(tclient);

        switch (reply.ResponseType)
        {
            case DisplayMessages.ErrorGenericMessageInvalid:
                {
                    break;
                }
            case DisplayMessages.ErrorGenericArmyUnidentified:
                {
                    break;
                }
            case DisplayMessages.ErrorGenericUnauthorised:
                {
                    break;
                }
            case DisplayMessages.Success:
                {
                    ProtoSiegeDisplay siege = (ProtoSiegeDisplay)reply;
                    break;
                }
        }

    }


    private void BtnSpyOnFief()
    {
        ProtoMessage reply = SpyFief(currentlyViewedFief.fiefID, protoClient.activeChar.charID, tclient);

        switch (reply.ResponseType)
        {
            case DisplayMessages.ErrorGenericMessageInvalid:
                {
                    DisplayMessageToUser("ErrorGenericMessageInvalid!");
                    break;
                }
            case DisplayMessages.ErrorGenericUnauthorised:
                {
                    DisplayMessageToUser("ErrorGenericUnauthorised!");
                    break;
                }
            case DisplayMessages.SpySuccessDetected:    // Successful but detected
                {
                    DisplayMessageToUser("SpySuccessDetected!");
                    break;
                }
            case DisplayMessages.SpySuccess:            // Successful not detected
                {
                    DisplayMessageToUser("SpySuccess!");
                    break;
                }
            case DisplayMessages.SpyFailDead:           // Not successful and spy died
                {
                    DisplayMessageToUser("SpyFailDead!");
                    break;
                }
            case DisplayMessages.SpyFailDetected:       // Not successful and detected
                {
                    DisplayMessageToUser("SpyFailDetected!");
                    break;
                }
            case DisplayMessages.SpyFail:                // Not successful but not detected
                {
                    DisplayMessageToUser("SpyFail!");
                    break;
                }
            default:
                {
                    DisplayMessageToUser("ErrorGenericMessageInvalid!");
                    break;
                }


        }

    }

    private void BtnAdjustExpenditure() {
        string[] inputValues = new string[5];
        inputValues[0] = txtTaxRate.text;
        inputValues[1] = txtOfficials.text;
        inputValues[2] = txtGarrison.text;
        inputValues[3] = txtInfrastructure.text;
        inputValues[4] = txtKeep.text;
        double[] adjustedValues;
        bool autoAdjust = true;
        ProtoMessage response;

        foreach(var value in inputValues) {
            if(!value.Equals("")) {
                autoAdjust = false;
                break;
            }
        }
        if(autoAdjust) {
            // Blank values trigger auto-adjust.
            //adjustedValues = null;
            //response = AdjustExpenditure(currentlyViewedFief.fiefID, adjustedValues, tclient);

            return; // Auto-adjust has issues server-side which cause it to never work (it always thinks there's no money to spend). 
        }
        else {
            adjustedValues = new double[5];
            for(int i = 0; i < 5; i++) {
                double newValue = 0.0f;
                try {
                    newValue = Convert.ToDouble(inputValues[i]);
                }
                catch(FormatException) {
                    // If invalid entry, use the finances original value.
                    newValue = currentlyViewedFief.keyStatsNext[i+2];
                }
                finally {
                    adjustedValues[i] = newValue;
                }
            }
            response = AdjustExpenditure(currentlyViewedFief.fiefID, adjustedValues, tclient);
        }
                
        if(response.ResponseType == DisplayMessages.FiefExpenditureAdjusted) {
            currentlyViewedFief = (ProtoFief)response;
            DisplayFiefDetails();
            DisplayMessageToUser("Finances " + response.Message + ".");
            txtTaxRate.text = "";
            txtOfficials.text = "";
            txtGarrison.text = "";
            txtInfrastructure.text = "";
            txtKeep.text = "";
        }
        else if(response.ResponseType == DisplayMessages.FiefExpenditureAdjustment) {
            DisplayMessageToUser("The fief treasury is short by " + response.MessageFields[1] + " to cover this budget.");
        }
        else {
            DisplayMessageToUser("Error: " + response.ResponseType.ToString());
        }
    }

    //void BtnAutoAdjustExpenditures() {
    //    // Blank values triggers auto-adjust.
    //    double[] adjustedValues = null;
    //    ProtoMessage response = AdjustExpenditure(currentlyViewedFief.fiefID, adjustedValues, tclient);
    //    currentlyViewedFief = (ProtoFief)response;
    //    DisplayFiefDetails();
    //    DisplayMessageToUser("Finances auto-adjusted.");
    //}

    private void BtnListCharsInMeetingPlace() {
        globalString = ddMeetingPlaceType.options[ddMeetingPlaceType.value].text.ToLower();
        viewingListSelectAction = "ListChars";
        GoToScene(SceneName.ViewingList);
    }

    private void BtnRemoveBailiff() {
        //string bailiff = currentlyViewedFief.bailiff.charID;
        string fiefName = currentlyViewedFief.FiefName;

        ProtoMessage reply = RemoveBailiff(currentlyViewedFief.fiefID, tclient);
        DisplayMessages responseType = reply.ResponseType;
        if(responseType == DisplayMessages.Success) {
            DisplayMessageToUser(currentlyViewedFief.bailiff.charID + " removed from bailiff position in " + fiefName + ".");
        }
        else if(responseType == DisplayMessages.FiefNoBailiff) {
            DisplayMessageToUser("There is no bailiff to remove in " + fiefName + ".");
        }
        else if(responseType == DisplayMessages.ErrorGenericUnauthorised) {
            DisplayMessageToUser("You are not authorised to make changes to " + fiefName + ".");
        }
        else {
            DisplayMessageToUser("ERROR: Response type: " + responseType.ToString());
        }

        DisplayFiefDetails();
    }

    private void BtnTransferFunds() {
        string strFunds = txtTransferFunds.text;
        if(!int.TryParse(strFunds, out int funds)) {
            DisplayMessageToUser("Invalid number.");
        }
        else if(funds < currentlyViewedFief.treasury) {
            DisplayMessageToUser("The treasury does not have that amount of money to transfer!");
        }
        else {
            viewingListSelectAction = "TransferFundsToFief";
            globalInt = funds;
            GoToScene(SceneName.ViewingList);
        }
    }

    private void BtnTravelTo() {
        if(protoClient.activeChar.location == fiefToViewID) {
            DisplayMessageToUser("You are already in this fief!");
            return;
        }
        string fullName = protoClient.activeChar.firstName + " " + protoClient.activeChar.familyName;
        ProtoMessage reply = TravelTo(protoClient.activeChar.charID, fiefToViewID, tclient);
        if(reply.ResponseType == DisplayMessages.Success) {
            //string message = protoClient.activeChar.firstName + " " + protoClient.activeChar.familyName;
            //if(!string.IsNullOrWhiteSpace(protoClient.activeChar.armyID)) {
            //    message += " and his army have";
            //}
            //else {
            //    message += " has";
            //}
            //message += " arrived in " + fiefNames[fiefToViewID] + ".";

            //DisplayMessageToUser(message);
            userMessageOnSceneLoad = fullName + " has arrived in " + fiefNames[fiefToViewID];
            GoToScene(SceneName.ViewFief);
        }
        else if(reply.ResponseType == DisplayMessages.CharacterDaysJourney) {
            userMessageOnSceneLoad = fullName + "only had enough days left in the season to make it as far as " + fiefNames[reply.Message];
            fiefToViewID = reply.Message;
            GoToScene(SceneName.ViewFief);
        }
        else {
            DisplayMessageToUser("ERROR: Response type: " + reply.ResponseType.ToString());
        }
    }

    private void BtnViewArmies() {
        viewingListSelectAction = "ArmiesInFief";
        GoToScene(SceneName.ViewingList);
    }

    private void BtnViewPossibleBailiffs() {
        // chars to view, male 14+
        ProtoGenericArray<ProtoCharacterOverview> possibleBailiffs = GetNPCList("Family Employ", tclient);
        //foreach(ProtoCharacterOverview character in possibleBailiffs.fields) {

        //}
        characterList = possibleBailiffs;
        viewingListSelectAction = "AssignBailiff";

        GoToScene(SceneName.ViewingList);
    }

    void Update()
    {
        
    }

    public void PillageFief()
    {
        ProtoMessage reply = PillageFief(tclient);

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
                    ProtoPillageResult result = (ProtoPillageResult)reply;
                    Debug.Log(result.treasuryLoss);
                    PillageResultOutput.GetChild(0).GetComponent<TMPro.TMP_Text>().SetText(result.fiefID);
                    PillageResultOutput.GetChild(1).GetComponent<TMPro.TMP_Text>().SetText(result.fiefName);
                    PillageResultOutput.GetChild(2).GetComponent<TMPro.TMP_Text>().SetText(result.isPillage.ToString());
                    PillageResultOutput.GetChild(3).GetComponent<TMPro.TMP_Text>().SetText(result.fiefOwner);
                    PillageResultOutput.GetChild(4).GetComponent<TMPro.TMP_Text>().SetText(result.defenderLeader);
                    PillageResultOutput.GetChild(5).GetComponent<TMPro.TMP_Text>().SetText(result.armyOwner);
                    PillageResultOutput.GetChild(6).GetComponent<TMPro.TMP_Text>().SetText(result.armyLeader);
                    PillageResultOutput.GetChild(7).GetComponent<TMPro.TMP_Text>().SetText(result.daysTaken.ToString());
                    PillageResultOutput.GetChild(8).GetComponent<TMPro.TMP_Text>().SetText(result.populationLoss.ToString());
                    PillageResultOutput.GetChild(9).GetComponent<TMPro.TMP_Text>().SetText(result.treasuryLoss.ToString());
                    PillageResultOutput.GetChild(10).GetComponent<TMPro.TMP_Text>().SetText(result.industryLoss.ToString());
                    PillageResultOutput.GetChild(11).GetComponent<TMPro.TMP_Text>().SetText(result.loyaltyLoss.ToString());
                    PillageResultOutput.GetChild(12).GetComponent<TMPro.TMP_Text>().SetText(result.fieldsLoss.ToString());
                    PillageResultOutput.GetChild(13).GetComponent<TMPro.TMP_Text>().SetText(result.baseMoneyPillaged.ToString());
                    PillageResultOutput.GetChild(14).GetComponent<TMPro.TMP_Text>().SetText(result.bonusMoneyPillaged.ToString());
                    PillageResultOutput.GetChild(15).GetComponent<TMPro.TMP_Text>().SetText(result.moneyPillagedOwner.ToString());
                    PillageResultOutput.GetChild(16).GetComponent<TMPro.TMP_Text>().SetText(result.jackpot.ToString());
                    PillageResultOutput.GetChild(17).GetComponent<TMPro.TMP_Text>().SetText(result.statureModifier.ToString());
                    DisplayMessageToUser("Success!");
                    PillagePanel.SetActive(true);
                    break;
                }
            default:
                {
                    DisplayMessageToUser("default!");
                    break;
                }
        }

    }

    void DisplayMessageToUser(string message) {
        lblMessageForUser.text = "[" + DateTime.Now.ToString("h:mm:ss tt") + "] " + message;
    }

    void DisplayFiefDetails() {
        string bailiff = (currentlyViewedFief.bailiff) != null ? currentlyViewedFief.bailiff.charName : "-";
        string status;
        switch(currentlyViewedFief.status) {
            case 'C': status = "Calm"; break;
            case 'U': status = "Unrest"; break;
            case 'R': status = "Rebellion"; break;
            default: status = "Unknown"; break;
        }

        lblDetails1.text = ""
            + "Title Holder:\t\t\t" + currentlyViewedFief.titleHolder + "\n"
            + "Owner:\t\t\t\t\t" + currentlyViewedFief.owner + "\n"
            + "Ancestral Owner:\t\t" + currentlyViewedFief.ancestralOwner.charName
            + "\nBailiff:\t\t\t\t\t" + bailiff
            + "\nRank:\t\t\t\t\t" + currentlyViewedFief.rank + "\n"
            + "Population:\t\t\t" + currentlyViewedFief.population.ToString() + "\n"
            ;

        lblDetails2.text = "Status:\t\t\t\t" + status + "\n";

        if(currentlyViewedFief.keyStatsCurrent != null) {
            lblDetails2.text += ""
                + "Troops in Fief:\t\t" + currentlyViewedFief.troops.ToString() + "\n"
                + "Militia:\t\t\t\t\t" + currentlyViewedFief.militia.ToString() + "\n"
                + "Keep Level:\t\t\t" + currentlyViewedFief.keepLevel.ToString() + "\n"
                + "Fields Level:\t\t\t" + currentlyViewedFief.fields.ToString() + "\n"
                + "Industry Level:\t\t" + currentlyViewedFief.industry.ToString() + "\n"
                ;

            // TODO: ADD INCOME + LOYALTY
            double[] stats = currentlyViewedFief.keyStatsCurrent;
            lblThisSeason.text = ""
                + "This Season\n\n"
                + currentlyViewedFief.treasury + "\n"
                + stats[1].ToString() + "\n"
                + stats[2].ToString() + "%\n"
                + "\n\n"
                + stats[3].ToString() + "\n"
                + stats[4].ToString() + "\n"
                + stats[5].ToString() + "\n"
                + stats[6].ToString() + "\n"
                + "\n\n"
                + stats[9].ToString() + "\n"
                + stats[10].ToString() + "\n"
                + "\n\n"
                + stats[11].ToString() + "\n"
                + stats[12].ToString() + "\n"
                + "\n"
                + stats[13].ToString()
                ;

            stats = currentlyViewedFief.keyStatsNext;
            lblNextSeason.text = ""
                + "Next Season\n\n"
                + currentlyViewedFief.treasury + "\n"
                + stats[1].ToString() + "\n"
                + stats[2].ToString() + "%\n"
                + "\n\n"
                + stats[3].ToString() + "\n"
                + stats[4].ToString() + "\n"
                + stats[5].ToString() + "\n"
                + stats[6].ToString() + "\n"
                + "\n\n"
                + stats[9].ToString() + "\n"
                + stats[10].ToString() + "\n"
                + "\n\n"
                + stats[11].ToString() + "\n"
                + stats[12].ToString() + "\n"
                + "\n"
                + stats[13].ToString()
                ;
        }
    }
}
