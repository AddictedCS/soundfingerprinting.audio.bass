namespace SoundFingerprinting.Audio.Bass
{
    using System;

    using Un4seen.Bass;
    using Un4seen.Bass.AddOn.Fx;

    internal class BassResampler : IBassResampler
    {
        private readonly IBassServiceProxy proxy;

        private readonly IBassStreamFactory streamFactory;

        private readonly ISamplesAggregator samplesAggregator;

        public BassResampler(IBassServiceProxy proxy, IBassStreamFactory streamFactory, ISamplesAggregator samplesAggregator)
        {
            this.proxy = proxy;
            this.streamFactory = streamFactory;
            this.samplesAggregator = samplesAggregator;
        }

        public float[] Resample(int sourceStream, int sampleRate, double seconds, double startAt, Func<int, ISamplesProvider> getSamplesProvider)
        {
            int mixerStream = 0;
            try
            {
                SeekToSecondInCaseIfRequired(sourceStream, startAt);

                mixerStream = streamFactory.CreateMixerStream(sampleRate);
               // int streamFX = BassFx.BASS_FX_TempoCreate(sourceStream, BASSFlag.BASS_FX_FREESOURCE | BASSFlag.BASS_STREAM_DECODE);
               // int fxLowFilter = Bass.BASS_ChannelSetFX(streamFX, BASSFXType.BASS_FX_BFX_BQF, 1);
               // BASS_BFX_BQF lowFilter = new BASS_BFX_BQF { lFilter = BASSBFXBQF.BASS_BFX_BQF_LOWPASS, fCenter = 5512, fBandwidth = 6 };
               // Bass.BASS_FXSetParameters(fxLowFilter, lowFilter);
                proxy.ChannelSetAttribute(mixerStream, BASSAttribute.BASS_ATTRIB_SRC, 4);
                CombineStreams(mixerStream, sourceStream);
                float[] samples = samplesAggregator.ReadSamplesFromSource(getSamplesProvider(mixerStream), seconds, sampleRate);
                return samples;
            }
            finally
            {
                ReleaseStream(mixerStream);
                ReleaseStream(sourceStream);
            }
        }

        private void SeekToSecondInCaseIfRequired(int stream, double startAtSecond)
        {
            if (startAtSecond > 0)
            {
                if (!proxy.ChannelSetPosition(stream, startAtSecond))
                {
                    throw new BassException(proxy.GetLastError());
                }
            }
        }

        private void CombineStreams(int mixerStream, int stream)
        {
            if (!proxy.CombineMixerStreams(mixerStream, stream, BASSFlag.BASS_SAMPLE_FLOAT))
            {
                throw new BassException(proxy.GetLastError());
            }
        }

        private void ReleaseStream(int stream)
        {
            if (stream != 0)
            {
                proxy.FreeStream(stream);
            }
        }
    }
}
