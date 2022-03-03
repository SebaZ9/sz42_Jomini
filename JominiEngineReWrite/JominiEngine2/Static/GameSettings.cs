using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JominiGame
{
    public static class GameSettings
    {

        /// <summary>
        /// Holds probabilities for battle occurring at certain combat odds under certain conditions
        /// Key = 'odds', 'battle', 'pillage'
        /// Value = percentage likelihood of battle occurring
        /// </summary>
        public static readonly double[] BATTLE_PROBABILITIES_ODDS = { 2, 3, 4, 5, 6, 99 };
        public static readonly double[] BATTLE_PROBABILITIES_BATTLE = { 10, 30, 50, 70, 80, 90 };
        public static readonly double[] BATTLE_PROBABILITIES_PILLAGE = { 10, 20, 30, 40, 50, 60 };

        public static readonly int START_YEAR = 1337;

        public static readonly Dictionary<string, double[]> RECRUIT_RATIOS = new Dictionary<string, double[]>()
        {
            {"Eng", new double[] { 0.01, 0.02, 0, 0.12, 0.03, 0.32, 0.49 } },
            {"Fr", new double[] { 0.01, 0.02, 0.03, 0, 0.04, 0.40, 0.49 } },
            {"Sco", new double[] { 0.01, 0.02, 0, 0, 0.04, 0.43, 0.49 } },
            {"Oth", new double[] { 0.01, 0.02, 0.03, 0, 0.04, 0.40, 0.49 } }
        };

        public static readonly Dictionary<string, uint[]> COMBAT_VALUES = new Dictionary<string, uint[]>()
        {
            {"Eng", new uint[] { 9, 9, 1, 9, 5, 3, 1 } },
            {"Fr", new uint[] { 7, 7, 3, 2, 4, 2, 1 } },
            {"Sco", new uint[] { 8, 8, 1, 2, 4, 4, 1 } },
            {"Oth", new uint[] { 7, 7, 3, 2, 4, 2, 1 } }
        };

        public static readonly Dictionary<string, double[]> BATTLE_PROBABILITIES = new Dictionary<string, double[]>()
        {
            {"odds", new double[] { 2, 3, 4, 5, 6, 99 } },
            {"battle", new double[] { 10, 30, 50, 70, 80, 90 } },
            {"pillage", new double[] { 10, 20, 30, 40, 50, 60 } }
        };

        public static Dictionary<Tuple<uint, uint>, double> TROOP_TYPE_ADVANTAGE = new Dictionary<Tuple<uint, uint>, double>()
        {
            { new Tuple<uint, uint>(0, 5), 3 },
            { new Tuple<uint, uint>(0, 6), 3 },
            { new Tuple<uint, uint>(1, 5), 3 },
            { new Tuple<uint, uint>(1, 6), 3 },
            { new Tuple<uint, uint>(2, 3), 3 },
            { new Tuple<uint, uint>(2, 4), 3 },
            { new Tuple<uint, uint>(3, 5), 3 },
            { new Tuple<uint, uint>(3, 6), 3 },
            { new Tuple<uint, uint>(4, 0), 3 },
            { new Tuple<uint, uint>(4, 1), 3 },
            { new Tuple<uint, uint>(5, 2), 3 }
        };

    }
}
