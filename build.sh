#!/bin/sh

if [ -z "$1" ]; then
    TARGET="Release"
else
    TARGET=$1    
fi

nuget install NUnit.Runners -Version 3.5.0 -OutputDirectory build/testrunner

msbuild src/SoundFingerprinting.Audio.Bass.sln /t:Restore
msbuild src/SoundFingerprinting.Audio.Bass.sln /p:Configuration=$TARGET

dotnet test ./src/SoundFingerprinting.Audio.Bass.Tests/SoundFingerprinting.Audio.Bass.Tests.csproj -c $TARGET -netcoreapp2.0 -v n

mono build/testrunner/NUnit.ConsoleRunner.3.5.0/tools/nunit3-console.exe src/SoundFingerprinting.Audio.Bass.Tests/bin/$TARGET/net461/SoundFingerprinting.Audio.Bass.Tests.dll

msbuild /t:Pack src/SoundFingerprinting.Audio.Bass/SoundFingerprinting.Audio.Bass.csproj /p:PackageOutputPath=../../build