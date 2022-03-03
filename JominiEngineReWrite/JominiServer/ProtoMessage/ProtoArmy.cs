using JominiGame;
using ProtoBuf;
using ProtoMessageClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtoMessageClient
{
    /// <summary>
    /// Class for serializing an Army
    /// The amount of information a player can view about an army depends on whether that player 
    /// ownes the army, how close the player is etc. 
    /// Can be tuned later to include information obtained via methods such as spying, interrogation, or defection
    /// </summary>
    [ProtoContract]
    public class ProtoArmy : ProtoMessage
    {

        /// <summary>
        /// Holds army ID
        /// </summary>
        [ProtoMember(1)]
        public string ArmyID { get; set; }
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
        [ProtoMember(2)]
        public uint[] Troops;
        /// <summary>
        /// Holds army leader name
        /// </summary>
        [ProtoMember(3)]
        public string LeaderName { get; set; }
        /// <summary>
        /// Holds army leader ID
        /// </summary>
        [ProtoMember(4)]
        public string leaderID { get; set; }
        /// <summary>
        /// Holds army owner name
        /// </summary>
        [ProtoMember(5)]
        public string OwnerName { get; set; }
        /// <summary>
        /// Gets or sets the owner's character id
        /// </summary>
        [ProtoMember(6)]
        public string OwnerID { get; set; }
        /// <summary>
        /// Holds army's remaining days in season
        /// </summary>
        [ProtoMember(7)]
        public double Days { get; set; }
        /// <summary>
        /// Holds army location in the format:
        /// fiefID|fiefName|provinceName|kingdomName
        /// </summary>
        [ProtoMember(8)]
        public string LocationID { get; set; }
        /// <summary>
        /// Holds location name
        /// </summary>
        [ProtoMember(9)]
        public string LocationName { get; set; }
        /// <summary>
        /// Indicates whether army is being actively maintained by owner
        /// </summary>
        [ProtoMember(10)]
        public bool IsMaintained { get; set; }
        /// <summary>
        /// Indicates the army maintenance cost
        /// </summary>
        [ProtoMember(11)]
        public uint MaintenanceCost { get; set; }
        /// <summary>
        /// Indicates army's aggression level (automated response to combat)
        /// </summary>
        [ProtoMember(12)]
        public byte Aggression { get; set; }
        /// <summary>
        /// Indicates army's combat odds value (i.e. at what odds will attempt automated combat action)
        /// </summary>
        [ProtoMember(13)]
        public byte CombatOdds { get; set; }
        /// <summary>
        /// String indicating army nationality
        /// </summary>
        [ProtoMember(14)]
        public string Nationality { get; set; }
        /// <summary>
        /// Holds siege status of army
        /// One of BESIEGING, BESIEGED, FIEF or none
        /// BESIEGING: army is currently besieging fief
        /// BESIEGED: army is under siege
        /// FIEF: the fief the army is in is under siege
        /// </summary>
        [ProtoMember(15)]
        public string SiegeStatus { get; set; }

        public ProtoArmy() : base()
        {

        }

        public ProtoArmy(Army a, Character observer)
        {
            OwnerID = a.Owner.ID;
            OwnerName = a.Owner.FullName();
            ArmyID = a.ID;            
            if (a.Leader != null)
            {
                LeaderName = a.Leader.FullName();
                leaderID = a.Leader.ID;
            }
            LocationID = a.Location.Name + ", "
                + a.Location.FiefsProvince.Name + ", "
                + a.Location.FiefsProvince.ProvinceKingdom.Name;
            Nationality = a.Owner.Nationality.NatID;

            if (a.CheckIfSiegeDefenderGarrison() != null || a.CheckIfSiegeDefenderAdditional() != null)
            {
                SiegeStatus = "BESIEGED";
            }
            else
            {
                // if is besieger in a siege, indicate
                if (a.CheckIfBesieger() != null)
                {
                    SiegeStatus = "BESIEGER";
                }
                // check if is siege in fief (but army not involved)
                else
                {
                    if (a.Location.CurrentSiege != null)
                    {
                        SiegeStatus = "FIEF";
                    }
                }
            }
            Troops = a.GetTroopsEstimate(observer);
        }
        public void IncludeAll(Army a)
        {
            Aggression = a.Aggression;
            CombatOdds = a.CombatOdds;
            Troops = a.Troops;
            Days = a.Days;
            IsMaintained = a.IsMaintained;
            MaintenanceCost = a.GetMaintenanceCost();
        }

        public void IncludeSpy(Army a)
        {

        }

    }

    /// <summary>
    /// Class for summarising the important details of an army
    /// Can be used in conjunction with byte arrays to create a list of armies
    /// </summary>
    public class ProtoArmyOverview : ProtoMessage
    {
        // Holds army id
        public string ArmyID { get; set; }
        public string LeaderID { get; set; }
        public string LeaderName { get; set; }
        public string LocationID { get; set; }
        public string OwnerID { get; set; }
        public string OwnerName { get; set; }
        public uint ArmySize { get; set; }

        public ProtoArmyOverview()
            : base()
        {
        }
        public ProtoArmyOverview(Army a)
        {
            ArmyID = a.ID;
            if (a.Leader != null)
            {
                LeaderID = a.Leader.ID;
                LeaderName = a.Leader.FullName();
            }
            else
            {
                LeaderName = "No leader";
            }

            if (a.Owner != null)
            {
                OwnerID = a.Owner.ID;
                OwnerName = a.Owner.FullName();
            }
            else
            {                
                OwnerName = "No owner";
            }
        }

        public void IncludeAll(Army a)
        {
            LocationID = a.Location.ID;
            ArmySize = a.CalcArmySize();
        }

    }
}
