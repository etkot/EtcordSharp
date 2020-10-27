using EtcordSharp.Packets.Attributes;
using LiteNetLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace EtcordSharp.Packets
{
    public static class PacketTransport
    {
        public static void SendPacket<T>(NetPeer peer, T packet) where T : IPacketStruct
        {
            Packet packetAttr = (Packet)typeof(T).GetCustomAttribute(typeof(Packet), false);
            if (packetAttr == null)
            {
                Console.WriteLine("Error: No Packet attribute on packet \"" + typeof(T).FullName + "\"");
                return;
            }

            DeliveryMethod deliveryMethod = packetAttr.Reliable ? DeliveryMethod.ReliableOrdered : DeliveryMethod.Unreliable;
            byte[] data = PacketSerializer.SerializePacket(packet);
            peer.Send(data, deliveryMethod);
        }

        public static void SendEvent(NetPeer peer, PacketType packetType)
        {
            peer.Send(PacketSerializer.SerializeEvent(packetType), DeliveryMethod.ReliableOrdered);
        }

        public static void Receive(NetPeer peer, object client, byte[] data, int offset)
        {
            bool reliable;
            byte[] response = PacketSerializer.ReceivePacket(client, data, offset, out reliable);
            if (response != null)
            {
                DeliveryMethod deliveryMethod = reliable ? DeliveryMethod.ReliableOrdered : DeliveryMethod.Unreliable;
                peer.Send(response, deliveryMethod);
            }
        }
    }
}
