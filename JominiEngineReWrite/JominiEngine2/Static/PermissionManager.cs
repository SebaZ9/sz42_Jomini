﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JominiGame
{
    public static class PermissionManager
    {

        public delegate bool AuthorizeDelegate(PlayerCharacter pc, object o = null);

        public static bool OwnsArmy(PlayerCharacter pc, object o)
        {
            if (o == null) return false;
            Army army = (Army)o;
            return (army.Owner == pc);
        }

        public static bool OwnsFief(PlayerCharacter pc, object o)
        {
            if (o == null) return false;
            Fief fief = (Fief)o;
            return (fief.Owner == pc);
        }

        public static bool OverlordOfFief(PlayerCharacter pc, object o)
        {
            if (o == null) return false;
            Fief fief = (Fief)o;
            throw new NotImplementedException();
            //return (fief.Overlord == pc);
        }

        public static bool HeadOfFamily(PlayerCharacter pc, object o)
        {
            if (o == null) return false;
            Character c = (Character)o;
            return (c.GetHeadOfFamily() == pc);
        }

        public static bool isAdmin(PlayerCharacter pc, object o = null)
        {
            throw new NotImplementedException();
            //return pc.CheckIsSysAdmin();
        }
        public static bool isKing(PlayerCharacter pc, object o = null)
        {
            throw new NotImplementedException();
            //return pc.CheckIsKing();
        }
        public static bool isHerald(PlayerCharacter pc, object o = null)
        {
            throw new NotImplementedException();
            //return pc.CheckIsHerald();
        }
        public static bool isPrince(PlayerCharacter pc, object o = null)
        {
            throw new NotImplementedException();
            //return pc.CheckIsPrince();
        }
        public static bool isAlive(PlayerCharacter pc, object o = null)
        {
            return pc.IsAlive;
        }

        /// <summary>
        /// Determines whether a player is authorised to perform an action
        /// </summary>
        /// <param name="delegates">delegate methods for determining authorization conditions</param>
        /// <param name="pc">Player Character to be authorised</param>
        /// <param name="o">Object PlayerCharacter requires permission to act on (can be null)</param>
        /// <returns></returns>
        public static bool isAuthorized(AuthorizeDelegate[] delegates, PlayerCharacter pc, Object o)
        {
            bool b = false;
            foreach (AuthorizeDelegate d in delegates)
            {
                // Must be satisfy at least one of the delegates
                b = b || d(pc, o);
            }
            // Must be alive
            b = b && pc.IsAlive;
            return b;
        }
        /// <summary>
        /// Method to determine if player has permission to view fief
        /// </summary>
        /// <param name="pc">PlayerCharacter who wants to view fief</param>
        /// <param name="o">Fief to view</param>
        /// <returns>Whether or not a character can see a fief</returns>
        public static bool canSeeFief(PlayerCharacter pc, object o)
        {
            Fief f = o as Fief;
            if (pc.OwnedFiefs.Contains(f))
            {
                return true;
            }
            bool isInFief = (pc.Location == f);
            // Note: Captives cannot see anything
            if (isInFief && pc.Captor == null) return true;
            foreach (Character character in pc.MyNPCs)
            {
                if (character.Location == f && character.Captor == null)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Checks whether a character can see an army
        /// </summary>
        /// <param name="pc">Head of family attempting to view army</param>
        /// <param name="o">Army</param>
        /// <returns>Whetehr or not army can be seen</returns>
        public static bool canSeeArmy(PlayerCharacter pc, object o)
        {
            // Can see army if owns army or a character can see fief army is in
            Army army = (Army)o;
            if (OwnsArmy(pc, o)) return true;
            else
            {
                if (canSeeFief(pc, army.Location))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        /// <summary>
        /// Method to determine if a PlayerCharacter owns, or is, a character
        /// </summary>
        /// <param name="pc">PlayerCharacter who is/owns </param>
        /// <param name="o"></param>
        /// <returns></returns>
        public static bool ownsCharacter(PlayerCharacter pc, object o)
        {
            Character character = (Character)o;
            if (character is PlayerCharacter)
            {
                bool owns = (character as PlayerCharacter) == pc;
                return owns;
            }
            else
            {
                bool owns = ((NonPlayerCharacter)character).GetHeadOfFamily() == pc || ((NonPlayerCharacter)character).Employer == pc;
                return owns;
            }
        }

        public static bool ownsCharNotCaptured(PlayerCharacter pc, object o)
        {
            Character character = (Character)o;
            if (!ownsCharacter(pc, o))
            {
                return false;
            }
            else if (character.Captor != null) return false;
            else return true;
        }

        public static AuthorizeDelegate[] ownsCharOrAdmin = { isAdmin, ownsCharacter };
        public static AuthorizeDelegate[] ownsCharNotCapturedOrAdmin = { ownsCharNotCaptured, isAdmin };
        public static AuthorizeDelegate[] ownsFiefOrAdmin = { OwnsFief, isAdmin };
        public static AuthorizeDelegate[] ownsArmyOrAdmin = { OwnsArmy, isAdmin };
        public static AuthorizeDelegate[] canSeeFiefOrAdmin = { canSeeFief, isAdmin };
        public static AuthorizeDelegate[] canSeeArmyOrAdmin = { canSeeArmy, isAdmin };

    }
}
