#!/bin/sh

if [ -z "$1" ]; then
    TARGET="Release"
else
    TARGET=$1    
fi

nuget install NUnit.Runners -Version 3.5.0 -OutputDirectory build/testrunner
msbuild src/SoundFingerprinting.Audio.Bass.sln /p:Configuration=Release

dotnet test ./src/SoundFingerprinting.Audio.Bass.Tests/SoundFingerprinting.Audio.Bass.Tests.csproj -c Release -f netcoreapp2.0

mono build/testrunner/NUnit.ConsoleRunner.3.5.0/tools/nunit3-console.exe src/SoundFingerprinting.Audio.Bass.Tests/bin/Release/net461/SoundFingerprinting.Audio.Bass.Tests.dll

msbuild /t:Pack src/SoundFingerprinting.Audio.Bass/SoundFingerprinting.Audio.Bass.csproj /p:PackageOutputPath=../../build
