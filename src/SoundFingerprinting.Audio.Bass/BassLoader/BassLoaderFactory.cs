namespace SoundFingerprinting.Audio.Bass.BassLoader
{
    using System;
    using System.Runtime.InteropServices;
    using Info;
    using LibraryLoader;

    internal class BassLoaderFactory : IBassLoaderFactory
    {
        public IBassLoader CreateLoader()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return new BassLoader(new NoOpLibraryLoader(), new EmptyBassLibraryInfo());
            }
            
            switch (Environment.OSVersion.Platform)
            {
                case PlatformID.Unix:
                    if (Type.GetType("Mono.Runtime") != null)
                    {
                        return new BassLoader(new NoOpLibraryLoader(), new EmptyBassLibraryInfo());
                    }
                    return new BassLoader(new LinuxLibraryLoader(), new LinuxBassLibraryInfo());
                default:
                    return new BassLoader(new WindowsLibraryLoader(), new WindowsBassLibraryInfo());
            }
        }
    }
}
