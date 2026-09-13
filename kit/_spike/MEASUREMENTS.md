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

### Godot loop and persistence, verified deterministically

`--headless --fixed-fps 60 --quit-after 3000` forces a constant delta per frame, so 3,000
frames is exactly 50 s of game time regardless of wall clock — enough to run the 45 s
shift to its fail state without anyone playing.

**The first attempt was a false negative, and it was my slice's fault, not Godot's.**
`best.cfg` was not written, because the save only fires on an *improved* score and a
headless run completes nothing, so `0 > 0` was false. Added a `-- --autoserve` test hook
that completes services without a finger. Then:

| Check | Result |
|---|---|
| `user://best.cfg` written | **yes** — `[score] best=56` |
| Value read back by a fresh process | **yes**, same value |
| Services completed in a 45 s shift | 56, against a theoretical ceiling of 60 (4 lanes × 15 at 3 s) — the loop, lane resets and scoring all behave |
| Errors or warnings during the run | none |

The `--autoserve` hook also earns its keep on the phone: the device window can check
persistence across a real `force-stop` without someone having to play first.

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

---

# Candidate B — Unity

Editor: **Unity 6.3 LTS (`6000.3.24f1`, released 2026-09-10)**, confirmed available via
the Hub's own `--headless editors --releases`.

## Install — the one candidate that fought back

| Step | Result |
|---|---|
| `winget install Unity.UnityHub` | **Reported "Successfully installed" and installed nothing.** No `Unity Hub.exe`, no registry entry. Cause found in the winget manifest: the NSIS installer is `Scope: machine`, so it needs elevation, and non-interactively it silently no-ops |
| Official `UnityHubSetup.exe` from the documented CDN path | **404 Not Found** — that URL is dead; the current ones are versioned (`.../hub/prod/3.21.2/UnityHubSetup-3.21.2-x64.exe`) |
| **MSIX variant**, `Add-AppxPackage` | **Worked**, per-user, no admin, 238.9 MB |
| Hub CLI without sign-in | Works — `--headless editors --releases` listed 6000.7.0a6 / 6000.3.24f1 / 6000.0.83f1 / 6000.6.0f1 |
| First `--headless install` | No-op. The MSIX sandbox could not write its cache (`Unable to move the cache: Acceso denegado`) and the default install path is under `%ProgramFiles%`, which needs admin |
| After `ip -s C:\Users\gianv\Unity\Editor` | **Downloads started.** The CLI also *detaches* — a second invocation prints `attaching to existing in-flight download` and exits −1, which is why the first attempts looked like silent failures |
| Download size | **7.75 GB**, against Godot's ~1.2 GB of export templates and Capacitor's ~40 s of npm |
| Install after the download | **FAILED**, 12.3 min in: `[Unity (6000.3.24f1)] failed to install. Error given: The Windows elevation prompt was cancelled or timed out.` The 7.75 GB downloaded fine; the install step itself needs **UAC**, which no unattended session can answer |

Modules pulled as dependencies of `android-sdk-ndk-tools`: NDK r27c, CMake 3.22.1,
build-tools 36.0.0, platform-tools 36.0.0, platforms 34/35/36/37, cmdline-tools 16.0.
**Unity brings its own complete Android toolchain**, so it does not care that Godot wants
JDK 17 and Capacitor wants JDK 21 — a real advantage the other two do not have.

**The finding that matters for this line, now settled rather than suspected:** Unity is the
only candidate that **cannot** be provisioned unattended. Godot went in with one `winget`
command and two direct downloads; Capacitor with one `npm install`. Unity needed a
package-format workaround, an install-path change, diagnosis of a detaching CLI, 7.75 GB,
and then still stopped dead at a **UAC elevation prompt** — with a **Unity account sign-in**
waiting behind that before the editor will run at all.

That is **two human gates to Godot's zero and Capacitor's zero**. For a line whose stated
premise is remote, low-attention work driven from a phone, this is not a one-time setup
annoyance to be waved away: it is the candidate telling you what working with it is like.
Whether it outweighs Unity's first-party monetization backing is Gian's call, not the
spike's — but it belongs on the scale.

## Writing the slice

Written while the editor downloaded, so it is ready for the licensed window:

- `SpikeBootstrap.cs` — the whole slice, building its UI in code rather than in a `.unity`
  scene, so it stays text-reviewable and diffable like the other two. Unity scene YAML is
  GUID-bound and not sensibly hand-written.
- `PolygonGraphic.cs` — UGUI has no polygon primitive, so the aircraft needs a `Graphic`
  that builds its own mesh, **including an ear-clipping triangulator**: the silhouette is
  concave, so a triangle fan renders it wrong. Godot and Phaser both took the same polygon
  as a one-line call. This is the clearest single example of Unity's floor being higher
  for 2D work.
- `LanePanel.cs`, `TrayButton.cs`, `SpikeSfx.cs` — same behaviour, same procedural audio.
- `Editor/SpikeBuild.cs` — batch-mode build: creates the scene, sets arm64-only, IL2CPP,
  portrait, min SDK 24 / target 36, and builds the APK, so the licensed window is one
  command rather than an editor session.

### Gian's attended steps (2026-09-13)

Gian created a Unity account, signed in to Hub and activated **Personal**. With him at the
machine, the Android module install then completed in **3.7 min with no UAC prompt at all**
(the editor path is user-writable). So the real cost of the two gates was one account
creation, one sign-in, one licence click.

The licence path was shaky but held: the batch-mode editor logged
`Code 10 while verifying Licensing Client signature ... LicensingClient has failed validation; ignoring`
and `Access token is unavailable`, almost certainly because Hub is the sandboxed MSIX build
and its licensing client lives under `WindowsApps`. Compilation proceeded regardless.

### Unity build attempts

| # | What happened | Cost |
|---|---|---|
| 0 | `build_unity.ps1` failed before reaching Unity: *Missing string terminator*. **My script, not Unity:** Windows PowerShell 5.1 reads BOM-less UTF-8 as ANSI, so an em dash's byte `0x94` became cp1252's closing double quote. `device_window.ps1` had the same defect and would have died mid phone session. Both made ASCII-only and checked with the real parser. Not counted against Unity. | 1 fix, 2 files |
| 1 | **Correction 5 — Unity's first.** 30 × `CS0246`/`CS0234`: `UnityEngine.UI` and `UnityEngine.EventSystems` not found. **One root cause:** since 2019 UGUI is a *package* (`com.unity.ugui`), not engine code. The hand-written project had no `Packages/manifest.json`, so Unity generated one with the built-in modules only — including `com.unity.modules.ui`, which sounds like the right thing and is not. Script compilation took 12.0 s to report it. | 1 manifest line |
| 2 | Rebuild **succeeded**: `[spike] result=Succeeded ... errors=0`, **zero C# errors**. Everything written unaided against the API while the editor was still downloading compiled on the first real attempt once the package was present. | — |

**Unity correction count at compile time: 1** (the package manifest), **0** C# errors — but see the first visual run below: four more defects compiled clean.

### Unity — build reality (2026-09-13)

| Measure | Unity 6.3 LTS | Godot 4.7.2 | Capacitor 8.5.2 |
|---|---|---|---|
| Corrections | **5** — 1 package manifest + 4 slice defects that compiled clean (#6–#9) | 1 | 3 + 1 toolchain |
| Debug APK, arm64-v8a | **27.78 MB** like-for-like, clean build (34.98 MB with the 11 packages Unity added on its own — see below) | 26.96 MB | 4.34 MB |
| Cold build | **312.5 s** first build incl. project import (6 min 19 s wall); clean rebuilds **204.2 s** with the extra packages, **150.8 s** (168.2 s wall) like-for-like | 10.4 s | ~2 min incl. downloads; 55 s warm |
| of which Gradle | 101.1 s | n/a (prebuilt template) | most of it |
| Edit → APK | **71.6 s** wall (Unity reports 44.7 s build), one-constant change | 10.1 s | 16.4 s |
| `targetSdkVersion` | **36** (enum resolved, no fallback) | 36 | 36 |
| `minSdkVersion` | **25** — the script set 24; Unity raised it **silently**, no log warning | 24 | 24 |
| Signing | v2 | v2 + v3 | v2 |
| Permissions | like-for-like: `INTERNET` + AndroidX receiver permission; with the extra packages also **`com.android.vending.BILLING`** and `ACCESS_NETWORK_STATE` | **none** | `INTERNET` + AndroidX receiver permission |
| Native `.so` | **6** — like-for-like: `libil2cpp` 31.20 MB, `libunity` 30.68 MB, `libc++_shared` 1.23, `libgame` 0.24, `libswappywrapper` 0.22, `libmain` 0.01 (with the extra packages: 45.37 / 32.55) | 2 | 0 |
| 16 KB page compliance | **PASS, verified empirically** — `p_align = 16384` on all six | PASS, empirical | n/a, no native code |
| JDK | **bundled OpenJDK** — no system JDK needed at all | 17 | 21 |
| Build toolchain | bundled Gradle 9.1.0 / AGP 9.0.0 | none needed for prebuilt template | AGP 8.13.0 / Gradle 8.14.3 |

**Do not quote Unity's `size=539.17 MB`.** That is `BuildReport.summary.totalSize`, the
uncompressed staging content, not the APK. The APK is 34.90 MB.

**Nor the incremental APK's 48.46 MB.** The one-constant rebuild produced a 48.46 MB APK
from the *same* content. Walking the zip structure directly (central directory → local
headers → gaps) accounted for 48.44 of its 48.46 MB:

| Part | Size |
|---|---|
| Compressed payload, 465 entries | 34.81 MB — same as the cold build |
| **Holes between entries** | **13.57 MB in 4 gaps** |
| Alignment padding in local extra fields | 0.00 MB |
| Central directory | 0.04 MB |

The largest holes are **11.23 MB** right before `lib/arm64-v8a/libmain.so`, where the old
`libil2cpp` slot sat, and **2.07 MB**, exactly the compressed size of `global-metadata.dat`.
Changing one constant rewrote IL2CPP's binary and metadata; Gradle's incremental packager
appended the new copies and left the old slots as dead space rather than repacking.
Every native library's raw size was byte-identical to the cold build.

**For the kit:** a Unity dev APK's size is meaningless after an incremental build. Size
budgets must be read from a clean build, and anything installed on a device for
measurement should be built clean too.

**Phase timeline of the cold build**, from artifact timestamps: 13:20:35 start ·
13:22:27 Unity linker · 13:22:41 IL2CPP conversion done · 13:24:59 native C++ compile done
and Gradle project exported · 13:25:10 Gradle starts · 13:26:54 exit 0.

**Fairness notes.** All three are debug/development arm64 builds, so the sizes compare like
for like, but none is release-optimised: Unity's IL2CPP with managed stripping, Godot's
release template (25.21 MB unsigned) and a minified Capacitor bundle would each shrink.
Unity's six native libraries total ~64 MB uncompressed like-for-like (~80 MB with the extra packages). Its bundled toolchain is a genuine
advantage — no JDK juggling — bought with a 7.75 GB install.

**Licence path held, but noisily:** the log still carries `LicensingClient has failed
validation; ignoring` and `Access token is unavailable`, followed by `Successfully resolved
entitlement details`. Works today; worth knowing if a future Hub or editor update tightens
the check against the sandboxed MSIX licensing client.

### Unity — first visual run: compiles clean, renders wrong

The Godot and Phaser slices were checked visually on the desktop before any phone existed,
so the Unity one got the same: a Windows player (Mono, windowed 540×960, **39.3 s** to build)
launched with `--shot`, captured by `ScreenCapture.CaptureScreenshotAsTexture()` inside the
game — never a desktop grab.

**Unity is the only candidate whose first visual run was wrong.** Zero compiler errors,
three visible defects plus one caught by reading the code. All four are **Claude's bugs**,
not engine faults — but three of them live in code the other two engines never asked for.

| # | Defect | How found | Root cause | Fix |
|---|---|---|---|---|
| 6 | Tray buttons blank | code review, before any run | `NewText("Label", …)` created, `.text` never assigned | assign `ServiceNames[s]` |
| 7 | **Aircraft drawn as disconnected shards** | screenshot | Hand-written ear clipping (UGUI has no polygon primitive). `SignedArea` used the `(b.x−a.x)(b.y+a.y)` form, **positive for clockwise**, so the outline was walked backwards and real ears rejected; and `InTriangle` counted points *on* an edge as inside, which the fuselage's collinear vertices trip | standard shoelace; strict-interior test |
| 8 | White squares in unused need slots | screenshot | unused slots hid icon and label but not the **fill** image; a sprite-less `Image` draws solid white | deactivate the fill too |
| 9 | **Shift clock lost ~2 s at launch** | on-screen perf readout: `drift +2.041 s` (Godot +0.032, Phaser 0.000) | clock started in `Awake`; the first `Update`'s `unscaledDeltaTime` spans everything before the first rendered frame, splash included | arm the clock on the first rendered frame, discard that delta |

**Fix 7 was proven before spending a rebuild on it.** A line-for-line JS port of the
triangulator, run on the real outline in the same pixel space:

| Version | Triangles (need n−2 = 19) | Area covered (px²) | Result |
|---|---|---|---|
| Original | 10 | 20,522 of 32,533 | **FAIL** — matches the shards |
| Fixed | 19 | 32,533 of 32,533 | **PASS** |

Also turned off: `PlayerSettings.SplashScreen.show = false` in both build methods — removes
the splash from timer and cold-start numbers, and tests the research claim that the splash is
optional on Unity 6 Personal against a real Personal licence rather than a blog post.

Not fixed, noted: the gate light is a square (an `Image` without a sprite), where the other
two draw a circle. Cosmetic.

**Status: VERIFIED 2026-09-13.** Re-built (25.2 s; Unity 11.8 s) and re-captured:

| Check | Before | After |
|---|---|---|
| Aircraft | disconnected shards | whole silhouette, identical to the Godot and Phaser slices |
| Unused need slots | solid white squares | hidden |
| Tray and need labels | tray blank | all present |
| Timer drift at launch | **+2.041 s** | **−0.005 s** |
| Time left ~1.5 s into the shift | 38.8 | 43.3 |

**Unity's total: 5 corrections** — 1 package manifest + 4 slice defects (#6–#9).
Fairness note: Capacitor's first version also shipped without the service-name labels; that
was logged as a parity pass rather than a correction. Counted the same way as Unity's #6,
Capacitor would be 3 API + 1 parity omission + 1 toolchain.

**Splash claim tested against a real licence:** `PlayerSettings.SplashScreen.show = false`
built successfully on **Unity Personal**, and `ProjectSettings.asset` persisted
`m_ShowUnitySplashScreen: 0`. The research claim holds. One oddity worth knowing: the build
log still lists `Built-in Texture2D: Splash Screen Unity Logo — 2.7 MB`, so the logo
texture ships even with the splash off (`m_ShowUnitySplashLogo` remains 1).

### Pre-flight catch: two of three device launch commands were wrong

Before any phone session, each built APK's manifest was read to check the activity names
hard-coded in `device_window.ps1`:

| Candidate | Script used | Actual launcher | Would it have worked? |
|---|---|---|---|
| Capacitor | `.MainActivity` | `com.gabella.cl01spikeweb.MainActivity` | yes |
| **Unity** | `com.unity3d.player.UnityPlayerActivity` | **`com.unity3d.player.UnityPlayerGameActivity`** (Unity 6's GameActivity default) | **no** — activity not found |
| **Godot** | `com.godot.game.GodotApp` | **activity-alias `com.godot.game.GodotAppLauncher`**, targeting `GodotApp`, which is `exported=false` | **no** — `adb am start` refuses non-exported activities |

`aapt2 dump badging` reports **no** launchable activity for the Godot APK at all, because it
does not follow activity-aliases; only the binary manifest's xmltree shows the alias.
Godot's cold start, logcat scan and persistence check, and Unity's cold start, would all have
**failed silently in the attended window**. `device_window.ps1` now resolves each launcher
on the phone itself with `cmd package resolve-activity -c android.intent.category.LAUNCHER`,
so no activity name is hard-coded anywhere.

### Unity added an IAP SDK and an analytics SDK on its own — and they shipped

Spotted in IL2CPP's assembly list during a clean rebuild (`Unity.Services.Analytics`,
`Unity.Services.Core.*`, `Newtonsoft.Json` — none used by the slice), then traced with
Unity's own `Logs/Packages-Update.log`:

| Time | Update mode | What Unity added |
|---|---|---|
| 13:18:37 | `resetToDefaultDependencies` — first open, no manifest | built-in modules + `com.unity.multiplayer.center` |
| 13:20:37 | `updateDependencies` — **the first open after Claude hand-edited the manifest to add uGUI** | 11 packages: **`com.unity.purchasing@4.15.1`**, **`com.unity.analytics@3.8.2`**, `collab-proxy`, `ide.rider`, `ide.visualstudio`, `test-framework`, `timeline`, `ai.navigation`, `xr.legacyinputhelpers`, `2d.sprite`, `2d.tilemap` |

No prompt, and nothing in the build output. **Claude committed that manifest in `187f3a5`
without noticing** — a miss worth owning, and exactly the kind a low-attention line would not
catch in review.

What those packages put into the store build (clean, 34.98 MB):

- permissions **`com.android.vending.BILLING`**, `INTERNET`, `ACCESS_NETWORK_STATE`
- Play Billing client code: **508** `com/android/billingclient` references in `classes.dex`,
  41 `com/unity/purchasing`, and a bundled `AmazonAppStore.aar`
- IL2CPP-compiled `UnityEngine.Purchasing`, `Unity.Services.Analytics`, `Unity.Services.Core`,
  `Newtonsoft.Json`

**Compliance checked, not assumed:** `com.unity.purchasing` 4.15.1 declares
`com.android.billingclient:billing:8.3.0` (changelog: *Billing Library updated to 8.3.0*), which
clears Play's v8 bar. The worry that this APK would be store-rejectable was checked and
**falsified**.

**Like-for-like rebuild** after removing the 12 non-slice packages (kept uGUI and every
built-in module):

| | With the extra packages | Like-for-like |
|---|---|---|
| APK | 34.98 MB | **27.78 MB** (−7.20 MB) |
| Zip entries | 465 | 403 |
| Clean build | 204.2 s | 150.8 s (168.2 s wall) |
| `BILLING` permission | yes | **gone** |
| billing / purchasing / Amazon refs in dex | 508 / 41 / present | **0 / 0 / 0** |
| `Unity.Services.*`, `Newtonsoft.Json` in IL2CPP metadata | present | **0** |
| Did Unity re-add packages? | — | **no** — `Packages-Update.log` untouched, manifest still 35 deps |

Residue, **not traced**: IL2CPP metadata still holds 16 `UnityEngine.Purchasing` and 17
`UnityEngine.Analytics` strings. Every built-in module (including
`com.unity.modules.unityanalytics`) was kept, which is the likely source, but that is not
verified. No billing code ships.

**Corrected like-for-like sizes: Unity 27.78 MB · Godot 26.96 MB · Capacitor 4.34 MB.** Unity and
Godot are effectively level; the unrequested packages had made Unity look ~30% heavier than it is.

**For the line:** a Unity project's package set can change underneath it on open, and what it
adds ships code, SDKs and permissions into the store build. A line that must declare Play Data
safety and gate analytics behind UMP consent cannot have an analytics SDK or a `BILLING`
permission nobody chose. Any Unity-based kit would need a pinned manifest and a check on the
manifest and the APK's permissions at every build. Godot and Capacitor showed no equivalent
behaviour in this spike.

---

# Device window — pre-flight (2026-09-13, phone at the desk)

Gian connected the **Galaxy S24 Ultra** (USB serial `R5CX21TWDZN`) with Developer options and
USB debugging on. Before any measurement, four things were wrong or missing — none on the
phone, and three of them Claude's.

| # | Problem | Evidence | Whose | Status |
|---|---|---|---|---|
| 1 | Documented command used a **relative path**; Gian's terminal was in `C:\Windows\System32\WindowsPowerShell\v1.0` | *"El argumento 'kit/_spike/device_window.ps1' para el parámetro -File no existe"* | Claude (doc) | **fixed**, `948ee06` — full path + `-ExecutionPolicy Bypass` |
| 2 | **No ADB driver on Windows.** `adb devices` listed nothing, not even `unauthorized` | Phone enumerated fine (`VID_04E8&PID_6860`, MTP, COM3). The ADB interface **MI_03** (`Class_ff&SubClass_42&Prot_01`) was bound to Microsoft's generic **`winusb.inf`** via `MS_COMP_WINUSB`, with **no `DeviceInterfaceGUIDs`** and the ADB interface GUID `{F72FE0D4-CBCB-407D-8814-9ED673D0DD6B}` **not registered**. Both adb backends (default and `ADB_LIBUSB=1`) saw nothing | Windows setup | **needs Gian**: Samsung Android USB Driver for Windows **v1.9.5.0 (35.5 MB, 2026-05-06)**, from `developer.samsung.com/android-usb-driver` — installing a system driver is his to do |
| 3 | **Capacitor live-reload flags were wrong.** The script called `cap run android --livereload --external` | Capacitor CLI 8.5.2's own `run --help`: the flags are `-l/--live-reload`, `--host`, `--port`, `--forwardPorts <port:port>` (runs `adb reverse`). `--external` is an Ionic CLI flag, not a Capacitor one | Claude (script) | **fixed** — flags now `-l --host localhost --port 5173 --forwardPorts 5173:5173 --target <serial>`, confirmed in the CLI source (see row 5) |
| 4 | **The network would have broken LAN live reload anyway.** PC on Ethernet `10.0.0.250`, network **`VEGuest`**, Windows profile **Public** | Guest networks commonly isolate clients from each other, and the Public profile blocks inbound connections | environment | avoided by design: live reload goes over the USB cable via `adb reverse` instead of the LAN |
| 5 | **Ctrl+C would have killed the whole script.** Step 1 told Gian to press Ctrl+C to end live reload "and this script continues" | Capacitor restores its config and manifest on SIGINT (`tasks/run.js`), so Ctrl+C is required, but in PowerShell Ctrl+C also stops the running script: steps 2–6 would never have run. The source also showed `cap run` never starts a dev server, `-l` writes `android:usesCleartextTraffic="true"`, and `--forwardPorts` is passed to native-run as `--forward` | Claude (script) | **fixed** — the script starts Vite itself, runs `cap run` in a **separate window** where Ctrl+C is safe, stops Vite by its listening port, and restores the native config with `cap sync` if the window was closed without Ctrl+C |

**Why this matters beyond the spike.** Two of the three candidates' device loops depend on adb,
and on this Windows machine adb did not work out of the box for a Samsung phone. That is a
one-time driver install — but it is a human-gated step that falls on *every* candidate equally,
like the Android SDK licence, and it belongs in the kit's machine-setup notes, not in any
engine's scorecard.

**Pre-flight checks have now caught four device-session bugs before Gian's time was spent on
them:** Unity's and Godot's launch activity names, the wrong Capacitor live-reload flags, and
Ctrl+C ending the whole script. One more reached Gian (the relative path). Every one was in a
Claude-written script and every one was found by checking the command against the actual
tool — its help, its source, or the built APK — rather than recalling it.

### Phone connected (2026-09-13, 14:42)

Gian installed the Samsung Android USB Driver v1.9.5.0 and accepted the RSA prompt. After an
adb server restart: `R5CX21TWDZN device product:e3qxxx model:SM_S928B`. The ADB interface
GUID is now registered, and `cmd package resolve-activity` works on the device, so the
script's launcher resolution is sound.

| Property | Value | Why it matters |
|---|---|---|
| Model | **SM-S928B** (Galaxy S24 Ultra) | flagship — see the reference-device `[OPEN]` |
| Android / API | **16 / 36** | same as every APK's targetSdk |
| One UI | 8.5 (`80500`) | |
| ABI | arm64-v8a | matches all three arm64-only APKs |
| Screen | 1440 × 3120 | portrait 9:16-ish; each slice scales to fit |
| **Page size** | **4096 bytes** | **the phone runs 4 KB pages, so 16 KB behaviour cannot be exercised on this device.** The 16 KB results in this spike stay APK-level (ELF `p_align`), not runtime-verified |
| Stay awake | off (`stay_on_while_plugged_in = 0`) | the screen can lock mid-run and distort cold start; Gian asked to enable it for the session |

**One more pre-flight catch, the fifth:** `cap run -l` builds the APK with
`http://localhost:5173` baked into its packaged `capacitor.config.json`. Restoring the config
afterwards does not rebuild that APK, so the cold-start step would have installed it and timed a
WebView pointed at a stopped dev server. The script now rebuilds the Capacitor APK from a clean
production bundle between live reload and cold start, and warns if a server URL is still packaged.

### The first real run failed: the Android SDK was invisible outside Claude's processes

Gian ran `device_window.ps1` twice — once in a standalone PowerShell, once in the Claude app's
Terminal panel — and both died on the first adb call:
`El término 'C:\Users\gianv\AppData\Local\Android\Sdk\platform-tools\adb.exe' no se reconoce`
(`CommandNotFoundException`). The same line ran fine in every Claude tool process.

Two theories were tested and **falsified** before the cause was found:

| Theory | Test | Result |
|---|---|---|
| Administrator window (the first shell opened in `C:\Windows\System32`) | the Terminal panel is a normal shell in the project folder, and it failed identically | **false** |
| `PATHEXT` missing `.EXE` in Gian's shells | registry `PATHEXT` (Machine) includes `.EXE`, no User override; `& $adb version` succeeds under it; removing `.EXE` gives a *different* error (`RuntimeException`); no PowerShell profile scripts exist | **false** |
| Permissions | `adb.exe` inherits exactly the ACL of `AppData\Local`; no mark-of-the-web | not the cause |

**Actual cause — MSIX file-system redirection.** The SDK physically lived at
`%LOCALAPPDATA%\Packages\Claude_pzs8sxrjxfjjc\LocalCache\Local\Android\Sdk`. Claude's tool
processes run as children of `Claude.exe` from `WindowsApps` (`Claude_1.52386.3.0`); when they
created the *new* folder `AppData\Local\Android`, Windows redirected it into the Claude package's
private `LocalCache`. Claude's processes see that merged view at the normal path; Gian's shells
see the real folder, where `Android` does not exist. Every existence check Claude ran from its own
processes therefore reported `True` — including the ones used to verify the fix path.

Why only the SDK: the redirect applies to **AppData folders that did not already exist**. Godot's
`Roaming\Godot` and `Local\Microsoft\WinGet` pre-existed, so the export templates, editor settings
and the Godot install are real. Everything outside AppData — the repo, the JDKs in
`C:\Program Files`, `C:\Users\gianv\Unity`, `~\.android\debug.keystore` — was never affected.
Unity's editor cache (`Local\Unity`, 81 MB) was also redirected but is not needed by Gian's tools.
(The Unity Hub behaviour seen earlier is the same mechanism, applied to Hub's own package.)

**Fix:** copied the SDK (550 MB, 11,814 files, sizes verified) to **`C:\Android\Sdk`**, outside
AppData; repointed `device_window.ps1` (now prefers `C:\Android\Sdk`), `build_unity.ps1`'s
build-tools path, and Godot's `export/android/android_sdk_path` (written to the real
`editor_settings-4.7.tres`, confirmed not redirected); restarted the adb server from the new copy —
`R5CX21TWDZN device`, launcher resolution working.

**A second, smaller trap found while writing this up:** in Claude's heredoc-written JavaScript,
`\` collapsed to `\`, so backslashes vanished from JS string literals. The first version of this
very section lost every backslash in its Windows paths, and one script patch silently matched
nothing. Text containing Windows paths is now written as plain data files, never as JS literals.

**For the kit — this is the most important environment finding of the spike.** Anything Claude
installs from inside the Claude desktop app into a *new* AppData folder exists only for Claude.
The build works, every check passes, and the founder's own tools cannot see it. Kit setup must
install toolchains to explicit paths outside AppData, and verification must be run from a shell the
founder actually uses, not only from Claude's processes. Proposed as an `[OPEN]` for the line's
machine-setup doctrine.

### Device results — first attended run (2026-09-13, 15:16–15:20)

Run by Gian from the Claude app's Terminal panel after the SDK move; all three APKs installed at
15:16:56 / 15:17:08 / 15:17:25. Raw output kept in `device_results.json`.

**Cold start** — `am start -W` `TotalTime`, 5 launches each, `force-stop` before every launch, debug
builds, SM-S928B, Android 16:

| Engine | Median | Min | Max | All five |
|---|---|---|---|---|
| **Unity** 6.3 (Development, IL2CPP) | **305 ms** | 283 | 375 | 375, 305, 305, 308, 283 |
| **Godot** 4.7.2 (debug template) | **323 ms** | 304 | 456 | 456, 315, 340, 323, 304 |
| **Capacitor** 8.5.2 + Phaser 4 (debug) | **408 ms** | 391 | 529 | 529, 391, 436, 408, 402 |

What these numbers can and cannot say:

- **Unity and Godot are level** — 18 ms apart, well inside each other's spread. **Capacitor is ~85–100
  ms slower at the median**, consistent with WebView start-up in front of the game.
- The **first launch in every series is the slowest** (456 / 529 / 375 ms): the first cold start after a
  fresh install pays for caches the later ones reuse.
- `TotalTime` ends when the activity draws its first frame. For Godot and Unity that is the engine's
  surface; for Capacitor it is the native activity, **not** the WebView finishing the page. So these are
  time-to-first-activity-frame, not time-to-playable — a like-for-like proxy, not the player's
  experience.
- **Flagship phone, debug builds.** Every number here will be kinder than on the mid-range device the brief
  asked about (see the reference-device `[OPEN]`), and release builds may differ.

**Live reload (step 1): not measured.** The second window's `cap run` exited before building and nothing
reached the phone; the Vite dev server also stopped. The error text was never captured. Separately
verified from Claude's side: target detection works (`R5CX21TWDZN` listed) and the Gradle build
succeeds in that window's environment in 20 s — so the failure is elsewhere, still unknown.

**Godot remote deploy (step 2): not reported.**

**Logcat scan (step 4): mislabeled, and clean.** The lines captured under "Godot" include a Swappy line
naming `libgame.so`, which is Unity's library — the scan picked up Unity's still-running process, so it
is not a per-app scan. What it caught is system graphics noise (Gralloc 4×4 buffer probes, Adreno QSPM).
Other error lines seen for all three apps (`AppOps attributionTag not declared`,
`libpenguin.so dlopen failed`) are Samsung system noise. **The crash buffer is empty: no crashes or
ANRs in any of the three.**

**Persistence (step 5): verification in progress.** Gian played each game to SHIFT FAILED and relaunched
after a real `force-stop`. Reading app storage directly: Godot's data directory holds **no `best.cfg`**
(only `shader_cache`) — the save fires only on an improved score, and step 3's reinstall wiped any
earlier data, so this needs Gian's account of whether any aircraft were served. Unity's and Capacitor's
stored values are still being read.

**Judgement items — drag feel, stress ceiling, audio: pending Gian's report.**

### Automated on-device test — drag and persistence, Godot and Capacitor (2026-09-13, 15:27–15:31)

Gian's own play left Godot and Capacitor with nothing saved. Rather than guess why, the same test was
run on both engines by Claude over adb: real touch input (`input swipe`, 450 ms, from each of the four
tray buttons onto each of the four lanes, three sweeps inside the 45 s shift), let the shift run out,
`force-stop`, read the stored value from app storage, relaunch, screenshot. Coordinates came from
on-device screenshots, not from layout maths.

| | Godot 4.7.2 | Capacitor 8.5.2 + Phaser 4 |
|---|---|---|
| **Drag works with real touch on device** | **yes** — SERVED 7 with 19.4 s left | **yes** — SERVED 7 with 15.7 s left |
| fps during drags | **120** | **121** |
| frame max / **drag max** | 8.3 ms / **8.3 ms** | 8.3 ms / **8.3 ms** |
| timer drift at that point | +0.165 s | +0.003 s |
| **Stored after `force-stop`** | `best.cfg`: `[score] best=12` | `localStorage` `cl01.best` = 12, **both before and after** the `force-stop` |
| BEST on relaunch | **12** | **12** |

What this settles:

- **Persistence works on device in all three engines.** Unity was already confirmed from Gian's session:
  `PlayerPrefs` held `cl01.best = 8`, and BEST 8 showed on relaunch.
- **Gian's zero scores in Godot and Capacitor were not a persistence bug.** The save fires only when a shift
  ends with an improved score, and step 3's reinstall had wiped any earlier value — so no aircraft were
  served in those two shifts.
- **The WebView-flush worry did not materialise.** Capacitor's `localStorage` was already on disk before
  the `force-stop`, about 3 s after the save.
- **Drag max 8.3 ms at 120 Hz in both engines means no dropped frame while dragging** — on this flagship.
  Drag *feel* is still Gian's judgement; this only says the frames were delivered.
- **100 extra aircraft cost nothing measurable** in either engine: `planes 104` showed with frame max
  still 8.3 ms. The real sprite ceiling was not reached and needs a deliberate stress run.

Unexplained, recorded rather than guessed: `stress +100` fired once during each automated run, and
Godot's relaunch screenshot also shows `planes 104` on what should have been a fresh process.

### On-device layout findings (from screenshots)

- **Unity's UI was clipped on this phone — correction #10, Claude's.** `CanvasScaler.matchWidthOrHeight = 1`
  (match height) on a 1440×3120 screen, which is taller than 9:16, scaled the 1080×1920 layout by 1.625 to
  1755 px wide. **The PUSH tray button, the perf readout, the stress button and the lanes' right edges were
  off-screen** — Gian's Unity score of 8 was earned without PUSH. Godot (`canvas_items` + `expand`) and
  Phaser (`Scale.FIT`) both fitted the same screen correctly. Fixed to match width; rebuild and on-device
  re-check in progress.
- **Capacitor is not full-screen by default.** The status bar and navigation bar stay visible and the game
  is letterboxed between them; Godot (`immersive_mode`) and Unity run edge-to-edge. Going full-screen in a
  WebView app needs a status-bar/immersive plugin or native config — small, but extra kit work for this
  candidate.
- **Idle frame rate:** Godot **121 fps** (the display's 120 Hz); Capacitor **101 fps** at idle in one
  screenshot, 95 in another, 121 during the automated drags — the WebView's frame pacing varied where
  Godot's did not.

### Automated test — Unity (2026-09-13, 15:34–15:35), scaler fix installed

Same script (`device_autotest.sh unity`), after installing the width-matching build with `install -r`.

| | Unity 6.3 LTS |
|---|---|
| App data across the update | **kept** — `cl01.best = 8` before and after `install -r` |
| **Drag works with real touch on device** | **yes** — SERVED 7 with 22.4 s left |
| fps / frame max / **drag max** | 60 / 16.7 ms / **16.7 ms** — one frame at 60 Hz, no dropped frame, but **capped at 60 by the slice's own `Application.targetFrameRate = 60`** (correction #11; 120 Hz rebuild in progress) |
| timer drift | −0.002 s |
| **Stored after `force-stop`** | `PlayerPrefs` `cl01.best` = **13**, both before and after the `force-stop` |
| BEST on relaunch | **13** |

Incidental: `planes 404` mid-shift (the stress button fired four times in this run) with frame max unchanged
at 16.7 ms. Unity's stress silhouettes render in the wrong place — off to the right and below the canvas —
because the stress layer's `RectTransform` is created with default centre anchors and no size. Cosmetic and
harness-only, Claude's.

**All three engines, on device:** drag registers with real touch input, the best score persists across a
real `force-stop`, and no frame was dropped while dragging at each engine's frame rate.

### Gian's observations (2026-09-13)

- **Drag feel: "fine"** — no engine singled out.
- **Stress +100: tried in some runs, "didn't see changes."** Consistent with the measurements — 104 aircraft
  (Godot, Capacitor) and 404 (Unity) left frame max flat. It is also partly the harness: the stress
  silhouettes are faint and drawn behind the lanes in Godot and Capacitor, and misplaced in Unity. **This
  phone's sprite ceiling is above 400 and was not found.**
- **Completion sound: played "on some engines."** Which engine(s) stayed silent is not yet known.
- **Godot remote deploy: not tried.** The on-device edit→see loop for Godot, and Capacitor's live reload,
  remain unmeasured.

**What this says about the productivity axis.** Correction counts alone flatter Unity here.
Godot's `draw_colored_polygon` and Phaser's `fillPoints` triangulate internally; UGUI made
Claude write a triangulator, and it was wrong in a way no compiler could catch. The clock bug is
subtler still: identical logic was correct in Godot and Phaser, and wrong in Unity only
because of what Unity's first-frame delta contains. On a line where Gian's attention goes to
play rather than build review, **defects that compile clean and only show up on screen are the
expensive kind.**

**Error-message quality:** accurate but noisy — thirty lines that all say the same thing, none
of which names the missing package. Compare Godot's single precise line and `tsc`'s
"Did you mean 'POINT'?". The fix is obvious *if you already know* UGUI moved to a package;
the message does not tell you.

## Still to measure

**Unity:** compiled, built, and visually verified on the desktop (2026-09-13). Still needs every
on-device number, like the other two.

**All three, blocked on the phone:**

- Cold start on device, drag feel and touch latency, audio latency, timer drift on
  device, persistence across a real `force-stop`, sprite-count ceiling.
- **Live-reload loops for both candidates** — see above.
- For Capacitor specifically: WebGL frame pacing in the Android System WebView, which is
  the one thing the paper research could not settle and the desktop run could not measure.

Device: **Samsung Galaxy S24 Ultra**, not currently at the desk. Noted for the Report:
that is a flagship, and the brief asks for cold start on a **mid-range** device. Numbers
from an S24 Ultra will flatter every candidate and cannot answer the question the brief
actually asked.
