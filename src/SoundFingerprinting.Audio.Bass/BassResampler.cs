﻿namespace SoundFingerprinting.Audio.Bass
{
    using System;
    using ManagedBass;

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

        public float[] Resample(
            int sourceStream,
            int sampleRate,
            double seconds, 
            double startAt, 
            int resampleQuality,
            Func<int, ISamplesProvider> getSamplesProvider)
        {
            int mixerStream = 0;
            try
            {
                SeekToSecondInCaseIfRequired(sourceStream, startAt);

                mixerStream = streamFactory.CreateMixerStream(sampleRate);
                proxy.ChannelSetAttribute(sourceStream, ChannelAttribute.SampleRateConversion, resampleQuality);
                CombineStreams(mixerStream, sourceStream);
                return samplesAggregator.ReadSamplesFromSource(getSamplesProvider(mixerStream), seconds, sampleRate);
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
            if (!proxy.CombineMixerStreams(mixerStream, stream, BassFlags.Float))
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
