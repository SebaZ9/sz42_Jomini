using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JominiGame
{
    /// <summary>
    /// Class storing base langauge data
    /// </summary>
    public class BaseLanguage
    {
        /// <summary>
        /// Holds base langauge ID
        /// </summary>
        public string BaseLangID { get; set; }
        /// <summary>
        /// Holds base language name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Constructor for BaseLanguage
        /// </summary>
        /// <param name="BaseLangID">String holding language ID</param>
        /// <param name="Name">String holding language name</param>
        public BaseLanguage(string BaseLangID, string Name)
        {
            this.BaseLangID = BaseLangID;
            this.Name = Name;
        }


    }
}
