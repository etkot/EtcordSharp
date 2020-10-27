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
            
            User = new ClientUser(login.user);
            User.SetLocal(true);
            Users.Add(User.UserID, User);

            State = ClientState.Connected;

            // Request channels
            SendEvent(PacketType.GetChannels);
            Console.WriteLine("Sent GetChannels");
        }



        [PacketReceiver(PacketType.GetClients)]
        public void ReceiveGetClients(GetUsers getClients)
        {
            Console.WriteLine("GetClients");

            for (int i = 0; i < getClients.users.Length; i++)
            {
                Packets.Types.Data.UserData data = getClients.users[i];

                ClientUser user;
                if (Users.TryGetValue(data.userID, out user))
                {
                    user.SetName(data.name);

                    OnUserUpdated?.Invoke(user);
                }
                else
                {
                    user = new ClientUser(data);
                    Users.Add(user.UserID, user);

                    OnUserAdded?.Invoke(user);
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

                ClientUser user;
                if (!Users.TryGetValue(voiceChannelJoin.userID, out user))
                {
                    SendPacket(PacketType.GetClients, new GetUsers(new UserData { userID = voiceChannelJoin.userID }));
                    user = new ClientUser(voiceChannelJoin.userID, "unknown");

                    foreach (KeyValuePair<int, ClientUser> userrrr in Users)
                    {
                        Console.WriteLine(userrrr.Value.UserID + "  " + voiceChannelJoin.userID);
                    }
                }

                user.SetVoiceChannel(channel);
                channel.VoiceUsers.Add(user.UserID, user);
                OnUserJoinVoice?.Invoke(user, channel);
            }
            else
            {
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
                ClientUser user;
                if (!Users.TryGetValue(voiceChannelLeave.userID, out user))
                {
                    SendPacket(PacketType.GetClients, new GetUsers(new UserData { userID = voiceChannelLeave.userID }));
                    user = new ClientUser(voiceChannelLeave.userID, "unknown");
                }
                else
                {
                    channel.VoiceUsers.Remove(user.UserID);
                }

                user.SetVoiceChannel(null);
                OnUserLeaveVoice?.Invoke(user, channel);
            }
            else
            {
                // TODO: GetChannels

                Console.WriteLine("Channel not found " + voiceChannelLeave.channelID);
            }
        }
    }
}
