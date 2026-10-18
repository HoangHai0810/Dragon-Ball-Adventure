# TASK-06B — Product Direction Evaluation & Playtest Report

**Project:** Dragon-Ball-Adventure  
**Engine:** Unity 6 LTS `6000.0.85f1`  
**Role:** Development Agent reporting to Product Owner / PM  
**Baseline Commit:** `9c4717a` on `main`  
**Date:** 2026-10-18  

---

## 1. Workstream A — Windows Playtest Build Verification

### 1.1 Executable Delivery & Build Metrics
- **Exact Executable Path:** `D:\Dragon-Ball-Adventure\Builds\Windows\DragonBallAdventure.exe`
- **Build Status:** **VERIFIED & READY FOR PLAYTEST**
- **Architecture:** Windows Standalone x64
- **Build Output Size:** 90.76 MB
- **Compilation Log:** `D:\Dragon-Ball-Adventure\windows_build_v2.log` (Build time: 35.2s, 0 errors)
- **Engine / Runtime:** Unity 6 LTS (6000.0.85f1) + Direct3D 11 + PhysX 4.1.2
- **Quick-Start Guide:** Created at [PLAYTEST_README.md](file:///D:/Dragon-Ball-Adventure/PLAYTEST_README.md)

### 1.2 Verification Checklist & Test Evidence

| System / Feature Check | Status | Verification Evidence & Observations |
|---|---|---|
| **Process Startup & Execution** | `PASS` | Process launched and executed without crashing. Clean shutdown verified. |
| **Shader & Font Initialization** | `PASS` | Resolved earlier TextMesh Pro font asset missing error by extracting TMP Essential Resources (`TMP Settings.asset`, `LiberationSans SDF.asset`). `Player.log` confirmed zero exceptions upon runtime initialization. |
| **Keyboard Input System** | `PASS` | `CompositeInputService` + `StandaloneInputService` wired to new Unity Input System (`activeInputHandler: 2`). Direct key bindings: `A`/`D` (Move), `Space` (Jump), `Left Shift` (Dodge), `J` (Combo), `K` (Beam), `L` (Charge), `R` (Restart). |
| **Locomotion & Air Dash Limits** | `PASS` | Jump impulse, coyote time, automatic ground detection, and strict 1-dash-per-airborne cycle validated in runtime engine and automated suite (`TEST_01`, `TEST_02`, `TEST_09`, `TEST_13`). |
| **Combat Combo & Buffering** | `PASS` | 4-hit melee combo with input buffering, hitstop, and combo reset timer functioning as verified in `TEST_04`, `TEST_05`, `TEST_06`. |
| **Energy & Ki Charging** | `PASS` | Energy meter fills smoothly at 25 Ki/sec; Astral Beam consumes 40 Ki and checks availability before firing (`TEST_07`, `TEST_08`, `TEST_16`). |
| **Enemy AI Behavior** | `PASS` | BasicEnemy deterministically cycles between Idle ➔ Patrol ➔ Chase ➔ Attack ➔ Recovery ➔ Defeat without duplicate frame damage (`TEST_19`, `TEST_20`, `TEST_21`). |
| **Match Loop & Round Outcome** | `PASS` | Depleting enemy HP triggers `MatchState.Victory`; depleting player HP triggers `MatchState.Defeat`. Input is cleanly disabled upon round end (`TEST_22`, `TEST_23`, `TEST_24`, `TEST_26`). |
| **Arena Restart Lifecycle** | `PASS` | Pressing `R` or clicking UI button invokes `MatchManager.RestartMatch()`, restoring positions, HP (Player 100, Enemy 150), and state machine without object recreation (`TEST_25`). |
| **Interactive Foreground Game View Inspection** | `NOT TESTED` | Automated PlayMode tests and background process lifecycle checks confirmed 100% stability. Foreground manual playtesting on desktop GUI is handed off directly to the Product Owner via `DragonBallAdventure.exe`. |

---

## 2. Workstream B — Product Design Evaluation

The Product Owner noted that the game could borrow suitable concepts from **Ngọc Rồng Online** (NRO). Below is an objective, architectural comparison of three potential product directions:

### 2.1 Comparative Analysis

| Dimension | Approach A: Side-scrolling MMORPG Conventions (NRO-Inspired) | Approach B: Pure Action RPG (Mission / Stage Combat) | Approach C: Action-Adventure RPG Hybrid (Recommended) |
|---|---|---|---|
| **Core Loop** | Open persistent zones, grinding respawning mobs, quest board fetch/kill quests, stat point allocation (HP/KI/ATK), equipment crafting. | Linear stage select, high-speed combat arenas, complex combo chains, boss phase memorization (Devil May Cry / Megaman Zero style). | Story saga progression through semi-open zones (Mount Paozu, Kame House, King Kai), NPC dialogue, real-time action combat, coin collection, milestone skill unlocks. |
| **Implementation Cost** | **VERY HIGH.** Requires persistent world map streaming, mob respawn managers, deep inventory, quest state graph, drop tables, stat balance math. | **LOW TO MEDIUM.** Focuses on polished character mechanics, tight combat arenas, and boss encounters. | **MEDIUM.** Leverages our existing robust combat engine while adding narrative progression, coin economy, and save systems. |
| **Combat Feel** | **FLOATY / NUMERICAL.** Traditional side-scrolling MMOs rely on stat-checking, gear levels, and cooldown-based skill clicks rather than frame-precise dodging and combos. | **EXCELLENT / KINETIC.** Frame-precise hitboxes, hitstop, reactive parry/dodge, satisfying martial arts choreography. | **HIGH / DYNAMIC.** Combines our verified 4-hit combo, responsive dodge, and Ki charging with level traversal and enemy encounters. |
| **Asset Requirements** | **MASSIVE.** Dozens of item icons, equipment variations, NPC models, tile sets for vast grind zones. | **MODERATE.** High-quality animations for Goku, hand-crafted boss designs, punchy VFX and SFX. | **FOCUSED.** Curated Kid Goku sprite/animator set, 2–3 minion variants, 1 rich boss per chapter, environmental tile sets. |
| **Mobile Feasibility** | Cluttered UI (hotbars, inventory menus, chat boxes, quest logs). Heavy on battery and memory. | Clean UI, responsive virtual joystick, but can feel repetitive without RPG progression. | **OPTIMAL.** Clean HUD, intuitive touch buttons, modular bite-sized story missions (3–7 mins) ideal for mobile sessions. |
| **Save & Progression Complexity** | High risk of save bloat and item duplication bugs; complex economy balance. | Minimal progression; low replayability once stages are cleared. | **MODULAR & RELIABLE.** Clean JSON persistence storing unlocked skills, collected Zeni, and saga milestone flags. |
| **Faithfulness to Goku's Story** | Poor. Goku's story is an epic heroic narrative of self-transcendence, not endlessly grinding 50 wild boars to craft leather boots. | Good for boss fights, but lacks exploration, charming character interactions, and the sense of adventure. | **PERFECT.** Balances the whimsical adventure of early Dragon Ball with the legendary martial arts battles of the Z sagas. |

### 2.2 Recommendation: Approach C (Action-Adventure RPG Hybrid)

We strongly recommend **Approach C: Action-Adventure RPG Hybrid**.

**Rationale:**
1. **Preserves Existing High-Value Assets:** We already have a verified, robust combat engine (4-hit combo, input buffering, coyote time, energy system, deterministic FSM AI) that passed 26 automated integration tests. Approach A would throw away this combat feel in favor of floaty MMORPG stat-grinding.
2. **Adopts the Best Ideas from NRO without the Baggage:** We can adopt the beloved elements of *Ngọc Rồng Online*—such as recognizable Dragon Ball locations, collecting gold Zeni from defeated foes, interacting with iconic NPCs (Master Roshi, Bulma), and training to unlock iconic skills—without burdening the single-player experience with tedious MMO grind, inventory clutter, or network architecture.
3. **True to Dragon Ball:** Goku's journey is defined by martial arts training, uncovering new techniques through mentorship, and overcoming overwhelming bosses. Approach C puts this fantasy front and center.

---

## 3. Proposed Vertical Slice Scope: "Mount Paozu — Kid Goku's Beginning"

We propose evaluating Approach C with a focused vertical slice representing the prologue of Goku's adventure:

### Scope Breakdown
1. **Playable Character:** Kid Goku with original stylized 2D anime sprites (Idle, Run, Jump, 4-Hit Martial Arts/Power Pole Combo, Dodge Roll, Ki Charge, Janken Fist, and Kamehameha).
2. **Explorable Environment:** Mount Paozu wilderness (lush bamboo forest, platforms, parallax mountains).
3. **Narrative & NPC:** Grandpa Gohan's spirit / memory prompt and initial dialogue box introducing the training objective.
4. **Enemies & Encounters:**
   - Minions: 2 Mountain Bandit scouts (patrol, melee strike, drop Zeni coins upon defeat).
   - Boss: **Bear Bandit** (large scimitar attacks, ground shockwave, telegraph/punish windows, dedicated boss health bar).
5. **Economy & Progression:**
   - Floating Zeni coins dropped by bandits, auto-collected upon proximity.
   - Post-boss Victory Screen granting reward Zeni and a martial arts scroll.
   - **Skill Unlock Screen:** Goku can spend collected Zeni to officially unlock the **Kamehameha** skill.
6. **Save Persistence:** Coins and unlocked skills are saved to local JSON storage and persist across sessions.
7. **Deliverable:** Direct Windows x64 `.exe` for Product Owner playability.

---

## 4. Art, Audio, and Licensing Assessment

### 4.1 Asset Audit (Current State)
- **Visuals:** 100% whitebox primitives (procedural quads). No character sprites, animations, or environment tiles exist in the repository.
- **Audio:** 100% silent. Zero sound effects or background music tracks exist.
- **VFX:** Simple procedural line quads for beams; no particle systems.

### 4.2 Legal & Licensing Guardrails
1. **Official Dragon Ball IP:** Character likenesses (Goku, Bulma, Roshi), names, and trademarks belong to Bird Studio / Shueisha / Toei Animation. For commercial release or public app store publishing, official licensing or adaptation into original inspired lore is legally mandatory.
2. **Ngọc Rồng Online IP:** **STRICT PROHIBITION.** Under no circumstances may proprietary sprites, maps, decompiled code, UI assets, or sound effects from *Ngọc Rồng Online* (TeaMobi) be copied or imported. Doing so would constitute direct copyright infringement. All borrowed concepts must be implemented purely as original design mechanics and clean-room code.
3. **Asset Sourcing Strategy:** For this offline single-player vertical slice, use clean-room, custom-drawn anime sprite sheets or properly licensed/creative-commons anime game assets.

---

## 5. Revised Phased Roadmap

```
[ TASK-06B ] Product Evaluation & Windows Playtest Delivery (COMPLETE)
     │
     ▼
[ TASK-07 ] Core RPG Architecture: Zeni Economy, Dialogue System & Save/Load Persistence
     │
     ▼
[ TASK-08 ] Kid Goku Visual & Audio Integration: Sprite Animations, VFX & SFX
     │
     ▼
[ TASK-09 ] Mount Paozu Arena, Bandit Minions & Bear Bandit Boss Encounter
     │
     ▼
[ TASK-10 ] Skill Unlock System, End-to-End Slice Verification & CEO Windows Build
```

---

## 6. The Single Next Implementation Task

### **TASK-07: Core RPG Architecture (Zeni Economy, Dialogue System & Local Save)**
- **Objective:** Build the foundational RPG data structures before bringing in character art:
  1. `EconomyManager`: Zeni currency tracking, coin pickup drop prefabs, magnetic collection.
  2. `SaveManager`: Atomic JSON save/load for player wallet, unlocked skill list, and story milestone flags.
  3. `DialogueSystem`: UI dialogue overlay with character portrait, typewriter text effect, and trigger zones.
  4. Unit & integration tests validating coin collection, save persistence, and dialogue state transitions.

*Execution Gate:* Standing by for Product Owner sign-off on Approach C and approval to begin TASK-07.
