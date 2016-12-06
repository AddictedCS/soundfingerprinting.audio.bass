namespace SoundFingerprinting.Audio.Bass.Tests
{
    using System.IO;

    using NUnit.Framework;

    public abstract class AbstractTest
    {
        protected const double Epsilon = 0.0001;

        protected const int BitsPerSample = 32;

        protected const int SampleRate = 5512;

        protected const int SamplesPerFingerprint = 128 * 64;

        protected const int WaveHeader = 58;

        protected readonly string PathToMp3 = Path.Combine(TestContext.CurrentContext.TestDirectory, "Chopin.mp3");

        protected readonly string PathToSamples = Path.Combine(TestContext.CurrentContext.TestDirectory, "chopinsamples.bin");

        protected const int SamplesToRead = 128 * 64;

        protected const int MinYear = 1501;
    }
}
