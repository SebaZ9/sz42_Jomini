using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JominiGame
{
    /// <summary>
    /// Class storing data on province
    /// </summary>
    [Serializable()]
    public class Province : Place
    {
        /// <summary>
        /// Holds province tax rate
        /// </summary>
        public double TaxRate { get; set; }
        /// <summary>
        /// Holds province kingdom object
        /// </summary>
        public Kingdom ProvinceKingdom { get; set; }

        /// <summary>
        /// Constructor for Province
        /// </summary>
        /// <param name="TaxRate">Double holding province tax rate</param>
        /// <param name="ProvinceKingdom">Province's Kingdom object</param>
        public Province(string ID, string Name, double TaxRate, Character TitleHolder, PlayerCharacter Owner, Kingdom ProvinceKingdom, Rank PlaceRank,
            GameClock Clock, IdGenerator IDGen, HexMapGraph GameMap)
            : base(ID, Name, TitleHolder, Owner, PlaceRank, Clock, IDGen, GameMap)
        {
            this.TaxRate = TaxRate;
            this.ProvinceKingdom = ProvinceKingdom;
        }

        public Province(GameClock Clock, IdGenerator IDGen, HexMapGraph GameMap) : base(Clock, IDGen, GameMap)
        {

        }

        /// <summary>
        /// Adjusts province tax rate
        /// </summary>
        /// <param name="taxRate">double containing new tax rate</param>
        public void AdjustTaxRate(double taxRate)
        {
            // ensure max 100 and min 0
            if (taxRate > 100)
            {
                taxRate = 100;
            }
            else if (taxRate < 0)
            {
                taxRate = 0;
            }

            TaxRate = taxRate;
        }

        /// <summary>
        /// Gets the province's rightful kingdom (i.e. the kingdom that it traditionally belongs to)
        /// </summary>
        /// <returns>The kingdom</returns>
        public Kingdom GetRightfulKingdom()
        {
            Kingdom thisKingdom = null;

            if (ProvinceKingdom != null)
            {
                thisKingdom = ProvinceKingdom;
            }

            return thisKingdom;
        }

        /// <summary>
        /// Transfers ownership of the province to the specified PlayerCharacter
        /// </summary>
        /// <param name="newOwner">The new owner</param>
        public void TransferOwnership(PlayerCharacter newOwner)
        {
            // get current title holder
            Character titleHolder = Owner;

            // remove from current title holder's titles
            titleHolder.MyTitles.Remove(this);

            // add to newOwner's titles
            newOwner.MyTitles.Add(this);

            // update province titleHolder property
            TitleHolder = newOwner;

            // remove from current owner's ownedProvinces
            Owner.OwnedProvinces.Remove(this);

            // add to newOwner's ownedProvinces
            newOwner.OwnedProvinces.Add(this);

            // update province owner property
            Owner = newOwner;
        }

    }
}
