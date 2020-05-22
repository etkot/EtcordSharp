using System;
using System.Collections.Generic;
using System.Text;

namespace EtcordSharp.Client
{
    public class Channel
    {
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


        public Channel(int id, int parentID, string name, ChannelType type)
        {
            ChannelID = id;
            ParentID = parentID;
            Name = name;
            Type = type;
        }
    }
}
