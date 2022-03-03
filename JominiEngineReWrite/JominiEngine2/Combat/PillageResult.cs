using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JominiGame
{
    public class PillageResult
    {
        public Fief PillagedFief { get; set; }
        public bool IsPillage { get; set; }
        
        public Character DefenderLeader { get; set; }
        public Character ArmyOwner { get; set; }
        public Character ArmyLeader { get; set; }

        public double DaysTaken { get; set; }
        public int PopulationLoss { get; set; }
        public int TreasuryLoss { get; set; }
        public double IndustryLoss { get; set; }
        public double LoyaltyLoss { get; set; }
        public double FieldsLoss { get; set; }
        public double BaseMoneyPillaged { get; set; }
        public double BonusMoneyPillaged { get; set; }
        public double MoneyPillagedOwner { get; set; }
        public double Jackpot { get; set; }
        public double StatureModifier { get; set; }

    }
}
