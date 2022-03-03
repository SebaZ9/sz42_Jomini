using System;
using System.IO;
using System.Runtime.Serialization;

namespace JominiGame
{
    public class Place : BaseGameObject
    {
        /// <summary>
        /// Holds place ID
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// Holds place name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Holds place owner (PlayerCharacter object)
        /// </summary>
        public PlayerCharacter Owner { get; set; }
        /// <summary>
        /// Holds place title holder (charID)
        /// </summary>
        public Character? TitleHolder { get; set; }
        /// <summary>
        /// Holds place rank (Rank object)
        /// </summary>
        public Rank PlaceRank { get; set; }

        /// <summary>
        /// Constructor for Place
        /// </summary>
        /// <param name="ID">String holding place ID</param>
        /// <param name="Name">String holding place name</param>
        /// <param name="TitleHolder">String holding place title holder (charID)</param>
        /// <param name="Owner">Place owner (PlayerCharacter)</param>
        /// <param name="PlaceRank">Place rank (Rank object)</param>
        public Place(string ID, string Name, Character TitleHolder, PlayerCharacter Owner, Rank PlaceRank, GameClock Clock, IdGenerator IDGen, HexMapGraph GameMap)
            : base(Clock, IDGen, GameMap)
        {
            this.ID = ID;
            this.Name = Name;
            this.Owner = Owner;
            this.TitleHolder = TitleHolder;
            this.PlaceRank = PlaceRank;
        }

        public Place(GameClock Clock, IdGenerator IDGen, HexMapGraph GameMap) : base(Clock, IDGen, GameMap)
        {

        }

    }

}