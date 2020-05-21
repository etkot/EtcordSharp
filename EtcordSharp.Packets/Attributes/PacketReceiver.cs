using System;

namespace EtcordSharp.Packets.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class PacketReceiver : Attribute
    {
        public PacketType messageType { get; private set; }

        public PacketReceiver(PacketType messageType)
        {
            this.messageType = messageType;
        }
    }
}
