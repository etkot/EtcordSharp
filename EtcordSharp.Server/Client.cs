using EtcordSharp.Packets;
using EtcordSharp.Packets.Attributes;
using EtcordSharp.Packets.Structs;
using System;
using System.Collections.Generic;
using System.Text;

namespace EtcordSharp
{
    public class Client
    {
        public enum State
        {
            Handshaking,
            Login,
            Connected,
        }

        private Server server;

        public int connectionId { get; private set; }
        public State state { get; private set; }


        public Client(Server server, int connectionId)
        {
            this.server = server;
            this.connectionId = connectionId;
            state = State.Handshaking;
        }

        ~Client()
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
        public Handshake? Handshake(Handshake handshake)
        {
            Console.WriteLine("Handshake");

            Console.WriteLine(handshake.protocolVersion);
            Console.WriteLine(handshake.nextState);

            var nextState = Packets.Structs.Handshake.NextState.None;
            if (handshake.protocolVersion == Server.ProtocolVersion)
            {
                nextState = Packets.Structs.Handshake.NextState.Login;

                return new Handshake()
                {
                    protocolVersion = Server.ProtocolVersion,
                    nextState = nextState,
                };
            }
            else
            {
                SendPacket(PacketType.Handshake, new Handshake()
                {
                    protocolVersion = Server.ProtocolVersion,
                    nextState = nextState,
                });

                Disconnect();

                return null;
            }
        }

        /*
        private void Login(DataReader obj)
        {
            throw new NotImplementedException();
        }

        private void GetClients(DataReader obj)
        {
            throw new NotImplementedException();
        }

        private void GetChannels(DataReader obj)
        {
            throw new NotImplementedException();
        }

        private void GetChatHistory(DataReader obj)
        {
            throw new NotImplementedException();
        }

        private void ChatMessage(DataReader obj)
        {
            throw new NotImplementedException();
        }

        private void VoiceChannelJoin(DataReader obj)
        {
            throw new NotImplementedException();
        }

        private void VoiceChannelLeave(DataReader obj)
        {
            throw new NotImplementedException();
        }
        */

        #endregion Packet receivers
    }
}
