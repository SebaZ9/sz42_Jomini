using System;
using System.Collections.Generic;
using System.IO;
using RiakClient;
using Lidgren.Network;
namespace JominiEngine
{


    /// <summary>
    /// enum representing all valid actions in the gane
    /// </summary>
    public enum Actions {
        Update = 0, LogIn, UseChar, GetPlayers, ViewChar, ViewArmy, GetNPCList, HireNPC, FireNPC, TravelTo, MoveCharacter, ViewFief, ViewMyFiefs, AppointBailiff, RemoveBailiff, BarCharacters, BarNationalities, UnbarCharacters, UnbarNationalities, GrantFiefTitle, AdjustExpenditure, TransferFunds,
        TransferFundsToPlayer, EnterExitKeep, ListCharsInMeetingPlace, TakeThisRoute, Camp, AddRemoveEntourage, ProposeMarriage, AcceptRejectProposal, RejectProposal, AppointHeir, TryForChild, RecruitTroops, MaintainArmy, AppointLeader, DropOffTroops,
        ListDetachments, ListArmies, PickUpTroops, PillageFief, BesiegeFief, AdjustCombatValues, ExamineArmiesInFief, Attack, ViewJournalEntries, ViewJournalEntry, SiegeRoundReduction, SiegeRoundStorm, SiegeRoundNegotiate, SiegeList, ViewSiege, EndSiege, DisbandArmy, SpyFief, SpyCharacter, SpyArmy, Kidnap, ViewCaptives, ViewCaptive, RansomCaptive, ReleaseCaptive, ExecuteCaptive, RespondRansom, SeasonUpdate,
        GetTravelCost, GetAvailableTravelDirections, LoadScenario, ViewWorldMap, GetProvince
    }
    /// <summary>
    /// enum representing all strings that may be sent to a client,
    ///  mapped to string from enum on client side
    /// </summary>
    public enum DisplayMessages {
        /// <summary>
        /// Default message; used only when nothing to display
        /// </summary>
        None = 0,
        /// <summary>
        /// Indicates action was successful
        /// </summary>
        Success, Error, Armies, Fiefs, Characters, JournalEntries, ArmyMaintainInsufficientFunds, ArmyMaintainCost, ArmyMaintainConfirm, ArmyMaintainedAlready, JournalProposal, JournalProposalReply, JournalMarriage, ChallengeKingSuccess, ChallengeKingFail, ChallengeProvinceSuccess, ChallengeProvinceFail, newEvent,
        SwitchPlayerErrorNoID, SwitchPlayerErrorIDInvalid, ChallengeErrorExists, SiegeNegotiateSuccess, SiegeNegotiateFail, SiegeStormSuccess, SiegeStormFail, SiegeEndDefault, SiegeErrorDays, SiegeRaised, SiegeReduction, ArmyMove,
        ArmyAttritionDebug, ArmyDetachmentArrayWrongLength, ArmyDetachmentNotEnoughTroops, ArmyDetachmentNotSelected, ArmyRetreat, ArmyDisband, ErrorGenericNotEnoughDays, ErrorGenericPoorOrganisation, ErrorGenericUnidentifiedRecipient, ArmyNoLeader, ArmyBesieged,
        ArmyAttackSelf, ArmyPickupsDenied, ArmyPickupsNotEnoughDays, BattleBringSuccess, BattleBringFail, BattleResults, ErrorGenericNotInSameFief, BirthAlreadyPregnant, BirthSiegeSeparation, BirthNotMarried, CharacterMarriageDeath, CharacterDeath, CharacterDeathNoHeir, CharacterEnterArmy,
        CharacterAlreadyArmy, CharacterNationalityBarred, CharacterBarred, CharacterRoyalGiftPlayer, CharacterRoyalGiftSelf, CharacterNotMale, CharacterNotOfAge, CharacterLeaderLocation, CharacterLeadingArmy, GetProvince,
        /// <summary>
        /// Character does not have enough days to make this journey, so their destination has been added to a Go-To list
        /// </summary>
        CharacterDaysJourney, CharacterSpousePregnant, CharacterSpouseNotPregnant, CharacterSpouseNeverPregnant,
        CharacterBirthOK, CharacterBirthChildDead, CharacterBirthMumDead, CharacterBirthAllDead, RankTitleTransfer, CharacterCombatInjury, CharacterProposalMan, CharacterProposalUnderage, CharacterProposalEngaged, CharacterProposalMarried, CharacterProposalFamily, CharacterProposalIncest, CharacterProposalAlready, CharacterRemovedFromEntourage, CharacterCamp,
        CharacterCampAttrition, CharacterBailiffDuty,
        /// <summary>
        /// Character must end siege before moving
        /// </summary>
        CharacterMoveEndSiege, MoveCancelled, CharacterInvalidMovement, ErrorGenericFiefUnidentified, CharacterHireNotEmployable, CharacterFireNotEmployee, CharacterOfferLow, CharacterOfferHigh, CharacterOfferOk, CharacterOfferAlmost, CharacterOfferHaggle, CharacterBarredKeep, CharacterRecruitOwn, CharacterRecruitAlready, CharacterLoyaltyLanguage, ErrorGenericInsufficientFunds, CharacterRecruitSiege, CharacterRecruitRebellion, RecruitCancelled,
        CharacterTransferTitle, CharacterTitleOwner, CharacterTitleHighest, CharacterTitleKing, CharacterTitleAncestral, CharacterHeir, PillageInitiateSiege, PillageRetreat, PillageDays, PillageOwnFief, PillageUnderSiege, PillageSiegeAlready, PillageAlready, PillageArmyDefeat, PillageSiegeRebellion, FiefExpenditureAdjustment, FiefExpenditureAdjusted, FiefStatus, FiefOwnershipHome,
        FiefOwnershipNewHome, FiefOwnershipNoFiefs, FiefChangeOwnership, FiefQuellRebellionFail, FiefEjectCharacter, FiefNoCaptives, ProvinceAlreadyOwn, KingdomAlreadyKing, KingdomOwnershipChallenge, ProvinceOwnershipChallenge, ErrorGenericCharacterUnidentified, ErrorGenericUnauthorised, ErrorGenericMessageInvalid, ErrorGenericTooFarFromFief, FiefNoBailiff, FiefCouldNotBar, FiefCouldNotUnbar,
        ErrorGenericBarOwnNationality, ErrorGenericPositiveInteger, GenericReceivedFunds, ErrorGenericNoHomeFief, CharacterRecruitInsufficientFunds, CharacterRecruitOk, SiegeNotBesieger, JournalEntryUnrecognised, JournalEntryNotProposal, ErrorGenericArmyUnidentified, ErrorGenericSiegeUnidentified, ErrorSpyDead, ErrorSpyCaptive, ErrorSpyOwn, SpyChance, SpySuccess, SpyFail, SpySuccessDetected, SpyFailDetected, SpyFailDead, SpyCancelled, EnemySpySuccess, EnemySpyFail, EnemySpyKilled, CharacterHeldCaptive, RansomReceived, RansomPaid, RansonDenied, RansomRepliedAlready, RansomCaptiveDead, RansomAlready, NotCaptive, EntryNotRansom, KidnapOwnCharacter, KidnapDead, KidnapNoPlayer, KidnapSuccess, KidnapSuccessDetected, KidnapFailDetected, KidnapFailDead, KidnapFail, EnemyKidnapSuccess, EnemyKidnapSuccessDetected, EnemyKidnapFail, EnemyKidnapKilled, CharacterExecuted, CharacterReleased, LogInSuccess, LogInFail, YouDied, YouDiedNoHeir, CharacterIsDead, Timeout
    }

    /// <summary>
    /// Class storing any required static variables for server-side
    /// </summary>
    public static class Globals_Server
    {

        

        /// <summary>
        /// Holds the usernames and Client objects of all players
        /// </summary>
        public static Dictionary<string, Client> Clients = new Dictionary<string, Client>();
        /// <summary>
        /// Holds all usernames/playerIDs
        /// </summary>
        public static List<string> client_keys = new List<string>();
        /// <summary>
        /// Holds target RiakCluster 
        /// </summary>
        public static IRiakEndPoint rCluster;
        /// <summary>
        /// Holds RiakClient to communicate with RiakCluster
        /// </summary>
        public static IRiakClient rClient;
        /// <summary>
        /// Holds next value for game ID
        /// </summary>
        public static uint newGameID = 1;
        /// <summary>
        /// Holds combat values for different troop types and nationalities
        /// Key = nationality & Value = combat value for knights, menAtArms, lightCavalry, longbowmen, crossbowmen, foot, rabble
        /// </summary>
        public static Dictionary<string, uint[]> combatValues = new Dictionary<string, uint[]>();
        /// <summary>
        /// Dictionary mapping two troop types to a value representing one's effectiveness against the other
        /// </summary>
        public static Dictionary<Tuple<uint,uint>, double> troopTypeAdvantages = new Dictionary<Tuple<uint,uint>,double>();
        /// <summary>
        /// Holds recruitment ratios for different troop types and nationalities
        /// Key = nationality & Value = % ratio for knights, menAtArms, lightCavalry, longbowmen, crossbowmen, foot, rabble
        /// </summary>
        public static Dictionary<string, double[]> recruitRatios = new Dictionary<string, double[]>();
        /// <summary>
        /// Holds probabilities for battle occurring at certain combat odds under certain conditions
        /// Key = 'odds', 'battle', 'pillage'
        /// Value = percentage likelihood of battle occurring
        /// </summary>
        public static Dictionary<string, double[]> battleProbabilities = new Dictionary<string, double[]>();
        /// <summary>
        /// Holds type of game  (sets victory conditions)
        /// </summary>
        public static Dictionary<uint, string> gameTypes = new Dictionary<uint, string>();
        /// <summary>
        /// Holds NetServer used for hosting game
        /// </summary>
        public static NetServer server;
        /// <summary>
        /// Gets the next available newGameID, then increments it
        /// </summary>
        /// <returns>string containing newGameID</returns>
        public static string GetNextGameID()
        {
            string gameID = "Game_" + newGameID;
            newGameID++;
            return gameID;
        }
        /// <summary>
        /// StreamWriter for writing output to a file
        /// </summary>
		public static StreamWriter LogFile;
        /// <summary>
        /// Writes any errors encountered to a logfile
        /// </summary>
        /// <param name="error">The details of the error</param>
        public static void logError(String error)
        {
            LogFile.WriteLine("Run-time error: " + error);
#if DEBUG
			Console.WriteLine ("Run-time error: " + error);
#endif
        }

        /// <summary>
        /// Write an event to the log file
        /// </summary>
        /// <param name="eventDetails">The details of the event</param>
        public static void logEvent(String eventDetails)
        {
            LogFile.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss") + "] " + eventDetails);
#if DEBUG
            Console.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss") + "] " + eventDetails);
#endif
        }
    }
}
