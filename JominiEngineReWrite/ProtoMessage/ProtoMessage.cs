using ProtoBuf;

namespace ProtoMessageClient
{

    [ProtoContract]
    public class ProtoMessage
    {

        /// <summary>
        /// Contains the underlying type of the message. Used identify which action the client took
        /// </summary>
        [ProtoMember(1)]
        public Actions ActionType { get; set; }

        /// <summary>
        /// Contains a message or messageID for the client
        /// Used when sending error messages
        /// </summary>
        [ProtoMember(2)]
        public string Message;

        /// <summary>
        /// Contains any fields that need to be sent along with the message
        /// e.g. amount of overspend in fief
        /// </summary>
        [ProtoMember(3)]
        public string[] MessageFields;

        /// <summary>
        /// Contains the server response
        /// </summary>
        [ProtoMember(4)]
        public DisplayMessages ResponseType { get; set; }


    }
}
