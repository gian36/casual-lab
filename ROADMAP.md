# ROADMAP.md — Casual Lab (working name)

**Status:** line founded 2026-09-11. **In flight:** 005 Flight Dispatch at prototype —
space founded, Brief written and **awaiting Gian's signature**. **Waits on:** Gian —
sign `flight-dispatch/PROTOTYPE_BRIEF.md`, rule `[OPEN-FD1]`–`[FD4]`, and CL-05.
**Build slot: EMPTY** (charter §6 — one line game in build at a time; a prototype does
not occupy the slot, pre-production does).

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
| 005 Flight Dispatch | **Cowork — 005's own space** | `flight-dispatch/` |
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
| **Sign the Brief + rule FD1–FD4** | 005 | Cowork line | ⛔ | **Blocked on Gian.** Everything below waits on this |
| Build `proto/` — 10 timetables, §5 switches, event log | 005 | **Cowork 005 space** | 🔴 | Solvability verified before any testing |
| Six watched playtests | 005 | device | 🔴 | ≥5 strangers, ≥3 non-sim players; touch device each |
| **Prototype verdict** → GREEN or kill | 005 | Cowork line | 🔴 | Deposited in `PORTFOLIO.md` C02. The line's next real decision point |

## Next (not started, not blocking)

| Milestone | Workstream | Surface | Status | Notes |
|---|---|---|---|---|
| Kit v0 on Unity 6.3 | Kit | Claude Code | 🔴 | Charter §3's three conditions are kit invariants. **Not urgent** — charter §2 puts the kit at pre-production, and 005's prototype is web |
| CL-05 backend posture | Operations | Cowork line | ⛔ | Gian reserved it for its own session. Gates C02 §11 and §16's last kill criterion |
| 002 re-charter → merged GDD v0.2 | 002 | Cowork 002 space | 🔴 | Also fixes **CL-13** — `map-painter/` is versioned nowhere |
| Gate C03 Route Lines · C04 Cargo Move | Gate | Cowork line | 🔴 | Charter §6 allows a second concept at gate while 005 prototypes |
| Studio-side acts: CL-06, CL-07, CL-15 | Operations | Cowork studio | 🔴 | 005's number in the studio ledger, gate template fix, Brief template harvest |
| ~~Register the line in `hephaestus.json`~~ | Operations | Cowork studio | ⛔ | **Withdrawn 2026-09-14 → studio ledger S17.** The premise was wrong: the cockpit already reads `PORTFOLIO.md` directly (HANDOFF_COCKPIT, 2026-09-11 amendment), the pulse file is generator *output* carrying an id issued by HEPHAESTUS, and a line is not a project. Hand-writing one would be the rival instrument the preamble forbids. Gian rules whether lines get a pulse at all |
| 005's `CLAUDE.md` is missing the studio preamble | 005 | Cowork 005 space | 🔴 | Written 2026-09-13, one day before S16 was signed. Cockpit v2 §B2 will render `preamble: missing`; no auto-fix — the owning space repairs it. See studio ledger S18 |

## Later (after a prototype passes)

Pre-production on the kit → MVP (C02 §8's boundary) → store release → maintenance tail.
Entering the MVP **requires a removal** (charter §6). Reference device (**CL-08**) must be
named before pre-production; the web prototype defers it, it does not answer it.

## End-of-week results (history)

_(none yet — the first W session opens this log. Demo-able only.)_
