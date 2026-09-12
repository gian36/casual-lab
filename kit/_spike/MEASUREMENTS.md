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

---

# Candidate C — web + Capacitor

Stack chosen to be what the line would actually ship, not a straw man: **Phaser 4.2.1**
(WebGL) + **Vite 8.3.0** + **Capacitor 8.5.2**, TypeScript under `strict`. Same slice,
same geometry (the aircraft silhouette is the identical unit-space polygon), same
procedurally generated one-shots, same measurements.

## Install

| Step | Result |
|---|---|
| `npm install phaser vite @capacitor/{core,cli,android}` | OK, one command, no account, no licence, ~40 s |
| `npx cap add android` | OK, **141 ms** |
| Android toolchain | reuses the JDK 17 + SDK already installed for Godot — no extra install |

Capacitor's generated project pins **AGP 8.13.0** and **Gradle 8.14.3**.

## Writing the slice

| # | What happened | Cost |
|---|---|---|
| 1 | `vite build` succeeded on the **first** attempt — but Vite does not type-check, so this proves nothing about correctness. Installed `typescript` and ran `tsc --noEmit` under `strict`, which is the honest bar. | — |
| 2 | **Correction 2.** `Phaser.Geom.Point` no longer exists in Phaser 4. Caught statically by `tsc`, with a "Did you mean 'POINT'?" hint. | 1 edit |
| 3 | **Correction 3.** Phaser 4's `fillPoints` signature wants real `Phaser.Math.Vector2` instances, not `{x, y}` literals — a second, separate Phaser 4 API change in the same three lines. Also caught statically. | 1 edit |
| 4 | Clean types, clean build, correct visuals on the first run in a browser. | — |
| 5 | Parity pass: the first version omitted the service-name labels the Godot slice draws. Added as pooled text objects created once and repositioned, never per frame. | ~10 min |

**Corrections: 3 (vs 1 for Godot)** — and all three were **Phaser 4 API drift**, not
language or engine problems. This is the honest shape of the productivity difference:
Claude's priors about Phaser are a major version out of date, while GDScript's surface
had not moved under it. Both toolchains caught their errors **before the code ran** —
Godot at import, TypeScript at `tsc`.

**Code Claude wrote unaided:** all of it, ~430 lines of TypeScript.

## Logic verified by driving the real handlers

The browser pane used for automation is hidden, and a hidden page has
`requestAnimationFrame` **paused outright** — `accumulated` advanced by exactly 0 over
2 s while `actualFps` still reported a stale 45. So no desktop frame-pacing number is
trustworthy here, and none is claimed. Instead the loop was stepped by hand and the
real drag handlers were driven on the real tray zones:

| Check | Result |
|---|---|
| Timer accuracy | 60 ticks at 16.67 ms → **exactly 1.00 s** of game time (44.886 → 43.89) |
| Valid drop | real `dragstart`/`drag`/`dragend` on the tray zone → lane accepted, `activeService` = the wanted service |
| Service completion | bar filled, `served` 0 → 1, the need removed from the lane |
| Refused drop | dropping a service the lane does not need → no service started, refuse flash set |
| Fail state | shift run to zero → `running` false, overlay visible, "SHIFT FAILED / served 1 · best 1" |
| Persistence | `localStorage` written, and **`BEST 1` survived a full page reload** |

**A real finding for this candidate, not an artefact of the tooling:** a WebView
suspends the game when it loses visibility, and the `AudioContext` will not start at all
before a user gesture — the slice has to call `unlock()` on first pointer-down. Neither
native candidate has to think about either. The same class of thing appears in the CSS:
`touch-action: none`, `user-select: none` and `overscroll-behavior: none` all had to be
set explicitly or a finger would scroll or text-select the page out from under a drag.

## Capacitor — build reality

**Correction 4, and it is a toolchain finding, not a coding one.** The first Gradle build
failed after 1 m 58 s with `error: invalid source release: 21`. Cause, read out of
`node_modules/@capacitor/android/capacitor/build.gradle:66`: Capacitor 8 compiles against
**`JavaVersion.VERSION_21`**. Godot 4.7 declares **JDK 17**. So the two candidates want
**different JDKs**, and a machine that builds both needs both installed. Temurin 21
installed via winget; the next build succeeded.

| Measure | Capacitor | Godot (for comparison) |
|---|---|---|
| Debug APK | **4.34 MB** | 26.96 MB |
| Release APK | not built (would need a release keystore) | 25.21 MB unsigned |
| Cold build | ~2 min including dependency downloads; 55 s once warm | 10.4 s |
| No-change rebuild | 12.2 s (`--no-daemon`) | 10.1 s |
| **Full edit → APK** | **16.4 s** (`vite build` 0.54 s + `cap sync` + Gradle) | 10.1 s |
| `targetSdkVersion` | **36** | 36 |
| `minSdkVersion` | 24 | 24 |
| Signing | v2 scheme, 1 signer | v2 + v3 |
| Native `.so` in the APK | **none at all** | 2 (`libgodot_android`, `libc++_shared`) |
| 16 KB page compliance | **trivially satisfied — there is no native code to align.** Note this changes the day the AdMob plugin arrives and brings the Google Mobile Ads native libraries with it | verified: `p_align = 16384` |
| JDK required | **21** | **17** |

**The size difference is the headline: 4.34 MB against 26.96 MB, roughly 6×.** For a
free-with-ads line where install-size affects conversion, that is not a detail.

**Not measured, and it matters:** Capacitor supports live-reload onto a running device
(`cap run android --livereload`), where a JS edit lands in under a second with no rebuild
at all — which would make the 16.4 s figure the *worst* case rather than the loop. Godot
offers one-click deploy with remote debug and claims script hot-reload into a running
remote instance. **Neither was tested**, because both need the phone. This is the single
biggest open question on the iteration axis and it should not be guessed at.

## Still to measure (needs the phone)

- Cold start on device, drag feel and touch latency, audio latency, timer drift on
  device, persistence across a real `force-stop`, sprite-count ceiling.
- **Live-reload loops for both candidates** — see above.
- For Capacitor specifically: WebGL frame pacing in the Android System WebView, which is
  the one thing the paper research could not settle and the desktop run could not measure.

Device: **Samsung Galaxy S24 Ultra**, not currently at the desk. Noted for the Report:
that is a flagship, and the brief asks for cold start on a **mid-range** device. Numbers
from an S24 Ultra will flatter every candidate and cannot answer the question the brief
actually asked.
