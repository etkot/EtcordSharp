using EtcordSharp.Packets.Types.Data;

namespace EtcordSharp.Packets.Packets
{
    public struct Login : IPacketStruct
    {
        public UserData user;
    }
}
