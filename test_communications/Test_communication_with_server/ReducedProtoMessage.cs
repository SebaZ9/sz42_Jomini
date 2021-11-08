using ProtoMessageClient;
using System;
using System.Collections.Generic;

/// <summary>
///     A lighter version of the Protomessage that contains only the needed information and with as little ProtoMessage variables 
///     e.g. the ProtoCharacter variables are generally replaced by a string containing the charID
/// </summary>
namespace JominiAI
{
    /// <summary>
    /// Class for serializing an Army
    /// The amount of information a player can view about an army depends on whether that player 
    /// ownes the army, how close the player is etc. 
    /// Can be tuned later to include information obtained via methods such as spying, interrogation, or defection
    /// </summary>
    public class RedProtoArmy
    {
        /// <summary>
        /// Holds army ID
        /// </summary>
        public String armyID { get; set; }
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
        public uint[] troops;
        /// <summary>
        /// Holds army leader name
        /// </summary>
        //public string leader { get; set; }
        /// <summary>
        /// Holds army leader ID
        /// </summary>
        public string leaderName { get; set; }
        /// <summary>
        /// Holds army owner name
        /// </summary>
        //public string owner { get; set; }
        /// <summary>
        /// Gets or sets the owner's character id
        /// </summary>
        public string ownerName { get; set; }
        /// <summary>
        /// Holds army's remaining days in season
        /// </summary>
        public double days { get; set; }
        /// <summary>
        /// Holds army location in the format:
        /// fiefID|fiefName|provinceName|kingdomName
        /// </summary>
        public string location { get; set; }
        /// <summary>
        /// Indicates whether army is being actively maintained by owner
        /// </summary>
        public bool isMaintained { get; set; }
        /// <summary>
        /// Indicates the army maintenance cost
        /// </summary>
        public uint maintCost { get; set; }
        /// <summary>
        /// Indicates army's aggression level (automated response to combat)
        /// </summary>
        //public byte aggression { get; set; }
        /// <summary>
        /// Indicates army's combat odds value (i.e. at what odds will attempt automated combat action)
        /// </summary>
        //public byte combatOdds { get; set; }
        /// <summary>
        /// String indicating army nationality
        /// </summary>
        //public string nationality { get; set; }
        /// <summary>
        /// Holds siege status of army
        /// One of BESIEGING, BESIEGED, FIEF or none
        /// BESIEGING: army is currently besieging fief
        /// BESIEGED: army is under siege
        /// FIEF: the fief the army is in is under siege
        /// </summary>
        public string siegeStatus { get; set; }
        public RedProtoArmy(ProtoArmy a)
        {
            this.ownerName = a.owner;
            this.armyID = a.armyID;
            //this.leader = a.leader;
            this.leaderName = a.leader;
            //this.owner = a.owner;
            this.location = a.location;
            //this.nationality = a.nationality;
            this.siegeStatus = a.siegeStatus;
            this.troops = a.troops;

            // IncludeAll
            //this.aggression = a.aggression;
            //this.combatOdds = a.combatOdds;
            this.troops = a.troops;
            this.days = a.days;
            this.isMaintained = a.isMaintained;
            this.maintCost = a.maintCost;
        }

        public RedProtoArmy() : base() { }
    }
    /// <summary>
    /// Class for sending details of a character
    /// </summary>
    public class RedProtoCharacter
    {
        /* BASIC CHARACTER DETAILS */
        /// <summary>
        /// Holds character ID
        /// </summary>
        public string charID { get; set; }
        /// <summary>
        /// Holds character's first name
        /// </summary>
        public string firstName { get; set; }
        /// <summary>
        /// Holds character's family name
        /// </summary>
        public string familyName { get; set; }
        /// <summary>
        /// Character's year of birth
        /// </summary>
        public uint birthYear { get; set; }
        /// <summary>
        /// Character's birth season
        /// </summary>
        //public byte birthSeason { get; set; }
        /// <summary>
        /// Holds if character male
        /// </summary>
        public bool isMale { get; set; }
        /// <summary>
        /// Holds the string representation of this character's nationality
        /// </summary>
        public string nationality { get; set; }
        /// <summary>
        /// Indicates whether a character is alive
        /// </summary>
        public bool isAlive { get; set; }
        /// <summary>
        /// Character's max health
        /// </summary>
        //public double maxHealth { get; set; }
        /// <summary>
        /// Character's current health
        /// </summary>
        public double health { get; set; }
        /// <summary>
        /// Character's stature
        /// </summary>
        //public double stature { get; set; }
        /// <summary>
        /// Character's virility
        /// </summary>
        //public double virility { get; set; }
        /// <summary>
        /// Bool detclaring whether character is in keep
        /// </summary>
        public bool inKeep { get; set; }
        /// <summary>
        /// Character's language ID
        /// </summary>
        public string language { get; set; }
        /// <summary>
        /// number of days left in season
        /// </summary>
        public double days { get; set; }
        /// <summary>
        /// Character's family ID
        /// </summary>
        public String familyID { get; set; }
        /// <summary>
        /// Character spouse charID
        /// </summary>
        public String spouse { get; set; }
        /// <summary>
        /// Character father charID
        /// </summary>
        public String father { get; set; }
        /// <summary>
        /// Character mother charID
        /// </summary>
        //public String mother { get; set; }
        /// <summary>
        /// Character mother charID
        /// </summary>
        public string fiancee { get; set; }
        /// <summary>
        /// Character location (FiefID)
        /// </summary>
        public string location { get; set; }
        /// <summary>
        /// Character statureModifier
        /// </summary>
        //public double statureModifier { get; set; }
        /// <summary>
        /// Character management rating
        /// </summary>
        public double management { get; set; }
        /// <summary>
        /// Character combat skill
        /// </summary>
        public double combat { get; set; }
        /// <summary>
        /// Holds character's traits
        /// </summary>
        //public Pair[] traits { get; set; }
        /// <summary>
        /// Bool to indicate whether char is pregnant
        /// </summary>
        public bool isPregnant { get; set; }
        /// <summary>
        /// Holds char's title
        /// </summary>
        public string[] titles { get; set; }
        /// <summary>
        /// ArmyID, if char leads army
        /// </summary>
        public string armyID { get; set; }
        /// <summary>
        /// Character's ailments
        /// </summary>
        //public Pair[] ailments { get; set; }
        /// <summary>
        /// IDs of Fiefs in char's GoTo list
        /// </summary>
        //public string[] goTo { get; set; }
        /// <summary>
        /// Holds name of captor (if is null character is not captive)
        /// </summary>
        public string captor { get; set; }
        // Holds information as to whether character is involved in a siege
        //public enum SiegeRole { None = 0, Besieger, Defender, DefenderAdd };
        //public SiegeRole siegeRole;
        public RedProtoCharacter(ProtoCharacter c)
        {

            this.charID = c.charID;
            this.firstName = c.firstName;
            this.familyName = c.familyName;
            this.birthYear = c.birthYear;
            //this.birthSeason = c.birthSeason;
            this.isMale = c.isMale;
            this.nationality = c.nationality;
            this.isAlive = c.isAlive;
            this.language = c.language;
            this.familyID = c.familyID;
            this.spouse = c.spouse;
            this.father = c.father;
            //this.mother = c.mother;
            this.fiancee = c.fiancee;
            this.isPregnant = c.isPregnant;
            this.titles = c.titles;
            this.armyID = c.armyID;
            //this.siegeRole = c.siegeRole;
            this.captor = c.captor;

            //IncludeAll
            this.inKeep = c.inKeep;
            //this.virility = c.virility;
            //this.maxHealth = c.maxHealth;
            this.health = c.health;
            //this.stature = c.stature;
            this.days = c.days;
            this.location = c.location;
            //this.statureModifier = c.statureModifier;
            this.management = c.management;
            this.combat = c.combat;
            //this.traits = c.traits;
            //this.ailments = c.ailments;
            //this.goTo = c.goTo;
            this.captor = c.captor;
        }

        public RedProtoCharacter() { }

    }

    /// <summary>
    /// Class for sending details of a PlayerCharacter
    /// </summary>
        public class RedProtoPlayerCharacter : RedProtoCharacter
    {
        /// <summary>
        /// Holds ID of player who is currently playing this PlayerCharacter
        /// Note that list of sieges and list of armies is stored elsewhere- see ProtoSiegeList and ProtoArmyList
        /// </summary>
        public string playerID { get; set; }
        /// <summary>
        /// Holds character outlawed status
        /// </summary>
        //public bool outlawed { get; set; }
        /// <summary>
        /// Holds character's treasury
        /// </summary>
        public uint purse { get; set; }
        /// <summary>
        /// Holds IDs and names of character's employees and family
        /// </summary>
        //public ProtoCharacterOverview[] myNPCs { get; set; }
        /// <summary>
        /// Holds details of heir
        /// </summary>
        public string myHeirId { get; set; }
        /// <summary>
        /// Holds IDs of character's owned fiefs
        /// </summary>
        public string[] ownedFiefIds { get; set; }
        /// <summary>
        /// Holds IDs of character's owned provinces
        /// </summary>
        //public string[] provinces { get; set; }
        /// <summary>
        /// Holds character's home fief (fiefID)
        /// </summary>
        public String homeFief { get; set; }
        /// <summary>
        /// Holds character's ancestral home fief (fiefID)
        /// </summary>
        public String ancestralHomeFief { get; set; }

        public RedProtoPlayerCharacter(ProtoPlayerCharacter pc) : base(pc)
        {
            this.homeFief = pc.homeFief;
            this.ancestralHomeFief = pc.ancestralHomeFief;
            //this.outlawed = pc.outlawed;

            // IncludeAll
            this.playerID = pc.playerID;
            this.purse = pc.purse;
            //this.myNPCs = pc.myNPCs;
            this.myHeirId = pc.myHeir != null ? pc.myHeir.charID : null;
            if (pc.ownedFiefs != null)
            {
                this.ownedFiefIds = new string[pc.ownedFiefs.Length];
                for (int i = 0; i < this.ownedFiefIds.Length; i++)
                    this.ownedFiefIds[i] = pc.ownedFiefs[i];
            }
            else
                this.ownedFiefIds = null;
            //this.provinces = pc.provinces;
        }

        public RedProtoPlayerCharacter() : base(){}

    }
    /// <summary>
    /// Class for sending details of Non-PlayerCharacter
    /// </summary>
    public class RedProtoNPC : RedProtoCharacter
    {
        /// <summary>
        /// Holds NPC's employer (charID)
        /// </summary>
        public string employerId { get; set; }
        /// <summary>
        /// Holds NPC's salary
        /// </summary>
        public uint salary { get; set; }
        /// <summary>
        /// Holds last wage offer from individual PCs
        /// </summary>
        // public string lastOfferID { get; set; }
        public uint lastOfferAmount { get; set; }
        /// <summary>
        /// Denotes if in employer's entourage
        /// </summary>
        public bool inEntourage { get; set; }
        /// <summary>
        /// Denotes if is player's heir
        /// </summary>
        public bool isHeir { get; set; }

        public RedProtoNPC(ProtoNPC npc) : base(npc)
        {
            this.employerId = npc.employer != null ? npc.employer.charID : null;
            this.salary = npc.salary;
            this.inEntourage = npc.inEntourage;
            this.isHeir = npc.isHeir;

            // IncludeHire
            //this.lastOfferID = npc.lastOfferID;
            this.lastOfferAmount = npc.lastOfferAmount;
        }

        public RedProtoNPC() : base() { }
    }

    /// <summary>
    /// Class for summarising the basic details of a character
    /// Can be used in conjunction with byte arrays to create a list of characters
    /// </summary>
    public class RedProtoCharacterOverview
    {
        // Contains character ID
        public string charID { get; set; }
        // Contains name of owning playercharacter
        public string ownerName { get; set; }
        // Contains character name (first name + delimiter + family name)
        //public string charName { get; set; }
        // Contains character role or function (son, wife, employee etc)
        //public string role { get; set; }
        // Containst character nationality ID
        //public string natID { get; set; }
        // Contains location ID
        public string locationID { get; set; }
        // boolean for character gender
        //public bool isMale { get; set; }

        public RedProtoCharacterOverview()
            : base()
        {
        }
        public RedProtoCharacterOverview(ProtoCharacterOverview c)
        {
            this.charID = c.charID;
            this.ownerName = c.owner;
            this.locationID = c.locationID;
        }
    }

    /// <summary>
    /// Class for sending fief details
    /// Province, language and terrain are all stored client side- unless this changes there is no need to send these
    /// </summary>
    public class RedProtoFief
    {
        /// <summary>
        /// ID of the fief
        /// </summary>
        public string fiefID { get; set; }
        /// <summary>
        /// CharID and name of fief title holder
        /// </summary>
        //public string titleHolder { get; set; }
        /// <summary>
        /// Name of fief owner
        /// </summary>
        //public string owner { get; set; }
        /// <summary>
        /// CharID of the fief owner
        /// </summary>
        public string ownerID { get; set; }
        /// <summary>
        /// Fief rank
        /// </summary>
        //public string rank { get; set; }
        /// <summary>
        /// Holds fief population
        /// </summary>
        //public int population { get; set; }
        /// <summary>
        /// Holds fief field level
        /// </summary>
        //public double fields { get; set; }
        /// <summary>
        /// Holds fief industry level
        /// </summary>
        //public double industry { get; set; }
        /// <summary>
        /// Holds number of troops in fief
        /// </summary>
        //public uint troops { get; set; }
        /// <summary>
        /// Holds number of troops that can be recruited in this fief
        /// </summary>
        public int militia { get; set; }
        /// <summary>
        /// Holds fief tax rate
        /// </summary>
        //public double taxRate { get; set; }
        /// <summary>
        /// Holds fief tax rate (next season)
        /// </summary>
        //public double taxRateNext { get; set; }
        /// <summary>
        /// Holds expenditure on officials (next season)
        /// </summary>
        //public uint officialsSpendNext { get; set; }
        /// <summary>
        /// Holds expenditure on garrison (next season)
        /// </summary>
        //public uint garrisonSpendNext { get; set; }
        /// <summary>
        /// Holds expenditure on infrastructure (next season)
        /// </summary>
        //public uint infrastructureSpendNext { get; set; }
        /// <summary>
        /// Holds expenditure on keep (next season)
        /// </summary>
        //public uint keepSpendNext { get; set; }
        /// <summary>
        /// Holds key data for current season.
        /// 0 = loyalty,
        /// 1 = GDP,
        /// 2 = tax rate,
        /// 3 = official expenditure,
        /// 4 = garrison expenditure,
        /// 5 = infrastructure expenditure,
        /// 6 = keep expenditure,
        /// 7 = keep level,
        /// 8 = income,
        /// 9 = family expenses,
        /// 10 = total expenses,
        /// 11 = overlord taxes,
        /// 12 = overlord tax rate,
        /// 13 = bottom line
        /// </summary>
        //public double[] keyStatsCurrent;
        /// <summary>
        /// Holds key data for previous season
        /// </summary>
        //public double[] keyStatsPrevious;
        /// <summary>
        /// Holds key data for next season
        /// </summary>
        //public double[] keyStatsNext;
        /// <summary>
        /// Holds fief keep level
        /// </summary>
        public double keepLevel { get; set; }
        /// <summary>
        /// Holds fief loyalty
        /// </summary>
        //public double loyalty { get; set; }
        /// <summary>
        /// Holds fief status (calm, unrest, rebellion)
        /// </summary>
        public char status { get; set; }
        /// <summary>
        /// Holds overviews of characters present in fief
        /// </summary>
        public string[] characterInFiefIDs { get; set; }
        /// <summary>
        /// Holds characters banned from keep (charIDs)
        /// </summary>
        public string[] barredCharactersId { get; set; }
        /// <summary>
        /// Holds nationalities banned from keep (IDs)
        /// </summary>
        public string[] barredNationalities { get; set; }
        /// <summary>
        /// Holds fief ancestral owner (PlayerCharacter object)
        /// </summary>
        public string ancestralOwnerId { get; set; }
        /// <summary>
        /// Holds fief bailiff (Character object)
        /// </summary>
        public string bailiffID { get; set; }
        /// <summary>
        /// Number of days the bailiff has been resident in the fief (this season)
        /// </summary>
        //public Double bailiffDaysInFief { get; set; }
        /// <summary>
        /// Holds fief treasury
        /// </summary>
        public int treasury { get; set; }
        /// <summary>
        /// Holds overviews of armies present in the fief (armyIDs)
        /// </summary>
        public string[] armyIDs { get; set; }
        /// <summary>
        /// Identifies if recruitment has occurred in the fief in the current season
        /// </summary>
        public bool hasRecruited { get; set; }
        /// <summary>
        /// Identifies if pillage has occurred in the fief in the current season
        /// </summary>
        public bool isPillaged { get; set; }
        /// <summary>
        /// Siege (siegeID) of active siege
        /// </summary>
        public String siege { get; set; }
        /// <summary>
        /// List of characters held captive in fief
        /// </summary>
        //public ProtoCharacterOverview[] gaol { get; set; }
        public RedProtoFief(ProtoFief f)
        {
            this.militia = f.militia;
            this.fiefID = f.fiefID;
            //this.titleHolder = f.titleHolder;
            //this.owner = f.owner;
            this.ownerID = f.ownerID;
            //this.rank = f.rank;
            //this.population = f.population;
            this.status = f.status;
            if (f.barredCharacters != null)
            {
                this.barredCharactersId = new string[f.barredCharacters.Length];
                for (int i = 0; i < this.barredCharactersId.Length; i++)
                {
                    this.barredCharactersId[i] = f.barredCharacters[i].charID;
                }
            }
            else
                this.barredCharactersId = null;
            this.barredNationalities = f.barredNationalities;
            if (f.charactersInFief != null)
            {
                this.characterInFiefIDs = new string[f.charactersInFief.Length];
                for (int i = 0; i < this.characterInFiefIDs.Length; i++)
                {
                    this.characterInFiefIDs[i] = f.charactersInFief[i].charID;
                }
            }
            else
                this.characterInFiefIDs = null;
            this.ancestralOwnerId = f.ancestralOwner != null ? f.ancestralOwner.charID : null;
            //this.bailiff = f.bailiff;
            this.isPillaged = f.isPillaged;
            this.siege = f.siege;
            if (f.armies != null)
            {
                this.armyIDs = new string[f.armies.Length];
                for (int i = 0; i < this.armyIDs.Length; i++)
                {
                    this.armyIDs[i] = f.armies[i].armyID;
                }
            }
            else
                this.armyIDs = null;
            this.hasRecruited = f.hasRecruited;
            this.bailiffID = f.bailiff != null ? f.bailiff.charID : null;

            // IncludeAll
            //this.keyStatsCurrent = new double[14];
            //this.keyStatsNext = new double[14]; ;
            //this.keyStatsPrevious = new double[14];
            // this.fields = f.fields;
            //this.industry = f.industry;
            //this.troops = f.troops;
            //this.taxRate = f.taxRate;
            this.keepLevel = f.keepLevel;
            //this.keyStatsNext = f.keyStatsNext;

            //this.keyStatsCurrent = f.keyStatsCurrent;
            //this.keyStatsPrevious = f.keyStatsPrevious;
            //this.loyalty = f.loyalty;
            //this.bailiffDaysInFief = f.bailiffDaysInFief;

            this.treasury = f.treasury;
        }

        public RedProtoFief() : base() { }
    }

    public class RedProtoSiegeDisplay
    {
        /// <summary>
        /// Holds siege ID
        /// </summary>
        public String siegeID { get; set; }
        /// <summary>
        /// Holds year the siege started
        /// </summary>
        //public uint startYear { get; set; }
        /// <summary>
        /// Holds season the siege started
        /// </summary>
        //public byte startSeason { get; set; }
        /// <summary>
        /// Holds besieging player
        /// </summary>
        public string besiegingPlayer { get; set; }
        /// <summary>
        /// Holds defending player
        /// </summary>
        public string defendingPlayer { get; set; }
        /// <summary>
        /// Holds besieging army (armyID)
        /// </summary>
        public string besiegerArmyID { get; set; }
        /// <summary>
        /// Holds defending garrison (armyID)
        /// </summary>
        public string defenderGarrisonID { get; set; }
        /// <summary>
        /// Holds fief being besieged (fiefID)
        /// </summary>
        public String besiegedFiefID { get; set; }
        /// <summary>
        /// Holds days left in current season
        /// </summary>
        public double days { get; set; }
        /// <summary>
        /// Holds the keep level at the start of the siege
        /// </summary>
        //public double startKeepLevel { get; set; }
        /// <summary>
        /// Holds current keep level
        /// </summary>
        public double keepLevel { get; set; }
        /// <summary>
        /// Casualties for attacker this round
        /// </summary>
        //public int casualtiesAttacker { get; set; }
        /// <summary>
        /// Casualties for defender this round
        /// </summary>
        //public int casualtiesDefender { get; set; }
        /// <summary>
        /// Total casualties this siege suffered by attacker
        /// </summary>
        //public int totalCasualtiesAttacker { get; set; }
        /// <summary>
        /// Holds total casualties suffered so far by defender
        /// </summary>
        //public int totalCasualtiesDefender { get; set; }
        /// <summary>
        /// Holds total duration of siege so far (days)
        /// </summary>
        //public double totalDays { get; set; }
        /// <summary>
        /// Holds additional defending army 
        /// </summary>
        public string defenderAdditional { get; set; }
        /// <summary>
        /// Holds season and year the siege ended
        /// </summary>
        public String endDate { get; set; }

        public string[] captivesTaken { get; set; }

        //public int totalRansom { get; set; }

        public bool besiegerWon { get; set; }
        public int lootLost { get; set; }

        public RedProtoSiegeDisplay(ProtoSiegeDisplay s)
        {
            siegeID = s.siegeID;
            //startYear = s.startYear;
            //startSeason = s.startSeason;
            besiegingPlayer = s.besiegingPlayer;
            defendingPlayer = s.defendingPlayer;
            besiegerArmyID = s.besiegerArmy;
            defenderGarrisonID = s.defenderGarrison;
            besiegedFiefID = s.besiegedFief;
            days = s.days;
            //totalDays = s.totalDays;
            //startKeepLevel = s.startKeepLevel;
            keepLevel = s.keepLevel;
            //totalCasualtiesAttacker = casualtiesAttacker;
            //totalCasualtiesDefender = totalCasualtiesDefender;
            defenderAdditional = s.defenderAdditional;
            endDate = s.endDate;
            captivesTaken = s.captivesTaken;
            besiegerWon = s.besiegerWon;
            lootLost = s.lootLost;
        }

        /*public RedProtoSiegeDisplay(ProtoSiegeOverview protoSiegeOverview)
        {
            siegeID = protoSiegeOverview.siegeID;
            besiegingPlayer = protoSiegeOverview.besiegingPlayer;
            defendingPlayer = protoSiegeOverview.defendingPlayer;
            besiegedFief = protoSiegeOverview.besiegedFief;
        }*/

        public RedProtoSiegeDisplay() : base() { }
    }

    public class RedProtoJournalEntry
    {
        /// <summary>
        /// Holds JournalEntry ID
        /// </summary>
        public uint jEntryID { get; set; }
        /// <summary>
        /// Holds event year
        /// </summary>
        //public uint year { get; set; }
        /// <summary>
        /// Holds event season
        /// </summary>
        //public byte season { get; set; }
        /// <summary>
        /// Holds characters associated with event and their role
        /// </summary>
        public string[] personaeIds { get; set; }
        /// <summary>
        /// Holds type of event (e.g. battle, birth)
        /// </summary>
        public String type { get; set; }
        /// <summary>
        /// Holds location of event (fiefID)
        /// </summary>
        public String location { get; set; }
        /// <summary>
        /// Indicates whether entry has been viewed
        /// </summary>
        public bool viewed { get; set; }
        /// <summary>
        /// Indicates whether entry has been replied to (e.g. for Proposals)
        /// </summary>
        public bool replied { get; set; }
        /// <summary>
        /// Holds ProtoMessage containing details of event. More flexible than strings.
        /// </summary>
        //public ProtoMessage eventDetails { get; set; }
        public RedProtoJournalEntry(ProtoJournalEntry j)
        {
            this.jEntryID = j.jEntryID;
            //this.year = j.year;
            //this.season = j.season;
            this.type = j.type;
            this.location = j.location;
            //this.eventDetails = j.eventDetails;
            this.viewed = j.viewed;
            this.replied = j.replied;
            if (j.personae != null)
            {
                this.personaeIds = new string[j.personae.Length];
                for (int i = 0; i < this.personaeIds.Length; i++)
                    this.personaeIds[i] = j.personae[i].charID;
            }
            else
                this.personaeIds = null;
        }

        public RedProtoJournalEntry() : base() { }
    }

    /// <summary>
    /// Class for sending details of a detachment
    /// Character ID of PlayerCharacter leaving detachment is obtained via connection details
    /// </summary>
    public class RedProtoDetachment
    {
        // ID of detachment
        public string id { get; set; }
        /// <summary>
        /// Array of troops (size = 7)
        /// </summary>
        public uint[] troops;
        /// <summary>
        /// Character detachment is left for
        /// </summary>
        public string leftFor { get; set; }
        /// <summary>
        /// ArmyID of army from which detachment was created
        /// </summary>
        //public string armyID { get; set; }
        /// <summary>
        /// Details of person who left this detachment (used in sending details of detachments to client)
        /// </summary>
        public string leftBy { get; set; }
        /// <summary>
        /// Days left of person who created detachment at time of creation
        /// </summary>
        public int days { get; set; }

        public RedProtoDetachment(ProtoDetachment protoDetachment)
        {
            this.id = protoDetachment.id;
            this.troops = protoDetachment.troops;
            this.leftFor = protoDetachment.leftFor;
            this.leftBy = protoDetachment.leftBy;
            //this.armyID = protoDetachment.armyID;
        }

        public RedProtoDetachment() : base() { }
    }

    /// <summary>
    /// Class summarising player information (for use in listing players)
    /// </summary>
    public class RedProtoPlayer
    {
        public string pcID;
        //public string pcName;
        public string playerID;
        //public string natID;

        public RedProtoPlayer(ProtoPlayer protoPlayer)
        {
            this.pcID = protoPlayer.pcID;
            //this.pcName = protoPlayer.pcName;
            this.playerID = protoPlayer.playerID;
            //this.natID = protoPlayer.natID;
        }

        public RedProtoPlayer() : base() { }
    }
}
