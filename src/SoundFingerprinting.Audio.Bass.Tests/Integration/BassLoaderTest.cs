namespace SoundFingerprinting.Audio.Bass.Tests.Integration
{
    using System.Runtime.InteropServices;
    using NUnit.Framework;
    using ManagedBass;
    using BassLoader;

    [TestFixture]
    public class BassLoaderTest
    {
        private readonly IBassLoader loader = new BassLoaderFactory().CreateLoader();

        [Test]
        [Platform("Linux")]
        public void ShouldLoadBassLibrariesOnLinux()
        {
            try
            {
                loader.Load();
                Assert.AreEqual("2.4.14.1", GetBassVersion());
                Assert.AreEqual("2.4.9.0", GetBassMixVersion());
            }
            finally
            {
                loader.Free();
            }
        }

        [Test]
        [Platform("Win")]
        public void ShouldLoadBassLibrariesOnWindows()
        {
            try
            {
                loader.Load();
                Assert.AreEqual("2.4.12.1", GetBassVersion());
                Assert.AreEqual("2.4.8.0", GetBassMixVersion());
            }
            finally
            {
                loader.Free();
            }
        }

        private static string GetBassVersion()
        {
            return Bass.Version.ToString();
        }

        [DllImport("bassmix")]
        private static extern int BASS_Mixer_GetVersion();

        private static string GetBassMixVersion()
        {
            var version = BASS_Mixer_GetVersion();
            var major = version >> 24 & byte.MaxValue;
            var minor = version >> 16 & byte.MaxValue;
            var build = version >> 8 & byte.MaxValue;
            var revision = version & byte.MaxValue;
            return $"{major}.{minor}.{build}.{revision}";
        }
    }
}
