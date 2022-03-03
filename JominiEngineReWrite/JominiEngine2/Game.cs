using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JominiGame
{
    /// <summary>
    ///     Class representing game
    ///     It will initialise all the game objects and control the flow of the game
    ///     The control code from all the form classes will either be put in here or accessed from here
    ///     For example, if a server gets an incoming message from a client and finds that it is some game action to be taken,
    ///     it might send this to Game, where the precise action will be determined from the message, carried out and the
    ///     result send back via server
    /// </summary>
    public class Game
    {

        private GameClock Clock;
        private IdGenerator IDGen;
        private HexMapGraph GameMap;

        // Main Game Objects
        private Dictionary<string, Terrain> Terrains;
        private Dictionary<string,Trait> Traits;
        private Dictionary<string,Nationality> Nationalities;
        private Dictionary<string,BaseLanguage> BaseLanguages;
        private Dictionary<string,Language> Languages;
        private Dictionary<byte,Rank> Ranks;
        private Dictionary<string, Kingdom> Kingdoms;
        private Dictionary<string, Province> Provinces;
        private Dictionary<string, Fief> Fiefs;
        private Dictionary<string, PlayerCharacter> PlayerCharacters;
        private Dictionary<string, NonPlayerCharacter> NonPlayerCharacters;
        private Dictionary<string, Character> AllCharacters;
        private Dictionary<string, Army> Armies;
        private Dictionary<string, Siege> Sieges;

        // Royalty
        private PlayerCharacter KingOne;
        private PlayerCharacter KingTwo;
        private PlayerCharacter PrinceOne;
        private PlayerCharacter PrinceTwo;
        private PlayerCharacter HeraldOne;
        private PlayerCharacter HeraldTwo;

        // Admin
        private PlayerCharacter Admin;

        private List<PlayerCharacter> PromotedNPCs;
        private List<JournalEntry> ScheduledEvents;

        public Game()
        {
            // Init base game objects
            Clock = new GameClock((uint)GameSettings.START_YEAR);
            IDGen = new IdGenerator();
            GameMap = new HexMapGraph();
            PromotedNPCs = new();
            ScheduledEvents = new();

            // Get directory of CSVs
            string directory = Directory.GetCurrentDirectory();
            string path;
            if (Environment.OSVersion.Platform == PlatformID.Unix || Environment.OSVersion.Platform == PlatformID.MacOSX)
            {
                path = Path.Combine(directory, "JominiEngine", "CSVs");
            }
            else
            {
                path = Path.Combine(directory, "CSVs");
            }

            Armies = new();

            #region ImportFromCSV
            Terrains = CSVImport.LoadTerrainsFromCSV(path);
            Traits = CSVImport.LoadTraitsFromCSV(path);
            Nationalities = CSVImport.LoadNationalitiesFromCSV(path);
            BaseLanguages = CSVImport.LoadBaseLanguagesFromCSV(path);
            Languages = CSVImport.LoadLanguagesFromCSV(path, BaseLanguages);
            Ranks = CSVImport.LoadRanksFromCSV(path, Languages);
            Kingdoms = CSVImport.LoadKingdomsFromCSV(path, Ranks, Clock, IDGen, GameMap,
                out Dictionary<string, string> KingdomTitleHolders,
                out Dictionary<string, string> KingdomOwners);
            Provinces = CSVImport.LoadProvincesFromCSV(path, Ranks, Kingdoms, Clock, IDGen, GameMap,
                out Dictionary<string, string> ProvinceTitleHolders,
                out Dictionary<string, string> ProvinceOwners);
            Fiefs = CSVImport.LoadFiefsFromCSV(path, Provinces, Languages, Terrains, Ranks, Clock, IDGen, GameMap,
                out Dictionary<string, string> FiefOwners,
                out Dictionary<string, string> FiefAncestralOwners,
                out Dictionary<string, string> FiefTitleHolder,
                out Dictionary<string, string> FiefsBailiff,
                out Dictionary<string, string> FiefsSiege);
            PlayerCharacters = CSVImport.LoadPlayerCharactersFromCSV(path, Nationalities, Languages, Fiefs, Clock, IDGen, GameMap,
                out Dictionary<string, string> PCSpouse,
                out Dictionary<string, string> PCFather,
                out Dictionary<string, string> PCMother,
                out Dictionary<string, string> PCFiancee,
                out Dictionary<string, string> PCArmy,
                out Dictionary<string, string> PCClient);
            NonPlayerCharacters = CSVImport.LoadNonPlayerCharactersFromCSV(path, Nationalities, Languages, Fiefs, PlayerCharacters, Clock, IDGen, GameMap,
                out Dictionary<string, string> NPCSpouse,
                out Dictionary<string, string> NPCFather,
                out Dictionary<string, string> NPCMother,
                out Dictionary<string, string> NPCFiancee,
                out Dictionary<string, string> NPCArmy);
            #endregion

            #region AssignDataToObjects
            AllCharacters = new();
            foreach (PlayerCharacter pc in PlayerCharacters.Values)
            {
                AllCharacters.Add(pc.ID, pc);
            }
            foreach (NonPlayerCharacter npc in NonPlayerCharacters.Values)
            {
                AllCharacters.Add(npc.ID, npc);
            }

            // Assign Kingdom Data
            foreach (string key in KingdomTitleHolders.Keys)
            {
                Kingdoms[key].TitleHolder = AllCharacters[KingdomTitleHolders[key]];
            }
            foreach (string key in KingdomOwners.Keys)
            {
                Kingdoms[key].Owner = PlayerCharacters[KingdomOwners[key]];
                Kingdoms[key].KingdomNationality = PlayerCharacters[KingdomOwners[key]].Nationality;
            }

            // Assign Province Data
            foreach (string key in ProvinceTitleHolders.Keys)
            {
                Provinces[key].TitleHolder = AllCharacters[ProvinceTitleHolders[key]];
            }
            foreach (string key in ProvinceOwners.Keys)
            {
                Provinces[key].Owner = PlayerCharacters[ProvinceOwners[key]];
            }

            // Assign Fief Data
            foreach (string key in FiefOwners.Keys)
            {
                Fiefs[key].Owner = PlayerCharacters[FiefOwners[key]];
            }
            foreach (string key in FiefAncestralOwners.Keys)
            {
                Fiefs[key].AncestralOwner = PlayerCharacters[FiefAncestralOwners[key]];
            }
            foreach (string key in FiefTitleHolder.Keys)
            {
                Fiefs[key].TitleHolder = AllCharacters[FiefTitleHolder[key]];
                AllCharacters[FiefTitleHolder[key]].MyTitles.Add(Fiefs[key]);
            }
            foreach (string key in FiefsBailiff.Keys)
            {
                Fiefs[key].Bailiff = AllCharacters[FiefsBailiff[key]];
            }
            foreach (string key in FiefsSiege.Keys)
            {
                throw new NotImplementedException("Sieges currently not saved");
            }

            // Assign Player Character Data
            foreach (string key in PCSpouse.Keys)
            {
                AllCharacters[key].Spouse = AllCharacters[PCSpouse[key]];
            }
            foreach (string key in PCFather.Keys)
            {
                AllCharacters[key].Father = AllCharacters[PCFather[key]];
            }
            foreach (string key in PCMother.Keys)
            {
                AllCharacters[key].Mother = AllCharacters[PCMother[key]];
            }
            foreach (string key in PCFiancee.Keys)
            {
                AllCharacters[key].Fiancee = AllCharacters[PCFiancee[key]];
            }
            foreach (string key in PCArmy.Keys)
            {
                throw new NotImplementedException("Armies currently not saved");
            }
            foreach (string key in PCClient.Keys)
            {
                throw new NotImplementedException("PC Client currently not saved");
            }

            // Assign Non Player Character Data
            foreach (string key in NPCSpouse.Keys)
            {
                AllCharacters[key].Spouse = AllCharacters[NPCSpouse[key]];
            }
            foreach (string key in NPCFather.Keys)
            {
                AllCharacters[key].Father = AllCharacters[NPCFather[key]];
            }
            foreach (string key in NPCMother.Keys)
            {
                AllCharacters[key].Mother = AllCharacters[NPCMother[key]];
            }
            foreach (string key in NPCFiancee.Keys)
            {
                AllCharacters[key].Fiancee = AllCharacters[NPCFiancee[key]];
            }
            foreach (string key in NPCArmy.Keys)
            {
                throw new NotImplementedException("Armies currently not saved");
            }
            #endregion

            //PrintPlayerCharacter(PlayerCharacters["Char_47"]);

        }

        public Character GetCharacter(string ID)
        {
            return AllCharacters[ID];
        }

        /// <summary>
        ///     Updates game objects when Season ticks to next season
        /// </summary>
        public void SeasonUpdate()
        {

            // Update Fiefs
            foreach(Fief f in Fiefs.Values)
            {
                f.UpdateFief();
            }

            // NPCs
            foreach(NonPlayerCharacter npc in NonPlayerCharacters.Values)
            {
                if (npc.IsAlive)
                {
                    npc.UpdateCharacter();
                    if (npc.IsAlive)
                    {
                        // Random move if NPC has no employer and is not part of family
                        if(npc.Employer == null && string.IsNullOrEmpty(npc.FamilyID))
                            npc.RandomMoveNPC();
                        // Finish previously started multi-hex move if necessary
                        if (npc.GoTo.Count > 0)
                            npc.CharacterMultiMove();
                    }
                }
            }

            // PCs
            foreach(PlayerCharacter pc in PlayerCharacters.Values)
            {
                if (pc.IsAlive)
                {
                    pc.UpdateCharacter();
                    if (pc.IsAlive)
                    {
                        if (pc.GoTo.Count > 0)
                            pc.CharacterMultiMove();
                    }
                }
            }

            // Add any newly promoted NPCs to PCs
            foreach (PlayerCharacter pc in PromotedNPCs)
            {
                PlayerCharacters.Add(pc.ID, pc);
            }
            PromotedNPCs.Clear();

            // Armies
            foreach (Army army in Armies.Values)
            {
                if (army.UpdateArmy())
                {
                    //Disband army
                    Armies.Remove(army.ID);
                    army.DisbandArmy();
                }
            }

            // Sieges
            foreach (Siege siege in Sieges.Values)
            {
                if (siege.UpdateSiege())
                {
                    siege.SiegeEnd(false);
                    Sieges.Remove(siege.ID);
                }
            }

            // Advance clock year/season
            Clock.AdvanceSeason();

            // Check for victory
            if (!CheckForVictory())
            {
                foreach (JournalEntry journalEntry in ScheduledEvents)
                {
                    Console.WriteLine("Update Scheduled events");
                }
            }

        }

        /// <summary>
        /// Checks if the character is a province overlord
        /// </summary>
        /// <returns>bool indicating if the character is an overlord</returns>
        public bool CheckIfOverlord(PlayerCharacter pc)
        {
            foreach (Place place in pc.MyTitles)
            {
                if (Provinces.ContainsKey(place.ID))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Check to see if the PlayerCharacter is a king
        /// </summary>
        /// <returns>bool indicating whether is a king</returns>
        public bool CheckIsKing(PlayerCharacter pc)
        {
            if ((pc == KingOne) || (pc == KingTwo))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Check to see if the PlayerCharacter is a sysAdmin
        /// </summary>
        /// <returns>bool indicating whether is a sysAdmin</returns>
        public bool CheckIsSysAdmin(PlayerCharacter pc)
        {
            return pc == Admin;
        }

        /// <summary>
        /// Check to see if the PlayerCharacter is a prince
        /// </summary>
        /// <returns>bool indicating whether is a prince</returns>
        public bool CheckIsPrince(PlayerCharacter pc)
        {
            if ((pc == PrinceOne) || (pc == PrinceTwo))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Check to see if the PlayerCharacter is a herald
        /// </summary>
        /// <returns>bool indicating whether is a herald</returns>
        public bool CheckIsHerald(PlayerCharacter pc)
        {
            if ((pc == HeraldOne) || (pc == HeraldTwo))
            {
                return true;
            }

            return false;
        }

        private bool CheckForVictory()
        {
            Console.WriteLine("Currently not checking for victory at the end of season");
            return false;
        }

        #region Testing
        public void PrintPlayerCharacter(PlayerCharacter pc)
        {
            if(pc.Traits != null)
            {
                foreach (Tuple<Trait, int> trait in pc.Traits)
                {
                    Console.WriteLine($"\t{trait.Item1.Name} : {trait.Item2}");
                }
            } else
            {
                Console.WriteLine("\tNone");
            }
            

            Console.WriteLine($"In Keep? {pc.InKeep}");
            Console.WriteLine($"Is Pregnant? {pc.IsPregnant}");
            Console.WriteLine($"FamilyID: {pc.FamilyID}");
            Console.WriteLine($"Spouse: {pc.Spouse}");
            Console.WriteLine($"Father: {pc.Father}");
            Console.WriteLine($"Mother: {pc.Mother}");
            Console.WriteLine($"Fiancee: {pc.Fiancee}");
            Console.WriteLine($"Location:  {pc.Location}");
            Console.WriteLine($"Titles:");

            if (pc.MyTitles != null)
            {
                foreach (Place place in pc.MyTitles)
                {
                    Console.WriteLine($"\tPlace: {place.ID}");
                }
            }
            else
            {
                Console.WriteLine("\tNone");
            }
            
            Console.WriteLine($"Army: {pc.ArmyID}");
            Console.WriteLine($"Captor: {pc.Captor}");
            Console.WriteLine($"Ailments:");
            
            if (pc.Ailments != null)
            {
                foreach (string a in pc.Ailments.Keys)
                {
                    Console.WriteLine($"\tAilment: {a}");
                }
            }
            else
            {
                Console.WriteLine("\tNone");
            }
            Console.WriteLine("----- Finished -----");
        }
        #endregion

    }
}
