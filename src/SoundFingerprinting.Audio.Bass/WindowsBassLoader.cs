using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace SoundFingerprinting.Audio.Bass
{
    internal class WindowsBassLoader
    {
        private int bassHandle;
        private int bassMixHandle;

        public bool LoadBass()
        {
            return Load(Path.Combine(GetPath(), "bass"), ref bassHandle);
        }

        public bool FreeBass()
        {
            return Free(ref bassHandle);
        }

        public bool LoadBassMix()
        {
            return Load(Path.Combine(GetPath(), "bassmix"), ref bassMixHandle);
        }

        public bool FreeBassMix()
        {
            return Free(ref bassMixHandle);
        }

        [DllImport("kernel32.dll")]
        private static extern int LoadLibrary(string path);

        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool FreeLibrary(int hModule);

        private static bool Load(string path, ref int handle)
        {
            if (handle == 0)
                handle = LoadLibrary(path);
            return (uint)handle > 0U;
        }

        private static bool Free(ref int handle)
        {
            return handle == 0 || FreeLibrary(handle);
        }

        private static string GetPath()
        {
            var executingPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase);
            if (string.IsNullOrEmpty(executingPath))
            {
                throw new BassException("Executing path of the application is null or empty. Could not find folders with native DLL libraries.");
            }

            var uri = new UriBuilder(executingPath);
            var path = Uri.UnescapeDataString(uri.Path);
            return Path.Combine(path, Environment.Is64BitProcess ? "x64" : "x86");
        }
    }
}
