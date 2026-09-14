# SESSION_PLAYBOOK.md — Casual Lab

The standing agenda. At the start of any session in **this** space, Claude reads
`CLAUDE.md` + this file + `ROADMAP.md`, reports which sessions are unlocked, and asks which
to run. Each session below has a **paste-ready ask** — copy it as your first message.
A session that doesn't commit didn't happen.

**If you are lost, run B.** That is what it is for.

**Roles:** Gian = Creative Director, final authority on every `[OPEN]`. Cowork = the line
office (governance, gates, portfolio, briefs). Claude Code = engineering (`kit/`, and any
game's code). Chat = the Hallway; decisions made there are deposited in writing or they
did not happen.

---

## Which space am I supposed to open?

| I want to… | Open | Session |
|---|---|---|
| Know where I am | **this space** | **B** |
| Rule things that are waiting on me | this space | **R** |
| Put a new concept through the gate | this space | **G** |
| Found a space for a concept that passed | this space | **S** |
| Design or build a specific game | **that game's own space** | **P** |
| Work on the kit | **Claude Code**, `kit/` | **K** |
| Review the week | this space | **W** |
| Think out loud about anything | this space | **X** |

**Session IDs:** `CL-S##` in this space · `<SLUG>-S##` in a game space (e.g. `FD-S01`) ·
`KIT-S##` in Claude Code. Sequential per space, never reused.

---

## B — Status Briefing (on demand, ~5 min) — **the one to run when you come back**

**Ask:** *"Run session B. Read `ROADMAP.md`, `PORTFOLIO.md`'s ledger, `SESSION_LOG.md`, and
every `HANDOFF_*` Status line. Give me the one-page state of the line: ⚖️ decisions waiting
on ME (first, if any) · ✅ done since the last briefing · 🟡 in flight and its next action ·
⛔ blocked on what · 🔜 what's next. Say which space I should open. End with the single
highest-value session I could run today, and the paste-ready ask for it."*

**Output:** nothing written — a briefing. **Exit:** you know what to run next.

## R — Ruling session (work the ⚖️ ledger)

**Unlocks:** whatever the row was blocking. **Read first:** `PORTFOLIO.md`'s ledger.
**Ask:** *"Run session R on [CL-nn, or 'whatever is cheapest to clear']. Give me the row,
what it blocks, and the options with their real costs — devil's advocate on the one you
recommend. WAIT for my ruling; don't batch rows. Land each ruling in `PORTFOLIO.md` with a
landing note, and amend `LINE_CHARTER.md` if the ruling is a law."*

**Output:** landed ledger rows. **Exit:** each row moved to Landed or annotated with why not.

## G — Concept Gate (~20 min, phone-friendly)

**Unlocks:** a concept's status. **Read first:** `../studio/templates/CASUAL_CONCEPT_GATE.template.md`,
the concept's `PORTFOLIO.md` entry. **Ask:** *"Run session G on [concept]. Take me through
the Concept Gate in the Hallway style — 3–4 sections per turn, you draft, I correct only
what's wrong. Devil's advocate at §12–§14. Hard floors decide; there is no score. I sign
§17. Deposit the filled gate as the `PORTFOLIO.md` entry."*

**Output:** a filled gate entry. **Exit:** RED archived in place with a reason · YELLOW
revised in place · GREEN → run **S**.

## S — Found a space (a concept went GREEN)

**Read first:** charter §2, `../studio/templates/NEW_GAME_BOOTSTRAP.md`, the gate entry.
**Ask:** *"Run session S for [concept]. Stand up `casual-lab/<slug>/` with its own
`CLAUDE.md`, `SESSION_LOG.md`, `AUTHORSHIP.md` (charter §4's one-paragraph floor), and write
its **Prototype Brief** — the glossary's six contents, not a GDD. Carry the gate's scope,
testers, success signal and kill criteria unchanged. Raise what the gate left open as
`[OPEN-<XX>n]`; don't resolve them. Add the slug to `.gitignore` and hand me the git
commands."*

**Output:** the game's space + its Brief. **Exit:** both repos pushed; `PORTFOLIO.md` points
at the new space; `ROADMAP.md` rows added.

## P — Prototype / game session — **runs in the game's own space, not here**

**Ask (in that space):** *"Run session P. Read `CLAUDE.md` and `PROTOTYPE_BRIEF.md`, report
any un-landed `[OPEN-*]`, then [the next thing on its `SESSION_LOG` baton]. Build only what
the Brief scopes — everything the gate forbade in the MVP is doubly forbidden here."*

**Exit:** `SESSION_LOG` 5 lines in that space; commit; line `ROADMAP.md` semaphore updated.

## K — Kit session — **runs in Claude Code, `kit/` only**

**Ask:** *"Run session K. Read `LINE_CHARTER.md` §3 and the active kit handoff. Charter §3's
three conditions are invariants: pin the package manifest, assert APK permissions at build
time, fail the build on any package the editor added unrequested. Install paths explicit and
outside AppData (CL-09). Propose `[OPEN]`s to the line space; never edit line docs."*

**Exit:** kit roadmap updated; commit. Cowork proposes kit `[OPEN]`s, never edits `kit/`.

## W — Weekly review (~20 min)

**Ask:** *"Run session W. Read `SESSION_LOG.md` and the git log since last week. Write
End-of-Week Results into `ROADMAP.md` history — what now WORKS, demo-able only. Flag stalled
semaphores and any ledger row older than two weeks. Name the most valuable session for next
week."*

## X — Free session (anytime)

**Ask:** *"Run session X on [topic]. Read the relevant documents first, challenge my idea
before accepting it, and end by depositing it — `PORTFOLIO.md`, the charter, or a ledger row.
Never leave it only in conversation."*

---

**Standing rules:** read before writing · one zone per session · a session that doesn't
commit didn't happen · `SESSION_LOG.md` gets exactly 5 lines · vocabulary per
`../studio/STUDIO_GLOSSARY.md` — missing terms are proposed as `[OPEN]`, never coined ·
new `[OPEN]`s are surfaced, never resolved silently.

**Exit protocol:** commit → Status lines → `SESSION_LOG.md` (5 lines, ends "Next for this
area: …") → `ROADMAP.md` semaphores.

**Continuation protocol:** the closing session writes "Next for this area: …"; the next
session reports inherited state before new work. **Documents remember; chats don't.**

**Git never runs in Cowork** — studio doctrine (`NEW_GAME_BOOTSTRAP.md`: "mount corrupts git
writes") and CL-09, which has now fired three times. Claude writes files and hands you the
commands; you run them in PowerShell.
