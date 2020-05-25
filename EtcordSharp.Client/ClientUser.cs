using System;
using System.Collections.Generic;
using System.Text;

namespace EtcordSharp.Client
{
    public class ClientUser
    {
        public int UserID { get; private set; }
        public string Name { get; private set; }

        public bool IsLocal { get; private set; }

        public ClientChannel voiceChannel { get; private set; }


        public ClientUser(Packets.Types.Data.UserData data)
        {
            UserID = data.userID;
            Name = data.name;
        }
        public ClientUser(int userID, string name)
        {
            UserID = userID;
            Name = name;
        }

        public void SetLocal(bool isLocal)
        {
            IsLocal = isLocal;
        }
        public void SetName(string name)
        {
            Name = name;
        }
        public void SetVoiceChannel(ClientChannel channel)
        {
            voiceChannel = channel;
        }
    }
}
