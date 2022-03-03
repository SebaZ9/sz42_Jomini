using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JominiGame
{
    public static class Battle
    {

        /// <summary>
        /// Calculates whether the attacking army is able to successfully bring the defending army to battle
        /// </summary>
        /// <returns>bool indicating whether battle has commenced</returns>
        /// <param name="attackerValue">uint containing attacking army battle value</param>
        /// <param name="defenderValue">uint containing defending army battle value</param>
        /// <param name="Circumstance">string indicating Circumstance of battle</param>
        private static bool BringToBattle(uint attackerValue, uint defenderValue, string Circumstance = "battle")
        {
            bool battleHasCommenced = false;
            double[] combatOdds = GameSettings.BATTLE_PROBABILITIES_ODDS;
            double[] battleChances;
            switch (Circumstance)
            {
                case "battle":
                    {
                        battleChances = GameSettings.BATTLE_PROBABILITIES_BATTLE;
                        break;
                    }
                case "pillage":
                    {
                        battleChances = GameSettings.BATTLE_PROBABILITIES_PILLAGE;
                        break;
                    }
                default:
                    {
                        battleChances = GameSettings.BATTLE_PROBABILITIES_BATTLE;
                        break;
                    }
            }
            double thisChance = 0;

            for (int i = 0; i < combatOdds.Length; i++)
            {
                if (i < combatOdds.Length - 1)
                {
                    // ReSharper disable once PossibleLossOfFraction
                    if (attackerValue / defenderValue < combatOdds[i])
                    {
                        thisChance = battleChances[i];
                        break;
                    }
                }
                else
                {
                    thisChance = battleChances[i];
                    break;
                }
            }

            // generate random percentage
            double randomPercentage = Random.Shared.NextDouble() * 100;

            // compare random percentage to battleChance
            if (randomPercentage <= thisChance)
            {
                battleHasCommenced = true;
            }
            return battleHasCommenced;
        }

        /// <summary>
        /// Determines whether the attacking army is victorious in a battle
        /// </summary>
        /// <returns>bool indicating whether attacking army is victorious</returns>
        /// <param name="attackerValue">uint containing attacking army battle value</param>
        /// <param name="defenderValue">uint containing defending army battle value</param>
        public static bool DecideBattleVictory(uint attackerValue, uint defenderValue)
        {
            bool attackerVictorious = false;

            // calculate chance of victory
            double attackerVictoryChance = CalcVictoryChance(attackerValue, defenderValue);

            // generate random percentage
            double randomPercentage = Random.Shared.NextDouble() * 100;

            // compare random percentage to attackerVictoryChance
            if (randomPercentage <= attackerVictoryChance)
            {
                attackerVictorious = true;
            }

            return attackerVictorious;
        }

        /// <summary>
        /// Calculates chance that the attacking army will be victorious in a battle
        /// </summary>
        /// <returns>double containing percentage chance of victory</returns>
        /// <param name="attackerValue">uint containing attacking army battle value</param>
        /// <param name="defenderValue">uint containing defending army battle value</param>
        public static double CalcVictoryChance(uint attackerValue, uint defenderValue)
        {
            return (attackerValue / (Convert.ToDouble(attackerValue + defenderValue))) * 100;
        }

        /// <summary>
        /// Calculates casualties from a battle for both sides
        /// </summary>
        /// <returns>double[] containing percentage loss modifier for each side</returns>
        /// <param name="attackerTroops">uint containing attacking army troop numbers</param>
        /// <param name="defenderTroops">uint containing defending army troop numbers</param>
        /// <param name="attackerValue">uint containing attacking army battle value</param>
        /// <param name="defenderValue">uint containing defending army battle value</param>
        /// <param name="attackerVictorious">bool indicating whether attacking army was victorious</param>
        public static double[] CalculateBattleCasualties(uint attackerTroops, uint defenderTroops, uint attackerValue, uint defenderValue, bool attackerVictorious)
        {
            double[] battleCasualties = new double[2];
            double largeArmyModifier = 0;
            bool largestWon = true;

            // determine highest/lowest battle value
            double maxBV = Math.Max(attackerValue, defenderValue);
            double minBV = Math.Min(attackerValue, defenderValue);

            // use BVs to determine high mark for base casualty rate of army with smallest battle value (see below)
            double highCasualtyRate = maxBV / (maxBV + minBV);

            // determine base casualty rate for army with smallest battle value
            double smallestModifier = Random.Shared.NextDouble() * (highCasualtyRate - 0.1) + 0.1;

            // determine if army with largest battle value won
            if (maxBV == attackerValue)
            {
                if (!attackerVictorious)
                {
                    largestWon = false;
                }
            }
            else
            {
                if (attackerVictorious)
                {
                    largestWon = false;
                }
            }

            // if army with largest battle value won
            if (largestWon)
            {
                // calculate casualty modifier for army with largest battle value
                // (based on adapted version of Lanchester's Square Law - i.e. largest army loses less troops than smallest)
                largeArmyModifier = (1 + ((minBV * minBV) / (maxBV * maxBV))) / 2;

                // attacker is large army
                if (attackerVictorious)
                {
                    battleCasualties[1] = smallestModifier;
                    // determine actual troop losses for largest army based on smallest army losses,
                    // modified by largeArmyModifier
                    uint largeArmyLosses = Convert.ToUInt32((defenderTroops * battleCasualties[1]) * largeArmyModifier);
                    // derive final casualty modifier for largest army
                    battleCasualties[0] = largeArmyLosses / (double)attackerTroops;
                }
                // defender is large army
                else
                {
                    battleCasualties[0] = smallestModifier;
                    uint largeArmyLosses = Convert.ToUInt32((attackerTroops * battleCasualties[0]) * largeArmyModifier);
                    battleCasualties[1] = largeArmyLosses / (double)defenderTroops;
                }
            }

            // if army with smallest battle value won
            else
            {
                // calculate casualty modifier for army with largest battle value
                // this ensures its losses will be roughly the same as the smallest army (because it lost)
                largeArmyModifier = Random.Shared.NextDouble() * (0.4) + 0.8;

                // defender is large army
                if (attackerVictorious)
                {
                    // smallest army losses reduced because they won
                    battleCasualties[0] = smallestModifier / 2;
                    // determine actual troop losses for largest army based on smallest army losses,
                    // modified by largeArmyModifier
                    uint largeArmyLosses = Convert.ToUInt32((attackerTroops * battleCasualties[0]) * largeArmyModifier);
                    // derive final casualty modifier for largest army
                    battleCasualties[1] = largeArmyLosses / (double)defenderTroops;
                }
                // attacker is large army
                else
                {
                    battleCasualties[1] = smallestModifier / 2;
                    uint largeArmyLosses = Convert.ToUInt32((defenderTroops * battleCasualties[1]) * largeArmyModifier);
                    battleCasualties[0] = largeArmyLosses / (double)attackerTroops;
                }
            }


            return battleCasualties;
        }

        /// <summary>
        /// Calculates whether either army has retreated due to the outcome of a battle
        /// </summary>
        /// <returns>int[] indicating the retreat distance (fiefs) of each army. First index is attacker, second is defender</returns>
        /// <param name="attacker">The attacking army</param>
        /// <param name="defender">The defending army</param>
        /// <param name="aCasualties">The attacking army casualty modifier</param>
        /// <param name="dCasualties">The defending army casualty modifier</param>
        /// <param name="attackerVictorious">bool indicating if attacking army was victorious</param>
        public static int[] CheckForRetreat(Army attacker, Army defender, double aCasualties, double dCasualties, bool attackerVictorious)
        {
            bool[] hasRetreated = { false, false };
            int[] retreatDistance = { 0, 0 };

            // check if loser retreats due to battlefield casualties
            if (!attackerVictorious)
            {
                // if have >= 20% casualties
                if (aCasualties >= 0.2)
                {
                    // indicate attacker has retreated
                    hasRetreated[0] = true;

                    // generate random 1-2 to determine retreat distance
                    retreatDistance[0] = Random.Shared.Next(1, 3);
                }
            }
            else
            {
                // if have >= 20% casualties
                if (dCasualties >= 0.2)
                {
                    // indicate defender has retreated
                    hasRetreated[1] = true;

                    // generate random 1-2 to determine retreat distance
                    retreatDistance[1] = Random.Shared.Next(1, 3);
                }
            }

            // check to see if defender retreats due to Aggression setting (i.e. was forced into battle)
            // NOTE: this will only happen if attacker and defender still present in fief
            if ((defender.Aggression == 0) && (!hasRetreated[0] && !hasRetreated[1]))
            {
                // indicate defender has retreated
                hasRetreated[1] = true;

                // indicate retreat distance
                retreatDistance[1] = 1;
            }

            return retreatDistance;
        }

        /// <summary>
        /// Calculates rough battle odds between two armies (i.e ratio of attacking army combat
        /// value to defending army combat value).  NOTE: does not involve leadership values
        /// </summary>
        /// <returns>int containing battle odds</returns>
        /// <param name="attacker">The attacking army</param>
        /// <param name="defender">The defending army</param>
        public static int GetBattleOdds(Army attacker, Army defender)
        {
            return Convert.ToInt32(Math.Floor(attacker.CalculateCombatValue() / defender.CalculateCombatValue()));
        }

        /// <summary>
        /// Return a string describing the results of a battle
        /// </summary>
        /// <param name="battle">Results of battle</param>
        /// <returns>String description</returns>
        public static string DisplayBattleResultss(BattleResults battle)
        {
            // Battle introduction
            string toDisplay = "The fief garrison and militia";
            if (battle.AttackerOwner != null)
            {
                toDisplay += ", led by " + battle.AttackerOwner.FullName() + ",";
            }
            switch (battle.Circumstance)
            {
                // Normal battle
                case 0:
                    {
                        toDisplay += " moved to attack ";
                    }
                    break;
                // Pillage
                case 1:
                    {
                        toDisplay += " sallied forth to bring the pillaging army,";

                    }
                    break;
                // Siege
                case 2:
                    {
                        toDisplay += ", sallied forth to bring the besieging army, ";
                    }
                    break;
                default:
                    toDisplay = "Unrecognised Circumstance";
                    break;
            }
            if (battle.DefenderOwner != null)
            {
                toDisplay += " led by " + battle.DefenderOwner.FullName() + " and";
            }
            toDisplay += " owned by " + battle.DefenderOwner?.FullName()
                         + ", to battle in the fief of " + battle.BattleLocation.Name + "."
                + "\r\n\r\nOutcome: ";
            if (battle.BattleTookPlace)
            {
                if (battle.AttackerVictorious)
                {
                    toDisplay += battle.AttackerOwner;
                }
                else
                {
                    toDisplay += battle.DefenderOwner;
                }

                // Victory status
                toDisplay += "'s army was victorious.\r\n\r\n";

                // Casualties
                toDisplay += battle.AttackerOwner.FullName() + "'s army suffered " + battle.AttackerCasualties +
                             " troop casualties.\n";
                toDisplay += battle.AttackerOwner.FullName() + "'s army suffered " + battle.DefenderCasualties +
                             " troop casualties.\n";

                // Retreats
                foreach (var retreater in battle.RetreatedArmies)
                {
                    toDisplay += retreater + "'s army retreated from the fief.\n";
                }

                // Disbands
                foreach (var disbander in battle.DisbandedArmies)
                {
                    toDisplay += disbander + "'s army disbanded due to heavy casualties.\n";
                }

                toDisplay += string.Join(", ", battle.Deaths) + " all died due to injuries received.\n";
                if (battle.Circumstance == 1)
                {
                    toDisplay += "The pillage in " + battle.BattleLocation.Name +
                                 " has been prevented";
                }

                // Siege results
                if (battle.Circumstance == 2)
                {
                    if (battle.AttackerVictorious || battle.RetreatedArmies.Contains(battle.AttackerOwner.ID))
                    {
                        toDisplay += battle.AttackerOwner + "'s defenders have defeated the forces of " +
                                     battle.DefenderOwner + ", relieving the siege of " +
                                     battle.BattleLocation.Name
                                     + ". " + battle.DefenderOwner +
                                     " retains ownership of the fief. The siege has been raised.\n";

                    }
                    else if (battle.DefenderDeadNoHeir)
                    {
                        // add to message
                        toDisplay += "The siege in " + battle.BattleLocation.Name + " has been raised";
                        toDisplay += " due to the death of the besieging party, ";
                        toDisplay += battle.SiegeBesieger + ".";
                    }
                }
            }
            else
            {
                if (battle.Circumstance > 0)
                {
                    toDisplay += battle.AttackerOwner + "'s forces failed to bring their aggressors to battle.\n";
                }
                else
                {
                    toDisplay += battle.DefenderOwner +
                                 "'s forces successfully refused battle and retreated from the fief.";
                }
            }
            return toDisplay;
        }

        /// <summary>
        /// Display the results of a siege (that has been resolved due to a battle) in a human readable format
        /// </summary>
        /// <param name="battle">Results of battle</param>
        /// <returns>String describing battle</returns>
        public static string DisplaySiegeResults(BattleResults battle)
        {
            string siegeDescription = "";
            if (battle.AttackerVictorious || battle.RetreatedArmies.Contains(battle.AttackerOwner.ID))
            {
                siegeDescription = "On this day of Our Lord the forces of ";
                siegeDescription += battle.SiegeDefender;
                siegeDescription += " have defeated the forces of " + battle.SiegeBesieger;
                siegeDescription += ", relieving the siege of " + battle.BattleLocation.Name + ".";
                siegeDescription += " " + battle.SiegeDefender;
                siegeDescription += " retains ownership of the fief.";
            }
            if (battle.DefenderDeadNoHeir)
            {
                // construct event description to be passed into siegeEnd
                siegeDescription = "On this day of Our Lord the forces of ";
                siegeDescription += battle.SiegeBesieger;
                siegeDescription += " attacked the forces of " + battle.SiegeDefender;
                siegeDescription += ", who was killed during the battle.";
                siegeDescription += "  Thus, despite losing the battle, " + battle.SiegeBesieger;
                siegeDescription += " has succeeded in relieving the siege of " + battle.BattleLocation.Name + ".";
                siegeDescription += " " + battle.SiegeDefender;
                siegeDescription += " retains ownership of the fief.";
            }
            return siegeDescription;

        }

        /// <summary>
        /// Implements the processes involved in a battle between two armies in the field
        /// </summary>
        /// <returns>bool indicating whether attacking army is victorious</returns>
        /// <remarks>
        /// Predicate: assumes attacker has sufficient days
        /// Predicate: assumes attacker has leader
        /// Predicate: assumes attacker in same fief as defender
        /// Predicate: assumes defender not besieged in keep
        /// Predicate: assumes attacker and defender not same army
        /// </remarks>
        /// <param name="attacker">The attacking army</param>
        /// <param name="defender">The defending army</param>
        /// <param name="Circumstance">string indicating Circumstance of battle</param>
        public static bool GiveBattle(Army attacker, Army defender, out BattleResults battleResults, string Circumstance = "battle")
        {
            battleResults = new BattleResults();
            bool attackerVictorious = false;
            bool battleHasCommenced = false;
            bool attackerLeaderDead = false;
            bool defenderLeaderDead = false;
            // check if losing army has disbanded
            bool attackerDisbanded = false;
            bool defenderDisbanded = false;

            bool siegeRaised = false;
            uint[] battleValues = new uint[2];
            double[] casualtyModifiers = new double[2];
            double statureChange = 0;

            // if applicable, get siege
            Siege thisSiege = defender.CheckIfBesieger();

            // get starting troop numbers
            uint attackerStartTroops = attacker.CalcArmySize();
            uint defenderStartTroops = defender.CalcArmySize();
            uint attackerCasualties = 0;
            uint defenderCasualties = 0;

            battleResults.AttackerOwner = attacker.Owner;
            battleResults.DefenderOwner = defender.Owner;            
            battleResults.BattleLocation = attacker.Location;

            // introductory text for message
            switch (Circumstance)
            {
                case "pillage":
                    battleResults.Circumstance = 1;
                    break;
                case "siege":
                    battleResults.Circumstance = 2;
                    break;
                default:
                    battleResults.Circumstance = 0;
                    break;
            }

            // get battle values for both armies
            battleValues = attacker.CalculateBattleValues(defender);

            // check if attacker has managed to bring defender to battle
            // case 1: defending army sallies during siege to attack besieger = battle always occurs
            if (Circumstance.Equals("siege"))
            {
                battleHasCommenced = true;
            }
            // case 2: defending militia attacks pillaging army during pollage = battle always occurs
            else if (Circumstance.Equals("pillage"))
            {
                battleHasCommenced = true;
            }
            // case 3: defender Aggression and combatOdds allows battle
            else if (defender.Aggression != 0)
            {
                if (defender.Aggression == 1)
                {
                    // get odds
                    int battleOdds = GetBattleOdds(attacker, defender);

                    // if odds OK, give battle
                    if (battleOdds <= defender.CombatOdds)
                    {
                        battleHasCommenced = true;
                    }

                    // if not, check for battle
                    else
                    {
                        battleHasCommenced = BringToBattle(battleValues[0], battleValues[1], Circumstance);

                        if (!battleHasCommenced)
                        {
                            defender.ProcessRetreat(1);
                            
                        }
                    }
                }

                else
                {
                    battleHasCommenced = true;
                }
            }

            // otherwise, check to see if the attacker can bring the defender to battle
            else
            {
                battleHasCommenced = Battle.BringToBattle(battleValues[0], battleValues[1], Circumstance);
                if (!battleHasCommenced)
                {
                    defender.ProcessRetreat(1);
                }
            }
            battleResults.BattleTookPlace = battleHasCommenced;
            if (battleHasCommenced)
            {
                List<string> disbandedArmies = new List<string>();
                List<string> retreatedArmies = new List<string>();
                List<string> deadCharacters = new List<string>();
                // WHO HAS WON?
                // calculate if attacker has won
                attackerVictorious = Battle.DecideBattleVictory(battleValues[0], battleValues[1]);

                // UPDATE STATURE
                if (attackerVictorious)
                {
                    statureChange = 0.8 * (defender.CalcArmySize() / Convert.ToDouble(10000));
                    battleResults.StatureChangeAttacker = statureChange;
                    attacker.Owner.AdjustStatureModifier(statureChange);
                    statureChange = -0.5 * (attacker.CalcArmySize() / Convert.ToDouble(10000));
                    battleResults.StatureChangeDefender = statureChange;
                    defender.Owner.AdjustStatureModifier(statureChange);
                }
                else
                {
                    statureChange = 0.8 * (attacker.CalcArmySize() / Convert.ToDouble(10000));
                    battleResults.StatureChangeDefender = statureChange;
                    defender.Owner.AdjustStatureModifier(statureChange);
                    statureChange = -0.5 * (defender.CalcArmySize() / Convert.ToDouble(10000));
                    battleResults.StatureChangeAttacker = statureChange;
                    attacker.Owner.AdjustStatureModifier(statureChange);
                }

                // CASUALTIES
                // calculate troop casualties for both sides
                casualtyModifiers = CalculateBattleCasualties(attackerStartTroops, defenderStartTroops, battleValues[0], battleValues[1], attackerVictorious);


                uint totalAttackTroopsLost = 0;
                uint totalDefendTroopsLost = 0;

                // if losing side sustains >= 50% casualties, disbands
                if (attackerVictorious)
                {
                    // either indicate losing army to be disbanded
                    if (casualtyModifiers[1] >= 0.5)
                    {
                        defenderDisbanded = true;
                        disbandedArmies.Add(defender.Owner.ID);
                        totalDefendTroopsLost = defender.CalcArmySize();
                    }
                    // OR apply troop casualties to losing army
                    else
                    {
                        totalDefendTroopsLost = defender.ApplyTroopLosses(casualtyModifiers[1]);
                    }

                    // apply troop casualties to winning army
                    totalAttackTroopsLost = attacker.ApplyTroopLosses(casualtyModifiers[0]);
                }
                else
                {
                    if (casualtyModifiers[0] >= 0.5)
                    {
                        attackerDisbanded = true;
                        disbandedArmies.Add(attacker.Owner.ID);
                        totalAttackTroopsLost = attacker.CalcArmySize();
                    }
                    else
                    {
                        totalAttackTroopsLost = attacker.ApplyTroopLosses(casualtyModifiers[0]);
                    }

                    totalDefendTroopsLost = defender.ApplyTroopLosses(casualtyModifiers[1]);
                }
                battleResults.AttackerCasualties = totalAttackTroopsLost;
                battleResults.DefenderCasualties = totalDefendTroopsLost;
                // UPDATE TOTAL SIEGE LOSSES, if appropriate
                // NOTE: the defender in this battle is the attacker in the siege and v.v.
                if (thisSiege != null)
                {
                    // update total siege attacker (defender in this battle) losses
                    thisSiege.TotalCasualtiesAttacker += Convert.ToInt32(totalDefendTroopsLost);

                    // update total siege defender (attacker in this battle) losses
                    if (Circumstance.Equals("siege"))
                    {
                        thisSiege.TotalCasualtiesDefender += Convert.ToInt32(totalAttackTroopsLost);
                    }
                }

                // get casualty figures (for message)
                if (!attackerDisbanded)
                {
                    // get attacker casualties
                    attackerCasualties = totalAttackTroopsLost;
                }
                if (!defenderDisbanded)
                {
                    // get defender casualties
                    defenderCasualties = totalDefendTroopsLost;
                }

                // DAYS
                // adjust days
                // NOTE: don't adjust days if is a siege (will be deducted elsewhere)
                if (!Circumstance.Equals("siege"))
                {
                    if (attacker.Leader != null)
                    {
                        attacker.Leader.AdjustDays(1);
                    }
                    // need to check for defender having no leader
                    if (defender.Leader != null)
                    {
                        defender.Leader.AdjustDays(1);
                    }
                    else
                    {
                        defender.Days -= 1;
                    }
                }

                // RETREATS
                // create array of armies (for easy processing)
                Army[] bothSides = { attacker, defender };

                // check if either army needs to retreat
                int[] retreatDistances = CheckForRetreat(attacker, defender, casualtyModifiers[0], casualtyModifiers[1], attackerVictorious);

                // if is pillage or siege, attacking army (the fief's army) doesn't retreat
                // if is pillage, the defending army (the pillagers) always retreats if has lost
                if (Circumstance.Equals("pillage") || Circumstance.Equals("siege"))
                {
                    retreatDistances[0] = 0;
                }

                if (Circumstance.Equals("pillage"))
                {
                    if (attackerVictorious)
                    {
                        retreatDistances[1] = 1;
                    }
                }

                // if have retreated, perform it
                for (int i = 0; i < retreatDistances.Length; i++)
                {
                    if (retreatDistances[i] > 0)
                    {
                        bothSides[i].ProcessRetreat(retreatDistances[i]);
                    }
                }
                // If attacker has retreated add to retreat list
                if (retreatDistances[0] > 0)
                {
                    retreatedArmies.Add(battleResults.AttackerOwner.ID);
                }
                // If defender retreated add to retreat list
                if (retreatDistances[1] > 0)
                {
                    retreatedArmies.Add(battleResults.DefenderOwner.ID);
                }
                // PC/NPC INJURIES/DEATHS
                // check if any PCs/NPCs have been wounded or killed
                bool characterDead = false;

                // 1. ATTACKER
                uint friendlyBV = battleValues[0];
                uint enemyBV = battleValues[1];

                // if army leader a PC, check entourage
                if (attacker.Leader is PlayerCharacter)
                {
                    for (int i = 0; i < (attacker.Leader as PlayerCharacter).MyNPCs.Count; i++)
                    {
                        if ((attacker.Leader as PlayerCharacter).MyNPCs[i].InEntourage)
                        {
                            characterDead = (attacker.Leader as PlayerCharacter).MyNPCs[i].CalculateCombatInjury(casualtyModifiers[0]);
                        }

                        // process death, if applicable
                        if (characterDead)
                        {
                            (attacker.Leader as PlayerCharacter).MyNPCs[i].ProcessDeath("injury");             
                        }
                    }
                }

                // check army leader
                if (attacker.Leader != null)
                {
                    attackerLeaderDead = attacker.Leader.CalculateCombatInjury(casualtyModifiers[0]);
                }
                

                // process death, if applicable
                if (attackerLeaderDead)
                {
                    deadCharacters.Add(attacker.Leader.FirstName + " " + attacker.Leader.FamilyName);
                    Character newLeader = null;

                    // if is pillage, do NOT elect new leader for attacking army
                    if (!Circumstance.Equals("pillage"))
                    {
                        // if possible, elect new leader from entourage
                        if (attacker.Leader is PlayerCharacter)
                        {
                            if ((attacker.Leader as PlayerCharacter).MyNPCs.Count > 0)
                            {
                                // get new leader
                                newLeader = (attacker.Leader as PlayerCharacter).ElectNewArmyLeader();
                            }
                        }

                        // assign newLeader (can assign null leader if none found)
                        attacker.AssignNewLeader(newLeader);
                    }
                }
                else
                {
                    // if pillage, if fief's army loses, make sure bailiff always returns to keep
                    if (Circumstance.Equals("pillage"))
                    {
                        if (!attackerVictorious)
                        {
                            attacker.Leader.InKeep = true;
                        }
                    }
                }

                // 2. DEFENDER

                // need to check if defending army had a leader
                if (defender.Leader != null)
                {
                    // if army leader a PC, check entourage
                    if (defender.Leader is PlayerCharacter)
                    {
                        for (int i = 0; i < (defender.Leader as PlayerCharacter).MyNPCs.Count; i++)
                        {
                            if ((defender.Leader as PlayerCharacter).MyNPCs[i].InEntourage)
                            {
                                characterDead = (defender.Leader as PlayerCharacter).MyNPCs[i].CalculateCombatInjury(casualtyModifiers[1]);
                            }

                            // process death, if applicable
                            if (characterDead)
                            {
                                (defender.Leader as PlayerCharacter).MyNPCs[i].ProcessDeath("injury");
                            }
                        }
                    }

                    // check army leader
                    defenderLeaderDead = defender.Leader.CalculateCombatInjury(casualtyModifiers[1]);

                    // process death, if applicable
                    if (defenderLeaderDead)
                    {
                        deadCharacters.Add(defender.Leader.FirstName + " " + defender.Leader.FamilyName);
                        Character newLeader = null;

                        // if possible, elect new leader from entourage
                        if (defender.Leader is PlayerCharacter)
                        {
                            if ((defender.Leader as PlayerCharacter).MyNPCs.Count > 0)
                            {
                                // get new leader
                                newLeader = (defender.Leader as PlayerCharacter).ElectNewArmyLeader();
                            }
                        }

                        // assign newLeader (can assign null leader if none found)
                        defender.AssignNewLeader(newLeader);
                    }
                }

                battleResults.Deaths = deadCharacters.ToArray();
                battleResults.RetreatedArmies = retreatedArmies.ToArray();
                
                battleResults.AttackerVictorious = attackerVictorious;

                // check for SIEGE RELIEF
                if (thisSiege != null)
                {
                    battleResults.IsSiege = true;
                    battleResults.SiegeBesieger = thisSiege.GetBesiegingPlayer();
                    battleResults.SiegeDefender = thisSiege.GetDefendingPlayer();
                    // attacker (relieving army) victory or defender (besieging army) retreat = relief
                    if ((attackerVictorious) || (retreatDistances[1] > 0))
                    {
                        // indicate siege raised
                        siegeRaised = true;
                        battleResults.SiegeRaised = true;
                    }

                    // check to see if siege raised due to death of siege owner with no heir
                    else if ((defenderLeaderDead) && ((defender.Leader as PlayerCharacter) == thisSiege.GetBesiegingPlayer()))
                    {
                        // get siege owner's heir
                        Character thisHeir = (defender.Leader as PlayerCharacter).GetHeir();

                        if (thisHeir == null)
                        {
                            battleResults.DefenderDeadNoHeir = true;
                            // indicate siege raised
                            siegeRaised = true;

                        }
                    }
                }

            }

            // =================== construct and send JOURNAL ENTRY
            // ID
            Console.WriteLine("Move Journal entry to where battle is being called!!!!!!!");
            /*uint entryID = Globals_Game.GetNextJournalEntryID();

            // personae
            // personae tags vary depending on Circumstance
            string attackOwnTag = "|attackerOwner";
            string attackLeadTag = "|attackerLeader";
            string defendOwnTag = "|defenderOwner";
            string defendLeadTag = "|defenderLeader";
            if ((Circumstance.Equals("pillage")) || (Circumstance.Equals("siege")))
            {
                attackOwnTag = "|sallyOwner";
                attackLeadTag = "|sallyLeader";
                defendOwnTag = "|defenderAgainstSallyOwner";
                defendLeadTag = "|defenderAgainstSallyLeader";
            }
            List<string> tempPersonae = new List<string>();
            tempPersonae.Add(defender.Owner.charID + defendOwnTag);
            if (attackerLeader != null)
            {
                tempPersonae.Add(attackerLeader.charID + attackLeadTag);
            }
            if (defenderLeader != null)
            {
                tempPersonae.Add(defenderLeader.charID + defendLeadTag);
            }
            tempPersonae.Add(attacker.Owner.charID + attackOwnTag);
            tempPersonae.Add(attacker.Location.owner.charID + "|fiefOwner");
            if ((!Circumstance.Equals("pillage")) && (!Circumstance.Equals("siege")))
            {
                tempPersonae.Add("all|all");
            }
            string[] battlePersonae = tempPersonae.ToArray();

            // location
            string battleLocation = attacker.Location.id;


            // put together new journal entry
            JournalEntry BattleResults = new JournalEntry(entryID, Globals_Game.clock.currentYear, Globals_Game.clock.currentSeason, battlePersonae, "battle",battleResults, loc: battleLocation);

            // add new journal entry to pastEvents
            Globals_Game.AddPastEvent(BattleResults);

            // display pop-up informational message
            battleResults.ActionType = Actions.Update;
            battleResults.ResponseType = DisplayMessages.BattleResultss;
            if (battleHasCommenced)
            {
                Globals_Game.UpdatePlayer(defender.Owner.PlayerID, DisplayMessages.BattleBringSuccess, new string[] { battleResults.attackerOwner });
            }
            else
            {
                Globals_Game.UpdatePlayer(defender.Owner.PlayerID, DisplayMessages.BattleBringFail, new string[] { battleResults.attackerOwner });
            }

            // end siege if appropriate
            if (siegeRaised)
            {
                //HACK
                thisSiege.SiegeEnd(false, DisplayMessages.BattleResultss,new string[]{DisplaySiegeResults(battleResults)});
                thisSiege = null;

                // ensure if siege raised correct value returned to Form1.siegeReductionRound method
                if (Circumstance.Equals("siege"))
                {
                    attackerVictorious = true;
                }
            }

            // process leader deaths
            if (defenderLeaderDead)
            {
                defenderLeader.ProcessDeath("injury");
            }
            else if (attackerLeaderDead)
            {
                attackerLeader.ProcessDeath("injury");
            }*/


            // DISBANDMENT

            // if is pillage, attacking (temporary) army always disbands after battle
            if (Circumstance.Equals("pillage"))
            {
                attackerDisbanded = true;
            }

            // process army disbandings (after all other functions completed)
            if (attackerDisbanded)
            {
                attacker.DisbandArmy();
                attacker = null;
            }

            if (defenderDisbanded)
            {
                defender.DisbandArmy();
                defender = null;
            }

            return attackerVictorious;

        }


    }
}
