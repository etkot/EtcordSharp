using EtcordSharp.Client.Audio;
using EtcordSharp.Client.Audio.Codecs;
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
        public Dictionary<int, ClientUser> VoiceUsers { get; private set; }


        public ClientChannel(Client client, int id, ClientChannel parent, string name, ChannelType type)
        {
            this.client = client;

            ChannelID = id;
            Parent = parent;
            Name = name;
            Type = type;

            Messages = new Dictionary<int, ClientMessage>();
            VoiceUsers = new Dictionary<int, ClientUser>();
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
            client.SendPacket(new Packets.Packets.GetChatHistory
            {
                channelID = ChannelID,
                count = 100,
                offsetID = -1,
            });
        }

        public void ReceiveVoiceData(ClientUser user, byte[] data)
        {
            short[] voiceData = user.VoiceCodec.Decode(data);
            user.audioPlayer.Play(voiceData, 0, voiceData.Length);
        }

        /// <summary>
        /// Gets voice data from the given AudioRecorder and compresses it with this channels codec
        /// </summary>
        /// <param name="recorder">The audio source</param>
        /// <param name="data">Compressed voice data</param>
        /// <returns>Whether there is more data to send</returns>
        public bool CreateVoicePacket(AudioRecorder recorder, out byte[] data)
        {
            // TODO: Don't make a new array every time
            short[] voiceData = new short[client.User.VoiceCodec.FrameSize];
            bool more = recorder.GetExact(voiceData, 0, voiceData.Length);

            if (!more)
            {
                data = null;
                return false;
            }

            data = client.User.VoiceCodec.Encode(voiceData);
            return more;
        }


        public override string ToString() => Name;
    }
}
