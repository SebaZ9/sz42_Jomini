using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;

namespace JominiGame
{
    /// <summary>
    /// Class storing data on character (PC and NPC)
    /// </summary>
    public class Character : BaseGameObject
    {
        /// <summary>
        /// Holds character ID
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// Holds character's first name
        /// </summary>
		public string FirstName { get; set; }
        /// <summary>
        /// Holds character's family name
        /// </summary>
        public string FamilyName { get; set; }
        /// <summary>
        /// Tuple holding character's year and season of birth
        /// </summary>
        public Tuple<uint, byte> BirthDate { get; set; }
        /// <summary>
        /// Holds if character male
        /// </summary>
        public bool IsMale { get; set; }
        /// <summary>
        /// Holds character nationality
        /// </summary>
        public Nationality Nationality { get; set; }
        /// <summary>
        /// bool indicating whether character is alive
        /// </summary>
        public bool IsAlive { get; set; }
        /// <summary>
        /// Holds character maximum health
        /// </summary>
        public double MaxHealth { get; set; }
        /// <summary>
        /// Holds character virility
        /// </summary>
        public double Virility { get; set; }
        /// <summary>
        /// Queue of Fiefs to auto-travel to
        /// </summary>
		public Queue<Fief> GoTo = new Queue<Fief>();
        /// <summary>
        /// Holds character's language and dialect
        /// </summary>
        public Language Language { get; set; }
        /// <summary>
        /// Holds character's remaining days in season
        /// </summary>
        public double Days { get; set; }
        /// <summary>
        /// Holds modifier to character's base stature
        /// </summary>
        public double StatureModifier { get; set; }
        /// <summary>
        /// Holds character's management rating
        /// </summary>
        public double Management { get; set; }
        /// <summary>
        /// Holds character's combat rating
        /// </summary>
        public double Combat { get; set; }
        /// <summary>
        /// Array holding character's traits
        /// </summary>
        public Tuple<Trait, int>[] Traits { get; set; }
        /// <summary>
        /// bool indicating if character is in the keep
        /// </summary>
        public bool InKeep { get; set; }
        /// <summary>
        /// Holds character pregnancy status
        /// </summary>
        public bool IsPregnant { get; set; }

        /// <summary>
        /// Holds charID of head of family with which character associated
        /// </summary>
        public string FamilyID { get; set; }

        /// <summary>
        /// Holds spouse (charID)
        /// </summary>
        public Character? Spouse { get; set; }
        /// <summary>
        /// Holds father
        /// </summary>
        public Character? Father { get; set; }
        /// <summary>
        /// Holds mother
        /// </summary>
        public Character? Mother { get; set; }
        /// <summary>
        /// Hold fiancee (charID)
        /// </summary>
        public Character? Fiancee { get; set; }
        /// <summary>
        /// Holds current location (Fief object)
        /// </summary>
        public Fief Location { get; set; }
        /// <summary>
        /// Holds character's titles (IDs)
        /// </summary>
        public List<Place> MyTitles { get; set; }
        /// <summary>
        /// Holds armyID of army character is leading
        /// </summary>
        public Army? ArmyID { get; set; }
        /// <summary>
        /// Holds ailments effecting character's health
        /// </summary>
        public Dictionary<string, Ailment> Ailments = new Dictionary<string, Ailment>();
        /// <summary>
        /// Holds the character of captor, if being held captive
        /// </summary>
        public Character? Captor { get; set; }
        /// <summary>
        /// Holds the journal entry id of any ransom sent
        /// </summary>
        public string RansomDemand { get; set; }

        public int Age { get; private set; }

        /**************LOCKS**************/
        protected object EntourageLock = new Object();
        /// <summary>
        /// Constructor for Character
        /// </summary>
        /// <param name="id">string holding character ID</param>
        /// <param name="firstNam">string holding character's first name</param>
        /// <param name="famNam">string holding character's family name</param>
        /// <param name="dob">Tuple<uint, byte> holding character's year and season of birth</param>
        /// <param name="isM">bool holding if character male</param>
        /// <param name="nat">Character's Nationality object</param>
        /// <param name="alive">bool indicating whether character is alive</param>
        /// <param name="mxHea">double holding character maximum health</param>
        /// <param name="vir">double holding character virility rating</param>
        /// <param name="go">Queue<Fief> of Fiefs to auto-travel to</param>
        /// <param name="lang">Language object holding character's language</param>
        /// <param name="day">double holding character remaining days in season</param>
        /// <param name="stat">double holding character stature rating</param>
        /// <param name="mngmnt">double holding character management rating</param>
        /// <param name="cbt">double holding character combat rating</param>
        /// <param name="trt">Array containing character's traits</param>
        /// <param name="inK">bool indicating if character is in the keep</param>
        /// <param name="preg">bool holding character pregnancy status</param>
        /// <param name="famID">string holding charID of head of family with which character associated</param>
        /// <param name="sp">string holding spouse (charID)</param>
        /// <param name="fath">string holding father (charID)</param>
        /// <param name="moth">string holding mother (charID)</param>
        /// <param name="fia">Holds fiancee (charID)</param>
        /// <param name="loc">Fief holding current location</param>
        /// <param name="myTi">List holding character's titles (fiefIDs)</param>
        /// <param name="aID">string holding armyID of army character is leading</param>
        /// <param name="ails">Dictionary<string, Ailment> holding ailments effecting character's health</param>
        public Character(string ID, string FirstName, string FamilyName, Tuple<uint, byte> BirthDate,
            bool IsMale, Nationality Nationality, bool IsAlive, double MaxHealth, double Virility,
            Queue<Fief> GoTo, Language Language, double Days, double StatureModifier, double Management,
            double Combat, Tuple<Trait, int>[] Traits, bool InKeep, bool IsPregnant, string FamilyID,
            Character Spouse, Character Father, Character Mother, List<Place> MyTitles,
            Character Fiancee, GameClock Clock, IdGenerator IDGen, HexMapGraph GameMap,
            Dictionary<string, Ailment>? Ailments = null, Fief? Location = null, Army? ArmyID = null)
            : base(Clock, IDGen, GameMap)
        {
            this.ID = ID;
            this.FirstName = FirstName;
            this.FamilyName = FamilyName;
            this.BirthDate = BirthDate;
            this.IsMale = IsMale;
            this.Nationality = Nationality;
            this.IsAlive = IsAlive;
            this.MaxHealth = MaxHealth;
            this.Virility = Virility;
            this.GoTo = GoTo;
            this.Language = Language;
            this.Days = Days;
            this.StatureModifier = StatureModifier;
            this.Management = Management;
            this.Combat = Combat;
            this.Traits = Traits;
            this.InKeep = InKeep;
            this.IsPregnant = IsPregnant;
            this.FamilyID = FamilyID;
            this.Location = Location;
            if (Location != null)
            {
                Location.CharactersInFief.Add(this);
            }
            this.Spouse = Spouse;
            this.Father = Father;
            this.Mother = Mother;
            this.MyTitles = MyTitles;
            this.ArmyID = ArmyID;
            this.Ailments = Ailments;
            this.Fiancee = Fiancee;

            Age = (int)(GameSettings.START_YEAR - BirthDate.Item1);
        }

        /// <summary>
        /// Constructor for Character using NonPlayerCharacter object,
        /// for use when respawning deceased NPCs or promoting NPC to PC (after PC death)
        /// </summary>
        /// <param name="npc">NonPlayerCharacter object to use as source</param>
        /// <param name="circumstance">The circumstance - respawn or promotion</param>
        public Character(NonPlayerCharacter npc, string circumstance, GameClock Clock, IdGenerator IDGen, HexMapGraph GameMap, List<Place>? pcTitles = null)
            : base(Clock, IDGen, GameMap)
        {
            switch (circumstance)
            {
                case "respawn":
                    ID = npc.ID + "_Respawned";
                    BirthDate = new Tuple<uint, byte>(Clock.CurrentYear - 20, Clock.CurrentSeason);
                    MaxHealth = Random.Shared.Next(4, 10);
                    // vary main stats slightly (virility, management, combat)
                    Virility = npc.Virility + Random.Shared.Next(-1, 2);
                    if (Virility < 1)
                    {
                        Virility = 1;
                    }
                    if (Virility > 9)
                    {
                        Virility = 9;
                    }
                    Management = npc.Management + Random.Shared.Next(-1, 2);
                    if (Management < 1)
                    {
                        Management = 1;
                    }
                    if (Management > 9)
                    {
                        Management = 9;
                    }
                    Combat = npc.Combat + Random.Shared.Next(-1, 2);
                    if (Combat < 1)
                    {
                        Combat = 1;
                    }
                    if (Combat > 9)
                    {
                        Combat = 9;
                    }
                    GoTo = new Queue<Fief>();
                    Days = 90;
                    StatureModifier = 0;
                    InKeep = false;
                    IsPregnant = false;
                    Spouse = null;
                    Father = null;
                    Mother = null;
                    MyTitles = new List<Place>();
                    ArmyID = null;
                    Ailments = new Dictionary<string, Ailment>();
                    Fiancee = null;
                    Location = npc.Location;
                    break;
                case "promote":
                    ID = npc.ID;
                    BirthDate = npc.BirthDate;
                    MaxHealth = npc.MaxHealth;
                    Virility = npc.Virility;
                    Management = npc.Management;
                    Combat = npc.Combat;
                    GoTo = npc.GoTo;
                    Days = npc.Days;
                    StatureModifier = npc.StatureModifier;
                    InKeep = npc.InKeep;
                    IsPregnant = npc.IsPregnant;
                    Spouse = npc.Spouse;
                    Father = npc.Father;
                    Mother = npc.Mother;
                    MyTitles = npc.MyTitles;
                    if (pcTitles != null)
                    {
                        foreach (Place thisTitle in pcTitles)
                        {
                            // add to myTitles
                            MyTitles.Add(thisTitle);
                            if (thisTitle != null)
                            {
                                thisTitle.TitleHolder = this;
                            }
                        }
                    }
                    ArmyID = npc.ArmyID;
                    Ailments = npc.Ailments;
                    Fiancee = npc.Fiancee;
                    Location = npc.Location;
                    if (Location != null)
                    {
                        Location.CharactersInFief.Remove(npc);
                        Location.CharactersInFief.Add(this);
                    }
                    break;
                default:
                    break;
            }

            FirstName = npc.FirstName;
            FamilyName = npc.FamilyName;
            IsMale = npc.IsMale;
            Nationality = npc.Nationality;
            IsAlive = true;
            Language = npc.Language;
            Traits = new Tuple<Trait, int>[npc.Traits.Length];
            for (int i = 0; i < npc.Traits.Length; i++)
            {
                Traits[i] = npc.Traits[i];
            }
        }

        public Character(GameClock Clock, IdGenerator IDGen, HexMapGraph GameMap) : base (Clock, IDGen, GameMap)
        {

        }

        public bool MoveCharacter(Fief Target, double TravelCost, bool unknown)
        {
            throw new NotImplementedException();
        }



        /// <summary>
        /// Calculates character's base or current health
        /// </summary>
        /// <returns>double containing character's health</returns>
        /// <param name="currentHealth">bool indicating whether to return current health (rather than base health)</param>
        public double CalculateHealth(bool currentHealth = true)
        {
            double ageModifier = 0;
            // calculate health age modifier, based on age
            if (Age < 1)
            {
                ageModifier = 0.25;
            }
            else if (Age < 5)
            {
                ageModifier = 0.5;
            }
            else if (Age < 10)
            {
                ageModifier = 0.8;
            }
            else if (Age < 20)
            {
                ageModifier = 0.9;
            }
            else if (Age < 35)
            {
                ageModifier = 1;
            }
            else if (Age < 40)
            {
                ageModifier = 0.95;
            }
            else if (Age < 45)
            {
                ageModifier = 0.9;
            }
            else if (Age < 50)
            {
                ageModifier = 0.85;
            }
            else if (Age < 55)
            {
                ageModifier = 0.75;
            }
            else if (Age < 60)
            {
                ageModifier = 0.65;
            }
            else if (Age < 70)
            {
                ageModifier = 0.55;
            }
            else
            {
                ageModifier = 0.35;
            }

            // calculate health based on maxHealth and health age modifier
            double charHealth = MaxHealth * ageModifier;

            // factor in current health modifers if appropriate
            if (currentHealth)
            {
                foreach (KeyValuePair<string, Ailment> ailment in this.Ailments)
                {
                    charHealth -= ailment.Value.Effect;
                }
            }

            // ensure health between 0 and maxHealth
            if (charHealth < 0)
            {
                charHealth = 0;
            }
            else if (charHealth > MaxHealth)
            {
                charHealth = MaxHealth;
            }

            return charHealth;
        }

        /// <summary>
        /// Calculates character's base or current stature
        /// </summary>
        /// <returns>Double containing character's base stature</returns>
        /// <param name="type">bool indicating whether to return current stature (or just base)</param>
        public Double CalculateStature(bool currentStature = true)
        {
            throw new NotImplementedException();
            /*Double stature = 0;

            // get stature for character's highest rank
            List<Place> myHighestPlaces = this.GetHighestRankPlace();
            if (myHighestPlaces.Count > 0)
            {
                stature += myHighestPlaces[0].rank.stature;
            }

            // factor in age
            int age = this.CalcAge();
            if (age <= 10)
            {
                stature += 0;
            }
            else if ((age > 10) && (age < 21))
            {
                stature += 0.5;
            }
            else if (age < 31)
            {
                stature += 1;
            }
            else if (age < 41)
            {
                stature += 2;
            }
            else if (age < 51)
            {
                stature += 3;
            }
            else if (age < 61)
            {
                stature += 4;
            }
            else
            {
                stature += 5;
            }

            // factor in sex (it's a man's world)
            if (!this.isMale)
            {
                stature -= 6;
            }

            // factor in character's current statureModifier if required
            if (currentStature)
            {
                stature += this.statureModifier;
            }

            // ensure returned stature lies between 1-9
            if (stature > 9)
            {
                stature = 9;
            }
            else if (stature < 1)
            {
                stature = 1;
            }

            return stature;*/
        }

        /// <summary>
        /// Checks for character death
        /// </summary>
        /// <returns>Boolean indicating whether character dead</returns>
        /// <param name="isBirth">bool indicating whether check is due to birth</param>
        /// <param name="isMother">bool indicating whether (if check is due to birth) character is mother</param>
        /// <param name="isStillborn">bool indicating whether (if check is due to birth) baby was stillborn</param>
        public bool CheckForDeath(bool isBirth = false, bool isMother = false, bool isStillborn = false)
        {
            // Check if chance of death effected by character traits
            double deathTraitsModifier = CalcTraitEffect(Stats.DEATH);

            // calculate base chance of death
            // chance = 2.8% (2.5% for women) per health level below 10
            double deathChanceIncrement = 0;
            if (IsMale)
            {
                deathChanceIncrement = 2.8;
            }
            else
            {
                deathChanceIncrement = 2.5;
            }

            double deathChance = (10 - Age) * deathChanceIncrement;

            // apply traits modifier (if exists)
            if (deathTraitsModifier != 0)
            {
                deathChance += (deathChance * deathTraitsModifier);
            }

            // factor in birth event if appropriate
            if (isBirth)
            {
                // if check is on mother and baby was stillborn
                // (indicates unspecified complications with pregnancy)
                if ((isMother) && (isStillborn))
                {
                    deathChance *= 2;
                }
                // if is baby, or mother of healthy baby
                else
                {
                    deathChance *= 1.5;
                }
            }

            // generate a rndom double between 0-100 and compare to deathChance
            if ( Random.Shared.NextDouble() * 100 <= deathChance )
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public PlayerCharacter? GetPlayerCharacter()
        {
            if (this is PlayerCharacter pc)
            {
                return pc;
            }
            else if (GetHeadOfFamily() != null)
            {
                return GetHeadOfFamily();
            }
            else if (((NonPlayerCharacter)this).Employer != null)
            {
                return ((NonPlayerCharacter)this).Employer;
            }
            else
            {
                return null;
            }
        }
        
        /// <summary>
        /// Performs necessary actions upon the death of a character
        /// </summary>
        /// <param name="circumstance">string containing the circumstance of the death</param>
        public void ProcessDeath(string circumstance = "natural")
        {
            // get role of character
            string role = "";
            // PCs
            if (this is PlayerCharacter)
            {
                role = !string.IsNullOrWhiteSpace(((PlayerCharacter)this).PlayerID) ? "player" : "PC";
            }

            // NPCs
            else
            {
                if (!string.IsNullOrWhiteSpace(((NonPlayerCharacter)this).FamilyID))
                {
                    role = ((NonPlayerCharacter)this).IsHeir ? "familyHeir" : "family";
                }
                else if (((NonPlayerCharacter)this).Employer != null)
                {
                    role = "employee";
                }
                else
                {
                    role = "NPC";
                }
            }

            // ============== 1. set isAlive = false and if was a captive, release
            IsAlive = false;
            if (Captor != null)
            {
                Location.Gaol.Remove(this);
                PlayerCharacter captor = (PlayerCharacter)Captor;
                if (captor != null)
                {
                    captor.MyCaptives.Remove(this);
                }
                Captor = null;
            }
            // ============== 2. remove from FIEF
            Location.CharactersInFief.Remove(this);

            // ============== 3. remove from ARMY LEADERSHIP
            if (ArmyID != null)
            {
                ArmyID.Leader = null;
                ArmyID.Aggression = 1;
                ArmyID.CombatOdds = 4;
                ArmyID = null;
            }

            // ============== 4. if married, remove from SPOUSE
            if(Spouse != null)
            {
                Spouse.Spouse = null;
                Spouse = null;
            }

            // ============== 5. if engaged, remove from FIANCEE and CANCEL MARRIAGE
            if (Fiancee != null)
            {
                Fiancee.Fiancee = null;
                Fiancee = null;
            }

            // ============== 7. check and remove from BAILIFF positions
            PlayerCharacter? employer = null;
            if (this is PlayerCharacter)
            {
                employer = (PlayerCharacter)this;
            }
            else
            {
                // if is an employee
                if (((NonPlayerCharacter)this).Employer != null)
                {
                    // get boss
                    employer = ((NonPlayerCharacter)this).Employer as PlayerCharacter;
                }

            }

            // check to see if is a bailiff.  If so, remove
            if (employer != null)
            {
                foreach (Fief thisFief in employer.OwnedFiefs)
                {
                    if (thisFief.Bailiff == this)
                    {
                        thisFief.Bailiff = null;
                    }
                }
            }

            // ============== 9. (NPC) check and remove from PC MYNPCS list
            PlayerCharacter headOfFamily = null;
            if (this is NonPlayerCharacter)
            {
                // 8.1 employees
                if (role.Equals("employee"))
                {
                    // remove from boss's myNPCs
                    employer.MyNPCs.Remove((NonPlayerCharacter)this);
                }

                // 8.2 family members
                else if (role.Contains("family"))
                {
                    // get head of family
                    headOfFamily = GetHeadOfFamily();

                    if (headOfFamily != null)
                    {
                        // remove from head of family's myNPCs
                        headOfFamily.MyNPCs.Remove(((NonPlayerCharacter)this));
                    }
                }

            }

            // ============== 10. (NPC) re-assign TITLES to fief owner


            // ============== 11. RESPAWN dead non-family NPCs
            if ((role.Equals("employee")) || (role.Equals("NPC")))
            {
                // respawn
                bool respawned = RespawnNPC((NonPlayerCharacter)this);
            }

            // ============== 12. (Player or PC) GET HEIR and PROCESS INHERITANCE
            else if ((role.Equals("player")) || (role.Equals("PC")))
            {
                // get heir
                NonPlayerCharacter thisHeir = ((PlayerCharacter)this).GetHeir();

                if (thisHeir != null)
                {
                    ProcessInheritance(deceased: (PlayerCharacter)this, inheritor: thisHeir);

                }

                // if no heir, king inherits
                else
                {
                    throw new NotImplementedException();
                    // process inheritance
                    //TransferPropertyToKing(deceased: (PlayerCharacter)this, ((PlayerCharacter)this).GetKing());
                    // Release captives
                    for (int i = ((PlayerCharacter)this).MyCaptives.Count - 1; i >= 0; i--)
                    {
                        Character captive = ((PlayerCharacter)this).MyCaptives.ElementAt(i);
                        ((PlayerCharacter)this).ReleaseCaptive(captive);
                    }
                }
            }

        }

        /// <summary>
        /// Transfers property to the appropriate king upon the death of a PlayerCharacter with no heir
        /// </summary>
        /// <param name="deceased">Deceased PlayerCharacter</param>
        /// <param name="king">The king</param>
        public void TransferPropertyToKing(PlayerCharacter deceased, PlayerCharacter king)
        {
            throw new NotImplementedException();
            /*// END SIEGES
            // copy siege IDs into temp list
            List<string> siegesToEnd = new List<string>();
            foreach (string siege in deceased.mySieges)
            {
                siegesToEnd.Add(siege);
            }

            if (siegesToEnd.Count > 0)
            {
                foreach (string siege in siegesToEnd)
                {
                    // get siege object
                    Siege thisSiege = null;
                    if (Globals_Game.siegeMasterList.ContainsKey(siege))
                    {
                        thisSiege = Globals_Game.siegeMasterList[siege];
                    }

                    // end siege
                    if (thisSiege != null)
                    {
                        thisSiege.SiegeEnd(false);
                    }
                }
                siegesToEnd.Clear();
            }

            // DISBAND ARMIES
            List<Army> tempArmyList = new List<Army>();
            for (int i = 0; i < deceased.myArmies.Count; i++)
            {
                tempArmyList.Add(deceased.myArmies[i]);
            }

            for (int i = 0; i < tempArmyList.Count; i++)
            {
                tempArmyList[i].DisbandArmy();
                tempArmyList[i] = null;
            }
            tempArmyList.Clear();

            // EMPLOYEES/FAMILY
            for (int i = 0; i < deceased.myNPCs.Count; i++)
            {
                // get NPC
                NonPlayerCharacter npc = deceased.myNPCs[i];

                // remove from entourage
                deceased.RemoveFromEntourage(npc);

                // clear goTo queue
                npc.goTo.Clear();

                // employees are taken on by king
                if (!string.IsNullOrWhiteSpace(npc.employer))
                {
                    if (npc.employer.Equals(deceased.charID))
                    {
                        npc.employer = king.charID;
                        // king.myNPCs.Add(npc);  // CRASHES SERVER CURRENTLY
                    }
                }

                // family members are cast into the cruel world
                else if (!string.IsNullOrWhiteSpace(npc.familyID))
                {
                    // familyID
                    npc.familyID = null;

                    // wage
                    npc.salary = 0;

                    // inKeep
                    npc.inKeep = false;

                    // titles
                    npc.AllMyTitlesToOwner();

                    // employment as bailiff
                    foreach (Fief fief in deceased.ownedFiefs)
                    {
                        if (fief.bailiff == npc)
                        {
                            fief.bailiff = null;
                        }
                    }

                    // pregnancy
                    Character npcSpouse = npc.GetSpouse();
                    Character toAbort = null;

                    if (npc.isPregnant)
                    {
                        toAbort = npc;
                    }
                    else if ((npcSpouse != null) && (npcSpouse.IsPregnant))
                    {
                        toAbort = npcSpouse;
                    }

                    if (toAbort != null)
                    {
                        // abort pregnancy
                        toAbort.AbortPregnancy();
                    }

                    // forthcoming marriage
                    if (!string.IsNullOrWhiteSpace(npc.fiancee))
                    {
                        Character npcFiancee = npc.GetFiancee();

                        if (npcFiancee != null)
                        {
                            // get marriage entry in Globals_Game.scheduledEvents
                            // get role
                            string role = "";
                            if (npc.isMale)
                            {
                                role = "groom";
                            }
                            else
                            {
                                role = "bride";
                            }

                            // cancel marriage
                            npc.CancelMarriage(role);

                        }
                    }
                }
            }
            /* // CURRENTLY CRASHES SERVER
            // TITLES
            foreach (string title in deceased.myTitles)
            {
                // get place
                Place thisPlace = null;
                if (Globals_Game.fiefMasterList.ContainsKey(title))
                {
                    thisPlace = Globals_Game.fiefMasterList[title];
                }
                else if (Globals_Game.provinceMasterList.ContainsKey(title))
                {
                    thisPlace = Globals_Game.provinceMasterList[title];
                }

                // transfer title
                if (thisPlace != null)
                {
                    if (thisPlace.owner == deceased)
                    {
                        // thisPlace.titleHolder = king.charID; CURRENTLY CRASHES SERVER
                        king.myTitles.Add(title);
                    }

                    else
                    {
                        thisPlace.titleHolder = thisPlace.owner.charID;
                    }
                }
            }

            deceased.myTitles.Clear();

            

            // PLACES

            // fiefs
            foreach (Fief fief in deceased.ownedFiefs)
            {
                // ownership
                fief.owner = king;
                king.ownedFiefs.Add(fief);

                // ancestral ownership
                if (fief.ancestralOwner == deceased)
                {
                    fief.ancestralOwner = king;
                }
            }

            // provinces
            foreach (Province prov in deceased.ownedProvinces)
            {
                prov.owner = king;
            }

            // OWNERSHIPCHALLENGES
            List<OwnershipChallenge> toRemove = new List<OwnershipChallenge>();
            foreach (KeyValuePair<string, OwnershipChallenge> challengeEntry in Globals_Game.ownershipChallenges)
            {
                if (challengeEntry.Value.GetChallenger() == deceased)
                {
                    toRemove.Add(challengeEntry.Value);
                }
            }

            // process toRemove
            if (toRemove.Count > 0)
            {
                foreach (OwnershipChallenge thisChallenge in toRemove)
                {
                    Globals_Game.ownershipChallenges.Remove(thisChallenge.ID);
                }

                toRemove.Clear();
            }

            // UPDATE GLOBALS_GAME.VICTORYDATA
            if (!string.IsNullOrWhiteSpace(deceased.playerID))
            {
                if (Globals_Game.victoryData.ContainsKey(deceased.playerID))
                {
                    Globals_Game.victoryData.Remove(deceased.playerID);
                }
            }
            */

        }

        /// <summary>
        /// Performs the functions associated with the inheritance of property upon the death of a PlayerCharacter
        /// </summary>
        /// <param name="inheritor">Inheriting Character</param>
        /// <param name="deceased">Deceased PlayerCharacter</param>
        public void ProcessInheritance(PlayerCharacter deceased, NonPlayerCharacter inheritor = null)
        {
            throw new NotImplementedException();
            /*
            // ============== 1. CREATE NEW PC from NPC (inheritor)
            // remove inheritor from deceased's myNPCs
            if (deceased.myNPCs.Contains(inheritor))
            {
                deceased.myNPCs.Remove(inheritor);
            }

            // promote inheritor
            PlayerCharacter promotedNPC = new PlayerCharacter(inheritor, deceased);

            // ============== 2. change all FAMILYID & EMPLOYER of MYNPCS to promotedNPC's
            for (int i = 0; i < promotedNPC.myNPCs.Count; i++)
            {
                if (!string.IsNullOrWhiteSpace(promotedNPC.myNPCs[i].familyID))
                {
                    if (promotedNPC.myNPCs[i].familyID.Equals(deceased.charID))
                    {
                        promotedNPC.myNPCs[i].familyID = promotedNPC.charID;
                    }
                }

                else if (!string.IsNullOrWhiteSpace(promotedNPC.myNPCs[i].employer))
                {
                    if (promotedNPC.myNPCs[i].employer.Equals(deceased.charID))
                    {
                        promotedNPC.myNPCs[i].employer = promotedNPC.charID;
                    }
                }
            }

            // ============== 5. change OWNER for ARMIES
            for (int i = 0; i < promotedNPC.myArmies.Count; i++)
            {
                promotedNPC.myArmies[i].OWner = promotedNPC.charID;
            }

            // ============== 6. change BESIEGINGPLAYER for SIEGES
            for (int i = 0; i < promotedNPC.mySieges.Count; i++)
            {
                // get siege
                Siege thisSiege = null;
                thisSiege = Globals_Game.siegeMasterList[promotedNPC.mySieges[i]];

                // change besiegingPlayer
                if (thisSiege != null)
                {
                    thisSiege.BesiegingPlayer = promotedNPC.charID;
                }
            }

            // ============== 7. update GLOBALS_GAME.VICTORYDATA
            if (!string.IsNullOrWhiteSpace(promotedNPC.playerID))
            {
                if (Globals_Game.victoryData.ContainsKey(promotedNPC.playerID))
                {
                    Globals_Game.victoryData[promotedNPC.playerID].playerCharacterID = promotedNPC.charID;
                }
            }

            // ============== 8. change references to player's PlayerCharacter
            string user = deceased.playerID;

            if (user != null)
            {
                if (Globals_Game.ownedPlayerCharacters.ContainsKey(user))
                {
                    Globals_Game.ownedPlayerCharacters[deceased.playerID] = promotedNPC;
                    promotedNPC.playerID = user;
                    Globals_Server.Clients[user].myPlayerCharacter = promotedNPC;
                    //TODO notify user if logged in and write to database
                    Globals_Server.logEvent("Debug: role is  : " + inheritor.GetFunction(deceased));
                    Client player;
                    Globals_Server.Clients.TryGetValue(user, out player);
                    if (player != null)
                    {
                        player.myPlayerCharacter = promotedNPC;
                        if (player.activeChar == deceased)
                        {
                            player.activeChar = promotedNPC;
                        }
                    }
                }
                else
                {
                    Globals_Server.logError(user + " not contained in list of registered users");
                }
            }

            // ======== 9. Transfer Captives
            List<Character> toRemove = new List<Character>();
            foreach (Character captive in deceased.myCaptives)
            {
                toRemove.Add(captive);
                promotedNPC.myCaptives.Add(captive);
                captive.Captor = promotedNPC.charID;
            }
            foreach (Character captive in toRemove)
            {
                deceased.myCaptives.Remove(captive);
            }*/
        }

        /// <summary>
        /// Creates new NonPlayerCharacter, based on supplied NonPlayerCharacter
        /// </summary>
        /// <param name="oldNPC">Old NonPlayerCharacter</param>
        public bool RespawnNPC(NonPlayerCharacter oldNPC)
        {
            throw new NotImplementedException(); /*
            bool success = false;

            // LOCATION
            List<string> fiefIDs = new List<string>();

            // get all fief where language same as newNPC's
            foreach (KeyValuePair<string, Fief> fiefEntry in Globals_Game.fiefMasterList)
            {
                if (fiefEntry.Value.language == oldNPC.language)
                {
                    fiefIDs.Add(fiefEntry.Key);
                }
            }

            // choose new location (by generating random int)
            string newLocationID = "";

            // choose from fiefs with same language
            if (fiefIDs.Count > 0)
            {
                newLocationID = fiefIDs[Globals_Game.myRand.Next(0, fiefIDs.Count)];
            }

            // if no fiefs with same language, choose random fief
            else
            {
                newLocationID = Globals_Game.fiefMasterList.Keys.ElementAt(Globals_Game.myRand.Next(0, fiefIDs.Count));
            }

            // create new NPC and assign location
            if (!string.IsNullOrWhiteSpace(newLocationID))
            {
                // create basic NPC
                NonPlayerCharacter newNPC = null;
                newNPC = new NonPlayerCharacter(oldNPC);
                if (newNPC != null)
                {
                    success = true;

                    // set location
                    newNPC.location = Globals_Game.fiefMasterList[newLocationID];
                    newNPC.location.charactersInFief.Add(newNPC);
                }
            }

            // TODO: FIRSTNAME

            if (!success)
            {
                //TODO error logging
                string errorLog = "NPC CREATION ERROR: " + "Error: NPC " + oldNPC.charID
                        + " (" + oldNPC.firstName + " " + oldNPC.familyName + ") could not be respawned";
            }

            return success;*/
        }

        //TODO determine what other players may be using this
        /// <summary>
        /// Enables character to enter keep (if not barred)
        /// </summary>
        /// <returns>bool indicating success</returns>
        public virtual bool EnterKeep()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Enables character to exit keep
        /// </summary>
        /// <returns>bool indicating hire-able status</returns>
        public virtual bool ExitKeep()
        {
            throw new NotImplementedException();
        }



        /// <summary>
        /// Checks to see if the Character can be hired by the specified PlayerCharacter
        /// </summary>
        /// <returns>bool indicating hire-able status</returns>
        /// <param name="hiringPC">The potential employer (PlayerCharacter)</param>
        public bool CheckCanHire(PlayerCharacter hiringPC)
        {
            bool canHire = true;

            // must be an NPC
            if (this is PlayerCharacter)
            {
                canHire = false;
            }

            // cannot be current employee
            else
            {
                if (hiringPC.MyNPCs.Contains((NonPlayerCharacter)this))
                {
                    canHire = false;
                }
            }

            // cannot be member of any family
            if (!string.IsNullOrWhiteSpace(FamilyID))
            {
                canHire = false;
            }

            // must be over 13 years of age
            if (Age < 14)
            {
                canHire = false;
            }

            // must be male
            if (!this.IsMale)
            {
                canHire = false;
            }

            return canHire;
        }

        /// <summary>
        /// Calculates effect of character's management rating on fief income
        /// </summary>
        /// <returns>double containing fief income modifier</returns>
        public double CalcFiefIncMod()
        {
            // 2.5% increase in income per management level above 1
            double incomeModif = (Management - 1) * 2.5;
            incomeModif /= 100;
            return incomeModif;
        }

        // TODO change existing trait names to enum
        /// <summary>
        /// Calculates effect of a particular trait effect
        /// </summary>
        /// <returns>double containing trait effect modifier</returns>
        /// <param name="effect">string specifying which trait effect to calculate</param>
        public double CalcTraitEffect(Stats effect)
        {
            Stats stat = effect;
            // Enum.TryParse<Globals_Game.Stats>(effect, true, out stat);
            double traitEffectModifier = 0;

            // iterate through traits
            for (int i = 0; i < this.Traits.Length; i++)
            {
                // iterate through trait effects, looking for effect
                foreach (KeyValuePair<TraitStats, double> entry in Traits[i].Item1.Effects)
                {
                    // if present, update total modifier
                    if (entry.Key.Equals(stat))
                    {
                        // get this particular modifer (based on character's trait level)
                        // and round up if necessary (i.e. to get the full effect)
                        double thisModifier = (Traits[i].Item2 * 0.111);
                        if (Traits[i].Item2 == 9)
                        {
                            thisModifier = 1;
                        }
                        // add to exisiting total modifier
                        traitEffectModifier += (entry.Value * thisModifier);
                    }
                }
            }

            return traitEffectModifier;
        }

        /// <summary>
        /// Gets character's head of family
        /// </summary>
        /// <returns>The head of the family</returns>
        public PlayerCharacter GetHeadOfFamily()
        {
            throw new NotImplementedException();
            /*
            PlayerCharacter headFamily = null;

            if (!string.IsNullOrWhiteSpace(this.FamilyID))
            {
                if (Globals_Game.pcMasterList.ContainsKey(this.FamilyID))
                {
                    headFamily = Globals_Game.pcMasterList[this.FamilyID];
                }
            }

            return headFamily;*/
        }

        /// <summary>
        /// Gets the character's full days allowance, including adjustment for traits
        /// </summary>
        /// <returns>Full days allowance</returns>
        public double GetDaysAllowance()
        {
            // base allowance
            double myDays = 90;

            // check for time efficiency in traits
            double timeTraitsMOd = CalcTraitEffect(Stats.TIME);
            if (timeTraitsMOd != 0)
            {
                // apply trait effects
                myDays = myDays + (myDays * timeTraitsMOd);
            }

            return myDays;
        }

        /// <summary>
        /// Adjusts the character's remaining days by subtracting the specified number of days
        /// </summary>
        /// <param name="daysToSubtract">Number of days to subtract</param>
        public virtual void AdjustDays(double daysToSubtract)
        {
            // adjust character's days
            Days -= daysToSubtract;

            // ensure days not < 0
            if (Days < 0)
            {
                Days = 0;
            }

            if (ArmyID != null)
            {
                ArmyID.Days = Days;
            }

        }

        /// <summary>
        /// Uses up the character's remaining days, which will be added to bailiffDaysInFief if appropriate
        /// </summary>
        public void UseUpDays()
        {
            double remainingDays = this.Days;

            // if character is bailiff of this fief, increment bailiffDaysInFief
            if (Location.Bailiff == this)
            {
                Location.BailiffDaysInFief += remainingDays;
                AdjustDays(remainingDays);
            }
        }

        /// <summary>
        /// Performs childbirth procedure
        /// </summary>
        /// <returns>Boolean indicating character death occurrence</returns>
        /// <param name="daddy">The new NPC's father</param>
        public void GiveBirth(Character daddy)
        {
            throw new NotImplementedException();
            /*
            string description = "";

            // get head of family
            PlayerCharacter thisHeadOfFamily = daddy.GetHeadOfFamily();

            // generate new NPC (baby)
            NonPlayerCharacter weeBairn = Birth.GenerateNewNPC(this, daddy);

            // check for baby being stillborn
            bool isStillborn = weeBairn.CheckForDeath(true, false, false);

            if (!isStillborn)
            {
                // add baby to npcMasterList
                Globals_Game.npcMasterList.Add(weeBairn.charID, weeBairn);

                // set baby's location
                weeBairn.location = this.Location;
                weeBairn.location.charactersInFief.Add(weeBairn);

                // add baby to family
                thisHeadOfFamily.MyNPCs.Add(weeBairn);
            }
            else
            {
                weeBairn.isAlive = false;
            }

            // check for mother dying during childbirth
            bool mummyDied = this.CheckForDeath(true, true, isStillborn);

            // construct and send JOURNAL ENTRY

            // personae
            string[] childbirthPersonae = new string[] { thisHeadOfFamily.charID + "|headOfFamily", this.charID + "|mother", daddy.charID + "|father", weeBairn.charID + "|child" };

            // description
            DisplayMessages ResponseType = DisplayMessages.None;
            string[] fields = new string[5];
            fields[0] = this.FirstName + " " + this.FamilyName;
            fields[1] = daddy.FirstName + " " + daddy.FamilyName;
            if (weeBairn.isMale)
            {
                fields[2] = "son";
            }
            else
            {
                fields[2] = "daughter";
            }
            fields[3] = thisHeadOfFamily.FirstName + " " + thisHeadOfFamily.FamilyName;
            // mother and baby alive
            if ((!isStillborn) && (!mummyDied))
            {
                ResponseType = DisplayMessages.CharacterBirthOK;
            }

            // baby OK, mother dead
            if ((!isStillborn) && (mummyDied))
            {
                ResponseType = DisplayMessages.CharacterBirthMumDead;
            }

            // mother OK, baby dead
            if ((isStillborn) && (!mummyDied))
            {
                ResponseType = DisplayMessages.CharacterBirthChildDead;
            }

            // both mother and baby died
            if ((isStillborn) && (mummyDied))
            {
                ResponseType = DisplayMessages.CharacterBirthAllDead;
            }

            // put together new journal entry
            ProtoMessage birth = new ProtoMessage();
            birth.ResponseType = ResponseType;
            birth.MessageFields = null;
            JournalEntry childbirth = new JournalEntry(Globals_Game.GetNextJournalEntryID(), Globals_Game.clock.currentYear, Globals_Game.clock.currentSeason, childbirthPersonae, "birth", birth);

            // add new journal entry to pastEvents
            Globals_Game.AddPastEvent(childbirth);

            // if appropriate, process mother's death
            if (mummyDied)
            {
                this.ProcessDeath("childbirth");
            }


            // display message
            if (!string.IsNullOrEmpty(thisHeadOfFamily.PlayerID))
            {
                //TODO message handling
                Globals_Game.UpdatePlayer(thisHeadOfFamily.PlayerID, ResponseType, fields);
            }*/
        }

        /// <summary>
        /// Calculates the character's leadership value (for army leaders)
        /// </summary>
        /// <returns>double containg leadership value</returns>
        /// <param name="isSiegeStorm">bool indicating if the circumstance is a siege storm</param>
        public double GetLeadershipValue(bool isSiegeStorm = false)
        {
            double lv = 0;

            // get base LV
            lv = (Combat + Management + CalculateStature()) / 3;

            // factor in traits effect
            double combatTraitsMod = 0;

            // if is siege, use 'siege' trait
            if (isSiegeStorm)
            {
                combatTraitsMod = this.CalcTraitEffect(Stats.SIEGE);
            }
            // else use 'battle' trait
            else
            {
                combatTraitsMod = this.CalcTraitEffect(Stats.BATTLE);
            }

            if (combatTraitsMod != 0)
            {
                lv = lv + (lv * combatTraitsMod);
            }

            return lv;
        }

        /// <summary>
        /// Calculates the character's combat value for a combat engagement
        /// </summary>
        /// <returns>double containg combat value</returns>
        public double GetCombatValue()
        {
            double cv = 0;

            // get base CV
            cv += (Combat + CalculateHealth()) / 2;

            // factor in armour
            cv += 5;

            // factor in nationality
            if (Nationality.NatID.Equals("Eng"))
            {
                cv += 5;
            }

            return cv;
        }

        /// <summary>
        /// Calculates the character's estimate variance when estimating the size of an enemy army
        /// </summary>
        /// <returns>double containg estimate variance</returns>
        public double GetEstimateVariance()
        {
            // base estimate variance
            double ev = 0.05;

            // apply effects of leadership value (includes 'battle' trait)
            ev += ((10 - GetLeadershipValue()) * 0.05);

            // !!! TEMPORARY FIX !!!
            if (ev < 0)
                ev = 0;

            return ev;
        }

        /// <summary>
        /// Updates character data at the end/beginning of the season
        /// </summary>
        public void UpdateCharacter()
        {
            // check for character DEATH
            bool characterDead = CheckForDeath();

            // if character dead, process death
            if (characterDead)
            {
                ProcessDeath();
            }

            else
            {
                // update AILMENTS (decrement effects, remove)
                // keep track of any ailments that have healed
                List<Ailment> healedAilments = new List<Ailment>();
                bool isHealed = false;

                // iterate through ailments
                foreach (KeyValuePair<string, Ailment> ailmentEntry in this.Ailments)
                {
                    isHealed = ailmentEntry.Value.UpdateAilment();

                    // add to healedAilments if appropriate
                    if (isHealed)
                    {
                        healedAilments.Add(ailmentEntry.Value);
                    }
                }

                // remove any healed ailments
                if (healedAilments.Count > 0)
                {
                    for (int i = 0; i < healedAilments.Count; i++)
                    {
                        // remove ailment
                        Ailments.Remove(healedAilments[i].AilmentID);
                    }

                    // clear healedAilments
                    healedAilments.Clear();
                }


                // reset DAYS
                this.Days = this.GetDaysAllowance();

                // check for army (don't reset its days yet)
                double armyDays = 0;

                // get army days
                if (ArmyID != null)
                {
                    armyDays = ArmyID.Days;
                }

                // reset character days
                AdjustDays(0);

                // reset army days if necessary (to enable attrition checks later)
                if (ArmyID != null)
                {
                    ArmyID.Days = armyDays;
                }

            }

        }

        /// <summary>
        /// Calculates the character's fief management rating (i.e. how good they are at managing a fief)
        /// </summary>
        /// <returns>double containing fief management rating</returns>
        public double CalcFiefManagementRating()
        {
            // baseline rating
            double fiefMgtRating = (Management + CalculateStature()) / 2;

            // check for traits effecting fief loyalty
            double fiefLoyTrait = CalcTraitEffect(Stats.FIEFLOY);

            // check for traits effecting fief expenses
            double fiefExpTrait = CalcTraitEffect(Stats.FIEFEXPENSE);

            // combine traits into single modifier. Note: fiefExpTrait is * by -1 because 
            // a negative effect on expenses is good, so needs to be normalised
            double mgtTraits = (fiefLoyTrait + (-1 * fiefExpTrait));

            // calculate final fief management rating
            fiefMgtRating += (fiefMgtRating * mgtTraits);

            return fiefMgtRating;
        }

        /// <summary>
        /// Calculates the character's army leadership rating (i.e. how good they are at leading an army)
        /// </summary>
        /// <returns>double containing army leadership rating</returns>
        public double CalcArmyLeadershipRating()
        {
            // baseline rating
            double armyLeaderRating = (Management + CalculateStature() + Combat) / 3;

            // check for traits effecting battle
            double battleTraits = CalcTraitEffect(Stats.BATTLE);

            // check for traits effecting siege
            double siegeTraits = CalcTraitEffect(Stats.SIEGE);

            // combine traits into single modifier 
            double combatTraits = battleTraits + siegeTraits;

            // calculate final combat rating
            armyLeaderRating += (armyLeaderRating * combatTraits);

            return armyLeaderRating;
        }

        /// <summary>
        /// Calculates chance and effect of character injuries resulting from a battle
        /// </summary>
        /// <returns>bool indicating whether character has died of injuries</returns>
        /// <param name="armyCasualtyLevel">double indicating friendly army casualty level</param>
        public bool CalculateCombatInjury(double armyCasualtyLevel)
        {
            bool isDead = false;
            uint healthLoss = 0;

            // calculate base chance of injury (based on armyCasualtyLevel)
            double injuryPercentChance = (armyCasualtyLevel * 100);

            // factor in combat trait of character
            injuryPercentChance += 5 - Combat;

            // ensure chance of injury between 1%-%80
            if (injuryPercentChance < 1)
            {
                injuryPercentChance = 1;
            }
            else if (injuryPercentChance > 80)
            {
                injuryPercentChance = 80;
            }

            // generate random percentage
            int randomPercent = Random.Shared.Next(101);

            // compare randomPercent with injuryChance to see if injury occurred
            if (randomPercent <= injuryPercentChance)
            {
                // generate random int 1-5 specifying health loss
                healthLoss = Convert.ToUInt32(Random.Shared.Next(1, 6));
            }

            // check if should create and add an ailment
            if (healthLoss > 0)
            {
                uint minEffect = 0;

                // check if character has died of injuries
                if (CalculateHealth() < healthLoss)
                {
                    isDead = true;
                }

                // if not dead, create ailment
                else
                {
                    // check if results in permanent damage
                    if (healthLoss > 4)
                    {
                        minEffect = 1;
                    }

                    Console.WriteLine("!!!--- Not creating Ailment ---!!!");
                    /*
                    // create ailment
                    Ailment myAilment = new Ailment(Globals_Game.GetNextAilmentID(),
                        "Battlefield injury",
                        Globals_Game.clock.seasons[Globals_Game.clock.currentSeason] + ", " + Globals_Game.clock.currentYear,
                        healthLoss, minEffect);
                    
                    // add to character
                    Ailments.Add(myAilment.AilmentID, myAilment);*/
                }


            }

            // =================== if is injured but not dead, create and send JOURNAL ENTRY
            if ((!isDead) && (healthLoss > 0))
            {
                // ID
                uint entryID = (uint)Random.Shared.Next(99999999);//Globals_Game.GetNextJournalEntryID();
                Console.WriteLine("!!!--- Add proper entry ID ---!!!");

                // personae
                PlayerCharacter concernedPlayer = null;
                List<string> tempPersonae = new List<string>();

                // add injured character
                tempPersonae.Add(ID + "|injuredCharacter");
                if (this is NonPlayerCharacter)
                {
                    if (!string.IsNullOrWhiteSpace(this.FamilyID))
                    {
                        concernedPlayer = ((NonPlayerCharacter)this).GetHeadOfFamily();
                        if (concernedPlayer != null)
                        {
                            tempPersonae.Add(concernedPlayer.ID + "|headOfFamily");
                        }
                    }
                }
                string[] injuryPersonae = tempPersonae.ToArray();

                // location
                string injuryLocation = Location.ID;

                // description
                string[] fields = new string[4];
                fields[0] = FirstName + " " + FamilyName;
                fields[1] = "";
                if (concernedPlayer != null)
                {
                    fields[1] = ", your " + ((NonPlayerCharacter)this).GetFunction(concernedPlayer) + ", ";
                }
                if (healthLoss > 4)
                {
                    fields[2] = "severe ";
                }
                else if (healthLoss < 2)
                {
                    fields[2] = "light ";
                }
                else
                {
                    fields[2] = "moderate ";
                }
                fields[3] = Location.Name;

                // create and send JOURNAL ENTRY
                //ProtoMessage injury = new ProtoMessage();
                //injury.ResponseType = DisplayMessages.CharacterCombatInjury;
                //injury.MessageFields = fields;
                //JournalEntry injuryEntry = new JournalEntry(entryID, Globals_Game.clock.currentYear, Globals_Game.clock.currentSeason, injuryPersonae, "injury", injury, loc: injuryLocation);

                // add new journal entry to pastEvents
                //Globals_Game.AddPastEvent(injuryEntry);
                Console.WriteLine("!!!--- Journal not being added ---!!!");
            }

            return isDead;
        }

        /// <summary>
        /// Gets the fiefs in which the character is the bailiff
        /// </summary>
        /// <returns>List containing the fiefs</returns>
        public List<Fief> GetFiefsBailiff()
        {
            throw new NotImplementedException();
            /*
            List<Fief> myFiefs = new List<Fief>();

            // get employer
            PlayerCharacter employer = null;
            if (this is PlayerCharacter)
            {
                employer = ((PlayerCharacter)this);
            }
            else if (!string.IsNullOrWhiteSpace(((NonPlayerCharacter)this).employer))
            {
                employer = ((NonPlayerCharacter)this).Employer;
            }
            else if (!string.IsNullOrWhiteSpace(FamilyID))
            {
                employer = this.GetHeadOfFamily();
            }

            if (employer != null)
            {
                // iterate through fiefs, searching for character as bailiff
                foreach (Fief thisFief in employer.OwnedFiefs)
                {
                    if (thisFief.Bailiff == this)
                    {
                        myFiefs.Add(thisFief);
                    }
                }
            }

            return myFiefs;*/
        }

        /// <summary>
        /// Gets the armies of which the character is the leader
        /// </summary>
        /// <returns>List<Army> containing the armies</returns>
        public List<Army> GetArmiesLeader()
        {
            List<Army> myArmies = new List<Army>();

            // get employer
            PlayerCharacter? employer = null;
            if (this is PlayerCharacter)
            {
                employer = ((PlayerCharacter)this);
            }
            else
            {
                employer = ((NonPlayerCharacter)this).Employer as PlayerCharacter;
            }

            if (employer != null)
            {
                // iterate through armies, searching for character as leader
                foreach (Army thisArmy in employer.MyArmies)
                {
                    if (thisArmy.Leader == this)
                    {
                        myArmies.Add(thisArmy);
                    }
                }
            }

            return myArmies;
        }


        /// <summary>
        /// Allows a character to propose marriage between himself and a female family member of another player 
        /// </summary>
        /// <returns>bool indicating whether proposal was processed successfully</returns>
        /// <param name="bride">The prospective bride</param>
        public bool ProposeMarriage(Character bride)
        {
            throw new NotImplementedException();
        }

        //TODO prettify, if have time. A few if-elses goes a long way towards readability
        /// <summary>
        /// Implements conditional checks on the character and his proposed bride prior to a marriage proposal
        /// </summary>
        /// <returns>bool indicating whether proposal can proceed</returns>
        /// <param name="bride">The prospective bride</param>
        public bool ChecksBeforeProposal(Character bride)
        {
            throw new NotImplementedException();
        }

        public bool ChecksForHeir(PlayerCharacter pc)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Moves character one hex in a random direction
        /// </summary>
        /// <returns>bool indicating success</returns>
        public bool RandomMoveNPC()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Moves character sequentially through fiefs stored in goTo queue
        /// </summary>
        /// <returns>bool indicating success</returns>
        public bool CharacterMultiMove()
        {
            throw new NotImplementedException();

        }

        /// <summary>
        /// Allows the character to remain in their current location for the specified
        /// number of days, incrementing bailiffDaysInFief if appropriate
        /// </summary>
        /// <returns>bool indicating success</returns>
        /// <param name="campDays">Number of days to camp</param>
        public bool CampWaitHere()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Allows the character to be moved along a specific route by using direction codes
        /// </summary>
        /// <param name="directions">string[] containing list of sequential directions to follow</param>
        public bool TakeThisRoute(string[] directions)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Moves the character to a specified fief using the shortest path
        /// </summary>
        /// <param name="fiefID">string containing the ID of the target fief</param>
        public bool MoveTo(Fief fief)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Spy on a fief to obtain information. Note: SpyCheck should be performed first
        /// </summary>
        /// <param name="fief">Fief to spy on</param>
        /// <param name="result"> Full details of spy result, including information if successful and spy status</param>
        /// <returns>boolean indicating spy success</returns>
        public bool SpyOn(Fief fief)
        {
            throw new NotImplementedException();   
        }

        public bool GetSpousePregnant()
        {
            throw new NotImplementedException();
        }


        // TODO use values from config
        /// <summary>
        /// Get the success chance for spying on a target
        /// </summary>
        /// <param name="target">Target to spy on- currently Fief, Character or Army</param>
        /// <returns>Chance of success</returns>
        public double GetSpySuccessChance(object target)
        {
            Type t = target.GetType();
            double baseChance;
            Character perceptiveCharacter = null;

            if (t.IsSubclassOf(typeof(Character)))
            {
                Character character = (Character)target;
                if (character == null)
                {
                    return -1;
                }
                baseChance = 40;
                perceptiveCharacter = character;
            }
            else if (t == typeof(Fief))
            {
                Fief fief = (Fief)target;
                if (fief == null)
                {
                    return -1;
                }
                baseChance = 40;
                if (fief.Bailiff != null)
                {
                    perceptiveCharacter = fief.Bailiff;
                }
            }
            else if (t == typeof(Army))
            {
                Army army = (Army)target;
                if (army == null)
                {
                    return -1;
                }

                if (army.Leader != null)
                {
                    perceptiveCharacter = army.Leader;
                }
                baseChance = 30;
            }
            else
            {
                return -1;
            }
            double stealth = CalcTraitEffect(Stats.STEALTH);
            double enemyPerception = 0;
            if (perceptiveCharacter != null)
            {
                enemyPerception = perceptiveCharacter.CalcTraitEffect(Stats.PERCEPTION);
            }
            return baseChance + ((stealth - enemyPerception) * 100);
        }

        public bool SpyCheck(Character character)
        {
            throw new NotImplementedException();
        }


        public bool SpyCheck(Fief fief)
        {
            throw new NotImplementedException();
        }

        public bool SpyCheck(Army army)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Spy on a character to gain additional information. Note: SpyCheck should be performed first
        /// </summary>
        /// <param name="character">Character to spy on</param>
        /// <param name="result">Returns protomessage containing the full spy result and any information gained</param>
        /// <returns>Bool indicating spy success</returns>
        public bool SpyOn(Character character)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Spy on an army to obtain information. Note: SpyCheck should be performed first
        /// </summary>
        /// <param name="army">Army to spy on</param>
        /// <param name="result">Result of spying, including additional information obtained</param>
        /// <returns>Bool for success</returns>
        /// <summary>
        /// Spy on an army to obtain information. Note: SpyCheck should be performed first
        /// </summary>
        /// <param name="army">Army to spy on</param>
        /// <param name="result">Result of spying, including additional information obtained</param>
        /// <returns>Bool for success</returns>
        public bool SpyOn(Army army)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Kidnap a character
        /// </summary>
        /// <param name="target">Character to kidnap</param>
        /// <param name="result">Result of kidnapping attempt or any errors</param>
        /// <returns>Success</returns>
        public bool Kidnap(Character target)
        {
            throw new NotImplementedException();
        }

        public bool ChecksBeforeGranting()
        {
            throw new NotImplementedException();
        }

        public bool ExitEnterKeep()
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Calculates how much this character can be ransomed for
        /// </summary>
        /// <returns>ransom amount</returns>
        public uint CalculateRansom()
        {
            uint ransom = 0;
            if (this is PlayerCharacter)
            {
                // calculate ransom (10% of total GDP)
                ransom = Convert.ToUInt32((((PlayerCharacter)this).GetTotalGDP() * 0.1));
            }
            else
            {
                string thisFunction = ((NonPlayerCharacter)this).GetFunction(GetPlayerCharacter());
                ransom = Convert.ToUInt32(((NonPlayerCharacter)this).CalcFamilyAllowance(thisFunction));
            }
            return ransom;
        }

        public double GetPathCost(Fief destination, HexMapGraph mapGraph)
        {
            Queue<Fief> path = mapGraph.GetShortestPath(Location, destination);
            double pathCost = 0;
            Fief currentFief = path.Dequeue();
            while (path.Count > 0)
            {
                Fief nextFief = path.Dequeue();
                pathCost += currentFief.getTravelCost(nextFief, ArmyID);
                currentFief = nextFief;
            }
            return pathCost;
        }

        /// <summary>
        ///     Search the possible directions the character can move to from its current location
        /// </summary>
        /// <returns>Array of all the possible directions</returns>
        public string[] FindAvailableTravelDirections(HexMapGraph mapGraph)
        {
            string[] correctDirections = new string[6] { "E", "W", "SE", "SW", "NE", "NW" };
            List<string> availableTravelDirections = new List<string>();
            foreach (string direction in correctDirections)
                if (mapGraph.GetFief(Location, direction) != null)
                    availableTravelDirections.Add(direction);
            return availableTravelDirections.ToArray();
        }

        public string FullName()
        {
            return FirstName + " " + FamilyName;
        }

        public bool Equals(Character other)
        {
            return ID.Equals(other.ID);
        }
    }

    

    
}
