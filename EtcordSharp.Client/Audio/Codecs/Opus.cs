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

        public override int FrameSize => 960;

        public override short[] Decode(byte[] data)
        {
            // framesize must be same as framesize used in input, you can use OpusPacketInfo.GetNumSamples() to determine this dynamically
            short[] outputBuffer = new short[FrameSize];

            int thisFrameSize = decoder.Decode(data, 0, data.Length, outputBuffer, 0, FrameSize, false);
            return outputBuffer;
        }

        public override byte[] Encode(short[] data)
        {
            byte[] outputBuffer = new byte[1000];

            int thisPacketSize = encoder.Encode(data, 0, FrameSize, outputBuffer, 0, outputBuffer.Length); // this throws OpusException on a failure, rather than returning a negative number
            
            byte[] buffer = new byte[thisPacketSize];
            Buffer.BlockCopy(outputBuffer, 0, buffer, 0, thisPacketSize);

            return buffer;
        }
    }
}
