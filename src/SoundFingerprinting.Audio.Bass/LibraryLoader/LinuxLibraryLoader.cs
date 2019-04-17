namespace SoundFingerprinting.Audio.Bass.LibraryLoader
{
    using System;
    using System.Runtime.InteropServices;

    internal class LinuxLibraryLoader : ILibraryLoader
    {
        [DllImport("libdl.so", EntryPoint = "dlopen")]
        private static extern IntPtr DLOpen(string fileName, Flags flags);

        [DllImport("libdl.so", EntryPoint = "dlclose")]
        private static extern int DLClose(IntPtr handle);

        [DllImport("libdl.so", EntryPoint = "dlerror")]
        private static extern IntPtr DLError();

        public IntPtr Load(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException($"Invalid library path: {path}");
            }
            DLError(); // clear previous errors, if any
            var handle = DLOpen(path, Flags.Lazy | Flags.Global);
            if (handle == IntPtr.Zero)
            {
                throw new LoaderException($"Failed to load library from {path}. " +
                                          $"Reason: {Marshal.PtrToStringAnsi(DLError())}");
            }
            return handle;
        }

        public void Free(IntPtr handle)
        {
            DLError(); // clear previous errors, if any
            if (DLClose(handle) != 0)
            {
                throw new LoaderException($"Failed to free library with handle={handle}. " +
                                          $"Reason: {Marshal.PtrToStringAnsi(DLError())}");
            }
        }

        [Flags]
        private enum Flags
        {
            Lazy = 0x00001,
            Now = 0x00002,

            Global = 0x00100,
            Local = 0,
            NoDelete = 0x01000,
            NoLoad = 0x00004,
            DeepBind = 0x00008
        }
    }
}
