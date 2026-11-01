# PORTFOLIO.md — Casual Lab concept ledger

**Status:** DRAFT, last touched 2026-09-13. Four entries deposited from the Hallway (Gian + ChatGPT,
2026-09-10, originally in Spanish; English from here on). **C02 has passed the gate
(GREEN — Prototype, 2026-09-11); C01 re-charters in its own space; C03 and C04 are
deposited, gate not filled.** Entries are append-only; status changes in place. Ledger numbers (C01…) are line-local;
studio project numbers are assigned at GREEN.
**Build slot: 005 Flight Dispatch**, since 2026-10-31 (prototype closed and promoted to build —
see C02 and CL-17). One game in build at a time (charter §6), so a second concept may be at
gate, not in build, until 005 ships.

## ⚖️ Awaiting Gian (line ledger — unioned by the studio cockpit)

**Effort tags** (studio ledger S15, ruled by Gian 2026-09-13): `⚡ ruling` ≤10 min ·
`🔧 review` ~30 min · `🎯 design` a 90-min block. Back-tagged in this space 2026-09-14.

| id | raised | effort | item | status |
|---|---|---|---|---|
| CL-05 | 2026-09-11 | 🎯 design | Backend posture for the line. Charter §6 makes "backend required = NO" a hard gate floor, but never asks whether a backend is *desirable* — and the standard fixes for weak retention (leaderboards, daily events, cloud save) all require one. Gian reserved this for its own session. Until ruled, every gate floor answer of NO is an answer about that concept's scope, not a line-wide ban | RAISED |
| CL-07 | 2026-09-11 | ⚡ ruling | `../studio/templates/CASUAL_CONCEPT_GATE.template.md` numbering slip: the header says "Gian decides §15", but §15 is the Prototype test and the Decision is §17. Surfaced from the C02 gate run; fix belongs to the studio's own space | RAISED |
| CL-08 | 2026-09-13 | ⚡ ruling | **Reference test device.** Every device number in the spike came from a Galaxy S24 Ultra; the brief asked for mid-range. Which phone is the line's target? Cold start, frame pacing and the sprite ceiling are all unknown below flagship — and C02's dense apron is the concept most exposed to that. Proposed by `kit/SPIKE_REPORT_ENGINE.md` §8.1 | RAISED |
| CL-09 | 2026-09-13 | 🔧 review | **Machine-setup doctrine.** The spike's most important environment finding: toolchains Claude installs from inside the Claude desktop app into a *new* AppData folder are MSIX-redirected into the app's private storage and are **invisible to Gian's own shells**. The Android SDK had to be moved to `C:\Android\Sdk`. Doctrine should require explicit install paths outside AppData, verified from the founder's shell. Compounds under Unity (7.75 GB, licence server, UAC). Proposed by §8.3. **Second instance, 2026-09-13 — same root cause, new victim:** Cowork's own Linux workspace failed to boot, so session CL-S02 could not run git. The mount that failed was **not** the project folder but the session's scratch `outputs` directory, whose real path is `C:\Users\gianv\AppData\Local\Packages\Claude_<pkg>\LocalCache\Roaming\Claude\…` — i.e. `AppData\Roaming` **redirected into the MSIX package container**, exactly the mechanism that hid the Android SDK. The sandbox could not reach it (Plan9 share `c` unmounted, following a Windows update of 2026-09-08) and the failed mount aborted the whole boot, taking `C:\Dev\gabella\casual-lab` with it — a plain path that would likely have mounted on its own. Claude Code was unaffected. So the doctrine needs two clauses, not one: explicit install paths outside AppData, **and** a named authoritative surface for git and builds, because Cowork's shell is not reliably available and its failure mode is all-or-nothing | RAISED — **clause 2 answered by studio S34 (SIGNED 2026-10-14): Claude Code is the named surface for git and builds** (handbook §4); clause 1 (install paths outside AppData) still open |
| CL-10 | 2026-09-13 | ⚡ ruling | **Attended-window ritual.** CL-01 needed Gian physically at the machine for the SDK licence, a Samsung USB driver, a Unity account behind UAC, and the phone. Charter §5 calls this line the low-attention, remotely-drivable slot; device-touching tasks contradict that unless scheduled. The Unity ruling raises the frequency of these windows, not lowers it. Proposed by §8.4 | RAISED |
| CL-11 | 2026-09-13 | ⚡ ruling | **CLI telemetry defaults.** The Capacitor CLI reported "sharing anonymous usage data" on Gian's machine during the spike. Small in itself; machine-setup doctrine should decide the telemetry default for every tool the line installs. Survives the engine ruling — Unity's editor has its own telemetry. Proposed by §8.6 | RAISED |
| CL-12 | 2026-09-13 | 🎯 design | **Unity dependency posture.** Reshaped from §8.2 (which asked for a plugin-abandonment trigger *if the engine is Godot*). Under Unity the monetization bus-factor risk dissolves, but two others replace it: **licence terms** (Unity introduced the Runtime Fee in 2023 and reversed it in 2024 — the line's premise is many small titles with long tails, so terms that moved once can move again) and **first-party package deprecation** — note the spike found Unity's `com.unity.purchasing` 4.15.1 on Billing 8.3.0 (runway to 2027) against the Godot plugin's 9.1.0 (2028), so first-party does not mean most-current. What is the trigger for re-opening the engine question, and what does the line do if Personal's $200K threshold or its terms change? Raised in the Hallway, 2026-09-13 | RAISED |
| CL-13 | 2026-09-13 | ⚡ ruling | **`map-painter/` is versioned nowhere.** The founding checklist's §2 set the line's convention — "`.gitignore` with `/map-painter/` (one line per game slug; **each game is its own nested repo**)" — but the nested repo was never created. `map-painter/` is gitignored by the line repo *and* contains no `.git`, so 002's GDD, its 21 open items and `CHRONICLE_INVARIANTS.md` are untracked on disk with no history and no remote. Found 2026-09-13 while ruling 005's repo posture. Belongs to 002's re-charter session to fix, but the line ledger records it because the line owns the convention | RAISED |
| CL-14 | 2026-09-13 | ⚡ ruling | **Charter §7's rename cost is overstated.** §7 says renaming `casual-lab/` is cheap "only before the first game repo exists," which is what gated 005's repo. But games are **gitignored**, not submodules — the loosest possible coupling. A nested repo carries its own remote and history and does not know what its parent folder is called, so a later line rename costs a directory rename plus one GitHub repo rename; the game repos are untouched. The clause was written for a tighter structure than the one actually built. Proposed rewording: state the real cost and stop treating `[OPEN-N1]` as a gate on founding game repos | RAISED |
| CL-16 | 2026-09-18 | ⚡ ruling | **A web prototype has no in-Cowork device check.** 005's prototype is one HTML page whose portrait layout has to be verified on a real phone — the engine spike's clipped-tray defect (§9) was found only on device. Cowork cannot render it: the in-app browser refuses `file://` and `data:` URLs, and a published render rig was blocked. So the check was made analytically (a box-model floor: 427 px of non-shrinkable height against 568 px on the shortest phone still sold) and the one failure analysis cannot see — mobile browsers size a fixed element to the *large* viewport, putting the drag source under the address bar — was fixed blind with `100dvh`. Neither is a substitute for looking. Every web prototype this line builds inherits the same gap. Interacts with **CL-08** (no reference device named) and **CL-10** (attended windows): charter §5 puts Gian's attention on gates and play, and a device check is neither — it is build correctness that only a human holding a phone can perform. Raised by 005 session FD-S02 | RAISED |
| CL-15 | 2026-09-13 | 🔧 review | **No Prototype Brief template exists.** `../studio/templates/` holds eleven templates; none is a Prototype Brief, though `STUDIO_GLOSSARY.md` defines the artifact's six required contents. 005's Brief was written to that glossary definition and is the line's first. Harvest it back as `templates/PROTOTYPE_BRIEF.template.md` once it has survived one real prototype — studio-side act, like CL-06 and CL-07 | RAISED |

**Dissolved by the CL-01 ruling, recorded rather than dropped:** §8.2 in its original form
(Godot plugin-abandonment trigger — moot; monetization is first-party under Unity, and the
reshaped risk is CL-12) · §8.5 (analytics vendor GameAnalytics vs custom Firebase — moot;
Unity ships first-party Firebase Analytics, v13.16.0, 2026-08-27).

Landed: CL-02 (kit is a tool, not a numbered project — studio S12, 2026-09-10) · CL-03
(authorship posture — studio S13, 2026-09-10) · **CL-04** (first gate = C02 Flight
Dispatch — ruled by Gian, 2026-09-11) · **CL-01** (engine = **Unity 6.3 LTS** — ruled by
Gian, 2026-09-13, this session; see below) · **CL-06** (C02 Flight Dispatch = **studio
project 005** — signed by Gian, 2026-09-13, this session; see below) · **CL-17** (lighter
pipeline: prototype optional, Gian's play tunes, the Play Store closed test is the stranger
test, kill criteria move to the market — ruled by Gian 2026-10-31 in 005 session FD-S05;
`LINE_CHARTER.md` §2).

### CL-06 — landed 2026-09-13

**Signed: C02 Flight Dispatch is studio project 005.** The number was never in doubt —
studio-wide numbering was signed at S11 (2026-09-10), "numbers are for shippable products
only" at S12, and map-painter keeping 002 across a line move set the precedent at S10.
Gian's signature closes it for the line.

**One thread does not close here.** The line ledger can record 005; the *studio* ledger
entry is a clerical act in `../studio/`, which this space never edits (`CLAUDE.md` — "know
these exist; never edit them"). Carried to the studio's own space, alongside **CL-07**,
which is also a studio-side fix. Until that is done, 005 is authoritative in this line's
documents and absent from the studio's.

### CL-01 — landed 2026-09-13

**Ruling: Unity 6.3 LTS.** Signed into `LINE_CHARTER.md` §3, closing `[OPEN-K1]`. Evidence:
`kit/SPIKE_REPORT_ENGINE.md` (frozen Report, sessions 2026-09-12/13, Claude Code, `kit/`
only).

**The ruling goes against the Report's recommendation.** The Report recommended Godot 4.7
on Claude productivity (1 correction vs Unity's 7; ~25 s vs ~60 s edit-to-device; zero
install gates; no permissions requested). Gian ruled on the axis the Report's dissent §6
names as the alternative weighting — **monetization longevity** — plus the 2D content
pipeline, which the spike did not measure in either engine (Unity's slice was built in
uGUI, not the sprite pipeline; the "hand-written triangulator drew the aircraft as shards"
defect is evidence about uGUI's missing polygon primitive, not about Unity's 2D art
tooling).

**Accepted risk, on the record:** slower loops, two human install gates, 7.75 GB, and
**six of seven Unity defects compiling clean** — caught only by review, screenshot or the
phone. Charter §5 puts Gian's attention on the gate and on play, not build review, and he
has no prior experience in any candidate engine; the line therefore has no strong human
reviewer downstream. Three kit conditions attached in charter §3 (pin the manifest, assert
APK permissions at build time, fail the build on unrequested packages) exist to hold the
Report's counter-argument that Unity's defects are one-time and kit-solvable.

**Unblocks:** the kit can start (charter §3), and with it C02's Prototype Brief — which
still also waits on CL-06.

---

## C01 — Map Painter — **studio project 002** (merge pending in its own space)

**Status:** existing project, moved to this line `[RULED by Gian, 2026-09-10 — studio S10]`.
It carries **everything discussed in 002** (GDD, glossary, 21 open items, IDEAS 001) into
the line; the re-charter session decides what sticks, merges, changes or is discarded.
The gate is filled **after** that session. Origin records: `../studio/IDEAS.md` 001 (the
toy, 2026-07-14) and the Hallway casual design (2026-09-10).

**Starting frame for the re-charter:** the studio's toy becomes **Sandbox** — paint
territories, name them, declare invasions and independences, watch each stroke resolve
seconds later, export the time-lapse (the chronicle stays as this game's "substance"). The
casual design wraps it as **Challenges** — a paint meter, optional timer, a rival color
that expands on its own, weak points (capital, port, mountain pass, bridge) that make the
one tactical decision: *paint frontally or reach the weak point first?* Fail = out of
paint / time / special moves; rewarded continue = +20% paint / +30 s. Progression by map:
USA states → Caribbean → Europe → South America → World → regional subdivisions;
unlockable colors, paint styles, flags, conquest effects. Monetization: interstitials
between challenges, rewarded continue, remove ads, map packs.

**MVP sketch (casual side):** one map (USA states), territory select, paint conquest,
adjacency, three terrain types, capital as weak point, Sandbox, five challenges.
**Ad hook:** Florida starts blue; drag Florida → Georgia → South Carolina; mountains slow
you; reroute, capture Atlanta, Georgia floods blue. *Paint. Expand. Conquer.*

**Open at the merge:** does terrain resistance replace or coexist with the toy's "eras"
toggle · does the rival exist in Sandbox (toy says no) · audience (8–12 vs mapping
community — IDEAS 001 [OPEN-1]) · real-map data licensing ([OPEN-2]) · name ([OPEN-3]).

---

## C02 — Flight Dispatch — casual apron management

**Status: GREEN — Build** `[RULED by Gian, 2026-10-31 — prototype closed on the evidence in
hand; verdict below]`. Was GREEN — Prototype `[RULED by Gian, 2026-09-11]`; gate run in the Hallway,
2026-09-11. **Studio project 005** `[SIGNED by Gian, 2026-09-13 — CL-06]`; the studio-side
ledger entry remains as a clerical act in `../studio/`. **Gate complete as of 2026-09-13**
— §17's "Why" is written and CL-01's engine is ruled, so the Prototype Brief is unblocked.
Nothing below binds the design; it was written to decide GREEN, not to specify the
game.

> **➜ The design work has moved to `flight-dispatch/` (its own space and nested repo,
> founded 2026-09-13).** The Prototype Brief lives at
> `flight-dispatch/PROTOTYPE_BRIEF.md` — **SIGNED by Gian 2026-09-15**, with all four
> `[OPEN-FD*]` ruled; it governs the prototype phase and the build is unblocked. **This ledger keeps owning the concept's status**:
> the prototype's verdict (GREEN → pre-production, or killed per §16) is deposited *here*.
> Two rulings made 2026-09-13 when the space was founded:
> **build surface = web**, a single self-contained HTML page, explicitly *not* Unity —
> CL-01's ruling is scoped to the `kit/`, which charter §2 places at pre-production, after
> a successful playtest; and **every tester plays on a touch device**.
> **Build slot still empty** — charter §6: prototype does not occupy it.
>
> **Prototype verdict** `[RULED by Gian, 2026-10-31, session FD-S05]`: **closed on the
> evidence in hand, promoted to build.** Assigning gates and vehicles produces
> deliberation: tester 01 retried seven times with a different plan each time, explained a
> strategy unprompted and waited for the near gate on purpose; tester 02 and Gian's PC pass
> point the same way. Deliberation *against announced arrivals* is **not proven**: nobody
> used pre-dispatch, because the two-step is not discoverable (`[OPEN-FD8]`). That moves
> into build as a design problem, judged by Gian's play. The Brief's pre-registered rule
> (five strangers) was **not met (2 of 5) and was set aside by this ruling**, not
> satisfied; the honesty check moves to the market (charter §2 as rewritten, CL-17).
> **Build slot: now filled by 005.**

### §1 Snapshot
- **Working title:** Flight Dispatch
- **Platform / orientation:** Android first · **portrait** `[RULED by Gian, 2026-09-11]` —
  the scene reads horizontally (terminal left, aircraft nose-left, taxiway right) inside a
  tall screen. Gate lanes stack top to bottom; service tray along the bottom edge.
  Confirmed against Gian's reference mockup, 2026-09-11.
- **Session length target:** ~100 s per shift. Service-vehicle travel and action delays are
  a real timing cost, unsized — a prototype question.
- **Business model:** free + ads (line default)
- **Pitch:** Aircraft land, each needing a different set of services — assign the gate, send
  the right vehicles, and push back clean before the next one is on final.
- **Fantasy:** *keep the whole airport moving.*

### §2 The 3-second test — **PASS**
- **Visual hook:** an aircraft covered in demanding icons going clean, then rolling
  off-screen while the gate light flips green.
- **Condition on the PASS:** need icons are this game's typography — max six in the whole
  game, silhouette-legible with colour stripped.
- **Ad test: YES**, five cuts (scarcity · overload · fail-then-continue · progression ·
  clean-turn ASMR). Recorded caveat: three of the five are one mechanic filmed three ways.
- **Art density** `[RULED by Gian, 2026-09-11]`: the in-game apron stays **dense** — ground
  crew, carts, cones, terminal life. It is service-in-progress feedback and it is what
  keeps the game from reading as a cheap asset flip. Lean recomposition applies to **ad
  creatives and store previews only**, not to the game.

### §3 Core loop
1. Aircraft arrives and occupies a gate → 2. Read its needs → 3. Drag matching service
vehicles → 4. Services complete, aircraft goes clean → 5. Pushback, gate free before the
next arrival.
- **Primary physical action:** drag.
- **Feedback < 1 s:** vehicle moves, crew animates at the stand, need icon becomes a
  filling bar, icon pops on completion.

### §4 Depth without complexity (law 1)
- **Core mechanic:** matching services to needs against a clock.
- **Tactical layer:** *allocation under time pressure* — more claimants than slots, where
  the player must estimate which timer can actually wait. One layer, expressed on whichever
  resource a level makes scarce (gate, or vehicle). The earlier portfolio framing
  ("scarce equipment") and Gian's queue framing ("which aircraft do I pull in next") were
  found to be the same layer on two resources, not two layers.
- **Occasional disruption:** early arrival · delayed departure · VIP · large aircraft
  (gate-restricted) · urgent turnaround · equipment out of service.
- **Complexity check:** every disruption above changes *who you serve next* → all pass.
  **Flagged, unresolved:** service-vehicle travel time passes only if lanes differ in
  distance or vehicles must return to base; a flat per-trip delay is administration.
  Prototype question.

### §5 Failure model (law 2)
- **Fails a level:** accumulated delay penalties cross a threshold → SHIFT FAILED.
- **Lost:** the shift's progress only. No permanent progression loss.
- **Rewarded continue:** "recover one aircraft" — clears the delay on the single worst
  aircraft, which is the specific plane that just sank the shift.

### §6 Progression
- **After level 1:** needs must be matched, and time is the constraint. (Gian's single-gate
  opening makes the queue choice the whole game at level 1 — the tutorial teaches the
  tactical layer rather than the UI.)
- **At level 10:** the new decision is *which claimant waits*.
- **At level 30:** disruptions recombine — VIP + equipment failure ≠ either alone.
- **At 50+:** layouts, traffic density, disruption combinations, objective variants. Never
  a stream of new systems. Portrait constraint accepted: **max six gate lanes on one
  screen**; hub difficulty comes from density, not gate count.

### §7 Content engine
- **How levels are made:** data-driven — a level is a timetable (arrival times, aircraft
  type, need set) + layout + disruption schedule. Hand-tuned early, generator-assisted later.
- **Cheap multiplier:** the timetable. New arrival patterns over the same art.
- **Where AI reduces cost:** timetable generation, difficulty balancing, solvability checks,
  asset ideation, UI variants, store copy, localization, ad creative variants.

### §8 MVP boundary
Smallest version that answers *is matching-under-pressure fun?*
- **Includes:** one small airport · 4 gates · 3 aircraft types · baggage + fuel + catering +
  pushback · arrival queue · delay penalty + fail · retry · 20 shifts.
- **Forbidden during MVP:** accounts, backend, leaderboards, live events, **currency /
  economy**, IAP, cosmetics systems, narrative, ads integration.
- **Content target:** 20 shifts · 1 layout · 4 services · 3 aircraft types · 2 disruptions.

### §9 Scope ceiling (first commercial release)
- **Max core systems: 3** (turnaround, queue/arrivals, progression) · **max mechanics:**
  6 services + 6 disruptions.
- **Backend required? NO** — hard floor passes *for this scope*. See CL-05: this is a
  statement about C02's scope, not a line-wide ruling.
- **Ongoing human operations? NO** — hard floor passes.
- **Maintenance risk: LOW** — no server, no content treadmill, no live economy. The tail is
  ads/consent SDK updates and annual Play target-API compliance, absorbed by the kit across
  every title in the line.

### §10 Monetization fit
- **Primary:** rewarded + interstitial + remove-ads; content packs secondary.
- **Rewarded moments:** recover-one-aircraft continue; pre-shift temporary second vehicle.
- **Interstitial moments:** after shift complete, after fail-and-decline. Never mid-shift.
- **One reasonable purchase:** remove ads, keeping the rewarded continue available.

### §11 Retention hook — *the gate's weakest section*
- **Why reopen tomorrow:** next shift in a run, next airport unlock. That is all.
- **Short lifecycle acceptable? YES** — weeks per player, not months.
- Honest note: without a backend there are no events or leaderboards; the driver is level
  curiosity, which fades. Survivable for free-with-ads, which monetizes session volume, but
  carried into §14 as a named risk rather than papered over. Interacts with CL-05.

### §12 Differentiation
| Comparable | What we share | What materially changes how the player thinks |
|---|---|---|
| Diner Dash / Cooking Fever family | Entities arrive with need-sets; serve under timers | Arrivals are **announced before they land** — the player decides against information about the next 60 s, so play is anticipatory, not reactive |
| Idle Airport Tycoon family | Airport theme, service vehicles, gates | No idle accrual, no building. Every second is an input; nothing earns while away |
| Flight Control / ATC-likes | Aircraft, arrival pressure, airport | No pathing or collision avoidance. Pressure is allocation at a stand, not routing in the air |

**The one thing the churn does not have:** *a visible future.* Aircraft are announced, not
spawned. That converts reaction into **estimation** — the concept's spine.

### §13 Production advantage
Gian's aviation domain familiarity (service sequencing, turnaround realism as texture) ·
Flag Carrier's airport/aircraft knowledge · the line kit (menus, save, ads, consent,
analytics) · AI-assisted timetable generation and solvability checking · studied texture
references (Airport CEO, SimAirport — texture, never scope).
*Devil's advocate correction on record:* Flag Carrier's **data** is not the advantage — a
fictional four-gate apron needs none of it. The advantage is domain familiarity and the
kit, and the kit is shared by every title in the line.

### §14 Risk register
| Risk | Likelihood | Impact | Cheap validation |
|---|---|---|---|
| Tactical layer collapses into "tap fast" — execution, not decision | Medium | High | One gate, three announced arrivals, clickable prototype. Do testers *deliberate*? |
| Differentiation vanishes in implementation — reads as reskinned Diner Dash | Medium | High | Show 5 strangers 30 s of play; ask what it reminds them of |
| Retention too shallow without backend-dependent hooks | High | Medium | Not prototype-testable. Accept short lifecycle, or wait on CL-05 |

### §15 Prototype test
- **Prototype question:** *Does allocation against announced arrivals produce deliberation,
  or does it collapse into tapping?* Not whether the airport is fun; not whether the art
  works.
- **Scope:** 1–2 gates · visible queue of 3–4 announced arrivals with timers · 3 services ·
  no art beyond coloured rectangles and the six icons · 10 hand-written timetables of
  rising density · fail and retry. No continue, no progression, no economy.
- **Who plays:** Gian, plus ≥5 people who did not design it, ≥3 of whom play neither sims
  nor aviation games.
- **Success signal:** players visibly **pause before choosing**; they change approach after
  failing rather than repeating faster; they can say *why* they picked one aircraft; they
  retry unprompted. **Deliberation is the signal — speed is not.**

### §16 Kill criteria
- After two rounds of simplification, testers still tap without deliberating.
- Five strangers play ten levels and can name no meaningful difference from the cooking-game
  family.
- Timetables need heavy hand-tuning per level — §7's cheap multiplier was false.
- MVP cannot reach 20 shifts inside the line's intended span, or entering the MVP requires a
  removal that breaks the core loop.
- Monetization needs live-service or backend hooks to work at all (interacts with CL-05).

### §17 Decision — **GREEN — Prototype**
**Hard floors:** core mechanic satisfaction *(untested — the prototype's purpose)* ·
immediate comprehension PASS · MVP simplicity PASS · differentiation PASS (with the
ad-vs-depth gap noted) · backend required NO · ongoing human operations NO.

**Devil's advocate (Claude):** The differentiation is real but invisible at the point of
sale. §2 passes because chaos converts; §12 claims the spine is estimation over announced
arrivals — something a player feels around level 15, not in a three-second ad. Those answer
different audiences, and the gap is where ad-funded casual games quietly bleed out.

§13 is thinner than it reads. Flag Carrier's asset is route and airport *data*, and a
fictional four-gate apron needs none of it. Strip the data claim and what remains is domain
familiarity and the kit — and the kit is an advantage every title in this line shares, so it
is not a reason to pick this concept first.

The sequencing argument cuts against Flight Dispatch specifically. This is the line's first
title, so it pays to build the kit; it is also the concept with the largest art surface —
moving vehicles, animated crew, multiple aircraft silhouettes, terminal, and a deliberately
dense apron. The kit bill and the heaviest art bill land in the same project at the moment
the line has the least infrastructure. Cargo Move would have been the cheaper way to learn
store publishing, ads and consent. The counter is that Flight Dispatch is the most
commercially legible of the four and the one Gian wants to make, and enthusiasm is a real
production input for a solo dev.

**Why (Gian):** I see a viable product here. Not a big one, but one that could catch some
players — and that is more than I can say for a concept I would only be building in order
to learn from it. Of the four in the portfolio this is the most legible to someone
scrolling a store page, and legibility is what an ad-funded model actually runs on.

The other half is that I want a real, shipped product. This line exists so the studio meets
store publishing, ads, consent and analytics before Flag Carrier needs them, and I cannot
see the inner workings of any of that from a prototype I never release. Flight Dispatch is
the one I want to finish. That answers the sequencing objection in §17's devil's advocate
only partly — Cargo Move would still have been the cheaper way to learn the machinery — but
a cheaper first title I am not motivated to ship would teach the studio nothing about
shipping, which is the whole point of the exercise.

*`[Dictated by Gian in the Hallway, 2026-09-13; drafted from his words by Cowork. Gian's
section — correct in place.]`*

**Signed:** Gian, 2026-09-11. **§17 "Why" completed 2026-09-13 — the gate is now complete.**

**Ad hook (kept from the Hallway deposit):** three aircraft on screen; one asks ⛽🍱🧳, one
is ready, "ARRIVING 00:10" — free the gate in time.

---

## C03 — Route Lines — network puzzle

**Status:** deposited; gate not filled.
**Pitch:** connect cities with routes so the *right* passengers reach their destinations
over a limited network. Not join-the-dots — routing with constraints.
**Fantasy:** *design the perfect route and watch it start working.*
**Loop:** demand appears between cities → draw routes → passengers move → some connect →
adjust the network → level demand met.
**Example:** demand SDQ→MIA 10, SJU→MIA 6, NAS→SDQ 4; build SJU→SDQ→MIA instead of two
separate links.
**Constraints per level:** number of routes · capacity · hubs · time · frequency.
**Special passengers:** business (max one connection) · direct (no connections) · tourist
(must pass through a city) · express cargo (deadline) · flexible.
**Tactical layer:** satisfy the most demand with the simplest network; complexity emerges
as flows share hubs.
**Fail:** an airport saturates or time runs out → retry; rewarded extra route / capacity /
time.
**Progression:** Caribbean → Florida/Southeast → North America → Europe → regional
challenges; later, simple distance or sea-route constraints.
**Monetization:** interstitial after set levels, rewarded route/capacity, remove ads,
region packs.
**MVP:** Caribbean, 8 cities, normal + direct passengers, route limit, connections, 25
puzzles.
**Ad hook:** cities fill with passengers; draw SDQ→MIA, SJU→SDQ; Miami decongests;
"Business Passenger — DIRECT ONLY" forces a redesign.
**Hallway note:** attractive and satisfying, but needs discipline not to become a
management game. Closest to Flag Carrier's territory — the gate's differentiation section
must say why it is not a Flag Carrier mini-mode.

---

## C04 — Cargo Move — spatial loading puzzle

**Status:** deposited; gate not filled.
**Pitch:** fit cargo pieces into an aircraft hold so everything fits **and** can be
unloaded in order along a multi-stop route.
**Fantasy:** *make it all fit — but think about what comes out first.*
**Loop:** route appears → cargo presented, each piece tagged with its destination →
arrange the hold → depart → at each stop the right cargo must be reachable → blocked
cargo fails the plan.
**Example:** SDQ→MIA→JFK with MIA, MIA, JFK, JFK, JFK — the Miami boxes must be reachable
before JFK.
**Special cargo (gradual):** fragile (nothing on top) · heavy (floor only) · refrigerated
(specific zone) · priority (unload first) · oversized (multiple slots) · restricted
aircraft.
**Later variations:** two aircraft, different holds, cargo transferring at a stop,
different cargo doors — always place-and-move as the only interaction.
**Fail:** blocked cargo, doesn't fit, constraint broken → retry; rewarded undo / remove
one item / temporary extra space.
**Progression:** more complex aircraft and holds. No real aviation specs, no weight &
balance — realism is theme, not simulation.
**Monetization:** interstitial between set flights, rewarded undo/continue, remove ads,
cosmetic aircraft/freighter themes.
**MVP:** one aircraft, max three-stop route, normal + fragile + heavy cargo, unload order,
30 puzzles.
**Hallway note:** very clear puzzle with emergent depth from unload order.

---

## Archived

*(none)*
