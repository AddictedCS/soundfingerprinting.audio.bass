## Un4seen.Bass audio integration with SoundFingerprinting
Bass module for SoundFingerprinting framework. Comes as a better substitute for **NAudio** audio library in [soundfingerprinting](https://github.com/AddictedCS/soundfingerprinting) algorithm.
It is a faster and more reliable counterpart that support a wide variaty of audio extensions. Please make sure you comply to un4seen license terms. Check un4seen [homepage](http://www.un4seen.com) for details regarding commercial projects. 

### Binaries
    git clone git@github.com:AddictedCS/soundfingerprinting.audio.bass.git
    
In order to build latest version of the **SoundFingerprinting.Audio.Bass** assembly run the following command from repository root

    .\build.cmd

### Running on Linux (.NET Core and Mono)

| HINT: Use `dotnet-sdk-2.1.202` instead of `dotnet-sdk-2.2` to avoid https://github.com/dotnet/core/issues/2540 |
| --- |

Install the BASS modules once into `/usr/lib/bass`. The following was tested on `Ubuntu 18.04.2 x64`:

	$ cd soundfingerprinting.audio.bass/
	$ sudo mkdir /usr/lib/bass
	$ sudo cp src/SoundFingerprinting.Audio.Bass/x64/*.so /usr/lib/bass
	$ sudo bash -c 'echo /usr/lib/bass >> /etc/ld.so.conf.d/bass.conf'
	$ sudo ldconfig
	$ ldconfig --print-cache | grep libbass
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
	
### Get it on NuGet

    Install-Package SoundFingerprinting.Audio.Bass
    
### Contribute
If you want to contribute you are welcome to open issues or discuss on [issues](https://github.com/AddictedCS/soundfingerprinting/issues) page. Feel free to contact me for any remarks, ideas, bug reports etc. 

### Licence
The framework is provided under [MIT](https://opensource.org/licenses/MIT) licence agreement. Linked Bass DLLs from un4seen are provided with a different license. Please check http://un4seen.com website for details.
