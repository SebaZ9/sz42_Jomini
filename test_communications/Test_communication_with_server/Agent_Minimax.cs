using System;
using System.Collections.Generic;
using System.Linq;

namespace JominiAI
{
    /// <summary>
	///     This AI agent is based on a Minimax searching algorithm.
	/// </summary>
    public class MinimaxAgent : Agent
    {
        private int seasonDepth;

        /// <summary>
        /// </summary>
        /// <param name="pSeasonDepth">The number of seasons the algorithms will simulate from the current one</param>
        public MinimaxAgent(string username, string password, int pSeasonDepth) : base(username, password)
        {
            seasonDepth = pSeasonDepth;
            maxDaysToDoAction = maxDaysToDoAction.ToDictionary(p => p.Key, p => p.Value + 1); // Offset of 1 to add a little margin and to avoid the complications of having actions costing 0 days.
        }

        public override MainActions findNextAction(GameState gamestate, out string[] arguments)
        {
            return findBestAction(seasonDepth, true, gamestate, out double gameStateScore, out arguments);
        }

        /// <summary>
		///     Return the best action for the agent to perform found by the algorithm
		/// </summary>
		/// <param name="pSeasonDepth">The number of seasons the algorithms will simulate from the current one</param>
        /// <param name="myTurn">To know which player the algorithm is currently simulating the actions (my turn or the enemy turn)</param>
		/// <param name="currentGameState">GameState from which so search the best action to perform</param>
        /// <returns>The best action found</returns>
        /// <returns>gameStateScore: score of the best action found</returns>
        private MainActions findBestAction(int pSeasonDepth, bool myTurn, GameState currentGameState, out double gameStateScore, out String[] arguments)
        {
            RedProtoPlayerCharacter currentPC = currentGameState.myPC;
            RedProtoFief currentFief = currentGameState.getCurrentFief();
            currentGameState.tryGetMainArmy(out RedProtoArmy mainArmy);

            String[] bestActionArguments = new string[0];
            MainActions bestAction = MainActions.None;
            double bestActionScore = Double.NegativeInfinity;
            List<MainActions> possibleActions = getPossibleActions(currentGameState);
            foreach (MainActions action in possibleActions)
            {
                GameState newGameState = new GameState();
                if (!maxDaysToDoAction.TryGetValue(action, out double maxCostToDoAction))
                    throw new Exception("The MainActions '" + action.ToString() + "' isn't present in this dictionnary, it needs to be added!");
                if (maxCostToDoAction != -1)
                    switch (action)
                    {
                        case MainActions.ArmyRecruit:
                            int maxToHire = currentFief.militia;
                            int indivTroopCost = 2000;
                            if(currentFief.ancestralOwnerId != null)
                                if (currentFief.ancestralOwnerId.Equals(currentPC.charID))
                                    indivTroopCost = 500;
                            int nbToHire;

                            // 100%
                            nbToHire = (int)(maxToHire);
                            newGameState = currentGameState.deepCopy();
                            newGameState.myPC.days -= maxCostToDoAction; // Day cost
                            newGameState.tryGetMainArmy(out RedProtoArmy newMainArmy);
                            newMainArmy.troops[0] += (uint)nbToHire;
                            newGameState.getCurrentFief().hasRecruited = true;
                            newGameState.getHomeFief().treasury -= nbToHire * indivTroopCost;
                            getThisBranchBestAction(pSeasonDepth, myTurn, newGameState, action, new string[] { newMainArmy.armyID, nbToHire.ToString() }, bestAction, bestActionScore, bestActionArguments, out bestAction, out bestActionScore, out bestActionArguments);
                            
                            // 50%
                            nbToHire = (int)(maxToHire / 2);
                            newGameState = currentGameState.deepCopy();
                            newGameState.myPC.days -= maxCostToDoAction; // Day cost
                            newGameState.tryGetMainArmy(out newMainArmy);
                            newMainArmy.troops[0] += (uint)nbToHire;
                            newGameState.getCurrentFief().hasRecruited = true;
                            newGameState.getHomeFief().treasury -= nbToHire * indivTroopCost;
                            getThisBranchBestAction(pSeasonDepth, myTurn, newGameState, action, new string[] { nbToHire.ToString() }, bestAction, bestActionScore, bestActionArguments, out bestAction, out bestActionScore, out bestActionArguments);
                            break;
                        case MainActions.ArmyMaintain:
                            newGameState = currentGameState.deepCopy();
                            newGameState.myPC.days -= maxCostToDoAction; // Day cost
                            newGameState.tryGetMainArmy(out RedProtoArmy newMainArmyBis);
                            newMainArmyBis.isMaintained = true;
                            newGameState.getHomeFief().treasury -= (int)newMainArmyBis.maintCost;
                            getThisBranchBestAction(pSeasonDepth, myTurn, newGameState, action, new string[] { newMainArmyBis.armyID }, 
                                bestAction, bestActionScore, bestActionArguments, out bestAction, out bestActionScore, out bestActionArguments);
                            break;
                        case MainActions.ArmyPickUp:
                            break;
                        case MainActions.ArmyDropOff:
                            break;
                        case MainActions.ArmyDisband:
                            foreach (RedProtoArmy myArmy in currentGameState.myArmies)
                            {
                                newGameState = currentGameState.deepCopy();
                                newGameState.tryGetMainArmy(out RedProtoArmy newMainArmyBBis);
                                newGameState.myPC.days -= maxCostToDoAction; // Day cost
                                for (int i = 0; i < newGameState.myArmies.Count; i++)
                                    if (newGameState.myArmies[i].armyID.Equals(myArmy.armyID))
                                    {
                                        newGameState.myArmies[i] = null;
                                        newGameState.myArmies.RemoveAt(i);
                                    }
                                getThisBranchBestAction(pSeasonDepth, myTurn, newGameState, action, new string[] { newMainArmyBBis.armyID }, 
                                    bestAction, bestActionScore, bestActionArguments, out bestAction, out bestActionScore, out bestActionArguments);
                            }
                            break;
                        case MainActions.ArmyAppointLeader:
                            break;
                        case MainActions.ArmyPillageCurrentFief:
                            break;
                        case MainActions.ArmyAdjustCombatValues:
                            break;
                        case MainActions.ArmyAttack:
                            break;
                        case MainActions.SiegeBesiegeCurrentFief:
                            break;
                        case MainActions.SiegeStorm:
                            break;
                        case MainActions.SiegeNegotiation:
                            break;
                        case MainActions.SiegeReduction:
                            break;
                        case MainActions.SiegeEnd:
                            break;
                        case MainActions.NPCmoveToFief:
                            break;
                        case MainActions.NPCmoveDirection:
                            break;
                        case MainActions.NPCmarryPC:
                            break;
                        case MainActions.NPCacceptRejectProposal:
                            break;
                        case MainActions.NPCtryForChild:
                            break;
                        case MainActions.NPChire:
                            break;
                        case MainActions.NPCfire:
                            break;
                        case MainActions.NPCbarCharacters:
                            break;
                        case MainActions.NPCunbarCharacters:
                            break;
                        case MainActions.NPCbarNationalities:
                            break;
                        case MainActions.NPCunbarNationalities:
                            break;
                        case MainActions.NPCtryEnterKeep:
                            newGameState = currentGameState.deepCopy();
                            newGameState.myPC.days -= maxCostToDoAction; // Day cost
                            newGameState.myPC.inKeep = true;
                            getThisBranchBestAction(seasonDepth, myTurn, newGameState, action, new string[] { currentPC.charID }, bestAction, bestActionScore, bestActionArguments, out bestAction, out bestActionScore, out bestActionArguments);
                            break;
                        case MainActions.NPCexitKeep:
                            /*newGameState = currentGameState.deepCopy();
                            newGameState.getCurrentCharacter().days -= maxDaysCost; // Day cost
                            newGameState.getCurrentCharacter().inKeep = false;
                            getThisBranchBestAction(seasonDepth, myTurn, newGameState, action, new string[] { currentPC.charID }, bestAction, bestActionScore, bestActionArguments, out bestAction, out bestActionScore, out bestActionArguments);*/
                            break;
                        case MainActions.NPCcamp: // !!! To continue !!!
                            byte nbDaysToCamp;

                            nbDaysToCamp = (byte)currentPC.days; // 100%
                            newGameState = currentGameState.deepCopy();
                            newGameState.myPC.days -= nbDaysToCamp; // Day cost
                            getThisBranchBestAction(pSeasonDepth, myTurn, newGameState, action, new string[] { currentPC.charID, nbDaysToCamp.ToString() }, 
                                bestAction, bestActionScore, bestActionArguments, out bestAction, out bestActionScore, out bestActionArguments);
                            break;
                        case MainActions.NPCaddToEntourage:
                            break;
                        case MainActions.NPCremoveFromEntourage:
                            break;
                        case MainActions.FiefGrantFiefTitle:
                            break;
                        case MainActions.FiefAutoAdjustExpenditure:
                            break;
                        case MainActions.FiefTransferFunds:
                            break;
                        case MainActions.FiefTransferFundsToPlayer:
                            break;
                        case MainActions.SpyFief:
                            break;
                        case MainActions.SpyCharacter:
                            break;
                        case MainActions.SpyArmy:
                            break;
                        case MainActions.None:
                            throw new Exception("Invalid action!");
                        default:
                            throw new Exception("Invalid action!");
                    }
            }
            if (bestActionScore == Double.NegativeInfinity)
            {
                //if (!myTurn)
                pSeasonDepth--;
                if (pSeasonDepth == 0)
                {
                    gameStateScore = Tools.calculateGameStateScore(currentGameState);
                    Console.WriteLine("gameStateScore= " + gameStateScore);
                    arguments = new string[0];
                    return MainActions.None;
                }
                myTurn = !myTurn;
            }
            gameStateScore = bestActionScore;
            arguments = bestActionArguments;
            return bestAction;
        }

        /// <summary>
		///     Explore the branch we would access by using this action and replace the current best action if a better one is found.
		/// </summary>
        private void getThisBranchBestAction(int seasonDepth, bool myTurn, GameState newGameState, MainActions currentAction, String[] currentActionArguments, MainActions bestAction, 
            double bestActionScore, String[] bestActionArguments, out MainActions newBestAction, out double newBestActionScore, out String[] newBestActionArguments)
        {
            double newActionScore;
            Console.WriteLine("<= Before " + currentAction.ToString() + " bestActionScore= " + bestActionScore);
            findBestAction(seasonDepth, myTurn, newGameState, out newActionScore, out String[] stringArray);
            if (newActionScore > bestActionScore)
            {
                newBestAction = currentAction;
                newBestActionScore = newActionScore;
                newBestActionArguments = currentActionArguments;
                Console.WriteLine("-------- newBestAction found: " + newBestAction + " => newActionScore = " + newActionScore);
            }
            else
            {
                newBestAction = bestAction;
                newBestActionScore = bestActionScore;
                newBestActionArguments = bestActionArguments;
            }
            Console.WriteLine("=> After " + currentAction.ToString() + " bestActionScore= " + newBestActionScore);
        }
    }
}