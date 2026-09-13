# CL-01 - visual check of the Unity slice on the desktop. THROWAWAY.
#
# Builds a Windows player (Mono, windowed 540x960), runs it with --shot, and reports where
# the engine-side capture landed. The capture comes from ScreenCapture inside the game, so
# it can only ever contain the game's own frame - never the desktop.
#
# ASCII only on purpose: Windows PowerShell 5.1 reads BOM-less UTF-8 as ANSI.

$ErrorActionPreference = "Stop"
$spike = Split-Path -Parent $MyInvocation.MyCommand.Path
$project = Join-Path $spike "unity"
$log = Join-Path $spike "unity_winshot.log"

$editor = Get-ChildItem "C:\Users\gianv\Unity\Editor" -Recurse -Filter "Unity.exe" -ErrorAction SilentlyContinue |
    Where-Object { $_.FullName -match "6000\.3" } | Select-Object -First 1
if (-not $editor) { Write-Host "Unity 6000.3 not found." -ForegroundColor Red; exit 1 }

# Unity.exe is a GUI-subsystem program; Start-Process -Wait is what actually waits for it.
if (Test-Path $log) { Remove-Item $log -Force }
$sw = [System.Diagnostics.Stopwatch]::StartNew()
$buildArgs = @("-quit", "-batchmode", "-nographics", "-projectPath", "`"$project`"",
               "-executeMethod", "SpikeBuild.BuildWindowsShot", "-logFile", "`"$log`"")
$build = Start-Process -FilePath $editor.FullName -ArgumentList $buildArgs -Wait -PassThru -NoNewWindow
$sw.Stop()
Write-Host ("windows build: exit {0}, {1:0.0} s" -f $build.ExitCode, $sw.Elapsed.TotalSeconds)
Select-String -Path $log -Pattern "error CS|\[spike\]" | Select-Object -First 20 | ForEach-Object { $_.Line }

$exe = Join-Path $project "build\win\CL01SpikeUnity.exe"
if (-not (Test-Path $exe)) { Write-Host "NO PLAYER at $exe - see $log" -ForegroundColor Red; exit 1 }

$shot = Join-Path $env:USERPROFILE "AppData\LocalLow\Gabella Games\CL-01 Spike Unity\shot.png"
if (Test-Path $shot) { Remove-Item $shot -Force }

$playerLog = Join-Path $spike "unity_winshot_player.log"
$runArgs = @("--shot", "-screen-width", "540", "-screen-height", "960", "-screen-fullscreen", "0",
             "-logFile", "`"$playerLog`"")
$run = Start-Process -FilePath $exe -ArgumentList $runArgs -PassThru
if (-not $run.WaitForExit(60000)) {
    Write-Host "player did not quit within 60 s; stopping it" -ForegroundColor Yellow
    Stop-Process -Id $run.Id -Force -Confirm:$false
}

Select-String -Path $playerLog -Pattern "\[spike\]|Exception|Error" -ErrorAction SilentlyContinue |
    Select-Object -First 10 | ForEach-Object { $_.Line }

if (Test-Path $shot) {
    Write-Host ("SHOT: {0} ({1:0} KB)" -f $shot, ((Get-Item $shot).Length / 1KB)) -ForegroundColor Green
} else {
    Write-Host "NO SHOT at $shot - see $playerLog" -ForegroundColor Red
    exit 1
}
