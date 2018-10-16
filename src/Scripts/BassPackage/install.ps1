param($installPath, $toolsPath, $package, $project)

$dlls = @("bass.dll", "bassflac.dll", "bassmix.dll", "tags.dll")
$dirs = @("x64", "x86")

foreach ($dir in $dirs)
{
	foreach ($dll in $dlls)
	{
		$item = $project.ProjectItems.Item($dir).ProjectItems.Item($dll)
		$item.Properties.Item("BuildAction").Value = 2
		$item.Properties.Item("CopyToOutputDirectory").Value = 2
	}
}

$osxDlls = @("libbass.dylib", "libbassmix.dylib")
$osxDir = "osx"

foreach ($dll in $osxDlls)
{
	$item = $project.ProjectItems.Item($osxDir).ProjectItems.Item($dll)
	$item.Properties.Item("BuildAction").Value = 2
	$item.Properties.Item("CopyToOutputDirectory").Value = 2
}