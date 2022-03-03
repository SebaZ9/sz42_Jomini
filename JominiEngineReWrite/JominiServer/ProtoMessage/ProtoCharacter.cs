using JominiGame;
using JominiServer;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtoMessageClient
{
    [ProtoContract]
    [ProtoInclude(101, typeof(ProtoPlayerCharacter))]
    [ProtoInclude(102, typeof(ProtoNonPlayerCharacter))]
    public class ProtoCharacter : ProtoMessage
    {
        [ProtoMember(1)]
        public string ID { get; set; }
        [ProtoMember(2)]
        public string FirstName { get; set; }
        [ProtoMember(3)]
        public string FamilyName { get; set; }
        [ProtoMember(4)]
        public uint BirthYear { get; set; }
        [ProtoMember(5)]
        public byte BirthSeason { get; set; }
        [ProtoMember(6)]
        public bool IsMale { get; set; }
        [ProtoMember(7)]
        public string NationalityID { get; set; }
        [ProtoMember(8)]
        public bool IsAlive { get; set; }
        [ProtoMember(9)]
        public double MaxHealth { get; set; }
        [ProtoMember(10)]
        public double Health { get; set; }
        [ProtoMember(11)]
        public double Stature { get; set; }
        [ProtoMember(12)]
        public double Virility { get; set; }
        [ProtoMember(13)]
        public bool InKeep { get; set; }
        [ProtoMember(14)]
        public string LanguageID { get; set; }
        [ProtoMember(15)]
        public double Days { get; set; }
        [ProtoMember(16)]
        public string FamilyID { get; set; }
        [ProtoMember(17)]
        public string SpouseID { get; set; }
        [ProtoMember(18)]
        public string FatherID { get; set; }
        [ProtoMember(19)]
        public string MotherID { get; set; }
        [ProtoMember(20)]
        public string FianceeID { get; set; }
        [ProtoMember(21)]
        public string LocationID { get; set; }
        [ProtoMember(22)]
        public double StatureModifier { get; set; }
        [ProtoMember(23)]
        public double Managment { get; set; }
        [ProtoMember(24)]
        public double Combat { get; set; }
        [ProtoMember(25)]
        public Pair[] Traits { get; set; }
        [ProtoMember(26)]
        public bool IsPregnant { get; set; }
        [ProtoMember(27)]
        public string[] TitleIDs { get; set; }
        [ProtoMember(28)]
        public string ArmyID { get; set; }
        [ProtoMember(29)]
        public Pair[] Ailments { get; set; }
        [ProtoMember(30)]
        public string[] GoToIDs { get; set; }
        [ProtoMember(31)]
        public string CaptorID { get; set; }
        [ProtoMember(32)]
        public SiegeRoles SiegeRole { get; set; }

        public ProtoCharacter(Character character)
        {

            ID = character.ID;
            FirstName = character.FirstName;
            FamilyName = character.FamilyName;
            BirthYear = character.BirthDate.Item1;
            BirthSeason = character.BirthDate.Item2;
            IsMale = character.IsMale;
            NationalityID = character.Nationality.NatID;
            IsAlive = character.IsAlive;
            MaxHealth = character.MaxHealth;
            Health = character.CalculateHealth(true);
            Stature = character.StatureModifier;
            Virility = character.Virility;
            InKeep = character.InKeep;
            LanguageID = character.Language.ID;
            Days = character.Days;
            FamilyID = character.FamilyID;
            LocationID = character.Location.ID;
            StatureModifier = character.StatureModifier;
            Managment = character.Management;
            Combat = character.Combat;
            IsPregnant = character.IsPregnant;

            if (character.Spouse != null)
                SpouseID = character.Spouse.ID;
            if (character.Father != null)
                FatherID = character.Father.ID;
            if (character.Mother != null)
                MotherID = character.Mother.ID;
            if (character.Fiancee != null)
                FianceeID = character.Fiancee.ID;
            if (character.ArmyID != null)
                ArmyID = character.ArmyID.ID;

            if (character.Traits != null)
            {
                if (character.Traits.Length > 0)
                {
                    Pair[] TraitPair = new Pair[character.Traits.Length];
                    for (int i = 0; i < TraitPair.Length; i++)
                    {
                        TraitPair[i] = new Pair()
                        {
                            Key = character.Traits[i].Item1.Name,
                            Value = character.Traits[i].Item2.ToString()
                        };
                    }
                    Traits = TraitPair;
                }        
            }

            if (character.MyTitles != null)
            {
                if(character.MyTitles.Count > 0)
                {
                    string[] Titles = new string[character.MyTitles.Count];
                    for (int i = 0; i < Titles.Length; i++)
                    {
                        Titles[i] = character.MyTitles[i].ID;
                    }
                    TitleIDs = Titles;
                }
            }

            if (character.Ailments != null)
            {
                if(character.Ailments.Count > 0)
                {
                    List<Pair> AilmentsPair = new();
                    foreach (KeyValuePair<string, Ailment> keyValue in character.Ailments)
                    {
                        AilmentsPair.Add(new Pair()
                        {
                            Key = keyValue.Key,
                            Value = keyValue.Value.Description
                        });
                    }
                    Ailments = AilmentsPair.ToArray();
                }
            }

            if (character.GoTo != null)
            {
                if(character.GoTo.Count > 0)
                {
                    List<string> GoToFiefs = new();
                    foreach (Fief fief in character.GoTo)
                    {
                        GoToFiefs.Add(fief.ID);
                    }
                    GoToIDs = GoToFiefs.ToArray();
                }                
            }


            if (character.ArmyID != null)
            {
                SiegeRole = SiegeRoles.None;
                if (character.ArmyID.CheckIfBesieger() != null)
                    SiegeRole = SiegeRoles.Besieger;
                if (character.ArmyID.CheckIfSiegeDefenderGarrison() != null)
                    SiegeRole = SiegeRoles.Defender;
                if (character.ArmyID.CheckIfSiegeDefenderAdditional() != null)
                    SiegeRole = SiegeRoles.DefenderAdd;
            }
        }

        public ProtoCharacter()
        {

        }
       
    }

    [ProtoContract]
    public class ProtoPlayerCharacter : ProtoCharacter
    {
        [ProtoMember(1)]
        public string PlayerID { get; set; }
        [ProtoMember(2)]
        public bool Outlawed { get; set; }
        [ProtoMember(3)]
        public uint Purse { get; set; }
        [ProtoMember(4)]
        public ProtoCharacterOverview[] MyNPCs { get; set; }
        [ProtoMember(5)]
        public ProtoCharacterOverview MyHeir { get; set; }
        [ProtoMember(6)]
        public string[] OwnedFiefs { get; set; }
        [ProtoMember(7)]
        public string[] Provinces { get; set; }
        [ProtoMember(8)]
        public string HomeFief { get; set; }
        [ProtoMember(9)]
        public string AncestralHomeFief { get; set; }

        public ProtoPlayerCharacter(PlayerCharacter playerCharacter) : base(playerCharacter)
        {
            PlayerID = playerCharacter.PlayerID;
            Outlawed = playerCharacter.Outlawed;
            Purse = playerCharacter.Purse;
            HomeFief = playerCharacter.HomeFief.ID;
            AncestralHomeFief = playerCharacter.AncestralHomeFief.ID;
        }

        public ProtoPlayerCharacter()
        {

        }

    }

    [ProtoContract]
    public class ProtoNonPlayerCharacter : ProtoCharacter
    {
        [ProtoMember(1)]
        public ProtoCharacterOverview Employer { get; set; }
        [ProtoMember(2)]
        public uint Salary { get; set; }
        [ProtoMember(3)]
        public string LastOfferID { get; set; }
        [ProtoMember(4)]
        public uint LastOfferAmount { get; set; }
        [ProtoMember(5)]
        public bool InEntourage { get; set; }
        [ProtoMember(6)]
        public bool IsHeir { get; set; }

        public ProtoNonPlayerCharacter(NonPlayerCharacter npc, Game game, Client client) : base(npc)
        {
            if(npc.Employer != null)
                Employer = new ProtoCharacterOverview(npc.Employer, game);
            Salary = npc.Salary;
            if (npc.LastOffer.ContainsKey(client.MyPlayerCharacter.ID))
            {
                LastOfferID = client.MyPlayerCharacter.ID;
                LastOfferAmount = npc.LastOffer[client.MyPlayerCharacter.ID];
            }
            InEntourage = npc.InEntourage;
            IsHeir = npc.IsHeir;
        }

        public ProtoNonPlayerCharacter()
        {

        }

    }

    [ProtoContract]
    public class ProtoCharacterOverview : ProtoMessage
    {
        [ProtoMember(1)]
        public string CharID { get; set; }
        [ProtoMember(2)]
        public string OwnerID { get; set; }
        [ProtoMember(3)]
        public string CharName { get; set; }
        [ProtoMember(4)]
        public string Role { get; set; }
        [ProtoMember(5)]
        public string NationalityID { get; set; }
        [ProtoMember(6)]
        public string LocationID { get; set; }
        [ProtoMember(7)]
        public bool IsMale { get; set; }

        public ProtoCharacterOverview(Character character, Game game)
        {

            CharID = character.ID;
            CharName = character.FullName();
            IsMale = character.IsMale;
            NationalityID = character.Nationality.NatID;
            LocationID = character.Location.ID;
            // Get PC or Head of Family or Employer

            PlayerCharacter? charPC = character.GetPlayerCharacter();
            if (charPC != null)
            {
                OwnerID = charPC.FullName();
            }

            if (character is NonPlayerCharacter npc)
            {
                PlayerCharacter? headOfFamily = npc.GetHeadOfFamily();
                if (headOfFamily == null)
                {
                    headOfFamily = npc.Employer;
                }
                if (headOfFamily != null)
                {
                    Role = npc.GetFunction(headOfFamily);
                }
            }
            else if (character is PlayerCharacter pc)
            {
                if (game.CheckIfOverlord(pc))
                {
                    Role = "Overlord";
                }
                if (game.CheckIsKing(pc))
                {
                   Role = "King";
                }
                if (game.CheckIsSysAdmin(pc))
                {
                    Role = "Admin";
                }
                if (game.CheckIsPrince(pc))
                {
                    Role = "Prince";
                }
                if (game.CheckIsHerald(pc))
                {
                    Role = "Herald";
                }
            }
        }

        public ProtoCharacterOverview()
        {

        }
    }

    public enum SiegeRoles { None = 0, Besieger, Defender, DefenderAdd }
}
