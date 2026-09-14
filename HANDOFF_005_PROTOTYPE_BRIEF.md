# HANDOFF_005_PROTOTYPE_BRIEF.md — stand up 005's space, then write its Prototype Brief

**Status: CLOSED 2026-09-13** (session CL-S03). The space `flight-dispatch/` exists with its
`CLAUDE.md`, `SESSION_LOG.md` and `AUTHORSHIP.md`; `PROTOTYPE_BRIEF.md` is written inside it
and is DRAFT awaiting Gian's signature. Both questions this handoff told the session to put
to Gian were ruled: **repo = its own nested repo** (the founding checklist's convention —
this handoff's premise that `map-painter/` set a folder-only precedent was wrong; it is
gitignored and has no repo at all, now raised as CL-13), and **build surface = web**, not
Unity. `../studio/` had to be attached to settle both — it also confirmed this handoff's
suspicion that **no Prototype Brief template exists** (CL-15); the glossary's definition
supplied the structure instead. Repo creation and commit are handed to Gian: git runs
outside Cowork by studio doctrine, and CL-09 fired a third time.

**Status (original):** OPEN, written 2026-09-13 in the line's Cowork space (session CL-S02).
**Surface:** Cowork.
**Produces:** `casual-lab/flight-dispatch/` (the 005 space) and, inside it, the
**Prototype Brief** for **005 Flight Dispatch**.
**Authority:** the Brief is written by Cowork and scopes the prototype. It does not design
the game and it does not rule anything — `[OPEN]`s go to Gian.

---

## Why this handoff exists at all

`SESSION_LOG.md` CL-S02's baton says "write C02's Prototype Brief". That is the
destination, not the next step. Charter §2's pipeline is:

```
GREEN → project: folder + repo + studio number + Cowork space → PROTOTYPE BRIEF (not a GDD)
```

and `CLAUDE.md` says each game that passes the gate "gets its own folder, repo, number and
Cowork space (`casual-lab/<slug>/`) and is worked there, **never here**." The gate closed
on 2026-09-13. **The Brief therefore does not belong in the line space.** Stand up the
space first, then write the Brief inside it.

## Preconditions — all met as of 2026-09-13

- **Gate complete.** C02 §17 GREEN — Prototype `[RULED by Gian, 2026-09-11]`; §17's "Why"
  written 2026-09-13. Note the "Why" is Cowork's draft of Gian's Hallway words and is
  marked for his correction — if he has since edited it, that text wins.
- **Number signed.** CL-06: **studio project 005**. The studio-side ledger entry is still
  an outstanding clerical act in `../studio/`; 005 is authoritative in this line's docs.
- **Engine ruled.** CL-01: **Unity 6.3 LTS** (`LINE_CHARTER.md` §3). See the caveat below —
  this does *not* automatically mean the prototype is built in Unity.

## Not a blocker, and do not treat it as one

**The kit does not exist yet, and the prototype does not need it.** Charter §2 puts the kit
at *pre-production*, after a successful playtest: `prototype → playtest → success signal? →
yes → pre-production on the kit → MVP`. The kit's own first session runs in parallel in
Claude Code, in `kit/`. Do not sequence this behind it, and do not build kit systems here.

## Step 1 — stand up `casual-lab/flight-dispatch/`

- **Check with Gian before `git init`.** Charter §7 says renaming the line folder is cheap
  "only before the first game repo exists", and `[OPEN-N1]` (the line's final public name,
  studio ledger S08) is still unruled. `map-painter/` already exists as a folder and
  CL-S01 flagged this; creating a second game *repo* makes the working slug `casual-lab/`
  concretely expensive to change. One question to Gian, not a blocker — but ask it before
  the repo exists, not after.
- **Slug:** `flight-dispatch`.
- **Minimum contents**, following the `map-painter/` precedent and studio doctrine:
  its own `CLAUDE.md` (constitution for that space, naming 005, the engine ruling, and its
  relationship to the line), `SESSION_LOG.md`, and `AUTHORSHIP.md` per charter §4 (one
  paragraph: who directs, what Claude produces, what tools generate).
- Cowork's Linux shell was wedged during CL-S02 (see CL-09) — `git init` may need Claude
  Code or PowerShell.

## Step 2 — write the Prototype Brief

**Look for a template first.** Check `../studio/templates/` for a Prototype Brief template
before inventing a structure; the line already has `CASUAL_CONCEPT_GATE.template.md` there.
CL-S02 could not reach the studio layer to confirm one exists. If none does, say so and
propose the structure rather than silently inventing a house format.

### What it is, and is not

- It is the smallest written thing that lets someone build the prototype and get an honest
  answer to one question. It scopes and instruments a test.
- **It is not a GDD.** Charter §6's "no design doc before GREEN" does not become "write the
  GDD now" the moment the gate closes. Nothing in the gate binds the design (C02's own
  preamble says so); the Brief must not quietly promote gate prose into specification.
- Scope smuggling applies with extra force (charter §6). Everything the gate forbade during
  MVP is doubly forbidden in a prototype.

### The one question it serves — C02 §15

> **Does allocation against announced arrivals produce deliberation, or does it collapse
> into tapping?**

Not whether the airport is fun. Not whether the art works. The Brief exists to make that
question answerable and to make a negative answer *visible* rather than survivable.

### Carry these in from the gate, unchanged

- **Scope (§15):** 1–2 gates · a visible queue of 3–4 announced arrivals with timers ·
  3 services · no art beyond coloured rectangles and the six icons · 10 hand-written
  timetables of rising density · fail and retry. **No continue, no progression, no economy.**
- **Who plays (§15):** Gian, plus ≥5 people who did not design it, ≥3 of whom play neither
  sims nor aviation games.
- **Success signal (§15):** players visibly pause before choosing; they change approach
  after failing rather than repeating faster; they can say *why* they picked one aircraft;
  they retry unprompted. **Deliberation is the signal — speed is not.**
- **Kill criteria (§16)**, in full. A prototype brief that omits them is a brief that cannot
  fail.
- **Portrait**, max six gate lanes on one screen (§6); need icons max six in the whole game,
  silhouette-legible with colour stripped (§2).

### Design questions the gate explicitly left to the prototype

These are flagged *unresolved* in C02 and must be instrumented, not assumed:

1. **Service-vehicle travel time (§4).** Passes as a tactical layer only if lanes differ in
   distance or vehicles must return to base. **A flat per-trip delay is administration, not
   depth.** The Brief must make this testable.
2. **Session length (§1).** ~100 s per shift is a target; travel and action delays are
   unsized.
3. **Contention** — the tactical layer itself. The engine spike kept unlimited vehicles
   (`VEHICLE_POOL = -1`) and never tested whether scarcity reads on a phone
   (`kit/SPIKE_REPORT_ENGINE.md` §9). This is the prototype's core subject; it cannot be
   deferred again.

### Carry these observations from the engine spike (§9, observations only)

- **Start the shift clock on the first rendered frame**, whatever the prototype is built in.
  All three engines drifted at start-up; Godot by ~0.13–0.17 s, Unity by ~2 s until fixed.
- **Check the portrait layout on a tall phone early.** One engine's default scaling clipped
  the bottom tray off-screen on a screen taller than 9:16, and it was found only on device.
- Drag is viable and cost no dropped frames at 120 Hz; sprite count is not a risk **on a
  flagship**. The line has no reference mid-range device yet — CL-08.

### Two things the Brief must decide or raise, not assume

- **What is the prototype built in?** Unity is ruled *for the kit*, and the kit is a
  pre-production concern. §15's scope is coloured rectangles and six icons, and §14's cheap
  validation says "clickable prototype" — which a web page answers in less time than a Unity
  project takes to configure. Against that: building it in Unity doubles as the line's first
  Unity work and avoids throwing the result away. **This is a real choice with a real
  trade-off. Put it to Gian; do not default to Unity because §3 says Unity.**
- **Does the prototype need to run on a phone at all**, given §15's question is about
  deliberation rather than performance, and CL-08 has not named a reference device? Drag on
  a touchscreen may or may not be load-bearing for the answer.

## Ledger context — un-landed rows the next session must report

Per the session protocol, report all of these before new work: **CL-05** (backend posture —
reserved by Gian for its own session; interacts with C02 §11 retention and §16), **CL-07**
(studio-side template fix), **CL-08** (reference test device — touches this Brief directly),
**CL-09** (machine-setup doctrine), **CL-10** (attended-window ritual — a device playtest is
exactly the kind of window it is about), **CL-11** (CLI telemetry), **CL-12** (Unity
dependency posture). Also outstanding: CL-06's and CL-07's studio-side acts.

## Exit

- `casual-lab/flight-dispatch/` exists with its `CLAUDE.md`, `SESSION_LOG.md` and
  `AUTHORSHIP.md`; repo created only if Gian cleared charter §7's naming question.
- The Prototype Brief is written **inside that space**.
- `PORTFOLIO.md`'s C02 entry is annotated to point at the new space (the line ledger keeps
  owning the concept's status; the design work moves).
- New `[OPEN]`s raised in `PORTFOLIO.md`, never resolved silently.
- `SESSION_LOG.md` (this space) gets its 5 lines; the new space starts its own.
- Commit. This handoff is marked CLOSED when the Brief lands.
