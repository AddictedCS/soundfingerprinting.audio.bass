namespace SoundFingerprinting.Audio.Bass
{
    public class BassStreamingUrlReader : IStreamingUrlReader
    {
        private readonly IBassServiceProxy proxy;
        private readonly IBassStreamFactory streamFactory;
        private readonly IBassResampler bassResampler;

        public BassStreamingUrlReader()
            : this(
                BassServiceProxy.Instance,
                new BassStreamFactory(BassServiceProxy.Instance),
                new BassResampler(
                    BassServiceProxy.Instance,
                    new BassStreamFactory(BassServiceProxy.Instance),
                    new SamplesAggregator()))
        {
        }

        internal BassStreamingUrlReader(IBassServiceProxy proxy, IBassStreamFactory streamFactory, IBassResampler bassResampler)
        {
            this.proxy = proxy;
            this.streamFactory = streamFactory;
            this.bassResampler = bassResampler;
        }

        public float[] ReadMonoSamples(string url, int sampleRate, int secondsToRead)
        {
            const int DefaultResamplerQuality = 4;
            int stream = streamFactory.CreateStreamFromStreamingUrl(url);
            return bassResampler.Resample(stream, sampleRate, secondsToRead, 0, DefaultResamplerQuality, mixerStream => new ContinuousStreamSamplesProvider(new BassSamplesProvider(proxy, mixerStream)));
        }
    }
}