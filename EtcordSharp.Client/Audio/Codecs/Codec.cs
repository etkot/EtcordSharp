using System;
using System.Collections.Generic;
using System.Text;

namespace EtcordSharp.Client.Audio.Codecs
{
    public abstract class Codec
    {
        public abstract byte[] Encode(short[] data);
        public abstract short[] Decode(byte[] data);
    }
}
