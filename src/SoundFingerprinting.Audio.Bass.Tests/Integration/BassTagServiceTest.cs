namespace SoundFingerprinting.Audio.Bass.Tests.Integration
{
    using System;

    using NUnit.Framework;
    using NUnit.Framework.Internal;

    using SoundFingerprinting.Audio;
    using SoundFingerprinting.Audio.Bass;

    [TestFixture]
    public class BassTagServiceTest : AbstractIntegrationTest
    {
        private readonly ITagService tagService = new BassTagService();

        [Test]
        public void TagAreSuccessfullyReadFromFile()
        {
            TagInfo tags = tagService.GetTagInfo(PathToMp3);
            Assert.IsNotNull(tags);
            Assert.AreEqual("3 Doors Down", tags.Artist);
            Assert.AreEqual("Kryptonite", tags.Title);
            Assert.AreEqual("USUR19980187", tags.ISRC);
            Assert.AreEqual(1997, tags.Year);
            Assert.IsTrue(Math.Abs(232 - tags.Duration) < 1);
        }
    }
}
