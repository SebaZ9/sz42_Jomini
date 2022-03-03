using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JominiGame
{
    /// <summary>
    /// Class storing data on PlayerCharacter
    /// </summary>
    public class PlayerCharacter : Character
    {
        /// <summary>
        /// Holds ID of player who is currently playing this PlayerCharacter
        /// </summary>
        public string PlayerID { get; set; }
        /// <summary>
        /// Holds character outlawed status
        /// </summary>
        public bool Outlawed { get; set; }
        /// <summary>
        /// Holds character's treasury
        /// </summary>
        public uint Purse { get; set; }
        /// <summary>
        /// Holds character's employees and family (NonPlayerCharacter objects)
        /// </summary>
        public List<NonPlayerCharacter> MyNPCs = new List<NonPlayerCharacter>();
        /// <summary>
        /// Holds character's owned fiefs
        /// </summary>
        public List<Fief> OwnedFiefs = new List<Fief>();
        /// <summary>
        /// Holds character's owned provinces
        /// </summary>
        public List<Province> OwnedProvinces = new List<Province>();
        /// <summary>
        /// Holds character's home fief (fiefID)
        /// </summary>
        public Fief HomeFief { get; set; }
        /// <summary>
        /// Holds character's ancestral home fief (fiefID)
        /// </summary>
        public Fief AncestralHomeFief { get; set; }
        /// <summary>
        /// Holds character's armies (Army objects)
        /// </summary>
        public List<Army> MyArmies = new List<Army>();
        /// <summary>
        /// Holds character's sieges (siegeIDs)
        /// </summary>
        public List<Siege> MySieges = new List<Siege>();
        /// <summary>
        /// Holds Characters in entourage
        /// </summary>
        public List<Character> MyEntourage = new List<Character>();
        /// <summary>
        /// Dictionary holding active proposals from family members to other NPCs. Each family member can only propose to one person at a time
        /// </summary>
        public Dictionary<string, string> ActiveProposals = new Dictionary<string, string>();
        /// <summary>
        /// Holds a list of all characters that have been taken captive (during battle, siege, kidnapping, failed spy attempts etc)
        /// </summary>
        public List<Character> MyCaptives = new List<Character>();


        /// <summary>
        /// Constructor for PlayerCharacter
        /// </summary>
        /// <param name="Outlawed">bool holding character outlawed status</param>
        /// <param name="Purse">uint holding character purse</param>
        /// <param name="MyNPCs">List(NonPlayerCharacter) holding employees and family of character</param>
        /// <param name="OwnedFiefs">List(Fief) holding fiefs owned by character</param>
        /// <param name="OwnedProvinces">List(Province) holding provinces owned by character</param>
        /// <param name="HomeFief">string holding character's home fief (fiefID)</param>
        /// <param name="anchome">string holding character's ancestral home fief (fiefID)</param>
        /// <param name="PlayerID">string holding ID of player who is currently playing this PlayerCharacter</param>
        /// <param name="MyArmies">List(Army) holding character's armies</param>
        /// <param name="MySieges">List(string) holding character's sieges (siegeIDs)</param>
        public PlayerCharacter(string ID, string FirstName, string FamilyName, Tuple<uint, byte> BirthDate, bool IsMale, Nationality Nationality,
            bool IsAlive, double MaxHealth, double Virtility, Queue<Fief> GoTo, Language Language, double Days, double Stature, double Managment,
            double Combat, Tuple<Trait, int>[] Traits, bool InKeep, bool IsPregnant, string FamilyID, string FamiltyID, Character Spouse, Character Father, Character Mother,
            bool Outlawed, uint Purse, List<NonPlayerCharacter> MyNPCs, List<Fief> OwnedFiefs, List<Province> OwnedProvinces,
            Fief HomeFief, Fief AncestralHomeFief, List<Place> MyTitles, List<Army> MyArmies, List<Siege> MySieges,
            Character Fiancee, GameClock Clock, IdGenerator IDGen, HexMapGraph GameMap, Dictionary<string, Ailment> Ailements = null, Fief Location = null, Army ArmyID = null, string PlayerID = null)
            : base(ID, FirstName, FamilyName, BirthDate, IsMale, Nationality, IsAlive, MaxHealth, Virtility, GoTo, Language, Days,
                  Stature, Managment, Combat, Traits, InKeep, IsPregnant, FamilyID, Spouse, Father, Mother, MyTitles, Fiancee, Clock, IDGen, GameMap, Ailements, Location, ArmyID)
        {            
            this.Outlawed = Outlawed;
            this.Purse = Purse;
            this.MyNPCs = MyNPCs;
            this.OwnedFiefs = OwnedFiefs;
            this.OwnedProvinces = OwnedProvinces;
            this.HomeFief = HomeFief;
            this.AncestralHomeFief = AncestralHomeFief;
            this.PlayerID = PlayerID;
            this.MyArmies = MyArmies;
            this.MySieges = MySieges;
        }

        /// <summary>
        /// Constructor for PlayerCharacter using NonPlayerCharacter object and a PlayerCharacter object,
        /// for use when promoting a deceased PC's heir
        /// </summary>
        /// <param name="npc">NonPlayerCharacter object to use as source</param>
		/// <param name="pc">PlayerCharacter object to use as source</param>
        public PlayerCharacter(NonPlayerCharacter npc, PlayerCharacter pc, GameClock Clock, IdGenerator IDGen, HexMapGraph GameMap)
            : base(npc, "promote",Clock, IDGen, GameMap, pc.MyTitles)
        {
            Outlawed = false;
            Purse = pc.Purse;
            MyNPCs = pc.MyNPCs;
            OwnedFiefs = pc.OwnedFiefs;
            for (int i = 0; i < OwnedFiefs.Count; i++)
            {
                OwnedFiefs[i].Owner = this;
            }
            OwnedProvinces = pc.OwnedProvinces;
            for (int i = 0; i < OwnedProvinces.Count; i++)
            {
                OwnedProvinces[i].Owner = this;
            }
            HomeFief = pc.HomeFief;
            AncestralHomeFief = pc.AncestralHomeFief;
            PlayerID = pc.PlayerID;
            MyArmies = pc.MyArmies;
            MySieges = pc.MySieges;
        }

        public PlayerCharacter(GameClock Clock, IdGenerator IDGen, HexMapGraph GameMap) : base (Clock, IDGen, GameMap)
        {

        }

        /// <summary>
        /// Identifies and returns the PlayerCharacter's heir
        /// </summary>
        /// <returns>The heir (NonPlayerCharacter)</returns>
        public NonPlayerCharacter GetHeir()
        {
            NonPlayerCharacter heir = null;
            List<NonPlayerCharacter> sons = new List<NonPlayerCharacter>();
            List<NonPlayerCharacter> brothers = new List<NonPlayerCharacter>();

            foreach (NonPlayerCharacter npc in this.MyNPCs)
            {
                // check for assigned heir
                if (npc.IsHeir)
                {
                    heir = npc;
                    break;
                }

                // take note of sons
                else if (npc.GetFunction(this).Equals("Son"))
                {
                    sons.Add(npc);
                }

                // take note of brothers
                else if (npc.GetFunction(this).Equals("Brother"))
                {
                    brothers.Add(npc);
                }
            }

            // if no assigned heir identified
            if (heir == null)
            {
                int age = 0;

                // if there are some sons
                if (sons.Count > 0)
                {
                    foreach (NonPlayerCharacter son in sons)
                    {
                        // if son is older, assign as heir
                        if (Age > age)
                        {
                            heir = son;
                            age = son.Age;
                        }
                    }
                }

                // if there are some brothers
                else if (brothers.Count > 0)
                {
                    foreach (NonPlayerCharacter brother in brothers)
                    {
                        // if brother is older, assign as heir
                        if (brother.Age > age)
                        {
                            heir = brother;
                            age = brother.Age;
                        }
                    }
                }
            }

            // make sure heir is properly identified
            if (heir != null)
            {
                if (!heir.IsHeir)
                {
                    heir.IsHeir = true;
                }
            }

            return heir;
        }

        public void AdjustStatureModifier(double amount)
        {
            throw new NotImplementedException();
        }

     
        /// <summary>
        /// Returns the siege object associated with the specified siegeID
        /// </summary>
        /// <returns>The siege object</returns>
        /// <param name="id">The siegeID of the siege</param>
        public Siege? GetSiege(string id)
        {
            foreach (Siege siege in MySieges)
            {
                if (siege.ID == id)
                    return siege;
            }
            return null;
        }

        /// <summary>
        /// Returns the current total GDP for all fiefs owned by the PlayerCharacter
        /// </summary>
        /// <returns>The current total GDP</returns>
        public int GetTotalGDP()
        {
            int totalGDP = 0;

            foreach (Fief thisFief in OwnedFiefs)
            {
                totalGDP += Convert.ToInt32(thisFief.KeyStatsCurrent[1]);
            }

            return totalGDP;
        }

        /// <summary>
        /// Finds the highest ranking fief(s) in the PlayerCharacter's owned fiefs
        /// </summary>
        /// <returns>A list containing the highest ranking fief(s)</returns>
        public List<Fief> GetHighestRankingFief()
        {
            List<Fief> highestFiefs = new List<Fief>();
            int highestRank = 0;

            foreach (Fief thisFief in this.OwnedFiefs)
            {
                if (thisFief.PlaceRank.ID > highestRank)
                {
                    // clear existing fiefs
                    if (highestFiefs.Count > 0)
                    {
                        highestFiefs.Clear();
                    }

                    // add fief to list
                    highestFiefs.Add(thisFief);

                    // update highest rank
                    highestRank = thisFief.PlaceRank.ID;
                }
            }

            return highestFiefs;
        }

        /// <summary>
        /// Processes an offer for employment
        /// </summary>
        /// <returns>bool indicating acceptance of offer</returns>
        /// <param name="npc">NPC receiving offer</param>
        /// <param name="offer">Proposed wage</param>
        public bool ProcessEmployOffer(NonPlayerCharacter npc, uint offer)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Hire an NPC
        /// </summary>
        /// <param name="npc">NPC to hire</param>
        /// <param name="wage">NPC's wage</param>
        public void HireNPC(NonPlayerCharacter npc, uint wage)
        {
            // if was in employ of another PC, fire from that position
            if (npc.Employer != null)
            {
                if (!npc.Employer.Equals(this))
                {
                    // get previous employer
                    PlayerCharacter oldBoss = (PlayerCharacter)npc.Employer;

                    if (oldBoss != null)
                    {
                        oldBoss.FireNPC(npc);
                    }
                }
            }

            // add to employee list
            MyNPCs.Add(npc);

            // set NPC wage
            npc.Salary = wage;

            // set this PC as NPC's boss
            npc.Employer = this;

            // remove any offers by this PC from NPCs lastOffer list
            npc.LastOffer.Clear();
        }

        //TODO send success to client
        /// <summary>
        /// Fire an NPC
        /// </summary>
        /// <param name="npc">NPC to fire</param>
        public void FireNPC(NonPlayerCharacter npc)
        {
            // remove from bailiff duties
            List<Fief> fiefsBailiff = npc.GetFiefsBailiff();
            if (fiefsBailiff.Count > 0)
            {
                for (int i = 0; i < fiefsBailiff.Count; i++)
                {
                    fiefsBailiff[i].Bailiff = null;
                }
            }

            // remove from army duties
            List<Army> armiesLeader = npc.GetArmiesLeader();
            if (armiesLeader.Count > 0)
            {
                for (int i = 0; i < armiesLeader.Count; i++)
                {
                    armiesLeader[i].Leader = null;
                }
                npc.ArmyID = null;
            }

            // take back titles, if appropriate
            if (npc.MyTitles.Count > 0)
            {
                foreach (Place thisTitle in npc.MyTitles)
                {
                    if (thisTitle is Fief)
                    {
                        Fief titleFief = (Fief)thisTitle;

                        if (titleFief != null)
                        {
                            if (titleFief.Owner == this)
                            {
                                // fief titleHolder
                                titleFief.TitleHolder = this;
                                // add to PC myTitles
                                MyTitles.Add(thisTitle);
                                npc.MyTitles.Remove(titleFief);
                            }
                        }
                    }
                    
                }
            }

            // remove from employee list
            MyNPCs.Remove(npc);

            // set NPC wage to 0
            npc.Salary = 0;

            // remove this PC as NPC's boss
            npc.Employer = null;

            // remove NPC from entourage
            RemoveFromEntourage(npc);

            // if NPC has entries in goTo, clear
            if (npc.GoTo.Count > 0)
            {
                npc.GoTo.Clear();
            }
        }

        /// <summary>
        /// Adds an NPC to the character's entourage
        /// </summary>
        /// <param name="npc">NPC to be added</param>
        public void AddToEntourage(NonPlayerCharacter npc)
        {
            // if NPC has entries in goTo, clear
            if (npc.GoTo.Count > 0)
            {
                npc.GoTo.Clear();
            }
            lock (EntourageLock)
            {
                // keep track of original days value for PC
                double myDays = this.Days;

                // ensure days are synchronised
                double minDays = Math.Min(this.Days, npc.Days);
                Days = minDays;
                npc.Days = minDays;

                // add to entourage
                npc.setEntourage(true);
                MyEntourage.Add(npc);
                // ensure days of entourage are synched with PC
                if (Days != myDays)
                {
                    AdjustDays(0);
                }
            }

        }

        /// <summary>
        /// Removes an NPC from the character's entourage
        /// </summary>
        /// <param name="npc">NPC to be removed</param>
        public void RemoveFromEntourage(NonPlayerCharacter npc)
        {
            lock (EntourageLock)
            {
                //remove from entourage
                npc.setEntourage(false);
                MyEntourage.Remove(npc);
            }
        }

        /// <summary>
        /// Adds a Fief to the character's list of owned fiefs
        /// </summary>
        /// <param name="f">Fief to be added</param>
        public void AddToOwnedFiefs(Fief f)
        {
            // add fief
            OwnedFiefs.Add(f);
        }

        /// <summary>
        /// Adds a Province to the character's list of owned provinces
        /// </summary>
        /// <param name="p">Province to be added</param>
        public void AddToOwnedProvinces(Province p)
        {
            // add fief
            OwnedProvinces.Add(p);
        }

        /// <summary>
        /// Removes a Fief from the character's list of owned fiefs
        /// </summary>
        /// <param name="f">Fief to be removed</param>
        public void RemoveFromOwnedFiefs(Fief f)
        {
            // remove fief
            OwnedFiefs.Remove(f);
        }

        /// <summary>
        /// Extends base method allowing PlayerCharacter to enter keep (if not barred).
        /// Then moves entourage (if not individually barred). Ignores nationality bar
        /// for entourage if PlayerCharacter allowed to enter
        /// </summary>
        /// <returns>bool indicating success</returns>
        public override bool EnterKeep()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Extends base method allowing PlayerCharacter to exit keep. Then exits entourage.
        /// </summary>
        public override bool ExitKeep()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Extends base method allowing PlayerCharacter to synchronise the days of their entourage
        /// </summary>
        /// <param name="daysToSubtract">Number of days to subtract</param>
        public override void AdjustDays(double daysToSubtract)
        {
            // use base method to subtract days from PlayerCharacter
            base.AdjustDays(daysToSubtract);

            // iterate through employees
            for (int i = 0; i < MyNPCs.Count; i++)
            {
                // if employee in entourage, set NPC days to same as player
                if (MyNPCs[i].InEntourage)
                {
                    MyNPCs[i].Days = Days;
                }
            }

        }

        /// <summary>
        /// Extends base method allowing PlayerCharacter to target fief. Then moves entourage.
        /// </summary>
        /// <returns>bool indicating success</returns>
        /// <param name="target">Target fief</param>
        /// <param name="cost">Travel cost (days)</param>
        /// <param name="siegeCheck">bool indicating whether to check whether the move would end a siege</param>
        public bool MoveCharacter(Fief target, double cost, bool siegeCheck = true)
        {
            throw new NotImplementedException();

        }

        /// <summary>
        /// Moves an NPC in a player's entourage (i.e. sets new location)
        /// </summary>
        /// <param name="target">Target fief</param>
        /// <param name="npc">NonPlayerCharacter to move</param>
        public void MoveEntourageNPC(Fief target, NonPlayerCharacter npc)
        {
            // remove character from current fief's character list
            npc.Location.RemoveCharacter(npc);
            // set location to target fief
            npc.Location = target;
            // add character to target fief's character list
            npc.Location.AddCharacter(npc);
            // arrives outside keep
            npc.InKeep = false;
        }

        /// <summary>
        /// Carries out conditional checks prior to recruitment
        /// </summary>
        /// <returns>bool indicating whether recruitment can proceed</returns>
        public bool ChecksBeforeRecruitment()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Recruits troops from the current fief
        /// </summary>
        /// <returns>uint containing number of troops recruited</returns>
        /// <param name="number">How many troops to recruit</param>
        /// <param name="thisArmy">Army to recruit into- null to create new army</param>
        /// <param name="isConfirm">Whether or not this action has been confirmed by client</param>
        public bool RecruitTroops(uint number, Army? thisArmy, bool isConfirm)
        {
            throw new NotImplementedException();
            bool armyExists = (thisArmy != null);
            // used to record outcome of various checks
            bool proceed = true;

            int troopsRecruited = 0;
            int revisedRecruited = 0;
            int indivTroopCost = 0;
            int troopCost = 0;
            int daysUsed = 0;

            throw new NotImplementedException();

            // get home fief
            Fief homeFief = null;// GetHomeFief();

            // calculate cost of individual soldier
            if (Location.AncestralOwner == this)
            {
                indivTroopCost = 500;
            }
            else
            {
                indivTroopCost = 2000;
            }


            // various checks to see whether to proceed
            proceed = ChecksBeforeRecruitment();

            // if have not passed all of checks above, return
            if (!proceed)
            {
                return false;
            }

            // actual days taken
            // see how long recuitment attempt will take: generate random int (1-5)
            daysUsed = Random.Shared.Next(1, 6);

            if (Days < daysUsed)
            {
                proceed = false;
            }

            //TODO move troop calculatioms + confirmation to client
            if (proceed)
            {
                // calculate potential cost
                troopCost = Convert.ToInt32(number) * indivTroopCost;
                //TODO client confirm revised troop numbers
                // check to see if can afford the specified number of troops
                // if can't afford specified number
                if (!(homeFief.GetAvailableTreasury() >= troopCost))
                {
                    // work out how many troops can afford
                    double roughNumber = homeFief.GetAvailableTreasury() / indivTroopCost;
                    revisedRecruited = Convert.ToInt32(Math.Floor(roughNumber));
                }

                if (proceed)
                {
                    // calculate number of troops responding to call (based on fief population)
                    troopsRecruited = Location.CallUpTroops(minProportion: 0.4);

                    // adjust if necessary
                    if (troopsRecruited >= number)
                    {
                        troopsRecruited = Convert.ToInt32(number);

                        // calculate total cost
                        troopCost = troopsRecruited * indivTroopCost;

                    }
                    // if less than specified number respond to call
                    else
                    {
                        // calculate total cost
                        troopCost = troopsRecruited * indivTroopCost;
                    }
                    if (!isConfirm)
                    {
                    }
                    // if no existing army, create one
                    if (!armyExists)
                    {
                        // if necessary, exit keep (new armies are created outside keep)
                        if (InKeep)
                        {
                            ExitKeep();
                        }

                        //thisArmy = new Army(Globals_Game.GetNextArmyID(), null, this.charID, this.Days, this.Location.id);
                        //thisArmy.AddArmy();
                    }

                    // deduct cost of troops from treasury
                    homeFief.AdjustTreasury(-troopCost);

                    // get army nationality
                    string thisNationality = Nationality.NatID;

                    // work out how many of each type recruited
                    uint[] typesRecruited = new uint[] { 0, 0, 0, 0, 0, 0, 0 };
                    uint totalSoFar = 0;
                    for (int i = 0; i < typesRecruited.Length; i++)
                    {
                        // work out 'trained' troops numbers
                        if (i < typesRecruited.Length - 1)
                        {
                            //typesRecruited[i] = Convert.ToUInt32(troopsRecruited * Globals_Server.recruitRatios[thisNationality][i]);
                            totalSoFar += typesRecruited[i];
                        }
                        // fill up with rabble
                        else
                        {
                            typesRecruited[i] = Convert.ToUInt32(troopsRecruited) - totalSoFar;
                        }
                    }
                    for (int i = 0; i < thisArmy.Troops.Length; i++)
                    {
                        thisArmy.Troops[i] += typesRecruited[i];
                    }

                    // indicate recruitment has occurred in this fief
                    Location.HasRecruited = true;
                }
            }

            // update character's days
            AdjustDays(daysUsed);
            return true;
        }


        /// <summary>
        /// Transfers the specified title to the specified character
        /// </summary>
        /// <param name="newTitleHolder">The new title holder</param>
        /// <param name="titlePlace">The place to which the title refers</param>
        public void TransferTitle(Character newTitleHolder, Place titlePlace)
        {
            Character oldTitleHolder = titlePlace.Owner;

            // remove title from existing holder
            if (oldTitleHolder != null)
            {
                oldTitleHolder.MyTitles.Remove(titlePlace);
            }

            // add title to new holder
            newTitleHolder.MyTitles.Add(titlePlace);
            titlePlace.TitleHolder = newTitleHolder;

            // CREATE JOURNAL ENTRY
            // get interested parties
            bool success = true;
            PlayerCharacter placeOwner = titlePlace.Owner;

            // ID
            //uint entryID = Globals_Game.GetNextJournalEntryID();
            Console.WriteLine("!!!--- FIX JOURNAL ---!!!");

            // date
            //uint year = Globals_Game.clock.currentYear;
           // byte season = Globals_Game.clock.currentSeason;

            // personae
            //List<string> tempPersonae = new List<string>();
            //tempPersonae.Add(placeOwner.charID + "|placeOwner");
            //tempPersonae.Add(newTitleHolder.charID + "|newTitleHolder");
            //if ((oldTitleHolder != null) && (oldTitleHolder != placeOwner))
            //{
            //    tempPersonae.Add(oldTitleHolder.charID + "|oldTitleHolder");
            //}
            //if (titlePlace is Province)
            //{
            //    tempPersonae.Add("all|all");
            //}
            //string[] thisPersonae = tempPersonae.ToArray();
            //
            //// type
            //string type = "";
            //if (titlePlace is Fief)
            //{
            //    type += "grantTitleFief";
            //}
            //else if (titlePlace is Province)
            //{
            //    type += "grantTitleProvince";
            //}
            //
            //// location
            //string location = titlePlace.id;
            //
            //// description
            //string[] fields = new string[4];
            //if (titlePlace is Fief)
            //{
            //    fields[0] = "fief";
            //}
            //else if (titlePlace is Province)
            //{
            //    fields[0] = "province";
            //}
            //fields[1] = titlePlace.name;
            //if ((newTitleHolder == placeOwner) && (oldTitleHolder != null))
            //{
            //    fields[3] = ".";
            //    fields[2] = "removed by His Royal Highness " + this.FirstName + " " + this.FamilyName + " from the previous holder " + oldTitleHolder.FirstName + " " + oldTitleHolder.FamilyName;
            //}
            //else
            //{
            //    fields[2] = "granted by its owner " + placeOwner.FirstName + " " + placeOwner.FamilyName + " to " + newTitleHolder.FirstName + " " + newTitleHolder.FamilyName;
            //    if ((oldTitleHolder != null) && (oldTitleHolder != placeOwner))
            //    {
            //        fields[3] = "; This has necessitated the removal of " + oldTitleHolder.FirstName + " " + oldTitleHolder.FamilyName + " from the title";
            //    }
            //    else
            //    {
            //        fields[3] = ".";
            //    }
            //}
            //
            //// create and add a journal entry to the pastEvents journal
            //ProtoMessage titleTrans = new ProtoMessage();
            //titleTrans.ResponseType = DisplayMessages.CharacterTransferTitle;
            //titleTrans.MessageFields = fields;
            //JournalEntry thisEntry = new JournalEntry(entryID, year, season, thisPersonae, type, titleTrans, loc: location);
            //success = Globals_Game.AddPastEvent(thisEntry);
        }

        /// <summary>
        /// Transfers the title of a fief or province to another character
        /// </summary>
        /// <remarks>
        /// Predicate: assumes newHolder is male
        /// Predicate: assumes newHolder age 14 or over 
        /// </remarks>
        /// <returns>bool indicating success</returns>
        /// <param name="newHolder">The character receiving the title</param>
        /// <param name="titlePlace">The place to which the title refers</param>
        /// <param name="result">The result of attempting to grant</param>
        public bool GrantTitle(Character newHolder, Place titlePlace)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the total population of fiefs governed by the PlayerCharacter
        /// </summary>
        /// <returns>int containing total population</returns>
        public int GetMyPopulation()
        {
            int totalPop = 0;

            foreach (Fief thisFief in OwnedFiefs)
            {
                totalPop += thisFief.Population;
            }

            return totalPop;
        }

        /// <summary>
        /// Gets the percentage of population in the game governed by the PlayerCharacter
        /// </summary>
        /// <returns>double containing percentage of population governed</returns>
        public double GetPopulationPercentage()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the percentage of total fiefs in the game owned by the PlayerCharacter
        /// </summary>
        /// <returns>double containing percentage of total fiefs owned</returns>
        public double GetFiefsPercentage()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the percentage of total money in the game owned by the PlayerCharacter
        /// </summary>
        /// <returns>double containing percentage of total money owned</returns>
        public double GetMoneyPercentage()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Calculates the total funds currently owned by the PlayerCharacter
        /// </summary>
        /// <returns>int containing the total funds</returns>
        public int GetMyMoney()
        {
            int totalFunds = 0;

            foreach (Fief thisFief in this.OwnedFiefs)
            {
                totalFunds += thisFief.Treasury;
            }

            return totalFunds;
        }

        /// <summary>
        /// Elects a new leader from NPCs accompanying an army (upon death of PC leader)
        /// </summary>
        /// <returns>The new leader</returns>
        public NonPlayerCharacter ElectNewArmyLeader()
        {
            NonPlayerCharacter newLeader = null;

            double highestRating = 0;

            foreach (NonPlayerCharacter candidate in MyNPCs)
            {
                double armyLeaderRating = candidate.CalcArmyLeadershipRating();

                if (armyLeaderRating > highestRating)
                {
                    highestRating = armyLeaderRating;
                    newLeader = candidate;
                }
            }

            return newLeader;
        }

        /// <summary>
        /// Add a captive to a fief's gaol
        /// </summary>
        /// <param name="captive"></param>
        /// <param name="fief"></param>
        public void AddCaptive(Character captive, Fief fief)
        {
            // Move captive and add to gaol
            MyCaptives.Add(captive);
            captive.Captor = this;
            //captive.MoveCharacter(fief);
            Console.WriteLine("!!!--- Need to move captive to fief ---!!!");
            captive.InKeep = false;
            fief.Gaol.Add(captive);

            // Remove char as bailiff;
            List<Fief> bailiffFiefs = captive.GetFiefsBailiff();
            foreach (Fief f in bailiffFiefs)
            {
                f.Bailiff = null;
            }

            // Remove char as army leader
            if (captive.ArmyID != null)
            {
                captive.ArmyID.Leader = null;
                captive.ArmyID = null;
            }
        }

        /// <summary>
        /// Kill the specified captive and update the captive's family/employer of the death
        /// </summary>
        /// <param name="captive">Captive to be executted</param>
        public void ExecuteCaptive(Character captive)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Send a ransom to the family/employer of one of your captives
        /// </summary>
        /// <param name="captive">Captive to be ransomed</param>
        public void RansomCaptive(Character captive)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Releases one of your captives. The captive will immediately be transported to their employer/family's home fief
        /// </summary>
        /// <param name="captive">The captive to be released</param>
        public void ReleaseCaptive(Character captive)
        {
            throw new NotImplementedException();   
        }
    }

}
