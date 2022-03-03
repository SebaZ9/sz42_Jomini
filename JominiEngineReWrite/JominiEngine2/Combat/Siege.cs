using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JominiGame
{
    public class Siege
    {

        public string ID { get; set; }
        /// <summary>
        /// Holds year the siege started
        /// </summary>
        public uint StartYear { get; set; }
        /// <summary>
        /// Holds season the siege started
        /// </summary>
        public byte StartSeason { get; set; }
        /// <summary>
        /// Holds besieging player (charID)
        /// </summary>
        public PlayerCharacter BesiegingPlayer { get; set; }
        /// <summary>
        /// Holds defending player (charID)
        /// </summary>
        public PlayerCharacter DefendingPlayer { get; set; }
        /// <summary>
        /// Holds besieging army (armyID)
        /// </summary>
        public Army BesiegerArmy { get; set; }
        /// <summary>
        /// Holds defending garrison (armyID)
        /// </summary>
        public Army DefenderGarrison { get; set; }
        /// <summary>
        /// Holds fief being besieged (fiefID)
        /// </summary>
        public Fief BesiegedFief { get; set; }
        /// <summary>
        /// Holds days left in current season
        /// </summary>
        public double Days { get; set; }
        /// <summary>
        /// Holds the keep level at the start of the siege
        /// </summary>
        public double StartKeepLevel { get; set; }
        /// <summary>
        /// Holds total casualties suffered so far by attacker
        /// </summary>
        public int TotalCasualtiesAttacker { get; set; }
        /// <summary>
        /// Holds total casualties suffered so far by defender
        /// </summary>
        public int TotalCasualtiesDefender { get; set; }
        /// <summary>
        /// Holds total duration of siege so far (days)
        /// </summary>
        public double TotalDays { get; set; }
        /// <summary>
        /// Holds additional defending army (armyID)
        /// </summary>
        public Army DefenderAdditional { get; set; }
        /// <summary>
        /// Holds season and year the siege ended
        /// </summary>
        public string EndDate { get; set; }

        /// <summary>
        /// Constructor for Siege
        /// </summary>
        /// <param name="startYr">uint holding year the siege started</param>
        /// <param name="startSeas">byte holding season the siege started</param>
        /// <param name="bsgPlayer">String holding besieging player (charID)</param>
        /// <param name="defPlayer">String holding defending player (charID)</param>
        /// <param name="bsgArmy">String holding besieging army (armyID)</param>
        /// <param name="defGarr">String holding defending garrison (armyID)</param>
        /// <param name="fief">String holding fief being besieged (fiefID)</param>
        /// <param name="day">double containing days left in current season</param>
        /// <param name="kpLvl">double containing the keep level at the start of the siege</param>
        /// <param name="totAtt">int containing total attacker casualties so far</param>
        /// <param name="totDef">int containing total defender casualties so far</param>
        /// <param name="totday">double containing days used by siege so far</param>
        /// <param name="defAdd">String holding additional defending army (armyID)</param>
        /// <param name="end">string holding season and year the siege ended</param>
        public Siege(uint StartYear, byte StartSeason, PlayerCharacter BesiegingPlayer, PlayerCharacter DefendingPlayer, Army BesiegerArmy,
            Army DefenderGarrison, Fief BesiegedFief, double Days, double StartKeepLevel, int TotalCasualtiesAttacker = 0,
            int TotalCasualtiesDefender = 0, double TotalDays = 1, Army DefenderAdditional = null, string EndDate = null)
        {            
            this.StartYear = StartYear;
            this.StartSeason = StartSeason;
            this.BesiegingPlayer = BesiegingPlayer;
            this.DefendingPlayer = DefendingPlayer;
            this.BesiegerArmy = BesiegerArmy;
            this.DefenderGarrison = DefenderGarrison;
            this.BesiegedFief = BesiegedFief;
            this.Days = Days;
            this.StartKeepLevel = StartKeepLevel;
            this.TotalCasualtiesAttacker = TotalCasualtiesAttacker;
            this.TotalCasualtiesDefender = TotalCasualtiesDefender;
            this.TotalDays = TotalDays;
            this.DefenderAdditional = DefenderAdditional;
            if (string.IsNullOrWhiteSpace(EndDate))
            {
                this.EndDate = null;
            }
            else
            {
                this.EndDate = EndDate;
            }
        }

        /// <summary>
        /// Gets the fief being besieged
        /// </summary>
        /// <returns>The besieged fief</returns>
        public Fief GetFief()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the besieging army
        /// </summary>
        /// <returns>The besieging army</returns>
        public Army GetBesiegingArmy()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the defending garrison
        /// </summary>
        /// <returns>The defending garrison (Army)</returns>
        public Army GetDefenderGarrison()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the additional defending army
        /// </summary>
        /// <returns>The additional defending army</returns>
        public Army GetDefenderAdditional()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the defending player
        /// </summary>
        /// <returns>The defending player</returns>
        public PlayerCharacter GetDefendingPlayer()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the besieging player
        /// </summary>
        /// <returns>The besieging player</returns>
        public PlayerCharacter GetBesiegingPlayer()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Synchronises days for component objects
        /// </summary>
        /// <param name="newDays">double indicating new value for days</param>
        /// <param name="checkForAttrition">bool indicating whether to check for attrition</param>
        public void SyncSiegeDays(double newDays, bool checkForAttrition = true)
        {
            Army besieger = GetBesiegingArmy();
            Army defenderGarr = GetDefenderGarrison();
            Army defenderAdd = GetDefenderAdditional();
            bool defenderAttritonApplies = false;
            byte attritionChecks = 0;

            // check to see if attrition checks are required
            // NOTE: no check required for seasonal update
            if (checkForAttrition)
            {
                // if the siege has had to 'wait' for some days
                if (Days > newDays)
                {
                    // get number of days difference
                    double difference = Days - newDays;

                    // work out number of attrition checks needed
                    attritionChecks = Convert.ToByte(difference / 7);

                    // check if attrition has kicked in for defending forces
                    defenderAttritonApplies = CheckAttritionApplies();
                }

            }

            // adjust siege days to specified days
            Days = newDays;

            // ATTACKING ARMY
            Character attackerLeader = besieger.Leader;
            if (attackerLeader != null)
            {
                attackerLeader.AdjustDays(attackerLeader.Days - Days);
            }
            else
            {
                besieger.Days = Days;
            }

            // check for attrition if required
            if (attritionChecks > 0)
            {
                uint totalAttackTroopsLost = 0;
                for (int i = 0; i < attritionChecks; i++)
                {
                    // calculate attrition
                    double attritionModifer = besieger.CalcAttrition();
                    // apply attrition
                    totalAttackTroopsLost += besieger.ApplyTroopLosses(attritionModifer);
                }
                // update total attacker siege losses
                TotalCasualtiesAttacker += Convert.ToInt32(totalAttackTroopsLost);
            }

            // DEFENDING GARRISON
            Character garrisonLeader = defenderGarr.Leader;
            if (garrisonLeader != null)
            {
                garrisonLeader.AdjustDays(garrisonLeader.Days - Days);
            }
            else
            {
                defenderGarr.Days = Days;
            }

            // check for attrition if required
            if (defenderAttritonApplies)
            {
                if (attritionChecks > 0)
                {
                    uint totalDefendTroopsLost = 0;
                    for (int i = 0; i < attritionChecks; i++)
                    {
                        // calculate attrition
                        double attritionModifer = defenderGarr.CalcAttrition();
                        // apply attrition
                        totalDefendTroopsLost += defenderGarr.ApplyTroopLosses(attritionModifer);
                    }
                    // update total defender siege losses
                    TotalCasualtiesDefender += Convert.ToInt32(totalDefendTroopsLost);
                }
            }

            // ADDITIONAL DEFENDING ARMY
            if (defenderAdd != null)
            {
                Character defAddLeader = defenderAdd.Leader;
                if (defAddLeader != null)
                {
                    defAddLeader.AdjustDays(defAddLeader.Days - Days);
                }
                else
                {
                    defenderAdd.Days = Days;
                }

                // check for attrition if required
                if (defenderAttritonApplies)
                {
                    if (attritionChecks > 0)
                    {
                        uint totalDefendTroopsLost = 0;
                        for (int i = 0; i < attritionChecks; i++)
                        {
                            // calculate attrition
                            double attritionModifer = defenderAdd.CalcAttrition();
                            // apply attrition
                            totalDefendTroopsLost += defenderAdd.ApplyTroopLosses(attritionModifer);
                        }
                        // update total defender siege losses
                        TotalCasualtiesDefender += Convert.ToInt32(totalDefendTroopsLost);
                    }
                }
            }
        }


        /// <summary>
        /// Checks to see if attrition applies to the defending forces (based on bailiff management rating)
        /// </summary>
        /// <returns>bool indicating whether attrition applies</returns>
        public bool CheckAttritionApplies()
        {
            bool attritionApplies = false;
            Character thisBailiff = GetFief().Bailiff;
            double bailiffManagement = 0;

            // get bailiff's management rating
            if (thisBailiff != null)
            {
                bailiffManagement = thisBailiff.Management;
            }
            else
            {
                bailiffManagement = 4;
            }

            // check to see if attrition needs to be applied
            if ((TotalDays / 60) > bailiffManagement)
            {
                attritionApplies = true;
            }

            return attritionApplies;
        }

        /// <summary>
        /// Updates siege at the end/beginning of the season
        /// </summary>
        /// <returns>bool indicating whether the siege has been dismantled</returns>
        public bool UpdateSiege()
        {
            bool siegeEnded = false;
            Character besiegerLeader = null;
            PlayerCharacter besiegerOwner = GetBesiegingPlayer();

            // check if besieger still in field (i.e. has not been disbanded)
            if (BesiegerArmy == null)
            {
                siegeEnded = true;
            }

            // check besieging player still alive
            else if ((besiegerOwner == null) || (!besiegerOwner.IsAlive))
            {
                siegeEnded = true;
            }

            else
            {
                // DAYS
                // base allowance
                double newDays = 90;

                // get besieging leader
                besiegerLeader = GetBesiegingArmy().Leader;
                if (besiegerLeader != null)
                {
                    // set days to besieger leader's days (may be effected by traits)
                    newDays = besiegerLeader.Days;
                }

                // synchronise days of all component objects
                SyncSiegeDays(newDays, false);
            }

            return siegeEnded;
        }

        /// <summary>
        /// Ends the siege
        /// </summary>
        /// <param name="siegeSuccessful">bool indicating whether the siege was successful</param>
        /// <param name="s">String containing circumstances under which the siege ended</param>
        public void SiegeEnd(bool siegeSuccessful, string[] fields = null)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Implements conditional checks prior to a siege operation
        /// </summary>
        /// <returns>bool indicating whether siege operation can proceed</returns>
        /// <param name="operation">The operation - round or end</param>
        public bool ChecksBeforeSiegeOperation(string operation = "round")
        {
            //error = null;
            bool proceed = true;
            int daysRequired = 0;

            if (operation.Equals("round"))
            {
                daysRequired = 10;
            }
            else if (operation.Equals("end"))
            {
                daysRequired = 1;
            }

            // check has min days required
            if (Days < daysRequired)
            {
                proceed = false;
                //error = new ProtoMessage();
                //error.ResponseType = DisplayMessages.SiegeErrorDays;
            }

            return proceed;
        }


        /// <summary>
        /// Processes the storming of the keep by attacking forces in the siege
        /// </summary>
        /// <param name="defenderCasualties">Defender casualties sustained during the reduction phase</param>
        /// <param name="originalKeepLvl">Keep level prior to the reduction phase</param>
        public void SiegeStormRound(uint defenderCasualties, double originalKeepLvl)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Processes a single negotiation round of the siege
        /// </summary>
        /// <returns>bool indicating whether negotiation was successful</returns>
        /// <param name="defenderCasualties">Defender casualties sustained during the reduction phase</param>
        /// <param name="originalKeepLvl">Keep level prior to the reduction phase</param>
        public bool SiegeNegotiationRound(uint defenderCasualties, double originalKeepLvl)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Processes a single reduction round of the siege
        /// </summary>
        /// <param name="type">The type of round - storm, negotiate, reduction (default)</param>
        public void SiegeReductionRound(string type = "reduction")
        {
            throw new NotImplementedException();
        }


    }
}
