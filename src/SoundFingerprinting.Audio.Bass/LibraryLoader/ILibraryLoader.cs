namespace SoundFingerprinting.Audio.Bass.LibraryLoader
{
    using System;

    internal interface ILibraryLoader
    {
        IntPtr Load(string path);

        void Free(IntPtr handle);
    }
}
