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

    private ProtoArmy currentlyViewedArmy;

    // Start is called before the first frame update
    public void Start()
    {
        btnAttackArmy.onClick.AddListener(BtnAttackArmy);
        lblMessageForUser.text = "";
        btnAttackArmy.interactable = false;

        ProtoMessage reply = GetArmyDetails(armyToViewID, tclient);
        if(reply.ResponseType == DisplayMessages.Success) {
            currentlyViewedArmy = (ProtoArmy)reply;
            DisplayArmyDetails();

            // Can't attack if you don't have an army.
            if(!string.IsNullOrWhiteSpace(protoClient.activeChar.armyID)) {
                // Can't attack your own armies.
                if(!currentlyViewedArmy.ownerID.Equals(protoClient.playerChar.charID)) {
                    if(currentlyViewedArmy.location.Equals(protoClient.playerChar.location)) {
                        btnAttackArmy.interactable = true;
                    }
                }
            }
        }
        else {
            DisplayMessageToUser("ERROR: Response type: " + reply.ResponseType.ToString());
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
