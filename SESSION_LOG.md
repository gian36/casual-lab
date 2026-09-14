# SESSION_LOG.md — Casual Lab (working name)

Every session appends exactly 5 lines. Newest first.

```
[CL-S02] 2026-09-13
Job: land CL-01 — take the frozen engine spike Report to Gian, get [OPEN-K1] ruled, and write the ruling down. (Extended in the same sitting: CL-06 signed and C02 §17's "Why" dictated, which together complete the C02 gate.)
Changed: LINE_CHARTER.md (§3 [OPEN-K1] replaced with the signed ruling + three kit conditions; changelog), PORTFOLIO.md (CL-01 moved to Landed with a full landing note; CL-08–CL-12 raised; two §8 items recorded as dissolved; stale "none has passed the gate" header fixed; CL-06 landed; C02 header + §17 "Why" completed), HANDOFF_KIT_ENGINE_SPIKE.md (marked CLOSED), CLAUDE.md (kit zone line now names the engine), SESSION_LOG.md. kit/ untouched — not Cowork's zone. **NOT COMMITTED** — Cowork's workspace shell could not mount the folder (see CL-09); commit is outstanding.
Rulings/[OPEN]: RULED — CL-01 engine = **Unity 6.3 LTS**, *against* the Report's recommendation of Godot 4.7, on the dissent's weighting (monetization longevity first-party via Google/Unity) plus the 2D content pipeline — the latter explicitly recorded as Gian's judgement, not spike evidence, since CL-01 measured 2D in neither engine. Counter-fact recorded too: on Billing runway Godot was ahead (9.1.0/2028 vs Unity 8.3.0/2027), so Unity's edge is who fixes it, not how current it is. Accepted risk recorded: ~60s vs ~25s loops, two human install gates, and six of seven Unity defects compiling clean with no strong reviewer downstream — Gian has no prior experience in any candidate engine and per §5 does not review builds. Conditions attached: pin the package manifest, assert APK permissions at build time, fail the build on unrequested packages (Unity silently added IAP/analytics during the spike). RAISED — CL-08 reference test device, CL-09 machine-setup doctrine (AppData/MSIX redirection hid the Android SDK from Gian's shells), CL-10 attended-window ritual, CL-11 CLI telemetry defaults, CL-12 Unity dependency posture (Runtime Fee precedent; reshaped from the spike's Godot-only §8.2). SIGNED — CL-06 C02 Flight Dispatch = **studio project 005** (never in doubt per S11/S12/S10; Gian's signature closes it for the line, but the studio-side ledger entry is still a clerical act in ../studio/, which this space never edits). WRITTEN — C02 §17 "Why (Gian)": viable product that could catch some players, most store-legible of the four, and the line's purpose is to reach a *shipped* product so the studio meets publishing/ads/consent/analytics before Flag Carrier does; drafted by Cowork from Gian's Hallway words and marked for his correction. **The C02 gate is now complete.** Still RAISED: CL-05, CL-07 (CL-07 also studio-side).
Next for this area: **C02's Prototype Brief is now fully unblocked** — write it (§15 question: does allocation against announced arrivals produce deliberation?), carrying the spike's §9 observations for 005: start the clock on the first rendered frame, check portrait layout on a tall phone early, and test contention, which the spike briefed away with VEHICLE_POOL = -1 and never measured. Also write the HANDOFF for the kit's first Claude Code session on Unity 6.3, carrying charter §3's three conditions as kit invariants and CL-09's explicit-install-path doctrine. Carry to the studio space: CL-06's ledger entry and CL-07's template fix. Open housekeeping: kit/_spike/ is dead code its README says is safe to delete now that the Report landed and [OPEN-K1] is signed — not deleted, Gian never ruled on it.
```

```
[CL-S01] 2026-09-11
Job: open the line space, close the founding's loose ends, run the line's first Concept Gate.
Changed: LINE_CHARTER.md (founding date, changelog), PORTFOLIO.md (founding date, ledger, C02 entry replaced with the filled gate), SESSION_LOG.md (first entry).
Rulings/[OPEN]: RULED — CL-04 first gate = C02 Flight Dispatch; C02 §17 GREEN — Prototype; portrait orientation; dense in-game apron with lean ad creatives only. RAISED — CL-05 backend posture for the line (reserved by Gian for its own session), CL-06 studio number for C02 (005 apparent, needs studio-side action), CL-07 gate template numbering slip (§15 vs §17). Still RAISED: CL-01 engine spike. Note: map-painter/ already exists as a folder, touching charter §7's "renaming is cheap only before the first game repo exists".
Next for this area: write the Prototype Brief for C02 (§15 question: does allocation against announced arrivals produce deliberation?) once CL-01's engine spike and CL-06's number land; Gian still owes §17's "Why".
```

---

Format:
```
[<session-id>] <YYYY-MM-DD>
Job: <the one job this session had>
Changed: <files touched>
Rulings/[OPEN]: <decisions made by Gian, or [OPEN]s raised — "none" is valid>
Next for this area: <the baton pass>
```
