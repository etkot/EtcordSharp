using EtcordSharp.Packets.Attributes;
using EtcordSharp.Packets.Types;

namespace EtcordSharp.Packets.Structs
{
    public struct Login : IPacketStruct
    {
        public String Username;
        public VarInt ClientID;
    }
}
