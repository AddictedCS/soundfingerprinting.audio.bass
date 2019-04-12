namespace SoundFingerprinting.Audio.Bass.Config
{
    public class BassConfig
    {
        public BassConfig(string email, string registrationKey)
        {
            this.Email = email;
            this.RegistrationKey = registrationKey;
        }

        public string Email { get; }

        public string RegistrationKey { get; }
    }
}
