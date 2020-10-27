using EtcordSharp.Packets.Attributes;
using EtcordSharp.Packets.Types;
using EtcordSharp.Packets.Types.Data;

namespace EtcordSharp.Packets.Packets
{
    [Packet(Reliable = true)]
    public struct GetChatHistory : IPacketStruct
    {
        public VarInt channelID;
        public VarInt count;
        public VarInt offsetID;
        public Array<MessageData> messages;
    }
}
