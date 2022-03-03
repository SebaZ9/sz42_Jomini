using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtoMessageClient
{
    [ProtoContract]
    public class ProtoLogIn : ProtoMessage
    {
        /// <summary>
        /// The session salt, used to salt the password hash
        /// </summary>
        [ProtoMember(1)]
        public byte[] SessionSalt { get; set; }
        /// <summary>
        /// The user's salt, used to salt the password hash
        /// </summary>
        [ProtoMember(2)]
        public byte[] UserSalt { get; set; }
        /// <summary>
        /// Key used in symmetric encryption. This key should be created on the client side and encrypted with the server's public key, then decrypted on the server side with the public key
        /// </summary>
        [ProtoMember(3)]
        public byte[] Key { get; set; }
        /// <summary>
        /// Challenge text to be signed by server
        /// </summary>
        [ProtoMember(4)]
        public string Text { get; set; }
        /// <summary>
        /// Result of server signing certificate
        /// </summary>
        [ProtoMember(5)]
        public byte[] Signature { get; set; }
        /// <summary>
        /// Holds the X509 certificate as a byte array for optionally verifying the peer
        /// </summary>
        [ProtoMember(6)]
        public byte[] Certificate { get; set; }
    }
}
