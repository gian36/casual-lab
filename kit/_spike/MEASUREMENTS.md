# MEASUREMENTS.md — CL-01, raw log

Kept live, in the order things happened, so the Report's productivity numbers are a
record rather than a reconstruction. **Working notes. THROWAWAY.**

Machine: Windows 11 Pro 26200, AMD Radeon 780M (integrated), 717 GB free.
Starting state: **no** Godot, **no** Unity, **no** JDK, **no** Android SDK, **no** adb.
Node 24.18 and git were the only relevant tools present.

## Godot — toolchain install

| Step | Result | Notes |
|---|---|---|
| `winget install GodotEngine.GodotEngine` | OK, 4.7.2 exactly | winget carried the exact current stable. No admin needed; it warned the `godot` PATH alias could not be symlinked without admin, so the exe is called by full path throughout |
| `winget install EclipseAdoptium.Temurin.17.JDK` | OK, 17.0.20.1+1 | JDK 17 is what Godot 4.7's `config.gradle` declares (`javaVersion VERSION_17`) |
| Android command-line tools (direct zip from dl.google.com) | OK, ~150 MB | `Invoke-WebRequest` was slow (several minutes); `curl.exe` was much faster and is the better choice for the rest |
| Android SDK packages (`platform-tools`, `build-tools;36.0.0`, `platforms;android-36`) | **BLOCKED** | `sdkmanager --licenses` would not accept piped input; 7 package licences remain unaccepted. Licence acceptance is Gian's to give — raised in chat, not self-approved |
| Debug keystore (`keytool`) | OK | `~/.android/debug.keystore`, alias `androiddebugkey`, 9999 days. No release keystore created: CL-01 does not need one and it is not the spike's to make |
| Godot editor settings wired | OK | `java_sdk_path`, `android_sdk_path`, `debug_keystore{,_user,_pass}` written into `editor_settings-4.7.tres`. Godot had already auto-detected the SDK path on first launch |
| Export templates 4.7.2 (~1 GB `.tpz`) | downloaded | needed even for a debug APK |

**Finding for the productivity axis:** the entire Godot toolchain is reachable from a
package manager plus two direct downloads, with **no account, no sign-in, and no licence
server**. The only human-gated step is the Android SDK licence, which is Google's and
would apply to every candidate equally.

## Godot — writing the slice

Authored **before** the editor was ever opened, as plain text (`project.godot`,
`main.tscn`, and five `.gd` files). The editor has still not been opened interactively;
everything below went through `--headless`.

| # | What happened | Cost |
|---|---|---|
| 1 | **Correction 1 (only one so far).** `const SERVICE_NAMES := PackedStringArray([...])` — GDScript rejected it: *"Assigned value for constant 'SERVICE_NAMES' isn't a constant expression."* Fixed by using a typed `Array[String]` const and matching the field type in `lane_panel.gd`. | ~2 min, one edit in each of two files |
| 2 | Second `--import`: clean. No warnings. | 3.6 s for a cold first import |
| 3 | `--headless --quit-after 240` (4 s of real frames): **no errors at all** — scene tree, `%`-unique-name lookups, procedural `AudioStreamWAV` generation and the whole loop ran first time. | — |
| 4 | First windowed run with an engine-side viewport capture: **correct on the first visual attempt.** Portrait 9:16, four lanes, aircraft nose-left, need icons with labels, gate lights, countdown, HUD, tray, stress button — all as intended. | — |

**Error-message quality (brief's column):** the single failure gave the exact file, exact
line, the constant's name, and the actual rule that was broken. No guessing was needed.

**Code Claude wrote unaided:** all of it. No sample project, no scaffold, no editor.
~430 lines of GDScript plus a hand-written `.tscn`.

**One process note that is not to Godot's credit or discredit:** a desktop screen-grab was
used for the first screenshot attempt and captured an unrelated private browser window of
Gian's instead of the game. Deleted immediately, and replaced with an engine-side capture
(`-- --shot` → `get_viewport().get_texture().get_image().save_png()`), which can only ever
see the game's own viewport. Recorded here because the kit will need a screenshot ritual
for store assets later, and it should be the engine-side one.

## Godot — build reality (SDK licences accepted by Gian, 2026-09-12)

Licences accepted on Gian's authorisation; `platform-tools 37.0.1`, `build-tools 36.0.0`
and `platforms;android-36` installed. First APK came out on the next attempt.

| Measure | Value | How |
|---|---|---|
| Debug APK, arm64-v8a only | **26.96 MB** | prebuilt template path (`use_gradle_build=false`) |
| Release APK, arm64-v8a only | **25.21 MB**, unsigned | export reports failure at the signing step because no release keystore exists — deliberately not created. Size is indicative, the payload is complete |
| Cold export | **10.4 s** | first export after a fresh import |
| Repeat export | **10.1 s** | no meaningful incremental saving at this project size |
| `targetSdkVersion` | **36** | `aapt2 dump badging` on the produced APK |
| `minSdkVersion` / `compileSdkVersion` | 24 / 36 | same |
| Signing | **v2 + v3 APK Signature Schemes**, 1 signer | `apksigner verify -v` |
| **16 KB page compliance** | **PASS, verified empirically** | `PT_LOAD p_align = 16384` on both `libgodot_android.so` and `libc++_shared.so`, parsed straight out of the ELF program headers of the APK Godot just produced. Not taken from a changelog |

**Toolchain friction on Windows — all minor, all one-time:**

- `sdkmanager --licenses` ignores piped input from PowerShell; it needs `cmd /c "... < file"`.
- `Expand-Archive` refuses a `.tpz` by extension; copy to `.zip` first.
- `apksigner.bat` needs `JAVA_HOME` in the calling shell even though Godot itself does not.
- `winget` could not create the `godot` PATH alias without admin, so the exe is called by full path.

None of these needed a workaround that a script cannot repeat.

## Still to measure (needs the phone)

- Cold start on device, drag feel, audio latency, timer drift on device, `best.cfg`
  surviving a real `force-stop`, sprite-count ceiling.

Build reality above needed no phone at all.

Device: **Samsung Galaxy S24 Ultra**, not currently at the desk. Noted for the Report:
that is a flagship, and the brief asks for cold start on a **mid-range** device. Numbers
from an S24 Ultra will flatter every candidate and cannot answer the question the brief
actually asked.
