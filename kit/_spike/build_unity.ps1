# CL-01 — build the Unity slice once the editor is installed and licensed. THROWAWAY.
#
# Run this after Unity Hub has signed in and installed 6000.3.24f1 with Android Build
# Support. Everything before that needs UAC and a Unity account, neither of which an
# unattended session can supply. See DEVICE_WINDOW.md.

$ErrorActionPreference = "Stop"
$spike = Split-Path -Parent $MyInvocation.MyCommand.Path
$project = Join-Path $spike "unity"

$editor = Get-ChildItem "C:\Users\gianv\Unity\Editor", "C:\Program Files\Unity\Hub\Editor" `
    -Recurse -Filter "Unity.exe" -ErrorAction SilentlyContinue |
    Where-Object { $_.FullName -match "6000\.3" } |
    Select-Object -First 1

if (-not $editor) {
    Write-Host "Unity 6000.3.24f1 not found. Install it in Unity Hub first (see DEVICE_WINDOW.md)." -ForegroundColor Red
    exit 1
}
Write-Host "editor: $($editor.FullName)" -ForegroundColor Cyan

$log = Join-Path $spike "unity_build.log"
$sw = [System.Diagnostics.Stopwatch]::StartNew()

& $editor.FullName -quit -batchmode -nographics `
    -projectPath $project `
    -executeMethod SpikeBuild.BuildAndroid `
    -logFile $log
$exit = $LASTEXITCODE
$sw.Stop()

Write-Host "`n=== compile errors and warnings ===" -ForegroundColor Cyan
Select-String -Path $log -Pattern "error CS|Compilation failed|\[spike\]" |
    Select-Object -First 40 |
    ForEach-Object { $_.Line }

Write-Host "`n=== result ===" -ForegroundColor Cyan
Write-Host ("exit code:  {0}" -f $exit)
Write-Host ("wall clock: {0:0.0} s" -f $sw.Elapsed.TotalSeconds)

$apk = Join-Path $project "build\cl01-spike-unity.apk"
if (Test-Path $apk) {
    Write-Host ("APK:        {0:0.00} MB" -f ((Get-Item $apk).Length / 1MB)) -ForegroundColor Green
    Write-Host "`nCompare against: Godot 26.96 MB debug / 25.21 MB release, Capacitor 4.34 MB debug."

    # Same compliance checks the other two got, so the three rows are comparable.
    $bt = "$env:LOCALAPPDATA\Android\Sdk\build-tools\36.0.0"
    if (Test-Path "$bt\aapt2.exe") {
        Write-Host "`n=== manifest ===" -ForegroundColor Cyan
        & "$bt\aapt2.exe" dump badging $apk 2>&1 |
            Select-String "^package:|sdkVersion|native-code" | Select-Object -First 5
        Write-Host "`n=== 16 KB page alignment ===" -ForegroundColor Cyan
        & "$bt\zipalign.exe" -c -P 16 -v 4 $apk 2>&1 | Select-String "\.so|Verification"
    }
} else {
    Write-Host "NO APK produced — see $log" -ForegroundColor Red
}

Write-Host "`nFull log: $log"
