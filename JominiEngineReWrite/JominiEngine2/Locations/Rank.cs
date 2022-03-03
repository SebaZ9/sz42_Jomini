using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JominiGame
{
    /// <summary>
    /// Class storing data on rank and title
    /// </summary>
    public class Rank
    {
        /// <summary>
        /// Holds rank ID
        /// </summary>
        public byte ID { get; set; }
        /// <summary>
        /// Holds title name in various languages
        /// </summary>
        public Dictionary<Language, TitleName> Title { get; set; }
        /// <summary>
        /// Holds base stature for this rank
        /// </summary>
        public byte Stature { get; set; }

        /// <summary>
        /// Constructor for Rank
        /// </summary>
        /// <param name="ID">byte holding rank ID</param>
        /// <param name="Title">TitleName[] holding title name in various languages</param>
        /// <param name="Stature">byte holding base stature for rank</param>
        public Rank(byte ID, Dictionary<Language, TitleName> Title, byte Stature)
        {
            this.ID = ID;
            this.Title = Title;
            this.Stature = Stature;

        }

        /// <summary>
        /// Gets the correct name for the rank depending on the specified Language
        /// </summary>
        /// <returns>string containing the name</returns>
        /// <param name="language">The Language to be used</param>
        public string GetName(Language language)
        {
            // iterate through TitleNames and get correct name
            foreach (TitleName titleName in Title.Values)
            {
                if (titleName.LangID == language.ID)
                {
                    return titleName.Name;
                }
            }
            // Corrent name not found
            return Title.Values.ToArray()[0].Name;
        }


    }

    public struct TitleName
    {
        /// <summary>
        /// Holds Language ID or "generic"
        /// </summary>
        public string LangID;
        /// <summary>
        /// Holds title name associated with specific language
        /// </summary>
        public string Name;

        /// <summary>
        /// Constructor for TitleName
        /// </summary>
        /// <param name="LangID">string holding Language ID</param>
        /// <param name="Name">string holding title name associated with specific language</param>
        public TitleName(string LangID, string Name)
        {
            this.LangID = LangID;
            this.Name = Name;
        }

    }

}
