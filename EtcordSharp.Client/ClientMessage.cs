using System;
using System.Collections.Generic;
using System.Text;

namespace EtcordSharp.Client
{
    public class ClientMessage
    {
        public ClientChannel Channel { get; private set; }
        public int MessageID;
        public int SenderID;
        public string SenderName;
        public string Content;


        public ClientMessage(ClientChannel channel, Packets.Types.Data.MessageData message)
        {
            Channel = channel;
            MessageID = message.MessageID;
            SenderID = message.SenderID;
            SenderName = message.SenderName;
            Content = message.Content;
        }
    }
}
