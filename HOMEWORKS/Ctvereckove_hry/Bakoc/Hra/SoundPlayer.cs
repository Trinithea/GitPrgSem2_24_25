using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hra
{
    public class SoundPlayer
    {
        private byte[] soundData;
        private WaveFormat waveFormat;

        public SoundPlayer(string filePath)
        {
            LoadSound(filePath);
        }

        private void LoadSound(string filePath)
        {
            using (var reader = new AudioFileReader(filePath))
            using (var ms = new MemoryStream())
            {
                waveFormat = reader.WaveFormat;
                reader.CopyTo(ms);
                soundData = ms.ToArray();
            }
        }

        public void PlaySound()
        {
            Task.Run(() =>
            {
                var memoryStream = new MemoryStream(soundData);
                var waveProvider = new RawSourceWaveStream(memoryStream, waveFormat);
                var outputDevice = new WaveOutEvent();

                outputDevice.Init(waveProvider);
                outputDevice.Play();

                outputDevice.PlaybackStopped += (s, e) =>
                {
                    outputDevice.Dispose();
                    waveProvider.Dispose();
                    memoryStream.Dispose();
                };
            });
        }
    }
}
