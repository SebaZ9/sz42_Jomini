using ProtoMessageClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ViewJournalEntry : Controller
{
    [SerializeField] private Text lblPageTitle;
    [SerializeField] private Text lblMessageForUser;
    [SerializeField] private Text lblJournalEntryInfo;

    //[SerializeField] private Button btnAttackArmy;

    private ProtoJournalEntry currentlyViewedJournalEntry;

    // Start is called before the first frame update
    public void Start()
    {
        //btnAttackArmy.onClick.AddListener(BtnAttackArmy);
        lblMessageForUser.text = "";

        //ProtoMessage reply = GetArmyDetails(armyToViewID, tclient);
        //if(reply.ResponseType == DisplayMessages.Success) {
        //    currentlyViewedJournalEntry = (ProtoArmy)reply;
        //}
        //else {
        //    DisplayMessageToUser("ERROR: Response type: " + reply.ResponseType.ToString());
        //}

        foreach(var jEntry in journalList.fields) {
            if(jEntry.jEntryID == journalEntryToViewID) {
                currentlyViewedJournalEntry = jEntry;
            }
        }
        
        DisplayJournalEntryDetails();
    }

    void DisplayMessageToUser(string message) {
        lblMessageForUser.text = "[" + DateTime.Now.ToString("h:mm:ss tt") + "] " + message;
    }

    private void DisplayJournalEntryDetails() {
        string jEntryType = currentlyViewedJournalEntry.type;
        jEntryType = jEntryType.First().ToString().ToUpper() + jEntryType.Substring(1);
        lblPageTitle.text = jEntryType;

        //uint totalTroops = 0;
        //foreach(uint troops in currentlyViewedJournalEntry.troops) {
        //    totalTroops += troops;
        //}

        string location = string.IsNullOrWhiteSpace(currentlyViewedJournalEntry.location) ? "Unknown" : fiefNames[currentlyViewedJournalEntry.location];
        lblJournalEntryInfo.text = ""
            + "Date of Event: " + SeasonToString(currentlyViewedJournalEntry.season) + " " + currentlyViewedJournalEntry.year
            + "\nLocation: " + currentlyViewedJournalEntry.location
            + "\n"
            ;

        if(currentlyViewedJournalEntry.eventDetails is ProtoBattle) {
            var battleEvent = currentlyViewedJournalEntry.eventDetails as ProtoBattle;

            string victor = battleEvent.attackerVictorious ? battleEvent.attackerLeader : battleEvent.defenderLeader;
            string noBattle = battleEvent.battleTookPlace ? "" : "\nThe defenders retreated before battle could be met";
            string battleType = "Unknown";
            switch(battleEvent.circumstance) {
                case 0: { battleType = "Normal"; break; }
                case 1: { battleType = "Pillage"; break; }
                case 2: { battleType = "Siege"; break; }
            }

            lblJournalEntryInfo.text += ""
                + "\nAttacker: " + battleEvent.attackerOwner
                + "\nDefender: " + battleEvent.defenderOwner
                + "\n"
                + "\nAttacker: " + battleEvent.attackerLeader
                + "\nDefender: " + battleEvent.defenderLeader
                + "\n"
                + "\nBattle Type: " + battleType
                + noBattle
                + "\nVictor: " + victor
                + "\n"
                + "\nAttacker Losses: " + battleEvent.attackerCasualties
                + "\nDefender Losses: " + battleEvent.defenderCasualties
                + "\n"
                ;

            if(battleEvent.deaths != null) {
                lblJournalEntryInfo.text += "\nNotable Deaths:";
                foreach(var character in battleEvent.deaths) {
                    lblJournalEntryInfo.text += "\n\t" + character;
                }
                lblJournalEntryInfo.text += "\n";
            }

            if(battleEvent.retreatedArmies != null) {
                lblJournalEntryInfo.text += "\nRetreated Armies:";
                foreach(var army in battleEvent.retreatedArmies) {
                    lblJournalEntryInfo.text += "\n\t" + army;
                }
                lblJournalEntryInfo.text += "\n";
            }

            if(battleEvent.disbandedArmies != null) {
                lblJournalEntryInfo.text += "\nDisbanded Armies:";
                foreach(var army in battleEvent.disbandedArmies) {
                    lblJournalEntryInfo.text += "\n\t" + army;
                }
                lblJournalEntryInfo.text += "\n";
            }
        }

        if(currentlyViewedJournalEntry.type.Contains("Death") || currentlyViewedJournalEntry.type.Contains("Death")) {
            lblPageTitle.text = currentlyViewedJournalEntry.type;
            string[] death = currentlyViewedJournalEntry.eventDetails.MessageFields;

            lblJournalEntryInfo.text += ""
                + "Deceased: " + death[0] // Name
                + "\nRole: " + death[1]
                + "\nCause of Death: " + death[2]
                ;
            if(death[3] != null) {
                lblJournalEntryInfo.text += "\nSucceeded by: " + death[3];
            }
        }

    }
}
