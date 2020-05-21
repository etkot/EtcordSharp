using EtcordSharp.Packets;
using System;
using System.Collections.Generic;
using System.Text;

namespace EtcordSharp
{
    public class Server
    {
        private Telepathy.Server tcpServer;

        private Dictionary<int, Client> clients;

        public const int ProtocolVersion = 0;


        public void Start(int port)
        {
            clients = new Dictionary<int, Client>();

            Telepathy.Logger.Log = msg => Console.WriteLine("Telepathy: " + msg);
            Telepathy.Logger.LogWarning = msg => Console.WriteLine("Telepathy: " + msg);
            Telepathy.Logger.LogError = msg => Console.WriteLine("Telepathy: " + msg);

            // create and start the server
            tcpServer = new Telepathy.Server();
            tcpServer.Start(port);

            Receive();
        }

        private void Receive()
        {
            Telepathy.Message msg;
            while (true)
            {
                if (tcpServer.GetNextMessage(out msg))
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

            Client client = new Client(this, msg.connectionId);
            clients.Add(msg.connectionId, client);
        }

        private void OnClientDisconnected(Telepathy.Message msg)
        {
            Console.WriteLine(msg.connectionId + " Disconnected");

            clients.Remove(msg.connectionId);
        }

        private void OnClientData(Telepathy.Message msg)
        {
            Console.WriteLine(msg.connectionId + " Sent data");
            
            Client client;
            if (clients.TryGetValue(msg.connectionId, out client))
            {
                byte[] response = PacketSerializer.ReceivePacket(client, msg.data);
                if (response != null)
                {
                    SendData(msg.connectionId, response);
                }
            }
        }


        public void SendPacket<T>(Client client, PacketType packetType, T packet) where T : IPacketStruct
        {
            SendData(client.connectionId, PacketSerializer.SerializePacket(packetType, packet));
        }
        private void SendData(int connectionId, byte[] data)
        {
            tcpServer.Send(connectionId, data);
        }

        public void DisconnectClient(Client client)
        {
            tcpServer.Disconnect(client.connectionId);
        }
    }
}
