namespace SoundFingerprinting.Audio.Bass.Tests.Unit
{
    using ManagedBass;
    using Moq;

    using NUnit.Framework;

    using SoundFingerprinting.Audio.Bass;

    using BassException = SoundFingerprinting.Audio.Bass.BassException;

    [TestFixture]
    public class BassStreamFactoryTest : AbstractTest
    {
        private IBassStreamFactory streamFactory;

        private Mock<IBassServiceProxy> proxy;

        [SetUp]
        public void SetUp()
        {
            proxy = new Mock<IBassServiceProxy>(MockBehavior.Strict);

            streamFactory = new BassStreamFactory(proxy.Object);
        }

        [TearDown]
        public void TearDown()
        {
            proxy.VerifyAll();
        }

        [Test]
        public void TestCreateStream()
        {
            const int StreamId = 100;
            proxy.Setup(p => p.CreateStream("path-to-audio-file", It.IsAny<BassFlags>())).Returns(StreamId);

            var result = streamFactory.CreateStream("path-to-audio-file");

            Assert.AreEqual(StreamId, result);
        }

        [Test]
        public void TestCreateStreamFailed()
        {
            proxy.Setup(p => p.CreateStream("path-to-audio-file", It.IsAny<BassFlags>())).Returns(0);
            proxy.Setup(p => p.GetLastError()).Returns("Failed to create stream");

            Assert.Throws<BassException>(() => streamFactory.CreateStream("path-to-audio-file"));
        }
        
        [Test]
        public void TestCreateMixerStream()
        {
            const int MixerStreamId = 100;
            proxy.Setup(p => p.CreateMixerStream(5512, BassConstants.NumberOfChannels, It.IsAny<BassFlags>())).Returns(
                MixerStreamId);

            var result = streamFactory.CreateMixerStream(5512);

            Assert.AreEqual(MixerStreamId, result);
        }

        [Test]
        public void TestCreateMixerStreamFailed()
        {
            proxy.Setup(p => p.CreateMixerStream(5512, BassConstants.NumberOfChannels, It.IsAny<BassFlags>())).Returns(0);
            proxy.Setup(p => p.GetLastError()).Returns("Failed to create mixer stream");

            Assert.Throws<BassException>(() => streamFactory.CreateMixerStream(5512));
        }

        [Test]
        public void TestCreateStreamFromUrl()
        {
            const int StreamToUrl = 100;
            proxy.Setup(p => p.CreateStreamFromUrl("url-to-streaming-resource", It.IsAny<BassFlags>())).Returns(
                StreamToUrl);

            var result = streamFactory.CreateStreamFromStreamingUrl("url-to-streaming-resource");

            Assert.AreEqual(StreamToUrl, result);
        }

        [Test]
        public void TestCreateStreamFromUrlFailed()
        {
            proxy.Setup(p => p.CreateStreamFromUrl("url-to-streaming-resource", It.IsAny<BassFlags>())).Returns(0);
            proxy.Setup(p => p.GetLastError()).Returns("Failed to create stream to url");

            Assert.Throws<BassException>(() => streamFactory.CreateStreamFromStreamingUrl("url-to-streaming-resource"));
        }

        [Test]
        public void TestCreateStreamToMicrophone()
        {
            const int StreamToMicrophone = 100;

            proxy.Setup(p => p.StartRecording(5512, BassConstants.NumberOfChannels, It.IsAny<BassFlags>())).Returns(
                StreamToMicrophone);

            var result = streamFactory.CreateStreamFromMicrophone(5512);

            Assert.AreEqual(StreamToMicrophone, result);
        }

        [Test]
        public void TestCreateStreamToMicrophoneFailed()
        {
            proxy.Setup(p => p.StartRecording(5512, BassConstants.NumberOfChannels, It.IsAny<BassFlags>())).Returns(0);
            proxy.Setup(p => p.GetLastError()).Returns("Failed to create stream to microphone");

            Assert.Throws<BassException>(() => streamFactory.CreateStreamFromMicrophone(5512));
        }
    }
}
