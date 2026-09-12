# LINE_CHARTER.md — Casual Lab (working name)

**Status:** DRAFT, founded from `studio/HANDOFF_CASUAL_LAB_FOUNDING.md` on ____-__-__.
Signed clauses carry `[RULED by Gian, date]`; unsigned ones are proposals.
**Purpose:** everything that makes this line different from the Core line, in one place.
What is not here is inherited unchanged from `../studio/STUDIO_HANDBOOK.md`.

## 0. Why the line exists

Core projects are long. This line produces small, finished, monetized mobile games in
short spans, mostly remotely and at low founder attention; it gets the studio through
store publishing, ads, consent and analytics before Flag Carrier needs them; and it gives
Hallway ideas a fast, honest place to be tried or archived. It does not diminish Core.

## 1. Line design laws (replace Core laws 1–2, handbook §1.1) `[RULED by Gian, 2026-09-10]`

1. **One mechanic, one tactical layer, one occasional surprise.** A new rule must change a
   player decision, not add administration.
2. **No permanent failure.** A level fails; the player's career never does. Every fail
   state has a natural rewarded continue that solves a real, immediate problem.
3. **Instant comprehension.** Three seconds of footage explains the game. The fantasy
   comes from the system; no narrative, no characters, no exposition.

Shared principles beneath the laws: simple, not trivial · short sessions · one deliberate
difference from the churn per title (gate §12) · new work per game goes into the mechanic,
everything else comes from the kit.

## 2. Pipeline (replaces handbook §3.1 exploration for this line)

```
Hallway idea → CONCEPT GATE (phone, 20 min) → PORTFOLIO.md entry
   RED    → archived in place, reason written
   YELLOW → revised in place
   GREEN  → project: folder + repo + studio number + Cowork space
            → PROTOTYPE BRIEF (not a GDD) → prototype → playtest → success signal?
               no  → kill per the entry's kill criteria (a successful use of the line)
               yes → pre-production on the kit → MVP → store release → maintenance tail
```

Gates keep the studio shape: written criteria, devil's advocate by Claude, signed decision
by Gian. There is no score; the hard floors in the template decide.

## 3. The kit rule `[RULED by Gian, 2026-09-10]`

`kit/` is the line's shared base, a means to accelerate and facilitate production: menus, save, settings, audio, haptics, analytics, ads,
IAP, consent, localization, achievements, level select, progression. Line games depend on
it. **Core projects never depend on the kit or on each other**; they may borrow a proven
piece with adaptation, recorded in their own DECISIONS.md.

- `[OPEN-K1]` **Engine.** Godot · Unity · web + Capacitor. Ruled by spike `[RULED by
  Gian, 2026-09-10]`: which gives Claude the highest productivity *and* the smoothest
  ads/consent/IAP path on Android.
- **The kit is a tool, not a project** `[RULED by Gian, 2026-09-10]` — studio numbers are
  for shippable products. It lives in `kit/` with its own roadmap and invariants, no
  number. It is allowed to be ugly. It ships nothing by itself.

## 4. Authorship and rights posture (lighter than handbook §11) `[RULED by Gian, 2026-09-10]`

The floor below may be deepened per title if the need appears or as the operation is
better understood.

- One-paragraph `AUTHORSHIP.md` per game: who directs, what Claude produces, what tools
  generate. Honest, short, not a devlog.
- Generation log for every asset (prompt, tool, date) — required for store disclosure
  anyway.
- Disclosure follows whatever the store requires at submission; check at each release.
- Name clearance before any store listing. Registration per title, only when earned.
- This is not legal advice; a lawyer's read is a per-title decision.

## 5. Cadence and attention

- Line sessions run alongside a Core session inside the studio parallelism budget
  (2 normal, 3 max). They are the low-attention slot and may be driven remotely.
- Gian's attention on the line: fill gates, play prototypes, rule `[OPEN]`s. Not build
  review — the kit and the gate exist so that build sessions need none.
- Exit protocol as studio: commit → Status lines → SESSION_LOG (5 lines) → semaphores.

## 6. Guards

- **One line game in build at a time** `[RULED by Gian, 2026-09-10]`. A second may be at
  gate or prototype, never in pre-production. Phased overlap (starting the next while the
  first is in its final phases) is a possible exception to rule on after game one ships.
- **The tail is real.** A shipped game is an ongoing obligation. Two hard gate floors:
  **backend required = NO** and **ongoing human operations = NO** `[RULED by Gian,
  2026-09-10 — "low maintenance" as a floor discarded as too ambiguous]`. Maintenance risk
  is judged in the gate, not a floor. The number of live titles is reviewed at every
  release.
- **No design doc before GREEN.** Enthusiasm is written into the gate, nowhere else.
- **Scope smuggling** (handbook anti-pattern 5) applies with extra force: entering the MVP
  requires a removal.

## 7. Identity

- Public: "Gabella Games — Casual Lab" (working). It will mostly be a mark on a loading
  screen; the name must survive at that size. `[OPEN-N1]` final name — studio ledger S08.
- Internal folder: `casual-lab/` (working slug; renaming a folder later is cheap only
  before the first game repo exists).

## Changelog
- ____-__-__ — founded from the studio handoff; clauses pending signature.
