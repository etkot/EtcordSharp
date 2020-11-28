using EtcordSharp.Packets;
using EtcordSharp.Packets.Attributes;
using EtcordSharp.Packets.Packets;
using EtcordSharp.Packets.Types.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace EtcordSharp.Client
{
    public partial class Client
    {
        [PacketReceiver(PacketType.Handshake)]
        public void ReceiveHandshake(Handshake handshake)
        {
            Console.WriteLine("Received Handshake");

            State = ClientState.Login;

            if (handshake.nextState == Packets.Packets.Handshake.NextState.Login && usernameToRequest != "")
            {
                SendLogin(usernameToRequest);
            }
        }

        [PacketReceiver(PacketType.Login)]
        public void ReceiveLogin(Login login)
        {
            Console.WriteLine("Received Login");

            AddUser(login.user.userID, login.user.name, true);

            State = ClientState.Connected;

            // Request channels
            SendEvent(PacketType.GetChannels);
            Console.WriteLine("Sent GetChannels");
        }



        [PacketReceiver(PacketType.GetUsers)]
        public void ReceiveGetUsers(GetUsers getUsers)
        {
            Console.WriteLine("GetUsers");

            for (int i = 0; i < getUsers.users.Length; i++)
            {
                Packets.Types.Data.UserData data = getUsers.users[i];

                ClientUser user;
                if (Users.TryGetValue(data.userID, out user))
                {
                    user.SetName(data.name);

                    OnUserUpdated?.Invoke(user);
                    eventListener?.OnUserUpdated(user);
                }
                else
                {
                    AddUser(data.userID, data.name);
                }
            }
        }

        [PacketReceiver(PacketType.GetChannels)]
        public void ReceiveGetChannels(GetChannels getChannels)
        {
            Console.WriteLine("GetChannels");

            for (int i = 0; i < getChannels.channels.Length; i++)
            {
                Packets.Types.Data.ChannelData data = getChannels.channels[i];

                ClientChannel parentChannel;
                Channels.TryGetValue(data.ParentID, out parentChannel);

                ClientChannel channel = new ClientChannel(this, data.ChannelID, parentChannel, data.Name, (ClientChannel.ChannelType)data.type);
                Channels.Add(data.ChannelID, channel);

                OnChannelAdded?.Invoke(channel);
                eventListener?.OnChannelAdded(channel);
            }
        }



        [PacketReceiver(PacketType.GetChatHistory)]
        public void ReceiveGetChatHistory(GetChatHistory getChatHistory)
        {
            Console.WriteLine("GetChatHistory");

            ClientChannel channel;
            if (Channels.TryGetValue(getChatHistory.channelID, out channel))
            {
                foreach (Packets.Types.Data.MessageData message in getChatHistory.messages)
                {
                    ClientMessage newMessage = channel.AddMessage(message);
                    OnMessageAdded?.Invoke(newMessage);
                    eventListener?.OnMessageAdded(newMessage);
                }
            }
            else
            {
                Console.WriteLine("Warning: Server sent chat history for a channel that doesn't exist");
            }
        }

        [PacketReceiver(PacketType.ChatMessage)]
        public void ReceiveChatMessage(ChatMessage chatMessage)
        {
            Console.WriteLine("ChatMessage");

            ClientChannel channel;
            if (Channels.TryGetValue(chatMessage.channelID, out channel))
            {
                ClientMessage newMessage = channel.AddMessage(chatMessage.message);
                OnMessageAdded?.Invoke(newMessage);
                eventListener?.OnMessageAdded(newMessage);
            }
            else
            {
                Console.WriteLine("Warning: Server sent chat message to a channel that doesn't exist");
            }
        }



        [PacketReceiver(PacketType.VoiceChannelJoin)]
        public void ReceiveVoiceChannelJoin(VoiceChannelJoin voiceChannelJoin)
        {
            Console.WriteLine("VoiceChannelJoin");

            if (voiceChannelJoin.channelID == -1)
            {
                Console.WriteLine("Warning: Invalid channel");
                return;
            }
            if (voiceChannelJoin.userID == -1)
            {
                Console.WriteLine("Warning: Invalid permissions");
                return;
            }

            ClientChannel channel;
            if (Channels.TryGetValue(voiceChannelJoin.channelID, out channel))
            {
                Console.WriteLine("User count: " + Users.Count);

                ClientUser user = GetUser(voiceChannelJoin.userID);

                user.SetVoiceChannel(channel);
                channel.VoiceUsers.Add(user.UserID, user);

                if (user.IsLocal)
                    JoinVoiceChannel(channel);

                OnUserJoinVoice?.Invoke(user, channel);
                eventListener?.OnUserJoinVoice(user, channel);
            }
            else
            {
                // Client doesn't have channel cached
                // TODO: GetChannels
            }
        }

        [PacketReceiver(PacketType.VoiceChannelLeave)]
        public void ReceiveVoiceChannelLeave(VoiceChannelLeave voiceChannelLeave)
        {
            Console.WriteLine("VoiceChannelLeave");

            if (voiceChannelLeave.channelID == -1)
            {
                Console.WriteLine("Warning: Already left");
                return;
            }

            ClientChannel channel;
            if (Channels.TryGetValue(voiceChannelLeave.channelID, out channel))
            {
                ClientUser user = GetUser(voiceChannelLeave.userID);
                channel.VoiceUsers.Remove(user.UserID);

                user.SetVoiceChannel(null);
                OnUserLeaveVoice?.Invoke(user, channel);
                eventListener?.OnUserLeaveVoice(user, channel);
            }
            else
            {
                // TODO: GetChannels

                Console.WriteLine("Channel not found " + voiceChannelLeave.channelID);
            }
        }

        [PacketReceiver(PacketType.VoiceData)]
        public void ReceiveVoiceData(VoiceData voiceData)
        {
            ClientChannel channel;
            if (Channels.TryGetValue(voiceData.channelID, out channel))
            {
                ClientUser user = GetUser(voiceData.userID);
                channel.ReceiveVoiceData(user, voiceData.data);
            }
            else
            {
                // TODO: GetChannels

                Console.WriteLine("Channel not found " + voiceData.channelID);
            }
        }
    }
}
