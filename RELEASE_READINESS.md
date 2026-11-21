# RELEASE_READINESS.md — Toybox line: from build to the Play Store

**Status:** DRAFT, written in session CL-S06 (2026-11-21). **Nothing here is ruled.** The
rows that wait on Gian are S44 (studio) and CL-19 to CL-24 (`PORTFOLIO.md`). Google facts were
checked on Google's own pages on **2026-11-21** (§9). Recheck them at each title's first
submission, because Google changes these pages.
**Serves every title in the line.** 005 Flight Dispatch is the first case. Facts about one
game are marked *(005)*, and the game's own changes go to its own session (§8).

**Not legal, tax or business advice.** Wherever a question is legal, tax or business, this
file flags it for Gian (**⚑**) and does not answer it (charter §4).

---

## 1. The goal, and the critical path

**Goal:** the closed test (12 testers × 14 continuous days) never becomes the critical
path to release.

**The one fact that makes this possible:** the 14-day clock counts *testers opted in to
the closed track*. It does not count the build. Google requires "a minimum of 12 testers who
have been opted in continuously for at least 14 days" when you apply for production [A].
Nothing on Google's pages says the closed track must carry the final build. So the test can
**open with the first build that can be published** (no ads, placeholder art) and run while
the later milestones are being built. Each new build is pushed to the same track, and
testers stay opted in.

**Proposed line rule (CL-19):** a title's closed test opens at the **first build that can be
published**, not after its monetization milestone. For 005, that is the M2 or M3 build,
not M4. Then M5 is only the production application and the release.

### Back-schedule from release (T = production listing live)

The days come from Google's stated review times (§9). "Not stated" means Google gives no
number, so the plan keeps a margin.

| When | Step | Who | Lead time (source) |
|---|---|---|---|
| T−56 | **Account type ruled** (S44). Account created, $25 paid, identity verified, device verified | Gian | Personal: days, not stated [C][G]. Organisation: **D-U-N-S first, "up to 30 days"** [B] |
| T−49 | **Roster committed**: 16–20 named people with a Google account, recruited into a Google Group (§3) | Gian, Claude drafts the ask | ~1 week of asking |
| T−49 | **Privacy policy live** on a public HTTPS page (§4) | Claude drafts, Gian publishes | Hours, after the host is chosen (CL-22) |
| T−45 | **App content forms** filled in Play Console: privacy policy URL, ads declaration, target audience, content rating, Data safety. **Store listing** filled with placeholder graphics | Gian, from Claude's drafts | One sitting. Target audience review "up to 7 days" [P5] |
| T−45 | First AAB on **internal testing** (no gate, "within seconds" [A]). This is also a smoke test | Gian uploads | Minutes |
| T−42 | Same AAB promoted to **closed testing**, submitted for review | Gian | Review of a new app's first release: not stated on these pages. Margin: 7 days |
| T−35 | Track published, so the **opt-in link appears** (it does not show while "Draft" or "Pending publication" [D]). Link sent to the roster | Gian | — |
| T−35 → T−14 | **The 14 days**, with a 7-day margin for testers who drop out. Later builds (art, then ads) are pushed to the same track | testers | 14 days minimum [A] |
| T−14 | **Apply for production** (Dashboard → Apply for production; three groups of questions, §3.4) | Gian, from Claude's draft | Review "usually takes seven days or less" [A] |
| T−7 | First **production release** submitted | Gian | Review time not stated. Margin: 7 days |
| T | Listing live | — | — |

**Latest safe start: about 8 weeks before the release date Gian wants.** Recommended
start for 005: **now**. With M3 and M4 not yet dated, the cheapest way to keep this off the
critical path is to start the account and the roster in parallel with M2 play. If S44 is
ruled for a personal account this week, the 14 days could close around the end of December
2026. That is before M4 is likely to be done.

---

## 2. Developer account type: personal or organisation (studio row S44 · line row CL-20)

Studio-wide: the same account publishes every Gabella title, Core included. So the
ruling is S44 in `../studio/LEDGER.md`; CL-20 carries its consequence for the line.

| | **Personal** | **Organisation** |
|---|---|---|
| Meant for | "personal use—for example, if you're a student, hobbyist, or amateur developer" [F]. Both types "can monetize on Google Play" [F] | Companies. *Required* for finance, health, VPN and government apps [F]. Games are not on that list |
| Closed test 12 × 14 | **Required** for personal accounts created after 2023-11-13 [A] | **Google's page does not say.** [A] is written only for personal accounts. It neither exempts organisations nor includes them. ⚑ confirm with Play support before ruling on this basis |
| Required information | Developer name, legal name and address, contact email and phone, a public developer email, trader status (EEA/UK) [B] | **D-U-N-S number**, organisation name, address and phone, **organisation website**, contact person, public developer email and phone [B] |
| Identity | Legal name and address from the Google payments profile, verified. Google "may require a valid government ID and a credit card, both issued under your legal name" [C] | Same, for the organisation |
| Device | Must verify a real Android 10+ device with the Play Console app, "less than a minute" [G]. The S24 Ultra qualifies | Not stated on [G] (written for personal accounts) |
| Shown publicly | Legal name, country, developer email. **"If you monetize on Google Play, Google displays your full address"** [B] | The organisation's details |
| D-U-N-S | — | **Free** ("a completely free process", with a warning about sites that charge) [B][F]. Lead time "can take up to 30 days" [B]; another Google page says "up to 28 days" [I]. Dun & Bradstreet's own figure was not checked (non-Google) |
| Fee | **US$25, one time** [C]. No prepaid cards [C] | Same [C] |
| Android developer verification | Took effect 2026-09-30 in Brazil, Indonesia, Singapore and Thailand, global in 2027 [E]. For Play developers, "no new action is required" in most cases. Play registers new package names automatically [H] | Same |

**⚑ Flagged for Gian, not answered here:**
1. Is Gabella a registered legal entity that *could* hold an organisation account (and get a
   D-U-N-S)? That is a business and legal question.
2. **Public address.** With a personal account, does ad revenue count as "monetize on Google
   Play", which would put Gian's full home address on the store? Google's page does not say
   whether ads-only counts [B]. This is a privacy and legal question.
3. **Trader status** (EU Digital Services Act). It is declared at sign-up, and a trader's
   contact details are shown in the EEA/UK [B]. Legal question.
4. **Tax.** Play's payments profile and AdMob both ask for tax information (AdMob's threshold
   for tax information is $0 [P8b]). Which forms apply to Gian's country of residence is a
   tax question.
5. **Switching later.** Whether a personal account can become an organisation account
   later, and what that does to live titles, was not checked on Google's pages. It may make
   the choice reversible.

**Neutral summary for the ruling:** personal is fastest (days) and costs the closed test.
Organisation costs a legal entity, a website and up to 30 days of D-U-N-S. Whether it saves
the closed test is unconfirmed. For 005, the closed test is already planned off the critical
path (§1), so on schedule alone the two routes cost about the same. The deciding questions
are ⚑1–⚑2.

---

## 3. The closed test and its 12 testers

### 3.1 Google's rules [A][D]
- At least 12 testers opted in when you apply, and **opted in continuously for the preceding
  14 days**. "Testers who opt in, test for fewer than 14 days, and then opt out do not count."
  If a tester opts out and back in, "the 14 days must be consecutive" [A].
- Google can refuse the application for "fewer than 12 opted-in testers **or insufficient
  tester engagement**" [A]. So the testers must actually play, not only opt in.
- Tester lists: email lists or **Google Groups** (no size limit) [D]. Each tester needs a
  Google account [D].
- Someone who opts into a title's *internal* test "is no longer eligible" for its open or
  closed test [D]. **Keep the roster off the internal track** and use internal testing for
  Gian alone. (Google implies this rather than stating it.)
- Testers can't post public reviews; their feedback doesn't touch the public rating [D].
- **Per title, or per account?** The pages say a closed test "for their app" [A]. This plan
  assumes **every new title** on a personal account repeats the 12 × 14. Not confirmed; check
  at 005's production application.

### 3.2 What the opt-in asks of a tester (the message Claude drafts for Gian)
1. Join the Google Group "Gabella testers" with the Google account used for Play on their
   phone, or send that address to Gian.
2. Open the opt-in link (web), read the tester terms, tap to become a tester [D].
3. Install from the **Play Store link Gian sends**. A closed-test app may not show up in
   search, and Google's two statements on this conflict [D]. Always send the link.
4. Keep it installed and **don't leave the test for 14 days**. Play it a few times; updates
   arrive by themselves.
5. Optional: one line of feedback by message. It feeds the production application (§3.4).

Cost to a tester: about 5 minutes once, plus a few short sessions. No account to create
beyond the Google account they already have.

### 3.3 Where 16–20 people could come from (aiming for 12 who stay)
Aim for **16–20** so that dropouts don't restart the clock. Ranked by how likely they are to
stay 14 days:

| Source | Likely stay | Notes |
|---|---|---|
| **Family** (charter §2: "family counts") | High | Each person needs their own Google account. Gian's son counts only with an account of his own, and that raises the target-audience question (§4, CL-21) |
| **Office colleagues** | High | Gian's productive hours are at the office (CL-18). They see him daily, which is the best way to keep people from dropping out |
| **Aviation acquaintances** | Medium–high | Feedback with domain knowledge, a bonus for 005 |
| **Friends abroad / social circles** | Medium | Easy to ask, weaker follow-through |
| **Tester-exchange communities** (subreddits and groups where developers test each other's apps for the 12 × 14) | Medium | Useful for filling the last few places. Costs reciprocal testing of other people's apps, which is construction-time work and allowed under charter §6's 2026-09-15 clarification. ⚑ Whether Google treats exchange testers as "engagement" is not stated |
| **Paid testing services** | — | **Not recommended without a ruling.** ⚑ Whether they comply with Play policy is not stated on these pages; check before considering |

**A roster that lasts:** one Google Group, reused by every later title, keeps the recruiting
cost to one time. A tester added for 005 is already in place for title two.

### 3.4 The production application (draft it during the 14 days) [A]
Three groups of questions: **the closed test** (how hard testers were to recruit, whether they
used all features, whether use matched production, a feedback summary and how it was
collected); **the game** (target audience, "what makes your game unique", installs
expected in the first year; not shown publicly); **readiness** (what changed after the test,
how you decided it was ready). For 005, the "unique" answer is already written: C02 §12,
"a visible future". Keep a short dated log of tester feedback from day 1. That log is the
evidence the first group of questions asks for.

---

## 4. Store prerequisites, with lead times

All Google-sourced (§9) unless marked *unverified*. "Gate" = what this item blocks.

| Item | What Google requires | Lead time | Gate | Who |
|---|---|---|---|---|
| **Privacy policy** | Required for every app, "even if your app doesn't access personal or sensitive user data". Linked **on the store listing and inside the app**. On "an active, publicly accessible URL" with valid SSL, worldwide, no login, "not a PDF" [P1]. Contents: developer and privacy contact, data collected, used and shared and with whom, security, **retention and deletion** [P2]. With an ad SDK, it has to cover what the SDK collects | Hours to draft. The host is the decision (CL-22) | Closed track (App content) and Data safety | Claude drafts, ⚑ Gian judges whether it is legally adequate, Gian publishes |
| **Developer website** | Needed for an organisation account [B]. For AdMob's `app-ads.txt`, *unverified* (that page wasn't fetched) | Same host as the privacy policy | Organisation account; ad fill later | Gian |
| **Data safety form** | Every app fills it, including apps that collect nothing. It must include what **third-party SDKs** collect or share [P3] | One sitting | Closed track | Claude drafts from the kit's manifest and SDK list. **Must be redone at M4, before the ads build reaches the track** |
| **Ads declaration** | "You must declare whether your app contains ads", SDK ads included; "Contains ads" is shown on the listing [P6] | Minutes | Closed track | Gian |
| **Target audience and content** | Declared before publishing; ads and privacy policy must be done first; review "up to 7 days" [P5]. If children are included: **Families policy**, only Families-certified ad SDKs, no interest-based ads, no advertising ID, no interstitial at launch, a neutral age screen for mixed audiences [P5][P7] | One sitting + up to 7 days | Closed track; the kit's ad setup | **Gian rules the age bands (CL-21)**. ⚑ child-privacy law is a legal question |
| **Content rating (IARC)** | Questionnaire in Play Console for each app. "Apps without a content rating will be removed" [P4]. Ads must not be "significantly more mature" than the app [P4] | Not stated. One sitting | Closed track | Gian, from Claude's draft answers |
| **Store listing** | Title **≤30 characters** [P9]. Short description **≤80** [P4b]. Full description: limit not confirmed (*unverified*, commonly 4,000). Icon 512×512 32-bit PNG ≤1 MB. Feature graphic 1024×500. **2–8 phone screenshots**, at least 4 at ≥1080 px recommended [P4b] | Placeholder: a day. Final: after the art pass (005: M3) | Closed track needs a listing (*practice*: the main listing has to be complete before publishing to a closed track; not quoted on these pages) | Claude produces from builds, Gian approves |
| **Ad SDK account (AdMob)** | Google account, country of residence (permanent), phone PIN, "typically up to 24 hours … in rare cases … up to 2 weeks" [P8]. Payment profile: Individual or Organization (**permanent**), name as on the bank account, postal address [P8a]. **Address PIN by mail** at a $10 balance; payout at $100 [P8b]. Consent for EEA/UK/CH through Google's UMP SDK, at every launch [P8c][P8d] | Sign-up: 1 day to 2 weeks. Mailed PIN: weeks, but it **blocks payouts, not release** | M4 (the kit's ads), not the closed test | Gian signs up; the kit builds UMP + ads at M4. ⚑ Individual or Organization should match S44 |
| **AdMob app linking / app review / `app-ads.txt`** | *Unverified*: those pages weren't fetched. Expect real ads to serve only after AdMob links the app to its Play listing. Use test ads until then | Unknown | Real ad revenue after release | Recheck at M4 |
| **Name clearance** | Charter §4: "before any store listing". §5 below | Done for Play, 2026-11-21. Trademark: ⚑ | Store listing | Gian rules (CL-23) |
| **Store disclosure** (AUTHORSHIP) | Charter §4: "follows whatever the store requires at submission". **Not checked this session** | At the production submission | Production | Recheck at T−14 |

---

## 5. Name clearance: "Flight Dispatch" *(005)*

**Play Store search, 2026-11-21**, built-in browser, store region Dominican Republic, plus
`hl=en&gl=US` searches for "Flight Dispatch" (with and without quotes), "Flight Dispatch game",
"Flight Dispatcher", "Airport Dispatch":

- **No listed app is titled "Flight Dispatch".**
- Close names:
  - **FL3XX Dispatch** (FL3XX): business-aviation operations software, 1K+ installs. A tool, not a game.
  - **iFly Dispatch** (Adventure Pilot, LLC): pilot app, 1+ installs.
  - **Flight dispatcher game** (ProfSel): an air-traffic game with ads, 100+ installs, last updated 2024-10-21.
  - **PlaneFlow: Airport Dispatcher** (MAUMIU WORKS): an arcade airport game. Its listing says
    each aircraft has "its own list of jobs" (fuel, boarding, repairs), with stars, rush hour and
    VIP flights. **This is a design neighbour, not only a name neighbour** (§8).
- "Flight Dispatch" is 15 characters, so a 30-character title leaves room for a subtitle.

**⚑ Flagged for Gian, not answered here:**
- No trademark search was run (USPTO, EUIPO, WIPO, or the country of residence).
- "Flight dispatch" and "flight dispatcher" are ordinary aviation job terms. Whether that makes
  the name weak to register, safe to use, or both is a trademark question.
- Whether the closeness to "Flight dispatcher game" or "FL3XX Dispatch" matters legally is
  also a trademark question.
- Charter §4 already says "a lawyer's read is a per-title decision".

---

## 6. What waits on Gian

**Rulings** (ledger rows; effort tags per studio S15):

| Row | Effort | The ruling | Needed by |
|---|---|---|---|
| **S44** (studio) | 🔧 review | Developer account type for all of Gabella: personal or organisation (§2, ⚑1–5) | T−56; for 005, now |
| **CL-20** | ⚡ ruling | Line side of S44: the line can't upload anything until the account exists. Mirrors S44 | Same |
| **CL-19** | 🔧 review | Adopt this file as the line's release plan, including the rule "the closed test opens at the first build that can be published" (§1) and the Google Group roster (§3.3) | Before 005's next milestone review |
| **CL-21** | ⚡ ruling | Target audience age bands for the line's default (adults/teens only, or including children, with the Families consequences in §4). ⚑ legal part | T−45 |
| **CL-22** | ⚡ ruling | Where the privacy policy and developer website are hosted (one static page for the line, all titles) | T−49 |
| **CL-23** | ⚡ ruling | Name clearance for "Flight Dispatch": Play search clean (§5); the trademark question is Gian's | Before the first store listing (T−45) |
| **CL-24** | ⚡ ruling | Tester sources: which of §3.3's sources Gian accepts (in particular exchange communities; paid services are not recommended) | T−49 |

**Acts** (Gian's hands, never Claude's; no ruling needed once the rows above are ruled):
create the developer account and pay the $25 · verify identity and the device · create
the Google Group and invite the roster · publish the privacy-policy page · fill App content
from Claude's drafts · upload and promote the AAB · send the opt-in link · sign up for AdMob
(before M4) · apply for production · submit the production release.

---

## 7. The checklist for each title (reuse for title two onward)

- [ ] Account exists (once for Gabella) · roster group exists (once for the line)
- [ ] Title's name cleared on Play; trademark question put to Gian
- [ ] Title's privacy policy page published (line host, CL-22)
- [ ] App content: privacy URL · ads · target audience · content rating · Data safety
- [ ] Store listing with placeholder assets
- [ ] First build that can be published → internal (Gian) → closed (the roster); opt-in link sent
- [ ] Feedback log open from day 1
- [ ] Data safety redone before the build with ads reaches the track
- [ ] Day 14+ with ≥12 still opted in → apply for production (draft §3.4)
- [ ] Final assets replace placeholders · disclosure checked · production release

---

## 8. For 005's own session (this file doesn't edit `flight-dispatch/`)

1. GDD §9 says "Start finding the 12 at M3". This plan recommends starting recruitment and the
   account **now**, and opening the closed test at the M2 or M3 build (CL-19). If CL-19 is
   ruled, §9 and §10's M5 row follow.
2. **PlaneFlow: Airport Dispatcher** (MAUMIU WORKS) is the nearest design neighbour found:
   aircraft with service lists, stars, rush hours. C02 §12's spine, "announced arrivals",
   isn't mentioned in its listing. Worth one look by 005 before the store copy is written.
   It is evidence, not a verdict.
3. M4's build must arrive with the Data safety form updated for the ad SDK and UMP.
4. The kit's minimum Android version doesn't block closed testing. The device verification
   needs Android 10+ [G], and the S24 Ultra meets it.

---

## 9. Sources (all checked 2026-11-21)

- [A] https://support.google.com/googleplay/android-developer/answer/14151465: App testing requirements for new personal developer accounts
- [B] https://support.google.com/googleplay/android-developer/answer/13628312: Required information to create a Play Console developer account
- [C] https://support.google.com/googleplay/android-developer/answer/6112435: Get started with Play Console (US$25 one-time fee)
- [D] https://support.google.com/googleplay/android-developer/answer/9845334: Set up an open, closed, or internal test
- [E] https://developer.android.com/developer-verification: Android developer verification
- [F] https://support.google.com/googleplay/android-developer/answer/13634885: Choose a developer account type
- [G] https://support.google.com/googleplay/android-developer/answer/14316361: Device verification requirements for new developer accounts
- [H] https://developer.android.com/developer-verification/guides/google-play-console: Verification for Play Console developers
- [I] https://developer.android.com/developer-verification/guides/faq: Developer verification FAQ (D-U-N-S "up to 28 days")
- [P1] https://support.google.com/googleplay/android-developer/answer/9859455: Prepare your app for review (privacy policy, ads, target audience, ratings)
- [P2] https://support.google.com/googleplay/android-developer/answer/10144311: User Data policy
- [P3] https://support.google.com/googleplay/android-developer/answer/10787469: Data safety section
- [P4] https://support.google.com/googleplay/android-developer/answer/9898843: Content ratings
- [P4b] https://support.google.com/googleplay/android-developer/answer/9866151: Preview assets
- [P5] https://support.google.com/googleplay/android-developer/answer/9867159: Target audience and content
- [P6] = [P1] (ads declaration)
- [P7] https://support.google.com/googleplay/android-developer/answer/9893335: Families policies
- [P8] https://support.google.com/admob/answer/7356219: Sign up for AdMob
- [P8a] https://support.google.com/admob/answer/12835021: AdMob payment details
- [P8b] https://support.google.com/admob/answer/2772208: AdMob payment thresholds
- [P8c] https://developers.google.com/admob/android/privacy: UMP SDK
- [P8d] https://support.google.com/admob/answer/10113004: GDPR ad partners (EU user consent policy)
- [P9] https://support.google.com/googleplay/android-developer/answer/9898842: Metadata (title ≤30 characters)
- Play Store searches: play.google.com/store/search, 2026-11-21, built-in browser.

**Not checked this session:** Dun & Bradstreet's own pages · AdMob app-ads.txt, app linking
and app review · Google's EU user consent policy page · Play's trader-status, monetization-and-ads
and AI-content policies · trademark registers.

## Changelog
- 2026-11-21: written, CL-S06. Nothing ruled.
