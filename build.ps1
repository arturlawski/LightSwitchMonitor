$ErrorActionPreference = "Stop"

dotnet publish "$PSScriptRoot\LightSwitchMonitor.csproj" -p:PublishProfile=SingleExe

Write-Host "Single-file build created:"
Write-Host "$PSScriptRoot\dist\LightSwitchMonitor.exe"
