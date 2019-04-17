namespace SoundFingerprinting.Audio.Bass.BassLoader
{
    using System;
    using Info;
    using LibraryLoader;

    internal class BassLoaderFactory : IBassLoaderFactory
    {
        public IBassLoader CreateLoader(PlatformID platformId)
        {
            switch (platformId)
            {
                case PlatformID.MacOSX:
                    return new BassLoader(new MacOsLibraryLoader(), new MacOsBassLibraryInfo());
                case PlatformID.Unix:
                    return new BassLoader(new LinuxLibraryLoader(), new LinuxBassLibraryInfo());
                default:
                    return new BassLoader(new WindowsLibraryLoader(), new WindowsBassLibraryInfo());
            }
        }

        public IBassLoader CreateLoader()
        {
            return CreateLoader(Environment.OSVersion.Platform);
        }
    }
}
