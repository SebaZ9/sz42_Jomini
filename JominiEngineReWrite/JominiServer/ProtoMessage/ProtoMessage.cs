using ProtoBuf;

namespace ProtoMessageClient
{

    [ProtoContract]
    [ProtoInclude(101, typeof(ProtoCharacter))]
    [ProtoInclude(102, typeof(ProtoCharacterOverview))]
    [ProtoInclude(103, typeof(ProtoArmy))]
    [ProtoInclude(104, typeof(ProtoArmyOverview))]
    [ProtoInclude(105, typeof(ProtoFief))]
    [ProtoInclude(106, typeof(ProtoDetachment))]
    [ProtoInclude(150, typeof(ProtoGenericArray<ProtoPlayer>))]
    [ProtoInclude(151, typeof(ProtoGenericArray<ProtoFief>))]
    [ProtoInclude(152, typeof(ProtoGenericArray<ProtoCharacterOverview>))]
    [ProtoInclude(153, typeof(ProtoGenericArray<ProtoArmyOverview>))]
    [ProtoInclude(154, typeof(ProtoGenericArray<ProtoDetachment>))]
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

        public ProtoMessage()
        {

        }

    }

    [ProtoContract]
    public class Pair
    {
        [ProtoMember(1)]
        public string Key { get; set; }
        [ProtoMember(2)]
        public string Value { get; set; }
        public Pair()
        { 
        }
    }

    [ProtoContract]
    public class ProtoGenericArray<T> : ProtoMessage where T : class
    {
        [ProtoMember(1)]
        public T[] fields { get; set; }
        public ProtoGenericArray()
            : base()
        {

        }

        public ProtoGenericArray(T[] t)
        {
            fields = t;
        }
    }

}
