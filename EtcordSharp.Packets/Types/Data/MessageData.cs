using EtcordSharp.Packets.Attributes;

namespace EtcordSharp.Packets.Types.Data
{
    public struct MessageData : IPacketSerializable
    {
        public VarInt MessageID;
        public VarInt SenderID;
        public String SenderName;
        public String Content;


        public int GetSize()
        {
            return MessageID.GetSize() +
                SenderID.GetSize() +
                SenderName.GetSize() +
                Content.GetSize();
        }
        public bool Deserialize(byte[] bytes, ref int position)
        {
            if (!MessageID.Deserialize(bytes, ref position)) return false;
            if (!SenderID.Deserialize(bytes, ref position)) return false;
            if (!SenderName.Deserialize(bytes, ref position)) return false;
            if (!Content.Serialize(bytes, ref position)) return false;

            return true;
        }
        public bool Serialize(byte[] bytes, ref int position)
        {
            if (!MessageID.Serialize(bytes, ref position)) return false;
            if (!SenderID.Serialize(bytes, ref position)) return false;
            if (!SenderName.Serialize(bytes, ref position)) return false;
            if (!Content.Serialize(bytes, ref position)) return false;

            return true;
        }
    }
}
