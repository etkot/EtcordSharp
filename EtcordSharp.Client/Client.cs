using EtcordSharp.Packets;
using EtcordSharp.Packets.Attributes;
using EtcordSharp.Packets.Structs;
using System;
using System.Threading;

namespace EtcordSharp.Client
{
    public class Client
    {
        private Telepathy.Client tcpClient;
        private Thread clientThread;

        private const int ProtocolVersion = 0;


        public void Connect(string address, int port)
        {
            Telepathy.Logger.Log = msg => Console.WriteLine("Telepathy: " + msg);
            Telepathy.Logger.LogWarning = msg => Console.WriteLine("Telepathy: " + msg);
            Telepathy.Logger.LogError = msg => Console.WriteLine("Telepathy: " + msg);

            // create and connect the client
            tcpClient = new Telepathy.Client();
            tcpClient.Connect(address, port);

            Console.WriteLine("Connecting");

            if (clientThread == null)
            {
                clientThread = new Thread(() => { Receive(); });
                clientThread.Start();
            }
        }

        private void Receive()
        {
            Telepathy.Message msg;
            while (true)
            {
                if (tcpClient.GetNextMessage(out msg))
                {
                    switch (msg.eventType)
                    {
                        case Telepathy.EventType.Connected:
                            OnClientConnected(msg);
                            break;
                        case Telepathy.EventType.Disconnected:
                            OnClientDisconnected(msg);
                            break;
                        case Telepathy.EventType.Data:
                            OnClientData(msg);
                            break;
                    }
                }
            }
        }

        private void OnClientConnected(Telepathy.Message msg)
        {
            Console.WriteLine(msg.connectionId + " Connected");

            tcpClient.Send(PacketSerializer.SerializePacket(PacketType.Handshake, new Packets.Structs.Handshake
            {
                protocolVersion = ProtocolVersion,
                nextState = Packets.Structs.Handshake.NextState.Login,
            }));
        }

        private void OnClientDisconnected(Telepathy.Message msg)
        {
            Console.WriteLine(msg.connectionId + " Disconnected");
        }

        private void OnClientData(Telepathy.Message msg)
        {
            Console.WriteLine("Received Data");

            PacketSerializer.ReceivePacket(this, msg.data);
        }


        #region Packet receivers
        
        [PacketReceiver(PacketType.Handshake)]
        public void Handshake(Handshake handshake)
        {
            Console.WriteLine("Handshake");

            Console.WriteLine(handshake.protocolVersion);
            Console.WriteLine(handshake.nextState);
        }

        #endregion
    }
}
