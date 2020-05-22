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
            this.ConnectionId = connectionId;
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
        public Packets.Structs.Handshake? Handshake(Packets.Structs.Handshake handshake)
        {
            Console.WriteLine("Handshake");
            
            if (handshake.protocolVersion == Server.ProtocolVersion)
            {
                State = ClientState.Login;

                return new Packets.Structs.Handshake()
                {
                    protocolVersion = Server.ProtocolVersion,
                    nextState = Packets.Structs.Handshake.NextState.Login,
                };
            }
            else
            {
                SendPacket(PacketType.Handshake, new Packets.Structs.Handshake()
                {
                    protocolVersion = Server.ProtocolVersion,
                    nextState = Packets.Structs.Handshake.NextState.None,
                });

                Disconnect();

                return null;
            }
        }
        
        [PacketReceiver(PacketType.Login)]
        public Packets.Structs.Login Login(Packets.Structs.Login login)
        {
            Username = login.Username.Value;
            if (Username.Length > UsernameCharacterLimit)
                Username = Username.Substring(0, UsernameCharacterLimit);

            State = ClientState.Connected;

            Console.WriteLine("Login \"" + Username + "\"");

            return new Packets.Structs.Login()
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
        public Packets.Structs.GetChannels GetChannels()
        {
            Packets.Structs.GetChannels getChannels = new Packets.Structs.GetChannels(server.Channels.Count);

            int i = 0;
            foreach (KeyValuePair<int, ServerChannel> channel in server.Channels)
            {
                Packets.Types.Data.ChannelData channelData = new Packets.Types.Data.ChannelData()
                {
                    ChannelID = channel.Value.ChannelID,
                    ParentID = channel.Value.ParentID,
                    Name = channel.Value.Name,
                    type = (Packets.Types.Data.ChannelData.ChannelType)channel.Value.Type,
                };

                getChannels.channels[i] = channelData;
                i++;
            }

            return getChannels;
        }

        /*
        public void GetChatHistory(DataReader obj)
        {
            throw new NotImplementedException();
        }

        public void ChatMessage(DataReader obj)
        {
            throw new NotImplementedException();
        }

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
