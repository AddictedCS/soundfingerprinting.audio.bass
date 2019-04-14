namespace SoundFingerprinting.Audio.Bass
{
    using ManagedBass;

    public class BassPlayAudioFileService : IPlayAudioFileService
    {
        private readonly IBassServiceProxy proxy;

        public BassPlayAudioFileService(): this(BassServiceProxy.Instance)
        {
        }

        internal BassPlayAudioFileService(IBassServiceProxy proxy)
        {
            this.proxy = proxy;
        }

        public object PlayFile(string pathToFile)
        {
            int stream = proxy.CreateStream(pathToFile, BassFlags.Default);

            if (stream == 0)
            {
                throw new BassException(proxy.GetLastError());
            }

            if (!proxy.StartPlaying(stream))
            {
                throw new BassException(proxy.GetLastError());
            }

            return stream;
        }

        public void StopPlayingFile(object stream)
        {
            if (stream != null && (int)stream != 0)
            {
                proxy.FreeStream((int)stream);
            }
        }
    }
}
