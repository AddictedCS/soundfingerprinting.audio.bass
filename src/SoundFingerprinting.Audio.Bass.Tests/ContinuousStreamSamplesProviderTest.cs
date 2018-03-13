namespace SoundFingerprinting.Audio.Bass.Tests
{
    using System.Collections.Generic;

    using NUnit.Framework;

    using SoundFingerprinting.Audio;
    using SoundFingerprinting.Tests.Unit.Audio;

    [TestFixture]
    public class ContinuousStreamSamplesProviderTest
    {
        private ContinuousStreamSamplesProvider samplesProvider;

        [Test]
        public void TestGetNextSamples()
        {
            float[] buffer = new float[1024];
            var queue = new Queue<int>(new[] { 1024, 0, 0, 512, 0, 0, 0, 256 });

            samplesProvider = new ContinuousStreamSamplesProvider(new QueueSamplesProvider(queue));

            int[] expectedResults = new[] { 1024, 512, 256 };
            for (int i = 0; i < 3; i++)
            {
                var result = samplesProvider.GetNextSamples(buffer);
                Assert.AreEqual(expectedResults[i], result);
            }
        }
    }
}
