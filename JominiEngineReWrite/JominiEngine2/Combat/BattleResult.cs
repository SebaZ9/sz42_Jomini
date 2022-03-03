using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JominiGame
{
    public class BattleResults
    {
        /// <summary>
        /// Full name of PlayerCharacter owning the attacking Army
        /// </summary>
        public PlayerCharacter AttackerOwner;
        /// <summary>
        /// Full name of PlayerCharacter owning the defending Army
        /// </summary>
        public PlayerCharacter DefenderOwner;

        /// <summary>
        /// ID of Fief where battle took place
        /// </summary>
        public Fief BattleLocation;

        /// <summary>
        /// Boolean indicating whether a battle took place (armies can retreat instead)
        /// </summary>
        public bool BattleTookPlace;

        /// <summary>
        /// Boolean indicating whether the attacker was victorious
        /// </summary>
        public bool AttackerVictorious;
        /// <summary>
        /// Holds the names of owners of all disbanded armies
        /// </summary>
        public string[] DisbandedArmies;
        /// <summary>
        /// Holds the names of owners of all retreated armies
        /// </summary>
        public string[] RetreatedArmies;
        /// <summary>
        /// Holds the full names of all Characters who died as a result of the battle
        /// </summary>
        public string[] Deaths;
        /// <summary>
        /// If the defender leader during a siege dies, and there is no heir to take over, then even if the defenders win the siege is raised
        /// </summary>
        public bool DefenderDeadNoHeir;
        /// <summary>
        /// Boolean indicating whether the siege was raised
        /// </summary>
        public bool SiegeRaised;
        /// <summary>
        /// Number of troop casualties incurred for the defender
        /// </summary>
        public uint DefenderCasualties;
        /// <summary>
        /// Number of troop casualties incurred for the attacker
        /// </summary>
        public uint AttackerCasualties;
        /// <summary>
        /// Boolean indicating whether or not this battle took place as a result of being a siege
        /// </summary>
        public bool IsSiege;
        /// <summary>
        /// The full name of the Character besieging
        /// </summary>
        public Character SiegeBesieger;
        /// <summary>
        /// The full name of the Character defending
        /// </summary>
        public Character SiegeDefender;
        /// <summary>
        /// 0 = normal battle, 1 = pillage, 2 = siege
        /// </summary>
        public byte Circumstance;
        /// <summary>
        /// Indicates how much stature the attacker has gained/lost
        /// </summary>
        public double StatureChangeAttacker;
        /// <summary>
        /// Indicates how much stature the defender has gained/lost
        /// </summary>
        public double StatureChangeDefender;
    }
}
