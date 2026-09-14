# CLAUDE.md — Casual Lab (working name) — the line-level space

<!-- STUDIO_PREAMBLE v1 — verbatim from studio/STUDIO_PREAMBLE.md; never edit here -->
## Gabella (read before anything else)
This repo is one project of **Gabella**, a solo studio (Gian + Claude). The tree is not
the org chart: `C:\Dev\gabella\studio\` holds doctrine (`STUDIO_HANDBOOK.md`, glossary,
`LEDGER.md`, `HARVEST.md`, `IDEAS.md`, templates, handoffs); `games\` is the Games **Core**
line (001 flag-carrier flagship · 003 fellowship · 004 humanity); `casual-lab\` is the
Games **Casual Lab** line (`PORTFOLIO.md` is its ledger; 002 map-painter · concepts);
`softworks\` is Gabella Softworks. Project numbers are studio-wide.

**Scope the ask before working, and say so in one line.** Does resolving it change
*this project's* documents or code? → project scope, work here. Does it change the
*line's* portfolio, gate or kit? → line scope, deposit in the line folder. Does it change
*doctrine, templates, vocabulary or studio tooling*, or concern more than one project? →
**studio scope: answer from `studio/`, deposit there (a `LEDGER.md` S-row, an amendment
to an existing handoff), and never turn it into an artifact of this project.** If
`studio/` is not attached to this space, say so and stop: the answer would be invented.

**Before building any cross-project instrument** (dashboard, wiki, tracker, "agent that
reads everything"): `studio/HANDOFF_COCKPIT.md` already specifies the studio cockpit and
rendered tree. Amend it; do not create a rival.

**Rulings are Gian's.** Every class-1/3/4 item gets a ledger row with an effort tag
(`⚡ ruling` ≤10 min · `🔧 review` ~30 min · `🎯 design` 90 min) before the session closes.
Chat is the Hallway; a decision counts only once deposited in the tree.
<!-- /STUDIO_PREAMBLE v1 -->

A second line inside Gabella Games: small mobile games, one mechanic each, free with ads,
released on the Play Store, built on a shared kit, with one deliberate difference from the
churn per title. Solo dev (Gian) + Claude, mostly remote and low-attention: Gian's minutes
here go to the **gate** and to **play**, not to build review. Governed by Gabella studio
doctrine (`../studio/STUDIO_HANDBOOK.md`) with the **line clauses** in `LINE_CHARTER.md`.

## What this space is

The *line* space, not a game space. It owns the portfolio, the gate, the kit, and the
line's ledger. Each game that passes the gate gets its own folder, repo, number and Cowork
space (`casual-lab/<slug>/`) and is worked there, never here.

## The studio around you (know these exist; never edit them)

- **Gabella** — the company. `../CLAUDE.md` is the parent constitution; `../studio/` is
  the doctrine layer (handbook, glossary, templates, `LEDGER.md`, `HARVEST.md`).
- **Gabella Games — Core line** (`../games/`): **001 flag-carrier** — the flagship, an
  airline-management sim, Phase 1, long-term; **003 fellowship** — exploration, warm
  adventure-tactics RPG; **004 humanity** — exploration, parked slow-burn, information-as-
  resource civilization game. Core games are bespoke; they never depend on this line's kit.
- **002 map-painter** — belongs to this line (merge of the studio's map toy and the casual
  conquest design; see `PORTFOLIO.md` entry). Its charter rewrite happens in its own space.
- **Gabella Softworks** (`../softworks/`) — non-game software; nothing active.

## Documents (read before working)

- `ROADMAP.md` — **where the line is**: semaphores, what is in flight, what each thing is
  blocked on, and which surface each workstream is worked in. Status lives here.
- `SESSION_PLAYBOOK.md` — **what to run**: the typed sessions (B, R, G, S, P, K, W, X) with
  paste-ready asks. Session **B** is the status briefing — run it when returning cold.
- `LINE_CHARTER.md` — the line's laws, pipeline, kit rule, authorship posture, guards.
- `PORTFOLIO.md` — every concept, its gate status, and the line ledger (⚖️ Awaiting Gian).
- `../studio/templates/CASUAL_CONCEPT_GATE.template.md` — the intake gate.
- `../studio/STUDIO_GLOSSARY.md` — studio vocabulary; its definitions win everywhere.
- `kit/` — the shared base, **Unity 6.3 LTS** (`LINE_CHARTER.md` §3, ruled 2026-09-13).
  Claude Code's zone.

## Zones

- **Docs zone** — this root (`CLAUDE.md`, `LINE_CHARTER.md`, `PORTFOLIO.md`, `ROADMAP.md`,
  `SESSION_PLAYBOOK.md`, `HANDOFF_*`, `SESSION_LOG.md`) — Cowork's.
- **Kit zone** — `kit/` — Claude Code's. Cowork proposes `[OPEN]` items, never edits.
- **Game folders** — `<slug>/` — their own spaces under their own `CLAUDE.md`.

## Session protocol

Start: read this file → `SESSION_PLAYBOOK.md` → `ROADMAP.md` → `LINE_CHARTER.md` → report
every un-landed row in `PORTFOLIO.md`'s ledger **and which sessions are unlocked**, then ask
which to run. End: land or annotate rows touched → `SESSION_LOG.md` (5 lines, ends "Next for
this area: …") → flip `ROADMAP.md` semaphores → commit. New `[OPEN]` questions are surfaced,
never resolved silently. Decisions reached in chat (the Hallway) are deposited here or they
did not happen.

**Git never runs in Cowork** (studio doctrine; CL-09 has fired three times). Claude writes
files and hands Gian the commands.

## Working rules that differ from Core

- One line game in build at a time.
- A concept is written up **only** through the gate; no design docs before GREEN.
- Hard gate floors: no backend, no ongoing human operations.
- The kit is a tool, not a numbered project.
- Line design laws replace Core laws 1–2 (`LINE_CHARTER.md` §1).

<!-- Keep under ~250 lines. Prune when the first game ships. -->
