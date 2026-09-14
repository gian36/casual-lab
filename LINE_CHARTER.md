# LINE_CHARTER.md — Casual Lab (working name)

**Status:** DRAFT, founded from `studio/HANDOFF_CASUAL_LAB_FOUNDING.md` on 2026-09-11.
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

- **Engine: Unity 6.3 LTS** `[RULED by Gian, 2026-09-13 — closes [OPEN-K1]]`. The question
  (Godot · Unity · web + Capacitor: highest Claude productivity *and* smoothest
  ads/consent/IAP path on Android) was answered by spike CL-01 —
  `kit/SPIKE_REPORT_ENGINE.md`, sessions 2026-09-12/13.
  - **The ruling goes against the Report's recommendation**, which was Godot 4.7 on the
    productivity axis. Gian ruled on the axis the Report's own dissent (§6) names as the
    alternative weighting: **monetization longevity**. Ads, consent and billing are
    first-party — Google and Unity ship the fixes. On Godot they rest on community plugins
    with a bus factor of about one, and a lapsed UMP or Play policy change hits every live
    title at once; forking and maintaining an Android plugin is the ongoing human
    operations **this charter's** §6 makes a hard floor. Unity also carries first-party
    Firebase analytics, which the Report calls Godot's weakest axis.
  - **A second reason was given that the spike does not support.** Gian also weighed
    Unity's 2D content pipeline (sprite atlasing, PSD import, skeletal 2D). Unity is ahead
    there on current tooling, but **CL-01 measured it in neither engine** — the Unity slice
    was built in uGUI, not the sprite pipeline, and the one 2D data point the spike does
    contain runs the other way (uGUI has no polygon primitive, so the aircraft outline
    needed a hand-written triangulator where Godot and Phaser drew it in one call). This
    part of the ruling rests on Gian's judgement, not on CL-01. Recorded as such so that a
    later reader does not mistake it for spike evidence.
  - **Accepted risk, in exchange:** ~60 s edit-to-device loops against Godot's ~25 s; two
    human install gates against zero (measured cost: one account creation, one sign-in,
    one licence click); 7.75 GB installed; and the defect profile that matters most to
    this line — **six of Unity's seven spike defects compiled clean** and were caught only
    by review, screenshot or the phone. §5 puts Gian's attention on the gate and on play,
    not build review, and he has no prior experience in any candidate engine `[stated by
    Gian, Hallway, 2026-09-13]`, so this line has no strong human reviewer downstream. The
    Report argues these are largely one-time and largely solvable by the kit; the ruling
    accepts that argument and the three conditions below exist to hold it.
  - **The one fact that cuts against the ruling's own axis, recorded rather than omitted:**
    on Play Billing runway, Godot was *ahead*. The Godot billing plugin ships Billing 9.1.0
    (runway to 2028) — the newest of the three candidates — while `com.unity.purchasing`
    4.15.1 as installed declares Billing 8.3.0 (runway to 2027). Unity's advantage on this
    axis is **who fixes it**, not how current it is today. Carried into CL-12.
  - **Conditions attached to the ruling** (kit invariants — `kit/` implements, Cowork does
    not): **(a)** the package manifest is pinned; **(b)** the APK's permissions are
    asserted at build time; **(c)** the build fails on any package the editor added
    unrequested. During the spike Unity silently added IAP and analytics packages, which
    for an ads-funded line is a governance question, not an annoyance.
  - **Licence:** Personal, free below $200K revenue+funding, splash optional (pricing
    research 2026-09-12; **verified against Gian's own licence with a real build,
    2026-09-13**). Runtime Fee cancelled 2024-09-12. Watched, not assumed — see
    `PORTFOLIO.md` CL-12.
  - **Not chosen:** Godot 4.7 (Report's recommendation), web + Capacitor (Report's
    runner-up). Both remain on the record in the Report; neither is revisited without a
    new `[OPEN]`.
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
- 2026-09-13 — §3 `[OPEN-K1]` signed: engine = **Unity 6.3 LTS**, against the spike
  Report's recommendation, on the dissent's weighting. Three kit conditions attached.
- 2026-09-11 — founded from the studio handoff; clauses pending signature.
