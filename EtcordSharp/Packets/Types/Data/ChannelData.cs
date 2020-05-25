using EtcordSharp.Packets.Attributes;

namespace EtcordSharp.Packets.Types.Data
{
    public struct ChannelData : IPacketSerializable
    {
        [EnumPackageType(typeof(VarInt))]
        public enum ChannelType
        {
            None = 0,
            TextChat = 1,
            VoiceChat = 2,
            Both = 3,
        }

        public VarInt ChannelID;
        public VarInt ParentID;
        public String Name;
        public ChannelType type;


        public int GetSize()
        {
            return ChannelID.GetSize() +
                ParentID.GetSize() +
                Name.GetSize() +
                PacketSerializer.GetObjectSize(type);
        }
        public bool Deserialize(byte[] bytes, ref int position)
        {
            if (!ChannelID.Deserialize(bytes, ref position)) return false;
            if (!ParentID.Deserialize(bytes, ref position)) return false;
            if (!Name.Deserialize(bytes, ref position)) return false;
            if (!PacketSerializer.Deserialize(bytes, ref position, out type)) return false;

            return true;
        }
        public bool Serialize(byte[] bytes, ref int position)
        {
            if (!ChannelID.Serialize(bytes, ref position)) return false;
            if (!ParentID.Serialize(bytes, ref position)) return false;
            if (!Name.Serialize(bytes, ref position)) return false;
            if (!PacketSerializer.Serialize(type, bytes, ref position)) return false;

            return true;
        }
    }
}
