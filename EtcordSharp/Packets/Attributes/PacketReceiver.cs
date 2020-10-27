using System;

namespace EtcordSharp.Packets.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class PacketReceiver : Attribute
    {
        public PacketType MessageType { get; private set; }

        public PacketReceiver(PacketType messageType)
        {
            MessageType = messageType;
        }
    }
}
