using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JominiGame
{
    public static class Birth
    {

        /// <summary>
        /// Generates a new NPC based on parents' statistics
        /// </summary>
        /// <returns>NonPlayerCharacter or null</returns>
        /// <param name="mummy">The new NPC's mother</param>
        /// <param name="daddy">The new NPC's father</param>
        public static NonPlayerCharacter GenerateNewNPC(string ID, Character Mother, Character Father, Tuple<uint, byte> BirthDate,
            GameClock Clock, IdGenerator IDGen, HexMapGraph GameMap)
        {
            bool IsMale = GenerateSex();
            NonPlayerCharacter newNPC = new NonPlayerCharacter(
                ID,
                "Baby",
                Father.FamilyName,
                BirthDate,
                IsMale,
                Father.Nationality,
                true,
                GenerateKeyCharacteristics(Mother.MaxHealth, Father.MaxHealth),
                GenerateKeyCharacteristics(Mother.Virility, Father.Virility),
                new Queue<Fief>(),
                Father.Language,
                90,
                0,
                GenerateKeyCharacteristics(Mother.Management, Father.Management),
                GenerateKeyCharacteristics(Mother.Combat, Father.Combat),
                GenerateTraitSetFromParents(Mother.Traits, Father.Traits, IsMale),
                Mother.InKeep,
                false,
                Father.FamilyID,
                null,
                Father,
                Mother,
                0,
                false,
                false,
                new List<Place>(),
                null,
                Clock,
                IDGen,
                GameMap

                );

            return newNPC;
        }

        /// <summary>
        /// Generates a random sex for a Character
        /// </summary>
        /// <returns>bool indicating whether is male</returns>
        public static bool GenerateSex()
        {
            bool isMale = false;

            // generate random (0-1) to see if male or female
            if (Random.Shared.Next(0, 2) == 0)
            {
                isMale = true;
            }

            return isMale;
        }

        /// <summary>
        /// Generates a characteristic stat for a Character, based on parent stats
        /// </summary>
        /// <returns>Double containing characteristic stat</returns>
        /// <param name="mummyStat">The mother's characteristic stat</param>
        /// <param name="daddyStat">The father's characteristic stat</param>
        public static double GenerateKeyCharacteristics(Double mummyStat, Double daddyStat)
        {
            Double newStat = 0;

            // get average of parents' stats
            Double parentalAverage = (mummyStat + daddyStat) / 2;

            // generate random (0 - 100) to determine relationship of new stat to parentalAverage
            double randPercentage = Random.Shared.NextDouble() * 100;

            // calculate new stat
            if (randPercentage <= 35)
            {
                newStat = parentalAverage;
            }
            else if (randPercentage <= 52.5)
            {
                newStat = parentalAverage - 1;
            }
            else if (randPercentage <= 70)
            {
                newStat = parentalAverage + 1;
            }
            else if (randPercentage <= 80)
            {
                newStat = parentalAverage - 2;
            }
            else if (randPercentage <= 90)
            {
                newStat = parentalAverage + 2;
            }
            else if (randPercentage <= 95)
            {
                newStat = parentalAverage - 3;
            }
            else
            {
                newStat = parentalAverage + 3;
            }

            // make sure new stat falls within acceptable range
            if (newStat < 1)
            {
                newStat = 1;
            }
            else if (newStat > 9)
            {
                newStat = 9;
            }

            return newStat;
        }

        /// <summary>
        /// Generates a trait set for a Character, based on parent traits
        /// </summary>
        /// <returns>Array containing trait set</returns>
        /// <param name="mummyTraits">The mother's traits</param>
        /// <param name="daddyTraits">The father's traits</param>
        /// <param name="isMale">Whether character is a male</param>
        public static Tuple<Trait, int>[] GenerateTraitSetFromParents(Tuple<Trait, int>[] mummyTraits, Tuple<Trait, int>[] daddyTraits, bool isMale)
        {
            // store all unique traitKeys from both parents
            List<string> uniqueTraitKeys = new List<string>();

            // mummy's traits
            for (int i = 0; i < mummyTraits.Length; i++)
            {
                uniqueTraitKeys.Add(mummyTraits[i].Item1.ID);
            }

            // daddy's traits
            for (int i = 0; i < daddyTraits.Length; i++)
            {
                if (!uniqueTraitKeys.Contains(daddyTraits[i].Item1.ID))
                {
                    uniqueTraitKeys.Add(daddyTraits[i].Item1.ID);
                }
            }

            // create new traits using uniqueTraitKeys
            Tuple<Trait, int>[] newTraits = null; //Utility_Methods.GenerateTraitSet(uniqueTraitKeys);
            Console.WriteLine("!!!--- TODO: Generate new traits on birth ---!!!");

            return newTraits;
        }

        /// <summary>
        /// Performs standard conditional checks before a pregnancy attempt
        /// </summary>
        /// <returns>bool indicating whether or not to proceed with pregnancy attempt</returns>
        /// <param name="husband">The husband</param>
        public static bool ChecksBeforePregnancyAttempt(Character husband)
        {
            ///error = null;
            bool proceed = true;
            bool isPlayer = husband is PlayerCharacter;
            bool isAncestorOfPlayer = (husband.GetHeadOfFamily()) is PlayerCharacter;
            if (!isPlayer && !isAncestorOfPlayer)
            {
                //TODO error log
                ///error = new ProtoMessage();
                ///error.ResponseType = DisplayMessages.CharacterProposalFamily;
                ///error.MessageFields = new string[] { "husband" };
                return false;
            }
            // Must be male, as I discovered when William Marshal got pregnant.
            if (!husband.IsMale)
            {
                ///error = new ProtoMessage();
                ///error.ResponseType = DisplayMessages.CharacterNotMale;
                return false;
            }

            // Husband cannot be a captive
            if (husband.Captor != null)
            {
                //error = new ProtoMessage();
                //error.ResponseType = DisplayMessages.CharacterHeldCaptive;
                return false;
            }

            // check is married
            // get spouse

            if (husband.Spouse != null)
            {
                // Husband cannot be a captive
                if (husband.Spouse.Captor != null)
                {
                    //error = new ProtoMessage();
                    //error.ResponseType = DisplayMessages.CharacterHeldCaptive;
                    return false;
                }
                // check to make sure is in same fief
                if (!(husband.Spouse.Location == husband.Location))
                {
                    if (isPlayer)
                    {
                        //error = new ProtoMessage();
                        //error.ResponseType = DisplayMessages.ErrorGenericNotInSameFief;
                    }
                    proceed = false;
                }

                else
                {
                    // make sure wife not already pregnant
                    if (husband.Spouse.IsPregnant)
                    {
                        if (isPlayer)
                        {
                            //error = new ProtoMessage();
                            //error.ResponseType = DisplayMessages.BirthAlreadyPregnant;
                            //error.MessageFields = new string[] { wife.firstName + " " + wife.familyName };
                        }
                        proceed = false;
                    }

                    // check if are kept apart by siege
                    else
                    {
                        if ((husband.Location.CurrentSiege != null) && (husband.InKeep != husband.Spouse.InKeep))
                        {
                            if (isPlayer)
                            {
                                //error = new ProtoMessage();
                                //error.ResponseType = DisplayMessages.BirthSiegeSeparation;
                            }
                            proceed = false;
                        }

                        else
                        {
                            // ensure player and spouse have at least 1 day remaining
                            double minDays = Math.Min(husband.Days, husband.Spouse.Days);

                            if (minDays < 1)
                            {
                                //error = new ProtoMessage();
                                //error.ResponseType = DisplayMessages.ErrorGenericNotEnoughDays;
                                proceed = false;
                            }
                            else
                            {
                                // ensure days are synchronised
                                if (husband.Days != husband.Spouse.Days)
                                {
                                    if (husband.Days != minDays)
                                    {
                                        if (husband is PlayerCharacter)
                                        {
                                            ((PlayerCharacter)husband).AdjustDays(husband.Days - minDays);
                                        }
                                        else
                                        {
                                            husband.AdjustDays(husband.Days - minDays);
                                        }
                                    }
                                    else
                                    {
                                        husband.Spouse.AdjustDays(husband.Spouse.Days - minDays);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            // If the husband is a player, alert the husband's user
            // otherwise, the husband is the son of a player- alert the husband's father's user
            else
            {
                string whoThisIs = "";
                if (isPlayer)
                {
                    whoThisIs = "You are ";
                }
                else
                {
                    whoThisIs = "This man is ";
                }
                //error = new ProtoMessage();
                //error.ResponseType = DisplayMessages.BirthNotMarried;
                //error.MessageFields = new string[] { whoThisIs };
                proceed = false;
            }

            return proceed;
        }

    }
}
