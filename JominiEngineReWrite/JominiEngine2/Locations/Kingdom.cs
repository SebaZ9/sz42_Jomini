using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JominiGame
{
    public class Kingdom : Place
    {

        /// <summary>
        /// Holds Kingdom nationality
        /// </summary>
        public Nationality? KingdomNationality { get; set; }

        /// <summary>
        /// Constructor for Kingdom
        /// </summary>
        /// <param name="nat">Kingdom's Nationality object</param>
        public Kingdom(string ID, string Name, Nationality KingdomNationality, Character TitleHolder, PlayerCharacter Owner, Rank PlaceRank,
            GameClock Clock, IdGenerator IDGen, HexMapGraph GameMap)
            : base(ID, Name, TitleHolder, Owner, PlaceRank, Clock, IDGen, GameMap)
        {
            this.KingdomNationality = KingdomNationality;
        }

        public Kingdom(GameClock Clock, IdGenerator IDGen, HexMapGraph GameMap) : base(Clock, IDGen, GameMap)
        {

        }

    }
}
