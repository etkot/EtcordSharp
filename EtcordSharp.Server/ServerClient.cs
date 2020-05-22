using EtcordSharp.Packets;
using EtcordSharp.Packets.Attributes;
using System;
using System.Collections.Generic;
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


        public ServerClient(Server server, int connectionId)
        {
            this.server = server;

            ConnectionId = connectionId;
            State = ClientState.Handshaking;
        }

        ~ServerClient()
        {

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
            Username = login.Username.Value;
            if (Username.Length > UsernameCharacterLimit)
                Username = Username.Substring(0, UsernameCharacterLimit);

            State = ClientState.Connected;

            Console.WriteLine("Login \"" + Username + "\"");

            return new Packets.Packets.Login
            {
                Username = Username,
                ClientID = ConnectionId,
            };
        }

        /*
        public void GetClients(DataReader obj)
        {
            throw new NotImplementedException();
        }
        */

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

        /*
        public void VoiceChannelJoin(DataReader obj)
        {
            throw new NotImplementedException();
        }

        public void VoiceChannelLeave(DataReader obj)
        {
            throw new NotImplementedException();
        }
        */

        #endregion Packet receivers
    }
}
