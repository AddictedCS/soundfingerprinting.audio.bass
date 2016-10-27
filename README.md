## soundfingerprinting.audio.bass
Bass module for Sound Fingerprinting framework. Comes as a better substitute for <code>NAudio</code> audio library in [soundfingerprinting](https://github.com/AddictedCS/soundfingerprinting) algorithm.
It is a more advanced and reliable counterpart, though its not free. Please check their [homepage](http://www.un4seen.com) before using it. In case you have Bass registration key you can specify it in <code>SoundFingerprinting</code> framework by adding them in your application configuration file:

```xml
<configuration>
  <configSections>
    <section name="BassConfigurationSection" type="SoundFingerprinting.Audio.Bass.BassConfigurationSection, SoundFingerprinting.Audio.Bass" />
  </configSections>

  <BassConfigurationSection email = "email" registrationKey = "registration-key" />
</configuration>
```
