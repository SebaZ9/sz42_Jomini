using JominiGame;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtoMessageClient
{
    /// <summary>
    /// Class for sending fief details
    /// Province, language and terrain are all stored client side- unless this changes there is no need to send these
    /// </summary>
    [ProtoContract]
    public class ProtoFief : ProtoMessage
    {
        [ProtoMember(1)]
        public string FiefName { get; set; }

        /// <summary>
        /// ID of the fief
        /// </summary>
        [ProtoMember(2)]
        public string FiefID { get; set; }
        /// <summary>
        /// CharID and name of fief title holder
        /// </summary>
        [ProtoMember(3)]
        public string TitleHolderID { get; set; }
        /// <summary>
        /// Name of fief owner
        /// </summary>
        [ProtoMember(4)]
        public string OwnerName { get; set; }
        /// <summary>
        /// CharID of the fief owner
        /// </summary>
        [ProtoMember(5)]
        public string OwnerID { get; set; }
        /// <summary>
        /// Fief rank
        /// </summary>
        [ProtoMember(6)]
        public string FiefRank { get; set; }
        /// <summary>
        /// Holds fief population
        /// </summary>
        [ProtoMember(7)]
        public int Population { get; set; }
        /// <summary>
        /// Holds fief field level
        /// </summary>
        [ProtoMember(8)]
        public double Fields { get; set; }
        /// <summary>
        /// Holds fief industry level
        /// </summary>
        [ProtoMember(9)]
        public double Industry { get; set; }
        /// <summary>
        /// Holds number of troops in fief
        /// </summary>
        [ProtoMember(10)]
        public uint Troops { get; set; }
        /// <summary>
        /// Holds number of troops that can be recruited in this fief
        /// </summary>
        [ProtoMember(11)]
        public int Militia { get; set; }
        /// <summary>
        /// Holds fief tax rate
        /// </summary>
        [ProtoMember(12)]
        public double TaxRate { get; set; }
        /// <summary>
        /// Holds fief tax rate (next season)
        /// </summary>
        [ProtoMember(13)]
        public double TaxRateNext { get; set; }
        /// <summary>
        /// Holds expenditure on officials (next season)
        /// </summary>
        [ProtoMember(14)]
        public uint OfficialsSpendNext { get; set; }
        /// <summary>
        /// Holds expenditure on garrison (next season)
        /// </summary>
        [ProtoMember(15)]
        public uint GarrisonSpendNext { get; set; }
        /// <summary>
        /// Holds expenditure on infrastructure (next season)
        /// </summary>
        [ProtoMember(16)]
        public uint InfrastructureSpendNext { get; set; }
        /// <summary>
        /// Holds expenditure on keep (next season)
        /// </summary>
        [ProtoMember(17)]
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
        [ProtoMember(18)]
        public double[] KeyStatsCurrent;
        /// <summary>
        /// Holds key data for previous season
        /// </summary>
        [ProtoMember(19)]
        public double[] KeyStatsPrevious;
        /// <summary>
        /// Holds key data for next season
        /// </summary>
        [ProtoMember(20)]
        public double[] KeyStatsNext;
        /// <summary>
        /// Holds fief keep level
        /// </summary>
        [ProtoMember(21)]
        public double KeepLevel { get; set; }
        /// <summary>
        /// Holds fief loyalty
        /// </summary>
        [ProtoMember(22)]
        public double Loyalty { get; set; }
        /// <summary>
        /// Holds fief status (calm, unrest, rebellion)
        /// </summary>
        [ProtoMember(23)]
        public char Status { get; set; }
        /// <summary>
        /// Holds overviews of characters present in fief
        /// </summary>
        [ProtoMember(24)]
        public ProtoCharacterOverview[] CharactersInFief { get; set; }
        /// <summary>
        /// Holds characters banned from keep (charIDs)
        /// </summary>
        [ProtoMember(25)]
        public ProtoCharacterOverview[] BarredCharacters { get; set; }
        /// <summary>
        /// Holds nationalities banned from keep (IDs)
        /// </summary>
        [ProtoMember(26)]
        public string[] BarredNationalities { get; set; }
        /// <summary>
        /// Holds fief ancestral owner (PlayerCharacter object)
        /// </summary>
        [ProtoMember(27)]
        public ProtoCharacterOverview AncestralOwner { get; set; }
        /// <summary>
        /// Holds fief bailiff (Character object)
        /// </summary>
        [ProtoMember(28)]
        public ProtoCharacterOverview Bailiff { get; set; }
        /// <summary>
        /// Number of days the bailiff has been resident in the fief (this season)
        /// </summary>
        [ProtoMember(29)]
        public double BailiffDaysInFief { get; set; }
        /// <summary>
        /// Holds fief treasury
        /// </summary>
        [ProtoMember(30)]
        public int Treasury { get; set; }
        /// <summary>
        /// Holds overviews of armies present in the fief (armyIDs)
        /// </summary>
        [ProtoMember(31)]
        public ProtoArmyOverview[] Armies { get; set; }
        /// <summary>
        /// Identifies if recruitment has occurred in the fief in the current season
        /// </summary>
        [ProtoMember(32)]
        public bool HasRecruited { get; set; }
        /// <summary>
        /// Identifies if pillage has occurred in the fief in the current season
        /// </summary>
        [ProtoMember(33)]
        public bool IsPillaged { get; set; }
        /// <summary>
        /// Siege (siegeID) of active siege
        /// </summary>
        [ProtoMember(34)]
        public string SiegeID { get; set; }
        /// <summary>
        /// List of characters held captive in fief
        /// </summary>
        [ProtoMember(35)]
        public ProtoCharacterOverview[] Gaol { get; set; }

        public ProtoFief()
            : base()
        {

        }
        public ProtoFief(Fief f, Game game)
        {
            FiefName = f.Name;
            Militia = f.CalcMaxTroops();
            FiefID = f.ID;
            if (f.TitleHolder != null)
            {
                TitleHolderID = f.TitleHolder.ID;
            }
            else
            {
                this.TitleHolderID = "None";
            }
            OwnerName = f.Owner.FullName();
            OwnerID = f.Owner.ID;
            FiefRank = f.PlaceRank.GetName(f.FiefsLanguage);
            Population = f.Population;
            Status = f.Status;

            BarredCharacters = new ProtoCharacterOverview[f.BarredCharacters.Count];
            int i = 0;
            foreach (string cID in f.BarredCharacters)
            {                
                Character c = game.GetCharacter(cID);
                if (c != null)
                {
                    BarredCharacters[i] = new ProtoCharacterOverview(c, game);
                    i++;
                }
            }
            i = 0;

            BarredNationalities = f.BarredNationalities.ToArray();
            CharactersInFief = new ProtoCharacterOverview[f.CharactersInFief.Count];
            foreach (Character c in f.CharactersInFief)
            {
                CharactersInFief[i] = new ProtoCharacterOverview(c, game);
                i++;
            }

            i = 0;
            AncestralOwner = new ProtoCharacterOverview(f.AncestralOwner, game);
            if (f.Bailiff != null) {
                Bailiff = new ProtoCharacterOverview(f.Bailiff, game);
            }

            IsPillaged = f.IsPillaged;
            SiegeID = f.CurrentSiege.ID;
            Armies = new ProtoArmyOverview[f.Armies.Count];
            foreach (Army a in f.Armies)
            {
                Armies[i] = new ProtoArmyOverview(a);
                i++;
            }
            HasRecruited = f.HasRecruited;
        }

        /// KeyStats ( key data for season )
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
        /// <summary>
        /// Includes all data in the ProtoMessage (useful for fief 
        /// </summary>
        /// <param name="f"></param>
        public void includeAll(Fief f)
        {
            KeyStatsCurrent = new double[14];
            KeyStatsNext = new double[14]; ;
            KeyStatsPrevious = new double[14];

            Fields = f.Fields;
            Industry = f.Industry;
            Troops = f.Troops;
            TaxRate = f.TaxRate;
            KeepLevel = f.KeepLevel;
            KeyStatsNext[0] = f.CalcNewLoyalty();
            KeyStatsNext[1] = f.CalcNewGDP();
            KeyStatsNext[2] = f.TaxRateNext;
            //Console.WriteLine("Tax rate next: " + f.taxRateNext);
            KeyStatsNext[3] = f.OfficialSpendNext;
            KeyStatsNext[4] = f.GarrisonSpendNext;
            KeyStatsNext[5] = f.InfrastructureSpendNext;
            KeyStatsNext[6] = f.KeepSpendNext;
            KeyStatsNext[7] = f.CalcNewKeepLevel();
            KeyStatsNext[8] = f.CalcNewIncome();
            KeyStatsNext[9] = f.CalcFamilyExpenses();
            KeyStatsNext[10] = f.CalcNewExpenses() + f.CalcFamilyExpenses();
            KeyStatsNext[11] = f.CalcNewOlordTaxes();
            KeyStatsNext[12] = f.FiefsProvince.TaxRate;
            KeyStatsNext[13] = f.CalcNewBottomLine();

            KeyStatsCurrent = f.KeyStatsCurrent;
            KeyStatsPrevious = f.KeyStatsPrevious;
            Loyalty = f.Loyalty;
            BailiffDaysInFief = f.BailiffDaysInFief;
            Treasury = f.GetAvailableTreasury(true);

        }

        public void includeSpy(Fief f, Game game)
        {
            Fields = f.Fields;
            Industry = f.Industry;
            Troops = f.Troops;
            TaxRate = f.TaxRate;
            KeepLevel = f.KeepLevel;
            Gaol = new ProtoCharacterOverview[f.Gaol.Count];
            int i = 0;
            foreach (Character captive in f.Gaol)
            {
                Gaol[i] = new ProtoCharacterOverview(captive, game);
                i++;
            }
        }

    }
}
