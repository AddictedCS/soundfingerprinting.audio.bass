using System;
using Microsoft.Extensions.Configuration;

namespace SoundFingerprinting.Audio.Bass.Config
{
    public class BassConfigReader
    {
        private const string AppSettings = "appsettings.json";

        private static readonly IConfiguration ConfigBuilder = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile(AppSettings, optional: false, reloadOnChange: false)
            .Build();

        public static BassConfig GetBassConfig()
        {
            var email = ConfigBuilder["email"];
            var registrationKey = ConfigBuilder["registrationKey"];
            var config = new BassConfig(email, registrationKey);
            return config;
        }
    }
}
