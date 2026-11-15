# kit/ — the Toybox line's shared base (Unity 6.3 LTS)

The kit is a tool, not a project (`../LINE_CHARTER.md` §3): no studio number, allowed to be
ugly, and it ships nothing by itself. Line games depend on it; Core games never do.

| Path | What it is |
|---|---|
| `package/` | **Kit v0**, the Unity package `com.gabella.kit` that games reference |
| `tools/build_unity.ps1` | Batch-mode build runner with the outside-the-editor package check |
| `SPIKE_REPORT_ENGINE.md` | CL-01's frozen Report (engine choice) |
| `_spike/` | Dead spike code, quarantined; never imported from |

## Kit v0 (FD-S09, 2026-11-07)

Built for **005 Flight Dispatch M1**, and kept to what M1 needs (GDD §10: "M1 needs kit v0
only for the project skeleton"). Nothing for ads, consent, analytics or IAP; those are M4.

### How a game uses it

The game's `Packages/manifest.json` references the package by a relative path, so the
game repo and the line repo must be checked out side by side (`casual-lab/kit/` next to
`casual-lab/<game>/`):

```json
"com.gabella.kit": "file:../../../kit/package"
```

The path is relative to the game's `Packages/` folder. For `flight-dispatch/unity/` that
resolves to `casual-lab/kit/package`.

### What is in it

**Runtime** (`Gabella.Kit`)

- `KitBoot`: runs before the first scene. Portrait only, frame rate matched to the display
  (the spike's hard 60 capped a 120 Hz phone), screen kept awake, multi-touch off so a
  second finger cannot hijack a drag. `KitBoot.Spawn<T>()` creates the game's root object
  and a camera that clears the screen. Games boot from an empty scene and build themselves
  in code, which keeps them text-reviewable, as the spike did.
- `KitUi`: a portrait canvas scaled to **1080 wide, matched on width** (the spike clipped
  the UI on a tall phone when it matched height), a safe-area root, and builders that
  place boxes, labels and buttons in top-left reference units.
- `DragSource`: the reusable drag helper. The ghost lives on its own top canvas, so a drag
  survives the game moving or re-parenting what is being dragged (005's prototype lost
  drags when an arrival landed). It is only an input helper: the game decides what a drop
  means. The ghost floats `Lift` (90) units above the finger; `DragSource.GhostScreen` is
  where it is, so a game can resolve drops where the player aims (added for 005 M2).
- `KitShot`: if the player is launched with `--shot`, it can save its own frames. Used for
  desktop self-checks, because six of the spike's seven Unity defects compiled clean.
- `EventLog`: one JSON object per line in the app's own folder. **Local only, not
  analytics**: no network, no SDK, no identifiers. It exists so a session can read what
  happened in Gian's play (`adb pull`).
  `EventLog.Bundle(maxChars)` packs every kept file into one text (gzip + base64 when long).
- `KitShare.Text(subject, body, title)`: opens Android's own share sheet (`ACTION_SEND`), so
  the player picks where text goes (Gmail, Drive ...). No permission, no network of its own.
  Lets a play log travel without a USB cable. On desktop it writes a file and the clipboard.

**Editor** (`Gabella.Kit.Editor`)

- `KitBuild.Android(settings)`: line defaults (ARM64, IL2CPP, minSdk 25 asked openly
  instead of being raised silently, targetSdk 36, no splash, debug APK), the build, then
  the permission guard. `KitBuild.Windows(settings)`: a windowed portrait player for
  screenshots.
- `KitGuards`: the **three conditions of the Unity ruling** (charter §3), as build failures:

| | Condition | How it is enforced |
|---|---|---|
| (a) | The package manifest is pinned | Every dependency is an exact version or a local `file:` path; no scoped registries; the manifest equals the game's `kit-guards.json` list exactly; the lock file's direct dependencies match. Runs before **every** player build (`IPreprocessBuildWithReport`), however the build was started. |
| (b) | The APK's permissions are asserted at build time | After the build, `aapt2 dump permissions` reads the APK. Any `uses-permission` not on the game's allow list fails the build, and the APK is renamed `*.REJECTED` so no install script can pick it up. |
| (c) | The build fails on any package the editor added unrequested | Every package the Package Manager has resolved must be requested or a dependency of something requested. `tools/build_unity.ps1` also hashes the manifest before and after the editor runs and fails if the editor rewrote it (the spike's additions happened on open, before any build code ran). |

The game owns the allow lists in `kit-guards.json`, next to its `Assets/` folder (Unity
never writes it). Widening either list is a deliberate edit with a reason in the commit;
M4's ads SDK will be the first.

### Building

```powershell
powershell -ExecutionPolicy Bypass -File kit\tools\build_unity.ps1 -Project <unity project> -Method <Class.Method>
```

Games wrap this in their own script (005: `flight-dispatch/unity/build.ps1`). The editor
must be signed in to Unity Hub with a Personal licence; batch mode fails with exit code 198
("No valid Unity Editor license found") when the Hub sign-in has lapsed. That is a human
gate, like the first install (CL-10).

### Not in v0, on purpose

Menus, save, settings, audio, haptics, level select, localization, achievements,
progression, ads, consent, analytics, IAP. Each arrives when a game milestone needs it,
and is pulled out of the game into the kit only if a second game would use it.
