namespace SoundFingerprinting.Audio.Bass.Tests.Integration.LibraryLoader
{
    using System;
    using System.IO;
    using NUnit.Framework;
    using BassLoader.Info;
    using Bass.LibraryLoader;

    [TestFixture]
    [Platform("Linux")]
    public class LinuxLibraryLoaderTest
    {
        private readonly ILibraryLoader loader = new LinuxLibraryLoader();
        private readonly IBassLibraryInfo info = new LinuxBassLibraryInfo();

        [Test]
        public void ShouldLoadAndFreeLibrary()
        {
            var path = Path.Combine(info.Path, info.BassFileName);
            var handle = IntPtr.Zero;

            Assert.DoesNotThrow(() => handle = loader.Load(path));
            Assert.AreNotEqual(IntPtr.Zero, handle);
            loader.Free(handle);
        }

        [Test]
        public void ShouldExplainWhyLoadFailed()
        {
            try
            {
                loader.Load("lib/libmissing.so");
            }
            catch (LoaderException e)
            {
                Assert.IsTrue(e.Message.StartsWith("Failed to load library from lib/libmissing.so. Reason:"));
            }
        }

        [Test]
        public void ShouldExplainWhyFreeFailed()
        {
            var path = Path.Combine(info.Path, info.BassMixFileName);
            var handle = IntPtr.Zero;
            Assert.DoesNotThrow(() => handle = loader.Load(path));
            loader.Free(handle);
            try
            {
                loader.Free(handle);
            }
            catch (LoaderException e)
            {
                Assert.AreEqual($"Failed to release library with handle={handle}. Reason:", e.Message);
            }
        }
    }
}
