using System;
using System.Collections.Generic;
using System.Text;

namespace EtcordSharp.Packets
{
    public interface IPacketSerializable
    {
        void Serialize(byte[] bytes, ref int position);
        void Deserialize(byte[] bytes, ref int position);
        int GetSize();
    }
    public interface IPacketStruct { }
}
