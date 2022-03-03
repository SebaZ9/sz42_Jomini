using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JominiGame
{
    /// <summary>
    /// Class allowing storage of game events (past and future)
    /// </summary>
    public class Journal
    {

        /// <summary>
        /// Holds entries
        /// </summary>
        public SortedList<uint, JournalEntry> Entries = new SortedList<uint, JournalEntry>();
        /// <summary>
        /// Indicates presence of new (unread) entries
        /// </summary>
        public bool areNewEntries = false;
        /// <summary>
        /// Priority level of new (unread) entries
        /// </summary>
        public byte priority = 0;

        /// <summary>
        /// Constructor for Journal
        /// </summary>
        /// <param name="entList">SortedList(uint, JournalEntry) holding entries</param>
        public Journal(SortedList<uint, JournalEntry> Entries = null)
        {
            if (Entries != null)
            {
                this.Entries = Entries;
            }
        }

        /// <summary>
        /// Returns any entries matching search criteria (year, season)
        /// </summary>
        /// <returns>SortedList of JournalEntrys</returns>
        /// <param name="yr">Year to search for</param>
        /// <param name="seas">Season to search for</param>
        public SortedList<uint, JournalEntry> GetEventsOnDate(uint yr = 9999, byte seas = 99)
        {
            SortedList<uint, JournalEntry> matchingEntries = new SortedList<uint, JournalEntry>();

            // determine scope of search
            string scope;
            // if no year specified, return events from all years and seasons
            if (yr == 9999)
            {
                scope = "all";
            }
            // if a year is specified
            else
            {
                // if no season specified, return events from all seasons in the specified year
                if (seas == 99)
                {
                    scope = "yr";
                }
                // if a season is specified, return events from specified season in the specified year
                else
                {
                    scope = "seas";
                }
            }

            switch (scope)
            {
                case "seas":
                    foreach (KeyValuePair<uint, JournalEntry> jEntry in Entries)
                    {
                        // year and season must match
                        if (jEntry.Value.Year == yr)
                        {
                            if (jEntry.Value.Season == seas)
                            {
                                matchingEntries.Add(jEntry.Key, jEntry.Value);
                            }
                        }
                    }
                    break;
                case "yr":
                    foreach (KeyValuePair<uint, JournalEntry> jEntry in Entries)
                    {
                        // year must match
                        if (jEntry.Value.Year == yr)
                        {
                            matchingEntries.Add(jEntry.Key, jEntry.Value);
                        }
                    }
                    break;
                default:
                    foreach (KeyValuePair<uint, JournalEntry> jEntry in Entries)
                    {
                        // get all events
                        matchingEntries.Add(jEntry.Key, jEntry.Value);
                    }
                    break;
            }

            return matchingEntries;
        }

        /// <summary>
        /// Retrieves all unviewed JournalEntrys
        /// </summary>
        /// <returns>SortedList(uint, JournalEntry) containing relevant entries</returns>
        public SortedList<uint, JournalEntry> GetUnviewedEntries()
        {
            SortedList<uint, JournalEntry> foundEntries = new SortedList<uint, JournalEntry>();

            foreach (KeyValuePair<uint, JournalEntry> jEntry in Entries)
            {
                if (!jEntry.Value.Viewed)
                {
                    foundEntries.Add(jEntry.Key, jEntry.Value);
                }
            }

            return foundEntries;
        }

        /// <summary>
        /// Retrieves JournalEntrys associated with the specified character, role, and JournalEntry type
        /// </summary>
        /// <returns>List<JournalEntry> containing relevant entries</returns>
        /// <param name="thisPerson">The ID of the person of interest</param>
        /// <param name="role">The person's role (in personae)</param>
        /// <param name="entryType">The JournalEntry type</param>
        public List<JournalEntry> GetSpecificEntries(string thisPersonID, string role, string entryType)
        {
            List<JournalEntry> foundEntries = new List<JournalEntry>();
            bool entryFound = false;

            foreach (KeyValuePair<uint, JournalEntry> jEntry in Entries)
            {
                // get entries of specified type
                if (jEntry.Value.Type.Equals(entryType))
                {
                    // iterate through personae
                    for (int i = 0; i < jEntry.Value.Personae.Length; i++)
                    {
                        // get and split personae
                        string thisPersonae = jEntry.Value.Personae[i];
                        string[] thisPersonaeSplit = thisPersonae.Split('|');

                        if (thisPersonaeSplit[0] != null)
                        {
                            // look for specified role
                            if (thisPersonaeSplit[1].Equals(role))
                            {
                                // look for matching charID
                                if (thisPersonaeSplit[0].Equals(thisPersonID))
                                {
                                    foundEntries.Add(jEntry.Value);
                                    entryFound = true;
                                    break;
                                }
                            }
                        }

                    }
                }

                if (entryFound)
                {
                    break;
                }
            }

            return foundEntries;
        }

        /// <summary>
        /// Adds a new JournalEntry to the Journal
        /// </summary>
        /// <returns>bool indicating success</returns>
        /// <param name="min">The JournalEntry to be added</param>
        public bool AddNewEntry(JournalEntry jEntry)
        {
            bool success = false;

            if (jEntry.EntryID > 0)
            {
                try
                {
                    // add entry
                    Entries.Add(jEntry.EntryID, jEntry);

                    if (Entries.ContainsKey(jEntry.EntryID))
                    {
                        success = true;
                    }
                    else
                    {
                        //HACK
                        /*
                        if (Globals_Client.showMessages)
                        {
                            System.Windows.Forms.MessageBox.Show("Error: JournalEntry not added.", "INSERTION ERROR");
                        }*/
                    }
                }
                catch (System.ArgumentException ae)
                {
                    //HACK
                    /*
                    if (Globals_Client.showMessages)
                    {
                        System.Windows.Forms.MessageBox.Show(ae.Message + "\r\nPlease check for duplicate jEventID.", "INSERTION ERROR");
                    }*/
                }
            }

            return success;

        }

        /// <summary>
        /// Returns a JournalEntry set, based on criteria passed in
        /// </summary>
        /// <returns>SortedList containing JournalEntrys</returns>
        /// <param name="setScope">The type of JournalEvent set to fetch</param>
        public SortedList<uint, JournalEntry> getJournalEntrySet(string setScope, uint thisYear, byte thisSeason)
        {
            SortedList<uint, JournalEntry> jEntrySet = new SortedList<uint, JournalEntry>();

            // get appropriate jEntry set
            switch (setScope)
            {
                // get entries for current year
                case "year":
                    jEntrySet = GetEventsOnDate(yr: thisYear);
                    break;
                // get entries for current season
                case "season":
                    jEntrySet = GetEventsOnDate(yr: thisYear, seas: thisSeason);
                    break;
                // get unread entries
                case "unread":
                    jEntrySet = GetUnviewedEntries();
                    break;
                // get all entries
                default:
                    jEntrySet = GetEventsOnDate();
                    break;
            }

            return jEntrySet;

        }

    }

    // Journal entries could be much better represented, as there are a fixed number of types and so the description can be stored on the client side
    /// <summary>
    /// Class containing details of a Journal entry
    /// </summary>
    public class JournalEntry
    {
        /// <summary>
        /// Holds JournalEntry ID
        /// </summary>
        public uint EntryID { get; set; }
        /// <summary>
        /// Holds event year
        /// </summary>
        public uint Year { get; set; }
        /// <summary>
        /// Holds event season
        /// </summary>
        public byte Season { get; set; }
        /// <summary>
        /// Holds main objects (IDs) associated with event and their role
        /// </summary>
        public string[] Personae { get; set; }
        /// <summary>
        /// Holds type of event (e.g. battle, birth)
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// Holds location of event (fiefID)
        /// </summary>
        public string Location { get; set; }
        /// <summary>
        /// Indicates whether entry has been viewed
        /// </summary>
        public bool Viewed { get; set; }
        /// <summary>
        /// Indicates whether entry has been replied to (e.g. for Proposals)
        /// </summary>
        public bool Replied { get; set; }
        /// <summary>
        /// Holds ProtoMessage containing details of event. More flexible than strings.
        /// </summary>
        public string EntryDetails { get; set; }
        /// <summary>
        /// Constructor for JournalEntry
        /// </summary>
        /// <param name="id">uint holding JournalEntry ID</param>
        /// <param name="yr">uint holding event year</param>
        /// <param name="seas">byte holding event season</param>
        /// <param name="pers">String[] holding main objects (IDs) associated with event and thier role</param>
        /// <param name="typ">String holding type of event</param>
        /// <param name="loc">String holding location of event (fiefID)</param>
        /// <param name="messageIdentifier">Enum representing description of event</param>
        public JournalEntry(uint EntryID, uint Year, byte Season, string[] Personae, string Type, string EntryDetails, string Location = null)
        {
            this.EntryID = EntryID;
            this.Year = Year;
            this.Season = Season;
            this.Personae = Personae;
            this.Type = Type;
            this.Location = Location;
            this.EntryDetails = EntryDetails;
            Viewed = false;
        }

        /// <summary>
        /// Check to see if the JournalEntry is a valid proposal
        /// </summary>
        /// <returns>bool indicating whether the controls be enabled</returns>
        public bool CheckForProposal(PlayerCharacter playerChar)
        {
            bool isValidProposal = false;

            // check if is a marriage proposal
            if (Type.Equals("proposalMade"))
            {
                // check if have already replied
                if (Replied)
                {
                    // check if player made or received proposal
                    for (int i = 0; i < Personae.Length; i++)
                    {
                        string thisPersonae = Personae[i];
                        string[] thisPersonaeSplit = thisPersonae.Split('|');
                        if (thisPersonaeSplit[0].Equals(playerChar.ID))
                        {
                            if (thisPersonaeSplit[1].Equals("headOfFamilyBride"))
                            {
                                isValidProposal = true;
                                break;
                            }
                        }
                    }
                }
            }

            return isValidProposal;
        }

    }

}
