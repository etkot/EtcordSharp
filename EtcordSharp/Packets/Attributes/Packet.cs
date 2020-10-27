using System;
using System.Collections.Generic;
using System.Text;

namespace EtcordSharp.Packets.Attributes
{
    [AttributeUsage(AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
    public class Packet : Attribute
    {
        public bool Reliable { get; set; }
    }
}
