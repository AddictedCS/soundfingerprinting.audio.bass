namespace SoundFingerprinting.Audio.Bass.Tests.Integration
{
    using System;
    using System.Linq;

    using Audio;
    using Bass;
    using Builder;
    using DAO.Data;
    using InMemory;

    using NUnit.Framework;

    using SoundFingerprinting.Data;

    using Tests;

    [TestFixture]
    public class FingerprintCommandBuilderIntTest : AbstractTest
    {
        private readonly IModelService modelService = new InMemoryModelService();
        private readonly IFingerprintCommandBuilder fingerprintCommandBuilder = new FingerprintCommandBuilder();
        private readonly IQueryCommandBuilder queryCommandBuilder = new QueryCommandBuilder();
        private readonly IAudioService audioService = new BassAudioService();
        private readonly ITagService tagService = new BassTagService();

        [Test]
        public void ShouldCreateFingerprintsInsertThenQueryAndGetTheRightResult()
        {
            const int SecondsToProcess = 10;
            const int StartAtSecond = 30;
            var tags = tagService.GetTagInfo(PathToMp3);
            var track = new TrackInfo(Guid.NewGuid().ToString(), tags.Title, tags.Artist, tags.Duration);

            var hashDatas = fingerprintCommandBuilder
                                            .BuildFingerprintCommand()
                                            .From(PathToMp3)
                                            .UsingServices(audioService)
                                            .Hash()
                                            .Result;

            var trackReference = modelService.Insert(track, hashDatas);

            var queryResult = queryCommandBuilder.BuildQueryCommand()
                               .From(PathToMp3, SecondsToProcess, StartAtSecond)
                               .UsingServices(modelService, audioService)
                               .Query()
                               .Result;

            Assert.IsTrue(queryResult.ContainsMatches);
            Assert.AreEqual(1, queryResult.ResultEntries.Count());
            Assert.AreEqual(trackReference, queryResult.BestMatch.Track.TrackReference);
            Assert.IsTrue(queryResult.BestMatch.QueryMatchLength > SecondsToProcess - 2);
            Assert.AreEqual(StartAtSecond, Math.Abs(queryResult.BestMatch.TrackStartsAt), 0.1d);
            Assert.IsTrue(queryResult.BestMatch.Confidence > 0.8);
        }
}
}
