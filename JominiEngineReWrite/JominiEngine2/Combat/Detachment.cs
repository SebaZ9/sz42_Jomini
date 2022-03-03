using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JominiGame
{
    /// <summary>
    /// Class for sending details of a detachment
    /// Character ID of PlayerCharacter leaving detachment is obtained via connection details
    /// </summary>
    public class Detachment
    {

        // ID of detachment
        public string ID { get; set; }
        /// <summary>
        /// Array of troops (size = 7)
        /// </summary>
        public uint[] Troops;
        /// <summary>
        /// Character detachment is left for
        /// </summary>
        public string LeftFor { get; set; }
        /// <summary>
        /// ArmyID of army from which detachment was created
        /// </summary>
        public Army FromArmy { get; set; }
        /// <summary>
        /// Details of person who left this detachment (used in sending details of detachments to client)
        /// </summary>
        public Character LeftBy { get; set; }
        /// <summary>
        /// Days left of person who created detachment at time of creation
        /// </summary>
        public int Days { get; set; }

        public Detachment(string ID, uint[] Troops, string LeftFor, Army FromArmy, Character LeftBy)
        {
            this.ID = ID;
            this.Troops = Troops;
            this.LeftFor = LeftFor;
            this.FromArmy = FromArmy;
            this.LeftBy = LeftBy;
        }

    }
}
