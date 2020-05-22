using EtcordSharp.Packets;
using EtcordSharp.Packets.Attributes;
using EtcordSharp.Packets.Packets;
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

            if (handshake.nextState == Packets.Packets.Handshake.NextState.Login && Username != "")
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

                ClientChannel channel = new ClientChannel(this, data.ChannelID, data.ParentID, data.Name, (ClientChannel.ChannelType)data.type);
                Channels.Add(data.ChannelID, channel);

                OnChannelAdded?.Invoke(channel);
            }
        }



        [PacketReceiver(PacketType.GetChatHistory)]
        public void GetChatHistory(GetChatHistory getChatHistory)
        {
            Console.WriteLine("GetChatHistory");

            ClientChannel channel;
            if (Channels.TryGetValue(getChatHistory.channelID, out channel))
            {
                foreach (Packets.Types.Data.MessageData message in getChatHistory.messages)
                {
                    ClientMessage newMessage = channel.AddMessage(message);
                    OnMessageAdded?.Invoke(newMessage);
                }
            }
            else
            {
                Console.WriteLine("Warning: Server sent chat history for a channel that doesn't exist");
            }
        }

        [PacketReceiver(PacketType.ChatMessage)]
        public void ChatMessage(ChatMessage chatMessage)
        {
            Console.WriteLine("ChatMessage");

            ClientChannel channel;
            if (Channels.TryGetValue(chatMessage.channelID, out channel))
            {
                ClientMessage newMessage = channel.AddMessage(chatMessage.message);
                OnMessageAdded?.Invoke(newMessage);
            }
            else
            {
                Console.WriteLine("Warning: Server sent chat message to a channel that doesn't exist");
            }
        }
    }
}
