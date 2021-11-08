using System;
using System.Collections.Generic;
using System.Linq;

namespace JominiAI
{
	/// <summary>
	/// Contains static functions that help lighten the other classes 
	/// </summary>
	public static class Tools
    {
		/// <summary>
		///     Calculate a score to estimate how well the agent is performing.
		///     Score = Money*1 + nbTroops*1*500 + nbFiefs*2500 + nbAllies*1000 + nbEnnemies*(-500) + titleScore +totalNPCleaderShip*100 + nbBaillifs*100
		///     titleScore => 'king'=1000000, 'Prince Bishop'='king'/2, 'Duc'='king'/4, etc...
		/// </summary>
		/// <param name="gameState">The GameState to calculate the score from</param>
		/// <returns>The estimated score of this GameState, the higher the better for the Agent</returns>
		public static double calculateGameStateScore(GameState gameState)
		{
			RedProtoPlayerCharacter currentCharacter = gameState.myPC;

			int kingVal = 1000000;
			Dictionary<String, double> titleValues = new Dictionary<String, double>() {
				{ "King", kingVal }, { "Prince Bishop", kingVal/2 }, { "Duc", kingVal/4 }, { "Dauphine", kingVal/8 }, { "Prince", kingVal/16 },
				{ "Earl", kingVal/32 }, { "Captal", kingVal/64 }, { "Vicomte", kingVal/128 }, { "Baron", kingVal/256 }, { "Seigneur", kingVal/512 },
				{ "Lord", kingVal/1024 }, { "Vidame", kingVal/2048 }, { "Chatelain", 0 }, };

			uint totalMoney = (uint)currentCharacter.purse;
			foreach (RedProtoFief myFief in gameState.myFiefs)
				totalMoney += (uint)myFief.treasury;

			uint totalTroops = 0;
			foreach (RedProtoArmy myArmy in gameState.myArmies)
				if (myArmy.troops != null)
					foreach (uint mytroop in myArmy.troops)
						totalTroops += mytroop;
			double totalNPCleadership = 0;
			foreach (RedProtoNPC myNPC in gameState.myEmployeeNPCs)
				totalNPCleadership += Tools.GetLeadershipValue(myNPC);
			double nbBaillifs = 0;
			foreach (RedProtoFief myFief in gameState.myFiefs)
				if (!string.IsNullOrWhiteSpace(myFief.bailiffID))
					nbBaillifs++;

			double multiplicatorMoney = 1;
			double multiplicatorTroops = multiplicatorMoney * 500;
			double multiplicatorFiefs = 2500;
			double multiplicatorAllies = 1000;
			double multiplicatorEnemies = -500;
			double multiplicatorNPCs = 100;
			double multiplicatorBaillifs = 100;

			double scoreMoney = totalMoney * multiplicatorMoney;
			double scoreTroops = totalTroops * multiplicatorTroops;
			double scoreFief = gameState.myFiefs.Count * multiplicatorFiefs;
			double scoreAllies = gameState.allyPCids.Count * multiplicatorAllies;
			double scoreEnemies = gameState.allyPCids.Count * multiplicatorEnemies;
			double scoreNpc = totalNPCleadership * multiplicatorNPCs;
			double scoreTitles = 0;
			double scoreBaillifs = nbBaillifs * multiplicatorBaillifs;
			foreach (String myTitle in currentCharacter.titles)
				if (titleValues.ContainsKey(myTitle))
				{
					double titleValue;
					titleValues.TryGetValue(myTitle, out titleValue);
					scoreTitles += titleValue;
				}

			return (scoreMoney + scoreTroops + scoreFief + scoreAllies + scoreEnemies + scoreNpc + scoreTitles + scoreBaillifs) / 100000; // Divided to get a smaller score that is easier to read
		}

		/// <summary>
		/// </summary>
		/// <param name="toSumUp"></param>
		/// <returns>The sum of all the uint of the array</returns>
		public static uint CalculSumUintArray(uint[] toSumUp)
		{
			uint total = 0;
			foreach (uint troop in toSumUp)
				total += troop;
			return total;
		}

        /// <summary>
        ///		Estimates the character's leadership value
        /// </summary>
        /// <param name="character"></param>
        /// <returns>the leadership value</returns>
        public static double GetLeadershipValue(RedProtoCharacter character)
        {
            Double stature = ! character.isMale ? -6 : 0;
            return (character.combat + character.management + stature) / 3; ;
        }

		/// <summary>
		///     Used with the ratios to know what differential to apply to reach the targeted ratio
		/// </summary>
		/// <param name="dividend">dividend of the current ratio</param>
		/// <param name="diviser">diviser of the current ratio</param>
		/// <param name="targetedRatio"></param>
		/// <returns>Number of units to add or remove to reach target</returns>
		public static double calculDifferentialToDividendToReachTarget(double dividend, double diviser, double targetedRatio)
		{
			double differentialToDividendToReachTarget = (targetedRatio * diviser - dividend) / (1 + targetedRatio);
			return differentialToDividendToReachTarget;
		}
	}
}