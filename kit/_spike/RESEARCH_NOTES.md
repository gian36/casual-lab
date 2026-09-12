# RESEARCH_NOTES.md — CL-01 engine spike, desk-research phase

**Status:** WORKING NOTES, not the Report. The Report is `kit/SPIKE_REPORT_ENGINE.md`,
assembled once when slice evidence exists (glossary: a Report is produced once, then frozen).
**Everything in `kit/_spike/` is dead on arrival** — see `kit/_spike/README.md`.

**Session:** CL-01, 2026-09-12. **Brief:** `../../HANDOFF_KIT_ENGINE_SPIKE.md`.
**Sequencing** `[RULED by Gian, 2026-09-12]`: desk-research all three candidates first,
then build the slice only for survivors. Reason: the brief assumed ads/consent would be the
deciding axis and that Godot was cheapest to falsify there; first-pass checking showed all
three clear that bar, so the paper phase was run first to avoid three toolchain installs.

## How every claim below was verified

Search-engine summaries were found unreliable for dates (one reported a plugin as
"actively maintained, min Godot 4.7" that the GitHub API shows **archived since
2025-08-05**). So, in order of preference:

1. **GitHub REST API** (`/releases`, `/repos`, `/contributors`) — exact `published_at`
   timestamps, archive flag, licence, contributor distribution. Used for every plugin claim.
2. **Source read at a pinned tag** — dependency versions read out of `build.gradle` /
   `build.gradle.kts` / `variables.gradle` at the exact release tag.
3. **Vendor's own page, fetched and dated** — Google Play policy pages, Unity blog and
   manual, Unity's own release API.

Every row says how it was verified. All fetches: **2026-09-12** unless stated.

---

## Baseline: what Google Play actually requires today (all candidates)

| Requirement | Current bar | Deadline | Source (fetched 2026-09-12) |
|---|---|---|---|
| Target API level | **API 36 (Android 16)** for new apps *and* updates | in force since **2026-08-31**; extension to **2026-11-01** via Play Console | `developer.android.com/google/play/requirements/target-sdk` |
| Existing-app availability floor | API 35 to stay visible to new users on newer OS | ongoing | same page |
| 16 KB memory page size | Required for apps targeting API 35+ that ship native code; Play blocks the release otherwise | in force since **2025-11-01** | Android Developers Blog 2025-05 post; `developer.android.com/guide/practices/page-sizes` |
| Play Billing Library | **v8 or later** for new apps and updates | in force since **2026-08-31**; extension to **2026-11-01**. v9's own deadline is 2028-08-31 | `developer.android.com/google/play/billing/deprecation-faq` |

The 16 KB row is **not in the brief's measurement table** and it is a hard release blocker
for anything shipping native code — which all three candidates do. Added as a compliance row.

---

## Candidate A — Godot

Engine: **4.7.2-stable**, released **2026-08-18** (GitHub API, `godotengine/godot`).
The 4.7 line opened 2026-06-18; 3.6.3 is still maintained (2026-08-22).

| Axis | Finding | Verified how |
|---|---|---|
| **Ads** | `poingstudios/godot-admob-plugin` — **v5.0.0 stable 2026-07-21**, nightly `v5.1.0` **2026-09-12** (same day as this research). MIT, 618 stars, not archived, repo pushed 2026-09-12. Banner / interstitial / rewarded / rewarded-interstitial / app-open / native. Android side on **Google Mobile Ads Next-Gen SDK 25.1.0**, min SDK raised to 23. GDScript **and** C#. iOS resolves deps via SPM. | GitHub API releases + repo; v5.0.0 release body |
| | **Community, not first-party.** No Google or Godot Foundation involvement. | ownership inspection |
| | Second option inside the community org: `godot-sdk-integrations/godot-admob`, 112 stars, pushed 2026-05-27. | GitHub API |
| | **Falsified counter-example:** `cengiz-pz/godot-android-admob-plugin` and its iOS/addon siblings are **archived, last push 2025-08-05**, while a search summary and the asset-store listing still present them as current with "min Godot 4.7". A store listing is not evidence of life. | GitHub API `archived: true` |
| **Consent** | Same plugin. v5.0.0 added **UMP + Privacy Options Form** (US state regulations) and ships a UMP guide. A compliant CMP path exists today, from the same plugin that does ads. | v5.0.0 release body |
| **IAP** | `godot-sdk-integrations/godot-google-play-billing` **3.3.0, 2026-07-27**. MIT, 262 stars. Source at tag 3.3.0: `com.android.billingclient:billing-ktx:`**`9.1.0`**, `compileSdk 36`, `minSdk 23`, JDK 17, built against `org.godotengine:godot:4.5.1.stable`. **Billing 9.1.0 clears the v8 bar with headroom to 2028.** | source read at tag 3.3.0 |
| | Hosted in `godot-sdk-integrations`, whose own org description reads: *"Centralized hosting for community-maintained Godot SDK integrations. The Godot Foundation does not maintain or endorse these SDKs."* The asset store shows the publisher as `godot-foundation`, which reads as endorsement and is not. | org API description |
| | Godot core contributors do appear in its history — `m4gr3d` (Godot's Android platform maintainer) and `akien-mga` — alongside lead `syntaxerror247` (52 commits). | contributors API |
| **Analytics** | **Weakest axis for Godot.** No maintained native Firebase Analytics plugin of substance: the two active repos are 0-star and 2-star personal projects (pushed 2026-09-10 / 2026-09-08). `GodotNuts/GodotFirebase` (682 stars, MIT, pushed 2026-08-27) is a **GDScript/REST** implementation covering Auth, Database, Firestore, Functions, Storage — its own credits list **no Analytics**, consistent with Analytics requiring the native SDK. | repo search + README read |
| | **The real answer is not Firebase:** `GameAnalytics/GA-SDK-GODOT` is **vendor-official** (GameAnalytics' own org), **3.1.0 released 2026-07-10**, repo pushed 2026-09-11, MIT, GDExtension, **Godot 4.5+**, Android/iOS/Web/desktop, and ships a **prebuilt 26 MB zip** (no SCons build needed, despite the README leading with compile instructions). | GitHub API + release assets + README |
| **Store compliance** | **Passes unpatched.** Source at tag `4.7.2-stable`, `platform/android/java/app/config.gradle`: `compileSdk 36`, **`targetSdk 36`**, `minSdk 24`, `javaVersion 17`, `ndkVersion 29.0.14206865`, AGP 8.6.1. Identical in 4.6.3. | source read at tag |
| | **16 KB pages: handled.** PR #106358 "[Android] Add support for 16 KB page sizes, update to NDK r28b" merged **2025-05-15**, milestone **4.5**. The .NET-export gap (issue #110262 — .NET 8 lacked 16 KB support, .NET 9 has it) closed **2025-09-05**, also 4.5. NDK 29 and AGP 8.6.1 are both above Google's r28 / 8.5.1 threshold. Godot 3.6 had its own ticket (#110852). | GitHub API issue/PR states |
| **Longevity** | Engine: MIT, no royalty, no seat cost, no revenue threshold, Godot Foundation. **The monetization layer is the exposure:** the AdMob plugin is effectively **bus factor 1** — `gumaciel` has 1,046 commits; the next human contributor has 3. Billing is near enough bus factor 1 too (52 vs 8), though with Godot core devs nearby. Both MIT, so forking is legally free; the cost is that the line would then be maintaining an Android plugin — exactly the ongoing obligation Charter §6 exists to refuse. | contributors API |
| **Productivity (paper)** | v5.0.0's **in-editor interactive mock ads for every format**, with signal parity across GDScript and C#, lets the whole ad / consent / reward flow be exercised **without a device**. For a line whose premise is low founder attention and remote work, that is the most relevant productivity feature found on any candidate. | v5.0.0 release body |
| **Risk to test in the slice** | The billing plugin is compiled against `godot:4.5.1.stable` while the editor is 4.7.2. Godot Android plugins are normally forward-compatible inside 4.x; unverified here. | inference — flagged, not asserted |

---

## Candidate B — Unity

Editor: **Unity 6.3 LTS = `6000.3.24f1`, released 2026-09-10.** Also current:
`6000.6.0f1` (SUPPORTED, 2026-08-31) and `6000.0.83f1` (LTS, 2026-09-02); `6000.7.0a6` alpha.
Verified against Unity's own release API (`services.api.unity.com/unity/editor/release/v1`).

| Axis | Finding | Verified how |
|---|---|---|
| **Ads** | `googleads/googleads-mobile-unity` — **v11.5.0, 2026-09-03**, repo pushed 2026-09-11. Apache-2.0, 1,547 stars. **Maintained by Google itself** — the one genuinely first-party AdMob integration among the three candidates. | GitHub API |
| | Unity also has its own first-party ads/mediation stack (Unity Ads / LevelPlay). Not evaluated: the line's model is AdMob. | scope note |
| **Consent** | **First-party UMP, in the same package:** `GoogleMobileAds.Ump.Android.dll`, `GoogleMobileAds.Ump.Unity.dll`, `GoogleUmpDependencies.xml`, plus an editor `ConsentForm` prefab. | repo tree listing |
| **IAP** | `com.unity.purchasing` **5.0.4, 2025-12-03** — first-party Unity. **Google Play Billing Library 8.0.0** (up from 7.1.1 in 5.0.0). Requires Unity 2021.3+; iOS 15+ after the StoreKit 2 move. | Unity package changelog |
| | **Note the reversal:** Unity's first-party IAP sits on **Billing 8.0.0** — compliant today, but that version's own sunset is **2027-08-31**. Godot's community plugin is on **9.1.0**, good to 2028. First-party did not mean fresher. | cross-read |
| **Analytics** | Strongest of the three. `firebase/firebase-unity-sdk` **v13.16.0, 2026-08-27** — first-party Google, roughly three-week cadence (13.14.0 2026-07-17, 13.15.0 2026-08-06). Unity Gaming Services analytics also exists first-party. | GitHub API |
| **Store compliance** | Unity 6 manual (`6000.2` docs, fetched 2026-09-12): *"You can now target Android API levels 35 and 36"*; minimum Android 6.0 / API 23. **16 KB pages supported**, with the documented caveat that *"Unity applications built for devices with 4 KB memory page size might not work on devices with 16 KB memory page size"* — a build-configuration matter, not automatic. | Unity manual |
| **Longevity** | **Unity Personal is free below $200,000 USD total annual revenue *and funding*** — doubled from $100k — and the **Runtime Fee was cancelled outright**, both announced **2024-09-12** on Unity's own blog. The *Made with Unity* splash became **optional** for Unity Personal on Unity 6. Unity Pro is required above $200k; Enterprise above $25M. | unity.com blog 2024-09-12 (primary) |
| | **RESOLVED against Unity's own pricing page, fetched 2026-09-12:** Personal free below **$200K** revenue/funding in the prior 12 months; **Pro $210/month or $2,310/year**, required above $200K; Enterprise custom, required above **$25M**; Unity Industry custom, for non-gaming above $1M. The $2,200 figure in the 2024 blog has since risen to $2,310. | unity.com/pricing (primary) |
| | The threshold is measured on the **whole company**, so it is a Gabella-level number, not a per-title one. Comfortable today; a term to re-read, not a cost. | reading of the policy |
| | Plugin-abandonment risk materially lower than Godot's: Google is on the hook for ads, consent and analytics, Unity for IAP. | repo ownership |
| **Cost to test** | Heaviest install of the three (Hub + editor + Android Build Support + NDK/SDK) and a licence sign-in. | environment probe |

---

## Candidate C — web + Capacitor

Capacitor **8.5.2, published 2026-09-11** (`@capacitor/core`, `@capacitor/android`).
Game framework would be Phaser **4.2.1, 2026-07-09**, or equivalent.

| Axis | Finding | Verified how |
|---|---|---|
| **Ads** | `@capacitor-community/admob` **8.1.0, published 2026-08-14**; repo pushed **2026-09-11**, 296 stars, not archived, and with an unusually healthy contributor spread for a community plugin (`rdlabo` 455, `distante` 54, `BETOXL` 25, `jcesarmobile` 17 — the last a Capacitor core maintainer). Wraps the native Google Mobile Ads SDK: banner, interstitial, rewarded, rewarded-interstitial, app-open. | npm registry + GitHub API |
| **Consent** | **Covered.** README states it *"also covers Google User Messaging Platform (UMP) consent and App Tracking Transparency helpers on iOS"*; API surface includes `requestConsentInfo`, `showConsentForm`, `showPrivacyOptionsForm`, `resetConsentInfo`, against `com.google.android.ump:user-messaging-platform:4.0.0`. | README read |
| | **Structural catch, from the plugin's own docs:** *"The banner sits above the WebView and can cover HTML"* — banner ads are native views composited over the web layer, so the game must inset its own layout. A real UI constraint the other two do not have. | README read |
| **IAP** | Strong and vendor-backed rather than volunteer: `@revenuecat/purchases-capacitor` **13.5.1, 2026-09-10** (RevenueCat's own) and `cordova-plugin-purchase` **13.18.0, 2026-07-16** (Fovea). **RESOLVED at source:** the Capacitor plugin pulls `purchases-hybrid-common:18.38.0`, and `purchases-android` pins `billingClient = "8.3.0"` in its version catalogue — **clears the v8 bar**, same tier as Unity, a major behind Godot's 9.1.0. Its Android module also compiles at `JavaVersion.VERSION_21`, consistent with Capacitor 8's JDK 21 requirement. | source read, `libs.versions.toml` + plugin `build.gradle` |
| **Analytics** | `@capacitor-firebase/analytics` **8.5.1, 2026-09-02** (Capawesome) and `@capacitor-community/firebase-analytics` **8.1.0, 2026-08-27**. Native Firebase Analytics, unlike Godot's REST-only situation. | npm registry |
| **Store compliance** | `ionic-team/capacitor`, `android-template/variables.gradle`: `minSdkVersion 24`, `compileSdkVersion 36`, **`targetSdkVersion 36`**, `androidxWebkitVersion 1.14.0`, `cordovaAndroidVersion 14.0.1`. Passes the API-36 bar unpatched. | source read, main branch |
| | 16 KB pages: the app's own code is JS, so the exposure is limited to plugin `.so` files and the system WebView — the lowest native-code surface of the three. Not verified per-plugin. | reasoning — flagged as unverified |
| **Policy risk** | Play's Spam policy targets *"app submissions that are just webviews of existing websites, most of which are submitted with a primary purpose of driving traffic"*. A Capacitor game with assets bundled locally and no corresponding website does **not** match that description. Honest limit: **no policy sentence was found that speaks directly to locally-bundled WebView games**, so this is a reading of the policy's stated target, not a guarantee. | `support.google.com/googleplay/android-developer/answer/9899034` |
| **Longevity** | No licence cost, no revenue threshold, permissive licences across the stack; Capacitor is Ionic/OutSystems-backed. The ads plugin is community but with a real contributor spread. Largest number of separate moving parts to keep current (runtime + framework + four or five plugins). | composite |
| **What the slice must test here** | *Not* plugin existence — that is settled. Frame pacing for a 60 fps drag inside a WebView, touch latency, audio latency on the one-shot, the banner-over-WebView inset, and cold start. | brief slice, adapted |

---

## Where the paper phase leaves the question

The brief expected ads / consent / IAP to decide this, and named Godot's ads story as *"the
single largest known risk in the whole question."* **That expectation is not supported.**
All three candidates, today, have a maintained AdMob integration, a UMP consent path in the
same package, a Play Billing path at or above the v8 bar, an API-36 toolchain, and a 16 KB
page answer. Nobody is eliminated on paper.

What the paper phase *did* separate:

1. **Who is on the hook.** Unity: Google for ads/consent/analytics, Unity for IAP.
   Capacitor: a healthy community plugin plus RevenueCat. Godot: **one person** for ads,
   near enough one for billing. This is the brief's "longevity risk" row, and it is the only
   axis where the three differ by a wide margin.
2. **Analytics**, where Godot is genuinely thin and the answer is GameAnalytics rather than
   Firebase — a choice with its own consequences (vendor, data location, free tier).
3. **Freshness is not a function of first-party status** — Godot's community billing plugin
   is a full Billing major ahead of Unity's first-party one.

So the deciding evidence must come from the slice: **productivity, iteration loop, and
on-device feel** — plus, for Capacitor, whether a WebView can carry a 60 fps drag.

## Open, to resolve before the Report is frozen

- ~~Unity Pro's current per-seat price from Unity's own store.~~ **RESOLVED 2026-09-12:**
  $210/mo or $2,310/yr, Personal free below $200K, Enterprise above $25M.
- ~~Billing Library version inside `@revenuecat/purchases-capacitor`.~~ **RESOLVED
  2026-09-12:** Play Billing 8.3.0 via `purchases-hybrid-common` → `purchases-android`.
- Whether the Godot billing plugin (built against 4.5.1) loads clean under 4.7.2. **Still
  open** — needs a Gradle-build export with the plugin installed, which the slice did not
  need and did not do.
- Per-plugin 16 KB alignment for the Capacitor path. **Partly answered empirically:** the
  Capacitor APK as built contains **no native `.so` at all**, so the question only arises
  when the AdMob plugin brings the Google Mobile Ads native libraries in.

## Play Billing standing, all three side by side

| Candidate | Billing Library | Clears the v8 bar (since 2026-08-31)? | Headroom |
|---|---|---|---|
| Godot (`godot-google-play-billing` 3.3.0) | **9.1.0** | yes | to 2028-08-31 |
| Unity (`com.unity.purchasing` 5.0.4) | 8.0.0 | yes | to 2027-08-31 |
| Capacitor (RevenueCat 13.5.1) | 8.3.0 | yes | to 2027-08-31 |

The community plugin is the one with the most runway. That is worth saying plainly,
because it is the opposite of what the brief's framing expected.
