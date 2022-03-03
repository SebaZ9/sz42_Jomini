using JominiGame;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtoMessageClient
{
    [ProtoContract]
    public class ProtoPillageResult : ProtoMessage
    {
        [ProtoMember(1)]
        public string PillagedFiefID { get; set; }
        [ProtoMember(2)]
        public bool IsPillage { get; set; }
        [ProtoMember(3)]
        public string DefenderLeaderID { get; set; }
        [ProtoMember(4)]
        public string ArmyOwnerID { get; set; }
        [ProtoMember(5)]
        public string ArmyLeaderID { get; set; }
        [ProtoMember(6)]
        public double DaysTaken { get; set; }
        [ProtoMember(7)]
        public int PopulationLoss { get; set; }
        [ProtoMember(8)]
        public int TreasuryLoss { get; set; }
        [ProtoMember(9)]
        public double IndustryLoss { get; set; }
        [ProtoMember(10)]
        public double LoyaltyLoss { get; set; }
        [ProtoMember(11)]
        public double FieldsLoss { get; set; }
        [ProtoMember(12)]
        public double BaseMoneyPillaged { get; set; }
        [ProtoMember(13)]
        public double BonusMoneyPillaged { get; set; }
        [ProtoMember(14)]
        public double MoneyPillagedOwner { get; set; }
        [ProtoMember(15)]
        public double Jackpot { get; set; }
        [ProtoMember(16)]
        public double StatureModifier { get; set; }

        public ProtoPillageResult(PillageResult pillageResult)
        {
            PillagedFiefID = pillageResult.PillagedFief.ID;
            IsPillage = pillageResult.IsPillage;
            DefenderLeaderID = pillageResult.DefenderLeader.ID;
            ArmyOwnerID = pillageResult.ArmyOwner.ID;
            ArmyLeaderID = pillageResult.ArmyLeader.ID;
            DaysTaken = pillageResult.DaysTaken;
            PopulationLoss = pillageResult.PopulationLoss;
            TreasuryLoss = pillageResult.TreasuryLoss;
            IndustryLoss = pillageResult.IndustryLoss;
            LoyaltyLoss = pillageResult.LoyaltyLoss;
            FieldsLoss = pillageResult.FieldsLoss;
            BaseMoneyPillaged = pillageResult.BaseMoneyPillaged;
            BonusMoneyPillaged = pillageResult.BonusMoneyPillaged;
            MoneyPillagedOwner = pillageResult.MoneyPillagedOwner;
            Jackpot = pillageResult.Jackpot;
            StatureModifier = pillageResult.StatureModifier;
        }

        public ProtoPillageResult()
        {

        }

    }
}
