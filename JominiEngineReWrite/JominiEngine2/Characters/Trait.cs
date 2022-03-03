using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JominiGame
{
    /// <summary>
    /// Class storing data on a Trait
    /// </summary>
    public class Trait
    {
        /// <summary>
        /// Holds trait ID
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// Holds strait name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Holds trait effects
        /// </summary>
        public Dictionary<TraitStats, double> Effects;

        /// <summary>
        /// Constructor for Trait
        /// </summary>
        /// <param name="ID">String holding trait ID</param>
        /// <param name="Name">String holding trait name</param>
        /// <param name="Effects">Dictionary(string, double) holding trait effects</param>
        public Trait(string ID, string Name, Dictionary<TraitStats, double> Effects)
        {
            this.ID = ID;
            this.Name = Name;
            this.Effects = Effects;

        }

    }

    /// <summary>
    /// Enum representing character stats, which affect the success of certain actions and are affected by traits
    /// </summary>
    public enum TraitStats : byte
    {
        /// <summary>
        /// Affects how effecive character is in leading battle
        /// </summary>
        BATTLE,

        /// <summary>
        /// Affects character's effectiveness during a siege
        /// </summary>
        SIEGE,

        /// <summary>
        /// Affects price at which NPCs can be hired at
        /// </summary>
        NPCHIRE,

        /// <summary>
        /// Affects how many expenses will be paid to family
        /// </summary>
        FAMEXPENSE,

        /// <summary>
        /// Affects fief expenses
        /// </summary>
        FIEFEXPENSE,

        /// <summary>
        /// Affects fief loyalty
        /// </summary>
        FIEFLOY,

        /// <summary>
        /// Affects character's likelihood of dying
        /// </summary>
        DEATH,

        /// <summary>
        /// Affects how many days certain actions will take
        /// </summary>
        TIME,

        /// <summary>
        /// Affects how likely a character is to produce offspring
        /// </summary>
        VIRILITY,

        /// <summary>
        /// Affects how likely a character is to notice and prevent acts of subterfuge
        /// </summary>
        PERCEPTION,

        /// <summary>
        /// Affects how likely a character is to succeed in committing subterfuge
        /// </summary>
        STEALTH
    }

}
