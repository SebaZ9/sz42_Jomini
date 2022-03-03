using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JominiGame
{
    /// <summary>
    /// Class storing data on ailments effecting character health
    /// </summary>
    public class Ailment
    {
        /// <summary>
        /// Holds ailment ID
        /// </summary>
        public string AilmentID { get; set; }
        /// <summary>
        /// Holds ailment description
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Holds ailment date
        /// </summary>
        public string When { get; set; }
        /// <summary>
        /// Holds current ailment effect
        /// </summary>
        public uint Effect { get; set; }
        /// <summary>
        /// Holds minimum ailment effect
        /// </summary>
        public uint MinimumEffect { get; set; }

        /// <summary>
        /// Constructor for Ailment
        /// </summary>
        /// <param name="AilmentID">String holding ailment ID</param>
        /// <param name="Description">string holding ailment description</param>
        /// <param name="When">string holding ailment date</param>
        /// <param name="Effect">uint holding current ailment effect</param>
        /// <param name="MinimumEffect">uint holding minimum ailment effect</param>
        public Ailment(string AilmentID, string Description, string When, uint Effect, uint MinimumEffect)
        {
            this.AilmentID = AilmentID;
            this.Description = Description;
            this.When = When;
            this.Effect = Effect;
            this.MinimumEffect = MinimumEffect;
        }

        /// <summary>
        /// Updates the ailment, reducing effect where approprite
        /// </summary>
        /// <returns>bool indicating whether ailment should be deleted</returns>
        public bool UpdateAilment()
        {
            // reduce effect, if appropriate
            if (Effect > MinimumEffect)
            {
                Effect--;
            }

            // remove effect if has reached 0
            if (Effect == 0)
            {
                return true;
            } else
            {
                return false;
            }
        }
    }
}
