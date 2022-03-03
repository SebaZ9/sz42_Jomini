using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JominiGame;
using JominiServer;
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
        public double HomeFiefTreasury;
        [ProtoMember(4)]
        public uint Purse;
        [ProtoMember(5)]
        public uint Year;
        [ProtoMember(6)]
        public byte Season;

        public ProtoClient(Client cl, uint Year, byte Season)
        {
            Purse = cl.MyPlayerCharacter.Purse;
            this.Year = Year;
            this.Season = Season;
            HomeFiefTreasury = cl.MyPlayerCharacter.HomeFief.GetAvailableTreasury(true);
            PlayerCharacter = cl.MyPlayerCharacter;
            if (cl.ActiveCharacter != cl.MyPlayerCharacter)
            {
                ActiveCharacter = cl.ActiveCharacter;
            }
            else
            {
                ActiveCharacter = cl.MyPlayerCharacter;
            }
        }


    }
}
