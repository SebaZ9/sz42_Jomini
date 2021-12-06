using ProtoMessageClient;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewSiege : Controller
{

    private string SiegeID;
    private ProtoFief currentlyViewedFief;

    public Text pageTitle;
    public Text lblMessageForUser;

    public TMPro.TMP_Text _SiegeID;
    public TMPro.TMP_Text _StartYear;
    public TMPro.TMP_Text _StartSeason;
    public TMPro.TMP_Text _BesiegingPlayer;
    public TMPro.TMP_Text _BesiegingArmy;
    public TMPro.TMP_Text _StartKeepLevel;
    public TMPro.TMP_Text _CasualitiesAttacker;
    public TMPro.TMP_Text _EndDate;
    public TMPro.TMP_Text _Days;
    public TMPro.TMP_Text _TotalDays;
    public TMPro.TMP_Text _BesiegedFief;
    public TMPro.TMP_Text _DefendingPlayer;
    public TMPro.TMP_Text _DefenderGarrison;
    public TMPro.TMP_Text _KeepLevel;
    public TMPro.TMP_Text _CasualtiesDefender;
    public TMPro.TMP_Text _DefenderAdditional;

    // Start is called before the first frame update
    void Start()
    {
        ProtoMessage reply = GetFiefDetails(fiefToViewID, tclient);
        if (reply.ResponseType == DisplayMessages.Success)
        {
            currentlyViewedFief = (ProtoFief)reply;
            SiegeID = currentlyViewedFief.siege;
            if (SiegeID == "" || SiegeID == null) GoToScene(SceneName.ViewFief);
        }
        else
        {
            GoToScene(SceneName.ViewFief);
        }

        ProtoMessage protoReply = ViewSiege(SiegeID, tclient);
        ProcessProtoSiegeDisplay(protoReply);
    }

    public void RoundStorm()
    {
        ProtoMessage protoReply = SiegeRoundStorm(SiegeID, tclient);
        ProcessProtoSiegeDisplay(protoReply);
    }

    public void RoundReduction()
    {
        ProtoMessage protoReply = SiegeRoundReduction(SiegeID, tclient);
        ProcessProtoSiegeDisplay(protoReply);
    }

    public void RoundNegotiate()
    {
        ProtoMessage protoReply = SiegeRoundNegotiate(SiegeID, tclient);
        ProcessProtoSiegeDisplay(protoReply);
    }

    public void EndThisSiege()
    {
        ProtoMessage protoReply = EndSiege(SiegeID, tclient);
        ProcessProtoSiegeDisplay(protoReply);
    }

    private void ProcessProtoSiegeDisplay(ProtoMessage protoSiegeDisplay)
    {
        Debug.Log($"CURRENT DISPLAY MESSAGE: {protoSiegeDisplay.ResponseType}");
        switch (protoSiegeDisplay.ResponseType)
        {
            case DisplayMessages.ErrorGenericMessageInvalid:
                {
                    DisplayMessageToUser("ErrorGenericMessageInvalid.");
                    break;
                }
            case DisplayMessages.ErrorGenericUnauthorised:
                {
                    DisplayMessageToUser("ErrorGenericUnauthorised.");
                    break;
                }
            case DisplayMessages.SiegeErrorDays:
                {
                    DisplayMessageToUser("Not enough days to perform action!");
                    break;
                }
            case DisplayMessages.Success:
            case DisplayMessages.None:
                {
                    DisplayMessageToUser("Successfuly updated siege window.");
                    pageTitle.text = "Siege on " + currentlyViewedFief.FiefName;
                    ProtoSiegeDisplay siegeDisplay = (ProtoSiegeDisplay)protoSiegeDisplay;

                    _SiegeID.SetText(siegeDisplay.siegeID);
                    _StartYear.SetText(siegeDisplay.startYear.ToString());
                    _StartSeason.SetText(SeasonToString(siegeDisplay.startSeason));
                    _BesiegingPlayer.SetText(siegeDisplay.besiegingPlayer);
                    _BesiegingArmy.SetText(siegeDisplay.besiegerArmy);
                    _StartKeepLevel.SetText(siegeDisplay.startKeepLevel.ToString());
                    _CasualitiesAttacker.SetText(siegeDisplay.casualtiesAttacker.ToString());
                    _EndDate.SetText(siegeDisplay.endDate);
                    _Days.SetText(siegeDisplay.days.ToString());
                    _TotalDays.SetText(siegeDisplay.totalDays.ToString());
                    _BesiegedFief.SetText(siegeDisplay.besiegedFief);
                    _DefendingPlayer.SetText(siegeDisplay.defendingPlayer);
                    _DefenderGarrison.SetText(siegeDisplay.defenderGarrison);
                    _KeepLevel.SetText(siegeDisplay.keepLevel.ToString());
                    _CasualtiesDefender.SetText(siegeDisplay.casualtiesDefender.ToString());
                    _DefenderAdditional.SetText(siegeDisplay.defenderAdditional);


                    break;
                }
        }
    }


    void DisplayMessageToUser(string message)
    {
        lblMessageForUser.text = "[" + DateTime.Now.ToString("h:mm:ss tt") + "] " + message;
    }

}
