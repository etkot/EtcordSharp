using EtcordSharp.Packets.Attributes;
using EtcordSharp.Packets.Types;

namespace EtcordSharp.Packets.Packets
{
    [Packet(PacketType.VoiceData, Reliable = false)]
    public struct VoiceData : IPacketStruct
    {
        public VarInt channelID;
        public VarInt userID;
        public Array<byte> data;
    }
}
