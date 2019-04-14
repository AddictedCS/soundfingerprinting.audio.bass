#Bass.Net audio integration with SoundFingerprinting
Bass module for Sound Fingerprinting framework. Comes as a better substitute for **NAudio** audio library in [soundfingerprinting](https://github.com/AddictedCS/soundfingerprinting) algorithm.
It is a more advanced and reliable counterpart, though its not free. Please check their [homepage](http://www.un4seen.com) before using it. In case you have Bass registration key you can specify it in **SoundFingerprinting** framework by adding them in your appsettings.json file:

```json
{
   "email": "license-email",
   "registrationKey": "registration-key"
}
```

### Binaries
    git clone git@github.com:AddictedCS/soundfingerprinting.audio.bass.git
    
In order to build latest version of the **SoundFingerprinting.Audio.Bass** assembly run the following command from repository root

    .\build.cmd
### Get it on NuGet

    Install-Package SoundFingerprinting.Audio.Bass
    
### Contribute
If you want to contribute you are welcome to open issues or discuss on [issues](https://github.com/AddictedCS/soundfingerprinting/issues) page. Feel free to contact me for any remarks, ideas, bug reports etc. 

### Licence
The framework is provided under [MIT](https://opensource.org/licenses/MIT) licence agreement.