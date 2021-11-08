using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using ProtoMessageClient;

namespace JominiAI
{
	/// <summary>
	///     Lists the actions that can be used by the agents.
	/// </summary>
	public enum MainActions
	{
		None,

		ArmyRecruit,
		ArmyMaintain,
		ArmyPickUp,
		ArmyDropOff,
		ArmyDisband,
		ArmyAppointLeader,
		ArmyPillageCurrentFief,
		ArmyAdjustCombatValues,
		ArmyAttack,

		SiegeBesiegeCurrentFief,
		SiegeStorm,
		SiegeNegotiation,
		SiegeReduction,
		SiegeEnd,

		NPCmoveDirection,
		NPCmoveToFief,
		NPCmarryPC,
		NPCmarryChild,
		NPCacceptRejectProposal,
		NPCtryForChild,
		NPChire,
		NPCfire,
		NPCbarCharacters,
		NPCunbarCharacters,
		NPCbarNationalities,
		NPCunbarNationalities,
		NPCtryEnterKeep,
		NPCexitKeep,
		NPCcamp,
		NPCaddToEntourage,
		NPCremoveFromEntourage,
		NPCappointHeir,
		NPCappointBaillif,
		NPCremoveBaillif,

		FiefAutoAdjustExpenditure,
		FiefTransferFunds,
		FiefTransferFundsToPlayer,
		FiefGrantFiefTitle,

		SpyFief,
		SpyCharacter,
		SpyArmy,

		KidnapCharacter, // Not working on server side
		KidnapRansomCaptive, // Needs KidnapCharacter to work
		KidnapReleaseCaptive, // Needs KidnapCharacter to work
		KidnapExecuteCaptive, // Needs KidnapCharacter to work
		KidnapRespondRansom, // Needs KidnapCharacter to work
	}

	/// <summary>
	///		Contain the functions and variables used by most agents
	/// </summary>
	public abstract class Agent : Player
	{
		/// <summary>
		///     Return the next action for the agent to execute. Each agent will use a different algorithm to find the action.
		/// </summary>
		/// <returns>The next action to execute</returns>
		public abstract MainActions findNextAction(GameState gameState, out string[] arguments);

		protected List<String> nationalities = new List<String>() { "Oth", "Eng", "Fr", "Sco" };

		private HashSet<MainActions> FailedActions = new HashSet<MainActions>(); // When failing to execute an action, it is added to this set. Is reinitialized when the agent succesfuly execute an action.
		protected HashSet<string> fiefSpiedThisSeasonIDs = new HashSet<string>();
		protected HashSet<string> charSpiedThisSeasonIDs = new HashSet<string>();
		protected HashSet<string> armySpiedThisSeasonIDs = new HashSet<string>();
		protected HashSet<string> fiefVisitedThisSeasonIDs = new HashSet<string>();
		protected int nbConsecutiveNPCmoveDirections = 0;
		protected MoveDirections lastNPCmoveDirection = MoveDirections.E; // The first value assigned to it doesn't matter
		protected string lastOfferToNPCid = null;
		protected uint lastOfferToNPCamount = 0;
		protected HashSet<string> expenditureAdjustedThisSeasonFiefIDs = new HashSet<string>();

		protected HashSet<string> charToBarIDs = new HashSet<string>();
		protected HashSet<string> charToUnbarIDs = new HashSet<string>();
		protected HashSet<string> charToReleaseIDs = new HashSet<string>();
		protected HashSet<string> myRansomedCaptiveIDs = new HashSet<string>();

		protected Dictionary<string, double> dayCostToTravelToKnownFiefs = new Dictionary<string, double>(); // Contains the day cost to travel to any known fief
		protected Dictionary<MoveDirections, double> dayCostToTravelToAdjacentFiefs = new Dictionary<MoveDirections, double>(); // Contains the day cost to travel to any adjecent fief (the fief may not be known)

		private double estimationNbDaysUsed = 0; // For testing purpose

		protected Dictionary<MoveDirections, MoveDirections> oppositeDirections = new Dictionary<MoveDirections, MoveDirections>{
			{ MoveDirections.E, MoveDirections.W},
			{ MoveDirections.W, MoveDirections.E},
			{ MoveDirections.SE, MoveDirections.NW},
			{ MoveDirections.SW, MoveDirections.NE},
			{ MoveDirections.NE, MoveDirections.SW},
			{ MoveDirections.NW, MoveDirections.SE} };

		/// <summary>
		///		Filled with the maximum possible day cost for each action.
		/// </summary>
		protected Dictionary<MainActions, double> maxDaysToDoAction = new Dictionary<MainActions, double>{
			{MainActions.ArmyRecruit, 5},  
			{MainActions.ArmyMaintain, 0},  
			{MainActions.ArmyPickUp, 30},  
			{MainActions.ArmyDropOff, 30},   
			{MainActions.ArmyDisband, 0},  
			{MainActions.ArmyAppointLeader, 0}, 
			{MainActions.ArmyPillageCurrentFief, 15},  
			{MainActions.ArmyAdjustCombatValues, 0},
			{MainActions.ArmyAttack, 1},

			{MainActions.SiegeBesiegeCurrentFief, 0},  
			{MainActions.SiegeStorm, 10},  
			{MainActions.SiegeNegotiation, 10},  
			{MainActions.SiegeReduction, 10},  
			{MainActions.SiegeEnd, 0},  

			{MainActions.NPCmoveDirection, 0}, // Calculed in function of the location as it changes a lot depending on the terrain type
			{MainActions.NPCmoveToFief, 0}, // Calculed in function of the location as it changes a lot depending on the route
			{MainActions.NPCmarryPC, 0},  
			{MainActions.NPCmarryChild, 0},  
			{MainActions.NPCacceptRejectProposal, 0},  
			{MainActions.NPCtryForChild, 1},
			{MainActions.NPChire, 0},
			{MainActions.NPCfire, 0},
			{MainActions.NPCbarCharacters, 0},
			{MainActions.NPCunbarCharacters, 0},
			{MainActions.NPCbarNationalities, 0},
			{MainActions.NPCunbarNationalities, 0},
			{MainActions.NPCtryEnterKeep, 0},
			{MainActions.NPCexitKeep, 0},
			{MainActions.NPCcamp, 1}, // *nb days camped* => Minimum 1
			{MainActions.NPCaddToEntourage, 0},  
			{MainActions.NPCremoveFromEntourage, 0},  
			{MainActions.NPCappointHeir, 0},  
			{MainActions.NPCappointBaillif, 0},  
			{MainActions.NPCremoveBaillif, 0},  
			
			{MainActions.FiefAutoAdjustExpenditure, 0},  
			{MainActions.FiefTransferFunds, 0},  
			{MainActions.FiefTransferFundsToPlayer, 0},  
			{MainActions.FiefGrantFiefTitle, 0},  

			{MainActions.SpyFief, 10},  
			{MainActions.SpyCharacter, 10},  
			{MainActions.SpyArmy, 10},  

			{MainActions.KidnapCharacter, 10},  
			{MainActions.KidnapRansomCaptive, 0},  
			{MainActions.KidnapReleaseCaptive, 0},
			{MainActions.KidnapExecuteCaptive, 0},
			{MainActions.KidnapRespondRansom, 0},
		};

		public Agent(string username, string password) : base(username, password)
		{
			
		}

		/// <summary>
		///     The agent will chose and execute actions for a given number of days.
		/// </summary>
		/// <param name="withBreaks">Indicates if there are breaks after each action execution</param>
		/// <param name="maxEstimationNbDays">maximum number of days after. If the agent reaches this limit then he will stop</param>
		/// <param name="firstGS">The first GameState before the agent performs any action</param>
		/// <returns>The final gameState</returns>
		public GameState play(bool withBreaks, out GameState firstGS, int maxEstimationNbDays = 0)
		{
			firstGS = new GameState();

			List<MainActions> actionsPlayed = new List<MainActions>();
			List<string[]> actionsPlayedArguments = new List<string[]>();
			GameState gameState = new GameState();
			bool firstGSinitialized = false;
			while (true)
			{
				gameState = ObtainCurrentGSandUpdateAgentLists(gameState);
				if (!firstGSinitialized)
                {
					firstGS = gameState.deepCopy();
					firstGSinitialized = true;
				}
				
				Console.WriteLine("\n<----------------------------------------------->");
				RedProtoPlayerCharacter currentPC = gameState.myPC;
				RedProtoFief currenFief = gameState.getCurrentFief();
				RedProtoFief homeFief = gameState.getHomeFief();
				if(maxEstimationNbDays > 0)
					Console.WriteLine("estimationNbDaysUsed= " + estimationNbDaysUsed);
				Console.WriteLine("currentPCid= " + currentPCid);
				Console.WriteLine("currentPC.days= " + currentPC.days);
				Console.WriteLine("currenFief.fiefID= " + currenFief.fiefID);
				Console.WriteLine("currenFief.ownerID= " + currenFief.ownerID);
				Console.WriteLine("homeFief.treasury= " + homeFief.treasury);
				Console.WriteLine("Total Nb troops= " + calculMyTotalNbTroops(gameState));
				uint nbTroops = gameState.tryGetMainArmy(out RedProtoArmy mainArmy) ? Tools.CalculSumUintArray(mainArmy.troops) : 0;
				Console.WriteLine("Nb troops in mainArmy= " + nbTroops);
				Console.WriteLine("enemyPCids.Count()= " + gameState.enemyPCids.Count());
				Console.WriteLine("");

				if (withBreaks)
				{
					Console.WriteLine("---> Press a key to find and execute the next action");
					Console.ReadKey();
					Console.WriteLine("");
				}

				gameState = playOneAction(gameState, out MainActions actionChosen, out string[] actionArguments, out bool executionSuccess, out bool endOfSeason, maxEstimationNbDays);

                if (endOfSeason)
                {
					Console.WriteLine("No more actions available for this season, here is the list of executed actions: ");
					for (int i = 0; i < actionsPlayed.Count; i++)
					{
						Console.Write("\n   " + actionsPlayed[i] + "(");
						foreach (string argument in actionsPlayedArguments[i])
							Console.Write(" '" + argument + "'");
						Console.Write(" )");
					}
					Console.WriteLine("");
					return gameState;
				}
                else
                {
					Console.Write("\nThe chosen action was '" + actionChosen.ToString() + "' with the arguments: ");
					foreach (String arg in actionArguments)
						Console.Write("'" + arg + "' ");
					Console.WriteLine("");

					if (executionSuccess)
					{
						Console.WriteLine("Execution status: Success");
						actionsPlayed.Add(actionChosen);
						actionsPlayedArguments.Add(actionArguments);
					}
					else
					{
						Console.WriteLine("Execution status: Failure");
						Console.WriteLine("---> Press a key to continue");
						Console.ReadKey();
						Console.WriteLine("");
					}
				}
			}
		}

		/// <summary>
		///     
		/// </summary>
		/// <param name=""></param>
		/// <returns>The current GameState</returns>
		public GameState playOneAction(GameState gameState, out MainActions actionChosen, out string[] actionArguments, out bool executionSuccess, out bool endOfSeason, double maxEstimationNbDays = 0)
        {
			MainActions nextAction = findNextAction(gameState, out string[] arguments);

			actionChosen = nextAction;
			actionArguments = arguments;
			executionSuccess = true;
			endOfSeason = false;

			if (nextAction == MainActions.None || (maxEstimationNbDays > 0 && estimationNbDaysUsed >= maxEstimationNbDays)) // End of current season
				endOfSeason = true;
			else
			{
				if (ExecuteAction(nextAction, arguments, gameState, out gameState))
				{
					executionSuccess = true;
					FailedActions = new HashSet<MainActions>();
				}
                else
                {
					executionSuccess = false;
					FailedActions.Add(nextAction);
				}
			}
			return gameState;
		}

		/// <summary>
		///     Calls the ObtainCurrentGameState() function and updates the necessary class variables
		/// </summary>
		/// <param name="previousGS">Previous GameState</param>
		/// <returns>The current GameState</returns>
		public GameState ObtainCurrentGSandUpdateAgentLists(GameState previousGS = new GameState())
		{
			GameState currentGS = ObtainCurrentGameState(previousGS);
			RedProtoFief currentFief = currentGS.getCurrentFief();


			// Update availableDirections list
			dayCostToTravelToAdjacentFiefs = new Dictionary<MoveDirections, double>();
			string[] availableTravelDirections = GetAvailableTravelDirections().MessageFields;
			if(availableTravelDirections != null)
				foreach (string availableTravelDirection in availableTravelDirections)
				{
					if (!Enum.TryParse(availableTravelDirection, out MoveDirections moveDirection))
						throw new Exception("Couldn't parse the string '" + availableTravelDirection + "' to create a MainActions");
					double travelDayCost = double.Parse(getTravelDayCost("", availableTravelDirection).Message);
					dayCostToTravelToAdjacentFiefs.Add(moveDirection, travelDayCost);
				}

			// Add new ennemies
			foreach (RedProtoSiegeDisplay siegeAgainstMe in currentGS.siegeOnMyFiefs)
				if (!currentGS.enemyPCids.Contains(siegeAgainstMe.besiegingPlayer))
					AddEnemy(currentGS, siegeAgainstMe.besiegingPlayer, out currentGS);
			foreach (RedProtoJournalEntry unreadJournalEntry in currentGS.unreadJournalEntries)
				if (unreadJournalEntry.type.Equals("pillage"))
					if (unreadJournalEntry.personaeIds[0].Equals(currentPCid))
						AddEnemy(currentGS, unreadJournalEntry.personaeIds[1], out currentGS);

			// Remove ransomed captives that are no longer captives
			foreach (string myRansomedCaptiveID in myRansomedCaptiveIDs)
				if(! currentGS.myCaptiveIDs.Contains(myRansomedCaptiveID))
					myRansomedCaptiveIDs.Remove(myRansomedCaptiveID);

			// charToBarIDs update
			HashSet<string> newCharToBarIDs = new HashSet<string>(charToBarIDs);
			foreach (string charToBarID in charToBarIDs)
				if (!CanBarCharFromOneOfMyFiefs(currentGS, charToBarID, out RedProtoFief redProtoFief))
					newCharToBarIDs.Remove(charToBarID); // Remove
			charToBarIDs = newCharToBarIDs;
			foreach (string enemyPCid in currentGS.enemyPCids)
				if (CanBarCharFromOneOfMyFiefs(currentGS, enemyPCid, out RedProtoFief redProtoFief))
					charToBarIDs.Add(enemyPCid); // Add

			// charToUnbarIDs and charToReleaseIDs update
			HashSet<string> newcharToUnbarIDs = new HashSet<string>(charToUnbarIDs);
			foreach (string charToUnbarID in charToUnbarIDs)
            {
				bool canUnbarChar = false;
				foreach (RedProtoFief myFief in currentGS.myFiefs)
					if (myFief.barredCharactersId.Contains(charToUnbarID))
					{
						canUnbarChar = true;
						break;
					}
				if (!canUnbarChar)
					newcharToUnbarIDs.Remove(charToUnbarID);  // Remove
			}
			charToUnbarIDs = newcharToUnbarIDs;
			foreach (string allyPCid in currentGS.allyPCids)
			{
				if(currentGS.myCaptiveIDs.Contains(allyPCid))
					charToReleaseIDs.Add(allyPCid);
				foreach (RedProtoFief myFief in currentGS.myFiefs)
					if (myFief.barredCharactersId.Contains(allyPCid))
						charToUnbarIDs.Add(allyPCid); // Add
			}
			foreach (RedProtoCharacter myNPCorPC in currentGS.myFamilyNPCs.Cast<RedProtoCharacter>().Concat(currentGS.myEmployeeNPCs).Concat(new RedProtoCharacter[] { currentGS.myPC }))
				foreach (RedProtoFief myFief in currentGS.myFiefs)
					if(myFief.barredCharactersId != null)
						if (myFief.barredCharactersId.Contains(myNPCorPC.charID))
						{
							charToUnbarIDs.Add(myNPCorPC.charID); // Add
							break;
						}

			dayCostToTravelToKnownFiefs = new Dictionary<string, double>();
			foreach (RedProtoFief knownFief in currentGS.myFiefs.Concat(currentGS.notMyFiefs))
				if (!knownFief.fiefID.Equals(currentGS.myPC.location))
					dayCostToTravelToKnownFiefs.Add(knownFief.fiefID, double.Parse(getTravelDayCost(knownFief.fiefID).Message));

			fiefVisitedThisSeasonIDs.Add(currentFief.fiefID);

			return currentGS;
		}

		/// <summary>
		///     Fills a GameState with all the relevant information currently available by requesting them to the server.
		///     If previousGS isn't null, we take into account the information previously collected.
		/// </summary>
		/// <param name="previousGS">The previous GameState from which we will keep certain information</param>
		/// <returns>The GameState that corresponds to the current situation</returns>
		private GameState ObtainCurrentGameState(GameState previousGS)
		{
			GameState currentGS = ObtainCurrentGameState();
			RedProtoPlayerCharacter currentPC = currentGS.myPC;

			if (previousGS.notMyFiefs != null) // Check the previous GameState isn't empty
			{
				currentGS.allyPCids = previousGS.allyPCids;
				currentGS.enemyPCids = previousGS.enemyPCids;

				if (previousGS.currentYear > currentGS.currentYear)
					currentGS.currentYear = previousGS.currentYear;

				currentGS.spiedFiefs = previousGS.spiedFiefs;
				currentGS.spiedCharacters = previousGS.spiedCharacters;
				currentGS.spiedArmies = previousGS.spiedArmies;

				List<RedProtoFief> newNotMyFiefs = new List<RedProtoFief>(currentGS.notMyFiefs);
				foreach (RedProtoFief previousNotMyFief in previousGS.notMyFiefs)
				{
					bool isAlreadyInList = false;
					foreach (RedProtoFief currentNotMyFief in currentGS.notMyFiefs)
						if (previousNotMyFief.fiefID.Equals(currentNotMyFief.fiefID))
						{
							isAlreadyInList = true;
							break;
						}
					if (!isAlreadyInList)
						newNotMyFiefs.Add(RedDeepCopier.DeepCopyRedProtoFief(previousNotMyFief));
				}
				currentGS.notMyFiefs = newNotMyFiefs;
				newNotMyFiefs = new List<RedProtoFief>(currentGS.notMyFiefs);
				foreach (RedProtoFief currentNotMyFief in currentGS.notMyFiefs)
					foreach (RedProtoFief currentMyFief in currentGS.myFiefs)
						if (currentNotMyFief.fiefID.Equals(currentMyFief.fiefID)) // If I took one of the ennemy fief, then update the list
						{
							newNotMyFiefs.Remove(currentNotMyFief);
							break;
						}
				currentGS.notMyFiefs = newNotMyFiefs;

				foreach (RedProtoNPC previousNotMyNpc in previousGS.notMyNPCs)
				{
					bool isAlreadyInList = false;
					foreach (RedProtoNPC currentNotMyNpc in currentGS.notMyNPCs)
						if (previousNotMyNpc.charID.Equals(currentNotMyNpc.charID))
						{
							isAlreadyInList = true;
							break;
						}
					bool isNowMyNPC = false;
					if (!isAlreadyInList)
                    {
						foreach (RedProtoNPC currentMyNpc in currentGS.myEmployeeNPCs.Concat(currentGS.myFamilyNPCs))
							if (currentMyNpc.charID.Equals(previousNotMyNpc.charID))
							{
								isNowMyNPC = true;
								break;
							}
						if(!isNowMyNPC)
						currentGS.notMyNPCs.Add(RedDeepCopier.DeepCopyRedProtoNPC(previousNotMyNpc));
					}
						
				}

				foreach (RedProtoPlayerCharacter previousNotMyPc in previousGS.notMyPCs)
				{
					bool isAlreadyInList = false;
					foreach (RedProtoPlayerCharacter currentNotMyPcList in currentGS.notMyPCs)
						if (previousNotMyPc.charID.Equals(currentNotMyPcList.charID))
						{
							isAlreadyInList = true;
							break;
						}
					if (!isAlreadyInList)
						currentGS.notMyPCs.Add(RedDeepCopier.DeepCopyRedProtoPlayerCharacter(previousNotMyPc));
				}

				foreach (RedProtoArmy previousNotMyArmy in previousGS.notMyArmies)
				{
					bool isAlreadyInList = false;
					foreach (RedProtoArmy currentNotMyArmy in currentGS.notMyArmies)
						if (previousNotMyArmy.armyID.Equals(currentNotMyArmy.armyID))
						{
							isAlreadyInList = true;
							break;
						}
					if (!isAlreadyInList)
						currentGS.notMyArmies.Add(RedDeepCopier.DeepCopyRedProtoArmy(previousNotMyArmy));
				}

				if(previousGS.hasReceivedRansomForMyPC && ! string.IsNullOrWhiteSpace(currentGS.myPC.captor)) // Check if still captive
                {
					currentGS.hasReceivedRansomForMyPC = true;
					currentGS.ransomJentryID = previousGS.ransomJentryID;
				}

				currentGS.myCharsWhoProposedThisSeason = previousGS.myCharsWhoProposedThisSeason;
			}
			return currentGS;
		}

		/// <summary>
		///     Fill a GameState with all the relevant information currently available by requesting them to the server.
		/// </summary>
		/// <returns>The GameState that corresponds to the current situation</returns>
		protected GameState ObtainCurrentGameState()
		{
			GameState currentGS = new GameState();

			currentGS.myCurrentPCid = currentPCid;
			currentGS.enemyPCids = new HashSet<String>();
			currentGS.allyPCids = new HashSet<String>();

			
			ProtoGenericArray<ProtoPlayer> playersArray = (ProtoGenericArray<ProtoPlayer>)GetPlayers();
			currentGS.ExistingPlayers = new List<RedProtoPlayer>();
			if (playersArray != null)
				foreach (ProtoPlayer protoPlayer in playersArray.fields.ToList())
					currentGS.ExistingPlayers.Add(new RedProtoPlayer(protoPlayer));

			currentGS.hasReceivedRansomForMyPC = false; // TO CONTINUE

			 currentGS.myPC = new RedProtoPlayerCharacter((ProtoPlayerCharacter)ViewCharacter(currentPCid));
			currentGS.myFamilyNPCs = new List<RedProtoNPC>();
			ProtoCharacterOverview[] myCharArray = ((ProtoGenericArray<ProtoCharacterOverview>)GetNPCList("Family")).fields;
			if (myCharArray != null && myCharArray.Length > 0)
				foreach (ProtoCharacterOverview myChar in myCharArray)
                {
					//if (int.Parse(myChar.charID.Substring(5)) > 300) // If is an NPC
					//currentGS.myFamilyNPCs.Add(new RedProtoNPC((ProtoNPC)ViewCharacter(myChar.charID)));
					ProtoCharacter protoCharacter = (ProtoCharacter)ViewCharacter(myChar.charID);
					if(protoCharacter is ProtoNPC)
						currentGS.myFamilyNPCs.Add(new RedProtoNPC((ProtoNPC)protoCharacter));
				}
			currentGS.myEmployeeNPCs = new List<RedProtoNPC>();
			ProtoCharacterOverview[] myCharArrayBis = ((ProtoGenericArray<ProtoCharacterOverview>)GetNPCList("Employee")).fields;
			if (myCharArrayBis != null && myCharArrayBis.Length > 0)
				foreach (ProtoCharacterOverview myChar in myCharArrayBis)
				{
					currentGS.myEmployeeNPCs.Add(new RedProtoNPC((ProtoNPC)ViewCharacter(myChar.charID))); // Should be an NPC
				}
			currentGS.myFiefs = new List<RedProtoFief>();
			ProtoGenericArray<ProtoFief> myFiefsArray = (ProtoGenericArray<ProtoFief>)ViewMyFiefs();
			if (myFiefsArray.fields != null)
				foreach(ProtoFief protoFief in myFiefsArray.fields)
					currentGS.myFiefs.Add(new RedProtoFief(protoFief));
			currentGS.myArmies = new List<RedProtoArmy>();
			ProtoArmyOverview[] armyStatus = ((ProtoGenericArray<ProtoArmyOverview>)ListArmies()).fields;
			if (armyStatus != null)
				foreach (ProtoArmyOverview protoArmyOverview in armyStatus)
					if(!protoArmyOverview.armyID.Contains("Garrison"))
						currentGS.myArmies.Add(new RedProtoArmy((ProtoArmy)ViewArmy(protoArmyOverview.armyID)));
			currentGS.mySieges = new List<RedProtoSiegeDisplay>();
			currentGS.siegeOnMyFiefs = new List<RedProtoSiegeDisplay>();
			ProtoSiegeOverview[] protoSiegeOverviewArray = ((ProtoGenericArray<ProtoSiegeOverview>)SiegeList()).fields;
			if (protoSiegeOverviewArray != null)
				foreach (ProtoSiegeOverview protoSiegeOverview in protoSiegeOverviewArray)
					if(protoSiegeOverview.defendingPlayer.Equals(currentPCid))
						currentGS.siegeOnMyFiefs.Add(new RedProtoSiegeDisplay((ProtoSiegeDisplay)ViewSiege(protoSiegeOverview.siegeID)));
					else
						currentGS.mySieges.Add(new RedProtoSiegeDisplay((ProtoSiegeDisplay)ViewSiege(protoSiegeOverview.siegeID)));
			currentGS.myCaptiveIDs = new HashSet<string>();
			foreach (RedProtoFief myFief in currentGS.myFiefs)
			{
				ProtoMessage reply = ViewCaptives(myFief.fiefID);
                if (reply is ProtoGenericArray<ProtoCharacterOverview>)
                {
					ProtoGenericArray<ProtoCharacterOverview> protoGenericArray = (ProtoGenericArray<ProtoCharacterOverview>)reply;
					if (protoGenericArray != null)
						foreach (ProtoCharacterOverview captive in protoGenericArray.fields)
							currentGS.myCaptiveIDs.Add(captive.charID);
				}
			}

			RedProtoPlayerCharacter currentPC = currentGS.myPC; // getCurrentCharacter()
			RedProtoFief currentFief = new RedProtoFief((ProtoFief)ViewFief(currentPC.location));

			currentGS.notMyFiefs = new List<RedProtoFief>();
			if(! currentFief.ownerID.Equals(currentPC.charID))
				currentGS.notMyFiefs.Add(currentFief);

			currentGS.notMyNPCs = new List<RedProtoNPC>();
			currentGS.notMyPCs = new List<RedProtoPlayerCharacter>();
			foreach (string characterInFiefId in currentFief.characterInFiefIDs)
            {
				ProtoCharacter charInFief = (ProtoCharacter)ViewCharacter(characterInFiefId);
				bool isMine = false;
				if (charInFief.familyID != null)
					if (charInFief.familyID.Equals(currentPC.familyID))
						isMine = true;
				if (charInFief is ProtoNPC)
					if((charInFief as ProtoNPC).employer != null)
						if ((charInFief as ProtoNPC).employer.charID.Equals(currentPC.charID))
							isMine = true;
				if (!isMine)
                {
					if (charInFief is ProtoPlayerCharacter)
						currentGS.notMyPCs.Add(new RedProtoPlayerCharacter(charInFief as ProtoPlayerCharacter));
					else
						currentGS.notMyNPCs.Add(new RedProtoNPC(charInFief as ProtoNPC));
				}
			}
			currentGS.spiedFiefs = new Dictionary<string, RedProtoFief>();
			currentGS.spiedCharacters = new Dictionary<string, RedProtoCharacter>();
			currentGS.spiedArmies = new Dictionary<string, RedProtoArmy>();

			currentGS.notMyArmies = new List<RedProtoArmy>();
			currentGS.notMyGarrisons = new List<RedProtoArmy>();
			if (currentFief.armyIDs != null)
				foreach (string armyInFiefId in currentFief.armyIDs)
                {
					ProtoArmy armyInFief = (ProtoArmy)ViewArmy(armyInFiefId);
                    if(! armyInFief.ownerID.Equals(currentPC.charID))
                    {
						if (armyInFiefId.Contains("Garrison"))
							currentGS.notMyGarrisons.Add(new RedProtoArmy(armyInFief));
						else
							currentGS.notMyArmies.Add(new RedProtoArmy(armyInFief));
					}
				}
			currentGS.currentYear = 1194; // The actual version of the server starts in 1194
			currentGS.receivedProposalFromThisChar = null;
			currentGS.unreadJournalEntries = new List<RedProtoJournalEntry>();
			ProtoJournalEntry[] UnreadJournalEntriesArray = ViewJournalEntries("unread").fields;
			if (UnreadJournalEntriesArray != null)
				foreach (ProtoJournalEntry protoJournalEntry in UnreadJournalEntriesArray)
                {
					currentGS.unreadJournalEntries.Add(new RedProtoJournalEntry(protoJournalEntry));
					if (protoJournalEntry.year > currentGS.currentYear)
						currentGS.currentYear = (int)protoJournalEntry.year;
					if (protoJournalEntry.type.Equals("proposalMade"))
						if (!protoJournalEntry.personae[3].charID.Equals(currentPC.charID))
							currentGS.receivedProposalFromThisChar = new RedProtoCharacterOverview(protoJournalEntry.personae[2]);
				}
			currentGS.armyDetachmentsForMe = new List<RedProtoDetachment>();
			foreach (RedProtoArmy myArmy in currentGS.myArmies)
			{
				ProtoDetachment[] detachmentsFromArmy = ((ProtoGenericArray<ProtoDetachment>)ListDetachments(myArmy.armyID)).fields;
				if (detachmentsFromArmy != null)
					foreach (ProtoDetachment detachmentFromArmy in detachmentsFromArmy)
						if (detachmentFromArmy.leftFor.Equals(currentPCid))
							currentGS.armyDetachmentsForMe.Add(new RedProtoDetachment(detachmentFromArmy));
			}
			currentGS.myCharsWhoProposedThisSeason = new Dictionary<string, bool>();
			currentGS.myCharsWhoProposedThisSeason.Add(currentPCid, false);
			foreach (RedProtoNPC myFamilyNPC in currentGS.myFamilyNPCs)
				currentGS.myCharsWhoProposedThisSeason.Add(myFamilyNPC.charID, false);

			return currentGS;
		}

		/// <summary>Search for all the possible actions to execute from a certain Gamestate</summary>
		/// <param name="gameState">GameState from which to check what are the available actions to perform</param>
		/// <returns>The list of all actions the agent can perform from this GameSate</returns>
		protected List<MainActions> getPossibleActions(GameState gameState)
		{
			RedProtoPlayerCharacter currentPC = gameState.myPC;
			RedProtoFief currentFief = gameState.getCurrentFief();
			RedProtoFief homeFief = gameState.getHomeFief();

			HashSet<MainActions> possibleActions = new HashSet<MainActions>();

			if (currentPC.isAlive && currentPC.days > 0)
			{
				if (! string.IsNullOrWhiteSpace(currentPC.captor)) 
				{
					if (gameState.hasReceivedRansomForMyPC)
						possibleActions.Add(MainActions.KidnapRespondRansom); // MainActions.KidnapRespondRansom
				}
				else
				{
					if (dayCostToTravelToAdjacentFiefs.Values.Min() <= currentPC.days)
						possibleActions.Add(MainActions.NPCmoveDirection); // MainActions.NPCmove
					if(dayCostToTravelToKnownFiefs.Values.Min() <= currentPC.days)
						possibleActions.Add(MainActions.NPCmoveToFief); // MainActions.NPCmoveToFief
					possibleActions.Add(MainActions.NPCcamp); // MainActions.NPCcamp
					foreach (RedProtoNPC myFamilyNPC in gameState.myFamilyNPCs)
						if (!String.IsNullOrWhiteSpace(myFamilyNPC.familyID) && myFamilyNPC.isMale)
							if (myFamilyNPC.familyID.Equals(currentPC.familyID))
							{
								possibleActions.Add(MainActions.NPCappointHeir); // MainActions.NPCappointHeir
								break;
							}
					if (gameState.myArmies.Count > 0)
					{
						possibleActions.Add(MainActions.ArmyDisband); // MainActions.ArmyDisband
						foreach (RedProtoArmy myArmy in gameState.myArmies)
                            if (!string.IsNullOrWhiteSpace(myArmy.leaderName))
                            {
								possibleActions.Add(MainActions.ArmyDropOff); // MainActions.ArmyDropOff
								break;
							}
						foreach (RedProtoCharacter myPCorEmployeeNPC in gameState.myEmployeeNPCs.Concat(new RedProtoCharacter[]{ gameState.myPC }))
                        {
							if (possibleActions.Contains(MainActions.ArmyAppointLeader))
								break;
							if (myPCorEmployeeNPC.isMale)
								foreach (RedProtoArmy myArmy in gameState.myArmies)
									if(currentFief.armyIDs != null)
										if (currentFief.armyIDs.Contains(myArmy.armyID) && myPCorEmployeeNPC.location.Equals(currentFief.fiefID))
											if (string.IsNullOrWhiteSpace(myArmy.leaderName))
											{
												possibleActions.Add(MainActions.ArmyAppointLeader); // MainActions.ArmyAppointLeader
												break;
											}
											else
											{
												if (!myArmy.leaderName.Equals(myPCorEmployeeNPC.firstName + " " + myPCorEmployeeNPC.familyName))
												{
													possibleActions.Add(MainActions.ArmyAppointLeader); // MainActions.ArmyAppointLeader
													break;
												}
											}
						}
						if (!currentFief.ownerID.Equals(currentPC.charID) && currentFief.siege == null)
						{
							foreach (RedProtoArmy myArmy in gameState.myArmies)
								if (gameState.tryGetArmyFiefID(myArmy.armyID, out string armyFiefID))
									if(armyFiefID.Equals(currentFief.fiefID))
									{
										possibleActions.Add(MainActions.SiegeBesiegeCurrentFief); // MainActions.SiegeBesiegeCurrentFief
										if (!currentFief.isPillaged)
											possibleActions.Add(MainActions.ArmyPillageCurrentFief); // MainActions.ArmyPillageFief
										break;
									}	
						}
						if (currentFief.armyIDs != null)
							foreach (string armyInFiefID in currentFief.armyIDs)
                            {
								bool isMine = false;
								foreach (RedProtoArmy myArmy in gameState.myArmies)
									if (armyInFiefID.Equals(myArmy.armyID))
									{
										isMine = true;
										break;
									}
                                if (!isMine)
                                {
									possibleActions.Add(MainActions.ArmyAttack); // MainActions.ArmyAttack
									break;
								}
							}
						foreach (RedProtoDetachment protoDetachment in gameState.armyDetachmentsForMe)
							if (protoDetachment.leftFor.Equals(currentPC.charID))
							{
								possibleActions.Add(MainActions.ArmyPickUp); // MainActions.ArmyPickUp
								break;
							}
						foreach (RedProtoArmy myArmy in gameState.myArmies)
							if (!myArmy.isMaintained && homeFief.treasury >= myArmy.maintCost)
                            {
								possibleActions.Add(MainActions.ArmyMaintain); // MainActions.ArmyMaintain
								break;
							}
						int indivTroopCost = 2000;
						if (currentFief.ancestralOwnerId != null)
							if (currentFief.ancestralOwnerId.Equals(currentPC.charID))
								indivTroopCost = 500;
						if(gameState.tryGetMainArmyFiefID(out string mainArmyFiefID))
							if (mainArmyFiefID.Equals(currentFief.fiefID) && currentFief.ownerID.Equals(currentPC.charID) && !currentFief.hasRecruited && homeFief.treasury >= indivTroopCost)
							{
								possibleActions.Add(MainActions.ArmyRecruit); // MainActions.ArmyHire
							}
						possibleActions.Add(MainActions.ArmyAdjustCombatValues); // MainActions.ArmyAdjustCombatValues
					}
					foreach(RedProtoSiegeDisplay mySiege in gameState.mySieges)
					if (mySiege.besiegedFiefID.Equals(currentFief.fiefID))
					{
						possibleActions.Add(MainActions.SiegeStorm); // MainActions.SiegeStorm
						possibleActions.Add(MainActions.SiegeReduction); // MainActions.SiegeReduction
						possibleActions.Add(MainActions.SiegeNegotiation); // MainActions.SiegeNegotiation
						possibleActions.Add(MainActions.SiegeEnd); // MainActions.SiegeEnd
					}
					if (gameState.myFiefs.Count > 0)
					{
						if (!possibleActions.Contains(MainActions.NPCbarCharacters))
						{
							bool canBeBarred = false;
							foreach (RedProtoCharacter notMyChar in gameState.notMyNPCs.Cast<RedProtoCharacter>().Concat(gameState.notMyPCs.Cast<RedProtoCharacter>()))
								if (canBeBarred = CanBarCharFromOneOfMyFiefs(gameState, notMyChar.charID, out RedProtoFief redProtoFief))
									break;
							if (!canBeBarred)
                            {
								foreach (RedProtoFief notMyFief in gameState.notMyFiefs)
									if (CanBarCharFromOneOfMyFiefs(gameState, notMyFief.ownerID, out RedProtoFief redProtoFief))
										break;
								foreach (RedProtoArmy notMyArmy in gameState.notMyArmies)
									if (CanBarCharFromOneOfMyFiefs(gameState, notMyArmy.ownerName, out RedProtoFief redProtoFief))
										break;
							}
							if (canBeBarred)
								possibleActions.Add(MainActions.NPCbarCharacters); // MainActions.NPCbarCharacters
						}
						foreach (RedProtoFief myFief in gameState.myFiefs)
						{
							if(!possibleActions.Contains(MainActions.FiefAutoAdjustExpenditure) && myFief.treasury < 0)
								possibleActions.Add(MainActions.FiefAutoAdjustExpenditure); // MainActions.AutoAdjustExpenditure
							if (!possibleActions.Contains(MainActions.FiefTransferFunds) && gameState.notMyFiefs.Count > 0 || gameState.myFiefs.Count >= 2)
								possibleActions.Add(MainActions.FiefTransferFunds); // MainActions.FiefTransferFunds
							if (!possibleActions.Contains(MainActions.NPCunbarCharacters) && myFief.barredCharactersId != null)
								if (myFief.barredCharactersId.Length > 0)
									possibleActions.Add(MainActions.NPCunbarCharacters); // MainActions.NPCunbarCharacters
							if (!possibleActions.Contains(MainActions.NPCbarNationalities) || !possibleActions.Contains(MainActions.NPCbarNationalities))
							{
								bool hasBaredAllNationalities = true;
								foreach (String nationality in nationalities)
									if (myFief.barredNationalities != null)
										if (myFief.barredNationalities.Contains(nationality))
										{
											if (!possibleActions.Contains(MainActions.NPCbarNationalities))
												possibleActions.Add(MainActions.NPCunbarNationalities); // MainActions.NPCunbarNationalities
										}
										else
											hasBaredAllNationalities = false;
								if (!possibleActions.Contains(MainActions.NPCbarNationalities) && !hasBaredAllNationalities)
									possibleActions.Add(MainActions.NPCbarNationalities); // MainActions.NPCbarNationalities
							}
						}
						if (gameState.ExistingPlayers.Count > 2 && homeFief.treasury > 0)
							possibleActions.Add(MainActions.FiefTransferFundsToPlayer); // MainActions.TransferFundsToPlayer
						if (gameState.notMyFiefs.Count > 0)
                        {
							foreach (RedProtoFief myFief in gameState.myFiefs)
								if (myFief.ancestralOwnerId == null || currentPC.titles.Contains("King"))
									possibleActions.Add(MainActions.FiefGrantFiefTitle); // MainActions.FiefGrantFiefTitle
						}	
					}
					if (gameState.myEmployeeNPCs.Count > 0)
                    {
						possibleActions.Add(MainActions.NPCfire); // MainActions.NPCfire
						foreach (RedProtoNPC myNpc in gameState.myFamilyNPCs.Concat(gameState.myEmployeeNPCs))
							if (myNpc.inEntourage)
							{
								possibleActions.Add(MainActions.NPCremoveFromEntourage); // MainActions.NPCremoveFromEntourage
								break;
							}
						foreach (RedProtoNPC myNPC in gameState.myFamilyNPCs.Concat(gameState.myEmployeeNPCs))
							if (!myNPC.inEntourage && !string.IsNullOrWhiteSpace(myNPC.location))
								if (myNPC.location.Equals(currentFief.fiefID))
								{
									possibleActions.Add(MainActions.NPCaddToEntourage); // MainActions.NPCaddToEntourage
									break;
								}
						foreach (RedProtoNPC myEmployeeNpc in gameState.myEmployeeNPCs)
							if (myEmployeeNpc.isMale && string.IsNullOrWhiteSpace(myEmployeeNpc.captor) && gameState.currentYear - myEmployeeNpc.birthYear >= 14)
							{
								possibleActions.Add(MainActions.NPCappointBaillif); // MainActions.NPCappointBaillif
								break;
							}
						foreach (RedProtoFief myFief in gameState.myFiefs)
							if (! string.IsNullOrWhiteSpace(myFief.bailiffID))
							{
								possibleActions.Add(MainActions.NPCremoveBaillif); // MainActions.NPCappointBaillif
								break;
							}
					}
					if (! string.IsNullOrWhiteSpace(currentPC.spouse))
					{
						if (! gameState.tryGetCharacter(currentPC.spouse, out RedProtoCharacter spouse))
							throw new Exception("Shouldn't happen");
						if (! spouse.isPregnant && spouse.location.Equals(currentFief.fiefID) && (string.IsNullOrWhiteSpace(currentFief.siege)
						|| currentPC.inKeep == spouse.inKeep) && spouse.days > 1)
							possibleActions.Add(MainActions.NPCtryForChild); // MainActions.NPCtryForChild
					}
					if (gameState.receivedProposalFromThisChar != null)
						possibleActions.Add(MainActions.NPCacceptRejectProposal); // MainActions.NPCacceptRejectProposal
					foreach (RedProtoNPC notMyNPC in gameState.notMyNPCs)
						if (!notMyNPC.isMale && string.IsNullOrWhiteSpace(notMyNPC.spouse) && string.IsNullOrWhiteSpace(notMyNPC.fiancee) && string.IsNullOrWhiteSpace(notMyNPC.captor) && !notMyNPC.familyID.Equals(currentPC.familyID)
							& gameState.currentYear - notMyNPC.birthYear >= 14) // Check age (only if I have an estimation of the current year)
						{
							if (!gameState.myCharsWhoProposedThisSeason.TryGetValue(currentPCid, out bool hasCurrentPCproposed))
								throw new Exception("Couldn't find '" + currentPCid + "', it shouldn't happen");
							if (string.IsNullOrWhiteSpace(currentPC.spouse) && string.IsNullOrWhiteSpace(currentPC.fiancee) && !hasCurrentPCproposed)
								possibleActions.Add(MainActions.NPCmarryPC); // MainActions.NPCmarry
							foreach(RedProtoNPC myFamilyNPC in gameState.myFamilyNPCs)
                            {
								if (!gameState.myCharsWhoProposedThisSeason.TryGetValue(myFamilyNPC.charID, out bool hasMyChildProposed))
									throw new Exception("Couldn't find '" + myFamilyNPC.charID + "', it shouldn't happen");
								if (myFamilyNPC.isMale && string.IsNullOrWhiteSpace(myFamilyNPC.spouse) && string.IsNullOrWhiteSpace(myFamilyNPC.fiancee) && !hasMyChildProposed)
								{
									possibleActions.Add(MainActions.NPCmarryChild); // MainActions.NPCmarrySon
									break;
								}
							}
							break;
						}
					if (gameState.receivedProposalFromThisChar != null && string.IsNullOrWhiteSpace(currentPC.spouse) && string.IsNullOrWhiteSpace(currentPC.fiancee))
						possibleActions.Add(MainActions.NPCacceptRejectProposal); // MainActions.NPCacceptRejectProposal
					if (currentPC.inKeep)
						possibleActions.Add(MainActions.NPCexitKeep); // MainActions.NPCexitKeep
					else
					{
						bool canEnterKeep = true;
						bool hasArmyInFief = false;
						foreach (RedProtoArmy myArmy in gameState.myArmies)
							if (gameState.tryGetArmyFiefID(myArmy.armyID, out string armyFiefID))
                                if (armyFiefID.Equals(currentFief.fiefID))
                                {
									hasArmyInFief = true;
									break;
								}
						if (currentFief.barredNationalities != null)
							if (currentFief.barredNationalities.Contains(currentPC.nationality))
								canEnterKeep = false;
						if (currentFief.barredCharactersId != null)
							if (currentFief.barredCharactersId.Contains(currentPC.charID))
								canEnterKeep = false;
						if (hasArmyInFief && !currentFief.ownerID.Equals(currentPC.charID))
							canEnterKeep = false;
						if (canEnterKeep)
							possibleActions.Add(MainActions.NPCtryEnterKeep); // MainActions.NPCenterKeep
					}
					foreach (string charInFiefId in currentFief.characterInFiefIDs)
					{
						if(! gameState.tryGetCharacter(charInFiefId, out RedProtoCharacter charInFief))
							throw new Exception("Couldn't find the character '" + charInFiefId + "', it shouldn't happen");
						bool isMine = false;
						if (!string.IsNullOrWhiteSpace(charInFief.familyID))
							if (charInFief.familyID.Equals(currentPC.familyID))
								isMine = true;
						if (charInFief is RedProtoNPC)
							if (!string.IsNullOrWhiteSpace((charInFief as RedProtoNPC).employerId))
							{
								if ((charInFief as RedProtoNPC).employerId.Equals(currentPC.charID))
									isMine = true;
							}
							else
							{
								if (!possibleActions.Contains(MainActions.NPChire) && charInFief.isMale && gameState.currentYear - charInFief.birthYear >= 14 && string.IsNullOrWhiteSpace(charInFief.familyID)) // Check age (only if I have an estimation of the current year))
										possibleActions.Add(MainActions.NPChire); // MainActions.NPChire
							}
						if (!isMine)
							if (!possibleActions.Contains(MainActions.SpyCharacter))
								possibleActions.Add(MainActions.SpyCharacter); // MainActions.SpyCharacter
						if (possibleActions.Contains(MainActions.SpyCharacter) && possibleActions.Contains(MainActions.NPChire))
							break;
					}
					if (currentFief.armyIDs != null)
						foreach (string armyInFiefId in currentFief.armyIDs)
						{
                            if (!armyInFiefId.Contains("Garrison"))
                            {
								if (!gameState.tryGetArmy(armyInFiefId, out RedProtoArmy armyInFief))
									throw new Exception("Couldn't find the army'" + armyInFiefId + "', it shouldn't happen");
								bool isMine = false;
								foreach (RedProtoArmy myArmy in gameState.myArmies)
									if (myArmy.armyID.Equals(armyInFiefId))
									{
										isMine = true;
										break;
									}
								if (!isMine && !armySpiedThisSeasonIDs.Contains(armyInFiefId))
								{
									possibleActions.Add(MainActions.SpyArmy); // MainActions.SpyArmy
									break;
								}
							}
						}
					if (!currentFief.ownerID.Equals(currentPC.charID) && !fiefSpiedThisSeasonIDs.Contains(currentFief.fiefID))
						possibleActions.Add(MainActions.SpyFief); // MainActions.SpyFief
					foreach (string characterIdInFief in currentFief.characterInFiefIDs)
					{
						if (gameState.tryGetNotMyPC(characterIdInFief, out RedProtoPlayerCharacter notMyCharacter))
							if (string.IsNullOrWhiteSpace(notMyCharacter.captor))
							{
								possibleActions.Add(MainActions.KidnapCharacter); // MainActions.KidnapCharacter
								break;
							}
					}
					if (gameState.myCaptiveIDs.Count > 0)
					{
						possibleActions.Add(MainActions.KidnapReleaseCaptive); // MainActions.KidnapReleaseCaptive
						possibleActions.Add(MainActions.KidnapExecuteCaptive); // MainActions.KidnapExecuteCaptive
						if (myRansomedCaptiveIDs.Count < gameState.myCaptiveIDs.Count)
							possibleActions.Add(MainActions.KidnapRansomCaptive); // MainActions.KidnapRansomCaptive
					}
				}
			}
			List<MainActions> newPossibleActions = new List<MainActions>(possibleActions);
			foreach (MainActions possibleAction in possibleActions)
            {
				if (!maxDaysToDoAction.TryGetValue(possibleAction, out double maxCostToDoAction))
					throw new Exception("The key '" + possibleAction.ToString() + "' isn't present in this dictionnary, it needs to be added!");
				if (maxCostToDoAction > currentPC.days || FailedActions.Contains(possibleAction))
					newPossibleActions.Remove(possibleAction);
			}
			return newPossibleActions;
		}

		/// <summary>
		///     Execute an action and check if it was correctly executed.
		/// </summary>
		/// <param name="actionToExecute">action to execute</param>
		/// <param name="arguments">arguments to execute the action with</param>
		/// <param name="gameState">The current GameState so we can modify it if it's needed</param>
		/// <param name="gameStateWithModifs">Modified GameState</param>
		/// <returns></returns>
		protected bool ExecuteAction(MainActions actionToExecute, String[] arguments, GameState gameState, out GameState gameStateWithModifs)
		{
			RedProtoFief currentFief = gameState.getCurrentFief();
			ProtoMessage protoMessage;
			DisplayMessages respType;
			bool executionSuccess = false;
			switch (actionToExecute)
			{
				case MainActions.ArmyRecruit:
					executionSuccess = RecruitTroops(arguments[0], int.Parse(arguments[1])).ResponseType == DisplayMessages.Success;
					break;
				case MainActions.ArmyMaintain:
					executionSuccess = MaintainArmy(arguments[0]).ResponseType == DisplayMessages.ArmyMaintainConfirm;
					break;
				case MainActions.ArmyPickUp:
					executionSuccess = PickUpTroops(arguments[0], arguments.Skip(1).ToArray()).ResponseType == DisplayMessages.ArmyMaintainConfirm;
					if(executionSuccess) 
						foreach(RedProtoDetachment detachmentInCurrentFief in gameState.armyDetachmentsForMe)
                            if (detachmentInCurrentFief.id.Equals(arguments[0]))
                            {
								AddAlly(gameState, detachmentInCurrentFief.leftBy, out gameState); // The one that gave me the army become an ally
								break;
                            }
					break;
				case MainActions.ArmyDropOff:
					if (arguments.Length != 9)
						throw new Exception("arguments hasn't the correct length");
					uint[] troopsToDropOff = new uint[7];
					for (int i = 0; i < 7; i++)
						troopsToDropOff[i] = uint.Parse(arguments[i+1]);
;					executionSuccess = DropOffTroops(arguments[0], troopsToDropOff, arguments[arguments.Count() - 1]).ResponseType == DisplayMessages.Success;
					break;
				case MainActions.ArmyDisband:
					executionSuccess = DisbandArmy(arguments[0]).ResponseType == DisplayMessages.Success;
					break;
				case MainActions.ArmyAppointLeader:
					executionSuccess = AppointLeader(arguments[0], arguments[1]).ResponseType == DisplayMessages.None;
					break;
				case MainActions.ArmyPillageCurrentFief:
					executionSuccess = PillageFief(arguments[0]).ResponseType == DisplayMessages.Success;
					if (executionSuccess)
						AddEnemy(gameState, currentFief.ownerID, out gameState);
					break;
				case MainActions.ArmyAdjustCombatValues:
					executionSuccess = AdjustCombatValues(arguments[0], byte.Parse(arguments[1]), byte.Parse(arguments[2])).ResponseType == DisplayMessages.Success;
					break;
				case MainActions.ArmyAttack:
					executionSuccess = Attack(arguments[0], arguments[1]).ResponseType == DisplayMessages.Success;
					break;
				case MainActions.SiegeBesiegeCurrentFief:
					executionSuccess = BesiegeCurrentFief(arguments[0]).ResponseType == DisplayMessages.Success;
					if (executionSuccess)
						AddEnemy(gameState, currentFief.ownerID, out gameState);
					break;
				case MainActions.SiegeStorm:
					executionSuccess = SiegeRoundStorm(arguments[0]).ResponseType == DisplayMessages.None;
					break;
				case MainActions.SiegeNegotiation:
					executionSuccess = SiegeRoundNegotiate(arguments[0]).ResponseType == DisplayMessages.None;
					break;
				case MainActions.SiegeReduction:
					executionSuccess = SiegeRoundReduction(arguments[0]).ResponseType == DisplayMessages.None;
					break;
				case MainActions.SiegeEnd:
					executionSuccess = EndSiege(arguments[0]).ResponseType == DisplayMessages.Success;
					break;
				case MainActions.NPCmoveDirection:
					if (!Enum.TryParse(arguments[0], out MoveDirections direction))
						throw new Exception("Couldn't parse '" + arguments[0] + "' to a MoveDirections object");
					executionSuccess = Move(direction).ResponseType == DisplayMessages.Success;
					if (executionSuccess)
                    {
						nbConsecutiveNPCmoveDirections++;
						lastNPCmoveDirection = direction;
					}
					break;
				case MainActions.NPCmoveToFief:
					executionSuccess = MoveToFief(arguments[0]).ResponseType == DisplayMessages.Success;
					break;
				case MainActions.NPCmarryPC:
					respType = Marry(currentPCid, arguments[0]).ResponseType;
					executionSuccess = respType == DisplayMessages.Success || respType == DisplayMessages.CharacterProposalAlready;
					if(executionSuccess)
						gameState.myCharsWhoProposedThisSeason.Add(arguments[0], true);
					break;
				case MainActions.NPCmarryChild:
					respType = Marry(arguments[0], arguments[1]).ResponseType;
					executionSuccess = respType == DisplayMessages.Success || respType == DisplayMessages.CharacterProposalAlready;
					if (executionSuccess)
						gameState.myCharsWhoProposedThisSeason.Add(arguments[0], true);
					break;
				case MainActions.NPCacceptRejectProposal:
					executionSuccess = AcceptRejectProposal(bool.Parse(arguments[0])).ResponseType == DisplayMessages.Success;
					if (executionSuccess) 
					{
						AddAlly(gameState, gameState.receivedProposalFromThisChar.ownerName, out gameState); // The family of my wife naturally becomes an ally if not already
					}
					break;
				case MainActions.NPCtryForChild:
					respType = TryForChild().ResponseType;
					executionSuccess = respType == DisplayMessages.CharacterSpousePregnant || respType == DisplayMessages.CharacterSpouseNotPregnant || respType == DisplayMessages.CharacterSpouseNeverPregnant;
					break;
				case MainActions.NPChire:
					respType = hireNPC(arguments[0], uint.Parse(arguments[1])).ResponseType;
					executionSuccess = respType == DisplayMessages.CharacterOfferHigh || respType == DisplayMessages.CharacterOfferLow || respType == DisplayMessages.CharacterOfferOk || respType == DisplayMessages.CharacterOfferAlmost;
                    if (executionSuccess)
                    {
						if (respType == DisplayMessages.CharacterOfferHigh || respType == DisplayMessages.CharacterOfferOk) // If Npc accepted the bid
						{
							lastOfferToNPCid = null;
							lastOfferToNPCamount = 0;
						}
						else
						{
							lastOfferToNPCid = arguments[0];
							lastOfferToNPCamount = uint.Parse(arguments[1]);
						}
					}
					break;
				case MainActions.NPCfire:
					executionSuccess = fireNPC(arguments[0]).ResponseType == DisplayMessages.Success;
					break;
				case MainActions.NPCbarCharacters:
					executionSuccess = BarCharacters(arguments[0], arguments.Skip(1).ToArray()).ResponseType == DisplayMessages.None;
					break;
				case MainActions.NPCunbarCharacters:
					executionSuccess = UnbarCharacters(arguments[0], arguments.Skip(1).ToArray()).ResponseType == DisplayMessages.None;
					break;
				case MainActions.NPCbarNationalities:
					executionSuccess = BarNationalities(arguments[0], arguments.Skip(1).ToArray()).ResponseType == DisplayMessages.Success;
					break;
				case MainActions.NPCunbarNationalities:
					executionSuccess = UnbarNationalities(arguments[0], arguments.Skip(1).ToArray()).ResponseType == DisplayMessages.Success;
					break;
				case MainActions.NPCtryEnterKeep:
					respType = EnterExitKeep(arguments[0]).ResponseType;
					executionSuccess = respType == DisplayMessages.Success || respType == DisplayMessages.CharacterAlreadyArmy;
					break;
				case MainActions.NPCexitKeep:
					executionSuccess = EnterExitKeep(arguments[0]).ResponseType == DisplayMessages.Success;
					break;
				case MainActions.NPCcamp:
					executionSuccess = Camp(arguments[0], byte.Parse(arguments[1])).ResponseType == DisplayMessages.CharacterCamp;
					break;
				case MainActions.NPCaddToEntourage:
					executionSuccess = AddRemoveEntourage(arguments[0]).ResponseType == DisplayMessages.Success;
					break;
				case MainActions.NPCremoveFromEntourage:
					executionSuccess = AddRemoveEntourage(arguments[0]).ResponseType == DisplayMessages.Success;
					break;
				case MainActions.NPCappointHeir:
					executionSuccess = AppointHeir(arguments[0]).ResponseType == DisplayMessages.Success;
					break;
				case MainActions.NPCappointBaillif:
					executionSuccess = AppointBailiff(arguments[0], arguments[1]).ResponseType == DisplayMessages.Success;
					break;
				case MainActions.NPCremoveBaillif:
					executionSuccess = RemoveBailiff(arguments[0]).ResponseType == DisplayMessages.Success;
					break;
				case MainActions.FiefGrantFiefTitle:
					executionSuccess = GrantFiefTitle(arguments[0], arguments[1]).ResponseType == DisplayMessages.Success;
					break;
				case MainActions.FiefAutoAdjustExpenditure:
					executionSuccess = AutoAdjustExpenditure(arguments[0]).ResponseType == DisplayMessages.FiefExpenditureAdjusted;
					if (executionSuccess)
						expenditureAdjustedThisSeasonFiefIDs.Add(arguments[0]);
					break;
				case MainActions.FiefTransferFunds:
					executionSuccess = TransferFunds(arguments[0], arguments[1], int.Parse(arguments[2])).ResponseType == DisplayMessages.Success;
					break;
				case MainActions.FiefTransferFundsToPlayer:
					executionSuccess = TransferFundsToPlayer(arguments[0], int.Parse(arguments[1])).ResponseType == DisplayMessages.Success;
					break;
				case MainActions.SpyFief:
					protoMessage = SpyFief(arguments[0], arguments[1]);
					respType = protoMessage.ResponseType;
					executionSuccess = respType == DisplayMessages.SpySuccessDetected || respType == DisplayMessages.SpySuccess || respType == DisplayMessages.SpyFailDead || respType == DisplayMessages.SpyFailDetected || respType == DisplayMessages.SpyFail;
					if (respType == DisplayMessages.SpySuccessDetected || respType == DisplayMessages.SpySuccess)
                    {
						fiefSpiedThisSeasonIDs.Add(arguments[0]);
						if (gameState.spiedFiefs.ContainsKey(arguments[0]))
							gameState.spiedFiefs.Remove(arguments[0]);
						gameState.spiedFiefs.Add(arguments[0], new RedProtoFief((ProtoFief)protoMessage));
					}
					if (respType == DisplayMessages.SpySuccessDetected || respType == DisplayMessages.SpyFailDead || respType == DisplayMessages.SpyFailDetected)
                    {
						if (!gameState.tryGetFief(arguments[0], out RedProtoFief fief))
							throw new Exception("Couldn't find the fief '" + arguments[0] + "', it shouldn't happen");
						AddEnemy(gameState, fief.ownerID, out gameState);
                    }
					break;
				case MainActions.SpyCharacter:
					protoMessage = SpyCharacter(arguments[0], arguments[1]);
					respType = protoMessage.ResponseType;
					executionSuccess = respType == DisplayMessages.SpySuccessDetected || respType == DisplayMessages.SpySuccess || respType == DisplayMessages.SpyFailDead || respType == DisplayMessages.SpyFailDetected || respType == DisplayMessages.SpyFail;
					if (respType == DisplayMessages.SpySuccessDetected || respType == DisplayMessages.SpySuccess)
                    {
						charSpiedThisSeasonIDs.Add(arguments[0]);
						if (gameState.spiedCharacters.ContainsKey(arguments[0]))
							gameState.spiedCharacters.Remove(arguments[0]);
						RedProtoCharacter redProtoCharacter;
						if (protoMessage is ProtoPlayerCharacter)
							redProtoCharacter = new RedProtoPlayerCharacter((ProtoPlayerCharacter)protoMessage);
						else
							redProtoCharacter = new RedProtoNPC((ProtoNPC)protoMessage);
						gameState.spiedCharacters.Add(arguments[0], redProtoCharacter);
					}
					if (respType == DisplayMessages.Success || respType == DisplayMessages.SpySuccessDetected || respType == DisplayMessages.SpyFailDead || respType == DisplayMessages.SpyFailDetected)
					{
						if (!gameState.tryGetCharacter(arguments[0], out RedProtoCharacter spiedCharacter))
							throw new Exception("Shouldn't be null");
						if(spiedCharacter is RedProtoPlayerCharacter)
							AddEnemy(gameState, spiedCharacter.charID, out gameState);
                        else
                        {
							RedProtoNPC spiedNPC = (RedProtoNPC)spiedCharacter;
							if(! string.IsNullOrWhiteSpace(spiedNPC.employerId))
								AddEnemy(gameState, spiedNPC.employerId, out gameState);
						}
					}
					break;
				case MainActions.SpyArmy:
					protoMessage = SpyArmy(arguments[0], arguments[1]);
					respType = protoMessage.ResponseType;
					executionSuccess = respType == DisplayMessages.SpySuccessDetected || respType == DisplayMessages.SpySuccess || respType == DisplayMessages.SpyFailDead || respType == DisplayMessages.SpyFailDetected || respType == DisplayMessages.SpyFail;
					if (respType == DisplayMessages.SpySuccessDetected || respType == DisplayMessages.SpySuccess)
                    {
						armySpiedThisSeasonIDs.Add(arguments[0]);
						if (gameState.spiedArmies.ContainsKey(arguments[0]))
							gameState.spiedArmies.Remove(arguments[0]);
						gameState.spiedArmies.Add(arguments[0], new RedProtoArmy((ProtoArmy)protoMessage));
					}
					if (respType == DisplayMessages.SpySuccessDetected || respType == DisplayMessages.SpyFailDead || respType == DisplayMessages.SpyFailDetected)
					{
						if (!gameState.tryGetArmy(arguments[0], out RedProtoArmy spiedArmy))
							throw new Exception("Shouldn't be null");
						AddEnemy(gameState, spiedArmy.ownerName, out gameState);
					}
					break;
				case MainActions.KidnapCharacter:
					executionSuccess = Kidnap(arguments[0], arguments[1]) == null;
					// TO DO: add ennemy if got detected
					break;
				case MainActions.KidnapRansomCaptive:
					executionSuccess = RansomCaptive(arguments[0]).ResponseType == DisplayMessages.Success;
                    if (executionSuccess)
                    {
						if (!gameState.tryGetNotMyPC(arguments[0], out RedProtoPlayerCharacter captiveRansomed))
							throw new Exception("Shouldn't be null");
						AddEnemy(gameState, captiveRansomed.charID, out gameState);
					}
					break;
				case MainActions.KidnapExecuteCaptive:
					executionSuccess = ExecuteCaptive(arguments[0]).ResponseType == DisplayMessages.Success;
					break;
				case MainActions.KidnapReleaseCaptive:
					executionSuccess = ReleaseCaptive(arguments[0]).ResponseType == DisplayMessages.Success;
					if (executionSuccess)
						charToReleaseIDs.Remove(arguments[0]);
					break;
				case MainActions.KidnapRespondRansom:
					executionSuccess = RespondRansom(uint.Parse(arguments[0]), bool.Parse(arguments[1])).ResponseType == DisplayMessages.Success;
					break;
				case MainActions.None:
					throw new Exception("The action " + actionToExecute.ToString() + " isn't a valid one");
				default:
					throw new Exception("The action " + actionToExecute.ToString() + " isn't taken into account");
			}

            if (executionSuccess)
            {
                switch (actionToExecute)
                {
					case MainActions.NPCmoveDirection:
						if (!Enum.TryParse(arguments[0], out MoveDirections direction))
							throw new Exception("Couldn't parse '" + arguments[0] + "' to a MoveDirections object");
						if (!dayCostToTravelToAdjacentFiefs.TryGetValue(direction, out double costToTravelToAdjacentFief))
							throw new Exception("Couldn't find the key '" + actionToExecute.ToString() + "', it shouldn't happen");
						estimationNbDaysUsed += costToTravelToAdjacentFief;
						break;
					case MainActions.NPCmoveToFief:
						if(!dayCostToTravelToKnownFiefs.TryGetValue(arguments[0], out double dayCostToKnownFief))
							Console.WriteLine("Couldn't find the key '" + arguments[0] + "', the cost wasn't taken into account");
						estimationNbDaysUsed += dayCostToKnownFief;
						break;
					default:
						if (!maxDaysToDoAction.TryGetValue(actionToExecute, out double maxDaysToDoActionValue))
							throw new Exception("The key '" + actionToExecute.ToString() + "' isn't present in this dictionnary, it needs to be added!");
						estimationNbDaysUsed += maxDaysToDoActionValue * 0.75;
						break;
                }
				if (actionToExecute != MainActions.NPCmoveDirection)
					nbConsecutiveNPCmoveDirections = 0;
			}
			gameStateWithModifs = gameState;
			return executionSuccess;
		}

		/// <summary>
		///     Modify the GameState accordingly in the case of the adding of an ally
		/// </summary>
		/// <param name="gameState"></param>
		/// <param name="charId">ID of the char that becomes an ally</param>
		/// <param name="gameStateWithModifs"></param>
		private void AddAlly(GameState gameState, string charId, out GameState gameStateWithModifs)
        {
			gameState.allyPCids.Add(charId);
			gameState.enemyPCids.Remove(charId);
			gameStateWithModifs = gameState;
		}

		/// <summary>
		///     Modify the GameState accordingly in the case of the adding of an enemy
		/// </summary>
		/// <param name="gameState"></param>
		/// <param name="charId">ID of the char that becomes an enemy</param>
		/// <param name="gameStateWithModifs"></param>
		private void AddEnemy(GameState gameState, string charId, out GameState gameStateWithModifs)
		{
			gameState.enemyPCids.Add(charId);
			gameState.allyPCids.Remove(charId);
			gameStateWithModifs = gameState;
		}

		/// <summary>
		///     Search if a char can be barred from one of my fiefs
		/// </summary>
		/// <param name="gameState"></param>
		/// <param name="charID"></param>
		/// <param name="fief"></param>
		/// <returns></returns>
		protected bool CanBarCharFromOneOfMyFiefs(GameState gameState, string charID, out RedProtoFief fief)
		{
			fief = null;
			foreach (RedProtoFief myFief in gameState.myFiefs)
            {
				bool isBarredInThisFief = false;
				if(myFief.barredCharactersId != null)
					foreach (string barredCharId in myFief.barredCharactersId)
						if (barredCharId.Equals(charID))
						{
							isBarredInThisFief = true;
							break;
						}
				if (!isBarredInThisFief)
				{
					fief = myFief;
					return true;
				}
			}
			return false;
		}

		/// <summary>
		///     Calculates my total number of troops
		/// </summary>
		/// <param name="gameState"></param>
		/// <returns>My total number of troops</returns>
		protected uint calculMyTotalNbTroops(GameState gameState)
		{
			uint myTotalNbTroops = 0;
			foreach (RedProtoArmy myArmy in gameState.myArmies)
				myTotalNbTroops += Tools.CalculSumUintArray(myArmy.troops);
			return myTotalNbTroops;
		}

		/// <summary>
		///     Estimates the acceptable salary limit for an NPC
		/// </summary>
		/// <param name="npcToHire">npc from which to estimate the acceptable salary limit</param>
		/// <returns>The acceptable salary limit</returns>
		protected double estimateNPCacceptableSalary(RedProtoNPC npcToHire)
		{
			double basicSalary = 1500;
			double maxRating = Math.Max(Tools.GetLeadershipValue(npcToHire), npcToHire.management);
			double minRating = Math.Min(Tools.GetLeadershipValue(npcToHire), npcToHire.management);

			// calculate potential salary, based on highest rating
			if(minRating == 0)
				return Math.Max(basicSalary, basicSalary * maxRating);
			else
				return Math.Max(basicSalary, basicSalary * (maxRating + minRating / 2));
		}
	}
}
