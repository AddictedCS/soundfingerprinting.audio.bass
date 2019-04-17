namespace SoundFingerprinting.Audio.Bass.BassLoader.Info
{
    using System;

    internal class WindowsBassLibraryInfo : IBassLibraryInfo
    {
        public string Path
        {
            get
            {
                var baseDir = AppDomain.CurrentDomain.BaseDirectory;
                var arch = Environment.Is64BitProcess ? "x64" : "x86";
                return System.IO.Path.Combine(baseDir, arch);
            }
        }

        public string BassFileName => "bass.dll";
        public string BassMixFileName => "bassmix.dll";
    }
}
