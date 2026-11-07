# Kit v0 - run a Unity build method in batch mode and enforce the package guards from
# outside the editor too.
#
#   powershell -ExecutionPolicy Bypass -File kit\tools\build_unity.ps1 -Project <unity project> -Method <Class.Method>
#
# What it adds to the in-editor guards (KitGuards.cs):
#  - the manifest and lock file are hashed before and after the run; if the editor changed
#    either while it was open, the run FAILS even if the build itself succeeded (the spike's
#    silent package additions happened on open, before any build code ran);
#  - Unity.exe is a GUI-subsystem program, so it is started with Start-Process and waited on
#    with WaitForExit (the & operator returns at once and once produced a 0.0 s "result"
#    from a stale log; -Wait hangs on leftover descendant processes).
#
# ASCII only on purpose: Windows PowerShell 5.1 reads BOM-less UTF-8 as ANSI.
param(
    [Parameter(Mandatory = $true)][string]$Project,
    [Parameter(Mandatory = $true)][string]$Method,
    [string]$Log = "",
    [string]$EditorVersion = "6000.3.24f1"
)

$ErrorActionPreference = "Stop"
$Project = (Resolve-Path $Project).Path
if (-not $Log) { $Log = Join-Path $Project "Logs\kit_build.log" }
New-Item -ItemType Directory -Force (Split-Path -Parent $Log) | Out-Null

$editor = Get-ChildItem "C:\Users\gianv\Unity\Editor", "C:\Program Files\Unity\Hub\Editor" `
    -Recurse -Filter "Unity.exe" -ErrorAction SilentlyContinue |
    Where-Object { $_.FullName -match [regex]::Escape($EditorVersion) } | Select-Object -First 1
if (-not $editor) { Write-Host "Unity $EditorVersion not found." -ForegroundColor Red; exit 2 }

function Get-PkgHash($p) {
    $files = @("Packages\manifest.json", "Packages\packages-lock.json") | ForEach-Object { Join-Path $p $_ }
    ($files | ForEach-Object { if (Test-Path $_) { (Get-FileHash $_ -Algorithm SHA256).Hash } else { "absent" } }) -join "/"
}

$before = Get-PkgHash $Project
$beforeManifest = Get-Content (Join-Path $Project "Packages\manifest.json") -Raw
if (Test-Path $Log) { Remove-Item $Log -Force }
$sw = [System.Diagnostics.Stopwatch]::StartNew()
$argList = @("-quit", "-batchmode", "-nographics", "-projectPath", "`"$Project`"",
             "-executeMethod", $Method, "-logFile", "`"$Log`"")
$proc = Start-Process -FilePath $editor.FullName -ArgumentList $argList -PassThru -NoNewWindow
# Wait on the editor process itself. Start-Process -Wait also waits for every descendant,
# and the editor can leave one behind (it pinged Unity Hub on 2026-11-07), which hung the
# script for 10+ minutes after a finished build.
$handle = $proc.Handle   # keep a handle so ExitCode stays readable after exit
$proc.WaitForExit()
$sw.Stop()
$exit = $proc.ExitCode
$after = Get-PkgHash $Project

Write-Host "`n=== compile errors, kit lines ===" -ForegroundColor Cyan
Select-String -Path $Log -Pattern "error CS|Compilation failed|\[kit\]|\[fd\]|BuildFailedException" |
    Select-Object -First 60 | ForEach-Object { $_.Line }

$guardFail = $false
$afterManifest = Get-Content (Join-Path $Project "Packages\manifest.json") -Raw
if ($afterManifest -ne $beforeManifest) {
    $guardFail = $true
    Write-Host "`n[kit] guard (c): FAIL the editor rewrote Packages/manifest.json during this run" -ForegroundColor Red
    Write-Host "Check Logs/Packages-Update.log, restore the manifest from git, and rebuild."
} elseif ($after -ne $before) {
    Write-Host "`n[kit] note: packages-lock.json changed during this run (first resolve or a pinned upgrade); commit it if intended." -ForegroundColor Yellow
}

Write-Host ("`nexit code {0}, wall clock {1:0.0} s, log {2}" -f $exit, $sw.Elapsed.TotalSeconds, $Log)
if ($guardFail) { exit 3 }
exit $exit
