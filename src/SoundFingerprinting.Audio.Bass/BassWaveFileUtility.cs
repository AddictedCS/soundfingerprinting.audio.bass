namespace SoundFingerprinting.Audio.Bass
{
    using System.Collections.Generic;
    using System.IO;
    using ManagedBass;
    using WaveFormat = ManagedBass.WaveFormat;

    public class BassWaveFileUtility : IWaveFileUtility
    {
        public void WriteSamplesToFile(float[] samples, int sampleRate, string destination)
        {
            var waveFormat = WaveFormat.CreateIeeeFloat(sampleRate, BassConstants.NumberOfChannels);

            using (var stream = new FileStream(destination, FileMode.Create))
            using (var waveWriter = new WaveFileWriter(stream, waveFormat))
            {
                waveWriter.Write(samples, samples.Length * sizeof(float));
            }
        }

        public void WriteSamplesToFile(short[] samples, int sampleRate, string destination)
        {
            var waveFormat = new WaveFormat(sampleRate, BassConstants.NumberOfChannels);
            using (var stream = new FileStream(destination, FileMode.Create))
            using (var waveWriter = new WaveFileWriter(stream, waveFormat))
            {
                waveWriter.Write(samples, samples.Length * sizeof(short));
            }
        }

        public static short[] Convert(IReadOnlyList<float> samples)
        {
            short[] result = new short[samples.Count];
            for (int i = 0; i < samples.Count; ++i)
            {
                int value = (int)(samples[i] * short.MaxValue);
                if (value > short.MaxValue)
                    value = short.MaxValue;
                else if (value < short.MinValue)
                    value = short.MinValue;

                result[i] = (short)value;
            }

            return result;
        }
    }
}