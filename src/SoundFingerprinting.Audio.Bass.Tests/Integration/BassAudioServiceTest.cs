﻿namespace SoundFingerprinting.Audio.Bass.Tests.Integration
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization.Formatters.Binary;

    using NUnit.Framework;

    [TestFixture]
    public class BassAudioServiceTest : AbstractIntegrationTest
    {
        private readonly BassAudioService bassAudioService;
        private readonly BassWaveFileUtility bassWaveFileUtility;

        public BassAudioServiceTest()
        {
            bassAudioService = new BassAudioService();
            bassWaveFileUtility = new BassWaveFileUtility();
        }

        [Test]
        public void ShouldEstimateDurationCorrectly()
        {
            float duration = bassAudioService.GetLengthInSeconds(PathToMp3);

            Assert.AreEqual(193.07, duration, 0.5);
        }

        [Test]
        public void DurationOfReadAudioIsCorrect()
        {
            var audioSamples = bassAudioService.ReadMonoSamplesFromFile(PathToMp3, SampleRate);

            Assert.AreEqual(193.07, audioSamples.Duration, 0.01);
            Assert.AreEqual(PathToMp3, audioSamples.Origin);
        }

        // [Test]
        public void CompareReadingFromASpecificPartOfTheSong()
        {
            const int SecondsToRead = 10;
            const int StartAtSecond = 20;
            const int AcceptedError = 5;
                    
            BinaryFormatter serializer = new BinaryFormatter();

            using (Stream stream = new FileStream(PathToSamples, FileMode.Open, FileAccess.Read))
            {
                AudioSamples samples = (AudioSamples)serializer.Deserialize(stream);
                float[] subsetOfSamples = GetSubsetOfSamplesFromFullSong(samples.Samples, SecondsToRead, StartAtSecond);
                var audioSamples = bassAudioService.ReadMonoSamplesFromFile(PathToMp3, SampleRate, SecondsToRead, StartAtSecond);
                Assert.AreEqual(subsetOfSamples.Length, audioSamples.Samples.Length);
                Assert.IsTrue(
                    Math.Abs(subsetOfSamples.Sum(s => Math.Abs(s)) - audioSamples.Samples.Sum(s => Math.Abs(s))) < AcceptedError,
                    "Seek is working wrong!");
            }
        }

        [Test]
        public void ReadMonoFromFileTest()
        {
            string tempFile = string.Format(@"{0}{1}", Path.GetTempPath(), "0.wav");
            var audioSamples = bassAudioService.ReadMonoSamplesFromFile(PathToMp3, SampleRate);

            bassWaveFileUtility.WriteSamplesToFile(audioSamples.Samples, SampleRate, tempFile);
            
            FileInfo info = new FileInfo(tempFile);
            long expectedSize = info.Length - WaveHeader;
            long actualSize = audioSamples.Samples.Length * (BitsPerSample / 8);
            Assert.AreEqual(expectedSize, actualSize);
            File.Delete(tempFile);
        }

        private float[] GetSubsetOfSamplesFromFullSong(float[] samples, int secondsToRead, int startAtSecond)
        {
            float[] array = new float[SampleRate * secondsToRead];
            Array.Copy(samples, startAtSecond * SampleRate, array, 0, SampleRate * secondsToRead);
            return array;
        }
    }
}
