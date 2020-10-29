using EtcordSharp.Client.Audio;
using EtcordSharp.Client.Audio.Codecs;
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

        public ClientChannel VoiceChannel { get; private set; }

        public Codec VoiceCodec { get; private set; }
        public AudioPlayer audioPlayer { get; private set; }


        public ClientUser(int userID, string name)
        {
            UserID = userID;
            Name = name;

            VoiceCodec = new Opus();
            audioPlayer = Client.CreateAudioPlayer();
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
            VoiceChannel = channel;
        }
    }
}
