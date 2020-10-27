using EtcordSharp.Packets.Attributes;
using EtcordSharp.Packets.Types;
using EtcordSharp.Packets.Types.Data;

namespace EtcordSharp.Packets.Packets
{
    [Packet(PacketType.GetUsers, Reliable = true)]
    public struct GetUsers : IPacketStruct
    {
        public Array<UserData> users;


        public GetUsers(int count)
        {
            users = new Array<UserData>(count);
        }
        public GetUsers(UserData userData)
        {
            users = new Array<UserData>(new UserData[] { userData });
        }
        public GetUsers(UserData[] userDatas)
        {
            users = new Array<UserData>(userDatas);
        }
    }
}
