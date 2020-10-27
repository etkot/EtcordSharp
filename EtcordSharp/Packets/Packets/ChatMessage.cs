using EtcordSharp.Packets.Attributes;
using EtcordSharp.Packets.Types;
using EtcordSharp.Packets.Types.Data;

namespace EtcordSharp.Packets.Packets
{
    [Packet(Reliable = true)]
    public struct ChatMessage : IPacketStruct
    {
        public VarInt channelID;
        public MessageData message;
    }
}
