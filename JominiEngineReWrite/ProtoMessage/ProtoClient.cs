using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JominiGame;
using ProtoBuf;

namespace ProtoMessageClient
{

    [ProtoContract]
    public class ProtoClient : ProtoMessage
    {

        [ProtoMember(1)]
        public Character PlayerCharacter;
        [ProtoMember(2)]
        public Character ActiveCharacter;
        [ProtoMember(3)]
        public double TravelModifier;
        [ProtoMember(4)]
        public double HomeFiefTreasury;
        [ProtoMember(5)]
        public uint Purse;
        [ProtoMember(6)]
        public uint Year;
        [ProtoMember(7)]
        public byte Season;

        public ProtoClient(Client cl)
        {

        }


    }
}
