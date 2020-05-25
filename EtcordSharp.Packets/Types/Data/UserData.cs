using EtcordSharp.Packets.Attributes;

namespace EtcordSharp.Packets.Types.Data
{
    public struct UserData : IPacketSerializable
    {
        public VarInt userID;
        public String name;


        public int GetSize()
        {
            return userID.GetSize() +
                name.GetSize();
        }
        public bool Deserialize(byte[] bytes, ref int position)
        {
            if (!userID.Deserialize(bytes, ref position)) return false;
            if (!name.Deserialize(bytes, ref position)) return false;

            return true;
        }
        public bool Serialize(byte[] bytes, ref int position)
        {
            if (!userID.Serialize(bytes, ref position)) return false;
            if (!name.Serialize(bytes, ref position)) return false;

            return true;
        }
    }
}
