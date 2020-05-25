using EtcordSharp.Packets.Types;
using EtcordSharp.Packets.Types.Data;

namespace EtcordSharp.Packets.Packets
{
    public struct VoiceChannelJoin : IPacketStruct
    {
        public VarInt channelID;
        public VarInt userID;
    }
}
