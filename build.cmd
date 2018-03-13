@echo Off
set config=%1
if "%config%" == "" (
   set config=Release
)

dotnet restore .\src\SoundFingerprinting.Audio.Bass.sln
dotnet test .\src\SoundFingerprinting.Audio.Bass.Tests\SoundFingerprinting.Audio.Bass.Tests.csproj -c %config%
dotnet pack .\src\SoundFingerprinting.Audio.Bass\SoundFingerprinting.Audio.Bass.csproj -c %config% -o ..\..\build -v n