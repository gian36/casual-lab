#!/usr/bin/env bash
# CL-01 - automated on-device drag + persistence test. THROWAWAY, like all of kit/_spike.
#
# Real touch input via `adb shell input swipe`: every tray button onto every lane, three sweeps
# inside the 45 s shift; then the shift runs out, the app is force-stopped, the stored best
# score is read from app storage, and the app is relaunched. Screenshots at each stage.
# Coordinates were read off on-device screenshots of the S24 Ultra (1440x3120), not computed.
#
#   bash kit/_spike/device_autotest.sh godot|capacitor|unity [outdir]

set -u
engine="${1:?usage: device_autotest.sh godot|capacitor|unity [outdir]}"
out="${2:-./devtest}"
adb="/c/Android/Sdk/platform-tools/adb.exe"
mkdir -p "$out"

case "$engine" in
  godot)     pkg=com.gabella.cl01spike;      tray_y=2306; lanes="516 958 1399 1842" ;;
  capacitor) pkg=com.gabella.cl01spikeweb;   tray_y=2586; lanes="786 1207 1629 2050" ;;
  unity)     pkg=com.gabella.cl01spikeunity; tray_y=2306; lanes="507 928 1349 1771" ;;
  *) echo "unknown engine: $engine"; exit 1 ;;
esac
tray_xs="195 544 894 1243"
lane_x=718

launch() {
  local comp
  comp=$("$adb" shell cmd package resolve-activity --brief -c android.intent.category.LAUNCHER "$pkg" | tr -d '\r' | tail -1)
  "$adb" shell am start -n "$comp" >/dev/null 2>&1
}

stored_best() {
  case "$engine" in
    godot)     "$adb" shell run-as "$pkg" cat files/best.cfg 2>&1 | tr -d '\r' | tr '\n' ' ' ;;
    unity)     "$adb" shell "run-as $pkg cat shared_prefs/$pkg.v2.playerprefs.xml" 2>&1 | tr -d '\r' | grep -o 'name="cl01.best" value="[0-9]*"' ;;
    capacitor) "$adb" exec-out "run-as $pkg find app_webview -path '*Local?Storage/leveldb*' -type f -exec cat {} +" 2>/dev/null \
                 | LC_ALL=C grep -a -o "cl01\.best.\{0,6\}" | LC_ALL=C tr -c '[:print:]' '.' | sort -u | tr '\n' ' ' ;;
  esac
}

echo "=== $engine: start $(date +%H:%M:%S) ==="
echo "  stored best before run: $(stored_best)"
"$adb" shell am force-stop "$pkg"
launch
"$adb" shell sleep 3
t0=$(date +%s)
for round in 1 2 3; do
  for ly in $lanes; do
    for tx in $tray_xs; do
      "$adb" shell input swipe "$tx" "$tray_y" "$lane_x" "$ly" 450
    done
  done
  "$adb" exec-out screencap -p > "$out/${engine}_round${round}.png"
  echo "  round $round done at +$(( $(date +%s) - t0 ))s"
done
while [ $(( $(date +%s) - t0 )) -lt 52 ]; do "$adb" shell sleep 2; done
"$adb" exec-out screencap -p > "$out/${engine}_fail.png"
echo "  fail screen captured at +$(( $(date +%s) - t0 ))s"
echo "  stored best BEFORE force-stop: $(stored_best)"
"$adb" shell sleep 3
"$adb" shell am force-stop "$pkg"
"$adb" shell sleep 2
echo "  stored best AFTER force-stop:  $(stored_best)"
launch
"$adb" shell sleep 5
"$adb" exec-out screencap -p > "$out/${engine}_relaunch.png"
echo "  relaunch screen captured; done $(date +%H:%M:%S)"
