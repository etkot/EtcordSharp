using EtcordSharp.Packets;
using System;
using System.Collections.Generic;
using System.Text;
using LiteNetLib;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace EtcordSharp.Server
{
    public class Server : INetEventListener, INetLogger
    {
        private NetManager netServer;

        public Dictionary<int, ServerClient> Clients { get; private set; }
        public Dictionary<int, ServerChannel> Channels { get; private set; }

        public const int ProtocolVersion = 0;


        public Server()
        {

        }

        public void Start(int port)
        {
            Clients = new Dictionary<int, ServerClient>();
            Channels = new Dictionary<int, ServerChannel>()
            {
                { 1, new ServerChannel(this, 1, 0, "Parent", ServerChannel.ChannelType.None) },
                { 2, new ServerChannel(this, 2, 1, "Text Chat", ServerChannel.ChannelType.TextChat) },
                { 3, new ServerChannel(this, 3, 1, "Voice Chat", ServerChannel.ChannelType.VoiceChat) },
                { 4, new ServerChannel(this, 4, 0, "Both", ServerChannel.ChannelType.Both) },
            };

            // create and start the server
            NetDebug.Logger = this;
            netServer = new NetManager(this);
            netServer.Start(port);
            netServer.BroadcastReceiveEnabled = true;
            netServer.UpdateTime = 15;

            while (true)
            {
                netServer.PollEvents();
                Thread.Sleep(10);
            }
        }

        public void SendToAuthenticated<T>(PacketType packetType, T packet) where T : IPacketStruct
        {
            foreach (KeyValuePair<int, ServerClient> pair in Clients)
            {
                ServerClient client = pair.Value;
                if (client.State == ServerClient.ClientState.Authenticated)
                {
                    PacketTransport.SendPacket(client.Peer, packetType, packet);
                }
            }
        }


        #region Message receive

        public void OnPeerConnected(NetPeer peer)
        {
            Console.WriteLine(peer.Id + " Connected");

            ServerClient client = new ServerClient(this, peer);
            Clients.Add(peer.Id, client);
        }

        public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            Console.WriteLine(peer.Id + " Disconnected");

            Clients.Remove(peer.Id);
        }

        public void OnNetworkError(IPEndPoint endPoint, SocketError socketError)
        {
            Console.WriteLine("[SERVER] error " + socketError);
        }

        public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
        {
            Console.WriteLine(peer.Id + " Sent data");

            ServerClient client;
            if (Clients.TryGetValue(peer.Id, out client))
            {
                PacketTransport.Receive(peer, client, reader.RawData, reader.Position);
            }
        }

        public void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType)
        {
            throw new NotImplementedException();
        }

        public void OnNetworkLatencyUpdate(NetPeer peer, int latency)
        {

        }

        public void OnConnectionRequest(ConnectionRequest request)
        {
            // TODO: Limit connections with count and password

            request.Accept();
        }

        #endregion Message receive

        public void WriteNet(NetLogLevel level, string str, params object[] args)
        {
            switch (level)
            {
                case NetLogLevel.Warning:
                    Console.WriteLine("Warning: " + str);
                    break;
                case NetLogLevel.Error:
                    Console.WriteLine("Error: " + str);
                    break;
                case NetLogLevel.Trace:
                    Console.WriteLine("Trace: " + str);
                    break;
                case NetLogLevel.Info:
                    Console.WriteLine("Info: " + str);
                    break;
                default:
                    break;
            }
        }
    }
}
