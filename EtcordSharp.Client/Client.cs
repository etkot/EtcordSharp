using EtcordSharp.Packets;
using EtcordSharp.Packets.Packets;
using System;
using System.Collections.Generic;
using System.Threading;

namespace EtcordSharp.Client
{
    public partial class Client
    {
        private const int ProtocolVersion = 0;

        public enum ClientState
        {
            Unconnected,
            Handshaking,
            Login,
            Connected,
        }

        private ClientState state;
        public ClientState State
        {
            get { return state; }
            private set
            {
                state = value;
                OnClientStateChanged?.Invoke(value);
            }
        }
        public string Username { get; private set; }

        public Dictionary<int, ClientChannel> Channels { get; private set; }

        public Action<ClientState> OnClientStateChanged;
        public Action<ClientChannel> OnChannelAdded;
        public Action<ClientChannel> OnChannelUpdated;
        public Action<ClientMessage> OnMessageAdded;

        private Telepathy.Client tcpClient;
        private int clientID;


        public Client()
        {
            Telepathy.Logger.Log = msg => Console.WriteLine("Telepathy: " + msg);
            Telepathy.Logger.LogWarning = msg => Console.WriteLine("Telepathy: " + msg);
            Telepathy.Logger.LogError = msg => Console.WriteLine("Telepathy: " + msg);

            Channels = new Dictionary<int, ClientChannel>();

            tcpClient = new Telepathy.Client();
            State = ClientState.Unconnected;
        }



        public void Connect(string address, int port)
        {
            Console.WriteLine("Connecting");

            Connect(address, port, Username);
        }
        public void Connect(string address, int port, string username)
        {
            Console.WriteLine("Connecting");

            tcpClient.Connect(address, port);
            Username = username;
        }

        public void SendPacket<T>(PacketType packetType, T packet) where T : IPacketStruct
        {
            tcpClient.Send(PacketSerializer.SerializePacket(packetType, packet));
        }
        public void SendEvent(PacketType packetType)
        {
            tcpClient.Send(PacketSerializer.SerializeEvent(packetType));
        }

        public void Disconnect()
        {
            tcpClient.Disconnect();
            State = ClientState.Unconnected;
            Username = "";
        }



        public void SetUsername(string name)
        {
            Username = name;
            if (State == ClientState.Login)
            {
                SendPacket(PacketType.Login, new Login()
                {
                    Username = name,
                });
            }
        }

        public void SendMessage(ClientChannel channel, string content)
        {
            Console.WriteLine("Sending chat message to channel " + channel.ChannelID + " with content \"" + content + "\"");

            SendPacket(PacketType.ChatMessage, new ChatMessage
            {
                channelID = channel.ChannelID,
                message = new Packets.Types.Data.MessageData
                {
                    Content = content
                }
            });
        }



        #region Message receive

        public void Receive()
        {
            Telepathy.Message msg;
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

        private void OnClientConnected(Telepathy.Message msg)
        {
            Console.WriteLine("Connected");

            State = ClientState.Handshaking;
            tcpClient.Send(PacketSerializer.SerializePacket(PacketType.Handshake, new Packets.Packets.Handshake
            {
                protocolVersion = ProtocolVersion,
                nextState = Packets.Packets.Handshake.NextState.Login,
            }));
        }

        private void OnClientDisconnected(Telepathy.Message msg)
        {
            Console.WriteLine("Disconnected");
        }

        private void OnClientData(Telepathy.Message msg)
        {
            //Console.WriteLine("Received Data");

            PacketSerializer.ReceivePacket(this, msg.data);
        }

        #endregion Message Receive
    }
}
