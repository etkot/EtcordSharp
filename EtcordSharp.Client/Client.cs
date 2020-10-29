using EtcordSharp.Packets;
using EtcordSharp.Packets.Packets;
using System;
using System.Collections.Generic;
using System.Threading;
using LiteNetLib;
using System.Net;
using System.Net.Sockets;
using EtcordSharp.Client.Audio;
using EtcordSharp.Packets.Types.Data;

namespace EtcordSharp.Client
{
    public partial class Client : INetEventListener
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
        public ClientUser User { get; private set; }

        public Dictionary<int, ClientUser> Users { get; private set; }
        public Dictionary<int, ClientChannel> Channels { get; private set; }

        public Action<ClientState> OnClientStateChanged;
        public Action<ClientChannel> OnChannelAdded;
        public Action<ClientChannel> OnChannelUpdated;
        public Action<ClientMessage> OnMessageAdded;
        public Action<ClientUser> OnUserAdded;
        public Action<ClientUser> OnUserUpdated;
        public Action<ClientUser, ClientChannel> OnUserJoinVoice;
        public Action<ClientUser, ClientChannel> OnUserLeaveVoice;

        private NetManager netClient;
        private int clientID;

        public static Func<AudioPlayer> CreateAudioPlayer;
        public static Func<AudioRecorder> CreateAudioRecorder;

        private AudioRecorder audioRecorder;

        private string usernameToRequest;


        public Client()
        {
            Users = new Dictionary<int, ClientUser>();
            Channels = new Dictionary<int, ClientChannel>();

            State = ClientState.Unconnected;
        }



        public void Connect(string address, int port)
        {
            Console.WriteLine("Connecting");

            Connect(address, port, usernameToRequest);
        }
        public void Connect(string address, int port, string username)
        {
            Console.WriteLine("Connecting");

            netClient = new NetManager(this);
            netClient.UnconnectedMessagesEnabled = true;
            netClient.UpdateTime = 15;
            netClient.Start();

            netClient.Connect(address, port, "");

            usernameToRequest = username;

            //SendHandshake();
        }

        public void Disconnect()
        {
            netClient.FirstPeer.Disconnect();
            State = ClientState.Unconnected;
            usernameToRequest = "";
            
            Users = new Dictionary<int, ClientUser>();
            Channels = new Dictionary<int, ClientChannel>();
        }


        public void Receive()
        {
            if (netClient != null)
                netClient.PollEvents();

            if (User != null && User.VoiceChannel != null)
                SendVoiceData();
        }


        public void SendPacket<T>(T packet) where T : IPacketStruct
        {
            PacketTransport.SendPacket(netClient.FirstPeer, packet);
        }
        public void SendEvent(PacketType packetType)
        {
            PacketTransport.SendEvent(netClient.FirstPeer, packetType);
        }


        private ClientUser AddUser(int userID, string username, bool isLocal = false)
        {
            ClientUser user = new ClientUser(userID, username);
            Users.Add(user.UserID, user);

            if (isLocal)
            {
                User = user;
                User.SetLocal(true);
            }

            OnUserAdded?.Invoke(user);
            return user;
        }
        private ClientUser GetUser(int userID)
        {
            ClientUser user;
            if (!Users.TryGetValue(userID, out user))
            {
                user = new ClientUser(userID, "");
                SendPacket(new GetUsers
                {
                    users = new Packets.Types.Array<UserData>(new UserData[] { new UserData { userID = userID } })
                });
            }

            return user;
        }

        private void JoinVoiceChannel(ClientChannel channel)
        {
            audioRecorder = CreateAudioRecorder();
        }

        #region Send Helpers

        public void SendHandshake()
        {
            Console.WriteLine("Send Handshake");

            State = ClientState.Handshaking;
            SendPacket(new Packets.Packets.Handshake
            {
                protocolVersion = ProtocolVersion,
                nextState = Packets.Packets.Handshake.NextState.Login,
            });
        }

        public void SendLogin(string name)
        {
            Console.WriteLine("Send Login");

            if (State == ClientState.Login)
            {
                SendPacket(new Login()
                {
                    user = { name = name },
                });
            }
        }

        public void SendChatMessage(ClientChannel channel, string content)
        {
            Console.WriteLine("Sending chat message to channel " + channel.ChannelID + " with content \"" + content + "\"");

            SendPacket(new ChatMessage
            {
                channelID = channel.ChannelID,
                message = new Packets.Types.Data.MessageData
                {
                    Content = content
                }
            });
        }



        public void SendJoinVoiceChannel(ClientChannel channel)
        {
            Console.WriteLine(channel.Name);
            if (User.VoiceChannel == channel)
                return;

            SendPacket(new VoiceChannelJoin
            {
                channelID = channel.ChannelID
            });
        }

        public void SendLeaveVoiceChannel()
        {
            if (User.VoiceChannel == null)
                return;

            SendPacket(new VoiceChannelLeave
            {
                channelID = User.VoiceChannel.ChannelID,
            });
        }

        private void SendVoiceData()
        {
            byte[] data;
            while (User.VoiceChannel.CreateVoicePacket(audioRecorder, out data))
            {
                SendPacket(new VoiceData
                {
                    userID = User.UserID,
                    channelID = User.VoiceChannel.ChannelID,
                    data = data
                });
            }
        }

        #endregion Send Helpers

        #region Message receive

        public void OnPeerConnected(NetPeer peer)
        {
            Console.WriteLine("Connected");

            SendHandshake();
        }

        public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            Console.WriteLine("Disconnected");
        }

        public void OnNetworkError(IPEndPoint endPoint, SocketError socketError)
        {
            Console.WriteLine("[CLIENT] We received error " + socketError);
        }

        public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
        {
            //Console.WriteLine("Received Data");

            PacketTransport.Receive(peer, this, reader.RawData, reader.Position);
        }

        public void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType)
        {
            throw new NotImplementedException();
        }

        public void OnNetworkLatencyUpdate(NetPeer peer, int latency)
        {

        }

        public void OnConnectionRequest(ConnectionRequest request) { }

        #endregion Message Receive
    }
}
