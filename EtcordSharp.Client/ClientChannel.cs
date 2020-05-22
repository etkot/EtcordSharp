using System;
using System.Collections.Generic;
using System.Text;

namespace EtcordSharp.Client
{
    public class ClientChannel
    {
        public enum ChannelType
        {
            None = 0,
            TextChat = 1,
            VoiceChat = 2,
            Both = 3,
        }

        private Client client;

        public int ChannelID { get; private set; }
        public ClientChannel Parent { get; private set; }
        public string Name { get; private set; }
        public ChannelType Type { get; private set; }

        public Dictionary<int, ClientMessage> Messages { get; private set; }


        public ClientChannel(Client client, int id, ClientChannel parent, string name, ChannelType type)
        {
            this.client = client;

            ChannelID = id;
            Parent = parent;
            Name = name;
            Type = type;

            Messages = new Dictionary<int, ClientMessage>();
        }

        public void ClearMessages()
        {
            Messages.Clear();
        }

        public ClientMessage AddMessage(Packets.Types.Data.MessageData message)
        {
            ClientMessage newMessage = new ClientMessage(this, message);
            Messages.Add(message.MessageID, newMessage);

            return newMessage;
        }

        public void GetChatHistory()
        {
            ClearMessages();
            client.SendPacket(Packets.PacketType.GetChatHistory, new Packets.Packets.GetChatHistory
            {
                channelID = ChannelID,
                count = 100,
                offsetID = -1,
            });
        }


        public override string ToString() => Name;
    }
}
