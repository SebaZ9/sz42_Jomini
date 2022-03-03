using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JominiGame
{
    /// <summary>
    /// Class storing data on terrain
    /// </summary>
    public class Terrain
    {

        /// <summary>
        /// Holds terrain ID
        /// </summary>
		public string ID { get; set; }
        /// <summary>
        /// Holds terrain description
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Holds terrain travel cost
        /// </summary>
        public double TravelCost { get; set; }

        /// <summary>
        /// Constructor for Terrain
        /// </summary>
		/// <param name="id">String holding terrain code</param>
        /// <param name="desc">String holding terrain description</param>
        /// <param name="tc">double holding terrain travel cost</param>
		public Terrain(string ID, string Description, double TravelCost)
        {            
            this.ID = ID;
            this.Description = Description;
            this.TravelCost = TravelCost;
        }

    }
}
