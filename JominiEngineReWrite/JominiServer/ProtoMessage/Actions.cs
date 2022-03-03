using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtoMessageClient
{
    /// <summary>
    /// enum representing all valid actions in the gane
    /// </summary>
    public enum Actions
    {
        Update, LogIn, UseChar, GetPlayers,

        ViewChar, ViewArmy,  GetNPCList, HireNPC,
        FireNPC, TravelTo, MoveCharacter, ViewFief, ViewMyFiefs,
        AppointBailiff, RemoveBailiff, BarCharacters, BarNationalities,
        UnbarCharacters, UnbarNationalities, GrantFiefTitle, AdjustExpenditure,
        TransferFunds, TransferFundsToPlayer,  EnterExitKeep, ListCharsInMeetingPlace,
        TakeThisRoute, Camp, AddRemoveEntourage, ProposeMarriage,
        AcceptRejectProposal, RejectProposal, AppointHeir, TryForChild,
        RecruitTroops, MaintainArmy,  AppointLeader, DropOffTroops,
        ListDetachments, ListArmies, PickUpTroops, PillageFief,
        BesiegeFief, AdjustCombatValues, ExamineArmiesInFief, Attack,
        ViewJournalEntries, ViewJournalEntry, SiegeRoundReduction, SiegeRoundStorm,
        SiegeRoundNegotiate, SiegeList, ViewSiege, EndSiege,
        DisbandArmy, SpyFief, SpyCharacter, SpyArmy,
        Kidnap, ViewCaptives, ViewCaptive, RansomCaptive,
        ReleaseCaptive, ExecuteCaptive, RespondRansom, 
        
        SeasonUpdate, GetTravelCost, GetAvailableTravelDirections, LoadScenario,
        ViewWorldMap, GetProvince
    }

}
