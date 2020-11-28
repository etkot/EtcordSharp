using System;
using System.Collections.Generic;
using System.Text;

namespace EtcordSharp.Client.WebView.JSON
{
    public class JSMessage : JSObject
    {
        public override string name => "message";

        public int ChannelID { get; private set; }
        public int MessageID { get; private set; }
        public int SenderID { get; private set; }
        public string SenderName { get; private set; }
        public string Content { get; private set; }


        public static explicit operator JSMessage(ClientMessage clientMessage)
        {
            JSMessage jsMessage = new JSMessage();
            jsMessage.ChannelID = clientMessage.Channel.ChannelID;
            jsMessage.MessageID = clientMessage.MessageID;
            jsMessage.SenderID = clientMessage.SenderID;
            jsMessage.Content = clientMessage.Content;

            return jsMessage;
        }
    }
}
