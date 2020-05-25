using EtcordSharp.Packets.Types;
using EtcordSharp.Packets.Types.Data;

namespace EtcordSharp.Packets.Packets
{
    public struct GetChannels : IPacketStruct
    {
        public Array<ChannelData> channels;

        
        public GetChannels(int count)
        {
            channels = new Array<ChannelData>(count);
        }
    }
}
