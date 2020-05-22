using System;
using System.Collections.Generic;
using System.Text;

namespace EtcordSharp.Server
{
    public class ServerMessage
    {
        private Server server;

        public ServerChannel Channel { get; private set; }
        public int MessageID { get; private set; }
        public ServerClient Sender { get; private set; }
        public string Content { get; private set; }


        public ServerMessage(Server server, ServerChannel channel, int id, ServerClient sender, string content)
        {
            this.server = server;

            Channel = channel;
            MessageID = id;
            Sender = sender;
            Content = content;
        }

        public Packets.Types.Data.MessageData GetMessageData()
        {
            return new Packets.Types.Data.MessageData
            {
                MessageID = MessageID,
                SenderID = Sender.ConnectionId,
                SenderName = Sender.Username,
                Content = Content,
            };
        }
    }
}
