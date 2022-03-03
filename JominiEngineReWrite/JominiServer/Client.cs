using JominiGame;
using Lidgren.Network;
using ProtoMessageClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JominiServer
{
    public class Client
    {
        /// <summary>
        /// Holds the client's connection
        /// </summary>
        public NetConnection Connection { get; set; }
        /// <summary>
        /// The client's username, aka playerID
        /// </summary>
        public string Username { get; set; }
        /// <summary>
        /// Holds PlayerCharacter associated with the player using this client
        /// </summary>
        public PlayerCharacter MyPlayerCharacter { get; set; }
        /// <summary>
        /// Holds Character to view in UI
        /// </summary>
        public Character ActiveCharacter { get; set; }
        /// <summary>
        /// Holds the algorithm to be used during encryption and decryption. Alg is generated using the peer and a key obtained from the client 
        /// </summary>
        public NetAESEncryption EncryptionAlg = null;

        /// <summary>
        /// Holds past events
        /// </summary>
        public Journal MyPastEvents;
        /// <summary>
        /// Holds current set of events being displayed in UI
        /// </summary>
        public SortedList<uint, JournalEntry> EventSetToView;

        /// <summary>
        /// Create a new client object
        /// </summary>
        /// <param name="user">username</param>
        /// <param name="pcID">PlayerCharacterID</param>
        public Client(string Username, PlayerCharacter pcID)
        {
            // set username associated with client
            this.Username = Username;

            // get playercharacter from master list of player characers
            MyPlayerCharacter = pcID;
            MyPlayerCharacter.PlayerID = Username;
            ActiveCharacter = MyPlayerCharacter;

            // Set up journal
            MyPastEvents = new Journal();
            // Set up journal events
            EventSetToView = new SortedList<uint, JournalEntry>();

        }

        /// <summary>
        /// Updates the client
        /// </summary>
        /// <param name="message">The message code to send</param>
        /// <param name="fields">Additional information to add to the message</param>
        public void Update(DisplayMessages message, string[] fields = null)
        {
            ProtoMessage m = new ProtoMessage();
            m.ActionType = Actions.Update;
            m.ResponseType = message;
            m.MessageFields = fields;
            if (Connection != null)
            {
                // LOGGING Globals_Server.logEvent("Update " + this.username + ": " + message.ToString()); 
                // LOGGING Console.WriteLine("Sending update " + message.ToString() + " to " + this.username);
                //Server.SendViaProto(m, Connection, EncryptionAlg);
            }
        }



    }

    

}
