using System;
using System.Collections.Generic;
using System.Text;

namespace EtcordSharp.Client.WebView.JSON
{
    public class JSUser : JSObject
    {
        public override string name => "user";

        public int UserID { get; private set; }
        public string Name { get; private set; }

        public bool IsLocal { get; private set; }

        public int VoiceChannelID { get; private set; }


        public static explicit operator JSUser(ClientUser clientUser)
        {
            JSUser jsUser = new JSUser();
            jsUser.UserID = clientUser.UserID;
            jsUser.Name = clientUser.Name;
            jsUser.IsLocal = clientUser.IsLocal;
            jsUser.VoiceChannelID = clientUser.VoiceChannel?.ChannelID ?? 0;

            return jsUser;
        }
    }
}
