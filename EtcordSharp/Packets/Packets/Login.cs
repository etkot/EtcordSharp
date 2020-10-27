using EtcordSharp.Packets.Attributes;
using EtcordSharp.Packets.Types.Data;

namespace EtcordSharp.Packets.Packets
{
    [Packet(PacketType.Login, Reliable = true)]
    public struct Login : IPacketStruct
    {
        public UserData user;
    }
}
