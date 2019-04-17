namespace SoundFingerprinting.Audio.Bass.BassLoader.Info
{
    using System;
    using System.IO;

    internal class LinuxBassLibraryInfo : IBassLibraryInfo
    {
        private const string BassHome = "BASS_HOME";

        public string Path
        {
            get
            {
                var bassHome = Environment.GetEnvironmentVariable(BassHome);
                if (string.IsNullOrWhiteSpace(bassHome))
                {
                    var baseDir = AppDomain.CurrentDomain.BaseDirectory;
                    var arch = Environment.Is64BitProcess ? "x64" : "x86";
                    var path = System.IO.Path.Combine(baseDir, arch);
                    var export = $"export BASS_HOME={path}; export LD_LIBRARY_PATH=$LD_LIBRARY_PATH:$BASS_HOME";

                    throw new BassException($"Please set the {BassHome} environment variable to the path where BASS *.so modules reside. " +
                                            $"Also, add the same path to LD_LIBRARY_PATH.\n\nTry: {export}\n");
                }
                if (!Directory.Exists(bassHome))
                {
                    throw new BassException($"{BassHome}={bassHome} does not exist.");
                }
                if (!File.Exists(System.IO.Path.Combine(bassHome, BassFileName)))
                {
                    throw new BassException($"{BassHome}={bassHome} does not contain {BassFileName}.");
                }
                return bassHome;
            }
        }

        public string BassFileName => "libbass.so";
        public string BassMixFileName => "libbassmix.so";
    }
}
