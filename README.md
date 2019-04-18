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
Add a `BASS_HOME` environment variable which points to the path where BASS *.so modules reside. Add the same path to `LD_LIBRARY_PATH`, e.g.:

    $ export BASS_HOME=/home/user/Projects/soundfingerprinting.audio.bass/src/SoundFingerprinting.Audio.Bass.Tests/bin/Release/netcoreapp2.0/x64
    $ export LD_LIBRARY_PATH=$LD_LIBRARY_PATH:$BASS_HOME
    
Alternatively, consider installing the BASS modules once to `/usr/lib/bass`. For example, the following was tested on `Ubuntu 18.04.2 x64`:

	$ cd soundfingerprinting.audio.bass/
	$ sudo mkdir /usr/lib/bass
	$ sudo cp src/SoundFingerprinting.Audio.Bass/x64/*.so /usr/lib/bass
	$ sudo bash -c 'echo /usr/lib/bass >> /etc/ld.so.conf.d/bass.conf'
	$ sudo ldconfig
	$ sudo ldconfig --print-cache | grep libbass
		libbassmix.so (libc6,x86-64) => /usr/lib/bass/libbassmix.so
		libbass.so (libc6,x86-64) => /usr/lib/bass/libbass.so
	$ echo '
	> # un4seen BASS library:
	> export BASS_HOME=/usr/lib/bass' >> ~/.profile
	# setting LD_LIBRARY_PATH in this case is not necessary
	# BASS_HOME will be visible after next logout-login, but until then:
	$ source ~/.profile
	$ echo $BASS_HOME
	/usr/lib/bass
	$ ./build.sh
    
### Contribute
If you want to contribute you are welcome to open issues or discuss on [issues](https://github.com/AddictedCS/soundfingerprinting/issues) page. Feel free to contact me for any remarks, ideas, bug reports etc. 

### Licence
The framework is provided under [MIT](https://opensource.org/licenses/MIT) licence agreement. Linked Bass DLLs from un4seen are provided with a different license. Please check http://un4seen.com website for details.
