namespace SoundFingerprinting.Audio.Bass
{
    using System.Collections.Generic;

    using SoundFingerprinting.Infrastructure;

    /// <summary>
    ///   Bass Audio Service
    /// </summary>
    /// <remarks>
    ///   BASS is an audio library for use in Windows and Mac OSX software. 
    ///   Its purpose is to provide developers with powerful and efficient sample, stream (MP3, MP2, MP1, OGG, WAV, AIFF, custom generated, and more via add-ons), 
    ///   MOD music (XM, IT, S3M, MOD, MTM, UMX), MO3 music (MP3/OGG compressed MODs), and recording functions. 
    /// </remarks>
    public class BassAudioService : AudioService
    {
        private static readonly IReadOnlyCollection<string> BaasSupportedFormats = new[] { ".wav", ".mp3", ".ogg", ".flac" };

        private readonly IBassServiceProxy proxy;
        private readonly IBassStreamFactory streamFactory;
        private readonly IBassResampler resampler;
        private readonly int resampleQuality;

        /// <summary>
        ///  Initializes a new instance of the <see cref="BassAudioService"/> class. 
        /// </summary>
        /// <param name="resamplerQuality">
        /// Resampler quality  0 (or below) = 4 points, 1 = 8 points, 2 = 16 points, 3 = 32 points, 4 = 64 points, 5 = 128 points, 6 (or above) = 256 points
        /// </param>
        public BassAudioService(int resamplerQuality = 4)
            : this(
                resamplerQuality, 
                DependencyResolver.Current.Get<IBassServiceProxy>(),
                DependencyResolver.Current.Get<IBassStreamFactory>(),
                DependencyResolver.Current.Get<IBassResampler>())
        {
        }

        internal BassAudioService(int resampleQuality, IBassServiceProxy proxy, IBassStreamFactory streamFactory, IBassResampler resampler)
        {
            this.proxy = proxy;
            this.streamFactory = streamFactory;
            this.resampler = resampler;
            this.resampleQuality = resampleQuality;
        }

        public override IReadOnlyCollection<string> SupportedFormats
        {
            get
            {
                return BaasSupportedFormats;
            }
        }

        public override AudioSamples ReadMonoSamplesFromFile(string pathToSourceFile, int sampleRate, double seconds, double startAt)
        {
            int stream = streamFactory.CreateStream(pathToSourceFile);
            float[] samples = resampler.Resample(stream, sampleRate, seconds, startAt, resampleQuality, mixerStream => new BassSamplesProvider(proxy, mixerStream));
            return new AudioSamples(samples, pathToSourceFile, sampleRate);
        }
    }
}
