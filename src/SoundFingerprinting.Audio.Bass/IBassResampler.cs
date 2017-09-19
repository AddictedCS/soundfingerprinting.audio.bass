namespace SoundFingerprinting.Audio.Bass
{
    using System;

    internal interface IBassResampler
    {
        float[] Resample(int sourceStream, int sampleRate, double seconds, double startAt, int resampleQuality, Func<int, ISamplesProvider> getSamplesProvider);
    }
}