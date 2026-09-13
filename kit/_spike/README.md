# kit/_spike/ — DEAD CODE

Everything in this folder is **dead**. It was written during **CL-01**, the engine spike
(`../../HANDOFF_KIT_ENGINE_SPIKE.md`), to answer `LINE_CHARTER.md` §3 `[OPEN-K1]`, and it
was expected to be deleted from the moment it was written.

- It is **not the kit.** The kit's systems (menus, save, settings, audio, haptics, analytics,
  ads, IAP, consent, localization, achievements, level select, progression) were deliberately
  not started here — the brief forbids it.
- It is **not 005 Flight Dispatch**, and must not be grown toward it. The slice exists only
  to exercise what 005 needs, at throwaway quality.
- Do not import from it, depend on it, or copy from it into the kit. If something here turns
  out to be worth keeping, it gets rewritten in the kit under the kit's own invariants.

The only durable output of CL-01 is **`../SPIKE_REPORT_ENGINE.md`** — a Report: produced
once, then frozen.

## Contents

| Path | What it is |
|---|---|
| `RESEARCH_NOTES.md` | Desk research on ads, consent, IAP, analytics, compliance and longevity for all three candidates, with dated sources |
| `MEASUREMENTS.md` | The raw measurement log, in the order things happened, including every correction and every harness mistake |
| `device_results.json` | Output of the first attended device run (cold start, logcat) |
| `godot/` | The Godot 4.7.2 slice |
| `capacitor/` | The web + Capacitor 8.5.2 + Phaser 4 slice |
| `unity/` | The Unity 6.3 LTS slice, UI built in code |
| `DEVICE_WINDOW.md`, `device_window.ps1` | The attended device session: checklist and script |
| `device_autotest.sh` | Automated on-device drag and persistence test (adb touch input) |
| `iteration_loop.sh` | Edit → running-on-device loop timing, with an APK-freshness check |
| `build_unity.ps1`, `shot_unity.ps1` | Unity batch-mode Android build, and an engine-side desktop capture |

Build outputs, caches and dependencies (`build/`, `Library/`, `node_modules/`, `.godot/`,
`android/`, logs) are git-ignored and can be deleted freely.

Safe to delete this whole folder once the Report is landed and `[OPEN-K1]` is signed.
