using System;
using System.Collections.Generic;
using System.Text;

namespace EtcordSharp.Packets
{
    interface IPacketSerializable
    {
        bool Serialize(byte[] bytes, ref int position);
        bool Deserialize(byte[] bytes, ref int position);
        int GetSize();
    }
    public interface IPacketStruct { }
}
