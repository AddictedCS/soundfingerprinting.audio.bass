## Un4seen.Bass audio integration with SoundFingerprinting
Bass module for SoundFingerprinting framework. Comes as a better substitute for **NAudio** audio library in [soundfingerprinting](https://github.com/AddictedCS/soundfingerprinting) algorithm.
It is a faster and more reliable counterpart that support a wide variaty of audio extensions. Please make sure you comply to un4seen license terms. Check un4seen [homepage](http://www.un4seen.com) for details regarding commercial projects. 

### Binaries
    git clone git@github.com:AddictedCS/soundfingerprinting.audio.bass.git
    
In order to build latest version of the **SoundFingerprinting.Audio.Bass** assembly run the following command from repository root

    .\build.cmd
### Get it on NuGet

    Install-Package SoundFingerprinting.Audio.Bass
	
### .NET Core on Linux
Add a BASS_HOME environment variable which points to the path where BASS *.so modules reside. Add the same path to LD_LIBRARY_PATH, e.g.:

    $ export BASS_HOME=/home/user/Projects/soundfingerprinting.audio.bass/src/SoundFingerprinting.Audio.Bass.Tests/bin/Release/netcoreapp2.0/x64
    $ export LD_LIBRARY_PATH=$LD_LIBRARY_PATH:$BASS_HOME
    
### Contribute
If you want to contribute you are welcome to open issues or discuss on [issues](https://github.com/AddictedCS/soundfingerprinting/issues) page. Feel free to contact me for any remarks, ideas, bug reports etc. 

### Licence
The framework is provided under [MIT](https://opensource.org/licenses/MIT) licence agreement. Linked Bass DLLs from un4seen are provided with a different license. Please check http://un4seen.com website for details.