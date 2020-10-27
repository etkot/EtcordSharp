using EtcordSharp.Packets.Attributes;
using EtcordSharp.Packets.Types;

namespace EtcordSharp.Packets
{
    [EnumPackageType(typeof(VarInt))]
    public enum PacketType
    {
        // Connection messages
        Handshake = 0x00,
        Login = 0x01,
        Kicked = 0x0a,
        Banned = 0x0b,

        // Server events (Only sent to client)
        ClientConnected = 0x10,
        ClientDisconnected = 0x11,

        // Client info requests
        GetUsers = 0x20,
        GetChannels = 0x21,

        // Text chat
        GetChatHistory = 0x30,
        ChatMessage = 0x31,

        // Voice chat
        VoiceChannelJoin = 0x40,
        VoiceChannelLeave = 0x41,
        VoiceData = 0x42,
    }
}
