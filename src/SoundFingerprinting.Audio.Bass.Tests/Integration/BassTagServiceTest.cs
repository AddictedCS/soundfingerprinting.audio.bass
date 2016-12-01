namespace SoundFingerprinting.Audio.Bass.Tests.Integration
{
    using Audio;
    using Bass;

    using NUnit.Framework;

    [TestFixture]
    public class BassTagServiceTest : AbstractIntegrationTest
    {
        private readonly ITagService tagService = new BassTagService();

        [Test]
        public void TagAreSuccessfullyReadFromFile()
        {
            TagInfo tags = tagService.GetTagInfo(PathToMp3);
            Assert.IsNotNull(tags);
            Assert.AreEqual(string.Empty, tags.Artist);
            Assert.AreEqual("Chopin", tags.Title);
            Assert.AreEqual(string.Empty, tags.ISRC);
            Assert.AreEqual(0, tags.Year);
            Assert.AreEqual(193.07d , tags.Duration, 0.1);
        }
    }
}
