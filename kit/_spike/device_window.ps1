# CL-01 - the attended device window, scripted. THROWAWAY.
#
# Everything here runs unattended once the phone is connected and authorised. Gian's
# part is three things: enable Developer options, turn on USB debugging, and accept the
# RSA prompt. See DEVICE_WINDOW.md.
#
#   powershell -ExecutionPolicy Bypass -File C:Devgabellacasual-labkit_spikedevice_window.ps1
#   (add -SkipUnity to leave the Unity APK out)

param(
    [switch]$SkipUnity,
    [int]$ColdStartRuns = 5
)

$ErrorActionPreference = "Stop"
$sdk = "$env:LOCALAPPDATA\Android\Sdk"
$adb = "$sdk\platform-tools\adb.exe"
$spike = Split-Path -Parent $MyInvocation.MyCommand.Path
$results = [ordered]@{}

function Say($msg) { Write-Host "`n=== $msg ===" -ForegroundColor Cyan }

# --- 0. wait for an authorised device ------------------------------------------------

Say "waiting for device"
& $adb start-server | Out-Null
$deadline = (Get-Date).AddMinutes(5)
while ((Get-Date) -lt $deadline) {
    $devices = & $adb devices | Select-Object -Skip 1 | Where-Object { $_ -match "\S" }
    if ($devices -match "\bdevice$") { break }
    if ($devices -match "unauthorized") {
        Write-Host "Device connected but UNAUTHORISED - accept the RSA prompt on the phone." -ForegroundColor Yellow
    }
    Start-Sleep -Seconds 3
}
$devices = & $adb devices | Select-Object -Skip 1 | Where-Object { $_ -match "\bdevice$" }
if (-not $devices) { throw "No authorised device after 5 minutes." }

$model = (& $adb shell getprop ro.product.model).Trim()
$release = (& $adb shell getprop ro.build.version.release).Trim()
$api = (& $adb shell getprop ro.build.version.sdk).Trim()
$abi = (& $adb shell getprop ro.product.cpu.abi).Trim()
$pageSize = (& $adb shell getconf PAGE_SIZE).Trim()
$results["device"] = "$model - Android $release (API $api) - $abi - page size $pageSize bytes"
Write-Host $results["device"] -ForegroundColor Green

# The 16 KB page-size question is only real on a device actually configured for it.
# Recording what this phone reports means the Report can say so rather than assume.

# --- helpers -------------------------------------------------------------------------

function Install-Apk($apkPath, $package) {
    if (-not (Test-Path $apkPath)) { Write-Host "missing APK: $apkPath" -ForegroundColor Yellow; return $false }
    & $adb uninstall $package 2>&1 | Out-Null
    $out = & $adb install -r $apkPath 2>&1
    if ($out -match "Success") { return $true }
    Write-Host "install failed: $out" -ForegroundColor Red
    return $false
}

# Launcher components are resolved ON THE DEVICE rather than hard-coded. A pre-flight
# check against the built APKs (2026-09-13) found two of three hard-coded names wrong:
#   Unity 6 launches com.unity3d.player.UnityPlayerGameActivity, not UnityPlayerActivity.
#   Godot 4.7 launches through an activity-alias, com.godot.game.GodotAppLauncher; the
#   GodotApp activity itself is exported=false, so adb am start is refused.
# A wrong name makes am start fail and the measurement silently come back empty.
function Get-LaunchComponent($package) {
    $out = & $adb shell cmd package resolve-activity --brief -c android.intent.category.LAUNCHER $package 2>&1
    $component = ($out | Where-Object { $_ -match "/" } | Select-Object -Last 1)
    if (-not $component) { Write-Host "no launcher found for $package" -ForegroundColor Red; return $null }
    return $component.ToString().Trim()
}

function Measure-ColdStart($package, $runs) {
    $component = Get-LaunchComponent $package
    if (-not $component) { return $null }
    Write-Host "  launching $component" -ForegroundColor DarkGray
    $samples = @()
    for ($i = 0; $i -lt $runs; $i++) {
        & $adb shell am force-stop $package | Out-Null
        Start-Sleep -Milliseconds 900
        $out = & $adb shell am start -W -n $component 2>&1
        $line = $out | Select-String "TotalTime:"
        if ($line) { $samples += [int](($line -split ":")[1].Trim()) }
    }
    if ($samples.Count -eq 0) { return $null }
    $sorted = $samples | Sort-Object
    return [ordered]@{
        median = $sorted[[math]::Floor($sorted.Count / 2)]
        min    = $sorted[0]
        max    = $sorted[-1]
        all    = ($samples -join ", ")
    }
}

function Test-Persistence($package, $label) {
    $component = Get-LaunchComponent $package
    if (-not $component) { return }
    # The point is a real process death, not an activity restart: play, force-stop,
    # relaunch, and see whether the best score came back.
    Write-Host "  $label - play until the shift fails, then press Enter" -ForegroundColor Yellow
    & $adb shell am start -n $component | Out-Null
    Read-Host "  press Enter once you have seen SHIFT FAILED"
    & $adb shell am force-stop $package | Out-Null
    Start-Sleep -Seconds 2
    & $adb shell am start -n $component | Out-Null
    Start-Sleep -Seconds 4
    Read-Host "  does the HUD show a non-zero BEST? press Enter after looking"
}

# --- 1. LIVE-RELOAD LOOPS FIRST (ruled by Gian, 2026-09-12) --------------------------
# This is the number most tied to the line's low-attention premise, and the one the
# desk phase could not estimate. It goes first in case the window is short.

Say "1. Capacitor live reload (over the USB cable)"
# Checked against Capacitor CLI 8.5.2's own help and source (2026-09-13):
#  - the flags are -l/--live-reload, --host, --port, --forwardPorts <port:port>; the earlier
#    --livereload / --external were wrong (--external is an Ionic CLI flag).
#  - --forwardPorts is handed to native-run as --forward, which sets up the USB port
#    forwarding, so the phone reaches the PC dev server over the cable and not over the LAN
#    (this PC is on a Public, guest-named network where devices may be isolated).
#  - cap run does NOT start a dev server, so Vite is started here.
#  - Ctrl+C is what makes Capacitor restore its config and manifest, but Ctrl+C would also
#    stop this whole script, so live reload runs in its own window.
$serial = (& $adb get-serialno).Trim()
Write-Host "  target device: $serial" -ForegroundColor DarkGray

$capDir = Join-Path $spike "capacitor"
$viteProc = Start-Process -FilePath "npx.cmd" -ArgumentList @("vite", "--host", "127.0.0.1", "--port", "5173", "--strictPort") -WorkingDirectory $capDir -WindowStyle Minimized -PassThru
$ready = $false
for ($i = 0; $i -lt 60; $i++) {
    try {
        $probe = Invoke-WebRequest -Uri "http://127.0.0.1:5173/" -UseBasicParsing -TimeoutSec 2
        if ($probe.StatusCode -eq 200) { $ready = $true; break }
    } catch { }
    Start-Sleep -Milliseconds 500
}
if (-not $ready) {
    Write-Host "  Vite dev server did not answer on 127.0.0.1:5173 within 30 s - skipping live reload" -ForegroundColor Red
} else {
    Write-Host "  Vite dev server up on http://127.0.0.1:5173" -ForegroundColor DarkGray
    $jdk21 = "C:\Program Files\Eclipse Adoptium\jdk-21.0.12.101-hotspot"
    $capCmd = "`$env:JAVA_HOME = '$jdk21'; `$env:ANDROID_HOME = '$sdk'; Set-Location '$capDir'; npx cap run android -l --host localhost --port 5173 --forwardPorts 5173:5173 --target $serial"
    Start-Process -FilePath "powershell.exe" -ArgumentList @("-NoExit", "-NoProfile", "-ExecutionPolicy", "Bypass", "-Command", $capCmd) | Out-Null
    Write-Host "  A second window is building and deploying the app with live reload." -ForegroundColor Yellow
    Write-Host "  When the app is on the phone: edit SHIFT_SECONDS in kit\_spike\capacitor\src\main.ts," -ForegroundColor Yellow
    Write-Host "  save, and time how long until the phone shows the new value. That is the loop." -ForegroundColor Yellow
    Write-Host "  Then press Ctrl+C in THAT window (so Capacitor restores its config), and come back here." -ForegroundColor Yellow
    Read-Host "  press Enter here once you have your number and stopped live reload"
}

Get-NetTCPConnection -LocalPort 5173 -State Listen -ErrorAction SilentlyContinue | ForEach-Object { Stop-Process -Id $_.OwningProcess -Force -ErrorAction SilentlyContinue }
if ($viteProc -and -not $viteProc.HasExited) { Stop-Process -Id $viteProc.Id -Force -ErrorAction SilentlyContinue }
Write-Host "  Vite dev server stopped" -ForegroundColor DarkGray

$capCfg = Join-Path $capDir "android\app\src\main\assets\capacitor.config.json"
if ((Test-Path $capCfg) -and ((Get-Content $capCfg -Raw) -match '"url"')) {
    Write-Host "  live-reload URL is still in the native config (window closed without Ctrl+C?) - restoring" -ForegroundColor Yellow
    Push-Location $capDir
    & npx cap sync android
    Pop-Location
}

Say "2. Godot one-click deploy / remote debug"
Write-Host "Godot's equivalent is the editor's Remote Deploy button with script hot-reload." -ForegroundColor Yellow
Write-Host "Open the editor on kit/_spike/godot, enable Remote Deploy, press Play, then edit" -ForegroundColor Yellow
Write-Host "SHIFT_SECONDS in main.gd and time the same thing." -ForegroundColor Yellow
Read-Host "press Enter when you have that number"

# --- 3. install and cold start -------------------------------------------------------

Say "2b. rebuild the Capacitor APK without live reload"
# cap run -l builds the APK with the live-reload URL (http://localhost:5173) baked into its
# packaged capacitor.config.json. Restoring the config afterwards does not change that APK, so
# installing it for cold start would time a WebView pointed at a dev server that has already
# been stopped. Rebuild from a clean production bundle first. (Found 2026-09-13, pre-flight.)
$capDir = Join-Path $spike "capacitor"
$env:JAVA_HOME = "C:\Program Files\Eclipse Adoptium\jdk-21.0.12.101-hotspot"
$env:ANDROID_HOME = $sdk
Push-Location $capDir
& npx vite build
& npx cap sync android
Push-Location (Join-Path $capDir "android")
& .\gradlew.bat assembleDebug --no-daemon -q
Pop-Location
Pop-Location
$packagedCfg = Join-Path $capDir "android\app\src\main\assets\capacitor.config.json"
if ((Test-Path $packagedCfg) -and ((Get-Content $packagedCfg -Raw) -match '"url"')) {
    Write-Host "  WARNING: packaged Capacitor config still has a server url - Capacitor cold start would be invalid" -ForegroundColor Red
} else {
    Write-Host "  Capacitor APK rebuilt without live reload" -ForegroundColor DarkGray
}

Say "3. cold start"

$godotApk = "$spike\godot\build\cl01-spike.apk"
if (Install-Apk $godotApk "com.gabella.cl01spike") {
    $results["godot_cold_start_ms"] = Measure-ColdStart "com.gabella.cl01spike" $ColdStartRuns
}

$capApk = "$spike\capacitor\android\app\build\outputs\apk\debug\app-debug.apk"
if (Install-Apk $capApk "com.gabella.cl01spikeweb") {
    $results["capacitor_cold_start_ms"] = Measure-ColdStart "com.gabella.cl01spikeweb" $ColdStartRuns
}

if (-not $SkipUnity) {
    $unityApk = "$spike\unity\build\cl01-spike-unity.apk"
    if (Install-Apk $unityApk "com.gabella.cl01spikeunity") {
        $results["unity_cold_start_ms"] = Measure-ColdStart "com.gabella.cl01spikeunity" $ColdStartRuns
    }
}

# --- 4. runtime errors ---------------------------------------------------------------

Say "4. logcat scan for runtime errors"
& $adb logcat -c
$godotComponent = Get-LaunchComponent "com.gabella.cl01spike"
if ($godotComponent) { & $adb shell am start -n $godotComponent | Out-Null }
Start-Sleep -Seconds 8
$results["godot_logcat_errors"] = (& $adb logcat -d -v brief *:E | Select-String -NotMatch "chatty|Scudo" | Select-Object -First 10) -join "`n"

# --- 5. persistence across a real process death --------------------------------------

Say "5. persistence"
Test-Persistence "com.gabella.cl01spike" "Godot"
Test-Persistence "com.gabella.cl01spikeweb" "Capacitor"
if (-not $SkipUnity) { Test-Persistence "com.gabella.cl01spikeunity" "Unity" }

# --- 6. report -----------------------------------------------------------------------

Say "results"
$out = "$spike\device_results.json"
$results | ConvertTo-Json -Depth 5 | Set-Content -Path $out -Encoding utf8
$results | Format-List
Write-Host "`nwritten to $out" -ForegroundColor Green
Write-Host "Drag feel, audio latency, timer drift and the sprite-count ceiling are read off" -ForegroundColor Yellow
Write-Host "the on-screen perf readout by hand - they are judgements, not numbers a script" -ForegroundColor Yellow
Write-Host "can take honestly." -ForegroundColor Yellow
