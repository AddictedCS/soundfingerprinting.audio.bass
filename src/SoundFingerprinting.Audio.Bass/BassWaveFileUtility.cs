namespace SoundFingerprinting.Audio.Bass
{
    using System.IO;
    using ManagedBass;
    using WaveFormat = ManagedBass.WaveFormat;

    public class BassWaveFileUtility : IWaveFileUtility
    {
        private const int FloatLength = 4;

        public void WriteSamplesToFile(float[] samples, int sampleRate, string destination)
        {
            var waveFormat = WaveFormat.CreateIeeeFloat(sampleRate, BassConstants.NumberOfChannels);

            using (var stream = new FileStream(destination, FileMode.Create))
            using (var waveWriter = new WaveFileWriter(stream, waveFormat))
            {
                waveWriter.Write(samples, samples.Length * FloatLength);
            }
        }
    }
}