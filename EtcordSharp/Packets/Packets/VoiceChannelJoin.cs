using EtcordSharp.Packets.Attributes;
using EtcordSharp.Packets.Types;
using EtcordSharp.Packets.Types.Data;

namespace EtcordSharp.Packets.Packets
{
    [Packet(Reliable = true)]
    public struct VoiceChannelJoin : IPacketStruct
    {
        public VarInt channelID;
        public VarInt userID;
    }
}
