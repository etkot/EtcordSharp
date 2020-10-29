using EtcordSharp.Client.Audio;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtcordSharp.Client.Windows
{
    public class WindowsAudioRecorder : AudioRecorder
    {
        WaveIn audioInput;
        BufferedWaveProvider bwp;

        public WindowsAudioRecorder()
        {
            audioInput = new WaveIn();
            audioInput.WaveFormat = new WaveFormat(48000, 1);
            audioInput.DataAvailable += AudioInput_DataAvailable;

            bwp = new BufferedWaveProvider(new WaveFormat(48000, 1));
            bwp.DiscardOnBufferOverflow = true;

            audioInput.StartRecording();
        }

        WindowsAudioPlayer player;

        private void AudioInput_DataAvailable(object sender, WaveInEventArgs e)
        {
            bwp.AddSamples(e.Buffer, 0, e.BytesRecorded);
        }

        public override bool GetExact(short[] data, int offset, int length)
        {
            if (bwp.BufferedBytes < length * 2)
                return false;

            byte[] byteData = new byte[length * 2];
            int count = bwp.Read(byteData, 0, length * 2);

            Buffer.BlockCopy(byteData, 0, data, offset, count);
            return true;
        }
    }
}