using ProtoMessageClient;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JominiAI
{
	/// <summary>
	///		Not used anymore, replaced by RedDeepCopier.cs
	///		Contains functions that can create deep copies of the needed ProtoMessages
	/// </summary>
	public static class DeepCopier
    {
		/// <summary>
		///     Create a deep copy of a ProtoFief
		/// </summary>
		/// <param name="protoFief">ProtoFief to copy</param>
		/// <returns>The deep copy</returns>
		public static ProtoFief DeepCopyProtoFief(ProtoFief protoFief)
		{
			ProtoFief deepCopy = new ProtoFief();
			deepCopy.ancestralOwner = protoFief.ancestralOwner != null ? DeepCopyProtoCharacterOverview(protoFief.ancestralOwner) : null;
			if (protoFief.armies != null)
			{
				deepCopy.armies = new ProtoArmyOverview[protoFief.armies.Length];
				for (int i = 0; i < deepCopy.armies.Length; i++)
					deepCopy.armies[i] = protoFief.armies[i] != null ? DeepCopyProtoArmyOverview(protoFief.armies[i]) : null;
			}
			deepCopy.bailiff = protoFief.bailiff != null ? DeepCopyProtoCharacterOverview(protoFief.bailiff) : null;
			deepCopy.bailiffDaysInFief = protoFief.bailiffDaysInFief;
			if (protoFief.barredCharacters != null)
			{
				deepCopy.barredCharacters = new ProtoCharacterOverview[protoFief.barredCharacters.Length];
				for (int i = 0; i < deepCopy.barredCharacters.Length; i++)
					deepCopy.barredCharacters[i] = protoFief.barredCharacters[i] != null ? DeepCopyProtoCharacterOverview(protoFief.barredCharacters[i]) : null;
			}
			deepCopy.barredNationalities = protoFief.barredNationalities != null ? (String[])protoFief.barredNationalities.Clone() : null;
			if (protoFief.charactersInFief != null)
			{
				deepCopy.charactersInFief = new ProtoCharacterOverview[protoFief.charactersInFief.Length];
				for (int i = 0; i < deepCopy.charactersInFief.Length; i++)
					deepCopy.charactersInFief[i] = protoFief.charactersInFief[i] != null ? DeepCopyProtoCharacterOverview(protoFief.charactersInFief[i]) : null;
			}
			deepCopy.fiefID = protoFief.fiefID;
			deepCopy.fields = protoFief.fields;
			if (protoFief.gaol != null)
			{
				deepCopy.gaol = new ProtoCharacterOverview[protoFief.gaol.Length];
				for (int i = 0; i < deepCopy.gaol.Length; i++)
					deepCopy.gaol[i] = protoFief.gaol[i] != null ? DeepCopyProtoCharacterOverview(protoFief.gaol[i]) : null;
			}
			deepCopy.garrisonSpendNext = protoFief.garrisonSpendNext;
			deepCopy.hasRecruited = protoFief.hasRecruited;
			deepCopy.industry = protoFief.industry;
			deepCopy.infrastructureSpendNext = protoFief.infrastructureSpendNext;
			deepCopy.isPillaged = protoFief.isPillaged;
			deepCopy.keepLevel = protoFief.keepLevel;
			deepCopy.keepSpendNext = protoFief.keepSpendNext;
			deepCopy.keyStatsCurrent = protoFief.keyStatsCurrent != null ? (double[])protoFief.keyStatsCurrent.Clone() : null;
			deepCopy.keyStatsNext = protoFief.keyStatsNext != null ? (double[])protoFief.keyStatsNext.Clone() : null;
			deepCopy.keyStatsPrevious = protoFief.keyStatsPrevious != null ? (double[])protoFief.keyStatsPrevious.Clone() : null;
			deepCopy.loyalty = protoFief.loyalty;
			deepCopy.militia = protoFief.militia;
			deepCopy.officialsSpendNext = protoFief.officialsSpendNext;
			deepCopy.owner = protoFief.owner;
			deepCopy.ownerID = protoFief.ownerID;
			deepCopy.population = protoFief.population;
			deepCopy.rank = protoFief.rank;
			deepCopy.siege = protoFief.siege;
			deepCopy.status = protoFief.status;
			deepCopy.taxRate = protoFief.taxRate;
			deepCopy.taxRateNext = protoFief.taxRateNext;
			deepCopy.titleHolder = protoFief.titleHolder;
			deepCopy.treasury = protoFief.treasury;
			deepCopy.troops = protoFief.troops;

			/*protoFief.barredNationalities = new string[]{ "TEST barredNationalities"};
			Console.WriteLine("protoArmyOverview.leaderID[0]= " + protoFief.barredNationalities[0]);
			Console.WriteLine("deepCopy.leaderID[0]= " + deepCopy.barredNationalities[0]);*/

			return deepCopy;
		}

		/// <summary>
		///     Create a deep copy of a ProtoCharacter
		/// </summary>
		/// <param name="protoCharacter">ProtoFief to copy</param>
		/// <returns>The deep copy</returns>
		public static ProtoCharacter DeepCopyProtoCharacter(ProtoCharacter protoCharacter, bool isProtoNPC = false)
		{
			ProtoCharacter deepCopy;
			if (!isProtoNPC)
				deepCopy = new ProtoPlayerCharacter(); // We create a ProtoPlayerCharacter so that the ProtoCharacter can be then be cast into a ProtoPlayerCharacter
			else
				deepCopy = new ProtoNPC(); // We create a ProtoNPC so that the ProtoCharacter can be then be cast into a ProtoNPC

			if (protoCharacter.ailments != null)
			{
				deepCopy.ailments = new Pair[protoCharacter.ailments.Length];
				for (int i = 0; i < deepCopy.ailments.Length; i++)
					deepCopy.ailments[i] = protoCharacter.ailments[i] != null ? DeepCopyPair(protoCharacter.ailments[i]) : null;
			}
			deepCopy.armyID = protoCharacter.armyID;
			deepCopy.birthSeason = protoCharacter.birthSeason;
			deepCopy.birthYear = protoCharacter.birthYear;
			deepCopy.captor = protoCharacter.captor;
			deepCopy.charID = protoCharacter.charID;
			deepCopy.combat = protoCharacter.combat;
			deepCopy.days = protoCharacter.days;
			deepCopy.familyID = protoCharacter.familyID;
			deepCopy.familyName = protoCharacter.familyName;
			deepCopy.father = protoCharacter.father;
			deepCopy.fiancee = protoCharacter.fiancee;
			deepCopy.firstName = protoCharacter.firstName;
			deepCopy.goTo = protoCharacter.goTo != null ? (String[])protoCharacter.goTo.Clone() : null;
			deepCopy.health = protoCharacter.health;
			deepCopy.inKeep = protoCharacter.inKeep;
			deepCopy.isAlive = protoCharacter.isAlive;
			deepCopy.isMale = protoCharacter.isMale;
			deepCopy.isPregnant = protoCharacter.isPregnant;
			deepCopy.language = protoCharacter.language;
			deepCopy.location = protoCharacter.location;
			deepCopy.management = protoCharacter.management;
			deepCopy.maxHealth = protoCharacter.maxHealth;
			deepCopy.mother = protoCharacter.mother;
			deepCopy.nationality = protoCharacter.nationality;
			deepCopy.siegeRole = protoCharacter.siegeRole;
			deepCopy.spouse = protoCharacter.spouse;
			deepCopy.stature = protoCharacter.stature;
			deepCopy.statureModifier = protoCharacter.statureModifier;
			deepCopy.titles = protoCharacter.titles != null ? (String[])protoCharacter.titles.Clone() : null;
			if (protoCharacter.traits != null)
			{
				deepCopy.traits = new Pair[protoCharacter.traits.Length];
				for (int i = 0; i < deepCopy.traits.Length; i++)
					deepCopy.traits[i] = protoCharacter.traits[i] != null ? DeepCopyPair(protoCharacter.traits[i]) : null;
			}
			deepCopy.virility = protoCharacter.virility;
			return deepCopy;
		}

		/// <summary>
		///     Create a deep copy of a ProtoPlayerCharacter
		/// </summary>
		/// <param name="protoPlayerCharacter">ProtoPlayerCharacter to copy</param>
		/// <returns>The deep copy</returns>
		public static ProtoPlayerCharacter DeepCopyProtoPlayerCharacter(ProtoPlayerCharacter protoPlayerCharacter)
		{
			ProtoPlayerCharacter deepCopy = (ProtoPlayerCharacter) DeepCopyProtoCharacter(protoPlayerCharacter);
			deepCopy.playerID = protoPlayerCharacter.playerID;
			deepCopy.outlawed = protoPlayerCharacter.outlawed;
			deepCopy.purse = protoPlayerCharacter.purse;
			if (protoPlayerCharacter.myNPCs != null)
			{
				deepCopy.myNPCs = new ProtoCharacterOverview[protoPlayerCharacter.myNPCs.Length];
				for (int i = 0; i < deepCopy.myNPCs.Length; i++)
					deepCopy.myNPCs[i] = protoPlayerCharacter.myNPCs[i] != null ? DeepCopyProtoCharacterOverview(protoPlayerCharacter.myNPCs[i]) : null;
			}
			deepCopy.myHeir = protoPlayerCharacter.myHeir != null ? DeepCopyProtoCharacterOverview(protoPlayerCharacter.myHeir) : null; ;
			deepCopy.ownedFiefs = protoPlayerCharacter.ownedFiefs != null ? (String[])protoPlayerCharacter.ownedFiefs.Clone() : null;
			deepCopy.provinces = protoPlayerCharacter.provinces != null ? (String[])protoPlayerCharacter.provinces.Clone() : null; ;
			deepCopy.homeFief = protoPlayerCharacter.homeFief;
			deepCopy.ancestralHomeFief = protoPlayerCharacter.ancestralHomeFief;
			return deepCopy;
		}

		/// <summary>
		///     Create a deep copy of a ProtoNPC
		/// </summary>
		/// <param name="protoNPC">ProtoNPC to copy</param>
		/// <returns>The deep copy</returns>
		public static ProtoNPC DeepCopyProtoNPC(ProtoNPC protoNPC)
		{
			ProtoNPC deepCopy = (ProtoNPC)DeepCopyProtoCharacter(protoNPC, true);
			deepCopy.employer = protoNPC.employer != null ? DeepCopyProtoCharacterOverview(protoNPC.employer) : null;
			deepCopy.salary = protoNPC.salary;
			deepCopy.lastOfferID = protoNPC.lastOfferID;
			deepCopy.lastOfferAmount = protoNPC.lastOfferAmount;
			deepCopy.inEntourage = protoNPC.inEntourage;
			deepCopy.isHeir = protoNPC.isHeir;
			return deepCopy;
		}

		/// <summary>
		///     Create a deep copy of a ProtoCharacterOverview
		/// </summary>
		/// <param name="protoCharacterOverview">ProtoFief to copy</param>
		/// <returns>The deep copy</returns>
		public static ProtoCharacterOverview DeepCopyProtoCharacterOverview(ProtoCharacterOverview protoCharacterOverview)
		{
			ProtoCharacterOverview deepCopy = new ProtoCharacterOverview();
			deepCopy.charID = protoCharacterOverview.charID;
			deepCopy.charName = protoCharacterOverview.charName;
			deepCopy.isMale = protoCharacterOverview.isMale;
			deepCopy.locationID = protoCharacterOverview.locationID;
			deepCopy.natID = protoCharacterOverview.natID;
			deepCopy.owner = protoCharacterOverview.owner;
			deepCopy.role = protoCharacterOverview.role;
			return deepCopy;
		}

		/// <summary>
		///     Create a deep copy of a ProtoPlayer
		/// </summary>
		/// <param name="protoProtoPlayer">ProtoFief to copy</param>
		/// <returns>The deep copy</returns>
		public static ProtoPlayer DeepCopyProtoPlayer(ProtoPlayer protoPlayer)
		{
			ProtoPlayer deepCopy = new ProtoPlayer();
			deepCopy.natID = protoPlayer.natID;
			deepCopy.pcID = protoPlayer.pcID;
			deepCopy.pcName = protoPlayer.pcName;
			deepCopy.playerID = protoPlayer.playerID;
			return deepCopy;
		}

		/// <summary>
		///     Create a deep copy of a ProtoArmy
		/// </summary>
		/// <param name="protoArmy">ProtoFief to copy</param>
		/// <returns>The deep copy</returns>
		public static ProtoArmy DeepCopyProtoArmy(ProtoArmy protoArmy)
		{
			ProtoArmy deepCopy = new ProtoArmy();
			deepCopy.aggression = protoArmy.aggression;
			deepCopy.armyID = protoArmy.armyID;
			deepCopy.combatOdds = protoArmy.combatOdds;
			deepCopy.days = protoArmy.days;
			deepCopy.isMaintained = protoArmy.isMaintained;
			deepCopy.leader = protoArmy.leader;
			deepCopy.leaderID = protoArmy.leaderID;
			deepCopy.location = protoArmy.location;
			deepCopy.maintCost = protoArmy.maintCost;
			deepCopy.nationality = protoArmy.nationality;
			deepCopy.owner = protoArmy.owner;
			deepCopy.ownerID = protoArmy.ownerID;
			deepCopy.siegeStatus = protoArmy.siegeStatus;
			deepCopy.troops = protoArmy.troops != null ? (uint[])protoArmy.troops.Clone() : null;
			return deepCopy;
		}

		/// <summary>
		///     Create a deep copy of a ProtoArmyOverview
		/// </summary>
		/// <param name="protoArmyOverview">ProtoFief to copy</param>
		/// <returns>The deep copy</returns>
		public static ProtoArmyOverview DeepCopyProtoArmyOverview(ProtoArmyOverview protoArmyOverview)
		{
			ProtoArmyOverview deepCopy = new ProtoArmyOverview();
			deepCopy.armyID = protoArmyOverview.armyID;
			deepCopy.armySize = protoArmyOverview.armySize;
			deepCopy.leaderID = protoArmyOverview.leaderID;
			deepCopy.leaderName = protoArmyOverview.leaderName;
			deepCopy.locationID = protoArmyOverview.locationID;
			deepCopy.ownerName = protoArmyOverview.ownerName;

			/*protoArmyOverview.leaderID = "TEST leaderID";
			Console.WriteLine("protoArmyOverview.leaderID= " + protoArmyOverview.leaderID);
			Console.WriteLine("deepCopy.leaderID= " + deepCopy.leaderID);
			protoArmyOverview.leaderName = "TEST leaderName";
			Console.WriteLine("protoArmyOverview.leaderName= " + protoArmyOverview.leaderName);
			Console.WriteLine("deepCopy.leaderName= " + deepCopy.leaderName);*/

			return deepCopy;
		}

		/// <summary>
		///     Create a deep copy of a ProtoDetachment
		/// </summary>
		/// <param name="protoDetachment">ProtoFief to copy</param>
		/// <returns>The deep copy</returns>
		public static ProtoDetachment DeepCopyProtoDetachment(ProtoDetachment protoDetachment)
		{
			ProtoDetachment deepCopy = new ProtoDetachment();
			deepCopy.armyID = protoDetachment.armyID;
			deepCopy.days = protoDetachment.days;
			deepCopy.id = protoDetachment.id;
			deepCopy.leftBy = protoDetachment.leftBy;
			deepCopy.leftFor = protoDetachment.leftFor;
			deepCopy.troops = protoDetachment.troops != null ? (uint[])protoDetachment.troops.Clone() : null;
			return deepCopy;
		}

		/// <summary>
		///     Create a deep copy of a ProtoDetachment
		/// </summary>
		/// <param name="protoDetachment">ProtoFief to copy</param>
		/// <returns>The deep copy</returns>
		public static Pair DeepCopyPair(Pair pair)
		{
			Pair deepCopy = new Pair();
			deepCopy.key = pair.key;
			deepCopy.value = pair.value;
			return deepCopy;
		}

		/// <summary>
		///     Create a deep copy of a ProtoSiegeOverview
		/// </summary>
		/// <param name="protoSiegeOverview">ProtoFief to copy</param>
		/// <returns>The deep copy</returns>
		public static ProtoSiegeOverview DeepCopyProtoSiegeOverview(ProtoSiegeOverview protoSiegeOverview)
		{
			ProtoSiegeOverview deepCopy = new ProtoSiegeOverview();
			deepCopy.besiegedFief = protoSiegeOverview.besiegedFief;
			deepCopy.besiegingPlayer = protoSiegeOverview.besiegingPlayer;
			deepCopy.defendingPlayer = protoSiegeOverview.defendingPlayer;
			deepCopy.siegeID = protoSiegeOverview.siegeID;
			return deepCopy;
		}
	}
}