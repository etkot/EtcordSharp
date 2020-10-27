using EtcordSharp.Packets;
using EtcordSharp.Packets.Packets;
using System;
using System.Collections.Generic;
using System.Text;

namespace EtcordSharp.Server
{
    public class ServerChannel
    {
        public enum ChannelType
        {
            None = 0, // or category
            TextChat = 1,
            VoiceChat = 2,
            Both = 3,
        }

        private Server server;

        public int ChannelID { get; private set; }
        public int ParentID { get; private set; }
        public string Name { get; private set; }
        public ChannelType Type { get; private set; }

        public Dictionary<int, ServerMessage> Messages { get; private set; }
        private int lastMessageID = 0;

        public List<ServerClient> VoiceClients { get; private set; }


        public ServerChannel(Server server, int id, int parentID, string name, ChannelType type)
        {
            this.server = server;

            ChannelID = id;
            ParentID = parentID;
            Name = name;
            Type = type;

            Messages = new Dictionary<int, ServerMessage>();
            VoiceClients = new List<ServerClient>();
        }

        public Packets.Types.Data.ChannelData GetChannelData()
        {
            return new Packets.Types.Data.ChannelData
            {
                ChannelID = ChannelID,
                ParentID = ParentID,
                Name = Name,
                type = (Packets.Types.Data.ChannelData.ChannelType)Type,
            };
        }

        public void SendMessage(ServerClient sender, string content)
        {
            ServerMessage message = new ServerMessage(server, this, ++lastMessageID, sender, content);
            Messages.Add(message.MessageID, message);

            server.SendToAuthenticated(new Packets.Packets.ChatMessage
            {
                channelID = ChannelID,
                message = message.GetMessageData(),
            });
        }

        public bool JoinVoice(ServerClient client)
        {
            if (Type == ChannelType.VoiceChat || Type == ChannelType.Both)
            {
                VoiceClients.Add(client);

                server.SendToAuthenticated(new Packets.Packets.VoiceChannelJoin
                {
                    channelID = ChannelID,
                    userID = client.ConnectionId,
                });

                return true;
            }

            return false;
        }
        public bool LeaveVoice(ServerClient client)
        {
            if (VoiceClients.Contains(client))
            {
                server.SendToAuthenticated(new Packets.Packets.VoiceChannelLeave
                {
                    channelID = ChannelID,
                    userID = client.ConnectionId,
                    reason = Packets.Packets.VoiceChannelLeave.LeaveReason.Left,
                });
            }

            return false;
        }
        public void SendVoice(ServerClient sender, VoiceData voiceData)
        {
            if (Type != ChannelType.VoiceChat && Type != ChannelType.Both)
                return;

            foreach (ServerClient client in VoiceClients)
            {
                if (client != sender)
                {
                    client.SendPacket(voiceData);
                }
            }
        }
    }
}
