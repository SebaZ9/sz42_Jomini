using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JominiGame
{
    /// <summary>
    /// Class storing data on language
    /// </summary>
    public class Language
    {
        /// <summary>
        /// Holds language ID
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// Holds base language
        /// </summary>
        public BaseLanguage BaseLang { get; set; }
        /// <summary>
        /// Holds language dialect code
        /// </summary>
        public int Dialect { get; set; }

        /// <summary>
        /// Constructor for Language
        /// </summary>
        /// <param name="BaseLang">BaseLanguage for the language</param>
        /// <param name="Dialect">int holding language dialect code</param>
        public Language(BaseLanguage BaseLang, int Dialect)
        {
            this.BaseLang = BaseLang;
            this.Dialect = Dialect;
            ID = this.BaseLang.BaseLangID + this.Dialect;
        }

    }

}