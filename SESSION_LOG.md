# SESSION_LOG.md — Casual Lab (working name)

Every session appends exactly 5 lines. Newest first.

```
[CL-S03] 2026-09-13
Job: stand up casual-lab/flight-dispatch/ (005's own space) and write its Prototype Brief, per HANDOFF_005_PROTOTYPE_BRIEF.md.
Changed: created flight-dispatch/{CLAUDE.md, PROTOTYPE_BRIEF.md, AUTHORSHIP.md, SESSION_LOG.md}; .gitignore (+ /flight-dispatch/); PORTFOLIO.md (C02 annotated to point at the new space and record the two rulings; CL-13/14/15 raised); HANDOFF_005_PROTOTYPE_BRIEF.md marked CLOSED; SESSION_LOG.md. kit/ untouched. Attached ../studio as a second read-only folder to settle the repo convention and the template question. **NOT COMMITTED, and flight-dispatch/ is not yet a repo** — git runs outside Cowork by studio doctrine (NEW_GAME_BOOTSTRAP.md: "mount corrupts git writes") and CL-09 fired a third time; commands handed to Gian. CL-S02's commit is still outstanding too.
Rulings/[OPEN]: RULED by Gian — **build surface = web** (single self-contained HTML page), explicitly not Unity: CL-01 is scoped to the kit and charter §2 puts the kit at pre-production, so GREEN named no engine; decisive factor was CL-01's own accepted risk — six of seven Unity defects compiled clean with no reviewer downstream (§5), which on an instrument yields a false PASS or false KILL rather than a loud failure. **Every tester plays on a touch device.** **Repo posture = its own nested repo**, per the founding checklist's stated convention ("each game is its own nested repo"), against my earlier misreading of map-painter as a folder-in-line-repo precedent. RAISED — CL-13 (map-painter/ is gitignored *and* has no .git: 002's GDD and 21 open items are versioned nowhere), CL-14 (charter §7's rename cost is overstated — games are gitignored, not submodules, so a nested repo does not know its parent's name), CL-15 (no Prototype Brief template exists in ../studio/templates/; the glossary defines the artifact's six contents and 005's Brief was written to it — harvest it back). Game-level: [OPEN-FD1]–[FD4] in the new space. Still RAISED: CL-05, CL-07, CL-08, CL-09, CL-10, CL-11, CL-12.
Next for this area: this space's job on 005 is done until the verdict — the Brief is DRAFT awaiting Gian's signature and FD1–FD4, and the build happens in flight-dispatch/. Line-space work remaining: write the HANDOFF for the kit's first Claude Code session on Unity 6.3 (carrying charter §3's three conditions as kit invariants and CL-09's explicit-install-path doctrine); rule CL-05 (backend posture — its own session, and it gates C02 §11/§16). Carry to the studio space: CL-06's ledger entry, CL-07's template fix, CL-15's harvest. Open housekeeping unchanged: kit/_spike/ is dead code Gian never ruled on deleting.
```

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
