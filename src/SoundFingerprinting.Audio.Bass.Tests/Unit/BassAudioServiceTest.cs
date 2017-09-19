namespace SoundFingerprinting.Audio.Bass.Tests.Unit
{
    using System;

    using Moq;

    using NUnit.Framework;

    using SoundFingerprinting.Audio;
    using SoundFingerprinting.Audio.Bass;

    [TestFixture]
    public class BassAudioServiceTest : AbstractTest
    {
        private IAudioService audioService;
       
        private Mock<IBassServiceProxy> proxy;
        private Mock<IBassStreamFactory> streamFactory;
        private Mock<IBassResampler> resampler;

        [SetUp]
        public void SetUp()
        {
            proxy = new Mock<IBassServiceProxy>(MockBehavior.Strict);
            streamFactory = new Mock<IBassStreamFactory>(MockBehavior.Strict);
            resampler = new Mock<IBassResampler>(MockBehavior.Strict);

            audioService = new BassAudioService(5, proxy.Object, streamFactory.Object, resampler.Object);
        }

        [TearDown]
        public void TearDown()
        {
            proxy.VerifyAll();
            streamFactory.VerifyAll();
            resampler.VerifyAll();
        }

        [Test]
        public void TestReadMonoFromFile()
        {
            const int StreamId = 100;
            float[] samplesToReturn = new float[1024];
            streamFactory.Setup(f => f.CreateStream("path-to-file")).Returns(StreamId);
            resampler.Setup(r => r.Resample(StreamId, SampleRate, 0, 0, It.IsAny<int>(), It.IsAny<Func<int, ISamplesProvider>>()))
                .Returns(samplesToReturn);

            var samples = audioService.ReadMonoSamplesFromFile("path-to-file", SampleRate);

            Assert.AreSame(samplesToReturn, samples.Samples);
        }

        [Test]
        public void TestReadMonoFromFileFromSpecificSecond()
        {
            const int StreamId = 100;
            float[] samplesToReturn = new float[1024];
            streamFactory.Setup(f => f.CreateStream("path-to-file")).Returns(StreamId);
            resampler.Setup(r => r.Resample(StreamId, SampleRate, 10, 20, It.IsAny<int>(), It.IsAny<Func<int, ISamplesProvider>>())).Returns(samplesToReturn);

            var samples = audioService.ReadMonoSamplesFromFile("path-to-file", SampleRate, 10, 20);

            Assert.AreSame(samplesToReturn, samples.Samples);
        }
    }
}
