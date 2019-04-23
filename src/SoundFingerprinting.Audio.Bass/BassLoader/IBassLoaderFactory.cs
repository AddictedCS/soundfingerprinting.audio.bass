namespace SoundFingerprinting.Audio.Bass.BassLoader
{
    using System;

    internal interface IBassLoaderFactory
    {
        IBassLoader CreateLoader(PlatformID platformId);

        IBassLoader CreateLoader();
    }
}
