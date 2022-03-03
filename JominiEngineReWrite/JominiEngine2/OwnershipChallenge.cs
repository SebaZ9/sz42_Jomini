using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JominiGame
{
    /// <summary>
    /// Struct storing data on ownership challenges (for Province or Kingdom)
    /// </summary>
    public class OwnershipChallenge
    {
        /// <summary>
        /// Holds ID of challenge
        /// </summary>
        public string ID;
        /// <summary>
        /// Holds ID of challenger
        /// </summary>
        public PlayerCharacter ChallengerPlayer { get; private set; }
        /// <summary>
        /// Holds type of place
        /// </summary>
        public string PlaceType;
        /// <summary>
        /// Holds ID of place
        /// </summary>
        public Place PlaceObj { get; private set; }
        /// <summary>
        /// Holds number of seasons so far that challenger has met ownership conditions
        /// </summary>
        public int Counter;

        /// <summary>
        /// Constructor for OwnershipChallenge
        /// </summary>
        /// <param name="ID">string holding challenge ID</param>
        /// <param name="ChallengerPlayer">string holding ID of challenger</param>
        /// <param name="PlaceType">string holding type of place</param>
        /// <param name="PlaceObj">string holding ID of place</param>
        public OwnershipChallenge(string ID, PlayerCharacter ChallengerPlayer, string PlaceType, Place PlaceObj)
        {           
            this.ID = ID;
            this.ChallengerPlayer = ChallengerPlayer;
            this.PlaceType = PlaceType;
            this.PlaceObj = PlaceObj;
            this.Counter = 0;
        }

    }
}
