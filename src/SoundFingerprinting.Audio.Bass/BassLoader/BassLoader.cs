namespace SoundFingerprinting.Audio.Bass.BassLoader
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using SoundFingerprinting.Audio.Bass.BassLoader.Info;
    using SoundFingerprinting.Audio.Bass.LibraryLoader;

    internal class BassLoader : IBassLoader
    {
        private IntPtr bass;
        private IntPtr bassMix;

        private readonly ILibraryLoader loader;
        private readonly IBassLibraryInfo info;

        public BassLoader(ILibraryLoader loader, IBassLibraryInfo info)
        {
            this.loader = loader;
            this.info = info;
        }

        public void Load()
        {
            Load(info.BassFileName, ref bass);
            Load(info.BassMixFileName, ref bassMix);
        }

        public void Free()
        {
            Free(ref bassMix);
            Free(ref bass);
        }

        private void Load(string libraryName, ref IntPtr handle)
        {
            if (handle == IntPtr.Zero)
            {
                handle = loader.Load(Path.Combine(info.Path, libraryName));
            }
        }

        private void Free(ref IntPtr handle)
        {
            if (handle == IntPtr.Zero)
            {
                return;
            }
            try
            {
                loader.Free(handle);
                handle = IntPtr.Zero;
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
            }
        }
    }
}
