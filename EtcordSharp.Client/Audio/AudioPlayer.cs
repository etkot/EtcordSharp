﻿using Concentus.Structs;
using System;
using System.Collections.Generic;
using System.Text;

namespace EtcordSharp.Client.Audio
{
    public abstract class AudioPlayer
    {
        public abstract void Play(short[] data, int offset, int length);
    }
}
