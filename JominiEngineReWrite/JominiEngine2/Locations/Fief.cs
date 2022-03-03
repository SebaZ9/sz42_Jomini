using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace JominiGame
{
    public class Fief : Place
    {
        /// <summary>
        /// Holds fief's Province object
        /// </summary>
        public Province FiefsProvince { get; set; }
        /// <summary>
        /// Holds fief population
        /// </summary>
        public int Population { get; set; }
        /// <summary>
        /// Holds fief field level
        /// </summary>
        public double Fields { get; set; }
        /// <summary>
        /// Holds fief industry level
        /// </summary>
        public double Industry { get; set; }
        /// <summary>
        /// Holds number of troops in fief
        /// </summary>
        public uint Troops { get; set; }
        /// <summary>
        /// Holds fief tax rate
        /// </summary>
        public double TaxRate { get; set; }
        /// <summary>
        /// Holds fief tax rate (next season)
        /// </summary>
        public double TaxRateNext { get; set; }
        /// <summary>
        /// Holds expenditure on officials (next season)
        /// </summary>
        public uint OfficialSpendNext { get; set; }
        /// <summary>
        /// Holds expenditure on garrison (next season)
        /// </summary>
        public uint GarrisonSpendNext { get; set; }
        /// <summary>
        /// Holds expenditure on infrastructure (next season)
        /// </summary>
        public uint InfrastructureSpendNext { get; set; }
        /// <summary>
        /// Holds expenditure on keep (next season)
        /// </summary>
        public uint KeepSpendNext { get; set; }
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
        public double[] KeyStatsCurrent = new double[14];
        /// <summary>
        /// Holds key data for previous season
        /// </summary>
        public double[] KeyStatsPrevious = new double[14];
        /// <summary>
        /// Holds fief keep level
        /// </summary>
        public double KeepLevel { get; set; }
        /// <summary>
        /// Holds fief loyalty
        /// </summary>
        public double Loyalty { get; set; }
        /// <summary>
        /// Holds fief status (calm, unrest, rebellion)
        /// </summary>
        public char Status { get; set; }
        /// <summary>
        /// Holds fief language and dialect
        /// </summary>
        public Language FiefsLanguage { get; set; }
        /// <summary>
        /// Holds terrain object
        /// </summary>
        public Terrain FiefsTerrain { get; set; }
        /// <summary>
        /// Holds characters present in fief
        /// </summary>
        public List<Character> CharactersInFief = new List<Character>();
        /// <summary>
        /// Stores captives in this fief
        /// </summary>
        public List<Character> Gaol = new List<Character>();
        /// <summary>
		/// Holds characters banned from keep (charIDs)
        /// </summary>
        public List<string> BarredCharacters = new List<string>();
        /// <summary>
        /// Holds nationalitie banned from keep (IDs)
        /// </summary>
        public List<string> BarredNationalities = new List<string>();
        /// <summary>
        /// Holds fief ancestral owner (PlayerCharacter object)
        /// </summary>
		public PlayerCharacter AncestralOwner { get; set; }
        /// <summary>
        /// Holds fief bailiff (Character object)
        /// </summary>
        public Character? Bailiff { get; set; }
        /// <summary>
        /// Number of days the bailiff has been resident in the fief (this season)
        /// </summary>
        public double BailiffDaysInFief { get; set; }
        /// <summary>
        /// Holds fief treasury
        /// </summary>
        public int Treasury;
        /// <summary>
        /// Holds armies present in the fief (armyIDs)
        /// </summary>
        public List<Army> Armies = new List<Army>();
        /// <summary>
        /// Identifies if recruitment has occurred in the fief in the current season
        /// </summary>
        public bool HasRecruited { get; set; }
        /// <summary>
        /// Identifies if pillage has occurred in the fief in the current season
        /// </summary>
        public bool IsPillaged { get; set; }
        /// <summary>
        /// Holds troop detachments in the fief awaiting transfer
        /// string[] contains from (charID), to (charID), troop numbers (each type), days left when detached
        /// </summary>
        public Dictionary<string, Detachment> TroopTransfers = new Dictionary<string, Detachment>();
        /// <summary>
        /// Siege (siegeID) of active siege
        /// </summary>
        public Siege CurrentSiege { get; set; }

        /**************LOCKS ***************/

        /// <summary>
        /// Create a new object to use as a treasury lock between transfers
        /// </summary>
        private object TreasuryTransferLock = new object();
        /// <summary>
        /// Create lock for manipulating treasury
        /// </summary>
        private object TreasuryLock = new object();
        /// <summary>
        /// Constructor for Fief
        /// </summary>
        /// <param name="id">string holding fief ID</param>
        /// <param name="nam">string holding fief name</param>
        /// <param name="own">PlayerCharacter holding fief owner</param>
        /// <param name="r">Fief's rank object</param>
        /// <param name="tiHo">Fief title holder (charID)</param>
        /// <param name="prov">Fief's Province object</param>
        /// <param name="Population">uint holding fief population</param>
        /// <param name="Fields">double holding fief field level</param>
        /// <param name="Fields">double holding fief industry level</param>
        /// <param name="Troops">uint holding no. of troops in fief</param>
        /// <param name="TaxRate">double holding fief tax rate</param>
        /// <param name="TaxRateNext">double holding fief tax rate (next season)</param>
        /// <param name="OfficialSpendNext">uint holding officials expenditure (next season)</param>
        /// <param name="GarrisonSpendNext">uint holding garrison expenditure (next season)</param>
        /// <param name="InfrastructureSpendNext">uint holding infrastructure expenditure (next season)</param>
        /// <param name="KeepSpendNext">uint holding keep expenditure (next season)</param>
        /// <param name="KeyStatsCurrent">double [] holding financial data for current season</param>
        /// <param name="KeyStatsPrevious">double [] holding financial data for previous season</param>
        /// <param name="KeepLevel">double holding fief keep level</param>
        /// <param name="Loyalty">double holding fief loyalty rating</param>
        /// <param name="Status">char holding fief status</param>
        /// <param name="FiefsLanguage">Language object holding language and dialect</param>
        /// <param name="FiefsTerrain">Terrain object for fief</param>
        /// <param name="CharactersInFief">List holding characters present in fief</param>
        /// <param name="BarredCharacters">List holding IDs of characters barred from keep</param>
        /// <param name="BarredNationalities">List holding IDs of nationalities barred from keep</param>
        /// <param name="ancOwn">PlayerCharacter holding fief ancestral owner</param>
        /// <param name="bail">Character holding fief bailiff</param>
        /// <param name="BailiffDaysInFief">byte holding days bailiff in fief</param>
        /// <param name="Treasury">int containing fief treasury</param>
        /// <param name="Armies">List holding IDs of armies present in fief</param>
        /// <param name="HasRecruited">bool indicating whether recruitment has occurred in the fief (current season)</param>
        /// <param name="IsPillaged">bool indicating whether pillage has occurred in the fief (current season)</param>
        /// <param name="TroopTransfers">Dictionary &lt string, string[] &rt containing troop detachments in the fief awaiting transfer</param>
        /// <param name="CurrentSiege">string holding siegeID of active siege</param>
        public Fief(string ID, string Name, Character TitleHolder, PlayerCharacter Owner, Rank PlaceRank, Province FiefsProvince, int Population,
            double Fields, double Industry, uint Troops, double TaxRate, double TaxRateNext, uint OfficialSpendNext, uint GarrisonSpendNext,
            uint InfrastructureSpendNext, uint KeepSpendNext, double[] KeyStatsCurrent, double[] KeyStatsPrevious,
            double KeepLevel, double Loyalty, char Status, Language FiefsLanguage, Terrain FiefsTerrain, List<Character> CharactersInFief,
            List<string> BarredCharacters, List<string> BarredNationalities, double BailiffDaysInFief, int Treasury, List<Army> Armies,
            bool HasRecruited, Dictionary<string, Detachment> TroopTransfers, bool IsPillaged, GameClock Clock, IdGenerator IDGen, HexMapGraph GameMap, PlayerCharacter ancOwn = null,
            Character bail = null, Siege CurrentSiege = null)
            : base(ID, Name, TitleHolder, Owner, PlaceRank, Clock, IDGen, GameMap)
        {
            this.FiefsProvince = FiefsProvince;
            this.Population = Population;
            this.AncestralOwner = ancOwn;
            this.Bailiff = bail;
            this.Fields = Fields;
            this.Industry = Industry;
            this.Troops = Troops;
            this.TaxRate = TaxRate;
            TaxRateNext = TaxRateNext;
            this.OfficialSpendNext = OfficialSpendNext;
            this.GarrisonSpendNext = GarrisonSpendNext;
            this.InfrastructureSpendNext = InfrastructureSpendNext;
            this.KeepSpendNext = KeepSpendNext;
            this.KeyStatsCurrent = KeyStatsCurrent;
            this.KeyStatsPrevious = KeyStatsPrevious;
            this.KeepLevel = KeepLevel;
            this.Loyalty = Loyalty;
            this.Status = Status;
            this.FiefsLanguage = FiefsLanguage;
            this.FiefsTerrain = FiefsTerrain;
            this.CharactersInFief = CharactersInFief;
            this.BarredCharacters = BarredCharacters;
            this.BarredNationalities = BarredNationalities;
            this.BailiffDaysInFief = BailiffDaysInFief;
            this.Treasury = Treasury;
            this.Armies = Armies;
            this.HasRecruited = HasRecruited;
            this.TroopTransfers = TroopTransfers;
            this.IsPillaged = IsPillaged;
            this.CurrentSiege = CurrentSiege;
        }

        public Fief(GameClock Clock, IdGenerator IDGen, HexMapGraph GameMap) : base(Clock, IDGen, GameMap)
        {

        }

        // would prefer to check that fief is currently viewed fief elsewhere
        /// <summary>
        /// Adjusts the fief's tax rate and expenditure levels (officials, garrison, infrastructure, keep)
        /// </summary>
        /// <param name="tx">Proposed tax rate</param>
        /// <param name="off">Proposed officials expenditure</param>
        /// <param name="garr">Proposed garrison expenditure</param>
        /// <param name="infr">Proposed infrastructure expenditure</param>
        /// <param name="kp">Proposed keep expenditure</param>
        public bool AdjustExpenditures(double tx, uint off, uint garr, uint infr, uint kp)
        {
            // keep track of whether any spends ahve changed
            bool spendChanged = false;
            // get total spend
            uint totalSpend = off + garr + infr + kp;

            // factor in bailiff traits modifier for fief expenses
            double bailiffModif = 0;

            // get bailiff modifier (passing in whether bailiffDaysInFief is sufficient)
            bailiffModif = CalcBailExpModif(BailiffDaysInFief >= 30);
            if (bailiffModif != 0)
            {
                totalSpend = totalSpend + Convert.ToUInt32(totalSpend * bailiffModif);
            }

            // check that expenditure can be supported by the treasury
            // if it can't, display a message and cancel the commit
            if (!CheckExpenditureOK(totalSpend))
            {
                int difference = Convert.ToInt32(totalSpend - GetAvailableTreasury());
                return false;

            }
            // if treasury funds are sufficient to cover expenditure, do the commit
            else
            {
                // tax rate
                // check if amount/rate changed
                if (tx != TaxRateNext)
                {
                    // adjust tax rate
                    AdjustTaxRate(tx);
                    spendChanged = true;
                }

                // officials spend
                // check if amount/rate changed
                if (off != OfficialSpendNext)
                {
                    // adjust officials spend
                    AdjustOfficialsSpend(off);
                    spendChanged = true;
                }

                // garrison spend
                // check if amount/rate changed
                if (garr != GarrisonSpendNext)
                {
                    // adjust garrison spend
                    AdjustGarrisonSpend(garr);
                    spendChanged = true;
                }

                // infrastructure spend
                // check if amount/rate changed
                if (infr != InfrastructureSpendNext)
                {
                    // adjust infrastructure spend
                    AdjustInfraSpend(infr);
                    spendChanged = true;
                }

                // adjust keep spend
                // check if amount/rate changed
                if (kp != KeepSpendNext)
                {
                    // adjust keep spend
                    AdjustKeepSpend(kp);
                    spendChanged = true;
                }

                // display appropriate message
                string toDisplay = "";
                if (spendChanged)
                {
                    toDisplay += "adjusted";
                }
                else
                {
                    toDisplay += "unchanged";
                }
                return true;
            }
        }

        /// <summary>
        /// Calculates fief GDP
        /// </summary>
        /// <returns>uint containing fief GDP</returns>
        public uint CalcNewGDP()
        {
            uint gdp = 0;
            uint fldProd = 0;
            uint indProd = 0;

            // calculate production from fields using next season's expenditure
            fldProd = Convert.ToUInt32((CalcNewFieldLevel() * 8997));
            indProd = Convert.ToUInt32(CalcNewIndustryLevel() * CalcNewPopulation());

            // calculate final gdp
            gdp = (fldProd + indProd) / (CalcNewPopulation() / 1000);

            gdp *= 1000;

            return gdp;
        }

        /// <summary>
        /// Calculates fief population increase
        /// </summary>
        /// <returns>uint containing new fief population</returns>
        public uint CalcNewPopulation()
        {
            uint newPop = Convert.ToUInt32(Population + (Population * 0.005));
            return newPop;
        }

        /// <summary>
        /// Calculates fief income (NOT including income loss due to unrest/rebellion)
        /// </summary>
        /// <returns>uint containing fief income</returns>
        public int CalcNewIncome()
        {
            int fiefBaseIncome = 0;
            int fiefIncome = 0;

            // check for siege
            if (CurrentSiege == null)
            {
                // no siege = use next season's expenditure and tax rate
                fiefBaseIncome = Convert.ToInt32(CalcNewGDP() * (TaxRateNext / 100));
            }
            else
            {
                // siege = use next season's expenditure and 0% tax rate = no income
                fiefBaseIncome = Convert.ToInt32(CalcNewGDP() * 0);
            }

            fiefIncome = fiefBaseIncome;

            // factor in bailiff modifier (also passing whether bailiffDaysInFief is sufficient)
            fiefIncome = fiefIncome + Convert.ToInt32(fiefBaseIncome * CalcBlfIncMod(BailiffDaysInFief >= 30));

            // factor in officials spend modifier
            fiefIncome = fiefIncome + Convert.ToInt32(fiefBaseIncome * CalcOffIncMod());

            // ensure min income = 0
            if (fiefIncome < 0)
            {
                fiefIncome = 0;
            }

            return fiefIncome;
        }

        /// <summary>
        /// Calculates effect of financial controller on fief family expenses
        /// </summary>
        /// <returns>double containing fief family expenses modifier</returns>
        /// <param name="ch">The financial controller (Character)</param>
        public double CalcFamExpenseMod(Character ch = null)
        {
            double famExpMod = 0;

            // set default management rating
            double manRating = 3;

            if (ch != null)
            {
                manRating = ch.Management;
            }

            // 2.5% decrease in family expenses per management level above 1
            famExpMod = (((manRating - 1) * 2.5) / 100) * -1;

            // factor in financial controller traits
            if (ch != null)
            {
                double famExpTraitsMod = ch.CalcTraitEffect(Stats.FAMEXPENSE);

                // apply to famExpMod
                if (famExpTraitsMod != 0)
                {
                    famExpMod = (famExpMod + famExpTraitsMod);
                }
            }

            return famExpMod;
        }

        /// <summary>
        /// Calculates effect of bailiff on fief income
        /// </summary>
        /// <returns>double containing fief income modifier</returns>
        /// <param name="daysInFiefOK">bool specifying whether bailiff has sufficient days in fief</param>
        public double CalcBlfIncMod(bool daysInFiefOK)
        {
            double incomeModif = 0;

            // check if auto-bailiff
            if ((Bailiff == null) || (!daysInFiefOK))
            {
                // modifer = 0.025 per management level above 1
                // if auto-baliff set modifier at equivalent of management rating of 3
                incomeModif = 0.05;
            }
            else if (Bailiff != null)
            {
                incomeModif = Bailiff.CalcFiefIncMod();
            }

            return incomeModif;
        }

        /// <summary>
        /// Calculates effect of officials spend on fief income
        /// </summary>
        /// <returns>double containing fief income modifier</returns>
        public double CalcOffIncMod()
        {
            double incomeModif = 0;

            // using next season's expenditure and population
            incomeModif = ((OfficialSpendNext - ((double)CalcNewPopulation() * 2)) / (CalcNewPopulation() * 2)) / 10;

            return incomeModif;
        }

        /// <summary>
        /// Calculates effect of unrest/rebellion on fief income
        /// </summary>
        /// <returns>double containing fief income modifier</returns>
        public double CalcStatusIncmMod()
        {
            double incomeModif = 0;

            switch (Status)
            {
                // unrest = income reduced by 50%
                case 'U':
                    incomeModif = 0.5;
                    break;
                // unrest = income reduced by 100%
                case 'R':
                    incomeModif = 0;
                    break;
                // anything else = no reduction
                default:
                    incomeModif = 1;
                    break;
            }

            return incomeModif;
        }

        /// <summary>
        /// Calculates overlord taxes
        /// </summary>
        /// <returns>uint containing overlord taxes</returns>
        public uint CalcNewOlordTaxes()
        {
            // calculate tax, based on income of specified season
            uint oTaxes = Convert.ToUInt32(this.CalcNewIncome() * (FiefsProvince.TaxRate / 100));
            return oTaxes;
        }

        /// <summary>
        /// Calculates fief expenses
        /// </summary>
        /// <returns>int containing fief expenses</returns>
        public int CalcNewExpenses()
        {
            int fiefExpenses = 0;

            // using next season's expenditure
            fiefExpenses = (int)OfficialSpendNext + (int)InfrastructureSpendNext + (int)GarrisonSpendNext + (int)KeepSpendNext;

            // factor in bailiff traits modifier for fief expenses
            double bailiffModif = 0;

            // get bailiff modifier (passing in whether bailiffDaysInFief is sufficient)
            bailiffModif = CalcBailExpModif(BailiffDaysInFief >= 30);

            if (bailiffModif != 0)
            {
                fiefExpenses += Convert.ToInt32(fiefExpenses * bailiffModif);
            }

            return fiefExpenses;
        }

        /// <summary>
        /// Calculates effect of bailiff traits on fief expenses
        /// </summary>
        /// <returns>double containing fief expenses modifier</returns>
        /// <param name="daysInFiefOK">bool specifying whether bailiff has sufficient days in fief</param>
        public double CalcBailExpModif(bool daysInFiefOK)
        {
            double expTraitsModifier = 0;

            if ((Bailiff != null) && (daysInFiefOK))
            {
                expTraitsModifier = Bailiff.CalcTraitEffect(Stats.FIEFEXPENSE);
            }

            return expTraitsModifier;
        }

        /// <summary>
        /// Calculates fief bottom line
        /// </summary>
        /// <returns>uint containing fief income</returns>
        public int CalcNewBottomLine()
        {
            int fiefBottomLine = 0;

            // factor in effect of fief status on specified season's income
            int fiefIncome = Convert.ToInt32(CalcNewIncome() * CalcStatusIncmMod());

            // calculate bottom line using specified season's income, and expenses
            fiefBottomLine = fiefIncome - (int)CalcNewExpenses() - CalcFamilyExpenses();

            // check for occupation before deducting overlord taxes
            if (!CheckEnemyOccupation())
            {
                fiefBottomLine -= (int)CalcNewOlordTaxes();
            }

            return fiefBottomLine;
        }

        /// <summary>
        /// Calculates family expenses
        /// </summary>
        /// <returns>int containing family expenses</returns>
        /// <param name="season">string specifying whether to calculate for this or next season</param>
        public int CalcFamilyExpenses()
        {
            int famExpenses = 0;
            Character thisController = null;

            // for all fiefs, get bailiff wages
            if (Bailiff != null)
            {
                // ensure isn't family member (they don't get paid)
                if (string.IsNullOrWhiteSpace(Bailiff.FamilyID))
                {
                    int bailiffSalary = Convert.ToInt32(((NonPlayerCharacter)Bailiff).Salary);

                    // get total no. of fiefs this guy is bailiff in
                    List<Fief> myFiefs = Bailiff.GetFiefsBailiff();

                    // if is bailiff in more than this fief, only pay proportion of salary
                    if (myFiefs.Count > 1)
                    {
                        bailiffSalary = bailiffSalary / myFiefs.Count;
                    }

                    famExpenses += bailiffSalary;
                }
            }

            // if home fief, also get all non-bailiff expenses
            // (i.e. family allowances, other employees' wages)
            if (this == Owner.HomeFief)
            {
                // get whoever has highest management rating
                if (Owner.Spouse != null && (Owner.Management < Owner.Spouse.Management))
                {
                    thisController = Owner.Spouse;
                }
                else
                {
                    thisController = Owner;
                }

                foreach (NonPlayerCharacter element in Owner.MyNPCs)
                {
                    if (!((element.GetResponsibilities(Owner).ToLower()).Contains("bailiff")))
                    {
                        // add wage of non-bailiff employees
                        if (string.IsNullOrWhiteSpace(element.FamilyID))
                        {
                            famExpenses += Convert.ToInt32(element.Salary);
                        }
                        // add family allowance of family NPCs
                        else
                        {
                            // get allowance (based on family function)
                            int thisExpense = Convert.ToInt32(element.CalcFamilyAllowance(element.GetFunction(Owner)));

                            // check for siege
                            if (CurrentSiege != null)
                            {
                                // siege = half allowance
                                thisExpense = thisExpense / 2;
                            }

                            // add to total family expenses
                            famExpenses += thisExpense;
                        }
                    }
                }

            }

            // if NOT home fief, use bailiff's stats/traits
            else
            {
                if ((BailiffDaysInFief >= 30) && (Bailiff != null))
                {
                    thisController = Bailiff;
                }
            }

            // factor in traits and stats of financial controller (player/spouse/bailiff)
            double famExpModifier = CalcFamExpenseMod(thisController);

            // apply to family expenses
            if (famExpModifier != 0)
            {
                famExpenses += Convert.ToInt32(famExpenses * famExpModifier);
            }

            return famExpenses;
        }

        /// <summary>
        /// Adjusts fief tax rate
        /// (rate adjustment messages done client side)
        /// </summary>
        /// <param name="tx">double containing new tax rate</param>
        public void AdjustTaxRate(double tx)
        {
            // ensure max 100 and min 0
            if (tx > 100)
            {
                tx = 100;
            }
            else if (tx < 0)
            {
                tx = 0;
            }

            TaxRateNext = tx;
        }

        /// <summary>
        /// Gtes the maximum permitted expenditure for the fief of the specified type
        /// </summary>
        /// <returns>uint containing maximum permitted expenditure</returns>
        /// <param name="type">string containing type of expenditure</param>
        public uint GetMaxSpend(string type)
        {
            uint maxSpend = 0;

            uint[] spendMultiplier = { 14, 6, 13, 4 };
            uint thisMultiplier = 0;

            switch (type)
            {
                case "garrison":
                    thisMultiplier = spendMultiplier[0];
                    break;
                case "infrastructure":
                    thisMultiplier = spendMultiplier[1];
                    break;
                case "keep":
                    thisMultiplier = spendMultiplier[2];
                    break;
                case "officials":
                    thisMultiplier = spendMultiplier[3];
                    break;
                default:
                    break;
            }

            maxSpend = Convert.ToUInt32(Population) * thisMultiplier;

            return maxSpend;
        }

        //TODO notify on client side that expeniture has been changed to min/max
        /// <summary>
        /// Adjusts fief officials expenditure
        /// </summary>
        /// <param name="os">uint containing new officials expenditure</param>
        public void AdjustOfficialsSpend(uint os)
        {
            // ensure min 0
            if (os < 0)
            {
                os = 0;
            }

            // ensure doesn't exceed max permitted (4 per head of population)
            uint maxSpend = GetMaxSpend("officials");
            if (os > maxSpend)
            {
                os = maxSpend;
            }

            OfficialSpendNext = os;
        }

        //TODO notify user of min/max adjustment
        /// <summary>
        /// Adjusts fief infrastructure expenditure
        /// </summary>
        /// <param name="infs">uint containing new infrastructure expenditure</param>
        public void AdjustInfraSpend(uint infs)
        {
            // ensure min 0
            if (infs < 0)
            {
                infs = 0;

            }

            // ensure doesn't exceed max permitted (6 per head of population)
            uint maxSpend = GetMaxSpend("infrastructure");
            if (infs > maxSpend)
            {
                infs = maxSpend;
            }
            InfrastructureSpendNext = infs;
        }

        //TODO notify user of autoadjustment
        /// <summary>
        /// Adjusts fief garrison expenditure
        /// </summary>
        /// <param name="gs">uint containing new garrison expenditure</param>
        public void AdjustGarrisonSpend(uint gs)
        {
            // ensure min 0
            if (gs < 0)
            {
                gs = 0;
            }

            // ensure doesn't exceed max permitted (14 per head of population)
            uint maxSpend = GetMaxSpend("garrison");
            if (gs > maxSpend)
            {
                gs = maxSpend;
            }

            GarrisonSpendNext = gs;
        }

        /// <summary>
        /// Adjusts fief keep expenditure
        /// </summary>
        /// <param name="ks">uint containing new keep expenditure</param>
        public void AdjustKeepSpend(uint ks)
        {
            ;
            // ensure min 0
            if (ks < 0)
            {
                ks = 0;
            }

            // ensure doesn't exceed max permitted (13 per head of population)
            uint maxSpend = GetMaxSpend("keep");
            if (ks > maxSpend)
            {
                ks = maxSpend;

            }

            KeepSpendNext = ks;
        }

        /// <summary>
        /// Calculates new fief field level (from next season's spend)
        /// </summary>
        /// <returns>double containing new field level</returns>
        public double CalcNewFieldLevel()
        {
            double fldLvl = 0;

            // if no expenditure, field level reduced by 1%
            if (InfrastructureSpendNext == 0)
            {
                fldLvl = Fields - (Fields / 100);
            }
            // field level increases by 0.2 per 100k spent
            else
            {
                fldLvl = Fields + (InfrastructureSpendNext / 500000.00);
            }

            // ensure not < 0
            if (fldLvl < 0)
            {
                fldLvl = 0;
            }

            return fldLvl;
        }

        /// <summary>
        /// Calculates new fief industry level (from next season's spend)
        /// </summary>
        /// <returns>double containing new industry level</returns>
        public double CalcNewIndustryLevel()
        {
            double indLvl = 0;

            // if no expenditure, industry level reduced by 1%
            if (InfrastructureSpendNext == 0)
            {
                indLvl = Industry - (Industry / 100);
            }
            // industry level increases by 0.1 per 150k spent
            else
            {
                indLvl = Industry + (InfrastructureSpendNext / 1500000.00);
            }

            // ensure not < 0
            if (indLvl < 0)
            {
                indLvl = 0;
            }

            return indLvl;
        }

        /// <summary>
        /// Calculates new fief keep level (from next season's spend)
        /// </summary>
        /// <returns>double containing new keep level</returns>
        public double CalcNewKeepLevel()
        {
            double kpLvl = 0;

            // if no expenditure, keep level reduced by 0.15
            if (KeepSpendNext == 0)
            {
                kpLvl = KeepLevel - 0.15;
            }
            // keep level increases by 0.25 per 100k spent
            else
            {
                kpLvl = KeepLevel + (KeepSpendNext / 400000.00);
            }

            // ensure not < 0
            if (kpLvl < 0)
            {
                kpLvl = 0;
            }

            return kpLvl;
        }

        /// <summary>
        /// Calculates new fief loyalty level (i.e. for next season)
        /// </summary>
        /// <returns>double containing new fief loyalty</returns>
        public double CalcNewLoyalty()
        {
            double newBaseLoy = 0;
            double newLoy = 0;

            // calculate effect of tax rate change = loyalty % change is direct inverse of tax % change
            newBaseLoy = Loyalty + (Loyalty * (((TaxRateNext - TaxRate) / 100) * -1));

            // calculate effect of surplus 
            if (CalcNewBottomLine() > 0)
            {
                // loyalty reduced in proportion to surplus divided by income
                newLoy = newBaseLoy - (CalcNewBottomLine() / Convert.ToDouble(CalcNewIncome()));
            }
            else
            {
                newLoy = newBaseLoy;
            }

            // calculate effect of officials spend
            newLoy = newLoy + (newBaseLoy * CalcOffLoyMod());

            // calculate effect of garrison spend
            newLoy = newLoy + (newBaseLoy * CalcGarrLoyMod());

            // factor in bailiff modifier (also passing whether bailiffDaysInFief is sufficient)
            newLoy = newLoy + (newBaseLoy * CalcBlfLoyAdjusted(BailiffDaysInFief >= 30));

            // ensure loyalty within limits
            if (newLoy < 0)
            {
                newLoy = 0;
            }
            else if (newLoy > 9)
            {
                newLoy = 9;
            }

            return newLoy;
        }

        /// <summary>
        /// Calculates effect of bailiff on fief loyalty level
        /// Also includes effect of traits
        /// </summary>
        /// <returns>double containing fief loyalty modifier</returns>
        /// <param name="daysInFiefOK">bool specifying whether bailiff has sufficient days in fief</param>
        public double CalcBlfLoyAdjusted(bool daysInFiefOK)
        {
            double loyModif = 0;
            double loyTraitModif = 0;

            // get bailiff stats (set to auto-bailiff values by default)
            double bailStature = 3;
            double bailMgmt = 3;
            Language bailLang = FiefsLanguage;

            // if not auto-bailiff and if has served appropriate days in fief
            if ((Bailiff != null) && (daysInFiefOK))
            {
                bailStature = Bailiff.CalculateStature();
                bailMgmt = Bailiff.Management;
                bailLang = Bailiff.Language;
            }

            // get base bailiff loyalty modifier
            loyModif = CalcBaseFiefLoyMod(bailStature, bailMgmt, bailLang);

            // check for trait modifier, passing in daysInFief
            loyTraitModif = CalcBailLoyTraitMod(daysInFiefOK);

            loyModif += (loyModif * loyTraitModif);

            return loyModif;
        }

        /// <summary>
        /// Calculates base effect of bailiff's stats on fief loyalty
        /// Takes bailiff language into account
        /// </summary>
        /// <returns>double containing fief loyalty modifier</returns>
        /// <param name="stature">Bailiff's stature</param>
        /// <param name="mngt">Bailiff's management rating</param>
        /// <param name="lang">Bailiff's language</param>
        public double CalcBaseFiefLoyMod(double stature, double mngt, Language lang)
        {
            double loyModif = 0;
            double bailStats = 0;

            bailStats = (stature + mngt) / 2;

            // check for language effects
            if (FiefsLanguage != lang)
            {
                bailStats -= 3;
                if (bailStats < 1)
                {
                    bailStats = 1;
                }
            }

            // 1.25% increase in fief loyalty per bailiff's stature/management average above 1
            loyModif = (bailStats - 1) * 0.0125;

            return loyModif;
        }

        /// <summary>
        /// Calculates bailiff's trait modifier for fief loyalty
        /// </summary>
        /// <returns>double containing fief loyalty modifier</returns>
        /// <param name="daysInFiefOK">bool specifying whether bailiff has sufficient days in fief</param>
        public double CalcBailLoyTraitMod(bool daysInFiefOK)
        {
            double loyTraitsModifier = 0;

            if ((Bailiff != null) && (daysInFiefOK))
            {
                loyTraitsModifier = Bailiff.CalcTraitEffect(Stats.FIEFLOY);
            }

            return loyTraitsModifier;
        }

        /// <summary>
        /// Calculates effect of officials spend on fief loyalty
        /// </summary>
        /// <returns>double containing fief loyalty modifier</returns>
        public double CalcOffLoyMod()
        {
            double loyaltyModif = 0;

            // using next season's officials spend and population
            loyaltyModif = ((OfficialSpendNext - ((double)CalcNewPopulation() * 2)) / (CalcNewPopulation() * 2)) / 10;

            return loyaltyModif;
        }

        /// <summary>
        /// Calculates effect of garrison spend on fief loyalty
        /// </summary>
        /// <returns>double containing fief loyalty modifier</returns>
        public double CalcGarrLoyMod()
        {
            double loyaltyModif = 0;

            // using next season's garrison spend and population
            loyaltyModif = ((GarrisonSpendNext - ((double)CalcNewPopulation() * 7)) / (CalcNewPopulation() * 7)) / 10;

            return loyaltyModif;
        }

        /// <summary>
        /// Validates proposed expenditure levels, auto-adjusting where necessary
        /// </summary>
        public void ValidateFiefExpenditure()
        {
            // get total spend
            uint totalSpend = Convert.ToUInt32(CalcNewExpenses()); ;

            // check to see if proposed expenditure level doesn't exceed fief treasury
            bool isOK = CheckExpenditureOK(totalSpend);

            // if expenditure does exceed fief treasury
            if (!isOK)
            {
                // get available treasury
                int availTreasury = GetAvailableTreasury();
                // calculate amount by which treasury exceeded
                uint difference = Convert.ToUInt32(totalSpend - availTreasury);
                // auto-adjust expenditure
                AutoAdjustExpenditure(difference);
            }
        }

        /// <summary>
        /// Automatically adjusts expenditure at end of season, if exceeds treasury
        /// </summary>
        /// <param name="difference">The amount by which expenditure exceeds treasury</param>
        public void AutoAdjustExpenditure(uint difference)
        {
            // get available treasury
            int availTreasury = this.GetAvailableTreasury();

            // if treasury empty or in deficit, reduce all expenditure to 0
            if (availTreasury <= 0)
            {
                OfficialSpendNext = 0;
                GarrisonSpendNext = 0;
                InfrastructureSpendNext = 0;
                KeepSpendNext = 0;
            }
            else
            {
                // bool to control do while loop
                bool finished = false;
                // keep track of new difference
                uint differenceNew = difference;
                // get total expenditure
                uint totalSpend = OfficialSpendNext + GarrisonSpendNext + InfrastructureSpendNext + KeepSpendNext;
                // proportion to take off each spend
                double reduceByModifierOff = (double)OfficialSpendNext / totalSpend;
                double reduceByModifierGarr = (double)GarrisonSpendNext / totalSpend;
                double reduceByModifierInf = (double)InfrastructureSpendNext / totalSpend;
                double reduceByModifierKeep = (double)KeepSpendNext / totalSpend;
                // double to reduce spend by
                double reduceBy = 0;
                // uint to deduct from each spend
                uint takeOff = 0;
                // list to hold individual spends
                List<string> spends = new List<string>();

                do
                {
                    // update difference
                    difference = differenceNew;

                    // clear current spends list
                    if (spends.Count > 0)
                    {
                        spends.Clear();
                    }

                    // re-populate spends list with appropriate codes
                    // but only spends > 0
                    if (OfficialSpendNext > 0)
                    {
                        spends.Add("off");
                    }
                    if (GarrisonSpendNext > 0)
                    {
                        spends.Add("garr");
                    }
                    if (InfrastructureSpendNext > 0)
                    {
                        spends.Add("inf");
                    }
                    if (KeepSpendNext > 0)
                    {
                        spends.Add("keep");
                    }

                    // if no remaining spends, finish
                    if (spends.Count == 0)
                    {
                        finished = true;
                    }

                    // iterate through each entry in spends list
                    // (remember: only spends > 0)
                    for (int i = 0; i < spends.Count; i++)
                    {
                        switch (spends[i])
                        {
                            // officials
                            case "off":
                                if (!finished)
                                {
                                    // calculate amount by which spend needs to be reduced
                                    reduceBy = (double)difference * reduceByModifierOff;
                                    // round up if < 1
                                    if ((reduceBy < 1) || (reduceBy == 1))
                                    {
                                        takeOff = 1;
                                    }
                                    // round down if > 1
                                    else if (reduceBy > 1)
                                    {
                                        takeOff = Convert.ToUInt32(Math.Truncate(reduceBy));
                                    }
                                    //TODO remove or log
                                    /*
                                    if (Globals_Client.showDebugMessages)
                                    {
                                        System.Windows.Forms.MessageBox.Show("difference: " + difference + "\r\noffSpend: " + OfficialSpendNext + "\r\ntotSpend: " + totalSpend + "\r\nreduceByModifierOff: " + reduceByModifierOff + "\r\nreduceBy: " + reduceBy + "\r\ntakeOff: " + takeOff);
                                    }*/

                                    if (!(differenceNew < takeOff))
                                    {
                                        // if is enough in spend to subtract takeOff amount
                                        if (OfficialSpendNext >= takeOff)
                                        {
                                            // subtract takeOff from spend
                                            OfficialSpendNext -= takeOff;
                                            // subtract takeOff from remaining difference
                                            differenceNew -= takeOff;
                                        }
                                        // if is less in spend than takeOff amount
                                        else
                                        {
                                            // subtract spend from remaining difference
                                            differenceNew -= OfficialSpendNext;
                                            // reduce spend to 0
                                            OfficialSpendNext = 0;
                                        }
                                        // check to see if is any difference remaining 
                                        if (differenceNew == 0)
                                        {
                                            // if no remaining difference, signal exit from do while loop
                                            finished = true;
                                        }
                                    }
                                }
                                break;
                            case "garr":
                                if (!finished)
                                {
                                    // calculate amount by which spend needs to be reduced
                                    reduceBy = (double)difference * reduceByModifierGarr;
                                    // round up if < 1
                                    if ((reduceBy < 1) || (reduceBy == 1))
                                    {
                                        takeOff = 1;
                                    }
                                    // round down if > 1
                                    else if (reduceBy > 1)
                                    {
                                        takeOff = Convert.ToUInt32(Math.Truncate(reduceBy));
                                    }
                                    //TODO remove or log
                                    /*if (Globals_Client.showDebugMessages)
                                    {
                                        System.Windows.Forms.MessageBox.Show("difference: " + difference + "\r\ngarrSpend: " + GarrisonSpendNext + "\r\ntotSpend: " + totalSpend + "\r\nreduceByModifierGarr: " + reduceByModifierGarr + "\r\nreduceBy: " + reduceBy + "\r\ntakeOff: " + takeOff);
                                    }*/

                                    if (!(differenceNew < takeOff))
                                    {
                                        if (GarrisonSpendNext >= takeOff)
                                        {
                                            GarrisonSpendNext -= takeOff;
                                            differenceNew -= takeOff;
                                        }
                                        else
                                        {
                                            differenceNew -= GarrisonSpendNext;
                                            GarrisonSpendNext = 0;
                                        }
                                        if (differenceNew == 0)
                                        {
                                            finished = true;
                                        }
                                    }
                                }
                                break;
                            case "inf":
                                if (!finished)
                                {
                                    // calculate amount by which spend needs to be reduced
                                    reduceBy = (double)difference * reduceByModifierInf;
                                    // round up if < 1
                                    if ((reduceBy < 1) || (reduceBy == 1))
                                    {
                                        takeOff = 1;
                                    }
                                    // round down if > 1
                                    else if (reduceBy > 1)
                                    {
                                        takeOff = Convert.ToUInt32(Math.Truncate(reduceBy));
                                    }
                                    //TODO remove or log
                                    /*
                                    if (Globals_Client.showDebugMessages)
                                    {
                                        System.Windows.Forms.MessageBox.Show("difference: " + difference + "\r\ninfSpend: " + InfrastructureSpendNext + "\r\ntotSpend: " + totalSpend + "\r\nreduceByModifierInf: " + reduceByModifierInf + "\r\nreduceBy: " + reduceBy + "\r\ntakeOff: " + takeOff);
                                    }*/

                                    if (!(differenceNew < takeOff))
                                    {
                                        if (InfrastructureSpendNext >= takeOff)
                                        {
                                            InfrastructureSpendNext -= takeOff;
                                            differenceNew -= takeOff;
                                        }
                                        else
                                        {
                                            differenceNew -= InfrastructureSpendNext;
                                            InfrastructureSpendNext = 0;
                                        }
                                        if (differenceNew == 0)
                                        {
                                            finished = true;
                                        }
                                    }
                                }
                                break;
                            case "keep":
                                if (!finished)
                                {
                                    // calculate amount by which spend needs to be reduced
                                    reduceBy = (double)difference * reduceByModifierKeep;
                                    // round up if < 1
                                    if ((reduceBy < 1) || (reduceBy == 1))
                                    {
                                        takeOff = 1;
                                    }
                                    // round down if > 1
                                    else if (reduceBy > 1)
                                    {
                                        takeOff = Convert.ToUInt32(Math.Truncate(reduceBy));
                                    }

                                    if (!(differenceNew < takeOff))
                                    {
                                        if (KeepSpendNext >= takeOff)
                                        {
                                            KeepSpendNext -= takeOff;
                                            differenceNew -= takeOff;
                                        }
                                        else
                                        {
                                            differenceNew -= KeepSpendNext;
                                            KeepSpendNext = 0;
                                        }
                                        if (differenceNew == 0)
                                        {
                                            finished = true;
                                        }
                                    }
                                }
                                break;
                            default:
                                break;
                        }
                    }
                } while (!finished);
            }
        }

        /// <summary>
        /// Calculates amount available in treasury for financial transactions
        /// </summary>
        /// <returns>int containing amount available</returns>
        /// <param name="deductFiefExpense">bool indicating whether to account for fief expenses in the calculation</param>
        public int GetAvailableTreasury(bool deductFiefExpense = false)
        {
            int amountAvail = 0;

            // get treasury
            amountAvail = Treasury;
            // deduct family expenses
            amountAvail -= CalcFamilyExpenses();
            // deduct overlord taxes
            amountAvail -= Convert.ToInt32(CalcNewOlordTaxes());

            if (deductFiefExpense)
            {
                // deduct fief expenditure
                amountAvail -= CalcNewExpenses();
            }

            return amountAvail;
        }

        /// <summary>
        /// Compares expenditure level with fief treasury
        /// </summary>
        /// <returns>bool indicating whether expenditure acceptable</returns>
        /// <param name="totalSpend">proposed total expenditure for next season</param>
        public bool CheckExpenditureOK(uint totalSpend)
        {
            bool spendLevelOK = true;

            // get available treasury
            int availTreasury = GetAvailableTreasury();

            // if there are funds in the treasury
            if (availTreasury > 0)
            {
                // expenditure should not exceed treasury
                if (totalSpend > availTreasury)
                {
                    spendLevelOK = false;
                }
            }
            // if treasury is empty or in deficit
            else
            {
                // expenditure should be 0
                if (totalSpend > 0)
                {
                    spendLevelOK = false;
                }
            }

            return spendLevelOK;
        }

        /// <summary>
        /// Updates fief data at the end/beginning of the season
        /// </summary>
        public void UpdateFief()
        {
            // update previous season's financial data
            KeyStatsCurrent.CopyTo(KeyStatsPrevious, 0);

            // update tax rate
            KeyStatsCurrent[2] = TaxRateNext;
            TaxRate = TaxRateNext;

            // update bailiffDaysInFief if appropriate
            if ((Bailiff != null) && (Bailiff.Days > 0))
            {
                Bailiff.UseUpDays();
            }

            // validate fief expenditure against treasury
            ValidateFiefExpenditure();

            // update loyalty level
            KeyStatsCurrent[0] = CalcNewLoyalty();
            Loyalty = KeyStatsCurrent[0];

            // update GDP
            KeyStatsCurrent[1] = CalcNewGDP();

            // update officials spend
            KeyStatsCurrent[3] = OfficialSpendNext;

            // update garrison spend
            KeyStatsCurrent[4] = GarrisonSpendNext;

            // update infrastructure spend
            KeyStatsCurrent[5] = InfrastructureSpendNext;

            // update keep spend
            KeyStatsCurrent[6] = KeepSpendNext;

            // update keep level (based on next season keep spend)
            KeyStatsCurrent[7] = CalcNewKeepLevel();
            KeepLevel = KeyStatsCurrent[7];

            // update income
            KeyStatsCurrent[8] = CalcNewIncome();

            // update family expenses
            KeyStatsCurrent[9] = CalcFamilyExpenses();

            // update total expenses (including bailiff modifiers)
            KeyStatsCurrent[10] = CalcFamilyExpenses() + CalcNewExpenses();

            // update overord taxes
            KeyStatsCurrent[11] = CalcNewOlordTaxes();

            // update overord tax rate
            KeyStatsCurrent[12] = FiefsProvince.TaxRate;

            // update bottom line
            KeyStatsCurrent[13] = CalcNewBottomLine();

            // check for unrest/rebellion
            Status = CheckFiefStatus();

            // update fields level (based on next season infrastructure spend)
            Fields = CalcNewFieldLevel();

            // update industry level (based on next season infrastructure spend)
            Industry = CalcNewIndustryLevel();

            // update fief treasury with new bottom line
            Treasury += Convert.ToInt32(KeyStatsCurrent[13]);

            // check for occupation before transfering overlord taxes into overlord's treasury
            if (!CheckEnemyOccupation())
            {
                // get overlord
                PlayerCharacter thisOverlord = GetOverlord();
                if (thisOverlord != null)
                {
                    // get overlord's home fief
                    Fief overlordHome = thisOverlord.HomeFief;

                    if (overlordHome != null)
                    {
                        // pay in taxes
                        overlordHome.Treasury += Convert.ToInt32(CalcNewOlordTaxes());
                    }
                }
            }

            // update population
            Population = Convert.ToInt32(CalcNewPopulation());

            // reset bailiffDaysInFief
            BailiffDaysInFief = 0;

            // reset hasRecruited
            HasRecruited = false;

            // reset isPillaged
            IsPillaged = false;

            // update transfers
            foreach (KeyValuePair<string, Detachment> transferEntry in TroopTransfers)
            {
                Console.WriteLine("Removed creation of armies for Detachements ??? Not sure what this is, check detachemetns after season update");
                /*
                // create temporary army to check attrition
                uint[] thisTroops = transferEntry.Value.Troops;
                int days = transferEntry.Value.Days;
                Character owner = transferEntry.Value.LeftBy;
                Army tempArmy = new Army(Globals_Game.GetNextArmyID(), null, owner, days, this.id, trp: thisTroops);

                // attrition checks
                byte attritionChecks = 0;
                attritionChecks = Convert.ToByte(days / 7);
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

                // update detachment days
                transferEntry.Value.days = 90;

                // set tempArmy to null
                tempArmy = null;*/
            }
        }

        /// <summary>
        /// Checks for transition from calm to unrest/rebellion, or from unrest to calm
        /// </summary>
        /// <returns>char indicating fief status</returns>
        public char CheckFiefStatus()
        {
            char originalStatus = Status;

            // if fief in rebellion it can only be recovered by combat or bribe,
            // so don't run check
            if (!Status.Equals('R'))
            {
                // method 1 (depends on tax rate and surplus)
                if ((TaxRate > 20) && (KeyStatsCurrent[13] > (KeyStatsCurrent[8] * 0.1)))
                {
                    if (Random.Shared.NextDouble() * 100 <= (TaxRate - 20))
                    {
                        Status = 'R';
                    }
                }

                // method 2 (depends on fief loyalty level)
                if (!Status.Equals('R'))
                {
                    double chance = Random.Shared.NextDouble() * 100;

                    // loyalty 3-4
                    if ((Loyalty > 3) && (Loyalty <= 4))
                    {
                        if (chance <= 2)
                        {
                            Status = 'R';
                        }
                        else if (chance <= 10)
                        {
                            Status = 'U';
                        }
                        else
                        {
                            Status = 'C';
                        }
                    }

                    // loyalty 2-3
                    else if ((Loyalty > 2) && (Loyalty <= 3))
                    {
                        if (chance <= 14)
                        {
                            Status = 'R';
                        }
                        else if (chance <= 30)
                        {
                            Status = 'U';
                        }
                        else
                        {
                            Status = 'C';
                        }
                    }

                    // loyalty 1-2
                    else if ((Loyalty > 1) && (Loyalty <= 2))
                    {
                        if (chance <= 26)
                        {
                            Status = 'R';
                        }
                        else if (chance <= 50)
                        {
                            Status = 'U';
                        }
                        else
                        {
                            Status = 'C';
                        }
                    }

                    // loyalty 0-1
                    else if ((Loyalty > 0) && (Loyalty <= 1))
                    {
                        if (chance <= 38)
                        {
                            Status = 'R';
                        }
                        else if (chance <= 70)
                        {
                            Status = 'U';
                        }
                        else
                        {
                            Status = 'C';
                        }
                    }

                    // loyalty 0
                    else if (Loyalty == 0)
                    {
                        if (chance <= 50)
                        {
                            Status = 'R';
                        }
                        else if (chance <= 90)
                        {
                            Status = 'U';
                        }
                        else
                        {
                            Status = 'C';
                        }
                    }
                }
            }

            // if status changed
            if (Status != originalStatus)
            {
                // if necessary, APPLY STATUS LOSS
                if (Status.Equals('R'))
                {
                    Console.WriteLine("Stature currently not affected");
                    //Owner.AdjustStatureModifier(-0.1);
                }

                // CREATE JOURNAL ENTRY
                // get old and new status
                string oldStatus = "";
                string newStatus = "";
                if (originalStatus.Equals('C'))
                {
                    oldStatus = "calm";
                }
                else if (originalStatus.Equals('U'))
                {
                    oldStatus = "unrest";
                }
                else if (originalStatus.Equals('R'))
                {
                    oldStatus = "rebellion";
                }
                if (Status.Equals('C'))
                {
                    newStatus = "CALM";
                }
                else if (Status.Equals('U'))
                {
                    newStatus = "UNREST";
                }
                else if (Status.Equals('R'))
                {
                    newStatus = "REBELLION";
                }

                // get interested parties
                bool success = true;
                PlayerCharacter fiefOwner = Owner;

                // ID
                uint entryID = IDGen.GetNextJournalEntryID();

                // date
                uint year = Clock.CurrentYear;
                byte season = Clock.CurrentSeason;

                // personae
                List<string> tempPersonae = new List<string>();
                tempPersonae.Add(fiefOwner.ID + "|fiefOwner");
                if ((Status.Equals('R')) || (oldStatus.Equals("R")))
                {
                    tempPersonae.Add("all|all");
                }
                string[] thisPersonae = tempPersonae.ToArray();

                // type
                string type = "";
                if (Status.Equals('C'))
                {
                    type = "fiefStatusCalm";
                }
                else if (Status.Equals('U'))
                {
                    type = "fiefStatusUnrest";
                }
                else if (Status.Equals('R'))
                {
                    type = "fiefStatusRebellion";
                }

                // location
                string location = ID;
                Console.WriteLine("Not Saving Journal Entry");
                //success = Globals_Game.AddPastEvent(newEntry);
            }

            return Status;
        }

        /// <summary>
        /// Adds character to characters list
        /// </summary>
        /// <param name="ch">Character to be inserted into characters list</param>
        internal void AddCharacter(Character ch)
        {
            // add character
            CharactersInFief.Add(ch);
        }

        /// <summary>
        /// Removes character from characters list
        /// </summary>
        /// <returns>bool indicating success/failure</returns>
        /// <param name="ch">Character to be removed from characters list</param>
        internal bool RemoveCharacter(Character ch)
        {
            // remove character
            bool success = CharactersInFief.Remove(ch);

            return success;
        }

        /// <summary>
        /// Adds army to armies list
        /// </summary>
        /// <param name="armyID">ID of army to be inserted</param>
        internal void AddArmy(Army armyID)
        {
            // add army
            Armies.Add(armyID);
        }

        /// <summary>
        /// Removes army from armies list
        /// </summary>
        /// <returns>bool indicating success/failure</returns>
        /// <param name="armyID">ID of army to be removed</param>
        internal bool RemoveArmy(Army armyID)
        {
            // remove army
            bool success = Armies.Remove(armyID);

            return success;
        }

        /// <summary>
        /// Bar a specific character from the fief's keep
        /// </summary>
        /// <param name="ch">Character to be barred</param>
        internal void BarCharacter(string ch)
        {
            // bar character
            BarredCharacters.Add(ch);
        }

        /// <summary>
        /// Removes a fief keep bar from a specific character
        /// </summary>
        /// <returns>bool indicating success/failure</returns>
        /// <param name="ch">Character for whom bar to be removed</param>
        internal bool RemoveBarCharacter(string ch)
        {
            // remove character bar
            bool success = BarredCharacters.Remove(ch);

            return success;
        }

        /// <summary>
        /// Calculates the result of a call for troops
        /// </summary>
        /// <returns>int containing number of troops responding to call</returns>
        /// <param name="minProportion">double specifying minimum proportion of total troop number required</param>
        /// <param name="maxProportion">double specifying maximum proportion of total troop number required</param>
        public int CallUpTroops(double minProportion = 0, double maxProportion = 1)
        {
            int numberTroops = 0;
            int maxNumber = this.CalcMaxTroops();

            // generate random double between min and max
            double myRandomDouble = Random.Shared.NextDouble() * maxProportion + minProportion;

            // apply random double as modifier to maxNumber
            numberTroops = Convert.ToInt32(maxNumber * myRandomDouble);

            // check for effects of unrest (only get 50% of troops)
            if (Status.Equals('U'))
            {
                numberTroops /= 2;
            }

            return numberTroops;
        }

        /// <summary>
        /// Calculates the maximum number of troops available for call up in the fief
        /// </summary>
        /// <returns>int containing maximum number of troops</returns>
        public int CalcMaxTroops()
        {
            return Convert.ToInt32(Population * 0.05);
        }

        /// <summary>
        /// Calculates the garrison size for the fief
        /// </summary>
        /// <returns>int containing the garrison size</returns>
        public int GetGarrisonSize()
        {
            int garrisonSize = 0;

            garrisonSize = Convert.ToInt32(KeyStatsCurrent[4] / 1000);

            return garrisonSize;
        }

        /// <summary>
        /// Gets fief's overlord
        /// </summary>
        /// <returns>The overlord</returns>
        public PlayerCharacter GetOverlord()
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Gets the fief's rightful king (i.e. the king of the kingdom that the fief traditionally belongs to)
        /// </summary>
        /// <returns>The king</returns>
        public PlayerCharacter GetRightfulKing()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the fief's current king (i.e. the king of the current owner)
        /// </summary>
        /// <returns>The king</returns>
        public PlayerCharacter GetCurrentKing()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Checks if the fief is under enemy occupation
        /// </summary>
        /// <returns>bool indicating whether is under enemy occupation</returns>
        public bool CheckEnemyOccupation()
        {
            bool isOccupied = false;

            if (GetRightfulKing() != GetCurrentKing())
            {
                isOccupied = true;
            }

            return isOccupied;
        }

        /// <summary>
        /// Gets the fief's rightful kingdom (i.e. the kingdom that it traditionally belongs to)
        /// </summary>
        /// <returns>The kingdom</returns>
        public Kingdom GetRightfulKingdom()
        {
            Kingdom thisKingdom = null;

            if (FiefsProvince.GetRightfulKingdom() != null)
            {
                thisKingdom = FiefsProvince.GetRightfulKingdom();
            }

            return thisKingdom;
        }

        /// <summary>
        /// Gets the fief's current kingdom (i.e. the kingdom of the current owner)
        /// </summary>
        /// <returns>The kingdom</returns>
        public Kingdom GetCurrentKingdom()
        {
            throw new NotImplementedException();
        }

        //ASK difference between owner and title holder, who to notify
        /// <summary>
        /// Processes the functions involved in a change of fief ownership
        /// </summary>
        /// <returns>bool indicating success</returns>
        /// <param name="newOwner">The new owner</param>
        /// <param name="circumstance">The circumstance under which the change of ownership is taking place</param>
        public bool ChangeOwnership(PlayerCharacter newOwner, string circumstance = "hostile")
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Checks to see if there is currently a field army in the fief keep
        /// </summary>
        /// <returns>bool indicating presence of a field army</returns>
        public bool CheckFieldArmyInKeep()
        {
            bool armyInKeep = false;

            foreach (Character thisChar in CharactersInFief)
            {
                if (thisChar.ArmyID != null && thisChar.InKeep)
                {
                    armyInKeep = true;
                    break;
                }
            }

            return armyInKeep;
        }

        /// <summary>
        /// Checks to see if an attempts to quell a rebellion has been successful
        /// </summary>
        /// <returns>bool indicating quell success or failure</returns>
        /// <param name="a">The army trying to quell the rebellion</param>
        public bool Quell_checkSuccess(Army a)
        {
            bool rebellionQuelled = false;

            // calculate base chance of success, based on army size and fief population
            double successChance = a.CalcArmySize() / (Population / Convert.ToDouble(1000));

            // get army leader
            Character aLeader = a.Leader;

            // get army owner
            Character aOwner = a.Owner;

            // apply any bonus for leadership traits
            if (aLeader != null)
            {
                successChance += aLeader.GetLeadershipValue();
            }

            // apply any bonus for ancestral ownership
            if (aOwner != null)
            {
                // only if army owner is ancestral owner & current owner isn't
                if ((aOwner == AncestralOwner) && (Owner != AncestralOwner))
                {
                    successChance += (aOwner.CalculateStature() * 2.22);
                }
            }

            // ensure successChance always between 1 > 99 (to allow for minimum chance of success/failure)
            if (successChance < 1)
            {
                successChance = 1;
            }
            else if (successChance > 99)
            {
                successChance = 99;
            }

            // generate random double 0-100 to check for success
            rebellionQuelled = Random.Shared.NextDouble() * 100 <= successChance;

            return rebellionQuelled;
        }

        /// <summary>
        /// Attempts to quell the rebellion in the fief using the specified army
        /// </summary>
        /// <returns>bool indicating quell success or failure</returns>
        /// <param name="a">The army trying to quell the rebellion</param>
        public bool QuellRebellion(Army a)
        {
            // check to see if quell attempt was successful
            bool quellSuccessful = Quell_checkSuccess(a);

            // if quell successful
            if (quellSuccessful)
            {
                // process change of ownership, if appropriate
                if (Owner != a.Owner)
                {
                    ChangeOwnership(a.Owner);
                }

                // set status
                Status = 'C';
            }

            // if quell not successful
            else
            {
                // CREATE JOURNAL ENTRY
                // get interested parties
                bool success = true;
                PlayerCharacter fiefOwner = Owner;
                PlayerCharacter attackerOwner = a.Owner;
                Character attackerLeader = null;
                if (a.Leader != null)
                {
                    attackerLeader = a.Leader;
                }

                // ID
                uint entryID = IDGen.GetNextJournalEntryID();

                // date
                uint year = Clock.CurrentYear;
                byte season = Clock.CurrentSeason;

                // personae
                List<string> tempPersonae = new List<string>();
                tempPersonae.Add(fiefOwner.ID + "|fiefOwner");
                tempPersonae.Add(attackerOwner.ID + "|attackerOwner");
                if (attackerLeader != null)
                {
                    tempPersonae.Add(attackerLeader.ID + "|attackerLeader");
                }
                string[] quellPersonae = tempPersonae.ToArray();

                // type
                string type = "rebellionQuellFailed";

                // location
                string location = ID;

                // description
                string[] fields = new string[4];
                fields[0] = attackerOwner.FirstName + " " + attackerOwner.FamilyName;
                fields[1] = "";
                if (attackerLeader != null)
                {
                    fields[1] = ", led by " + attackerLeader.FirstName + " " + attackerLeader.FamilyName + ",";
                }
                fields[2] = Name;
                fields[3] = fiefOwner.FirstName + " " + fiefOwner.FamilyName;

                //JournalEntry quellEntry = new JournalEntry(entryID, year, season, quellPersonae, type, quellRebellion, loc: location);
                //success = Globals_Game.AddPastEvent(quellEntry);
                Console.WriteLine("Not creating journal entry");
            }

            return quellSuccessful;
        }

        /// <summary>
        /// Gets travel cost (in days) to move to a nieghbouring fief
        /// </summary>
        /// <returns>double containing travel cost</returns>
        /// <param name="target">Target fief</param>
        public double getTravelCost(Fief target, Army armyID = null)
        {
            double cost = 0;
            // calculate base travel cost based on terrain for both fiefs
            cost = (FiefsTerrain.TravelCost + target.FiefsTerrain.TravelCost) / 2;

            // apply season modifier
            cost *= Clock.CalcSeasonTravMod();

            // if necessary, apply army modifier
            if (armyID != null)
            {
                cost *= armyID.CalcMovementModifier();
            }

            if (this == target)
            {
                cost = 0;
            }

            return cost;
        }

        /// <summary>
        /// Checks to see if display of financial data for the specified financial period
        /// is permitted due to ongoing siege
        /// </summary>
        /// <returns>bool indicating whether display is permitted</returns>
        /// <param name="target">int indicating desired financial period relative to current season</param>
        /// <param name="s">The siege</param>
        public bool CheckToShowFinancialData(int relativeSeason, Siege s)
        {
            bool displayData = true;

            uint financialPeriodYear = GetFinancialYear(relativeSeason);
            if (financialPeriodYear > s.StartYear)
            {
                displayData = false;
            }
            else if (financialPeriodYear == s.StartYear)
            {
                byte financialPeriodSeason = GetFinancialSeason(relativeSeason);
                if (financialPeriodSeason > s.StartSeason)
                {
                    displayData = false;
                }
            }

            return displayData;
        }

        /// <summary>
        /// Gets the year for the specified financial period
        /// </summary>
        /// <returns>The year</returns>
        /// <param name="target">int indicating desired financial period relative to current season</param>
        public uint GetFinancialYear(int relativeSeason)
        {
            uint financialYear = 0;
            uint thisYear = Clock.CurrentYear;

            switch (relativeSeason)
            {
                case (-1):
                    if (Clock.CurrentSeason == 0)
                    {
                        financialYear = thisYear - 1;
                    }
                    else
                    {
                        financialYear = thisYear;
                    }
                    break;
                case (1):
                    if (Clock.CurrentSeason == 4)
                    {
                        financialYear = thisYear + 1;
                    }
                    else
                    {
                        financialYear = thisYear;
                    }
                    break;
                default:
                    financialYear = thisYear;
                    break;
            }

            return financialYear;
        }

        /// <summary>
        /// Gets the season for the specified financial period
        /// </summary>
        /// <returns>The season</returns>
        /// <param name="target">int indicating desired financial period relative to current season</param>
        public byte GetFinancialSeason(int relativeSeason)
        {
            byte financialSeason = 0;
            byte thisSeason = Clock.CurrentSeason;

            switch (relativeSeason)
            {
                case (-1):
                    if (thisSeason == 0)
                    {
                        financialSeason = 4;
                    }
                    else
                    {
                        financialSeason = thisSeason;
                        financialSeason--;
                    }
                    break;
                case (1):
                    if (thisSeason == 4)
                    {
                        financialSeason = 0;
                    }
                    else
                    {
                        financialSeason = thisSeason;
                        financialSeason++;
                    }
                    break;
                default:
                    financialSeason = thisSeason;
                    break;
            }

            return financialSeason;
        }
        /// <summary>
        /// Transfers funds between the fief treasury and another fief's treasury
        /// </summary>
        /// <param name="to">The Fief to which funds are to be transferred</param>
        /// <param name="amount">How much to be transferred</param>
        public bool TreasuryTransfer(Fief to, int amount)
        {
            // ensure number is positive
            amount = Math.Abs(amount);
            // check enough for transfer
            if (Treasury < amount)
            {
                return false;
            }
            else
            {
                // Lock
                lock (TreasuryTransferLock)
                {
                    // subtract from source treasury
                    AdjustTreasury(-amount);

                    // add to target treasury
                    to.AdjustTreasury(amount);
                }

                return true;
            }
        }

        public void AdjustTreasury(int amount)
        {
            lock (TreasuryLock)
            {
                Treasury += amount;
            }
        }
        /// <summary>
        /// Creates a defending army for defence of a fief during pillage or siege
        /// </summary>
        /// <returns>The defending army</returns>
        public Army CreateDefendingArmy()
        {
            Army defender = null;
            Character armyLeader = null;
            string armyLeaderID = null;
            double armyLeaderDays = 90;

            // if present in fief, get bailiff and assign as army leader
            if (Bailiff != null)
            {
                for (int i = 0; i < CharactersInFief.Count; i++)
                {
                    if (CharactersInFief[i] == Bailiff)
                    {
                        armyLeader = Bailiff;
                        armyLeaderID = armyLeader.ID;
                        armyLeaderDays = armyLeader.Days;
                        break;
                    }
                }
            }

            // gather troops to create army
            uint garrisonSize = 0;
            uint militiaSize = 0;
            uint[] troopsForArmy = new uint[] { 0, 0, 0, 0, 0, 0, 0 };
            uint[] tempTroops = new uint[] { 0, 0, 0, 0, 0, 0, 0 };
            uint totalSoFar = 0;

            // get army nationality
            string thisNationality = Owner.Nationality.NatID;

            // get size of fief garrison
            garrisonSize = Convert.ToUInt32(GetGarrisonSize());

            // get size of fief 'militia' responding to emergency
            militiaSize = Convert.ToUInt32(CallUpTroops(minProportion: 0.33, maxProportion: 0.66));

            // get defending troops based on following troop type proportions:
            // militia = Global_Server.recruitRatios for types 0-4, fill with rabble
            // garrison = Global_Server.recruitRatios * 2 for types 0-3, fill with foot

            // 1. militia (includes proportion of rabble)
            for (int i = 0; i < tempTroops.Length; i++)
            {
                // work out 'trained' troops numbers
                if (i < tempTroops.Length - 1)
                {
                    tempTroops[i] = Convert.ToUInt32(militiaSize * GameSettings.RECRUIT_RATIOS[thisNationality][i]);
                }
                // fill up with rabble
                else
                {
                    tempTroops[i] = militiaSize - totalSoFar;
                }

                troopsForArmy[i] += tempTroops[i];
                totalSoFar += tempTroops[i];
            }

            // 2. garrison (all 'professional' troops)
            totalSoFar = 0;

            for (int i = 0; i < tempTroops.Length; i++)
            {
                // work out 'trained' troops numbers
                if (i < tempTroops.Length - 2)
                {
                    tempTroops[i] = Convert.ToUInt32(garrisonSize * (GameSettings.RECRUIT_RATIOS[thisNationality][i] * 2));
                }
                // fill up with foot
                else if (i < tempTroops.Length - 1)
                {
                    tempTroops[i] = garrisonSize - totalSoFar;
                }
                // no rabble in garrison
                else
                {
                    tempTroops[i] = 0;
                }

                troopsForArmy[i] += tempTroops[i];
                totalSoFar += tempTroops[i];
            }

            // create temporary army for battle/siege
            defender = new Army("Garrison" + IDGen.GetNextArmyID(), armyLeader, Owner, armyLeaderDays, this, Clock, IDGen, GameMap, Troops: troopsForArmy);
            //defender.AddArmy();
            Console.WriteLine("Removed AddArmy ?");

            return defender;
        }

        /// <summary>
        /// Bars a Character from the keep
        /// </summary>
        /// <returns>Bool indicating success</returns>
        /// <param name="toBeBarred">The Character to be barred</param>
        public bool BarCharacter(Character toBeBarred)
        {
            throw new NotImplementedException();
            /*
            bool success = true;
            string user = Owner.playerID;
            // add ID to barred characters
            this.barredCharacters.Add(toBeBarred.charID);

            // check if is currently in keep of barring fief, and remove if necessary
            if ((toBeBarred.inKeep) && (toBeBarred.location == this))
            {
                toBeBarred.inKeep = false;
                // update place owner
                if (!string.IsNullOrEmpty(user))
                {
                    Globals_Game.UpdatePlayer(user, DisplayMessages.FiefEjectCharacter, new string[] { toBeBarred.firstName + " " + toBeBarred.familyName, this.name });
                }
                // Get and notify owner of barred character that they have been ejected
                PlayerCharacter barredOwner = null;
                barredOwner = toBeBarred.GetHeadOfFamily();
                if (barredOwner == null)
                {
                    barredOwner = (toBeBarred as NonPlayerCharacter).GetEmployer();
                }
                if (barredOwner != null)
                {
                    if (barredOwner == toBeBarred) Globals_Game.UpdatePlayer(barredOwner.playerID, DisplayMessages.FiefEjectCharacter, new string[] { "You", this.name });
                    else Globals_Game.UpdatePlayer(barredOwner.playerID, DisplayMessages.FiefEjectCharacter, new string[] { toBeBarred.firstName + " " + toBeBarred.familyName, this.name });
                }
            }

            // check for success
            if (!this.barredCharacters.Contains(toBeBarred.charID))
            {
                success = false;
            }

            return success;*/
        }

        /// <summary>
        /// Unbars a Character
        /// </summary>
        /// <returns>Bool indicating success</returns>
        /// <param name="toBeUnbarred">ID of Character to be unbarred</param>
        public bool UnbarCharacter(Character toBeUnbarred)
        {
            throw new NotImplementedException();
            /*
            bool success = true;

            // check for empty string
            if (!String.IsNullOrWhiteSpace(toBeUnbarred))
            {
                // remove ID from barred characters
                if (this.barredCharacters.Contains(toBeUnbarred))
                {
                    this.barredCharacters.Remove(toBeUnbarred);
                }

                // check for success
                if (this.barredCharacters.Contains(toBeUnbarred))
                {
                    success = false;
                }
            }

            return success;*/
        }

        /// <summary>
        /// Bars a Nationality from entering the keep
        /// </summary>
        /// <returns>Bool indicating success</returns>
        /// <param name="toBeBarred">The ID of the Nationality to be barred</param>
        public void BarNationality(Nationality toBeBarred)
        {
            // add ID to barred nationalities
            BarredNationalities.Add(toBeBarred.NatID);
        }

        /// <summary>
        /// Unbars a Nationality from entering the keep
        /// </summary>
        /// <returns>Bool indicating success</returns>
        /// <param name="toBeUnbarred">The ID of the Nationality to be unbarred</param>
        public bool UnbarNationality(string toBeUnbarred)
        {
            bool success = true;

            // check for empty string
            if (!string.IsNullOrWhiteSpace(toBeUnbarred))
            {
                // remove ID from barred nationalities
                if (BarredNationalities.Contains(toBeUnbarred))
                {
                    BarredNationalities.Remove(toBeUnbarred);
                }

                // check for success
                if (BarredNationalities.Contains(toBeUnbarred))
                {
                    success = false;
                }
            }

            return success;
        }

        /// <summary>
        /// Returns descriptions of characters in Court, Tavern, outside keep for this fief
        /// </summary>
        /// <param name="place">String specifying whether court, tavern, outside keep</param>
        /// <param name="pc">PlayerCharacter viewing these details </param>
        public List<Character> ListCharsInMeetingPlace(string place, Character pc)
        {
            throw new NotImplementedException();
            /*
            List<ProtoCharacterOverview> charsToView = new List<ProtoCharacterOverview>();
            // select which characters to display - i.e. in the keep (court) or not (tavern)
            bool ifInKeep = false;
            if (place.Equals("court"))
            {
                ifInKeep = true;
            }

            // iterates through characters
            for (int i = 0; i < this.charactersInFief.Count; i++)
            {
                // only display characters in relevant location (in keep, or not)
                if (this.charactersInFief[i].inKeep == ifInKeep)
                {
                    // don't show the player or captive characters
                    if (this.charactersInFief[i] != pc && string.IsNullOrWhiteSpace(charactersInFief[i].captorID))
                    {
                        switch (place)
                        {
                            case "tavern":
                                // only show NPCs
                                if (this.charactersInFief[i] is NonPlayerCharacter)
                                {
                                    // only show unemployed
                                    if ((this.charactersInFief[i] as NonPlayerCharacter).salary == 0)
                                    {
                                        // Create an item and subitems for character
                                        charsToView.Add(new ProtoCharacterOverview(this.charactersInFief[i]));
                                    }
                                }
                                break;
                            default:
                                // Create an item and subitems for character
                                charsToView.Add(new ProtoCharacterOverview(this.charactersInFief[i]));
                                break;
                        }

                    }
                }

            }
            return charsToView.ToArray();*/
        }

    }
}
