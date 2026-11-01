# ROADMAP.md — Casual Lab (working name)

**Status:** line founded 2026-09-11. **In flight:** 005 Flight Dispatch in **build** since
2026-10-31: prototype closed on the evidence in hand and the pipeline lightened (CL-17,
charter §2). Next: 005's lean GDD and **kit v0 on Unity 6.3**, both in Claude Code.
**Waits on:** Gian — **CL-08** reference phone (⚡, now due) and studio **S38** (⚡); older rows
in `PORTFOLIO.md`'s ledger.
**Build slot: 005** (charter §6 — one line game in build at a time).

Single source of truth for **status**. Semaphores: 🔴 not started · 🟡 in progress ·
🟢 done (demo-able) · ⛔ blocked (name the blocker). "Done" means demo-able.

**Not duplicated here:** the ⚖️ decisions waiting on Gian live in `PORTFOLIO.md`'s ledger
and nowhere else. Concept *status* (RED/YELLOW/GREEN) lives in `PORTFOLIO.md` too. This
file tracks *work*; that file tracks *concepts and decisions*. If they ever disagree,
`PORTFOLIO.md` wins on concepts and decisions, this file wins on work.

## Where each thing is worked

| Workstream | Surface | Folder it may touch |
|---|---|---|
| Line governance, gates, portfolio | **Cowork — this space** | `casual-lab/` root only |
| The kit | **Claude Code** | `kit/` only |
| 005 Flight Dispatch | **Claude Code — 005's own folder** | `flight-dispatch/` |
| 002 Map Painter | **Cowork — 002's own space** | `map-painter/` |
| Studio doctrine, templates, ledger | **Cowork — studio space** | `../studio/` |
| Git, builds, installs | **PowerShell / Claude Code** | anywhere — never Cowork (CL-09) |

## Now

| Milestone | Workstream | Surface | Status | Notes |
|---|---|---|---|---|
| Line founding — charter, portfolio, gate | Operations | Cowork line | 🟢 | CL-S01, 2026-09-11 |
| Engine question CL-01 → **Unity 6.3 LTS** | Kit | Code + Cowork | 🟢 | Spike report frozen; ruled 2026-09-13 |
| C02 gate → **GREEN — Prototype** | Gate | Cowork line | 🟢 | Signed 2026-09-11; §17 "Why" 2026-09-13 |
| 005 space + Prototype Brief | 005 | Cowork line | 🟢 | CL-S03; both repos pushed 2026-09-13 |
| Sign the Brief + rule FD1–FD4 | 005 | Cowork line | 🟢 | **All four ruled and the Brief SIGNED 2026-09-15.** The Brief now governs the phase |
| Build `proto/` — 10 timetables, §5 switches + `QUEUE_VISIBLE`, event log | 005 | **Cowork 005 space** | 🟢 | Built FD-S02, 2026-09-16; frozen as a record 2026-10-31. Solvability verified before any testing; install the studio preamble first (S18) |
| ~~Six watched playtests~~ | 005 | device | — | **Withdrawn 2026-10-31 (CL-17).** 2 of 5 strangers played; the rest never came. The store's closed test replaces the cohort |
| **Prototype verdict** → GREEN or kill | 005 | Code 005 | 🟢 | **Closed → build, 2026-10-31** (FD-S05). Deposited in `PORTFOLIO.md` C02 |
| 005 lean GDD: MVP boundary + the one removal | 005 | Code 005 | 🟢 | **v0.1 SIGNED 2026-10-31** (FD-S07). Milestones M1–M5 in its §10; M1 needs kit v0 and CL-08 |
| Kit v0 on Unity 6.3 | Kit | Claude Code | 🔴 | **Now the long pole**: every store release needs it. Charter §3's three conditions are kit invariants. Needs CL-08 |

## Next (not started, not blocking)

| Milestone | Workstream | Surface | Status | Notes |
|---|---|---|---|---|
| ~~Kit v0 on Unity 6.3~~ | Kit | Claude Code | — | Moved to **Now**, 2026-10-31 |
| CL-05 backend posture | Operations | Cowork line | ⛔ | Gian reserved it for its own session. Gates C02 §11 and §16's last kill criterion |
| 002 re-charter → merged GDD v0.2 | 002 | Cowork 002 space | 🔴 | Also fixes **CL-13** — `map-painter/` is versioned nowhere |
| Gate C03 Route Lines · C04 Cargo Move | Gate | Cowork line | 🔴 | Charter §6 allows a second concept at gate while 005 prototypes |
| Studio-side acts: CL-06, CL-07, CL-15 | Operations | Cowork studio | 🔴 | 005's number in the studio ledger, gate template fix, Brief template harvest |
| ~~Register the line in `hephaestus.json`~~ | Operations | Cowork studio | ⛔ | **Withdrawn 2026-09-14 → studio ledger S17.** The premise was wrong: the cockpit already reads `PORTFOLIO.md` directly (HANDOFF_COCKPIT, 2026-09-11 amendment), the pulse file is generator *output* carrying an id issued by HEPHAESTUS, and a line is not a project. Hand-writing one would be the rival instrument the preamble forbids. Gian rules whether lines get a pulse at all |
| 005's `CLAUDE.md` is missing the studio preamble | 005 | Cowork 005 space | 🔴 | Written 2026-09-13, one day before S16 was signed. Cockpit v2 §B2 will render `preamble: missing`; no auto-fix — the owning space repairs it. See studio ledger S18 |

## Later

MVP (C02 §8's boundary) → Play Store closed test (12 testers × 14 days) → store release →
keep or retire on the numbers (charter §2, CL-17).
Entering the MVP **requires a removal** (charter §6). Reference device (**CL-08**) must be
named before pre-production; the web prototype defers it, it does not answer it.

## End-of-week results (history)

_(none yet — the first W session opens this log. Demo-able only.)_
