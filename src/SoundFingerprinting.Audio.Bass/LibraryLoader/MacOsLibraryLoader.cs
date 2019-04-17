namespace SoundFingerprinting.Audio.Bass.LibraryLoader
{
    using System;

    internal class MacOsLibraryLoader : ILibraryLoader
    {
        public IntPtr Load(string path)
        {
            // no-op
            return IntPtr.Zero;
        }

        public void Free(IntPtr handle)
        {
            // no-op
        }
    }
}
