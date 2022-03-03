using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JominiGame
{
    public enum Stats : byte
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
