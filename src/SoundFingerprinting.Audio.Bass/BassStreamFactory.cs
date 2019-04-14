namespace SoundFingerprinting.Audio.Bass
{
    using ManagedBass;

    internal class BassStreamFactory : IBassStreamFactory
    {
        private readonly IBassServiceProxy proxy;

        public BassStreamFactory(IBassServiceProxy proxy)
        {
            this.proxy = proxy;
        }

        public int CreateStream(string pathToFile)
        {
            int stream = proxy.CreateStream(pathToFile, GetDefaultFlags());
            ThrowIfStreamIsInvalid(stream);
            return stream;
        }

        public int CreateMixerStream(int sampleRate)
        {
            int mixerStream = proxy.CreateMixerStream(sampleRate, BassConstants.NumberOfChannels, GetDefaultFlags());
            ThrowIfStreamIsInvalid(mixerStream);
            return mixerStream;
        }

        public int CreateStreamFromStreamingUrl(string streamingUrl)
        {
            int stream = proxy.CreateStreamFromUrl(streamingUrl, GetDefaultFlags());
            ThrowIfStreamIsInvalid(stream);
            return stream;
        }

        public int CreateStreamFromMicrophone(int sampleRate)
        {
            int stream = proxy.StartRecording(sampleRate, BassConstants.NumberOfChannels, BassFlags.Mono | BassFlags.Float);
            ThrowIfStreamIsInvalid(stream);
            return stream;
        }

        private BassFlags GetDefaultFlags()
        {
            return BassFlags.Decode | BassFlags.Mono | BassFlags.Float;
        }

        private bool IsStreamValid(int stream)
        {
            return stream != 0;
        }

        private void ThrowIfStreamIsInvalid(int stream)
        {
            if (!IsStreamValid(stream))
            {
                throw new BassException(proxy.GetLastError());
            }
        }
    }
}
