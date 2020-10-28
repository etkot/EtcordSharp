using EtcordSharp.Client.Audio;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtcordSharp.Client.Windows
{
    public class WindowsAudioPlayer : AudioPlayer
    {
        DirectSoundOut audioOutput;
        BufferedWaveProvider bwp;

        public WindowsAudioPlayer()
        {
            audioOutput = new DirectSoundOut(50);

            bwp = new BufferedWaveProvider(new WaveFormat(48000, 1));
            bwp.DiscardOnBufferOverflow = true;

            audioOutput.Init(bwp);
            audioOutput.Play();
        }

        public override void Play(short[] data, int offset, int length)
        {
            byte[] byteArray = new byte[data.Length * 2];
            Buffer.BlockCopy(data, 0, byteArray, 0, byteArray.Length);

            bwp.AddSamples(byteArray, offset, length);
        }
    }
}
