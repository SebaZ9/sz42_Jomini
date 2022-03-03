using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JominiGame
{
    /// <summary>
    /// Class storing data on NonPlayerCharacter
    /// </summary>
    public class NonPlayerCharacter : Character
    {

        /// <summary>
        /// Holds NPC's employer (charID)
        /// </summary>
        public PlayerCharacter? Employer { get; set; }
        /// <summary>
        /// Holds NPC's salary
        /// </summary>
        public uint Salary { get; set; }
        /// <summary>
        /// Holds last wage offer from individual PCs
        /// </summary>
        public Dictionary<string, uint> LastOffer { get; set; }
        /// <summary>
        /// Denotes if in employer's entourage
        /// </summary>
        public bool InEntourage { get; set; }
        /// <summary>
        /// Denotes if is player's heir
        /// </summary>
        public bool IsHeir { get; set; }

        /// <summary>
        /// Constructor for NonPlayerCharacter
        /// </summary>
        /// <param name="empl">string holding NPC's employer (charID)</param>
        /// <param name="sal">string holding NPC's wage</param>
        /// <param name="inEnt">bool denoting if in employer's entourage</param>
        /// <param name="isH">bool denoting if is player's heir</param>
        public NonPlayerCharacter(string ID, string FirstName, string FamilyName, Tuple<uint, byte> BirthDate, bool IsMale,
            Nationality Nationality, bool IsAlive, double MaxHealth, double Virtility, Queue<Fief> GoTo, Language Language, double Days,
            double Stature, double Managment, double Combat, Tuple<Trait, int>[] Traits, bool InKeep, bool IsPregnant, string FamilyID,
            Character Spouse, Character Father, Character Mother, uint Salary, bool InEntourage, bool IsHeir, List<Place> MyTitles, Character Fiancee,
            GameClock Clock, IdGenerator IDGen, HexMapGraph GameMap,
            Dictionary<string, Ailment>? Ailments = null, Fief? Location = null, Army? ArmyID = null, PlayerCharacter? Employer = null)
            : base(ID, FirstName, FamilyName, BirthDate, IsMale, Nationality, IsAlive, MaxHealth, Virtility, GoTo, Language,
                  Days, Stature, Managment, Combat, Traits, InKeep, IsPregnant, FamilyID, Spouse, Father, Mother, MyTitles, Fiancee, Clock, IDGen, GameMap, Ailments, Location, ArmyID)
        {            
            this.Employer = Employer;
            this.Salary = Salary;
            this.InEntourage = InEntourage;
            LastOffer = new Dictionary<string, uint>();
            this.IsHeir = IsHeir;
        }

        public NonPlayerCharacter(GameClock Clock, IdGenerator IDGen, HexMapGraph GameMap) : base(Clock, IDGen, GameMap)
        {

        }

        /// <summary>
        /// Sets entourage value
        /// </summary>
        /// <param name="inEntourage"></param>
        public void setEntourage(bool inEntourage)
        {
            InEntourage = inEntourage;
        }
        /// <summary>
        /// Removes character from entourage
        /// </summary>
        public void removeSelfFromEntourage()
        {
            PlayerCharacter? pc = Employer as PlayerCharacter;
            if (pc == null)
            {
                pc = GetHeadOfFamily();
            }
            if (pc == null && InEntourage)
            {
                throw new Exception("Entourage discrepancy");
            }
            if (pc != null)
            {
                pc.RemoveFromEntourage(this);
            }
        }

        /// <summary>
        /// Calculates the family allowance of a family NPC, based on age and function
        /// </summary>
        /// <returns>uint containing family allowance</returns>
        /// <param name="func">NPC's function</param>
        public uint CalcFamilyAllowance(string func)
        {
            uint famAllowance = 0;
            double ageModifier = 1;

            // factor in family function
            if (func.ToLower().Equals("wife"))
            {
                famAllowance = 30000;
            }
            else
            {
                if (func.ToLower().Contains("heir"))
                {
                    famAllowance = 40000;
                }
                else if (func.ToLower().Equals("son"))
                {
                    famAllowance = 20000;
                }
                else if (func.ToLower().Equals("daughter"))
                {
                    famAllowance = 15000;
                }
                else
                {
                    famAllowance = 10000;
                }
                // calculate age modifier
                if ((Age <= 7))
                {
                    ageModifier = 0.25;
                }
                else if ((Age > 7) && (Age <= 14))
                {
                    ageModifier = 0.5;
                }
                else if ((Age > 14) && (Age <= 21))
                {
                    ageModifier = 0.75;
                }

                // apply age modifier
                famAllowance = Convert.ToUInt32(famAllowance * ageModifier);
            }

            return famAllowance;
        }

        /// <summary>
        /// Derives NPC function
        /// </summary>
        /// <returns>string containing NPC function</returns>
        /// <param name="pc">PlayerCharacter with whom NPC has relationship</param>
        public string GetFunction(PlayerCharacter pc)
        {
            string myFunction = "";

            // check for employees
            if (Employer != null)
            {
                if (Employer.Equals(pc))
                {
                    myFunction = "Employee";
                }
            }

            // check for family function
            else if ((!string.IsNullOrWhiteSpace(FamilyID)) && (FamilyID.Equals(pc.FamilyID)))
            {
                // default value
                myFunction = "Family Member";


                if (Father != null)
                {
                    // sons & daughters
                    if (Father.Equals(pc))
                    {
                        if (IsMale)
                        {
                            myFunction = "Son";
                        }
                        else
                        {
                            myFunction = "Daughter";
                        }
                    }

                    // brothers and sisters
                    if (pc.Father != null)
                    {
                        if (Father.Equals(pc.Father))
                        {
                            if (IsMale)
                            {
                                myFunction = "Brother";
                            }
                            else
                            {
                                myFunction = "Sister";
                            }
                        }
                    }

                    // uncles and aunts
                    if (pc.Father != null && pc.Father.Father != null)
                    {
                        if (Father.Equals(pc.Father.Father))
                        {
                            if (IsMale)
                            {
                                myFunction = "Uncle";
                            }
                            else
                            {
                                myFunction = "Aunt";
                            }
                        }
                    }

                    if (Father.Father != null)
                    {
                        // grandsons & granddaughters
                        if (Father.Father.Equals(pc))
                        {
                            if (IsMale)
                            {
                                myFunction = "Grandson";
                            }
                            else
                            {
                                myFunction = "Granddaughter";
                            }
                        }
                    }
                }

                // if haven't found function yet
                if (myFunction.Equals("Family Member"))
                {
                    // sons and daughters (just in case only mother recorded)
                    if (Mother != null && pc.Spouse != null)
                    {
                        if (Mother.Equals(pc.Spouse))
                        {
                            if (IsMale)
                            {
                                myFunction = "Son";
                            }
                            else
                            {
                                myFunction = "Daughter";
                            }
                        }
                    }

                    // grandmother
                    if (pc.Father != null)
                    {
                        if (pc.Father.Mother != null && pc.Father.Mother.Equals(this))
                        {
                            myFunction = "Grandmother";
                        }
                    }

                    if (pc.Mother != null && pc.Mother.Equals(this))
                    {
                        // mother
                        myFunction = "Mother";
                    }

                    // wife
                    if (Spouse != null && Spouse.Equals(pc))
                    {
                        if (IsMale)
                        {
                            myFunction = "Husband";
                        }
                        else
                        {
                            myFunction = "Wife";
                        }
                    }

                    if (Spouse != null && Spouse.Father != null)
                    {
                        if (Spouse.Father.Equals(pc))
                        {
                            myFunction = "Daughter-in-law";
                        }
                    }

                    // check for heir
                    if (IsHeir)
                    {
                        myFunction += " & Heir";
                    }
                }
            }

            return myFunction;
        }

        /// <summary>
        /// Gets an NPC's employment responsibilities
        /// </summary>
        /// <returns>string containing NPC responsibilities</returns>
        /// <param name="pc">PlayerCharacter by whom NPC is employed</param>
        public string GetResponsibilities(PlayerCharacter pc)
        {
            string myResponsibilities = "";
            List<Fief> bailiffDuties = new List<Fief>();

            // check for employment function
            if (Employer != null && Employer.Equals(pc)
                || ((!string.IsNullOrWhiteSpace(FamilyID)) && FamilyID.Equals(pc.FamilyID)))
            {
                // check PC's fiefs for bailiff
                foreach (Fief thisFief in pc.OwnedFiefs)
                {
                    if (thisFief.Bailiff == this)
                    {
                        bailiffDuties.Add(thisFief);
                    }
                }

                // create entry for bailiff duties
                if (bailiffDuties.Count > 0)
                {
                    myResponsibilities += "Bailiff (";
                    for (int i = 0; i < bailiffDuties.Count; i++)
                    {
                        myResponsibilities += bailiffDuties[i].ID;
                        if (i < (bailiffDuties.Count - 1))
                        {
                            myResponsibilities += ", ";
                        }
                    }
                    myResponsibilities += ")";
                }

                // check for army leadership
                if (ArmyID != null)
                {
                    if (!string.IsNullOrWhiteSpace(myResponsibilities))
                    {
                        myResponsibilities += ". ";
                    }
                    myResponsibilities += "Army leader (" + ArmyID + ").";
                }

                // if employee who isn't bailiff or army leader = 'Unspecified'
                if (string.IsNullOrWhiteSpace(myResponsibilities))
                {
                    if (Employer != null)
                    {
                        if (Employer.Equals(pc))
                        {
                            myResponsibilities = "Unspecified";
                        }
                    }
                }
            }

            return myResponsibilities;
        }


        /// <summary>
        /// Calculates the potential salary (per season) for the NonPlayerCharacter, based on his current salary
        /// </summary>
        /// <returns>double containing salary</returns>
        public double CalcSalary_BaseOnCurrent()
        {
            double salary = 0;

            // NPC will only accept a minimum offer of 5% above his current salary
            salary = Salary + (Salary * 0.05);

            // use minimum figure to calculate median salary to use as basis for negotiations
            // (i.e. the minimum figure is 90% of the median salary)
            salary += salary * 0.11;

            return salary;
        }

        /// <summary>
        /// Calculates the potential salary (per season) for the NonPlayerCharacter, based on his traits
        /// </summary>
        /// <returns>uint containing salary</returns>
        public double CalcSalary_BaseOnTraits()
        {
            double salary = 0;
            double basicSalary = 1500;

            // get fief management rating
            double fiefMgtRating = CalcFiefManagementRating();

            // get army leadership rating
            double armyLeaderRating = CalcArmyLeadershipRating();

            // determine lowest of 2 ratings
            double minRating = Math.Min(armyLeaderRating, fiefMgtRating);

            // determine highest of 2 ratings
            double maxRating = Math.Max(armyLeaderRating, fiefMgtRating);
            if (maxRating < 0)
            {
                maxRating = 0;
            }

            // calculate potential salary, based on highest rating
            salary = basicSalary * maxRating;
            // if appropriate, also including 'flexibility bonus' for lowest rating
            if (minRating > 0)
            {
                salary += basicSalary * (minRating / 2);
            }

            return salary;
        }

        /// <summary>
        /// Gets the potential salary (per season) for the NonPlayerCharacter,
        /// taking into account the stature of the hiring PlayerCharacter
        /// </summary>
        /// <returns>uint containing salary</returns>
        /// <param name="hiringPlayer">Hiring player</param>
        public uint CalcSalary(PlayerCharacter hiringPlayer)
        {
            // get potential salary based on NPC's traits
            double salary_traits = CalcSalary_BaseOnTraits();

            // get potential salary based on NPC's current salary
            double salary_current = 0;
            if (Salary > 0)
            {
                salary_current = CalcSalary_BaseOnCurrent();
            }

            // use maximum of the two salary calculations
            double salary = Math.Max(salary_traits, salary_current);

            // factor in hiring PC's stature and current employer's stature (if applicable)
            // (4% reduction in NPC's salary for each stature rank above 4)
            double statMod = 0;

            // hiring PC
            double hirerStatMod = 0;
            if (hiringPlayer.CalculateStature() > 4)
            {
                hirerStatMod = (hiringPlayer.CalculateStature() - 4) * 0.04;
            }

            // current employer (note: is made negative to counteract hiring PC's stature effect)
            double emplStatMod = 0;
            if (Employer != null)
            {
                if (Employer.CalculateStature() > 4)
                {
                    emplStatMod = ((hiringPlayer.CalculateStature() - 4) * 0.04) * -1;
                }
            }

            // add together the 2 stature modifiers and invert
            statMod = 1 - (hirerStatMod + emplStatMod);

            // apply to salary
            salary *= statMod;

            return Convert.ToUInt32(salary);
        }

        /// <summary>
        /// Gets the character's head of family
        /// </summary>
        /// <returns>The head of family or null</returns>
        public PlayerCharacter GetHeadOfFamily()
        {
            throw new NotImplementedException();
        }

        public bool ChecksBeforeGranting()
        {
            throw new NotImplementedException();
        }



    }

}
