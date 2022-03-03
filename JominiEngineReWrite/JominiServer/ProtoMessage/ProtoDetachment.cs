using JominiGame;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtoMessageClient
{
    /// <summary>
    /// Class for sending details of a detachment
    /// Character ID of PlayerCharacter leaving detachment is obtained via connection details
    /// </summary>
    [ProtoContract]
    public class ProtoDetachment : ProtoMessage
    {
        // ID of detachment
        [ProtoMember(1)]
        public string ID { get; set; }
        /// <summary>
        /// Array of troops (size = 7)
        /// </summary>
        [ProtoMember(2)]
        public uint[] Troops;
        /// <summary>
        /// Character detachment is left for
        /// </summary>
        [ProtoMember(3)]
        public string LeftFor { get; set; }
        /// <summary>
        /// ArmyID of army from which detachment was created
        /// </summary>
        [ProtoMember(4)]
        public string ArmyID { get; set; }
        /// <summary>
        /// Details of person who left this detachment (used in sending details of detachments to client)
        /// </summary>
        [ProtoMember(5)]
        public string LeftBy { get; set; }
        /// <summary>
        /// Days left of person who created detachment at time of creation
        /// </summary>
        [ProtoMember(6)]
        public int Days { get; set; }

        public ProtoDetachment(Detachment detachment)
        {
            ID = detachment.ID;
            Troops = detachment.Troops;
            LeftFor = detachment.LeftFor;
            ArmyID = detachment.FromArmy.ID;
            ActionType = Actions.DropOffTroops;
        }
        public ProtoDetachment() : base()
        {

        }
    }
}
