namespace SoundFingerprinting.Audio.Bass.Tests.Unit
{
    using Moq;

    using NUnit.Framework;
    using NUnit.Framework.Internal;

    using SoundFingerprinting.Audio.Bass;

    [TestFixture]
    public class BassSamplesProviderTest : AbstractTest
    {
        private const int SourceId = 100;

        private BassSamplesProvider samplesProvider;

        private Mock<IBassServiceProxy> proxy;

        [SetUp]
        public void SetUp()
        {
            proxy = new Mock<IBassServiceProxy>(MockBehavior.Strict);

            samplesProvider = new BassSamplesProvider(proxy.Object, SourceId);
        }

        [Test]
        public void TestGetSamplesProvider()
        {
            const int LengthInBytes = 1024 * 4;
            proxy.Setup(p => p.ChannelGetData(SourceId, It.IsAny<float[]>(), LengthInBytes)).Returns(LengthInBytes);

            var result = samplesProvider.GetNextSamples(new float[LengthInBytes / 4]);

            Assert.AreEqual(LengthInBytes, result);
        }
    }
}
