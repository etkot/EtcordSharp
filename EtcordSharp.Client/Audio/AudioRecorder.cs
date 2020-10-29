using Concentus.Structs;
using System;
using System.Collections.Generic;
using System.Text;

namespace EtcordSharp.Client.Audio
{
    public abstract class AudioRecorder
    {
        public abstract bool GetExact(short[] data, int offset, int length);
    }
}
