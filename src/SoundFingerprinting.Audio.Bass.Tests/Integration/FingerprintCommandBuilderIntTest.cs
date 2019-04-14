namespace SoundFingerprinting.Audio.Bass.Tests.Integration
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Audio;
    using Bass;
    using Builder;
    using InMemory;
    using Configuration;

    using NUnit.Framework;

    using SoundFingerprinting.Data;
    using SoundFingerprinting.Strides;

    [TestFixture]
    public class FingerprintCommandBuilderIntTest : AbstractTest
    {
        private readonly IAudioService audioService = new BassAudioService(6);

        [Test]
        public async Task CreateFingerprintsFromFileAndAssertNumberOfFingerprints()
        {
            const int StaticStride = 5096;

            var fingerprintConfiguration = new DefaultFingerprintConfiguration { Stride = new IncrementalStaticStride(StaticStride) };

            var command = FingerprintCommandBuilder.Instance.BuildFingerprintCommand()
                .From(PathToMp3)
                .WithFingerprintConfig(fingerprintConfiguration)
                .UsingServices(audioService);

            double seconds = audioService.GetLengthInSeconds(PathToMp3);
            int samples = (int) (seconds * 5512);
            int expectedFingerprints = (int)Math.Round((double) samples / StaticStride);

            var fingerprints = await command.Hash();

            Assert.AreEqual(expectedFingerprints, fingerprints.Count);
        }

        [Test]
        public async Task CreateFingerprintsWithTheSameFingerprintCommandTest()
        {
            const int SecondsToProcess = 8;
            const int StartAtSecond = 1;

            var fingerprintCommand = FingerprintCommandBuilder.Instance
                .BuildFingerprintCommand()
                .From(PathToMp3, SecondsToProcess, StartAtSecond)
                .UsingServices(audioService);

            var firstHashDatas = await fingerprintCommand.Hash();
            var secondHashDatas = await fingerprintCommand.Hash();

            AssertHashDatasAreTheSame(firstHashDatas, secondHashDatas);
        }

        [Test]
        public async Task ShouldCreateFingerprintsInsertThenQueryAndGetTheRightResult()
        {
            const int SecondsToProcess = 10;
            const int StartAtSecond = 30;

            var modelService = new InMemoryModelService();
            var track = new TrackInfo(Guid.NewGuid().ToString(), "title", "artist", audioService.GetLengthInSeconds(PathToMp3));

            var hashDatas = await FingerprintCommandBuilder.Instance
                .BuildFingerprintCommand()
                .From(PathToMp3)
                .UsingServices(audioService)
                .Hash();

            var trackReference = modelService.Insert(track, hashDatas);

            var queryResult = await QueryCommandBuilder.Instance.BuildQueryCommand()
                .From(PathToMp3, SecondsToProcess, StartAtSecond)
                .UsingServices(modelService, audioService)
                .Query();

            Assert.IsTrue(queryResult.ContainsMatches);
            Assert.AreEqual(1, queryResult.ResultEntries.Count());
            Assert.AreEqual(trackReference, queryResult.BestMatch.Track.TrackReference);
            Assert.IsTrue(queryResult.BestMatch.QueryMatchLength > SecondsToProcess - 2);
            Assert.AreEqual(StartAtSecond, Math.Abs(queryResult.BestMatch.TrackStartsAt), 0.1d);
            Assert.IsTrue(queryResult.BestMatch.Confidence > 0.8);
        }
    }
}
