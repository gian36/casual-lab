# PORTFOLIO.md — Casual Lab concept ledger

**Status:** DRAFT ____-__-__. Four entries deposited from the Hallway (Gian + ChatGPT,
2026-09-10, originally in Spanish; English from here on). None has passed the gate.
Entries are append-only; status changes in place. Ledger numbers (C01…) are line-local;
studio project numbers are assigned at GREEN.
**Build slot:** empty (one game in build at a time — charter §6).

## ⚖️ Awaiting Gian (line ledger — unioned by the studio cockpit)

| id | raised | item | status |
|---|---|---|---|
| CL-01 | founding | Engine for the kit (`LINE_CHARTER.md` [OPEN-K1]) — by spike, per studio S12. The spike is the line's first Claude Code session | RAISED |
| CL-04 | founding | Which concept goes through the gate first? Hallway priority was Map Painter → Flight Dispatch → Cargo Move → Route Lines. Map Painter carries a re-charter first (002's space), so the first *clean* gate may be Flight Dispatch or Cargo Move | RAISED |

Landed at founding: CL-02 (kit is a tool, not a numbered project — studio S12, 2026-09-10)
· CL-03 (authorship posture — studio S13, 2026-09-10).

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

**Status:** deposited; gate not filled.
**Pitch:** Diner Dash at an airport. Each aircraft arrives with needs; assign a gate, send
the services, push it back before the next arrival.
**Fantasy:** *keep the whole airport moving.*
**Loop:** aircraft arrives → assign gate → read needs → dispatch services → ready →
pushback → free the gate before the next arrival. Others keep landing meanwhile.
**Services:** baggage, fuel, catering, cleaning, lavatory, pushback — not every flight
needs all.
**Tactical layer:** scarce equipment. One catering truck, two aircraft need it, one leaves
in 45 s and one in 90 s — which first?
**Disruptions (introduced gradually):** early arrival · delayed departure · VIP flight ·
large aircraft (only some gates) · urgent turnaround · equipment out of service.
**Fail:** delays accumulate penalties → SHIFT FAILED → retry, or rewarded "recover one
aircraft". No permanent loss.
**Progression:** small regional → Caribbean airport → international terminal → major hub
→ special scenarios; gates and services added gradually.
**Monetization:** interstitial between shifts, rewarded continue, rewarded temporary
service boost, remove ads, cosmetic airport themes.
**MVP:** one small airport, 4 gates, 3 aircraft types, baggage + fuel + catering +
pushback, 20 shifts.
**Ad hook:** three aircraft on screen; one asks ⛽🍱🧳, one is ready, "ARRIVING 00:10" —
free the gate in time.
**Hallway note:** probably the most immediately legible commercially, strong visual
potential. Production advantage: Flag Carrier's airport/aircraft knowledge and data.

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
