using System;
using System.Collections.Generic;
using System.Linq;

namespace JominiAI
{
    /// <summary>
	/// Contains functions that can create deep copies of the needed RedProtoMessages (a lighter version of the ProtoMessages)
	/// </summary>
    public static class RedDeepCopier
    {
		/// <summary>
		///     Create a deep copy of a ProtoFief
		/// </summary>
		/// <param name="redProtoFief">ProtoFief to copy</param>
		/// <returns>The deep copy</returns>
		public static RedProtoFief DeepCopyRedProtoFief(RedProtoFief redProtoFief)
		{
			RedProtoFief deepCopy = new RedProtoFief();
			deepCopy.fiefID = redProtoFief.fiefID;
			deepCopy.ownerID = redProtoFief.ownerID;
			deepCopy.militia = redProtoFief.militia;
			deepCopy.status = redProtoFief.status;
			deepCopy.characterInFiefIDs = redProtoFief.characterInFiefIDs != null ? (string[])redProtoFief.characterInFiefIDs.Clone() : null;
			deepCopy.barredCharactersId = redProtoFief.barredCharactersId != null ? (string[])redProtoFief.barredCharactersId.Clone() : null;
			deepCopy.barredNationalities = redProtoFief.barredNationalities != null ? (string[])redProtoFief.barredNationalities.Clone() : null;
			deepCopy.ancestralOwnerId = redProtoFief.ancestralOwnerId;
			deepCopy.treasury = redProtoFief.treasury;
			deepCopy.armyIDs = redProtoFief.armyIDs != null ? (String[])redProtoFief.armyIDs.Clone() : null;
			deepCopy.hasRecruited = redProtoFief.hasRecruited;
			deepCopy.isPillaged = redProtoFief.isPillaged;
			deepCopy.siege = redProtoFief.siege;
			deepCopy.keepLevel = redProtoFief.keepLevel;
			deepCopy.bailiffID = redProtoFief.bailiffID;
			return deepCopy;
		}

		/// <summary>
		///     Create a deep copy of a ProtoCharacter
		/// </summary>
		/// <param name="redProtoCharacter">ProtoFief to copy</param>
		/// <returns>The deep copy</returns>
		public static RedProtoCharacter DeepCopyRedProtoCharacter(RedProtoCharacter redProtoCharacter)
		{
			RedProtoCharacter deepCopy;
			if (redProtoCharacter is RedProtoPlayerCharacter)
				deepCopy = new RedProtoPlayerCharacter();
			else
				deepCopy = new RedProtoNPC();
			deepCopy.charID = redProtoCharacter.charID;
			deepCopy.firstName = redProtoCharacter.firstName;
			deepCopy.familyName = redProtoCharacter.familyName;
			deepCopy.birthYear = redProtoCharacter.birthYear;
			deepCopy.isMale = redProtoCharacter.isMale;
			deepCopy.nationality = redProtoCharacter.nationality;
			deepCopy.isAlive = redProtoCharacter.isAlive;
			deepCopy.health = redProtoCharacter.health;
			deepCopy.inKeep = redProtoCharacter.inKeep;
			deepCopy.language = redProtoCharacter.language;
			deepCopy.days = redProtoCharacter.days;
			deepCopy.familyID = redProtoCharacter.familyID;
			deepCopy.spouse = redProtoCharacter.spouse;
			deepCopy.fiancee = redProtoCharacter.fiancee;
			deepCopy.location = redProtoCharacter.location;
			deepCopy.management = redProtoCharacter.management;
			deepCopy.combat = redProtoCharacter.combat;
			deepCopy.isPregnant = redProtoCharacter.isPregnant;
			deepCopy.titles = redProtoCharacter.titles != null ? (String[])redProtoCharacter.titles.Clone() : null;
			deepCopy.armyID = redProtoCharacter.armyID;
			deepCopy.captor = redProtoCharacter.captor;
			deepCopy.father = redProtoCharacter.father;
			return deepCopy;
		}

		/// <summary>
		///     Create a deep copy of a ProtoPlayerCharacter
		/// </summary>
		/// <param name="protoPlayerCharacter">ProtoPlayerCharacter to copy</param>
		/// <returns>The deep copy</returns>
		public static RedProtoPlayerCharacter DeepCopyRedProtoPlayerCharacter(RedProtoPlayerCharacter protoPlayerCharacter)
		{
			RedProtoPlayerCharacter deepCopy = (RedProtoPlayerCharacter) DeepCopyRedProtoCharacter(protoPlayerCharacter);
			deepCopy.playerID = protoPlayerCharacter.playerID;
			deepCopy.purse = protoPlayerCharacter.purse;
			deepCopy.myHeirId = protoPlayerCharacter.myHeirId;
			deepCopy.ownedFiefIds = protoPlayerCharacter.ownedFiefIds != null ? (String[])protoPlayerCharacter.ownedFiefIds.Clone() : null;
			deepCopy.homeFief = protoPlayerCharacter.homeFief;
			deepCopy.ancestralHomeFief = protoPlayerCharacter.ancestralHomeFief;
			return deepCopy;
		}

		/// <summary>
		///     Create a deep copy of a ProtoNPC
		/// </summary>
		/// <param name="redProtoNPC">ProtoNPC to copy</param>
		/// <returns>The deep copy</returns>
		public static RedProtoNPC DeepCopyRedProtoNPC(RedProtoNPC redProtoNPC)
		{
			RedProtoNPC deepCopy = (RedProtoNPC)DeepCopyRedProtoCharacter(redProtoNPC);
			deepCopy.employerId = redProtoNPC.employerId;
			deepCopy.salary = redProtoNPC.salary;
			deepCopy.inEntourage = redProtoNPC.inEntourage;
			deepCopy.isHeir = redProtoNPC.isHeir;
			return deepCopy;
		}

		/// <summary>
		///     Create a deep copy of a ProtoPlayer
		/// </summary>
		/// <param name="protoProtoPlayer">ProtoFief to copy</param>
		/// <returns>The deep copy</returns>
		public static RedProtoPlayer DeepCopyRedProtoPlayer(RedProtoPlayer protoPlayer)
		{
			RedProtoPlayer deepCopy = new RedProtoPlayer();
			deepCopy.pcID = protoPlayer.pcID;
			deepCopy.playerID = protoPlayer.playerID;
			return deepCopy;
		}

		/// <summary>
		///     Create a deep copy of a ProtoArmy
		/// </summary>
		/// <param name="protoArmy">ProtoFief to copy</param>
		/// <returns>The deep copy</returns>
		public static RedProtoArmy DeepCopyRedProtoArmy(RedProtoArmy protoArmy)
		{
			RedProtoArmy deepCopy = new RedProtoArmy();
			deepCopy.armyID = protoArmy.armyID;
			deepCopy.troops = protoArmy.troops != null ? (uint[])protoArmy.troops.Clone() : null;
			deepCopy.leaderName = protoArmy.leaderName;
			deepCopy.ownerName = protoArmy.ownerName;
			deepCopy.days = protoArmy.days;
			deepCopy.location = protoArmy.location;
			deepCopy.isMaintained = protoArmy.isMaintained;
			deepCopy.maintCost = protoArmy.maintCost;
			deepCopy.siegeStatus = protoArmy.siegeStatus;
			return deepCopy;
		}

		/// <summary>
		///     Create a deep copy of a ProtoDetachment
		/// </summary>
		/// <param name="protoDetachment">ProtoFief to copy</param>
		/// <returns>The deep copy</returns>
		public static RedProtoDetachment DeepCopyRedProtoDetachment(RedProtoDetachment protoDetachment)
		{
			RedProtoDetachment deepCopy = new RedProtoDetachment();
			deepCopy.id = protoDetachment.id;
			deepCopy.troops = protoDetachment.troops != null ? (uint[])protoDetachment.troops.Clone() : null;
			deepCopy.leftFor = protoDetachment.leftFor;
			deepCopy.leftBy = protoDetachment.leftBy;
			deepCopy.days = protoDetachment.days;
			return deepCopy;
		}


		/// <summary>
		///     Create a deep copy of a ProtoSiegeOverview
		/// </summary>
		/// <param name="redProtoSiegeDisplay">ProtoFief to copy</param>
		/// <returns>The deep copy</returns>
		public static RedProtoSiegeDisplay DeepCopyRedProtoSiegeDisplay(RedProtoSiegeDisplay redProtoSiegeDisplay)
		{
			RedProtoSiegeDisplay deepCopy = new RedProtoSiegeDisplay();
			deepCopy.siegeID = redProtoSiegeDisplay.siegeID;
			deepCopy.besiegedFiefID = redProtoSiegeDisplay.besiegedFiefID;
			deepCopy.besiegingPlayer = redProtoSiegeDisplay.besiegingPlayer;
			deepCopy.defendingPlayer = redProtoSiegeDisplay.defendingPlayer;
			deepCopy.besiegerArmyID = redProtoSiegeDisplay.besiegerArmyID;
			deepCopy.defenderGarrisonID = redProtoSiegeDisplay.defenderGarrisonID;
			deepCopy.keepLevel = redProtoSiegeDisplay.keepLevel;
			deepCopy.defenderAdditional = redProtoSiegeDisplay.defenderAdditional;
			return deepCopy;
		}

		/// <summary>
		///     Create a deep copy of a RedProtoJournalEntry
		/// </summary>
		/// <param name="redProtoJournalEntry">ProtoFief to copy</param>
		/// <returns>The deep copy</returns>
		public static RedProtoJournalEntry DeepCopyRedProtoJournalEntry(RedProtoJournalEntry redProtoJournalEntry)
        {
			RedProtoJournalEntry deepCopy = new RedProtoJournalEntry();
			deepCopy.jEntryID = redProtoJournalEntry.jEntryID;
			deepCopy.personaeIds = (string[])redProtoJournalEntry.personaeIds.Clone();
			deepCopy.type = redProtoJournalEntry.type;
			deepCopy.location = redProtoJournalEntry.location;
			deepCopy.viewed = redProtoJournalEntry.viewed;
			deepCopy.replied = redProtoJournalEntry.replied;
			return deepCopy;
		}

		/// <summary>
		///     Create a deep copy of a RedProtoCharacterOverview
		/// </summary>
		/// <param name="redProtoCharacterOverview">ProtoFief to copy</param>
		/// <returns>The deep copy</returns>
		public static RedProtoCharacterOverview DeepCopyRedProtoCharacterOverview(RedProtoCharacterOverview redProtoCharacterOverview)
		{
			RedProtoCharacterOverview deepCopy = new RedProtoCharacterOverview();
			deepCopy.charID = redProtoCharacterOverview.charID;
			deepCopy.ownerName = redProtoCharacterOverview.ownerName;
			deepCopy.locationID = redProtoCharacterOverview.locationID;
			return deepCopy;
		}
	}
}