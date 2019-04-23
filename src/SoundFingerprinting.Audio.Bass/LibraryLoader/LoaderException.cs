namespace SoundFingerprinting.Audio.Bass.LibraryLoader
{
    using System;

    public class LoaderException : Exception
    {
        public LoaderException(string message) : base(message) { }

        public LoaderException(string message, Exception innerException) : base(message, innerException) { }
    }
}
