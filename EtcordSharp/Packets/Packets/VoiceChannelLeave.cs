using EtcordSharp.Packets.Types;
using EtcordSharp.Packets.Attributes;

namespace EtcordSharp.Packets.Packets
{
    [Packet(PacketType.VoiceChannelLeave, Reliable = true)]
    public struct VoiceChannelLeave : IPacketStruct
    {
        [EnumPackageType(typeof(VarInt))]
        public enum LeaveReason
        {
            Left,
            Moved,
            Kicked,
            Banned,
        }

        public VarInt channelID;
        public VarInt userID;
        public LeaveReason reason;
    }
}
