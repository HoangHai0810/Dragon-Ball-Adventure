# TASK-06 — Dragon Ball Adventure: Product Audit & Development Roadmap

**Project:** Dragon-Ball-Adventure  
**Engine:** Unity 6 LTS `6000.0.85f1`  
**Auditor / Role:** Lead Development Agent  
**Recipient:** Product Owner / Project Manager  
**Baseline Commit:** `51f21c4` on `main`  
**Date:** 2026-10-18  

---

## 1. Executive Summary & Vision Alignment

The current repository state at commit `51f21c4` contains an early, functional 2D whitebox combat prototype developed under the generic test moniker *"Aether Surge"*. While it validates foundational real-time mechanics (movement, 4-hit melee combos, input buffering, energy charging, hurtboxes, a 6-state enemy AI, and automated test harnesses), **it does not yet reflect the Dragon Ball Action RPG product vision**.

### The Core Product Vision
A Dragon Ball Action RPG where the player controls **Goku**, reliving the iconic journey from his childhood adventures through major Dragon Ball sagas:
1. **Character Authenticity:** Play as Goku with recognizable character presentation, distinct animations, martial arts combos, Power Pole strikes, Ki blasts, and Kamehameha.
2. **Saga-Driven Progression:** Structured story missions, recognizable environments, dialogue sequences, minion skirmishes, and signature multi-phase boss battles.
3. **Action RPG Combat Depth:** Real-time combat with melee combo chains, evasive maneuvers (dodge/air dash/vanishing), Ki charging, special skill execution, and eventual transformations (Kaioken, Super Saiyan).
4. **RPG Economy & Progression:** Earn in-game Zeni/Coins through gameplay milestones, unlock and upgrade skills, and persist progression across play sessions.
5. **Direct Playability:** Frequent playable standalone Windows builds (`.exe`) enabling rapid evaluation and hands-on validation by the Product Owner before advancing to mobile deployment.

---

## 2. Comprehensive Repository Audit

### 2.1 Git & Workspace Status
- **Current Branch:** `main`
- **Current Commit:** `51f21c4` (`feat(task-05): first playable slice with basic enemy AI, match lifecycle, and 26/26 playmode tests`)
- **Remote Synchronization:** Up to date with `origin/main` (`https://github.com/HoangHai0810/Dragon-Ball-Adventure`)
- **Working Tree:** Clean working tree.
- **Automated Tests:** 26/26 PlayMode tests passing in Unity Test Framework (`test-results-v10.xml`).

### 2.2 System & Asset Audit Matrix

| System / Asset Category | Status | Technical Reality in Repository |
|---|---|---|
| **Character Visuals (Goku)** | `MISSING` | **Zero character models, sprites, or textures exist.** The current player is a procedurally generated cyan rectangle (`Texture2D.whiteTexture`). |
| **Character Animations** | `MISSING` | **Zero `.anim` clips or Animator Controllers exist.** Current visual feedback relies entirely on procedural code-driven scale shifts, rotation wobbles, and sprite color flashes. |
| **Enemy Assets & Variety** | `MISSING` | Enemies are represented by red procedural 2D quads. No Dragon Ball enemies (Bandits, Red Ribbon Soldiers, Pilaf robots) exist. |
| **Audio (BGM / SFX / Voice)** | `MISSING` | **Zero audio clips exist (`.wav`, `.mp3`, `.ogg`).** The project is 100% silent. No AudioSources, Sound Managers, or audio mixer pipelines are hooked up. |
| **Visual Effects (VFX)** | `MISSING` | No particle systems, flipbook animations, or custom shaders. The "Astral Beam" is a stretched 2D quad; charge is a color tween. Signature Ki aura and energy impact VFX are absent. |
| **Core Locomotion (Move, Jump, Dash)** | `IMPLEMENTED` | Grounded physics detection, horizontal acceleration, variable jump impulse, coyote time, and 1-per-air cycle air dash are functional and covered by unit tests. |
| **Melee Combat & Combos** | `IMPLEMENTED` | 4-hit melee combo with hit windows, input buffering, timeout reset, and discrete hurtbox collision checks (`Physics2D.OverlapCircleAll`). |
| **Ki Charge & Energy Pool** | `IMPLEMENTED` | Energy controller tracks pool (0–100), charging rate (25/sec), consumption, and firing gate for special skills. |
| **Ranged Ki Skill** | `PARTIAL` | Functional projectile logic exists (`AstralBeamProjectile.cs`), but it is a generic placeholder quad rather than a proper Kamehameha or Ki blast. |
| **Transformations (Kaioken / Super Saiyan)**| `MISSING` | No transformation system, stat-multiplier framework, aura burst states, or visual swap logic exists. |
| **Enemy Combat AI** | `IMPLEMENTED` | Deterministic 6-state FSM (`Idle`, `Patrol`, `Chase`, `Attack`, `Recovery`, `Defeated`) with discrete single-hit registration per attack window. |
| **Match Loop & Round Lifecycle** | `IMPLEMENTED` | `MatchManager.cs` orchestrates InProgress, Victory, and Defeat, disables player input upon match end, and cleanly resets round objects without memory leaks. |
| **UI & HUD** | `PARTIAL` | Functional whitebox HUD with Player HP, Energy bar, Enemy HP, Objective banner, Victory/Defeat overlay, and Restart button. Lacks Dragon Ball branding, fonts, or styling. |
| **Mobile Touch Controls** | `IMPLEMENTED` | Virtual joystick, touch buttons, and Notch/Safe-Area fitter are functional for mobile screen layouts. |
| **Story & Mission Structure** | `MISSING` | No mission progression manager, chapter select, or quest trigger system exists. |
| **Dialogue & Cutscenes** | `MISSING` | No dialogue UI, text typewriter effect, character portraits, or conversation parser exists. |
| **Economy & In-Game Coins** | `MISSING` | No coin counter, dropped pickup item, wallet manager, or reward allocation system exists. |
| **Skill Unlock & Tree System** | `MISSING` | Skills are hardcoded on the player component; no unlock screen, skill tree, or milestone unlocks exist. |
| **Save / Load Persistence** | `MISSING` | No PlayerPrefs, JSON serialization, save file manager, or progression persistence exists. |
| **Windows Standalone Build (`.exe`)** | `MISSING` | Only an iOS Xcode export exists (`Builds/iOS`). No standalone Windows executable has been built for direct Product Owner testing. |

---

## 3. Product Gap Analysis

1. **Thematic Disconnect:** The codebase was authored with placeholder generic names (`Aether Surge`, `Rogue Construct`, `Astral Beam`) to satisfy earlier whitebox testing constraints. It must be refactored into the Dragon Ball universe (Goku, Ki, Kamehameha, Power Pole, Zeni).
2. **Visual & Auditory Void:** A game cannot convey the Dragon Ball experience as untextured geometric boxes. Recognizable 2D sprites or cel-shaded assets, signature audio effects (Ki gather, Kamehameha blast, punch impacts), and explosive particle VFX are essential.
3. **RPG Progression Missing:** Currently, the game resets to baseline on every restart. There is no concept of persistent player level, collected coins, unlocked moves, or narrative progression.
4. **Developer Verification vs. PO Playability:** Automated tests verify mathematical physics logic, but the Product Owner needs a double-clickable Windows `.exe` to evaluate game feel, pacing, and visual authenticity.

---

## 4. Proposed Vertical Slice: "Goku's Early Adventure — Mount Paozu"

To demonstrate the actual game vision with the highest efficiency and lowest delivery risk, we propose a self-contained **Vertical Slice**:

### 4.1 Narrative & Setting
- **Location:** **Mount Paozu (Wilderness / Bamboo Forest)** — Goku's home territory.
- **Narrative Premise:** Young Goku is out hunting for food and training when he encounters mountain bandits terrorizing the area. After dispatching the bandit scouts, he confronts their leader, the **Bear Bandit**.
- **Dialogue / Interaction:**
  - Brief introductory dialogue box with Kid Goku portrait: *"Time for morning training! Grandpa Gohan always said to stay sharp."*
  - Pre-boss dialogue encounter with the Bear Bandit wielding a giant scimitar.

### 4.2 Playable Character: Kid Goku
- **Visuals:** Hand-crafted or curated 2D anime-style Kid Goku sprite sheet (Idle, Run, Jump, 4-Hit Martial Arts/Power Pole Combo, Ki Charge, Kamehameha Stance, Hurt, Victory Pose).
- **Moveset:**
  - **Basic Attacks:** 4-hit combo (Punch ➔ Kick ➔ Power Pole sweep ➔ Knockaway kick).
  - **Defensive:** Dodge roll / evasive backflip.
  - **Ki System:** Hold charge to gather Ki with a visual aura effect.
  - **Initial Skill:** *Rock-Scissors-Paper (Janken Fist)* — Quick rush strike.
  - **Unlockable Skill:** *Kamehameha* — Concentrated energy beam attack.

### 4.3 Encounters
1. **Minion Skirmish (Bandit Scouts):**
   - 2–3 humanoid bandit scouts using basic melee weapons.
   - Patrol and chase behaviors, drops **Coins (Zeni)** upon defeat.
2. **Boss Encounter (The Bear Bandit):**
   - Multi-phase combatant with telegraphed heavy attacks (Scimitar Slam, Ground Shockwave).
   - Dedicated Boss Health Bar at top of screen.
   - Stun/Vulnerable window after heavy misses, rewarding dodge-and-punish timing.

### 4.4 RPG Progression & Economy Loop
- **Coin Drops:** Defeated bandits drop gold coins that magnetize/are collected by walking over them.
- **Victory Rewards:** Defeating the Bear Bandit yields a victory screen awarding bonus Coins and an ancient training scroll.
- **Skill Unlock Screen:** A dedicated UI interface where Goku can spend earned Coins to unlock the **Kamehameha** skill.
- **Save System:** Player's unlocked skills, high score, and coin balance persist to local storage (JSON / PlayerPrefs) across restarts and app launches.

### 4.5 Deliverable: Playable Windows Build
- Automated Windows Build script (`WindowsBuildExporter.cs`) targeting `Builds/Windows/DragonBallAdventure.exe`.
- Direct delivery for Product Owner evaluation on PC with full keyboard/controller support.

---

## 5. Development Roadmap & Task Breakdown

```
[ TASK-06 ] Audit & Vertical Slice Proposal (THIS DOCUMENT)
     │
     ▼
[ TASK-07 ] Foundation & Architecture: Dragon Ball Lore, Economy, Dialogue & Save System
     │
     ▼
[ TASK-08 ] Character Presentation: Kid Goku Visuals, Animations & Audio Integration
     │
     ▼
[ TASK-09 ] Mount Paozu Environment, Minion & Boss Encounter (Bear Bandit)
     │
     ▼
[ TASK-10 ] Skill Unlock System, End-to-End Vertical Slice Integration & Windows Build Delivery
```

### Detailed Task Milestones

#### Milestone 1: TASK-07 — Core RPG Architecture & Economy (P0)
- Rename namespace and data models from placeholder `AetherSurge` to `DragonBallAdventure`.
- Implement `CoinManager` (currency tracking, drop pickups, wallet persistence).
- Implement `SaveSystem` (atomic JSON save/load for player stats, coins, unlocked skills, mission flags).
- Implement `DialogueManager` & UI (portrait presentation, typing effect, skip button, trigger zones).
- Expand test suite to validate save integrity, coin collection, and dialogue state flows.

#### Milestone 2: TASK-08 — Kid Goku Presentation & Audio Juice (P0)
- Integrate Kid Goku character art and sprite animation controller (Idle, Run, Jump, Attack1-4, Charge, Skill, Hurt, Defeat).
- Implement Ki Aura particle/visual effects and impact hit-sparks.
- Integrate open-source / licensed retro anime SFX (Ki blast, energy charge loop, punch impacts, UI clicks).
- Upgrade `PlayerCharacter` with *Janken Fist* and signature *Kamehameha* beam.

#### Milestone 3: TASK-09 — Mount Paozu Arena & Boss Encounter (P0)
- Build Mount Paozu environment with 2D parallax background (hills, bamboo, clouds, wooden platforming).
- Create Bandit Scout enemy prefab with distinct sprites and weapon attacks.
- Create Bear Bandit Boss with telegraphed attacks, shockwave mechanics, and boss HUD banner.
- Implement combat area boundary gating (must defeat minions to unlock boss arena).

#### Milestone 4: TASK-10 — Skill Unlock UI, Playable Loop & Windows Build (P0)
- Implement post-mission Skill Unlock Shop (unlock *Kamehameha* with collected Zeni).
- Connect complete loop: Intro Dialogue ➔ Minion Battle ➔ Boss Battle ➔ Victory & Rewards ➔ Skill Unlock ➔ Save.
- Build automated `WindowsBuildExporter.cs` producing `Builds/Windows/DragonBallAdventure.exe`.
- Verify full PlayMode test suite and deliver release package for Product Owner hands-on verification.

---

## 6. Verification Criteria for Approval

Before proceeding to code implementation of TASK-07:
1. **Scope Alignment:** Product Owner reviews and signs off on the "Mount Paozu — Kid Goku vs. Bear Bandit" vertical slice scope.
2. **Art Style Approval:** Confirmation of 2D high-detail stylized anime pixel/sprite approach vs. low-poly 3D cel-shaded approach for Kid Goku and Mount Paozu.
3. **Execution Gate:** Upon approval, immediate commencement of TASK-07 foundation architecture and Windows build automation.
