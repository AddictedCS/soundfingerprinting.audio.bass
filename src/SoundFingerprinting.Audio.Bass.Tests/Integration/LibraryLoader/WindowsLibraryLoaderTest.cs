namespace SoundFingerprinting.Audio.Bass.Tests.Integration.LibraryLoader
{
    using System;
    using System.IO;
    using NUnit.Framework;
    using BassLoader.Info;
    using Bass.LibraryLoader;

    [TestFixture]
    [Platform("Win")]
    public class WindowsLibraryLoaderTest
    {
        private readonly ILibraryLoader loader = new WindowsLibraryLoader();
        private readonly IBassLibraryInfo info = new WindowsBassLibraryInfo();

        [Test]
        public void ShouldLoadAndFreeLibrary()
        {
            var path = Path.Combine(info.Path, info.BassFileName);
            var handle = IntPtr.Zero;

            Assert.DoesNotThrow(() => handle = loader.Load(path));
            loader.Free(handle);
        }

        [Test]
        public void ShouldExplainWhyLoadFailed()
        {
            try
            {
                loader.Load("lib/missing.dll");
            }
            catch (LoaderException e)
            {
                Assert.AreEqual("Failed to load library from lib/missing.dll", e.Message);
                Assert.AreEqual("The specified module could not be found", e.InnerException.Message);
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
                Assert.AreEqual($"Failed to release library with handle={handle}", e.Message);
                Assert.AreEqual("The specified module could not be found", e.InnerException.Message);
            }
        }
    }
}
