using System;
using System.Collections.Generic;
using System.Text;

namespace EtcordSharp.Client.WebView.JSON
{
    public class JSChannel : JSObject
    {
        public override string name => "channel";

        public enum ChannelType
        {
            None = 0,
            TextChat = 1,
            VoiceChat = 2,
            Both = 3,
        }

        public int ChannelID { get; private set; }
        public int ParentID { get; private set; }
        public string Name { get; private set; }
        public ChannelType Type { get; private set; }


        public static explicit operator JSChannel(ClientChannel clientChannel)
        {
            JSChannel jsChannel = new JSChannel();
            jsChannel.ChannelID = clientChannel.ChannelID;
            jsChannel.ParentID = clientChannel.Parent?.ChannelID ?? 0;
            jsChannel.Name = clientChannel.Name;
            jsChannel.Type = (ChannelType)clientChannel.Type;

            return jsChannel;
        }
    }
}
