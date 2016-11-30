namespace SoundFingerprinting.Audio.Bass.Tests.Integration
{
    using System;
    using System.Linq;

    using NUnit.Framework;

    using SoundFingerprinting.Audio;
    using SoundFingerprinting.Audio.Bass;
    using SoundFingerprinting.Audio.Bass.Tests;
    using SoundFingerprinting.Builder;
    using SoundFingerprinting.Configuration;
    using SoundFingerprinting.DAO.Data;
    using SoundFingerprinting.InMemory;

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
            var track = new TrackData(tags);
            var trackReference = modelService.InsertTrack(track);

            var hashDatas = fingerprintCommandBuilder
                                            .BuildFingerprintCommand()
                                            .From(PathToMp3)
                                            .UsingServices(audioService)
                                            .Hash()
                                            .Result;

            modelService.InsertHashDataForTrack(hashDatas, trackReference);

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
