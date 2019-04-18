namespace SoundFingerprinting.Audio.Bass
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using ManagedBass;
    using ManagedBass.Mix;
    using BassLoader;

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

        public int GetVersion()
        {
            return Bass.Version.Major; 
        }

        public bool Init(int deviceNumber, int sampleRate, DeviceInitFlags flags)
        {
            return Bass.Init(deviceNumber, sampleRate, flags, IntPtr.Zero);
        }

        public bool SetConfig(Configuration config, bool value)
        {
            return Bass.Configure(config, value);
        }

        public bool RecordInit(int deviceNumber)
        {
            return Bass.RecordInit(deviceNumber);
        }

        public string GetLastError()
        {
            return Bass.LastError.ToString();
        }

        public int GetRecordingDevice()
        {
            return Bass.RecordingInfo.Inputs;
        }

        public int CreateStream(string pathToAudioFile, BassFlags flags)
        {
            return Bass.CreateStream(pathToAudioFile, 0, 0, flags);
        }

        public int CreateStreamFromUrl(string urlToResource, BassFlags flags)
        {
            return Bass.CreateStream(urlToResource, 0, flags, null, IntPtr.Zero);
        }

        public int StartRecording(int sampleRate, int numberOfChannels, BassFlags flags)
        {
            return Bass.RecordStart(sampleRate, numberOfChannels, flags, null, IntPtr.Zero);
        }

        public bool StartPlaying(int stream)
        {
            return Bass.ChannelPlay(stream);
        }

        public int CreateMixerStream(int sampleRate, int channels, BassFlags flags)
        {
            return BassMix.CreateMixerStream(sampleRate, channels, flags);
        }

        public bool CombineMixerStreams(int mixerStream, int stream, BassFlags flags)
        {
            return BassMix.MixerAddChannel(mixerStream, stream, flags);
        }

        public bool ChannelSetPosition(int stream, double seekToSecond)
        {
            return Bass.ChannelSetPosition(stream, Bass.ChannelSeconds2Bytes(stream, seekToSecond));
        }

        public bool ChannelSetAttribute(int stream, ChannelAttribute attribute, float value)
        {
            return Bass.ChannelSetAttribute(stream, attribute, value);
        }

        public int ChannelGetData(int stream, float[] buffer, int lengthInBytes)
        {
            return Bass.ChannelGetData(stream, buffer, lengthInBytes);
        }

        public double ChannelGetLengthInSeconds(int stream)
        {
            long bytes = Bass.ChannelGetLength(stream);
            return Bass.ChannelBytes2Seconds(stream, bytes);
        }

        public bool FreeStream(int stream)
        {
            if (!Bass.StreamFree(stream))
            {
                Trace.WriteLine("Could not release stream " + stream + ". Possible memory leak! Bass Error: " + GetLastError(), "Error");
                return false;
            }

            return true;
        }

        public bool PluginFree(int number)
        {
            return Bass.PluginFree(number);
        }

        public bool BassFree()
        {
            return Bass.Free();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
            alreadyDisposed = true;
        }

        protected virtual void Dispose(bool isDisposing)
        {
            if (!alreadyDisposed && isDisposing)
            {
                lifetimeManager.Dispose();
            }
        }

        private class BassLifetimeManager : IDisposable
        {
            private IBassLoader bassLoader = new BassLoaderFactory().CreateLoader();

            private static int initializedInstances;

            private readonly IBassServiceProxy proxy;

            private bool alreadyDisposed;

            public BassLifetimeManager(IBassServiceProxy proxy)
            {
                this.proxy = proxy;
                if (IsBassLibraryHasToBeInitialized(Interlocked.Increment(ref initializedInstances)))
                {
                    bassLoader.Load();

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

                        bassLoader.Free();
                    }
                }

                alreadyDisposed = true;
            }

            private bool IsBassLibraryHasToBeInitialized(int numberOfInstances)
            {
                return numberOfInstances == 1;
            }

            private void InitializeBassLibraryWithAudioDevices()
            {
                if (!proxy.Init(-1, 44100, DeviceInitFlags.Default | DeviceInitFlags.Mono))
                {
                    Trace.WriteLine("Failed to find a sound device on running machine. Playing audio files will not be supported. " + proxy.GetLastError(), "Warning");
                    if (!proxy.Init(0, 44100, DeviceInitFlags.Default | DeviceInitFlags.Mono))
                    {
                        throw new BassException(proxy.GetLastError());
                    }
                }
            }

            private void SetDefaultConfigs()
            {
                if (!proxy.SetConfig(Configuration.FloatDSP, true))
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
        }
    }
}