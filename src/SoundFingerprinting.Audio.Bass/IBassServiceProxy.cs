namespace SoundFingerprinting.Audio.Bass
{
    using System;
    using ManagedBass;

    internal interface IBassServiceProxy : IDisposable
    {
        int GetVersion();

        bool Init(int deviceNumber, int sampleRate, DeviceInitFlags flags);

        bool SetConfig(Configuration config, bool value);

        bool RecordInit(int deviceNumber);

        string GetLastError();

        int GetRecordingDevice();

        int CreateStream(string pathToAudioFile, BassFlags flags);

        int CreateStreamFromUrl(string urlToResource, BassFlags flags);

        int StartRecording(int sampleRate, int numberOfChannels, BassFlags flags);

        bool StartPlaying(int stream);

        int CreateMixerStream(int sampleRate, int channels, BassFlags flags);

        bool CombineMixerStreams(int mixerStream, int stream, BassFlags flags);

        bool ChannelSetPosition(int stream, double seekToSecond);

        bool ChannelSetAttribute(int stream, ChannelAttribute attribute, float value);

        int ChannelGetData(int stream, float[] buffer, int lengthInBytes);

        double ChannelGetLengthInSeconds(int stream);

        bool FreeStream(int stream);

        bool PluginFree(int number);

        bool BassFree();
    }
}
