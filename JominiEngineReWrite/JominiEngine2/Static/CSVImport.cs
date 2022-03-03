using QuickGraph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace JominiGame
{
    public static class CSVImport
    {

        public static Dictionary<string, Terrain> LoadTerrainsFromCSV(string path)
        {
            string csvPath = Path.Combine(path, "terrain.csv");
            if (!File.Exists(csvPath))
            {
                throw new FileNotFoundException("Terrain CSV file not found! Looking at: " + csvPath);
            }
            Dictionary<string, Terrain> result = new();

            using (StreamReader reader = new(csvPath))
            {
                // Skip over header
                reader.ReadLine();
                reader.ReadLine();
                
                // Loop over the dataset
                while (!reader.EndOfStream)
                {
                    string? line = reader.ReadLine();
                    if (line == null) break;
                    string[] values = line.Split(',');

                    result.Add(values[0], new Terrain(values[0], values[1], double.Parse(values[2])));
                }
            }

            return result;
        }

        public static Dictionary<string, Trait> LoadTraitsFromCSV(string path)
        {
            string csvPath = Path.Combine(path, "traits.csv");
            if (!File.Exists(csvPath))
            {
                throw new FileNotFoundException("Traits CSV file not found! Looking at: " + csvPath);
            }
            Dictionary<string, Trait> result = new();

            using (StreamReader reader = new(csvPath))
            {
                // Skip over header
                reader.ReadLine();
                reader.ReadLine();

                // Loop over the dataset
                while (!reader.EndOfStream)
                {
                    string? line = reader.ReadLine();
                    if (line == null) break;
                    string[] values = line.Split(',');
                    values = values.Where(x => !string.IsNullOrEmpty(x)).ToArray();

                    Dictionary<TraitStats, double> traitEffects = new();
                    for (int i = 2; values.Length > i; i+=2)
                    {
                        if(!Enum.TryParse(values[i].ToUpper(), out TraitStats trait))
                        {
                            throw new Exception("Failed to cast trait ID to ENUM");
                        }
                        traitEffects.Add(trait, double.Parse(values[i + 1]));

                    }

                    result.Add(values[0], new Trait(values[0], values[1], traitEffects));
                }
            }

            return result;
        }

        public static Dictionary<string, Nationality> LoadNationalitiesFromCSV(string path)
        {
            string csvPath = Path.Combine(path, "nationality.csv");
            if (!File.Exists(csvPath))
            {
                throw new FileNotFoundException("Nationality CSV file not found! Looking at: " + csvPath);
            }
            Dictionary<string, Nationality> result = new();

            using (StreamReader reader = new(csvPath))
            {
                // Skip over header
                reader.ReadLine();
                reader.ReadLine();

                // Loop over the dataset
                while (!reader.EndOfStream)
                {
                    string? line = reader.ReadLine();
                    if (line == null) break;
                    string[] values = line.Split(',');

                    result.Add(values[0], new Nationality(values[0], values[1]));
                }
            }

            return result;
        }

        public static Dictionary<string, BaseLanguage> LoadBaseLanguagesFromCSV(string path)
        {
            string csvPath = Path.Combine(path, "baseLanguage.csv");
            if (!File.Exists(csvPath))
            {
                throw new FileNotFoundException("BaseLanguage CSV file not found! Looking at: " + csvPath);
            }
            Dictionary<string, BaseLanguage> result = new();

            using (StreamReader reader = new(csvPath))
            {
                // Skip over header
                reader.ReadLine();
                reader.ReadLine();

                // Loop over the dataset
                while (!reader.EndOfStream)
                {
                    string? line = reader.ReadLine();
                    if (line == null) break;
                    string[] values = line.Split(',');

                    result.Add(values[0], new BaseLanguage(values[0], values[1]));
                }
            }

            return result;
        }

        public static Dictionary<string, Language> LoadLanguagesFromCSV(string path, Dictionary<string, BaseLanguage> baseLanguages)
        {
            string csvPath = Path.Combine(path, "language.csv");
            if (!File.Exists(csvPath))
            {
                throw new FileNotFoundException("Language CSV file not found! Looking at: " + csvPath);
            }
            Dictionary<string, Language> result = new();

            using (StreamReader reader = new(csvPath))
            {
                // Skip over header
                reader.ReadLine();
                reader.ReadLine();

                // Loop over the dataset
                while (!reader.EndOfStream)
                {
                    string? line = reader.ReadLine();
                    if (line == null) break;
                    string[] values = line.Split(',');

                    result.Add(values[0], new Language(baseLanguages[values[1]], int.Parse(values[2])));
                }
            }

            return result;
        }

        public static Dictionary<byte, Rank> LoadRanksFromCSV(string path, Dictionary<string, Language> Languages)
        {
            string csvPath = Path.Combine(path, "rank.csv");
            if (!File.Exists(csvPath))
            {
                throw new FileNotFoundException("Ranks CSV file not found! Looking at: " + csvPath);
            }
            Dictionary<byte, Rank> result = new();

            using (StreamReader reader = new(csvPath))
            {
                // Skip over header
                reader.ReadLine();
                reader.ReadLine();

                // Loop over the dataset
                while (!reader.EndOfStream)
                {
                    string? line = reader.ReadLine();
                    if (line == null) break;
                    string[] values = line.Split(',');
                    values = values.Where(x => !string.IsNullOrEmpty(x)).ToArray();

                    Dictionary<Language, TitleName> titleNames = new();

                    titleNames.Add(Languages["lang_E1"], new TitleName("lang_E1", values[3]));
                    titleNames.Add(Languages["lang_E2"], new TitleName("lang_E2", values[3]));

                    for (int i = 4; values.Length > i; i += 2)
                    {
                        TitleName titleName = new()
                        {
                            LangID = values[i],
                            Name = values[i + 1]
                        };
                        titleNames.Add(Languages[values[i]], titleName);

                    }

                    result.Add(byte.Parse(values[0]), new Rank(byte.Parse(values[0]), titleNames, byte.Parse(values[1])));
                }
            }

            return result;
        }

        public static Dictionary<string, Kingdom> LoadKingdomsFromCSV(string path, Dictionary<byte, Rank> ranks, GameClock Clock,
            IdGenerator IDGen, HexMapGraph GameMap, out Dictionary<string, string> KingdomTitleHolders, out Dictionary<string, string> KingdomOwners)
        {
            string csvPath = Path.Combine(path, "kingdom.csv");
            if (!File.Exists(csvPath))
            {
                throw new FileNotFoundException("Kingdom CSV file not found! Looking at: " + csvPath);
            }
            Dictionary<string, Kingdom> result = new();
            KingdomTitleHolders = new();
            KingdomOwners = new();

            using (StreamReader reader = new(csvPath))
            {
                // Skip over header
                reader.ReadLine();
                reader.ReadLine();

                // Loop over the dataset
                while (!reader.EndOfStream)
                {
                    string? line = reader.ReadLine();
                    if (line == null) break;
                    string[] values = line.Split(',');

                    result.Add(values[0], new Kingdom(Clock, IDGen, GameMap)
                    {
                        ID = values[0],
                        Name = values[1],
                        PlaceRank = ranks[byte.Parse(values[2])]
                    });
                    KingdomTitleHolders.Add(values[0], values[4]);
                    KingdomOwners.Add(values[0], values[5]);
                }
            }

            // Remove Empty elements
            KingdomTitleHolders = KingdomTitleHolders
                    .Where(f => f.Value.Length > 0)
                    .ToDictionary(x => x.Key, x => x.Value);

            KingdomOwners = KingdomOwners
                    .Where(f => f.Value.Length > 0)
                    .ToDictionary(x => x.Key, x => x.Value);

            return result;
        }

        public static Dictionary<string, Province> LoadProvincesFromCSV(string path, Dictionary<byte, Rank> ranks, Dictionary<string, Kingdom> kingdoms, GameClock Clock,
            IdGenerator IDGen, HexMapGraph GameMap, out Dictionary<string, string> ProvinceTitleHolders, out Dictionary<string, string> ProvinceOwners)
        {
            string csvPath = Path.Combine(path, "province.csv");
            if (!File.Exists(csvPath))
            {
                throw new FileNotFoundException("Province CSV file not found! Looking at: " + csvPath);
            }
            Dictionary<string, Province> result = new();
            ProvinceTitleHolders = new();
            ProvinceOwners = new();

            using (StreamReader reader = new(csvPath))
            {
                // Skip over header
                reader.ReadLine();
                reader.ReadLine();

                // Loop over the dataset
                while (!reader.EndOfStream)
                {
                    string? line = reader.ReadLine();
                    if (line == null) break;
                    string[] values = line.Split(',');

                    result.Add(values[0], new Province(Clock, IDGen, GameMap)
                    {
                        ID = values[0],
                        Name = values[1],
                        PlaceRank = ranks[byte.Parse(values[2])],
                        TaxRate = double.Parse(values[3]),
                        ProvinceKingdom = kingdoms[values[6]]
                    });
                    ProvinceTitleHolders.Add(values[0], values[4]);
                    ProvinceOwners.Add(values[0], values[5]);
                }
            }

            ProvinceTitleHolders = ProvinceTitleHolders
                    .Where(f => f.Value.Length > 0)
                    .ToDictionary(x => x.Key, x => x.Value);

            ProvinceOwners = ProvinceOwners
                    .Where(f => f.Value.Length > 0)
                    .ToDictionary(x => x.Key, x => x.Value);

            return result;
        }

        public static Dictionary<string, Fief> LoadFiefsFromCSV(string path, Dictionary<string, Province> provinces, Dictionary<string, Language> languages,
            Dictionary<string, Terrain> terrains, Dictionary<byte, Rank> ranks, GameClock Clock, IdGenerator IDGen, HexMapGraph GameMap,
            out Dictionary<string, string> FiefOwners, out Dictionary<string, string> FiefAncestralOwners,
            out Dictionary<string, string> FiefTitleHolder, out Dictionary<string, string> FiefsBailiff, out Dictionary<string, string> FiefsSiege)
        {
            string csvPath = Path.Combine(path, "Fief.csv");
            if (!File.Exists(csvPath))
            {
                throw new FileNotFoundException("Fief CSV file not found! Looking at: " + csvPath);
            }
            Dictionary<string, Fief> result = new();
            FiefOwners = new();
            FiefAncestralOwners = new();
            FiefTitleHolder = new();
            FiefsBailiff = new();
            FiefsSiege = new();

            using (StreamReader reader = new(csvPath))
            {
                // Skip over header
                reader.ReadLine();
                reader.ReadLine();

                // Loop over the dataset
                while (!reader.EndOfStream)
                {
                    string? line = reader.ReadLine();
                    if (line == null) break;
                    string[] values = line.Split(',');

                    double[] FiefKeyStatsCurrent =
                    {
                        double.Parse(values[13]),
                        double.Parse(values[14]),
                        double.Parse(values[15]),
                        double.Parse(values[16]),
                        double.Parse(values[17]),
                        double.Parse(values[18]),
                        double.Parse(values[19]),
                        double.Parse(values[20]),
                        double.Parse(values[21]),
                        double.Parse(values[22]),
                        double.Parse(values[23]),
                        double.Parse(values[24]),
                        double.Parse(values[25]),
                        double.Parse(values[26])
                    };

                    double[] FiefKeyStatsPrevious =
                    {
                        double.Parse(values[27]),
                        double.Parse(values[28]),
                        double.Parse(values[29]),
                        double.Parse(values[30]),
                        double.Parse(values[31]),
                        double.Parse(values[32]),
                        double.Parse(values[33]),
                        double.Parse(values[34]),
                        double.Parse(values[35]),
                        double.Parse(values[36]),
                        double.Parse(values[37]),
                        double.Parse(values[38]),
                        double.Parse(values[39]),
                        double.Parse(values[40]),
                    };

                    result.Add(values[0], new Fief(Clock, IDGen, GameMap)
                    {
                        ID = values[0],
                        Name = values[1],
                        FiefsProvince = provinces[values[2]],
                        Population = int.Parse(values[3]),
                        Fields =  double.Parse(values[4]),
                        Industry = double.Parse(values[5]),
                        Troops = uint.Parse(values[6]),
                        TaxRate = double.Parse(values[7]),
                        TaxRateNext = double.Parse(values[8]),
                        OfficialSpendNext = uint.Parse(values[9]),
                        GarrisonSpendNext = uint.Parse(values[10]),
                        InfrastructureSpendNext = uint.Parse(values[11]),
                        KeepSpendNext = uint.Parse(values[12]),
                        KeyStatsCurrent = FiefKeyStatsCurrent,
                        KeyStatsPrevious = FiefKeyStatsPrevious,
                        KeepLevel = double.Parse(values[41]),
                        Loyalty = double.Parse(values[42]),
                        Status = char.Parse(values[43]),
                        FiefsLanguage = languages[values[44]],
                        FiefsTerrain = terrains[values[45]],
                        BailiffDaysInFief = uint.Parse(values[46]),
                        Treasury = int.Parse(values[47]),
                        HasRecruited = bool.Parse(values[48]),
                        IsPillaged = bool.Parse(values[49]),
                        PlaceRank = ranks[byte.Parse(values[50])]
                    });


                    FiefTitleHolder.Add(values[0], values[51]);
                    FiefOwners.Add(values[0], values[52]);
                    FiefAncestralOwners.Add(values[0], values[53]);
                    FiefsBailiff.Add(values[0], values[54]);
                    FiefsSiege.Add(values[0], values[55]);
                }
            }

            FiefTitleHolder = FiefTitleHolder
                    .Where(f => f.Value.Length > 0)
                    .ToDictionary(x => x.Key, x => x.Value);

            FiefOwners = FiefOwners
                    .Where(f => f.Value.Length > 0)
                    .ToDictionary(x => x.Key, x => x.Value);

            FiefAncestralOwners = FiefAncestralOwners
                    .Where(f => f.Value.Length > 0)
                    .ToDictionary(x => x.Key, x => x.Value);

            FiefsBailiff = FiefsBailiff
                    .Where(f => f.Value.Length > 0)
                    .ToDictionary(x => x.Key, x => x.Value);

            FiefsSiege = FiefsSiege
                    .Where(f => f.Value.Length > 0)
                    .ToDictionary(x => x.Key, x => x.Value);

            return result;
        }

        public static Dictionary<string, PlayerCharacter> LoadPlayerCharactersFromCSV(string path, Dictionary<string, Nationality> nationalities,
            Dictionary<string, Language> languages, Dictionary<string, Fief> fiefs, GameClock Clock, IdGenerator IDGen, HexMapGraph GameMap,
            out Dictionary<string, string> PCSpouse, out Dictionary<string, string> PCFather, out Dictionary<string, string> PCMother,
            out Dictionary<string, string> PCFiancee, out Dictionary<string, string> PCArmy, out Dictionary<string, string> PCClient)
        {
            string csvPath = Path.Combine(path, "pc.csv");
            if (!File.Exists(csvPath))
            {
                throw new FileNotFoundException("Player Character CSV file not found! Looking at: " + csvPath);
            }
            Dictionary<string, PlayerCharacter> result = new();
            PCSpouse = new();
            PCFather = new();
            PCMother = new();
            PCFiancee = new();
            PCArmy = new();
            PCClient = new();

            using (StreamReader reader = new(csvPath))
            {
                // Skip over header
                reader.ReadLine();
                reader.ReadLine();

                // Loop over the dataset
                while (!reader.EndOfStream)
                {
                    string? line = reader.ReadLine();
                    if (line == null) break;
                    string[] values = line.Split(',');

                    result.Add(values[0], new PlayerCharacter(Clock, IDGen, GameMap)
                    {
                        ID = values[0],
                        FirstName = values[1],
                        FamilyName = values[2],
                        BirthDate = new Tuple<uint, byte>(uint.Parse(values[3]), byte.Parse(values[4])),
                        IsMale = bool.Parse(values[5]),
                        Nationality = nationalities[values[6]],
                        IsAlive = bool.Parse(values[7]),
                        MaxHealth = double.Parse(values[8]),
                        Virility = double.Parse(values[9]),
                        Language = languages[values[10]],
                        Days = double.Parse(values[11]),
                        StatureModifier = double.Parse(values[12]),
                        Management = double.Parse(values[13]),
                        Combat = double.Parse(values[14]),
                        InKeep = bool.Parse(values[15]),
                        IsPregnant = bool.Parse(values[16]),
                        FamilyID = values[17],
                        Outlawed = bool.Parse(values[22]),
                        Purse = uint.Parse(values[23]),
                        HomeFief = fiefs[values[24]],
                        AncestralHomeFief = fiefs[values[25]],
                        Location = fiefs[values[26]],
                        MyTitles = new()
                    });

                    PCSpouse.Add(values[0], values[18]);
                    PCFather.Add(values[0], values[19]);
                    PCMother.Add(values[0], values[20]);
                    PCFiancee.Add(values[0], values[21]);
                    PCArmy.Add(values[0], values[27]);
                    PCClient.Add(values[0], values[28]);

                }
            }

            PCSpouse = PCSpouse
                    .Where(f => f.Value.Length > 0)
                    .ToDictionary(x => x.Key, x => x.Value);

            PCFather = PCFather
                    .Where(f => f.Value.Length > 0)
                    .ToDictionary(x => x.Key, x => x.Value);

            PCMother = PCMother
                    .Where(f => f.Value.Length > 0)
                    .ToDictionary(x => x.Key, x => x.Value);

            PCFiancee = PCFiancee
                    .Where(f => f.Value.Length > 0)
                    .ToDictionary(x => x.Key, x => x.Value);

            PCArmy = PCArmy
                    .Where(f => f.Value.Length > 0)
                    .ToDictionary(x => x.Key, x => x.Value);

            PCClient = PCClient
                    .Where(f => f.Value.Length > 0)
                    .ToDictionary(x => x.Key, x => x.Value);

            return result;
        }

        public static Dictionary<string, NonPlayerCharacter> LoadNonPlayerCharactersFromCSV(string path, Dictionary<string, Nationality> nationalities,
            Dictionary<string, Language> languages, Dictionary<string, Fief> fiefs, Dictionary<string, PlayerCharacter> PCs,
            GameClock Clock, IdGenerator IDGen, HexMapGraph GameMap,
            out Dictionary<string, string> NPCSpouse, out Dictionary<string, string> NPCFather, out Dictionary<string, string> NPCMother,
            out Dictionary<string, string> NPCFiancee, out Dictionary<string, string> NPCArmy)
        {
            string csvPath = Path.Combine(path, "npc.csv");
            if (!File.Exists(csvPath))
            {
                throw new FileNotFoundException("Non Player Character CSV file not found! Looking at: " + csvPath);
            }
            Dictionary<string, NonPlayerCharacter> result = new();
            NPCSpouse = new();
            NPCFather = new();
            NPCMother = new();
            NPCFiancee = new();
            NPCArmy = new();

            using (StreamReader reader = new(csvPath))
            {
                // Skip over header
                reader.ReadLine();
                reader.ReadLine();

                // Loop over the dataset
                while (!reader.EndOfStream)
                {
                    string? line = reader.ReadLine();
                    if (line == null) break;
                    string[] values = line.Split(',');

                    PlayerCharacter? NPCEmployer = null;
                    if (PCs.ContainsKey(values[27]))
                    {
                        NPCEmployer = PCs[values[27]];
                    }

                    result.Add(values[0], new NonPlayerCharacter(Clock, IDGen, GameMap)
                    {
                        ID = values[0],
                        FirstName = values[1],
                        FamilyName = values[2],
                        BirthDate = new Tuple<uint, byte>(uint.Parse(values[3]), byte.Parse(values[4])),
                        IsMale = bool.Parse(values[5]),
                        Nationality = nationalities[values[6]],
                        IsAlive = bool.Parse(values[7]),
                        MaxHealth = double.Parse(values[8]),
                        Virility = double.Parse(values[9]),
                        Language = languages[values[10]],
                        Days = double.Parse(values[11]),
                        StatureModifier = double.Parse(values[12]),
                        Management = double.Parse(values[13]),
                        Combat = double.Parse(values[14]),
                        InKeep = bool.Parse(values[15]),
                        IsPregnant = bool.Parse(values[16]),
                        FamilyID = values[17],
                        Salary = uint.Parse(values[22]),
                        InEntourage = bool.Parse(values[23]),
                        IsHeir = bool.Parse(values[24]),
                        Location = fiefs[values[25]],
                        Employer = NPCEmployer
                    });

                    NPCSpouse.Add(values[0], values[18]);
                    NPCFather.Add(values[0], values[19]);
                    NPCMother.Add(values[0], values[20]);
                    NPCFiancee.Add(values[0], values[21]);
                    NPCArmy.Add(values[0], values[26]);

                }
            }

            NPCSpouse = NPCSpouse
                    .Where(f => f.Value.Length > 0)
                    .ToDictionary(x => x.Key, x => x.Value);

            NPCFather = NPCFather
                    .Where(f => f.Value.Length > 0)
                    .ToDictionary(x => x.Key, x => x.Value);

            NPCMother = NPCMother
                    .Where(f => f.Value.Length > 0)
                    .ToDictionary(x => x.Key, x => x.Value);

            NPCFiancee = NPCFiancee
                    .Where(f => f.Value.Length > 0)
                    .ToDictionary(x => x.Key, x => x.Value);

            NPCArmy = NPCArmy
                    .Where(f => f.Value.Length > 0)
                    .ToDictionary(x => x.Key, x => x.Value);

            return result;
        }




        /// <summary>
        /// Creates a map edges collection using data imported from a CSV file and either writes it to the database
        /// or creates a game map
        /// </summary>
        /// <returns>bool indicating success state</returns>
        /// <param name="filename">The name of the CSV file</param>
        /// <param name="bucketID">The name of the database bucket in which to store the game objects</param>
        /// <param name="toDatabase">bool indicating whether or not to save the game objects to database or game</param>
        public static bool CreateMapEdgesFromCSV(string filename, string bucketID = null, bool toDatabase = true)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates list of serialised map edges
        /// </summary>
        /// <returns>List containing map edges</returns>
        /// <param name="mapArray">string[,] containing main map data</param>
        /// <param name="customLinks">List(string) containing custom hex links (optional)</param>
        public static List<TaggedEdge<string, string>> CreateMapEdgesList(string[,] mapArray, List<string[]> customLinks = null)
        {
            List<TaggedEdge<string, string>> edgesOut = new List<TaggedEdge<string, string>>();
            TaggedEdge<string, string> thisEdge = null;

            // iterate row
            for (int i = 0; i < mapArray.GetLength(0); i++)
            {
                // iterate column
                for (int j = 0; j < mapArray.GetLength(1); j++)
                {
                    // don't process null entries
                    if (!string.IsNullOrWhiteSpace(mapArray[i, j]))
                    {
                        // if not first hex in row, ADD LINKS BETWEEN THIS HEX/FIEF AND PREVIOUS HEX/FIEF
                        if (j != 0)
                        {
                            if (!string.IsNullOrWhiteSpace(mapArray[i, j - 1]))
                            {
                                // add link to previous
                                thisEdge = new TaggedEdge<string, string>(mapArray[i, j], mapArray[i, j - 1], "W");
                                edgesOut.Add(thisEdge);

                                // add link from previous
                                thisEdge = new TaggedEdge<string, string>(mapArray[i, j - 1], mapArray[i, j], "E");
                                edgesOut.Add(thisEdge);
                            }
                        }

                        // if not first row, ADD LINKS BETWEEN THIS HEX/FIEF AND HEX/FIEFS ABOVE
                        if (i != 0)
                        {
                            // keep track of target columns
                            int col = 0;

                            // if not first column in even-numbered row, add link between this hex/fief and hex/fief above left
                            if (!((i % 2 == 0) && (j == 0)))
                            {
                                // target correct column (above left is different for odd/even numbered rows)
                                if (i % 2 == 1)
                                {
                                    col = j;
                                }
                                else
                                {
                                    col = j - 1;
                                }

                                if (!string.IsNullOrWhiteSpace(mapArray[i - 1, col]))
                                {
                                    // add link to above left
                                    thisEdge = new TaggedEdge<string, string>(mapArray[i, j], mapArray[i - 1, col], "NW");
                                    edgesOut.Add(thisEdge);

                                    // add link from above left
                                    thisEdge = new TaggedEdge<string, string>(mapArray[i - 1, col], mapArray[i, j], "SE");
                                    edgesOut.Add(thisEdge);
                                }
                            }

                            // if not last column in odd-numbered row, add link between this hex/fief and hex/fief above right
                            if (!(i % 2 == 1 && (j == mapArray.GetLength(1) - 1)))
                            {
                                // target correct column (above right is different for odd/even numbered rows)
                                if (i % 2 == 1)
                                {
                                    col = j + 1;
                                }
                                else
                                {
                                    col = j;
                                }

                                if (!String.IsNullOrWhiteSpace(mapArray[i - 1, col]))
                                {
                                    // add link to above right
                                    thisEdge = new TaggedEdge<string, string>(mapArray[i, j], mapArray[i - 1, col], "NE");
                                    edgesOut.Add(thisEdge);

                                    // add link from above right
                                    thisEdge = new TaggedEdge<string, string>(mapArray[i - 1, col], mapArray[i, j], "SW");
                                    edgesOut.Add(thisEdge);
                                }
                            }
                        }
                    }
                }
            }

            // check for custom links
            if (customLinks != null)
            {
                foreach (string[] linkEntry in customLinks)
                {
                    thisEdge = new TaggedEdge<string, string>(linkEntry[0], linkEntry[1], linkEntry[2]);
                    edgesOut.Add(thisEdge);
                }
            }

            return edgesOut;
        }


    }
}
