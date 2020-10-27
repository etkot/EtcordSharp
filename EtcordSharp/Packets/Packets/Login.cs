using EtcordSharp.Packets.Attributes;
using EtcordSharp.Packets.Types.Data;

namespace EtcordSharp.Packets.Packets
{
    [Packet(Reliable = true)]
    public struct Login : IPacketStruct
    {
        public UserData user;
    }
}
