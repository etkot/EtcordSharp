using Concentus.Enums;
using Concentus.Structs;
using System;
using System.Collections.Generic;
using System.Text;

namespace EtcordSharp.Client.Audio.Codecs
{
    public class Opus : Codec
    {
        private OpusEncoder encoder;
        private OpusDecoder decoder;


        public Opus()
        {
            decoder = OpusDecoder.Create(48000, 1);

            encoder = OpusEncoder.Create(48000, 1, OpusApplication.OPUS_APPLICATION_VOIP);
            encoder.Bitrate = 12000;
        }

        public override short[] Decode(byte[] data)
        {
            int frameSize = 960; // must be same as framesize used in input, you can use OpusPacketInfo.GetNumSamples() to determine this dynamically
            short[] outputBuffer = new short[frameSize];

            int thisFrameSize = decoder.Decode(data, 0, data.Length, outputBuffer, 0, frameSize, false);
            return outputBuffer;
        }

        public override byte[] Encode(short[] data)
        {
            byte[] outputBuffer = new byte[1000];
            int frameSize = 960;

            int thisPacketSize = encoder.Encode(data, 0, frameSize, outputBuffer, 0, outputBuffer.Length); // this throws OpusException on a failure, rather than returning a negative number
            
            byte[] buffer = new byte[thisPacketSize];
            Buffer.BlockCopy(outputBuffer, 0, buffer, 0, thisPacketSize);

            return buffer;
        }
    }
}
