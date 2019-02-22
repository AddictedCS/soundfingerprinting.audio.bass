namespace SoundFingerprinting.Audio.Bass
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Web;
    using System.Web.Configuration;

    using Un4seen.Bass;
    using Un4seen.Bass.AddOn.Mix;
    using Un4seen.Bass.AddOn.Tags;

    internal class BassServiceProxy : IBassServiceProxy
    {
        private readonly BassLifetimeManager lifetimeManager;

        private bool alreadyDisposed;

        protected BassServiceProxy()
        {
            lifetimeManager = new BassLifetimeManager(this);
        }

        ~BassServiceProxy()
        {
            Dispose(false);
        }

        public static BassServiceProxy Instance { get; } = new BassServiceProxy();

        public void RegisterBass(string email, string registrationKey)
        {
            BassNet.Registration(email, registrationKey);
        }

        public bool BassLoadMe(string targetPath)
        {
            return Bass.LoadMe(targetPath);
        }

        public bool BassMixLoadMe(string targetPath)
        {
            return BassMix.LoadMe(targetPath);
        }

        public int GetVersion()
        {
            return Bass.BASS_GetVersion();
        }

        public int GetMixerVersion()
        {
            return BassMix.BASS_Mixer_GetVersion();
        }

        public IDictionary<int, string> PluginLoadDirectory(string path)
        {
            return Bass.BASS_PluginLoadDirectory(path);
        }

        public bool Init(int deviceNumber, int sampleRate, BASSInit flags)
        {
            return Bass.BASS_Init(deviceNumber, sampleRate, flags, IntPtr.Zero);
        }

        public bool SetConfig(BASSConfig config, int value)
        {
            return Bass.BASS_SetConfig(config, value);
        }

        public bool SetConfig(BASSConfig config, bool value)
        {
            return Bass.BASS_SetConfig(config, value);
        }

        public bool RecordInit(int deviceNumber)
        {
            return Bass.BASS_RecordInit(deviceNumber);
        }

        public string GetLastError()
        {
            return Bass.BASS_ErrorGetCode().ToString();
        }

        public int GetRecordingDevice()
        {
            return Bass.BASS_RecordGetDevice();
        }

        public int CreateStream(string pathToAudioFile, BASSFlag flags)
        {
            return Bass.BASS_StreamCreateFile(pathToAudioFile, 0, 0, flags);
        }

        public int CreateStreamFromUrl(string urlToResource, BASSFlag flags)
        {
            return Bass.BASS_StreamCreateURL(urlToResource, 0, flags, null, IntPtr.Zero);
        }

        public int StartRecording(int sampleRate, int numberOfChannels, BASSFlag flags)
        {
            return Bass.BASS_RecordStart(sampleRate, numberOfChannels, flags, null, IntPtr.Zero);
        }

        public bool StartPlaying(int stream)
        {
            return Bass.BASS_ChannelPlay(stream, false);
        }

        public int CreateMixerStream(int sampleRate, int channels, BASSFlag flags)
        {
            return BassMix.BASS_Mixer_StreamCreate(sampleRate, channels, flags);
        }

        public bool CombineMixerStreams(int mixerStream, int stream, BASSFlag flags)
        {
            return BassMix.BASS_Mixer_StreamAddChannel(mixerStream, stream, flags);
        }

        public bool ChannelSetPosition(int stream, double seekToSecond)
        {
            return Bass.BASS_ChannelSetPosition(stream, seekToSecond);
        }

        public bool ChannelSetAttribute(int stream, BASSAttribute attribute, float value)
        {
            return Bass.BASS_ChannelSetAttribute(stream, attribute, value);
        }

        public int ChannelGetData(int stream, float[] buffer, int lengthInBytes)
        {
            return Bass.BASS_ChannelGetData(stream, buffer, lengthInBytes);
        }

        public double ChannelGetLengthInSeconds(int stream)
        {
            long bytes = Bass.BASS_ChannelGetLength(stream, BASSMode.BASS_POS_BYTES);
            return Bass.BASS_ChannelBytes2Seconds(stream, bytes);
        }

        public bool FreeStream(int stream)
        {
            if (!Bass.BASS_StreamFree(stream))
            {
                Trace.WriteLine("Could not release stream " + stream + ". Possible memory leak! Bass Error: " + GetLastError(), "Error");
                return false;
            }

            return true;
        }

        public bool PluginFree(int number)
        {
            return Bass.BASS_PluginFree(number);
        }

        public bool BassFree()
        {
            return Bass.BASS_Free();
        }

        public TAG_INFO GetTagsFromFile(string pathToFile)
        {
            return BassTags.BASS_TAG_GetFromFile(pathToFile);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
            alreadyDisposed = true;
        }

        protected virtual void Dispose(bool isDisposing)
        {
            if (!alreadyDisposed)
            {
                lifetimeManager.Dispose();
            }
        }

        private class BassLifetimeManager : IDisposable
        {
            private const string FlacDllName = "bassflac.dll";

            private static int initializedInstances;

            private readonly IBassServiceProxy proxy;

            private bool alreadyDisposed;

            public BassLifetimeManager(IBassServiceProxy proxy)
            {
                this.proxy = proxy;
                if (IsBassLibraryHasToBeInitialized(Interlocked.Increment(ref initializedInstances)))
                {
                    RegisterBassKey();

                    if (Environment.OSVersion.Platform != PlatformID.MacOSX && Environment.OSVersion.Platform != PlatformID.Unix)
                    {
                        // MacOS and Unix libraries do not have entry points
                        // for methods that load the native libraries from specific location
                        // thus for this platforms loading is ignored.
                        string targetPath = GetTargetPathToLoadLibrariesFrom();
                        LoadBassLibraries(targetPath);
                        CheckIfFlacPluginIsLoaded(targetPath);
                    }
                    else
                    {
                        // force loading the libs
                        proxy.GetVersion();
                    }

                    InitializeBassLibraryWithAudioDevices();
                    SetDefaultConfigs();
                    InitializeRecordingDevice();
                }
            }

            ~BassLifetimeManager()
            {
                Dispose();
            }

            public void Dispose()
            {
                GC.SuppressFinalize(this);

                if (!alreadyDisposed)
                {
                    if (Interlocked.Decrement(ref initializedInstances) == 0)
                    {
                        // 0 - free all loaded plugins
                        if (!proxy.PluginFree(0))
                        {
                            Trace.WriteLine("Could not unload plugins for Bass library.", "Error");
                        }

                        if (!proxy.BassFree())
                        {
                            Trace.WriteLine("Could not free Bass library. Possible memory leak!", "Error");
                        }
                    }
                }

                alreadyDisposed = true;
            }

            private bool IsBassLibraryHasToBeInitialized(int numberOfInstances)
            {
                return numberOfInstances == 1;
            }

            private void RegisterBassKey()
            {
                var config = GetConfiguration();

                var bassConfigurationSection = config.GetSection("BassConfigurationSection") as BassConfigurationSection;

                if (bassConfigurationSection != null)
                {
                    proxy.RegisterBass(bassConfigurationSection.Email, bassConfigurationSection.RegistrationKey); // Call to avoid the freeware splash screen
                }
            }

            private string GetTargetPathToLoadLibrariesFrom()
            {
                string executingPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase);
                if (string.IsNullOrEmpty(executingPath))
                {
                    throw new BassException("Executing path of the application is null or empty. Could not find folders with native DLL libraries.");
                }

                UriBuilder uri = new UriBuilder(executingPath);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.Combine(path, Utils.Is64Bit ? "x64" : "x86");
            }

            private void LoadBassLibraries(string targetPath)
            {
                if (!proxy.BassLoadMe(targetPath))
                {
                    throw new BassException("Could not load bass native libraries from the following path: " + targetPath);
                }

                if (!proxy.BassMixLoadMe(targetPath))
                {
                    throw new BassException("Could not load bassmix library from the following path: " + targetPath);
                }

                DummyCallToLoadBassLibraries();
            }

            private void DummyCallToLoadBassLibraries()
            {
                proxy.GetVersion();
                proxy.GetMixerVersion();
            }

            private void InitializeBassLibraryWithAudioDevices()
            {
                if (!proxy.Init(-1, 44100, BASSInit.BASS_DEVICE_DEFAULT | BASSInit.BASS_DEVICE_MONO))
                {
                    Trace.WriteLine("Failed to find a sound device on running machine. Playing audio files will not be supported. " + proxy.GetLastError(), "Warning");
                    if (!proxy.Init(0, 44100, BASSInit.BASS_DEVICE_DEFAULT | BASSInit.BASS_DEVICE_MONO))
                    {
                        throw new BassException(proxy.GetLastError());
                    }
                }
            }

            private void CheckIfFlacPluginIsLoaded(string targetPath)
            {
                var loadedPlugIns = proxy.PluginLoadDirectory(targetPath);
                if (!loadedPlugIns.Any(p => p.Value.EndsWith(FlacDllName)))
                {
                    Trace.WriteLine("Could not load bassflac.dll. FLAC format is not supported!", "Warning");
                }
            }

            private void SetDefaultConfigs()
            {
                if (!proxy.SetConfig(BASSConfig.BASS_CONFIG_FLOATDSP, true))
                {
                    throw new BassException(proxy.GetLastError());
                }
            }

            private void InitializeRecordingDevice()
            {
                const int DefaultDevice = -1;
                if (!proxy.RecordInit(DefaultDevice))
                {
                    Trace.WriteLine("No default recording device could be found on running machine. Recording is not supported: " + proxy.GetLastError(), "Warning");
                }
            }

            private Configuration GetConfiguration()
            {
                if (HttpContext.Current != null)
                {
                    return WebConfigurationManager.OpenWebConfiguration("~");
                }

                return ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            }
        }
    }
}