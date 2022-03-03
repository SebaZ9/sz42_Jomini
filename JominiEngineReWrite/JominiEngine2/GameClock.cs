using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JominiGame
{
    public class GameClock
    {
        /// <summary>
        /// Holds seasons
        /// </summary>
        public string[] Seasons = new string[4] { "Spring", "Summer", "Autumn", "Winter" };
        /// <summary>
        /// Holds current year
        /// </summary>
        public uint CurrentYear { get; set; }
        /// <summary>
        /// Holds current season
        /// </summary>
        public byte CurrentSeason { get; set; }

        /// <summary>
        /// Constructor for GameClock
        /// </summary>
		/// <param name="id">String holding clock ID</param>
        /// <param name="yr">uint holding starting year</param>
        /// <param name="s">byte holding current season (default: 0)</param>
        public GameClock(uint CurrentYear, byte CurrentSeason = 0)
        {
            this.CurrentYear = CurrentYear;
            this.CurrentSeason = CurrentSeason;
        }

        /// <summary>
        /// Calculates travel modifier for season
        /// </summary>
        /// <returns>double containing travel modifier</returns>
        public double CalcSeasonTravMod()
        {
            switch (CurrentSeason)
            {
                // spring
                case 0:
                    return 1.5;
                // winter
                case 3:
                    return 2;
                // summer & autumn
                default:
                    return 1;
            }
        }

        /// <summary>
        /// Advances GameClock to next season
        /// </summary>
        /// <returns>double containing travel modifier</returns>
        public void AdvanceSeason()
        {
            if (CurrentSeason == 3)
            {
                CurrentYear++;
                CurrentSeason = 0;
            }
            else
            {
                CurrentSeason++;
            }
        }

    }
}
