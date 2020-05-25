using EtcordSharp.Packets;
using EtcordSharp.Packets.Attributes;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace EtcordSharp.Server
{
    public class ServerClient
    {
        private const int UsernameCharacterLimit = 30;

        public enum ClientState
        {
            Handshaking,
            Login,
            Connected,
        }

        private Server server;

        public int ConnectionId { get; private set; }
        public ClientState State { get; private set; }

        public string Username { get; private set; }
        public ServerChannel VoiceChannel { get; private set; }
        public UdpClient VoiceClient { get; private set; }


        public ServerClient(Server server, int connectionId)
        {
            this.server = server;

            ConnectionId = connectionId;
            State = ClientState.Handshaking;
        }

        ~ServerClient()
        {

        }

        public Packets.Types.Data.UserData GetClientData()
        {
            return new Packets.Types.Data.UserData
            {
                userID = ConnectionId,
                name = Username,
            };
        }

        public void SendPacket<T>(PacketType packetType, T packet) where T : IPacketStruct
        {
            server.SendPacket(this, packetType, packet);
        }

        public void Disconnect()
        {
            server.DisconnectClient(this);
        }


        #region Packet receivers
        
        [PacketReceiver(PacketType.Handshake)]
        public Packets.Packets.Handshake? Handshake(Packets.Packets.Handshake handshake)
        {
            Console.WriteLine("Handshake");
            
            if (handshake.protocolVersion == Server.ProtocolVersion)
            {
                State = ClientState.Login;

                return new Packets.Packets.Handshake
                {
                    protocolVersion = Server.ProtocolVersion,
                    nextState = Packets.Packets.Handshake.NextState.Login,
                };
            }
            else
            {
                SendPacket(PacketType.Handshake, new Packets.Packets.Handshake
                {
                    protocolVersion = Server.ProtocolVersion,
                    nextState = Packets.Packets.Handshake.NextState.None,
                });

                Disconnect();

                return null;
            }
        }
        
        [PacketReceiver(PacketType.Login)]
        public Packets.Packets.Login Login(Packets.Packets.Login login)
        {
            Username = login.user.name;
            if (Username.Length > UsernameCharacterLimit)
                Username = Username.Substring(0, UsernameCharacterLimit);

            State = ClientState.Connected;

            Console.WriteLine("Login \"" + Username + "\"");

            return new Packets.Packets.Login
            {
                user =
                {
                    name = Username,
                    userID = ConnectionId,
                }
            };
        }
        
        [PacketReceiver(PacketType.GetClients)]
        public Packets.Packets.GetUsers GetClients (Packets.Packets.GetUsers getUsers)
        {
            List<Packets.Types.Data.UserData> userDatas = new List<Packets.Types.Data.UserData>();
            foreach (Packets.Types.Data.UserData userData in getUsers.users)
            {
                ServerClient client;
                if (server.Clients.TryGetValue(userData.userID, out client))
                {
                    userDatas.Add(client.GetClientData());
                }
            }

            return new Packets.Packets.GetUsers
            {
                users = userDatas.ToArray()
            };
        }

        [PacketReceiver(PacketType.GetChannels)]
        public Packets.Packets.GetChannels GetChannels()
        {
            Packets.Packets.GetChannels getChannels = new Packets.Packets.GetChannels(server.Channels.Count);

            int i = 0;
            foreach (KeyValuePair<int, ServerChannel> channel in server.Channels)
            {
                getChannels.channels[i++] = channel.Value.GetChannelData();
            }

            return getChannels;
        }

        [PacketReceiver(PacketType.GetChatHistory)]
        public Packets.Packets.GetChatHistory GetChatHistory(Packets.Packets.GetChatHistory getChatHistory)
        {
            ServerChannel channel;
            if (server.Channels.TryGetValue(getChatHistory.channelID, out channel))
            {
                // Channel found
                // Get messages
                List<Packets.Types.Data.MessageData> messages = new List<Packets.Types.Data.MessageData>();
                foreach (KeyValuePair<int, ServerMessage> message in channel.Messages)
                {
                    if (message.Key < getChatHistory.offsetID)
                        continue;
                    if (messages.Count >= getChatHistory.count)
                        break;

                    messages.Add(message.Value.GetMessageData());
                }

                // Send messages
                return new Packets.Packets.GetChatHistory
                {
                    channelID = channel.ChannelID,
                    count = messages.Count,
                    offsetID = getChatHistory.offsetID,
                    messages = messages.ToArray(),
                };
            }
            else
            {
                // Channel not found
                // Inform the client
                return new Packets.Packets.GetChatHistory
                {
                    channelID = getChatHistory.channelID,
                    count = -1, // No channel
                    offsetID = 0,
                    // messages = EMPTY ARRAY
                };
            }
        }

        [PacketReceiver(PacketType.ChatMessage)]
        public Packets.Packets.ChatMessage? ChatMessage(Packets.Packets.ChatMessage chatMessage)
        {
            ServerChannel channel;
            if (server.Channels.TryGetValue(chatMessage.channelID, out channel))
            {
                // Channel found
                Console.WriteLine(Username + " sent message: \"" + chatMessage.message.Content + "\"");

                channel.SendMessage(this, chatMessage.message.Content);
                return null;
            }
            else
            {
                // Channel not found
                // Inform the client
                return new Packets.Packets.ChatMessage
                {
                    channelID = -1,
                    message = chatMessage.message
                };
            }
        }
        
        [PacketReceiver(PacketType.VoiceChannelJoin)]
        public Packets.Packets.VoiceChannelJoin? VoiceChannelJoin(Packets.Packets.VoiceChannelJoin voiceChannelJoin)
        {
            ServerChannel channel;
            if (server.Channels.TryGetValue(voiceChannelJoin.channelID, out channel))
            {
                if (channel.JoinVoice(this))
                {
                    VoiceChannel = channel;
                    return null;
                }
                else
                {
                    return new Packets.Packets.VoiceChannelJoin
                    {
                        channelID = channel.ChannelID,
                        userID = -1,
                    };
                }
            }
            else
            {
                return new Packets.Packets.VoiceChannelJoin
                {
                    channelID = -1
                };
            }
        }

        [PacketReceiver(PacketType.VoiceChannelLeave)]
        public Packets.Packets.VoiceChannelLeave? VoiceChannelLeave(Packets.Packets.VoiceChannelLeave voiceChannelLeave)
        {
            if (VoiceChannel == null)
            {
                return new Packets.Packets.VoiceChannelLeave
                {
                    channelID = -1,
                };
            }

            VoiceChannel.LeaveVoice(this);
            VoiceChannel = null;
            return null;
        }

        #endregion Packet receivers
    }
}
