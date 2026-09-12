# HANDOFF_KIT_ENGINE_SPIKE.md — CL-01, the line's first Claude Code session

**Status:** OPEN, written 2026-09-12 in the line's Cowork space.
**Closes:** `PORTFOLIO.md` ledger row **CL-01** · `LINE_CHARTER.md` §3 **[OPEN-K1]**.
**Surface:** Claude Code, terminal, folder `C:\Dev\gabella\casual-lab`.
**Zone:** `kit/` only. This session never edits the docs zone (`CLAUDE.md`,
`LINE_CHARTER.md`, `PORTFOLIO.md`, `HANDOFF_*`, `SESSION_LOG.md`) — those are Cowork's.
**Authority:** the spike **recommends**. Gian rules. Cowork lands the ruling.

## The question

`LINE_CHARTER.md` §3 `[OPEN-K1]`, ruled by spike `[RULED by Gian, 2026-09-10]`:

> **Godot · Unity · web + Capacitor** — which gives Claude the highest productivity *and*
> the smoothest ads / consent / IAP path on Android?

Two axes, both binding. An engine Claude writes beautifully but that fights AdMob and the
consent flow fails the line, because the line's business model *is* ads. An engine with a
perfect monetization path that Claude cannot iterate in fails too, because the line's
premise is low founder attention.

## Why now, and what it is not

The kit cannot start until the engine is ruled, and **005 Flight Dispatch** (GREEN,
2026-09-11) cannot be prototyped until the kit exists in some form.

This session is a **spike, not the kit.** Do not begin building the kit's systems (menus,
save, settings, audio, analytics, ads, IAP, consent, localization, progression). Build
only the throwaway slice below, three times, and report. Spike code is expected to be
deleted. Say so in the report.

## The slice — identical in each candidate

Chosen because it is the smallest thing that exercises what **005** actually needs. It is
not the game and must not grow toward it.

- Portrait 9:16, scaled correctly on a real phone.
- Four horizontal lanes; a sprite (the "aircraft") in each.
- A tray of icon buttons at the bottom; **drag one onto a lane** to start a service.
- A service takes N seconds, showing a filling bar; on completion an icon pops and a
  one-shot sound plays.
- A countdown timer in the HUD; at zero, a fail state and a retry button.
- Persist one value (best score) across app restarts.
- **Ship it to a physical Android device** as a signed debug build. A slice that only runs
  in the editor has not been tested.

## What to measure — same table for all three

| Axis | What to record |
|---|---|
| Claude productivity | Wall-clock to a working slice; how many times Claude had to be corrected; how much of the code Claude wrote unaided; quality of the error messages it had to debug from |
| Iteration speed | Edit-to-see-it-on-device time; hot reload or not |
| Build reality | Cold start on a mid-range device, APK/AAB size, build time, toolchain friction on Windows |
| Ads path | **Verify current state, do not trust priors.** AdMob banner/interstitial/rewarded: first-party, officially maintained, or community plugin? Who maintains it and when did it last ship? |
| Consent path | Google UMP / a compliant CMP — does a supported integration exist for this engine today? |
| IAP path | Play Billing — same three questions |
| Analytics | Firebase or equivalent, same three questions |
| Store compliance | Current Play target-API requirement, and whether the engine's toolchain meets it without patching |
| Longevity risk | Licensing/pricing terms, and who is on the hook if the monetization plugin is abandoned |

Ads, consent and IAP are the axis most likely to decide this, and the axis where training
data goes stale fastest. **Check current sources; cite them with dates in the report.**

## Time box

One Claude Code session per candidate, three maximum. If a candidate cannot reach the
slice on a device inside its session, that is not a failed session — that is the finding,
and it goes in the report as evidence about the productivity axis.

Run **Godot first**. It is the cheapest to falsify: if its ads/consent story is weak today,
that is the single largest known risk in the whole question and finding out early may make
the other two sessions shorter.

## Deliverable

`kit/SPIKE_REPORT_ENGINE.md` — a **Report**: produced once, then frozen (glossary §).
Nothing else in `kit/` survives the spike.

It must contain:

1. The three measurement tables, filled, with dated sources for every ads/consent/IAP claim.
2. **A recommendation with a named runner-up**, and the condition under which the
   runner-up would win instead.
3. **A dissent section** — the strongest honest case against the recommendation. If the
   spike cannot argue against its own answer, the answer is not trustworthy.
4. Anything discovered that belongs to the line rather than the kit, listed as **proposed
   `[OPEN]` items for Cowork to raise**. Do not edit `PORTFOLIO.md` to add them.
5. Whatever the spike learned about **005 Flight Dispatch's** feasibility on the winner —
   drag latency, timer accuracy, sprite counts. Observations only; the Prototype Brief is
   written in Cowork, not here.

## Exit

- `kit/SPIKE_REPORT_ENGINE.md` written and committed.
- Spike code deleted or quarantined under `kit/_spike/` with a README saying it is dead.
- A one-line summary handed back to Gian for the Cowork space, which lands **CL-01** in
  `PORTFOLIO.md` and signs `LINE_CHARTER.md` §3 `[OPEN-K1]`.
- `SESSION_LOG.md` is Cowork's file — the spike does **not** write it. Report the five
  lines in chat and Cowork will deposit them.
