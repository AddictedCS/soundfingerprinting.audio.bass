namespace SoundFingerprinting.Audio.Bass.LibraryLoader
{
    using System;
    using System.ComponentModel;
    using System.Runtime.InteropServices;

    internal class WindowsLibraryLoader : ILibraryLoader
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr LoadLibrary(string path);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool FreeLibrary(IntPtr handle);

        public IntPtr Load(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException($"Invalid library path: {path}");
            }
            var handle = LoadLibrary(path);
            if (handle == IntPtr.Zero)
            {
                throw new LoaderException($"Failed to load library from {path}",
                    new Win32Exception(Marshal.GetLastWin32Error()));
            }
            return handle;
        }

        public void Free(IntPtr handle)
        {
            if (!FreeLibrary(handle))
            {
                throw new LoaderException($"Failed to release library with handle={handle}",
                    new Win32Exception(Marshal.GetLastWin32Error()));
            }
        }
    }
}
