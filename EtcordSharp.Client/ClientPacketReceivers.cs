using EtcordSharp.Packets;
using EtcordSharp.Packets.Attributes;
using EtcordSharp.Packets.Structs;
using System;
using System.Collections.Generic;
using System.Text;

namespace EtcordSharp.Client
{
    public partial class Client
    {
        [PacketReceiver(PacketType.Handshake)]
        public void Handshake(Handshake handshake)
        {
            Console.WriteLine("Handshake");

            State = ClientState.Login;

            if (handshake.nextState == Packets.Structs.Handshake.NextState.Login && Username != "")
            {
                SetUsername(Username);
            }
        }

        [PacketReceiver(PacketType.Login)]
        public void Login(Login login)
        {
            Console.WriteLine("Login");

            Username = login.Username;
            clientID = login.ClientID;

            State = ClientState.Connected;

            // Request channels
            SendEvent(PacketType.GetChannels);
        }



        [PacketReceiver(PacketType.GetChannels)]
        public void GetChannels(GetChannels getChannels)
        {
            Console.WriteLine("GetChannels");

            for (int i = 0; i < getChannels.channels.Length; i++)
            {
                Packets.Types.Data.ChannelData data = getChannels.channels[i];

                Channel channel = new Channel(data.ChannelID, data.ParentID, data.Name, (Channel.ChannelType)data.type);
                Channels.Add(data.ChannelID, channel);

                OnChannelUpdated?.Invoke(channel);
            }
        }
    }
}
