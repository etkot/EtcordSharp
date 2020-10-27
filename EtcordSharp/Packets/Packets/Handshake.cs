using EtcordSharp.Packets.Attributes;
using EtcordSharp.Packets.Types;

namespace EtcordSharp.Packets.Packets
{
    [Packet(PacketType.Handshake, Reliable = true)]
    public struct Handshake : IPacketStruct
    {
        [EnumPackageType(typeof(VarInt))]
        public enum NextState
        {
            None,
            Login,
        }
        
        public VarInt protocolVersion;
        public NextState nextState;
    }
}
