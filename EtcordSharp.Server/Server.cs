using EtcordSharp.Packets;
using System;
using System.Collections.Generic;
using System.Text;

namespace EtcordSharp.Server
{
    public class Server
    {
        private Telepathy.Server tcpServer;

        public Dictionary<int, ServerClient> Clients { get; private set; }
        public Dictionary<int, ServerChannel> Channels { get; private set; }

        public const int ProtocolVersion = 0;


        public void Start(int port)
        {
            Telepathy.Logger.Log = msg => Console.WriteLine("Telepathy: " + msg);
            Telepathy.Logger.LogWarning = msg => Console.WriteLine("Telepathy: " + msg);
            Telepathy.Logger.LogError = msg => Console.WriteLine("Telepathy: " + msg);


            Clients = new Dictionary<int, ServerClient>();
            Channels = new Dictionary<int, ServerChannel>()
            {
                { 1, new ServerChannel(this, 1, 0, "New Channel", ServerChannel.ChannelType.Both) }
            };

            // create and start the server
            tcpServer = new Telepathy.Server();
            tcpServer.Start(port);

            Receive();
        }

        public void SendPacketToConnected<T>(PacketType packetType, T packet) where T : IPacketStruct
        {
            foreach (KeyValuePair<int, ServerClient> client in Clients)
            {
                if (client.Value.State == ServerClient.ClientState.Connected)
                    SendData(client.Key, PacketSerializer.SerializePacket(packetType, packet));
            }
        }
        public void SendPacket<T>(ServerClient client, PacketType packetType, T packet) where T : IPacketStruct
        {
            SendData(client.ConnectionId, PacketSerializer.SerializePacket(packetType, packet));
        }
        private void SendData(int connectionId, byte[] data)
        {
            tcpServer.Send(connectionId, data);
        }

        public void DisconnectClient(ServerClient client)
        {
            tcpServer.Disconnect(client.ConnectionId);
        }


        #region Message receive

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

            ServerClient client = new ServerClient(this, msg.connectionId);
            Clients.Add(msg.connectionId, client);
        }

        private void OnClientDisconnected(Telepathy.Message msg)
        {
            Console.WriteLine(msg.connectionId + " Disconnected");

            Clients.Remove(msg.connectionId);
        }

        private void OnClientData(Telepathy.Message msg)
        {
            Console.WriteLine(msg.connectionId + " Sent data");
            
            ServerClient client;
            if (Clients.TryGetValue(msg.connectionId, out client))
            {
                byte[] response = PacketSerializer.ReceivePacket(client, msg.data);
                if (response != null)
                {
                    SendData(msg.connectionId, response);
                }
            }
        }

        #endregion Message receive
    }
}
