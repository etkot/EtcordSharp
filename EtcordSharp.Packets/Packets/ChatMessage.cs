using EtcordSharp.Packets.Types;
using EtcordSharp.Packets.Types.Data;

namespace EtcordSharp.Packets.Packets
{
    public struct ChatMessage : IPacketStruct
    {
        public VarInt channelID;
        public MessageData message;
    }
}
