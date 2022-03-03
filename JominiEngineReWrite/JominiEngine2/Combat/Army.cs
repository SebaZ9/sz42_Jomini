using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JominiGame
{
    /// <summary>
    /// Class storing data on army 
    /// </summary>
    public class Army : BaseGameObject
    {
        /// <summary>
        /// Holds army ID
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// Holds troops in army
        /// 0 = knights
        /// 1 = menAtArms
        /// 2 = lightCav
        /// 3 = longbowmen
        /// 4 = crossbowmen
        /// 5 = foot
        /// 6 = rabble
        /// </summary>
        public uint[] Troops = new uint[7] { 0, 0, 0, 0, 0, 0, 0 };
        /// <summary>
        /// Holds army leader (ID)
        /// </summary>
        public Character? Leader { get; set; }
        /// <summary>
        /// Holds army owner (ID)
        /// </summary>
        public PlayerCharacter Owner { get; set; }
        /// <summary>
        /// Holds army's remaining days in season
        /// </summary>
        public double Days { get; set; }
        /// <summary>
        /// Holds army location (fiefID)
        /// </summary>
        public Fief Location { get; set; }
        /// <summary>
        /// Indicates whether army is being actively maintained by owner
        /// </summary>
        public bool IsMaintained { get; set; }
        /// <summary>
        /// Indicates army's aggression level (automated response to combat)
        /// </summary>
        public byte Aggression { get; set; }
        /// <summary>
        /// Indicates army's combat odds value (i.e. at what odds will attempt automated combat action)
        /// </summary>
        public byte CombatOdds { get; set; }

        /// <summary>
        /// Constructor for Army
        /// </summary>
		/// <param name="ID">String holding ID of army</param>
        /// <param name="Leader">string holding ID of army leader</param>
        /// <param name="Owner">string holding ID of army owner</param>
        /// <param name="RemainingDays">double holding remaining days in season for army</param>
        /// <param name="Location">string holding army location (fiefID)</param>
        /// <param name="IsMaintained">bool indicating whether army is being actively maintained by owner</param>
        /// <param name="Aggression">byte indicating army's aggression level</param>
        /// <param name="CombatOdds">byte indicating army's combat odds value</param>
        /// <param name="Troops">uint[] holding troops in army</param>
        public Army(string ID, Character Leader, PlayerCharacter Owner, double RemainingDays, Fief Location, GameClock Clock, IdGenerator IDGen, HexMapGraph GameMap,
            bool IsMaintained = false, byte Aggression = 1, byte CombatOdds = 9, uint[] Troops = null)
            : base(Clock, IDGen, GameMap)
        {            
            this.ID = ID;
            this.Leader = Leader;
            this.Owner = Owner;
            this.Days = RemainingDays;
            this.Location = Location;
            this.IsMaintained = IsMaintained;
            this.Aggression = Aggression;
            this.CombatOdds = CombatOdds;
            if (Troops != null)
            {
                if (Troops.Length == 7)
                {
                    this.Troops = Troops;
                }
            }
        }

        /// <summary>
        /// Calculates the maintenance cost for this army
        /// </summary>
        /// <returns>uint representing cost</returns>
        public uint GetMaintenanceCost()
        {
            return CalcArmySize() * 500;
        }

        /// <summary>
        /// Maintains the specified field army
        /// </summary>
        public bool MaintainArmy()
        {
            // get cost
            uint maintCost = GetMaintenanceCost();

            // get available treasury
            Fief homeFief = ((PlayerCharacter)Owner).HomeFief;
            int availTreas = homeFief.GetAvailableTreasury(true);

            // check if army is already maintained
            if (!IsMaintained)
            {
                // check if can afford maintenance
                if (maintCost > availTreas)
                {
                    return false;
                }
                else
                {
                    // set isMaintained
                    IsMaintained = true;
                    // deduct funds from treasury
                    homeFief.AdjustTreasury(-Convert.ToInt32(maintCost));
                    return true;
                }
            }
            else
            {
                return false;
            }

        }

        /// <summary>
        /// Updates the army's aggression and combatOdds values
        /// </summary>
        /// <returns>bool indicating success</returns>
        /// <param name="newAggroLevel">The new aggression value</param>
        /// <param name="newOddsValue">The new combatOdds value</param>
        public bool AdjustStandingOrders(byte newAggroLevel, byte newOddsValue)
        {
            bool success = true;

            // check values and alter if appropriate
            if (newAggroLevel < 0)
            {
                newAggroLevel = 0;
            }
            else if (newAggroLevel > 2)
            {
                newAggroLevel = 2;
            }
            if (newOddsValue < 0)
            {
                newOddsValue = 0;
            }
            else if (newOddsValue > 9)
            {
                newOddsValue = 9;
            }

            // update army's values
            this.Aggression = newAggroLevel;
            this.CombatOdds = newOddsValue;

            if ((this.Aggression != newAggroLevel) || (this.CombatOdds != newOddsValue))
            {
                success = false;
            }

            return success;
        }

        /// <summary>
        /// Assigns a new leader to the army
        /// </summary>
        /// <remarks>
        /// Predicate: assumes leader is in same fief as army
        /// NOTE: you CAN assign a null character as leader (i.e. the army becomes leaderless)
        /// </remarks>
        /// /// <param name="newLeader">The new leader (can be null)</param>
        public void AssignNewLeader(Character newLeader)
        {
            // check if already leader of another army and remove if necessary
            Army otherArmy = null;
            if (newLeader.ArmyID != null)
            {
                if (newLeader.ArmyID != null)
                {
                    if (newLeader.ArmyID != this)
                    {
                        newLeader.ArmyID.Leader = null;
                    }
                }
            }

            // check if army is involved in a siege
            Siege mySiege = CheckForSiegeRole();

            // Remove army from current leader
            if (Leader != null)
            {
                Leader.ArmyID = null;
            }

            // if no new leader (i.e. if just removing old leader)
            if (newLeader == null)
            {
                // in army, set new leader
                Leader = null;
            }

            // if is new leader
            else
            {
                // add army to new leader
                newLeader.ArmyID = this;

                // in army, set new leader
                Leader = newLeader;

                // if new leader is NPC, remove from player's entourage
                if (newLeader is NonPlayerCharacter)
                {
                    ((NonPlayerCharacter)newLeader).removeSelfFromEntourage();
                }

                // calculate days synchronisation
                double minDays = Math.Min(newLeader.Days, this.Days);
                double maxDays = Math.Max(newLeader.Days, this.Days);
                double difference = maxDays - minDays;

                if (newLeader.Days != minDays)
                {
                    // synchronise days
                    newLeader.AdjustDays(difference);
                }
                else
                {
                    // if army not involved in siege, check for attrition in normal way
                    if (mySiege == null)
                    {
                        byte attritionChecks = 0;
                        attritionChecks = Convert.ToByte(difference / 7);

                        for (int i = 0; i < attritionChecks; i++)
                        {
                            // calculate attrition
                            double attritionModifer = this.CalcAttrition();
                            // apply attrition
                            this.ApplyTroopLosses(attritionModifer);
                        }
                    }

                    // if army is involved in siege, attrition applied at siege level
                    else
                    {
                        mySiege.SyncSiegeDays(newLeader.Days);
                    }

                }

            }
        }

        /// <summary>
        /// Calculates total army size
        /// </summary>
        /// <returns>uint containing army size</returns>
        public uint CalcArmySize()
        {
            uint armySize = 0;

            foreach (uint troopType in Troops)
            {
                armySize += troopType;
            }

            return armySize;
        }

        /// <summary>
        /// Moves army to another fief
        /// </summary>
        /// <remarks>
        /// Predicate: assumes leader has enough days for movement
        /// Must be called from within MoveCharacter!
        /// If moving a leaderless army, use MoveWithoutLeader
        /// </remarks>
        /// <returns>bool indicating success</returns>
        /// <param name="showAttrition">bool indicating whether to display message containing attrition losses</param>
        public bool MoveArmy(bool showAttrition = false)
        {
            if (Leader == null)
            {
                return false;
            }
            // get new fief

            // remove from old fief
            if (!Location.RemoveArmy(this))
            {
                return false;
            }
            else
            {
                // add to new fief
                Leader.Location.AddArmy(this);

                // change location
                Location = Leader.Location;

                // update days
                Days = Leader.Days;

                // calculate attrition
                double attritionModifer = CalcAttrition();
                // apply attrition
                uint troopsLost = ApplyTroopLosses(attritionModifer);

                // inform player of losses
                if (showAttrition)
                {
                    if (troopsLost > 0)
                    {
                        //string[] fields = new string[] { ID, troopsLost.ToString(), myNewFief.name };
                        //Globals_Game.UpdatePlayer(Owner.playerID, DisplayMessages.ArmyMove, fields);
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Calculates movement modifier for the army
        /// </summary>
        /// <returns>uint containing movement modifier</returns>
        public uint CalcMovementModifier()
        {
            uint movementMod = 1;

            // generate random double (0-100)
            double myRandomDouble = Random.Shared.NextDouble() * 100;

            // calculate chance of modifier based on army size
            double modifierChance = (Math.Floor(this.CalcArmySize() / (double)1000) * 3);

            // check to see if modifier required
            if (myRandomDouble <= modifierChance)
            {
                movementMod = 3;
            }

            return movementMod;
        }

        /// <summary>
        /// Calculates attrition for the army
        /// </summary>
        /// <returns>double containing casualty modifier to be applied troops</returns>
        public double CalcAttrition()
        {
            uint troopNumbers = CalcArmySize();
            double casualtyModifier = 0;
            double attritionChance = 0;
            string toDisplay = "";
            // initialise fields for display
            string[] fields = new string[] { "", "", "", "", "" };
            // ensure is no attrition if army maintained
            if (!IsMaintained)
            {
                // calculate base chance of attrition
                attritionChance = (troopNumbers / Convert.ToDouble(Location.Population)) * 100;
                fields[0] = attritionChance + "";
                toDisplay += "Base chance: " + attritionChance + "\r\n";

                // factor in effect of leader (need to check if army has leader)
                if (Leader != null)
                {
                    // apply effect of leader
                    //attritionChance = attritionChance - ((Leader.CalculateStature() + Leader.management) / 2);
                    //fields[1] = "Leader effect: " + (Leader.CalculateStature() + Leader.management) / 2 + "\r\n";
                }

                /*// factor in effect of season (add 20 if is winter or spring)
                if ((Globals_Game.clock.currentSeason == 0) || (Globals_Game.clock.currentSeason == 3))
                {
                    attritionChance = attritionChance + 20;
                    fields[2] = "Season effect: 20\r\n";
                }*/

                // normalise chance of attrition
                if (attritionChance < 10)
                {
                    attritionChance = 10;
                }
                else if (attritionChance > 100)
                {
                    attritionChance = 100;
                }

                // generate random number (0-100) to check if attrition occurs
                Double randomPercent = Random.Shared.NextDouble() * 100;

                // check if attrition occurs
                if (randomPercent <= attritionChance)
                {
                    // calculate base casualtyModifier
                    casualtyModifier = (troopNumbers / Convert.ToDouble(Location.Population)) / 10;
                    fields[3] = "casualtyModifier: " + casualtyModifier + "\r\n";

                    /*// factor in effect of season on potential losses (* 3 if is winter or spring)
                    if ((Globals_Game.clock.currentSeason == 0) || (Globals_Game.clock.currentSeason == 3))
                    {
                        casualtyModifier = casualtyModifier * 3;
                        fields[4] = "casualtyModifier after seasonal effect: " + casualtyModifier + "\r\n";
                    }*/

                }
            }

            return casualtyModifier;
        }

        /// <summary>
        /// Applies troop losses after attrition, battle, siege, etc.
        /// </summary>
        /// <remarks>
        /// Predicate: assumes (lossModifier gt 0) && (lossModifier lt 1)
        /// </remarks>
        /// <returns>uint containing total number of troops lost</returns>
        /// <param name="lossModifier">modifier to be applied to each troop type</param>
        public uint ApplyTroopLosses(double lossModifier)
        {
            // keep track of total troops lost
            uint troopsLost = 0;

            for (int i = 0; i < Troops.Length; i++)
            {
                uint thisTypeLost = Convert.ToUInt32(Troops[i] * lossModifier);
                troopsLost += thisTypeLost;
                Troops[i] -= thisTypeLost;
            }

            return troopsLost;
        }

        /// <summary>
        /// Creates a detachment from the army's troops and leaves it in the fief
        /// </summary>
        /// <remarks>
        /// Predicate: assumes details[0] through details[details.Length-1] contain strings that can be converted to uint
        /// </remarks>
        /// <returns>bool indicating success of transfer</returns>
        /// <param name="details">string[] containing troop numbers and recipient (ID)</param>
        public bool CreateDetachment(uint[] troops, Character? character = null)
        {
            throw new NotImplementedException();
            /*
            bool proceed = true;
            bool adjustDays = true;
            int daysTaken = 0;
            uint totalTroopsToTransfer = 0;
            string[] troopTypeLabels = new string[] { "knights", "men-at-arms", "light cavalry", "longbowmen", "crossbowmen", "foot", "rabble" };
            Character myLeader = null;

            // carry out CONDITIONAL CHECKS
            // 1. check arry length
            if (troops.Length != 7)
            {
                proceed = false;
                adjustDays = false;
            }
            else
            {
                // 2. check each troop type; if not enough in army, cancel
                for (int i = 0; i < troops.Length; i++)
                {
                    if (troops[i] > Troops[i])
                    {
                        proceed = false;
                        adjustDays = false;
                    }
                    else
                    {
                        totalTroopsToTransfer += troops[i];
                    }
                }

                if (proceed)
                {
                    // 3. if no troops selected for transfer, cancel
                    if (totalTroopsToTransfer < 1)
                    {
                        proceed = false;
                        adjustDays = false;
                    }
                    else
                    {
                        // 4. check have minimum days necessary for transfer
                        if (RemainingDays < 10)
                        {
                            proceed = false;
                            adjustDays = false;
                        }
                        else
                        {
                            // 5. check if have enough days for transfer in this instance

                            // calculate time taken for transfer
                            daysTaken = Random.Shared.Next(21) + 10;

                            if (daysTaken > RemainingDays)
                            {
                                proceed = false;
                                adjustDays = false;
                            }
                            else
                            {
                                // 6. check transfer recipient exists
                                if (leftFor == null)
                                {
                                    proceed = false;
                                    adjustDays = false;
                                }
                                else
                                {
                                    // 7. check army has a leader

                                    // get leader
                                    if (Leader == null)
                                    {
                                        proceed = false;
                                        adjustDays = false;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (proceed)
            {
                // remove troops from army
                for (int i = 0; i < this.Troops.Length; i++)
                {
                    Troops[i] -= troops[i];
                }

                // create transfer entry
                ProtoDetachment thisTransfer = new ProtoDetachment();
                thisTransfer.leftBy = Owner;
                thisTransfer.leftFor = leftFor;
                thisTransfer.troops = troops;
                thisTransfer.days = (int)this.RemainingDays - daysTaken;
                // add to fief's troopTransfers list
                string transferID = GameGlobals.GetNextDetachmentID();
                thisTransfer.id = transferID;
                Location.troopTransfers.Add(transferID, thisTransfer);

                // check detachment added to troopTransfers
                if (!Location.troopTransfers.ContainsKey(transferID))
                {
                    proceed = false;
                }
            }

            if (adjustDays)
            {
                // adjust days
                myLeader.AdjustDays(daysTaken);

                // calculate possible attrition for army
                byte attritionChecks = Convert.ToByte(daysTaken / 7);
                for (int i = 0; i < attritionChecks; i++)
                {
                    // calculate attrition
                    double attritionModifer = this.CalcAttrition();

                    // apply attrition
                    if (attritionModifer > 0)
                    {
                        this.ApplyTroopLosses(attritionModifer);
                    }
                }
            }

            return proceed;*/
        }

        /// <summary>
        /// Calculates the army's combat value for a combat engagement (NOTE: doesn't include leadership modifier)
        /// </summary>
        /// <returns>double containing combat value</returns>
        /// <param name="keepLvl">Keep level (if for a keep storm)</param>
        public double CalculateCombatValue(int keepLvl = 0)
        {
            double cv = 0;

            // get leader and owner
            Character myLeader = this.Leader;
            PlayerCharacter myOwner = this.Owner;

            // get nationality (effects combat values)
            string troopNationality = myOwner.Nationality.NatID;

            // get combat values for that nationality
            uint[] thisCombatValues = GameSettings.COMBAT_VALUES[troopNationality];

            // get CV for each troop type
            for (int i = 0; i < Troops.Length; i++)
            {
                cv += Troops[i] * thisCombatValues[i];
            }

            // if calculating defender during keep storm, account for keep level
            // (1000 foot per level)
            if (keepLvl > 0)
            {
                cv += (keepLvl * 1000) * thisCombatValues[5];
            }

            // get leader's combat value
            if (myLeader != null)
            {
                cv += myLeader.GetCombatValue();

                // if leader is PC, get CV of entourage (male characters only)
                if (myLeader is PlayerCharacter)
                {
                    for (int i = 0; i < (myLeader as PlayerCharacter).MyNPCs.Count; i++)
                    {
                        if ((myLeader as PlayerCharacter).MyNPCs[i].InEntourage)
                        {
                            if ((myLeader as PlayerCharacter).MyNPCs[i].IsMale)
                            {
                                cv += (myLeader as PlayerCharacter).MyNPCs[i].GetCombatValue();
                            }
                        }
                    }
                }
            }

            return cv;
        }

        /// <summary>
        /// Calculates the estimated number of troops of all types in the army
        /// </summary>
        /// <returns>uint[] containing estimated troop numbers for all types</returns>
        /// <param name="observer">The character making the estimate</param>
        public uint[] GetTroopsEstimate(Character observer)
        {
            uint[] troopNumbers = new uint[7] { 0, 0, 0, 0, 0, 0, 0 };
            Console.WriteLine("___TEST: Number of troop types: " + Troops.Length + ", length in Troops Estimate: " + troopNumbers.Length);
            if (observer != null)
            {
                // get random int (0-2) to decide whether to over- or under-estimate troop number
                int overUnder = Random.Shared.Next(0, 3);

                // get observer's estimate variance (based on his leadership value)
                double estimateVariance = observer.GetEstimateVariance();

                // perform estimate for each troop type
                for (int i = 0; i < troopNumbers.Length; i++)
                {
                    // get troop number upon which to base estimate
                    troopNumbers[i] = Troops[i];

                    // generate random double between 0 and estimate variance to decide variance in this case
                    double thisVariance = Random.Shared.NextDouble() * estimateVariance;

                    // apply variance (negatively or positively) to troop number
                    // 0 = under-estimate, 1-2 = over-estimate
                    if (overUnder == 0)
                    {
                        troopNumbers[i] = troopNumbers[i] - Convert.ToUInt32(troopNumbers[i] * thisVariance);
                    }
                    else
                    {
                        troopNumbers[i] = troopNumbers[i] + Convert.ToUInt32(troopNumbers[i] * thisVariance);
                    }
                }
            }

            return troopNumbers;
        }



        /// <summary>
        /// Performs functions associated with army move for an army unaccompanied by a leader 
        /// </summary>
        /// <remarks>
        /// Predicate: assumes army has no leader
        /// Predicate: assumes army has sufficient days
        /// </remarks>
        /// <param name="target">The fief to move to</param>
        /// <param name="travelCost">The cost of moving to target fief</param>
        public bool MoveWithoutLeader(Fief target, double travelCost)
        {
            if (Days < travelCost)
            {
                return false;
            }
            // get current location
            Fief from = this.Location;

            // remove from current fief
            from.Armies.Remove(this);

            // add to target fief
            target.Armies.Add(this);

            // change location
            Location = target;

            // change days
            Days = Days - travelCost;

            // calculate attrition
            double attritionModifer = CalcAttrition();

            // apply attrition
            uint troopsLost = ApplyTroopLosses(attritionModifer);
            return true;
        }

        /// <summary>
        /// Checks to see if army is besieging a fief/keep
        /// </summary>
        /// <returns>string containing the siegeID</returns>
        public Siege? CheckIfBesieger()
        {
            if (Location.CurrentSiege != null)
            {
                // check if this army is besieging army
                if (Location.CurrentSiege.GetBesiegingArmy() == this)
                {
                    return Location.CurrentSiege;
                }
            }
            return null;
        }

        /// <summary>
        /// Checks to see if army is the defending garrison in a siege
        /// </summary>
        /// <returns>string containing the siegeID</returns>
        public Siege? CheckIfSiegeDefenderGarrison()
        {
            if (Location.CurrentSiege != null)
            {
                // check if this army is besieging army
                if (Location.CurrentSiege.GetDefenderGarrison() == this)
                {
                    return Location.CurrentSiege;
                }
            }
            return null;
        }

        /// <summary>
        /// Checks to see if army is an additional defending army in a siege
        /// </summary>
        /// <returns>string containing the siegeID</returns>
        public Siege? CheckIfSiegeDefenderAdditional()
        {
            if (Location.CurrentSiege != null)
            {
                // check if this army is besieging army
                if (Location.CurrentSiege.GetDefenderAdditional() == this)
                {
                    return Location.CurrentSiege;
                }
            }
            return null;
        }

        /// <summary>
        /// Checks to see if army has any role (defending or besieging) in a siege
        /// </summary>
        /// <returns>string containing the siegeID</returns>
        public Siege? CheckForSiegeRole()
        {
            Siege? thisSiegeID = null;

            // check if army is a defending garrison in a siege
            thisSiegeID = CheckIfSiegeDefenderGarrison();

            if (thisSiegeID == null)
            {
                // check if army is an additional defending army in a siege
                thisSiegeID = CheckIfSiegeDefenderAdditional();

                if (thisSiegeID == null)
                {
                    // check if army is besieger in a siege
                    thisSiegeID = CheckIfBesieger();
                }
            }

            return thisSiegeID;
        }

        /// <summary>
        /// Updates army data at the end/beginning of the season
        /// </summary>
        /// <returns>bool indicating if army has dissolved</returns>
        public bool UpdateArmy()
        {
            bool hasDissolved = false;
            bool attritionApplies = true;
            Siege? siegeID = null;
            bool isSiegeDefGarr = false;
            bool isSiegeDefAdd = false;
            Siege? thisSiege = null;

            // check for SIEGE INVOLVEMENT
            // check that army is a defending garrison in a siege
            siegeID = CheckIfSiegeDefenderGarrison();
            if (siegeID != null)
            {
                isSiegeDefGarr = true;
                thisSiege = CheckForSiegeRole();
            }
            else
            {
                siegeID = CheckIfSiegeDefenderAdditional();
                // check that army is an additional defending army in a siege
                if (siegeID != null)
                {
                    isSiegeDefAdd = true;
                    thisSiege = CheckForSiegeRole();
                }
            }

            // check to see if attrition applies to defending forces in siege
            // (based on besieged fief bailiff management rating)
            if ((isSiegeDefGarr) || (isSiegeDefAdd))
            {
                attritionApplies = thisSiege.CheckAttritionApplies();
            }

            if (attritionApplies)
            {
                // check for attrition due to days remaining
                byte attritionChecks = Convert.ToByte(Days / 7);
                for (int i = 0; i < attritionChecks; i++)
                {
                    // calculate attrition
                    double attritionModifer = CalcAttrition();
                    // apply attrition
                    uint troopsLost = ApplyTroopLosses(attritionModifer);

                    // update siege losses, if applicable
                    if (thisSiege != null)
                    {
                        // siege defenders
                        if ((isSiegeDefGarr) || (isSiegeDefAdd))
                        {
                            thisSiege.TotalCasualtiesDefender += Convert.ToInt32(troopsLost);
                        }

                        // siege attackers
                        else
                        {
                            thisSiege.TotalCasualtiesAttacker += Convert.ToInt32(troopsLost);
                        }
                    }
                }
            }

            if (!((isSiegeDefGarr) || (isSiegeDefAdd)))
            {
                // check if army dissolves (less than 100 men)
                // NOTE: siege defenders do not dissolve in this way
                if (CalcArmySize() < 100)
                {
                    hasDissolved = true;
                }
            }

            // update army days
            if (!hasDissolved)
            {
                if (Leader != null)
                {
                    Days = Leader.Days;
                }
                else
                {
                    Days = 90;
                }
            }

            // reset isMaintained
            IsMaintained = false;

            return hasDissolved;
        }

        /// <summary>
        /// Runs conditional checks prior to the army launching an attack on another army
        /// </summary>
        /// <returns>bool indicating whether attack can proceed</returns>
        /// <param name="targetArmy">The army to be attacked</param>
        public bool ChecksBeforeAttack(Army targetArmy)
        {
            bool proceed = true;
            // check has enough days to give battle (1)
            if (Days < 1)
            {
                return false;
            }
            if (Location != targetArmy.Location)
            {
                return false;
            }
            else
            {
                // SIEGE INVOLVEMENT (DEFENDER)
                // check if defending army is the garrison in a siege
                Siege? siegeID = targetArmy.CheckIfSiegeDefenderGarrison();
                if (siegeID != null)
                {

                }
                else
                {
                    // check if defending army is the additional defender in a siege
                    siegeID = targetArmy.CheckIfSiegeDefenderAdditional();
                    if (siegeID != null)
                    {
                        proceed = false;
                    }

                    else
                    {
                        // check if are attacking your own army
                        if (Owner == targetArmy.Owner)
                        {
                            proceed = false;
                        }
                    }
                }
            }

            if (proceed)
            {
                // SIEGE INVOLVEMENT (BESIEGER)
                // check if attacking army is besieging a keep

                //TODO client side confirmation of end siege if besieging
                Siege? siegeID = CheckIfBesieger();
                if (siegeID != null)
                {

                }
            }

            return proceed;
        }

        /// <summary>
        /// Disbands the army
        /// </summary>
        public void DisbandArmy()
        {
            // check for siege involvement
            Siege? thisSiege = CheckForSiegeRole();

            // remove from siege
            if (thisSiege != null)
            {
                // check if are additional defending army
                Siege? whichRole = CheckIfSiegeDefenderAdditional();
                if (whichRole != null)
                {
                    thisSiege.DefenderAdditional = null;
                }

                // check if are besieging army
                else
                {
                    whichRole = CheckIfBesieger();
                    if (whichRole != null)
                    {
                        // end siege
                        thisSiege.SiegeEnd(false);
                    }
                }
            }

            // remove from fief
            Fief thisFief = Location;
            thisFief.Armies.Remove(this);

            // remove from owner
            PlayerCharacter thisOwner = Owner;
            if (thisOwner != null)
            {
                thisOwner.MyArmies.Remove(this);
            }

            // remove from leader
            Character? thisLeader = Leader;
            if (thisLeader != null)
            {
                thisLeader.ArmyID = null;
            }
        }

        /// <summary>
        /// Calculates any advantages troops may have against an enemy army based on how effective certain troop types are against each other
        /// </summary>
        /// <param name="enemyTroops">Array of enemy troops</param>
        /// <returns>Resulting advantage based on calculations</returns>
        public uint CalculateTroopTypeAdvatages(uint[] enemyTroops)
        {
            uint advantage = 0;
            // Iterate over troops
            // Outer loop = this army troop type
            for (int i = 0; i < Troops.Length; i++)
            {
                // Inter loop = enemy army troop type
                for (int j = 0; j < Troops.Length; j++)
                {
                    // Get advantage of of this army's current troop type versus the enemy's troop type
                    double thisAdvantage = GameSettings.TROOP_TYPE_ADVANTAGE[new Tuple<uint, uint>((uint)i, (uint)j)];
                    if (thisAdvantage == 0) continue;
                    else
                    {
                        // Calculate difference between troops taking advantage multiplier into account
                        //double difference = this.troops[i] * thisAdvantage - enemyTroops[j];
                        double difference = enemyTroops[j] - (Troops[i] * thisAdvantage);
                        // if enemy has enough troops to counter advantage, advantage is 0
                        if (difference <= 0)
                        {
                            difference = enemyTroops[j];
                        }
                        // If this army has advantage, ensure advantage does not exceed number of enemy troops for this troop types
                        advantage += (uint)difference;
                    }
                }
            }
            return advantage;
        }
        /// <summary>
        /// Calculates battle values of both armies participating in a battle or siege
        /// </summary>
        /// <remarks>This method should be called by the attacking army</remarks>
        /// <returns>uint[] containing battle values of attacking & defending armies</returns>
        /// <param name="attacker">The attacking army</param>
        /// <param name="defender">The defending army</param>
        /// <param name="keepLvl">Keep level (if for a keep storm)</param>
        /// <param name="isSiege">bool indicating if the circumstance is a siege storm</param>
        public uint[] CalculateBattleValues(Army defender, int keepLvl = 0, bool isSiegeStorm = false)
        {

            uint[] battleValues = new uint[2];
            double attackerLV = 1;
            double defenderLV = 1;

            // get leaders
            Character? attackerLeader = Leader;
            Character? defenderLeader = defender.Leader;
            // get leadership values for each army leader
            if (attackerLeader != null)
            {
                attackerLV = attackerLeader.GetLeadershipValue(isSiegeStorm);
            }


            // defender may not have leader
            if (defenderLeader != null)
            {
                defenderLV = defenderLeader.GetLeadershipValue(isSiegeStorm);
            }

            // calculate battle modifier based on LVs
            // determine highest/lowest of 2 LVs
            double maxLV = Math.Max(attackerLV, defenderLV);
            double minLV = Math.Min(attackerLV, defenderLV);
            double battleModifier = maxLV / minLV;

            // get base combat value for each army
            uint attackerCV = Convert.ToUInt32(CalculateCombatValue());
            uint defenderCV = Convert.ToUInt32(defender.CalculateCombatValue(keepLvl));

            // apply battle modifer to the army CV corresponding to the highest LV
            if (attackerLV == maxLV)
            {
                attackerCV = Convert.ToUInt32(attackerCV * battleModifier);
            }
            else
            {
                defenderCV = Convert.ToUInt32(defenderCV * battleModifier);
            }

            battleValues[0] = attackerCV;
            battleValues[1] = defenderCV;

            return battleValues;
        }

        /// <summary>
        /// Process the retreat of the army
        /// </summary>
        /// <param name="retreatDistance">The retreat distance</param>
        public void ProcessRetreat(int retreatDistance)
        {
            // get starting fief
            Fief startingFief = Location;

            // get army leader
            Character thisLeader = Leader;

            // get army owner
            PlayerCharacter thisOwner = Owner;

            // for each hex in retreatDistance, process retreat
            for (int i = 0; i < retreatDistance; i++)
            {
                // get current location
                Fief from = Location;

                // get fief to retreat to
                Fief target = GameMap.chooseRandomHex(from, true, thisOwner, startingFief);

                if (target != null)
                {
                    // get travel cost
                    double travelCost = from.getTravelCost(target);

                    // check for army leader (defender may not have had one)
                    if (thisLeader != null)
                    {
                        // ensure leader has enough days (retreats are immune to running out of days)
                        if (thisLeader.Days < travelCost)
                        {
                            thisLeader.AdjustDays(thisLeader.Days - travelCost);
                        }

                        // perform retreat
                        bool success = thisLeader.MoveCharacter(target, travelCost, false);
                    }

                    // if no leader
                    else
                    {
                        // ensure army has enough days (retreats are immune to running out of days)
                        if (Days < travelCost)
                        {
                            Days = travelCost;
                        }

                        // perform retreat
                        MoveWithoutLeader(target, travelCost);
                    }
                }
            }
        }

        /// <summary>
        /// Processes the addition of one or more detachments to the army
        /// Returns an error message on fail
        /// UPDATE: Can now leave detachments for other players. Allows to better team play
        /// </summary>
        /// <param name="detachments">The detachments to add</param>
        public bool ProcessPickups(Detachment detachment)
        {
            throw new NotImplementedException();
            /*
            ProtoMessage error = new ProtoMessage();
            error.ResponseType = DisplayMessages.Error;
            bool proceed = true;
            bool adjustDays = true;
            double daysTaken = 0;
            double minDays = 0;
            bool displayNotAllMsg = false;
            uint[] totTroopsToAdd = new uint[] { 0, 0, 0, 0, 0, 0, 0 };
            Dictionary<string, ProtoDetachment> troops = new Dictionary<string, ProtoDetachment>();
            DisplayMessages toDisplay = DisplayMessages.None;

            // set minDays to thisArmy.days (as default value)
            minDays = this.RemainingDays;

            // get leader and owner
            Character myLeader = this.Leader;
            PlayerCharacter myOwner = this.Owner;

            // check have minimum days necessary for transfer
            if (this.RemainingDays < 10)
            {
                error.ResponseType = DisplayMessages.ErrorGenericNotEnoughDays;
                return error;
                proceed = false;
                adjustDays = false;
            }
            else
            {
                // calculate time taken for transfer
                daysTaken = Globals_Game.myRand.Next(10, 31);

                // check if have enough days for transfer in this instance
                if (daysTaken > this.RemainingDays)
                {
                    error.ResponseType = DisplayMessages.ErrorGenericPoorOrganisation;
                    return error;
                    daysTaken = this.RemainingDays;
                    proceed = false;
                }
                else
                {
                    // make sure collecting army is owned by recipient or donator
                    foreach (string id in detachmentIDs)
                    {
                        if (this.Location.troopTransfers.ContainsKey(id))
                        {
                            ProtoDetachment details = this.Location.troopTransfers[id];
                            // get donating player
                            PlayerCharacter pcFrom = null;
                            if (Globals_Game.pcMasterList.ContainsKey(details.leftBy))
                            {
                                pcFrom = Globals_Game.pcMasterList[details.leftBy];
                            }
                            // get target player
                            PlayerCharacter pcFor = null;
                            if (Globals_Game.pcMasterList.ContainsKey(details.leftFor))
                            {
                                pcFor = Globals_Game.pcMasterList[details.leftFor];
                            }
                            // check for appropriate collecting player
                            if (myOwner != pcFor)
                            {
                                error.ResponseType = DisplayMessages.ArmyPickupsDenied;
                                return error;
                                proceed = false;
                                adjustDays = false;
                            }
                            else
                            {
                                troops.Add(id, details);
                                if (minDays < Convert.ToDouble(details.days))
                                {
                                    minDays = Convert.ToDouble(details.days);
                                }
                            }
                        }
                    }
                }
            }

            if (proceed)
            {
                // get fief
                Fief thisFief = this.Location;

                // check for minimum days
                foreach (KeyValuePair<string, ProtoDetachment> pair in troops)
                {
                    ProtoDetachment item = pair.Value;
                    double thisDays = Convert.ToDouble(item.days);

                    // check if detachment has enough days for transfer in this instance
                    // if not, flag display of message at end of process, but do nothing else
                    if (thisDays < daysTaken)
                    {
                        displayNotAllMsg = true;
                        toDisplay = DisplayMessages.ArmyPickupsNotEnoughDays;
                    }
                    else
                    {
                        uint[] thisTroops = item.troops;

                        Army tempArmy = new Army(Globals_Game.GetNextArmyID(), null, item.leftFor,
                                thisDays, this.Location, trp: thisTroops);
                        if (thisDays > minDays)
                        {
                            // check for attrition (to bring it down to minDays)
                            byte attritionChecks = 0;
                            attritionChecks = Convert.ToByte((thisDays - minDays) / 7);
                            double attritionModifier = 0;

                            for (int i = 0; i < attritionChecks; i++)
                            {
                                attritionModifier = tempArmy.CalcAttrition();

                                // apply attrition
                                if (attritionModifier > 0)
                                {
                                    tempArmy.ApplyTroopLosses(attritionModifier);
                                }
                            }
                        }
                        for (int i = 0; i < totTroopsToAdd.Length; i++)
                        {
                            totTroopsToAdd[i] += tempArmy.Troops[i];
                        }

                        // remove detachment from fief
                        thisFief.troopTransfers.Remove(pair.Key);

                        // nullify tempArmy
                        tempArmy = null;
                    }
                }
            }

            if (adjustDays)
            {
                if (this.RemainingDays == minDays)
                {
                    // add troops to army (this could be 0)
                    for (int i = 0; i < this.Troops.Length; i++)
                    {
                        this.Troops[i] += totTroopsToAdd[i];
                    }

                    // adjust days
                    myLeader.AdjustDays(daysTaken);

                    // calculate attrition for army
                    byte attritionChecks = Convert.ToByte(daysTaken / 7);
                    double attritionModifier = 0;

                    for (int i = 0; i < attritionChecks; i++)
                    {
                        attritionModifier = this.CalcAttrition();
                        if (attritionModifier > 0)
                        {
                            this.ApplyTroopLosses(attritionModifier);
                        }
                    }
                }
                else
                {
                    // any days army has had to 'wait' should go towards days taken
                    // for the transfer (daysTaken)
                    double differenceToMin = (this.RemainingDays - minDays);
                    if (differenceToMin >= daysTaken)
                    {
                        daysTaken = 0;
                    }
                    else
                    {
                        daysTaken = daysTaken - differenceToMin;
                    }

                    // adjust days
                    myLeader.AdjustDays(differenceToMin);

                    // calculate attrition for army (to bring it down to minDays)
                    byte attritionChecks = Convert.ToByte(differenceToMin / 7);
                    double attritionModifier = 0;

                    for (int i = 0; i < attritionChecks; i++)
                    {
                        attritionModifier = this.CalcAttrition();
                        if (attritionModifier > 0)
                        {
                            this.ApplyTroopLosses(attritionModifier);
                        }
                    }

                    // add troops to army
                    for (int i = 0; i < this.Troops.Length; i++)
                    {
                        this.Troops[i] += totTroopsToAdd[i];
                    }

                    // check if are any remaining days taken for the transfer (daysTaken) 
                    if (daysTaken > 0)
                    {
                        // adjust days
                        myLeader.AdjustDays(daysTaken);

                        // calculate attrition for army for days taken for transfer
                        attritionChecks = Convert.ToByte(daysTaken / 7);

                        for (int i = 0; i < attritionChecks; i++)
                        {
                            attritionModifier = this.CalcAttrition();
                            if (attritionModifier > 0)
                            {
                                this.ApplyTroopLosses(attritionModifier);
                            }
                        }
                    }
                }

                // if not all selected detachments could be picked up (not enough days), show message
                if (displayNotAllMsg)
                {
                    error.ResponseType = toDisplay;
                    return error;
                }
            }
            return null;
            */
        }
    }

    /// <summary>
    /// Class for sending details of a detachment
    /// Character ID of PlayerCharacter leaving detachment is obtained via connection details
    /// </summary>
    public class ArmyDetachment
    {
        // ID of detachment
        public string ID { get; set; }
        /// <summary>
        /// Array of troops (size = 7)
        /// </summary>
        public uint[] Troops;
        /// <summary>
        /// Character detachment is left for
        /// </summary>
        public Character LeftFor { get; set; }
        /// <summary>
        /// ArmyID of army from which detachment was created
        /// </summary>
        public Army OriginatingArmy { get; set; }
        /// <summary>
        /// Details of person who left this detachment (used in sending details of detachments to client)
        /// </summary>
        public Character LeftBy { get; set; }
        /// <summary>
        /// Days left of person who created detachment at time of creation
        /// </summary>
        public int Days { get; set; }

        public ArmyDetachment(string ID, uint[] Troops, Character LeftFor, Army OriginatingArmy, Character LeftBy)
        {
            this.ID = ID;
            this.Troops = Troops;
            this.LeftFor = LeftFor;
            this.OriginatingArmy = OriginatingArmy;
            this.LeftBy = LeftBy;
        }
    }    

}
