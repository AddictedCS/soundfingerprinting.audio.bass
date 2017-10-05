namespace SoundFingerprinting.Audio.Bass.Tests.Unit
{
    using System.Collections.Generic;

    using Moq;

    using NUnit.Framework;

    using SoundFingerprinting.Audio;
    using SoundFingerprinting.Audio.Bass;

    using Un4seen.Bass;

    [TestFixture]
    public class BassResamplerTest : AbstractTest
    {
        private BassResampler resampler;

        private Mock<IBassServiceProxy> proxy;
        private Mock<IBassStreamFactory> streamFactory;
        private Mock<ISamplesAggregator> samplesAggregator;

        [SetUp]
        public void SetUp()
        {
            proxy = new Mock<IBassServiceProxy>(MockBehavior.Strict);
            streamFactory = new Mock<IBassStreamFactory>(MockBehavior.Strict);
            samplesAggregator = new Mock<ISamplesAggregator>(MockBehavior.Strict);

            resampler = new BassResampler(proxy.Object, streamFactory.Object, samplesAggregator.Object);
        }

        [TearDown]
        public void TearDown()
        {
            proxy.VerifyAll();
            streamFactory.VerifyAll();
            samplesAggregator.VerifyAll();
        }

        [Test]
        public void TestResample()
        {
            const int SourceStream = 100;
            const int MixerStream = 101;
            const int Seconds = 50;
            const int StartAt = 0;
            const int ResamplerQuality = 5;
            float[] samplesToReturn = new float[1024];

            streamFactory.Setup(f => f.CreateMixerStream(SampleRate)).Returns(MixerStream);
            proxy.Setup(p => p.CombineMixerStreams(MixerStream, SourceStream, BASSFlag.BASS_SAMPLE_FLOAT)).Returns(true);
            proxy.Setup(p => p.FreeStream(SourceStream)).Returns(true);
            proxy.Setup(p => p.FreeStream(MixerStream)).Returns(true);
            proxy.Setup(p => p.ChannelSetAttribute(MixerStream, BASSAttribute.BASS_ATTRIB_SRC, ResamplerQuality)).Returns(true);
            samplesAggregator.Setup(s => s.ReadSamplesFromSource(It.IsAny<ISamplesProvider>(), Seconds, SampleRate))
                .Returns(samplesToReturn);
            var queue = new Queue<float[]>(new[] { samplesToReturn });
            float[] samples = resampler.Resample(SourceStream, SampleRate, Seconds, StartAt, ResamplerQuality, mixerStream => new QueueSamplesProvider(queue));

            Assert.AreEqual(samplesToReturn.Length, samples.Length);
        }

        [Test]
        public void TestCombineStreamsFailsDuringResample()
        {
            const int SourceStream = 100;
            const int MixerStream = 101;
            const int Seconds = 50;
            const int StartAt = 0;
            const int ResamplerQuality = 5;

            streamFactory.Setup(f => f.CreateMixerStream(SampleRate)).Returns(MixerStream);
            proxy.Setup(p => p.FreeStream(SourceStream)).Returns(true);
            proxy.Setup(p => p.FreeStream(MixerStream)).Returns(true);
            proxy.Setup(p => p.CombineMixerStreams(MixerStream, SourceStream, BASSFlag.BASS_SAMPLE_FLOAT)).Returns(false);
            proxy.Setup(p => p.GetLastError()).Returns("Combining streams failed");
            proxy.Setup(p => p.ChannelSetAttribute(MixerStream, BASSAttribute.BASS_ATTRIB_SRC, ResamplerQuality)).Returns(true);

            var queue = new Queue<float[]>(new[] { new float[0] });

            Assert.Throws<BassException>(
                () =>
                    resampler.Resample(
                        SourceStream,
                        SampleRate,
                        Seconds,
                        StartAt,
                        ResamplerQuality,
                        mixerStream => new QueueSamplesProvider(queue)));
        }

        [Test]
        public void TestSeekToSecondFailedBeforeResample()
        {
            const int SourceStream = 100;
            const int Seconds = 50;
            const int StartAt = 10;
            const int ResamplerQuality = 5;

            proxy.Setup(p => p.ChannelSetPosition(SourceStream, StartAt)).Returns(false);
            proxy.Setup(p => p.GetLastError()).Returns("Failed to seek to a specific second");
            proxy.Setup(p => p.FreeStream(SourceStream)).Returns(true);

            Assert.Throws<BassException>(
                () =>
                    resampler.Resample(
                        SourceStream,
                        SampleRate,
                        Seconds,
                        StartAt,
                        ResamplerQuality,
                        mixerStream => new QueueSamplesProvider(new Queue<float[]>())));
        }
    }
}
