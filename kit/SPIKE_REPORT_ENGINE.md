# SPIKE_REPORT_ENGINE.md — CL-01: which engine for the Casual Lab kit

**Type:** Report (glossary): produced once, then frozen.
**Answers:** `LINE_CHARTER.md` §3 `[OPEN-K1]` and `PORTFOLIO.md` ledger row **CL-01**, per
`HANDOFF_KIT_ENGINE_SPIKE.md`.
**Authority:** this Report **recommends**. Gian rules. Cowork lands the ruling.
**Sessions:** 2026-09-12 and 2026-09-13, Claude Code, `kit/` only.
**Evidence:** `kit/_spike/RESEARCH_NOTES.md` (dated sources), `kit/_spike/MEASUREMENTS.md`
(the raw log, in the order things happened), `kit/_spike/device_results.json`, and the three
slices under `kit/_spike/`. **All spike code is dead** and is quarantined in `kit/_spike/`
(see its README). Nothing else in `kit/` survives the spike.

---

## 1. The answer, in one paragraph

**Recommendation: Godot 4.7.** **Runner-up: web + Capacitor.** Unity 6.3 is a credible third,
and its strongest argument is the subject of the dissent (§6). On this line's two binding axes,
Godot came out best on **Claude productivity** by a clear margin. It needed 1 correction and was
right on its first visual run. It had the fastest measured edit-to-device loop (~25 s), no
account, licence or elevation gates, and the smallest permission footprint. On device it was
level with the others: cold start within 18 ms of Unity, 120 fps with no dropped frames while
dragging, and persistence verified. On the **ads/consent/IAP path**, the handoff expected the
research to hurt Godot, and it doesn't: a current AdMob plugin with UMP consent exists, and so
does a Play Billing plugin on Billing 9.1.0, the newest of the three candidates. **Godot's real
risk is who maintains the monetization plugins**, which are effectively one person each, not
whether they exist. That is why the runner-up and the dissent exist.

---

## 2. The question the spike actually answered

The handoff expected ads/consent/IAP to decide this, and named Godot's ads story *"the single
largest known risk."* **Checked on 2026-09-12, all three candidates clear that bar.** Each has a
maintained AdMob integration, a UMP consent path, Play Billing at or above the v8 requirement, a
toolchain targeting API 36, and an answer to the 16 KB page-size rule. Nobody was eliminated on
paper.

So the deciding evidence had to come from building the same slice three times and running it on
a phone. Two sessions covered all three candidates, inside the handoff's three-session budget.

---

## 3. Measurement tables

### 3.1 Claude productivity

| | Godot 4.7.2 | Unity 6.3 LTS (`6000.3.24f1`) | Capacitor 8.5.2 + Phaser 4.2.1 |
|---|---|---|---|
| Code written unaided | all (~430 lines GDScript + hand-written `.tscn`) | all (~600 lines C#) | all (~430 lines TypeScript) |
| **Corrections** | **1** | **7** | **3** (2 code + 1 toolchain) |
| …of which compiled clean and were only caught afterwards | 0 | **6** | 0 |
| Correct on first visual run | **yes** | **no** | yes (labels added in a parity pass) |
| Error-message quality | exact file, line, rule | 30 lines naming no package for one missing dependency | `tsc` exact, with "Did you mean" hints |

What the correction counts contain:

- **Godot, 1:** `PackedStringArray()` is not a constant expression.
- **Capacitor, 3:** two Phaser 4 API changes Claude's priors didn't know about (`Geom.Point` removed,
  `fillPoints` retyped), plus Capacitor 8 needing **JDK 21** where Godot needs 17.
- **Unity, 7:** the uGUI package missing from the manifest; tray labels never assigned; **a
  hand-written triangulator that drew the aircraft as shards** (UGUI has no polygon primitive;
  Godot and Phaser draw the same outline in one call); unused slots rendering as white squares; a
  shift clock that lost ~2 s at launch; **UI clipped off-screen on a phone taller than 9:16**
  (found only on device); and a 60 fps cap that made its frame comparison unfair (found only on
  device).
- **All three counts are Claude's mistakes**, not engine faults. The pattern still matters,
  because this line's founder reviews play, not builds. Six of Unity's defects compiled clean and
  were only caught afterwards, by review, screenshot or the phone: the most expensive kind for a
  low-attention line.

### 3.2 Iteration speed

| | Godot | Unity | Capacitor |
|---|---|---|---|
| **Edit → running on device**, plain rebuild, verified by screenshot | **24.4 / 24.7 s** | 60.7 / 67.7 s (incremental) | **not measured** (see below) |
| of which build / export | ~10 s | 40–47 s (Unity reports 31–33 s) | desktop edit → APK 16.4 s, before any install (2026-09-12) |
| of which `install -r` | ~11 s | ~17 s | 5.5 s |
| Hot reload / live reload on device | not measured | n/a | not measured |

Capacitor's on-device loop is missing for two reasons. The first run was a harness bug that
installed a stale APK; only the proof screenshot caught it. The re-run hung inside Gradle and was
stopped when Gian needed the phone. **Capacitor's live reload, the one loop that could beat
everything above, was never measured**, and neither was Godot's editor Remote Deploy with script
hot-reload. Both are listed as limits in §7.

### 3.3 Build reality

| | Godot | Unity | Capacitor |
|---|---|---|---|
| Debug APK, arm64-v8a | 26.96 MB | 27.78 MB like-for-like (34.98 MB with the packages Unity added on its own) | **4.34 MB** |
| Release APK | 25.21 MB (unsigned, indicative) | not built | not built |
| Cold build | 10.4 s | 148–204 s clean | ~55 s warm, ~2 min with downloads |
| **Cold start on device**, median of 5 (min–max) | 323 ms (304–456) | **305 ms** (283–375) | 408 ms (391–529) |
| targetSdk / minSdk | 36 / 24 | 36 / **25** (script set 24; Unity raised it without warning) | 36 / 24 |
| 16 KB pages | **pass**, `p_align = 16384` read from the APK | **pass**, all six `.so` | no native code |
| Permissions | **none** | `INTERNET` (+ `BILLING`, `ACCESS_NETWORK_STATE` while the unrequested packages were present) | `INTERNET` |
| Full-screen by default | yes | yes | **no**, status and navigation bars visible |
| JDK needed | 17 | bundled | 21 |
| Human gates to install on Windows | **0** | **2**: UAC elevation + Unity account and licence | **0** |
| Install size | ~1.2 GB templates | **7.75 GB** | npm install, ~40 s |

On device (Galaxy S24 Ultra, Android 16, 120 Hz), **all three** registered drag with real touch
input. None dropped a frame while dragging: drag max 8.3 ms at 120 fps for Godot and Capacitor, and
16.7 ms (one frame) for Unity at its then 60 fps cap. After the frame-cap fix Unity ran at 120 fps
with frame max 8.4 ms; its drag max at 120 fps was not re-measured. **All three persisted the best score across a real `force-stop`** (Godot 12,
Capacitor 12, Unity 8 → 13, also kept across an `install -r` update). The crash buffer shows no
crashes or ANRs. Gian's report: **drag feels fine.** The completion sound played on some engines;
which one stayed silent is unknown.

### 3.4 Ads, consent, IAP, analytics, compliance, longevity (all dated; sources in `RESEARCH_NOTES.md`)

| | Godot | Unity | Capacitor |
|---|---|---|---|
| **AdMob** | `poingstudios/godot-admob-plugin` **v5.0.0, 2026-07-21** (nightly 2026-09-12); GMA Next-Gen SDK 25.1.0; MIT; **community** | `googleads/googleads-mobile-unity` **v11.5.0, 2026-09-03**; Apache-2.0; **maintained by Google** | `@capacitor-community/admob` **8.1.0, 2026-08-14** (repo pushed 2026-09-11); community, healthy contributor spread; banner sits over the WebView |
| **UMP consent** | in the same plugin, with Privacy Options Form (v5.0.0) | first-party, in Google's package | in the same plugin (`requestConsentInfo`, `showPrivacyOptionsForm`) |
| **Play Billing** | `godot-google-play-billing` **3.3.0, 2026-07-27**, **Billing 9.1.0** (runway to 2028); community org that says the Godot Foundation does not maintain or endorse it | `com.unity.purchasing` first-party: 5.0.4 changelog Billing 8.0.0; **4.15.1 as installed declares Billing 8.3.0** (runway to 2027) | RevenueCat `purchases-capacitor` **13.5.1, 2026-09-10**, Billing **8.3.0**; vendor-maintained |
| **Analytics** | **weakest**: no maintained native Firebase Analytics plugin; vendor-official **GameAnalytics** GDExtension 3.1.0 (2026-07-10) | Firebase Unity SDK **v13.16.0, 2026-08-27**, first-party | `@capacitor-firebase/analytics` **8.5.1, 2026-09-02** |
| **Play target API 36** (required for new apps and updates since **2026-08-31**) | met unpatched (source + APK) | met (APK) | met (template + APK) |
| **16 KB pages** (required since **2025-11-01**) | verified in APK | verified in APK | no native code in the app |
| **Licence cost** | MIT, none | Personal free below **$200K** revenue+funding; Pro **$2,310/yr**; Runtime Fee cancelled 2024-09-12; splash optional on Personal (**verified on Gian's licence**) | MIT, none |
| **Who is on the hook if the monetization plugin stalls** | **one person** (AdMob: 1,046 commits vs next contributor's 3); billing near bus-factor 1 | **Google and Unity** | a healthy community plugin + **RevenueCat** |

---

## 4. Recommendation

**Godot 4.7, standard build, GDScript.**

1. **Productivity:** 1 correction and right first time, against 7 for Unity (6 of which compiled
   clean) and 3 for Capacitor (2 Phaser API drift, 1 JDK toolchain). Precise errors.
2. **Iteration:** the fastest measured edit-to-device loop, ~25 s, of which ~11 s is just
   installing the APK.
3. **Low attention:** no account, licence server or UAC; installable unattended from a package
   manager.
4. **Device parity:** cold start level with Unity and ~85 ms ahead of Capacitor; 120 fps; no
   dropped drag frames; persistence verified; no permissions requested.
5. **Monetization path exists and is current:** AdMob with UMP consent and in-editor mock ads
   (useful for remote, device-free work), and Play Billing 9.1.0. **Analytics would go through
   GameAnalytics, not Firebase.**

## 5. Runner-up, and when it wins

**Runner-up: web + Capacitor.** It **wins instead if either condition holds:**

- **Godot's AdMob or billing plugin stalls.** For example, no release that tracks a UMP or Play
  policy change within one Play requirement cycle (roughly six months), and the line is unwilling
  to fork and maintain an Android plugin. Charter §6 exists to refuse exactly that obligation.
  Capacitor's AdMob plugin has a real contributor base, and its IAP is vendor-maintained
  (RevenueCat).
- **Install size proves decisive for store conversion.** 4.34 MB against ~27 MB is a 6× gap no
  engine setting closes.

Capacitor over Unity as runner-up: it keeps zero install gates and code in a language Claude
writes fluently, where Unity costs two human gates, ~60 s loops and the most silent defects. What
counts against Capacitor, all measured: ~85–100 ms slower cold start (WebView start-up), not
full-screen by default, JDK 21 alongside Godot's 17, WebView frame pacing that varied (95–121 fps)
where the native engines held 120, and an on-device loop never measured.

## 6. Dissent — the strongest honest case against Godot

**The line's business model is ads, and Godot's ad, consent and billing path rests on community
plugins with a bus factor of about one. Unity's rests on Google and Unity.**

- A lapsed UMP or Play policy change hits every live title at once. On Unity, Google ships the fix.
  On Godot, the line waits for one maintainer or maintains a fork itself, which is ongoing human
  operations the charter treats as a hard floor.
- Unity's weaknesses in this spike are **largely one-time and largely Claude's.** The UGUI polygon
  floor, the missing manifest entry, the clipped layout and the frame cap were all fixed in a day,
  and a kit would solve them once. The install gates are a one-time founder cost. The package drift
  is solved by pinning the manifest and checking the APK's permissions at build time. What remains
  is a ~60 s loop against ~25 s, and **Unity had the best cold start of the three.**
- Analytics is Godot's weakest axis. Unity has first-party Firebase; Godot has a GameAnalytics
  vendor dependency the line has not chosen.
- **The case for Capacitor:** if live reload works on device, its loop could be under a second.
  Nobody measured it. Its APK is 6× smaller, and its two code corrections were Phaser API drift that
  fades as the kit accumulates its own conventions.
- **If Gian weighs monetization longevity above Claude's productivity, Unity is the answer, not
  Godot.** This Report weighs them the other way, because the charter makes low founder attention
  the premise, and Unity was the only candidate that repeatedly needed that attention.

## 7. What this evidence cannot tell you

- **The phone is a flagship.** The brief asked for mid-range. Cold start, frame pacing and the
  sprite ceiling will all be worse on a cheaper phone, and possibly not equally so.
- **Debug and development builds only.** Release builds were not measured on device.
- **16 KB behaviour was verified at APK level only.** The phone runs 4 KB pages.
- **Unmeasured:** Capacitor's on-device loop, Capacitor live reload, Godot Remote Deploy with
  hot-reload, the real sprite ceiling (404 aircraft cost nothing measurable), audio latency, and
  which engine's sound failed.
- **Unity's cold start was measured on the build with the clipped layout and 60 fps cap.** Both
  fixes are UI scaling and frame pacing, unlikely to move a first-frame launch time, but that was
  not re-measured.
- **None of the monetization SDKs were integrated.** Their existence, versions, maintainers and
  Billing versions were verified from sources, not from running ads.

## 8. Found along the way that belongs to the line, not the kit: proposed `[OPEN]` items for Cowork

*Proposed only. `PORTFOLIO.md` was not edited.*

1. **Reference test device.** Which mid-range phone is the line's target? Every device number here
   came from an S24 Ultra.
2. **Plugin-abandonment posture.** If the engine is Godot: what triggers fork-and-maintain, versus
   switching to the runner-up, versus accepting the risk? (§5 proposes a trigger.)
3. **Machine-setup doctrine: the most important environment finding of the spike.** Toolchains
   Claude installs from inside the Claude desktop app into a *new* AppData folder are redirected
   into the app's private storage and are **invisible to the founder's own shells.** The Android
   SDK had to be moved to `C:\Android\Sdk`. Setup should install to explicit paths outside AppData
   and be verified from the founder's shell.
4. **Attended-window ritual.** CL-01 needed Gian at the machine for the SDK licence, a Samsung USB
   driver, a Unity account with UAC, and the phone. The line should expect and schedule this for
   every device-touching task.
5. **Analytics vendor** if Godot is chosen: GameAnalytics (vendor-official for Godot) versus a
   custom Firebase integration. This is a data-location and vendor decision.
6. **CLI telemetry.** The Capacitor CLI reported *"sharing anonymous usage data"* on Gian's machine
   during the run. Small, but machine-setup doctrine should decide telemetry defaults for every
   tool the line installs.

## 9. For 005 Flight Dispatch: observations only

*The Prototype Brief is written in Cowork; nothing here specifies the game.*

- **Drag is viable in all three engines.** Real touch drags registered and no frame was dropped
  while dragging at 120 Hz; Gian: "drag feels fine." Flagship caveat applies.
- **Timer accuracy:** drift over a shift stayed small. Capacitor ±0.01 s; Unity ±0.01 s after
  fixing a start-of-shift bug; **Godot showed a consistent ~+0.13–0.17 s** on device. That looks
  like the same class of issue Unity had, the first frame's time step including start-up, at a
  smaller size. **005's clock should start on the first rendered frame, whatever the engine.**
- **Sprite counts:** 104 extra aircraft (Godot, Capacitor) and 404 (Unity) cost nothing measurable
  on this phone. **005's dense apron is not at risk from sprite count on a flagship.** The real
  ceiling, and any mid-range ceiling, is unknown.
- **Portrait on tall phones:** one engine's default scaling clipped the tray off-screen on a
  screen taller than 9:16. 005's layout must be checked on a tall phone early.
- **Contention (scarcity):** the slice kept the briefed unlimited vehicles (`VEHICLE_POOL = -1`).
  Whether contention reads on a phone, which is 005's tactical layer, was flagged on 2026-09-12 and
  not tested. It belongs in the Prototype Brief.

---

*Frozen on commit. Corrections to facts go in a dated addendum below, never as edits above.*
