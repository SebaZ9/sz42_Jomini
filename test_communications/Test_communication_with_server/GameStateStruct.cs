using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace JominiAI
{
	/// <summary>
	///     The GameState contains all the necessary information for an agent to decide what action to take.
	/// </summary>
	public struct GameState
	{
		public HashSet<String> enemyPCids;
		public HashSet<String> allyPCids;
		public List<RedProtoPlayer> ExistingPlayers;
		public List<RedProtoDetachment> armyDetachmentsForMe;
		public List<RedProtoSiegeDisplay> siegeOnMyFiefs;
		public bool hasReceivedRansomForMyPC;
		public uint ransomJentryID;
		public int currentYear;   // That's an estimation based on the most recent journal entry date sent by the server
		public List<RedProtoJournalEntry> unreadJournalEntries;

		public List<RedProtoFief> notMyFiefs;
		public List<RedProtoPlayerCharacter> notMyPCs;
		public List<RedProtoNPC> notMyNPCs;
		public List<RedProtoArmy> notMyArmies;
		public List<RedProtoArmy> notMyGarrisons;

		public Dictionary<string, RedProtoFief> spiedFiefs;
		public Dictionary<string, RedProtoCharacter> spiedCharacters;
		public Dictionary<string, RedProtoArmy> spiedArmies;

		public RedProtoPlayerCharacter myPC;
		public List<RedProtoNPC> myFamilyNPCs;
		public List<RedProtoNPC> myEmployeeNPCs;
		public List<RedProtoFief> myFiefs;
		public List<RedProtoArmy> myArmies;
		public List<RedProtoSiegeDisplay> mySieges;
		public HashSet<String> myCaptiveIDs;
		public RedProtoCharacterOverview receivedProposalFromThisChar;
		public Dictionary<string, bool> myCharsWhoProposedThisSeason;
		public string myCurrentPCid;

		/// <summary>
		///     It creates a deep copy of the GameState and fill copy only the relevant fields (e.g. actionType and responseType are not copied).
		///     It is used by the agents to simulate the possible outcomes by creating trees that are composed of GameStates.
		///     None of the variables of the structure is supposed to be null.
		/// </summary>
		/// <returns>A deep copy of the GameState</returns>
		public GameState deepCopy()
		{
			GameState deepCopyGameState = new GameState();

			deepCopyGameState.enemyPCids = new HashSet<String>(this.enemyPCids);
			deepCopyGameState.allyPCids = new HashSet<String>(this.allyPCids);
			deepCopyGameState.ExistingPlayers = new List<RedProtoPlayer>();
			foreach (RedProtoPlayer player in this.ExistingPlayers)
				deepCopyGameState.ExistingPlayers.Add(RedDeepCopier.DeepCopyRedProtoPlayer(player));
			deepCopyGameState.armyDetachmentsForMe = new List<RedProtoDetachment>();
			foreach (RedProtoDetachment detachmentInCurrentFief in this.armyDetachmentsForMe)
				deepCopyGameState.armyDetachmentsForMe.Add(RedDeepCopier.DeepCopyRedProtoDetachment(detachmentInCurrentFief));
			deepCopyGameState.armyDetachmentsForMe = new List<RedProtoDetachment>();
			foreach (RedProtoDetachment detachmentInCurrentFief in this.armyDetachmentsForMe)
				deepCopyGameState.armyDetachmentsForMe.Add(RedDeepCopier.DeepCopyRedProtoDetachment(detachmentInCurrentFief));
			deepCopyGameState.siegeOnMyFiefs = new List<RedProtoSiegeDisplay>();
			foreach (RedProtoSiegeDisplay notMySiege in this.siegeOnMyFiefs)
				deepCopyGameState.siegeOnMyFiefs.Add(RedDeepCopier.DeepCopyRedProtoSiegeDisplay(notMySiege));
			deepCopyGameState.hasReceivedRansomForMyPC = this.hasReceivedRansomForMyPC;
			deepCopyGameState.ransomJentryID = this.ransomJentryID;
			deepCopyGameState.currentYear = this.currentYear;
			deepCopyGameState.unreadJournalEntries = new List<RedProtoJournalEntry>();
			foreach (RedProtoJournalEntry unreadJournalEntry in this.unreadJournalEntries)
				deepCopyGameState.unreadJournalEntries.Add(RedDeepCopier.DeepCopyRedProtoJournalEntry(unreadJournalEntry));

			deepCopyGameState.notMyFiefs = new List<RedProtoFief>();
			foreach (RedProtoFief notMyFief in this.notMyFiefs)
				deepCopyGameState.notMyFiefs.Add(RedDeepCopier.DeepCopyRedProtoFief(notMyFief));
			deepCopyGameState.notMyPCs = new List<RedProtoPlayerCharacter>();
			foreach (RedProtoPlayerCharacter notMyPc in this.notMyPCs)
				deepCopyGameState.notMyPCs.Add(RedDeepCopier.DeepCopyRedProtoPlayerCharacter(notMyPc));
			deepCopyGameState.notMyNPCs = new List<RedProtoNPC>();
			foreach (RedProtoNPC notMyNpc in this.notMyNPCs)
				deepCopyGameState.notMyNPCs.Add(RedDeepCopier.DeepCopyRedProtoNPC(notMyNpc));
			deepCopyGameState.notMyArmies = new List<RedProtoArmy>();
			foreach (RedProtoArmy notMyArmy in this.notMyArmies)
				deepCopyGameState.notMyArmies.Add(RedDeepCopier.DeepCopyRedProtoArmy(notMyArmy));
			deepCopyGameState.notMyGarrisons = new List<RedProtoArmy>();
			foreach (RedProtoArmy notMyGarrison in this.notMyGarrisons)
				deepCopyGameState.notMyGarrisons.Add(RedDeepCopier.DeepCopyRedProtoArmy(notMyGarrison));

			deepCopyGameState.spiedFiefs = new Dictionary<string, RedProtoFief>();
			foreach (string spiedFiefKey in this.spiedFiefs.Keys)
            {
				this.spiedFiefs.TryGetValue(spiedFiefKey, out RedProtoFief spiedFiefValue);
				deepCopyGameState.spiedFiefs.Add(spiedFiefKey, RedDeepCopier.DeepCopyRedProtoFief(spiedFiefValue));
			}
			deepCopyGameState.spiedCharacters = new Dictionary<string, RedProtoCharacter>();
			foreach (string spiedCharacterKey in this.spiedCharacters.Keys)
			{
				this.spiedCharacters.TryGetValue(spiedCharacterKey, out RedProtoCharacter spiedCharacterValue);
				deepCopyGameState.spiedCharacters.Add(spiedCharacterKey, RedDeepCopier.DeepCopyRedProtoCharacter(spiedCharacterValue));
			}
			deepCopyGameState.spiedArmies = new Dictionary<string, RedProtoArmy>();
			foreach (string spiedArmiesKey in this.spiedArmies.Keys)
			{
				this.spiedArmies.TryGetValue(spiedArmiesKey, out RedProtoArmy spiedArmiesValue);
				deepCopyGameState.spiedArmies.Add(spiedArmiesKey, RedDeepCopier.DeepCopyRedProtoArmy(spiedArmiesValue));
			}

			deepCopyGameState.myPC = RedDeepCopier.DeepCopyRedProtoPlayerCharacter(this.myPC);
			deepCopyGameState.myFamilyNPCs = new List<RedProtoNPC>();
			foreach (RedProtoNPC myNpc in this.myFamilyNPCs)
				deepCopyGameState.myFamilyNPCs.Add(RedDeepCopier.DeepCopyRedProtoNPC(myNpc));
			deepCopyGameState.myEmployeeNPCs = new List<RedProtoNPC>();
			foreach (RedProtoNPC myNpc in this.myEmployeeNPCs)
				deepCopyGameState.myEmployeeNPCs.Add(RedDeepCopier.DeepCopyRedProtoNPC(myNpc));
			deepCopyGameState.myFiefs = new List<RedProtoFief>();
			foreach (RedProtoFief myFief in this.myFiefs)
				deepCopyGameState.myFiefs.Add(RedDeepCopier.DeepCopyRedProtoFief(myFief));
			deepCopyGameState.myArmies = new List<RedProtoArmy>();
			foreach (RedProtoArmy myArmy in this.myArmies)
				deepCopyGameState.myArmies.Add(RedDeepCopier.DeepCopyRedProtoArmy(myArmy));
			deepCopyGameState.mySieges = new List<RedProtoSiegeDisplay>();
			foreach (RedProtoSiegeDisplay mySiege in this.mySieges)
				deepCopyGameState.mySieges.Add(RedDeepCopier.DeepCopyRedProtoSiegeDisplay(mySiege));
			deepCopyGameState.myCaptiveIDs = new HashSet<string>(this.myCaptiveIDs);
			/*deepCopyGameState.myUnreadJournalEntryList = new List<RedProtoJournalEntry>();
			foreach (RedProtoJournalEntry myUnreadJournalEntry in this.myUnreadJournalEntryList)
				deepCopyGameState.myUnreadJournalEntryList.Add(RedDeepCopier.DeepCopyRedProtoJournalEntry(myUnreadJournalEntry));*/
			if(this.receivedProposalFromThisChar != null)
				deepCopyGameState.receivedProposalFromThisChar = RedDeepCopier.DeepCopyRedProtoCharacterOverview(this.receivedProposalFromThisChar);
			deepCopyGameState.myCharsWhoProposedThisSeason = new Dictionary<string, bool>();
			foreach (string myCharWhoProposedKey in this.myCharsWhoProposedThisSeason.Keys)
			{
				this.myCharsWhoProposedThisSeason.TryGetValue(myCharWhoProposedKey, out bool myCharWhoProposedValue);
				deepCopyGameState.myCharsWhoProposedThisSeason.Add(myCharWhoProposedKey, myCharWhoProposedValue);
			}
			deepCopyGameState.myCurrentPCid = this.myCurrentPCid;

			return deepCopyGameState;
		}

		/// <summary>
		///     
		/// </summary>
		/// <returns></returns>
		public RedProtoFief getCurrentFief()
		{
			RedProtoCharacter currentCharacter = this.myPC;
			if (tryGetFief(currentCharacter.location, out RedProtoFief currentFief))
				return currentFief;
			else
				throw new Exception("Couldn't find the current fief");
		}

		/// <summary>
		///     
		/// </summary>
		/// <returns></returns>
		public bool tryGetMainArmy(out RedProtoArmy mainArmy)
		{
			RedProtoArmy biggestArmy = null;
			uint biggestArmyNbTroops = uint.MinValue;
			if (myArmies.Count > 0)
            {
				foreach (RedProtoArmy myArmy in myArmies)
                {
					uint nbTroops = Tools.CalculSumUintArray(myArmy.troops);
					if (nbTroops > biggestArmyNbTroops)
					{
						biggestArmy = myArmy;
						biggestArmyNbTroops = nbTroops;
					}
				}
				mainArmy = biggestArmy;
				return true;
			}
            else
            {
				mainArmy = null;
				return false;
			}
		}

		/// <summary>
		///     The only way to get an army fiefID is to find the leader and get his location
		/// </summary>
		/// <returns></returns>
		public bool tryGetArmyFiefID(string armyID, out string armyFiefID)
		{
			if(tryGetArmy(armyID, out RedProtoArmy army))
				if (!string.IsNullOrWhiteSpace(army.leaderName))
				{
					foreach(RedProtoCharacter myCharacter in this.myEmployeeNPCs.Concat(this.myFamilyNPCs).Concat(new RedProtoCharacter[] { this.myPC }))
						if (army.leaderName.Equals(myCharacter.firstName + " " + myCharacter.familyName))
						{
							armyFiefID = myCharacter.location;
							return true;
						}
					throw new Exception("Couldn't find the character '" + army.leaderName + "', it shouldn't happen");
				}
			armyFiefID = null;
			return false;
		}

		/// <summary>
		///     
		/// </summary>
		/// <returns></returns>
		public bool tryGetMainArmyFiefID(out string mainArmyFiefID)
		{
			if (tryGetMainArmy(out RedProtoArmy mainArmy))
				if (tryGetArmyFiefID(mainArmy.armyID, out mainArmyFiefID))
					return true;
			mainArmyFiefID = null;
			return false;
		}

		/// <summary>
		///     
		/// </summary>
		/// <returns></returns>
		public bool tryGetArmy(string armyId, out RedProtoArmy redProtoArmy)
		{
			foreach (RedProtoArmy army in this.myArmies.Concat(this.notMyArmies))
			{
				if (army.armyID.Equals(armyId))
                {
					redProtoArmy = army;
					return true;
				}
			}
			redProtoArmy = null;
			return false;
		}

		/// <summary>
		///     
		/// </summary>
		/// <returns></returns>
		public RedProtoFief getHomeFief()
		{
			RedProtoPlayerCharacter currentChar = this.myPC;
			if (tryGetFief(currentChar.homeFief, out RedProtoFief homeFief))
				return homeFief;
			else
				throw new Exception("Couldn't find home fief");
		}

		/// <summary>
		///     
		/// </summary>
		/// <returns></returns>
		public bool tryGetFief(string fiefID, out RedProtoFief redProtoFief)
		{
			foreach (RedProtoFief knownFief in this.myFiefs.Concat(this.notMyFiefs))
			{
				if (knownFief.fiefID.Equals(fiefID))
                {
					redProtoFief = knownFief;
					return true;
				}
			}
			redProtoFief = null;
			return false;
		}

		/// <summary>
		///     
		/// </summary>
		/// <returns></returns>
		public bool tryGetCharacter(string charId, out RedProtoCharacter redProtoCharacter)
		{
			foreach (RedProtoCharacter character in
				new RedProtoCharacter[] { this.myPC }.Cast<RedProtoCharacter>().Concat(this.myFamilyNPCs).Concat(this.myEmployeeNPCs).Concat(this.notMyPCs).Concat(this.notMyNPCs))
			{
				if (character.charID.Equals(charId))
                {
					redProtoCharacter = character;
					return true;
				}
			}
			redProtoCharacter = null;
			return false;
		}

		/// <summary>
		///     
		/// </summary>
		/// <returns></returns>
		public bool tryGetNotMyPC(string charId, out RedProtoPlayerCharacter redProtoPlayerCharacter)
		{
			foreach (RedProtoPlayerCharacter notMyPC in this.notMyPCs)
			{
				if (notMyPC.charID.Equals(charId))
                {
					redProtoPlayerCharacter = notMyPC;
					return true;
				}
			}
			redProtoPlayerCharacter = null;
			return false;
		}

		/// <summary>
		///     
		/// </summary>
		/// <returns></returns>
		public bool tryGetNotMyNPC(string charId, out RedProtoNPC redProtoNPC)
		{
			foreach (RedProtoNPC notMyNPC in this.notMyNPCs)
			{
				if (notMyNPC.charID.Equals(charId))
				{
					redProtoNPC = notMyNPC;
					return true;
				}
			}
			redProtoNPC = null;
			return false;
		}

		/// <summary>
		///     
		/// </summary>
		/// <returns></returns>
		public bool tryGetMySiege(string siegeID, out RedProtoSiegeDisplay redProtoSiegeDisplay)
		{
			foreach (RedProtoSiegeDisplay mySiege in this.mySieges)
			{
				if (mySiege.siegeID.Equals(siegeID))
				{
					redProtoSiegeDisplay = mySiege;
					return true;
				}
			}
			redProtoSiegeDisplay = null;
			return false;
		}

		/// <summary>
		///     
		/// </summary>
		/// <returns></returns>
		public bool tryGetSiegeAgainstMe(string siegeID, out RedProtoSiegeDisplay redProtoSiegeDisplay)
		{
			foreach (RedProtoSiegeDisplay siegeAgainstMe in this.siegeOnMyFiefs)
			{
				if (siegeAgainstMe.siegeID.Equals(siegeID))
				{
					redProtoSiegeDisplay = siegeAgainstMe;
					return true;
				}
			}
			redProtoSiegeDisplay = null;
			return false;
		}

		/// <summary>
		///     
		/// </summary>
		/// <returns></returns>
		public bool tryGetNotMyGarrison(string garrisonID, out RedProtoArmy redProtoArmy)
		{
			foreach (RedProtoArmy notMyGarrison in this.notMyGarrisons)
			{
				if (notMyGarrison.armyID.Equals(garrisonID))
				{
					redProtoArmy = notMyGarrison;
					return true;
				}
			}
			redProtoArmy = null;
			return false;
		}
	}
}
