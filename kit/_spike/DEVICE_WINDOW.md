# DEVICE_WINDOW.md — what to do when the phone is at the desk

**THROWAWAY**, like everything in `kit/_spike/`. Written so the attended part of CL-01 is
short: your minutes go to the phone and to judging feel, not to setup.

Device: **Samsung Galaxy S24 Ultra**.

> **Recorded for the Report:** the S24 Ultra is a flagship. The brief asks for cold start
> on a **mid-range** device. Every number from this phone will flatter all three candidates
> and none of them will answer the question the brief actually asked. This is raised as a
> proposed `[OPEN]` for Cowork: *which device is this line's reference target?*

## Unity gates: done (2026-09-13)

Gian created a Unity account, signed in to Hub and activated **Personal**; the Android
module then installed without a UAC prompt, and the Unity slice builds with
`build_unity.ps1`. Nothing Unity-specific is left before the phone.

## Your part — about three minutes

**First, once per PC:** install the **Samsung Android USB Driver for Windows** (v1.9.5.0,
35.5 MB, 2026-05-06) from `developer.samsung.com/android-usb-driver`, accepting its UAC prompt.
Without it, Windows attaches the phone's ADB interface to a generic WinUSB driver and
`adb devices` shows nothing at all — not even `unauthorized`. Found on this PC 2026-09-13.

1. **Settings → About phone → Software information**, tap **Build number** seven times.
2. **Settings → Developer options** → turn on **USB debugging**.
3. Plug the phone into this machine with a **data** cable (a charge-only cable will show
   nothing).
4. A dialog appears on the phone: *"Allow USB debugging?"* → tick **Always allow from this
   computer** → **Allow**.

Then, in a terminal here:

```bash
powershell -ExecutionPolicy Bypass -File C:Devgabellacasual-labkit_spikedevice_window.ps1
```

Use the full path: it works from any folder the terminal happens to be open in.

Add `-SkipUnity` if the Unity build has not happened yet.

## What the script does, in the order Gian ruled (live-reload first)

| # | Step | Attended? |
|---|---|---|
| 1 | **Capacitor live reload, over the USB cable** — the script starts Vite and opens a **second window** running `cap run android -l --host localhost --port 5173 --forwardPorts 5173:5173`; you edit one constant, time how long until the phone shows it, then press **Ctrl+C in that second window** and Enter in the first | yes, you time it |
| 2 | **Godot remote deploy** — same edit, same timing, via the editor's Remote Deploy with script hot-reload | yes, you time it |
| 3 | **Cold start** — installs each APK, then 5 × `force-stop` + `am start -W`, reporting median/min/max `TotalTime` | no |
| 4 | **Logcat scan** — launches and collects any error-level output | no |
| 5 | **Persistence** — play until SHIFT FAILED, real `force-stop`, relaunch, check BEST survived | yes, two prompts |
| 6 | **Results** — writes `device_results.json` | no |

It also records what the phone reports for `PAGE_SIZE`, so the Report can say whether the
16 KB question was tested on a device actually configured for 16 KB pages rather than
assumed.

## What the script deliberately does not measure

**Drag feel, audio latency, timer drift and the sprite-count ceiling.** The slice shows all
four on screen — `fps`, `frame max`, `drag max`, `drift`, `planes` — and the honest way to
take them is to drag for a few seconds, tap `stress +100` until the numbers move, and look.
A script could print those figures but not the judgement, and the judgement is the point:
*does dragging a fuel truck onto a lane feel immediate on this phone, in each engine?*

That judgement is yours. It is the one measurement in CL-01 that Claude cannot take.

## If something goes wrong

- `adb devices` shows **`unauthorized`** → the RSA prompt was missed or dismissed. Unplug,
  replug, watch the phone screen.
- `adb devices` shows **nothing** → charge-only cable, or USB debugging is off.
- An install fails with **`INSTALL_FAILED_UPDATE_INCOMPATIBLE`** → a previous build with the
  same package name and a different signature is on the phone; the script uninstalls first,
  but if it persists, remove the app by hand.
- Live reload goes over the **USB cable** (`adb reverse`), not the network, so the guest
  network this PC is on does not matter.
- Stop live reload with **Ctrl+C in its own window**, never in the main one: Ctrl+C in the main
  window ends the whole script. If the second window is closed some other way, the script
  notices and restores Capacitor's config with `cap sync`.
- `adb devices` shows **nothing** even with USB debugging on → the Samsung USB driver is
  missing (see the top of "Your part").
