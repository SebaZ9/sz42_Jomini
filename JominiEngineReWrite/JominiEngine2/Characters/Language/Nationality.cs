using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JominiGame
{

    /// <summary>
    /// Class storing data on nationality
    /// </summary>
    public class Nationality
    {
        /// <summary>
        /// Holds nationality ID
        /// </summary>
        public string NatID { get; set; }
        /// <summary>
        /// Holds nationality name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Constructor for Nationality
        /// </summary>
        /// <param name="NatID">String holding nationality ID</param>
        /// <param name="Name">String holding nationality name</param>
        public Nationality(string NatID, string Name)
        {
            this.NatID = NatID;
            this.Name = Name;

        }
    }
}
