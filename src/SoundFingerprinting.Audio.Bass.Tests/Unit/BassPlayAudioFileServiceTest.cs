namespace SoundFingerprinting.Audio.Bass.Tests.Unit
{
    using ManagedBass;
    using Moq;

    using NUnit.Framework;

    using SoundFingerprinting.Audio.Bass;
    using BassException = SoundFingerprinting.Audio.Bass.BassException;

    [TestFixture]
    public class BassPlayAudioFileServiceTest : AbstractTest
    {
        private BassPlayAudioFileService playAudioFileService;

        private Mock<IBassServiceProxy> proxy;

        [SetUp]
        public void SetUp()
        {
            proxy = new Mock<IBassServiceProxy>(MockBehavior.Strict);

            playAudioFileService = new BassPlayAudioFileService(proxy.Object);
        }

        [TearDown]
        public void TearDown()
        {
            proxy.VerifyAll();
        }

        [Test]
        public void TestPlayFile()
        {
            const int StreamId = 100;
            proxy.Setup(p => p.CreateStream("path-to-audio-file", BassFlags.Default)).Returns(StreamId);
            proxy.Setup(p => p.StartPlaying(StreamId)).Returns(true);

            var result = playAudioFileService.PlayFile("path-to-audio-file");

            Assert.AreEqual(StreamId, (int)result);
        }

        [Test]
        public void TestStopPlayingFile()
        {
            const int StreamId = 100;

            proxy.Setup(p => p.FreeStream(StreamId)).Returns(true);

            playAudioFileService.StopPlayingFile(StreamId);
        }

        [Test]
        public void TestPlayFileFailsWithExceptionNoStreamCreated()
        {
            proxy.Setup(p => p.CreateStream(It.IsAny<string>(), It.IsAny<BassFlags>())).Returns(0);
            proxy.Setup(p => p.GetLastError()).Returns("error-description");

            Assert.Throws<BassException>(() => playAudioFileService.PlayFile("path-to-audio-file"));
        }

        [Test]
        public void TestCouldNotStartPlayingTheFile()
        {
            proxy.Setup(p => p.CreateStream(It.IsAny<string>(), It.IsAny<BassFlags>())).Returns(1);
            proxy.Setup(p => p.GetLastError()).Returns("error-description");
            proxy.Setup(p => p.StartPlaying(It.IsAny<int>())).Returns(false);

            Assert.Throws<BassException>(() => playAudioFileService.PlayFile("path-to-audio-file"));
        }
    }
}
