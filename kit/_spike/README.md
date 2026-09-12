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

Contents:

- `RESEARCH_NOTES.md` — the desk-research phase (ads / consent / IAP / analytics /
  compliance / longevity for Godot, Unity, web + Capacitor), with dated sources. Working
  notes; the Report supersedes them.

Safe to delete this whole folder once the Report is landed and `[OPEN-K1]` is signed.
