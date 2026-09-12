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

## Still to measure (needs the SDK licence, then the phone)

- APK size, cold build time, incremental build time — **these need only the licence, not the phone.**
- Cold start on device, drag feel, audio latency, timer drift on device, `best.cfg`
  surviving a real `force-stop`, sprite-count ceiling — **these need the phone.**

Device: **Samsung Galaxy S24 Ultra**, not currently at the desk. Noted for the Report:
that is a flagship, and the brief asks for cold start on a **mid-range** device. Numbers
from an S24 Ultra will flatter every candidate and cannot answer the question the brief
actually asked.
