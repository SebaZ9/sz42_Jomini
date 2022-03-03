using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JominiGame
{
    public static class PillageSiege
    {

        /// <summary>
        /// Calculates the outcome of the pillage of a fief by an army
        /// </summary>
        /// <param name="f">The fief being pillaged</param>
        /// <param name="a">The pillaging army</param>
        /// <param name="circumstance">The circumstance under which the fief is being pillaged</param>
        public static PillageResult ProcessPillage(Fief f, Army a, string circumstance = "pillage")
        {
            PillageResult pillageResult = new PillageResult();
            double thisLoss = 0;
            double moneyPillagedTotal = 0;
            double moneyPillagedOwner = 0;
            double pillageMultiplier = 0;
            // get army leader
            Character armyLeader = a.Leader;

            // get pillaging army owner (receives a proportion of total spoils)
            PlayerCharacter armyOwner = a.Owner;

            pillageResult.PillagedFief = f;
            // get garrison leader (to add to journal entry)
            Character defenderLeader = null;
            if (f.Bailiff != null)
            {
                defenderLeader = f.Bailiff;
            }

            // calculate pillageMultiplier (based on no. pillagers per 1000 population)
            pillageMultiplier = a.CalcArmySize() / (f.Population / 1000);

            // calculate days taken for pillage
            double daysTaken = Random.Shared.Next(7, 16);
            if (daysTaken > a.Days)
            {
                daysTaken = a.Days;
            }

            // update army days
            armyLeader.AdjustDays(daysTaken);
            pillageResult.DaysTaken = daysTaken;

            // % population loss
            thisLoss = (0.007 * pillageMultiplier);
            // ensure is between 1%-20%
            if (thisLoss < 1)
            {
                thisLoss = 1;
            }
            else if (thisLoss > 20)
            {
                thisLoss = 20;
            }
            // apply population loss
            pillageResult.PopulationLoss = Convert.ToInt32((f.Population * (thisLoss / 100)));
            f.Population -= Convert.ToInt32((f.Population * (thisLoss / 100)));

            // % treasury loss
            if (!circumstance.Equals("quellRebellion"))
            {
                thisLoss = (0.2 * pillageMultiplier);
                // ensure is between 1%-80%
                if (thisLoss < 1)
                {
                    thisLoss = 1;
                }
                else if (thisLoss > 80)
                {
                    thisLoss = 80;
                }
                // apply treasury loss
                if (f.Treasury > 0)
                {
                    pillageResult.TreasuryLoss = Convert.ToInt32((f.Treasury * (thisLoss / 100)));
                    f.AdjustTreasury(-Convert.ToInt32((f.Treasury * (thisLoss / 100))));
                }
            }

            // % loyalty loss
            thisLoss = (0.33 * pillageMultiplier);
            // ensure is between 1%-20%
            if (thisLoss < 1)
            {
                thisLoss = 1;
            }
            else if (thisLoss > 20)
            {
                thisLoss = 20;
            }
            // apply loyalty loss
            pillageResult.LoyaltyLoss = (f.Loyalty * (thisLoss / 100));

            f.Loyalty -= (f.Loyalty * (thisLoss / 100));

            // % fields loss
            thisLoss = (0.01 * pillageMultiplier);
            // ensure is between 1%-20%
            if (thisLoss < 1)
            {
                thisLoss = 1;
            }
            else if (thisLoss > 20)
            {
                thisLoss = 20;
            }
            // apply fields loss
            pillageResult.FieldsLoss = (f.Fields * (thisLoss / 100));
            f.Fields -= (f.Fields * (thisLoss / 100));

            // % industry loss
            thisLoss = (0.01 * pillageMultiplier);
            // ensure is between 1%-20%
            if (thisLoss < 1)
            {
                thisLoss = 1;
            }
            else if (thisLoss > 20)
            {
                thisLoss = 20;
            }
            // apply industry loss
            pillageResult.IndustryLoss = (f.Industry * (thisLoss / Convert.ToDouble(100)));
            f.Industry -= (f.Industry * (thisLoss / 100));

            // money pillaged (based on GDP)
            thisLoss = (0.032 * pillageMultiplier);
            // ensure is between 1%-50%
            if (thisLoss < 1)
            {
                thisLoss = 1;
            }
            else if (thisLoss > 50)
            {
                thisLoss = 50;
            }
            // calculate base amount pillaged based on fief GDP
            double baseMoneyPillaged = (f.KeyStatsCurrent[1] * (thisLoss / 100));
            moneyPillagedTotal = baseMoneyPillaged;
            pillageResult.BaseMoneyPillaged = baseMoneyPillaged;

            // factor in no. days spent pillaging (get extra 5% per day > 7)
            int daysOver7 = Convert.ToInt32(daysTaken) - 7;
            if (daysOver7 > 0)
            {
                for (int i = 0; i < daysOver7; i++)
                {
                    moneyPillagedTotal += (baseMoneyPillaged * 0.05);
                }
                pillageResult.BonusMoneyPillaged = moneyPillagedTotal - baseMoneyPillaged;
                pillageResult.DaysTaken = daysOver7;
            }

            // check for jackpot
            // generate randomPercentage to see if hit the jackpot
            int myRandomPercent = Random.Shared.Next(101);
            if (myRandomPercent <= 30)
            {
                // generate random int to multiply amount pillaged
                int myRandomMultiplier = Random.Shared.Next(3, 11);
                pillageResult.Jackpot = moneyPillagedTotal * myRandomMultiplier - moneyPillagedTotal;
                moneyPillagedTotal = moneyPillagedTotal * myRandomMultiplier;
            }

            // check proportion of money pillaged goes to army owner (based on stature)
            double proportionForOwner = 0.05 * armyOwner.CalculateStature();
            moneyPillagedOwner = (moneyPillagedTotal * proportionForOwner);
            pillageResult.MoneyPillagedOwner = moneyPillagedOwner;

            // apply to army owner's home fief treasury
            armyOwner.HomeFief.AdjustTreasury(Convert.ToInt32(moneyPillagedOwner));

            // apply loss of stature to army owner if fief has same language
            if (armyOwner.Language.ID == f.FiefsLanguage.ID)
            {
                armyOwner.AdjustStatureModifier(-0.3);
                pillageResult.StatureModifier = (-0.3);
            }
            else if (armyOwner.Language.BaseLang.BaseLangID == f.FiefsLanguage.BaseLang.BaseLangID)
            {
                armyOwner.AdjustStatureModifier(-0.2);
                pillageResult.StatureModifier = (-0.2);
            }

            // set isPillaged for fief
            f.IsPillaged = true;

            #region Journal
            Console.WriteLine("!!!--- Journal Not Implemented ---!!!");
            /*
            // =================== construct and send JOURNAL ENTRY
            // ID
            uint entryID = Globals_Game.GetNextJournalEntryID();

            // personae
            List<string> tempPersonae = new List<string>();
            tempPersonae.Add(f.owner.charID + "|fiefOwner");
            tempPersonae.Add(armyOwner.charID + "|attackerOwner");
            if (armyLeader != null)
            {
                tempPersonae.Add(armyLeader.charID + "|attackerLeader");
            }
            if ((defenderLeader != null) && (!circumstance.Equals("quellRebellion")))
            {
                tempPersonae.Add(defenderLeader.charID + "|defenderLeader");
            }
            if (circumstance.Equals("quellRebellion"))
            {
                tempPersonae.Add("all|all");
            }
            string[] pillagePersonae = tempPersonae.ToArray();

            // location
            string pillageLocation = f.id;

            // type
            string type = "";
            if (circumstance.Equals("pillage"))
            {
                type += "pillage";
            }
            else if (circumstance.Equals("quellRebellion"))
            {
                type += "rebellionQuelled";
            }

            if (circumstance.Equals("pillage"))
            {
                pillageResult.isPillage = true;
            }
            else if (circumstance.Equals("quellRebellion"))
            {
                pillageResult.isPillage = false;
            }
            pillageResult.fiefName = f.name;
            pillageResult.fiefOwner = f.owner.firstName + " " + f.owner.familyName;

            if ((circumstance.Equals("pillage")) && (defenderLeader != null))
            {
                if (f.owner != defenderLeader)
                {
                    pillageResult.defenderLeader = defenderLeader.firstName + " " + defenderLeader.familyName;
                }
            }

            pillageResult.armyOwner = armyOwner.firstName + " " + armyOwner.familyName;
            if (armyLeader != null)
            {
                pillageResult.armyLeader = armyLeader.firstName + " " + armyLeader.familyName;
            }

            // put together new journal entry
            JournalEntry pillageEntry = new JournalEntry(pillageResult, entryID, Globals_Game.clock.currentYear, Globals_Game.clock.currentSeason, pillagePersonae, type, loc: pillageLocation);

            // add new journal entry to pastEvents
            Globals_Game.AddPastEvent(pillageEntry);
            */
            #endregion
            
            return pillageResult;
        }


        /// <summary>
        /// Implements conditional checks prior to the pillage or siege of a fief
        /// </summary>
        /// <returns>bool indicating whether pillage/siege can proceed</returns>
        /// <param name="f">The fief being pillaged/besieged</param>
        /// <param name="a">The pillaging/besieging army</param>
        /// <param name="circumstance">The circumstance - pillage or siege</param>
        public static bool ChecksBeforePillageSiege(Army a, Fief f, string circumstance = "pillage")
        {
            bool proceed = true;
            string operation = "";

            // check if is your own fief
            // note: not necessary for quell rebellion
            if (!circumstance.Equals("quellRebellion"))
            {
                if (f.Owner == a.Owner)
                {
                    proceed = false;
                    if (circumstance.Equals("pillage"))
                    {
                        //result = new ProtoMessage();
                        //result.ResponseType = DisplayMessages.PillageOwnFief;
                        //result.MessageFields = new string[] { "pillage" };
                    }
                    else if (circumstance.Equals("siege"))
                    {
                        //result = new ProtoMessage();
                        //result.ResponseType = DisplayMessages.PillageOwnFief;
                        //result.MessageFields = new string[] { "siege" };
                    }
                }
            }

            // check if fief is under siege
            // note: not necessary for quell rebellion
            if (!circumstance.Equals("quellRebellion"))
            {
                if (f.CurrentSiege != null && (proceed))
                {
                    proceed = false;
                    if (circumstance.Equals("pillage"))
                    {
                        //result = new ProtoMessage();
                        //result.ResponseType = DisplayMessages.PillageUnderSiege;
                    }
                    else if (circumstance.Equals("siege"))
                    {
                        //result = new ProtoMessage();
                        //result.ResponseType = DisplayMessages.PillageSiegeAlready;
                    }
                }
            }

            // check if fief already pillaged
            // note: not necessary for quell rebellion (get a 'free' pillage)
            if (!circumstance.Equals("quellRebellion"))
            {
                if (circumstance.Equals("pillage"))
                {
                    // check isPillaged = false
                    if ((f.IsPillaged) && (proceed))
                    {
                        proceed = false;
                        //result = new ProtoMessage();
                        //result.ResponseType = DisplayMessages.PillageAlready;
                    }
                }
            }

            // check if your army has a leader
            if (a.Leader == null)
            {
                proceed = false;

                if (circumstance.Equals("quellRebellion"))
                {
                    operation = "Operation";
                }
                if (circumstance.Equals("pillage"))
                {
                    operation = "Pillage";
                }
                else if (circumstance.Equals("siege"))
                {
                    operation = "Siege";
                }
                //result = new ProtoMessage();
                //result.ResponseType = DisplayMessages.ArmyNoLeader;
            }

            // check has min days required
            if ((circumstance.Equals("pillage")) || (circumstance.Equals("quellRebellion")))
            {
                // pillage = min 7
                if ((a.Days < 7) && (proceed))
                {
                    proceed = false;
                    if (circumstance.Equals("quellRebellion"))
                    {
                        operation = "Quell rebellion";
                    }
                    else
                    {
                        operation = "Pillage";
                    }
                    //result = new ProtoMessage();
                    //result.ResponseType = DisplayMessages.ErrorGenericNotEnoughDays;
                }
            }
            else if (circumstance.Equals("siege"))
            {
                // siege = 1 (to set up siege)
                if ((a.Days < 1) && (proceed))
                {
                    proceed = false;
                    //result = new ProtoMessage();
                    //result.ResponseType = DisplayMessages.ErrorGenericNotEnoughDays;
                }
            }

            // check for presence of armies belonging to fief owner
            if (proceed)
            {
                // iterate through armies in fief
                for (int i = 0; i < f.Armies.Count; i++)
                {
                    // get army
                    Army armyInFief = f.Armies[i];

                    // check if owned by fief owner
                    if (armyInFief.Owner.Equals(f.Owner.ID))
                    {
                        // army must be outside keep
                        if (!armyInFief.Leader.InKeep)
                        {
                            // army must have correct aggression settings
                            if (armyInFief.Aggression > 1)
                            {
                                proceed = false;
                                if (circumstance.Equals("pillage"))
                                {
                                    operation = "Pillage";
                                }
                                else if (circumstance.Equals("siege"))
                                {
                                    operation = "Siege";
                                }
                                else if (circumstance.Equals("quellRebellion"))
                                {
                                    operation = "Quell rebellion";
                                }
                                //result = new ProtoMessage();
                                //result.ResponseType = DisplayMessages.PillageArmyDefeat;
                                //result.MessageFields = new string[] { armyInFief.armyID };
                                break;
                            }
                        }
                    }
                }

                // check if fief in rebellion
                if ((circumstance.Equals("siege")) && (proceed))
                {
                    if (f.Status.Equals('R'))
                    {
                        proceed = false;
                        //result = new ProtoMessage();
                        //result.ResponseType = DisplayMessages.PillageSiegeRebellion;
                    }
                }
            }

            return proceed;
        }

        /// <summary>
        /// Allows an attacking army to lay siege to an enemy fief
        /// </summary>
        /// <param name="attacker">The attacking army</param>
        /// <param name="target">The fief to be besieged</param>
        public static Siege SiegeStart(Army attacker, Fief target)
        {
            Army defenderGarrison = null;
            Army defenderAdditional = null;

            // check for existence of army in keep
            for (int i = 0; i < target.Armies.Count; i++)
            {
                // get army
                Army armyInFief = target.Armies[i];

                // check is in keep
                Character armyLeader = armyInFief.Leader;
                if (armyLeader != null)
                {
                    if (armyLeader.InKeep)
                    {
                        // check owner is same as that of fief (i.e. can help in siege)
                        if (armyInFief.Owner == target.Owner)
                        {
                            defenderAdditional = armyInFief;
                            break;
                        }
                    }
                }
            }

            // create defending force
            defenderGarrison = target.CreateDefendingArmy();

            // get the minumum days of all army objects involved
            double minDays = Math.Min(attacker.Days, defenderGarrison.Days);
            if (defenderAdditional != null)
            {
                minDays = Math.Min(minDays, defenderAdditional.Days);
            }

            // get defenderAdditional ID, or null if no defenderAdditional
            Army defAddID = null;
            if (defenderAdditional != null)
            {
                defAddID = defenderAdditional;
            }

            // create siege object
            Siege mySiege = new Siege(
                0,
                0,
                attacker.Owner,
                target.Owner,
                attacker,
                defenderGarrison,
                target,
                minDays,
                target.KeepLevel,
                DefenderAdditional: defAddID);


            // add to siege owners
            mySiege.GetBesiegingPlayer().MySieges.Add(mySiege);
            mySiege.GetDefendingPlayer().MySieges.Add(mySiege);

            // add to fief
            target.CurrentSiege = mySiege;

            // reduce expenditures in fief, except for garrison
            target.InfrastructureSpendNext = 0;
            target.KeepSpendNext = 0;
            target.OfficialSpendNext = 0;

            // update days (NOTE: siege.days will be updated in syncDays)
            mySiege.TotalDays++;

            // sychronise days
            mySiege.SyncSiegeDays(mySiege.Days - 1);


            #region Journal
            Console.WriteLine("!!!--- Journal Not Implemented ---!!!");
            /*
            // =================== construct and send JOURNAL ENTRY
            // ID
            uint entryID = Globals_Game.GetNextJournalEntryID();

            // personae
            List<string> tempPersonae = new List<string>();
            tempPersonae.Add("all|all");
            tempPersonae.Add(mySiege.GetDefendingPlayer().charID + "|fiefOwner");
            tempPersonae.Add(mySiege.GetBesiegingPlayer().charID + "|attackerOwner");
            tempPersonae.Add(attacker.Leader.charID + "|attackerLeader");
            // get defenderLeader
            Character defenderLeader = defenderGarrison.Leader;
            if (defenderLeader != null)
            {
                tempPersonae.Add(defenderLeader.charID + "|defenderGarrisonLeader");
            }
            // get additional defending leader
            Character addDefendLeader = null;
            if (defenderAdditional != null)
            {
                addDefendLeader = defenderAdditional.Leader;
                if (addDefendLeader != null)
                {
                    tempPersonae.Add(addDefendLeader.charID + "|defenderAdditionalLeader");
                }
            }
            string[] siegePersonae = tempPersonae.ToArray();

            // location
            string siegeLocation = mySiege.GetFief().id;

            // description
            string[] fields = new string[6];
            fields[0] = mySiege.GetBesiegingPlayer().firstName + " " + mySiege.GetBesiegingPlayer().familyName;
            fields[1] = attacker.Leader.firstName + " " + attacker.Leader.familyName;
            fields[2] = mySiege.GetFief().name;
            fields[3] = mySiege.GetDefendingPlayer().firstName + " " + mySiege.GetDefendingPlayer().familyName;
            fields[4] = fields[5] = "";
            if (defenderLeader != null)
            {
                fields[4] = "The defending garrison is led by " + defenderLeader.firstName + " " + defenderLeader.familyName + ".";
            }
            if (addDefendLeader != null)
            {
                fields[5] = "Additional defending forces are led by " + addDefendLeader.firstName + " " + addDefendLeader.familyName + ".";
            }

            ProtoMessage siege = new ProtoMessage();
            siege.MessageFields = fields;
            siege.ResponseType = DisplayMessages.PillageInitiateSiege;
            // put together new journal entry
            JournalEntry siegeResult = new JournalEntry(entryID, Globals_Game.clock.currentYear, Globals_Game.clock.currentSeason, siegePersonae, "siege", siege, loc: siegeLocation);

            // add new journal entry to pastEvents
            Globals_Game.AddPastEvent(siegeResult);
            */
            #endregion

            return mySiege;
        }

    }
}
