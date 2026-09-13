#!/usr/bin/env bash
# CL-01 - edit -> running-on-device loop. THROWAWAY.
#
# For each engine: change SHIFT_SECONDS 45 -> 55 (visible on the timer), then time
# build/export + adb install -r + launch, screenshot; revert 55 -> 45 and time it again.
# Measures the plain rebuild loop every engine has - not hot reload.
#
# Every build step is checked: the APK's modification time must change, or the run is marked
# INVALID. (The first run installed a stale Capacitor APK because Gradle never started; only the
# screenshot caught it.)
#
#   bash kit/_spike/iteration_loop.sh [outdir] [engines...]     e.g.  ./loop capacitor

set -u
out="${1:-./loop}"
shift || true
engines="${*:-godot capacitor unity}"
mkdir -p "$out"
spike=/c/Dev/gabella/casual-lab/kit/_spike
adb=/c/Android/Sdk/platform-tools/adb.exe
godot=/c/Users/gianv/AppData/Local/Microsoft/WinGet/Packages/GodotEngine.GodotEngine_Microsoft.Winget.Source_8wekyb3d8bbwe/Godot_v4.7.2-stable_win64_console.exe
jdk21="$(cygpath -w '/c/Program Files/Eclipse Adoptium/jdk-21.0.12.101-hotspot')"
export ANDROID_HOME="$(cygpath -w /c/Android/Sdk)"

now() { date +%s.%N; }
secs() { awk -v a="$1" -v b="$2" 'BEGIN { printf "%.1f", b - a }'; }
mtime() { [ -f "$1" ] && date -r "$1" +%s.%N || echo 0; }

check_rebuilt() {
  if [ "$1" = "$2" ]; then echo "    APK rebuilt: NO - run INVALID"; else echo "    APK rebuilt: yes"; fi
}

install_launch() {
  local pkg="$1" apk="$2" label="$3" t_i t_l t_end comp
  t_i=$(now)
  "$adb" install -r "$(cygpath -w "$apk")" 2>&1 | tr -d '\r' | tail -1 | sed 's/^/    install: /'
  t_l=$(now)
  comp=$("$adb" shell cmd package resolve-activity --brief -c android.intent.category.LAUNCHER "$pkg" | tr -d '\r' | tail -1)
  "$adb" shell am force-stop "$pkg"
  "$adb" shell am start -W -n "$comp" | tr -d '\r' | grep -E "TotalTime" | sed 's/^/    launch /'
  t_end=$(now)
  "$adb" shell sleep 2
  "$adb" exec-out screencap -p > "$out/${label}.png"
  echo "    install $(secs "$t_i" "$t_l") s | launch step $(secs "$t_l" "$t_end") s"
}

loop_godot() {
  local from="$1" to="$2" apk="$spike/godot/build/cl01-spike.apk" before t0 t1 t2
  echo "--- godot: SHIFT_SECONDS $from -> $to ---"
  sed -i "s/const SHIFT_SECONDS := ${from}.0/const SHIFT_SECONDS := ${to}.0/" "$spike/godot/main.gd"
  grep -c "SHIFT_SECONDS := ${to}.0" "$spike/godot/main.gd" | sed 's/^/    edited lines: /'
  before=$(mtime "$apk"); t0=$(now)
  "$godot" --headless --path "$(cygpath -w "$spike/godot")" --export-debug Android "$(cygpath -w "$apk")" 2>&1 | grep -E "ERROR" | grep -v "project icon" | head -3
  t1=$(now)
  echo "    export $(secs "$t0" "$t1") s"; check_rebuilt "$before" "$(mtime "$apk")"
  install_launch com.gabella.cl01spike "$apk" "godot_${to}"
  t2=$(now); echo "    GODOT EDIT->ON DEVICE TOTAL $(secs "$t0" "$t2") s"
}

loop_capacitor() {
  local from="$1" to="$2" apk="$spike/capacitor/android/app/build/outputs/apk/debug/app-debug.apk" before t0 t1 t2 t3
  echo "--- capacitor: SHIFT_SECONDS $from -> $to ---"
  sed -i "s/const SHIFT_SECONDS = ${from}.0;/const SHIFT_SECONDS = ${to}.0;/" "$spike/capacitor/src/main.ts"
  grep -c "SHIFT_SECONDS = ${to}.0;" "$spike/capacitor/src/main.ts" | sed 's/^/    edited lines: /'
  before=$(mtime "$apk"); t0=$(now)
  (cd "$spike/capacitor" && npx vite build >/dev/null 2>&1 && npx cap sync android >/dev/null 2>&1)
  t1=$(now)
  (cd "$spike/capacitor/android" && JAVA_HOME="$jdk21" powershell -NoProfile -Command "& (Join-Path (Get-Location) 'gradlew.bat') assembleDebug -q" 2>&1 | tail -5)
  t2=$(now)
  echo "    vite+sync $(secs "$t0" "$t1") s | gradle $(secs "$t1" "$t2") s"; check_rebuilt "$before" "$(mtime "$apk")"
  install_launch com.gabella.cl01spikeweb "$apk" "capacitor_${to}"
  t3=$(now); echo "    CAPACITOR EDIT->ON DEVICE TOTAL $(secs "$t0" "$t3") s"
}

loop_unity() {
  local from="$1" to="$2" apk="$spike/unity/build/cl01-spike-unity.apk" before t0 t1 t2
  echo "--- unity: ShiftSeconds $from -> $to (incremental build) ---"
  sed -i "s/private const float ShiftSeconds = ${from}f;/private const float ShiftSeconds = ${to}f;/" "$spike/unity/Assets/Scripts/SpikeBootstrap.cs"
  grep -c "ShiftSeconds = ${to}f;" "$spike/unity/Assets/Scripts/SpikeBootstrap.cs" | sed 's/^/    edited lines: /'
  before=$(mtime "$apk"); t0=$(now)
  SPIKE_INCREMENTAL=1 powershell -NoProfile -ExecutionPolicy Bypass -File "$(cygpath -w "$spike/build_unity.ps1")" 2>&1 | grep -E "\[spike\]|error CS|exit code"
  t1=$(now)
  echo "    unity build $(secs "$t0" "$t1") s"; check_rebuilt "$before" "$(mtime "$apk")"
  install_launch com.gabella.cl01spikeunity "$apk" "unity_${to}"
  t2=$(now); echo "    UNITY EDIT->ON DEVICE TOTAL $(secs "$t0" "$t2") s"
}

echo "=== iteration loop start $(date +%H:%M:%S) - engines: $engines ==="
for e in $engines; do
  "loop_$e" 45 55
  "loop_$e" 55 45
done
echo "=== iteration loop end $(date +%H:%M:%S) ==="
