using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtoMessageClient
{
    [ProtoContract]
    public class ProtoPlayer : ProtoMessage
    {
        [ProtoMember(1)]
        public string PCID;
        [ProtoMember(2)]
        public string PCName;
        [ProtoMember(3)]
        public string PlayerID;
        [ProtoMember(4)]
        public string NatID;

        public ProtoPlayer()
        {
        }
    }
}
