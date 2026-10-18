# DISCOVERY, TECHNICAL PLANNING & ARCHITECTURAL CHARTER
**Project**: Mobile Offline Action RPG  
**Target Platforms**: Android & iOS  
**Phase**: Phase 1 — Discovery & Pre-Production  
**Date**: October 2026  
**Status**: PROPOSED & AUDITED  

---

## SECTION 1: WORKSPACE AUDIT

### 1.1 Repository & Project Structure
- **Workspace Path**: `D:\Dragon-Ball-Adventure`
- **Git Repository State**: Clean, freshly initialized on branch `main`. No commits present.
  - Remote origin: `https://github.com/HoangHai0810/Dragon-Ball-Adventure`
- **Existing Files & Assets**:
  - The repository currently contains **zero** project files, assets, scene files, scripts, or package manifests. Only `.git` directory exists.
  - There is no pre-existing Unity project structure (no `Assets/`, `Packages/`, or `ProjectSettings/`).

### 1.2 Development Toolchain & Host System Verification
*Audited on Windows 10/11 Host via PowerShell execution:*
- **Unity Editor / Unity Hub**: **BLOCKED / NOT INSTALLED**.
  - Checked standard paths: `C:\Program Files\Unity Hub\`, `C:\Program Files\Unity\Hub\Editor\`, `%LOCALAPPDATA%\Programs\unity-hub\`, and root drives. Neither Unity Hub nor any Unity Editor binary is present.
  - `winget list unity` verified no Unity packages registered in Windows Package Manager.
- **.NET SDK**: **NOT INSTALLED**.
  - `dotnet.exe` exists in `C:\Program Files\dotnet\`, but running `dotnet --version` outputs: `"No .NET SDKs were found."`
- **Node.js**: `v20+` present (`C:\Program Files\nodejs\node.exe`).
- **Python**: `3.13` present (`C:\Users\ADMIN\AppData\Local\Programs\Python\Python313\python.exe`).
- **Android SDK & NDK**: **BLOCKED / NOT INSTALLED**.
  - Neither `$env:ANDROID_HOME` nor `adb.exe` is configured or detected in system path.
- **Disk Storage**:
  - `C:` Drive: **7.76 GB Free** / 247 GB (CRITICAL CONSTRAINT).
  - `D:` Drive: **48.61 GB Free** / 151 GB.
  - *Implication*: Unity Editor installation (typically 12–25 GB with Android Build Support) **must strictly target Drive D:** to prevent host system disk exhaustion.

---

## SECTION 2: LEGAL & INTELLECTUAL PROPERTY (IP) ASSESSMENT

### 2.1 Licensing Risk Assessment
- The repository name and initial user inspiration reference *"Dragon Ball"*.
- **Legal Risk Summary**: *Dragon Ball*, *Dragon Ball Z/GT/Super*, their character likenesses (Goku, Vegeta, etc.), techniques (Kamehameha, Spirit Bomb, etc.), iconography (Dragon Balls, Capsule Corp, Red Ribbon, etc.), sound effects, musical scores, and names are protected intellectual property registered to Bird Studio, Shueisha, and Toei Animation, with interactive gaming licenses held under exclusive global agreements (e.g., Bandai Namco Entertainment).
- Distributing an unauthorized game on the Google Play Store or Apple App Store using protected IP will lead to:
  1. Immediate DMCA takedown and app delisting.
  2. Permanent developer account termination on Google Play Console and Apple Developer Program.
  3. Potential legal liability for trademark and copyright infringement.
- Fan-made, non-profit, or AI-generated recreations **do not** qualify as fair use under mobile commercial or store distribution regimes.

### 2.2 Proposed Original Anime IP: "AETHER SURGE: SAGA OF THE AURA BORN"
To deliver the beloved anime action fantasy with 100% legal compliance and brand equity ownership:
- **Core Setting**: *Solaria & The Shattered Spheres* — an ancient world powered by *Aether Core* crystals and spiritual energy (*Ki-resonance* / *Aether*).
- **Protagonist**: *Kaelen (Kael)* — an orphaned martial disciple of the *Solar Fist School*, who awakens the dormant *Primal Astral Core* within his spirit.
- **Visual & Thematic Tropes**:
  - High-velocity martial arts combos, airborne juggles, vanishing dashes (*Blink Strike*).
  - Energy charging stances with dynamic ground-cracking auras and wind vortex VFX.
  - Beam struggles, ki-disc projectiles (*Astral Discs*), explosive nova bursts (*Starlight Cascade*).
  - Multi-tier spiritual transformations (*Aura Awakening*, *Solar Ascendance*, *Cosmic Eclipse*), changing aura VFX, hair/clothing particle resonance, and stance archetypes.
- **Antagonistic Force**: The *Obsidian Syndicate* and the cosmic conqueror *Warlord Vane*, seeking the 7 *Primal Aether Seals* to rewrite the planetary orbit.

---

## SECTION 3: PRODUCT REQUIREMENTS DOCUMENT (PRD)

### 3.1 Target Audience & Mobile Experience
- **Target Audience**: Mobile action RPG players, anime fans (shonen genre), speedrunners, and offline single-player commuters (ages 13–35).
- **Form Factor**: Mobile phones and tablets running Android (v9.0 Pie / API 28+) and iOS (iOS 15.0+).
- **Session Design**:
  - *Micro-sessions*: 3 to 7 minutes for training challenges, skirmishes, and side bounties.
  - *Macro-sessions*: 15 to 30 minutes for narrative saga chapters, dungeon exploration, and multi-phase boss encounters.

### 3.2 Core Player Fantasy & Loop
```
[ Explore / Quest Hub ] 
         │
         ▼
[ Real-Time Combat / Dungeon Encounter ] 
         │
         ▼
[ Skill Execution, Combos, Energy Management, Aura Burst ]
         │
         ▼
[ Boss Victory / Loot & Aether Shards Earned ]
         │
         ▼
[ Training, Skill Tree Upgrades, Transformation Mastery ]
```

### 3.3 Functional Requirements Matrix

| Category | Requirement | Launch Status | Detail |
|---|---|---|---|
| **Orientation** | Landscape Mode (Fixed 16:9 to 20:9 safe area) | **Essential (Launch)** | Optimal dual-thumb mobile control ergonomics. |
| **Offline Play** | 100% Offline Single-Player | **Essential (Launch)** | Zero mandatory network calls, zero server DRM check. |
| **Local Save** | Atomic JSON + Binary Hash check | **Essential (Launch)** | Multi-slot, auto-save at checkpoints, crash-resilient backup. |
| **Combat** | Real-time 2.5D Side-Scroller Action | **Essential (Launch)** | Light/Heavy attack chains, Dodge roll/air-dash, Block/Parry. |
| **Special Moves** | Energy Management & Cooldowns | **Essential (Launch)** | 3 equipable Energy Skills + 1 Ultimate Beam / Nova. |
| **Transformation**| Tiered Surge State (*Aura Burst*) | **Essential (Launch)** | Temporary damage/speed buff, altered moveset, drain meter. |
| **Progression** | Level, Stats (ATK, DEF, KI, HP), Skill Tree | **Essential (Launch)** | Material-based skill upgrades and combat mastery nodes. |
| **Dialogue/Story** | Visual Novel style portrait sequences | **Essential (Launch)** | 2D character busts, skip/auto text, quest markers. |
| **Audio** | Sound FX, Ambient tracks, Dynamic Boss Music | **Essential (Launch)** | Volume sliders (BGM, SFX, Voice), mute on phone interruption. |
| **Settings** | Frame Rate (30/60/120), Graphics Tier, Controls | **Essential (Launch)** | Dynamic resolution scaling, vibration toggle, HUD scale. |
| **Monetization (IAP)** | Modular In-Game Purchases & Cosmetics | **Optional / Phased** | Free-to-play friendly, 100% optional, non-paywall. |
| **Cloud Save** | Google Play Games / Apple iCloud sync | *Post-Launch (v1.1)* | Optional backup; must never break local offline save. |
| **Endless Mode** | Survival Arena / Boss Rush | *Post-Launch (v1.2)* | Extended endgame replayability. |

---

## SECTION 4: GAME DESIGN PROPOSAL

### 4.1 Dimension & Perspective: 2D vs. 2.5D Evaluation
- **Option 1: Pure 2D Frame-by-Frame Sprites**
  - *Pros*: Classic hand-drawn retro feel.
  - *Cons*: Extremely labor-intensive for multi-tier transformations and equipment variations (thousands of bespoke hand-drawn frames required); rigid camera angles.
- **Option 2: 2.5D (3D Models with Stylized Cel-Shading on a 2D Constrained Physics Plane) [RECOMMENDED]**
  - *Justification*:
    1. **Animation Flexibility**: Character rigs can share complex martial arts animations, transitions, blending, and hit-stun states seamlessly without redrawing thousands of sprite frames.
    2. **Transformation Scalability**: Aura bursts, particle trails, hair mesh morphing, and dynamic lighting effects operate natively with modern shaders.
    3. **Dynamic Camera**: Smooth cinematic zoom-ins during super moves/boss intros while keeping strict 2D plane gameplay controls.
    4. **Performance**: Modern mobile GPUs (Mali-G77+, Adreno 640+) process low-to-mid poly cel-shaded models with batched shaders far more memory-efficiently than massive 4K uncompressed 2D sprite atlas textures.

### 4.2 Camera & Movement
- **Perspective**: 2.5D Side-View with dynamic framing. Orthographic/low-FOV perspective tracking player on `X` and `Y` axis with bounded clamp volumes.
- **Camera Dynamics**: Smooth damping, micro-shake on heavy impact, slow-motion hit-stop (`Time.timeScale` freeze for 40–80ms on critical parries), zoom punch on Ultimate abilities.
- **Locomotion**:
  - Virtual floating joystick on left screen half (auto-centers to thumb touch).
  - Ground sprint, double-jump, wall-slide, and high-speed directional air-dash (*Vanish Dash*).

### 4.3 Combat System Mechanics
```
[ Attack Chains ] ────────► [ Light x4 Combo ] ──┬──► [ Finisher Knockback ]
       │                                         └──► [ Launcher ] ──► [ Aerial Rave ]
       ▼
[ Defense / Mobility ] ───► [ Dodge Roll / Vanish ] (Invincibility Frames: 200ms)
       │              ───► [ Guard / Parry ] (Timed block deflects projectile & staggers)
       ▼
[ Ki / Aether Meter ] ───► [ Charge Stance ] (Hold button to channel energy)
       │              ───► [ Skill 1: Astral Beams ]
       │              ───► [ Skill 2: Teleport Strike ]
       │              ───► [ Skill 3: Barrier / Nova Burst ]
       │              ───► [ Ultimate: Solar Cataclysm ]
       ▼
[ Surge Mode ] ──────────► [ Aura Awakening ] (Full meter trigger: stat boost, hyper-armor)
```

### 4.4 Mobile UI & HUD Layout (Landscape)
- **Safe Area**: 48px padding from screen edges (notches, camera cutouts, rounded corners).
- **Left Side**:
  - Dynamic Floating Joystick (left bottom quadrant).
  - Character status widget (top left): HP bar, Aether/Ki bar, Surge gauge, active character portrait.
- **Right Side**:
  - Primary Action Cluster (ergonomic arc matching natural thumb reach):
    - Large Primary Attack button (bottom right).
    - Jump button (inner lower).
    - Dash / Vanish button (inner upper).
    - Guard / Charge button (outer upper).
  - Skill Diamond: 3 Skill buttons + 1 Center Ultimate trigger.
- **Top Right**: Pause / Menu button, objective tracker summary.

---

## SECTION 5: TECHNICAL ARCHITECTURE (UNITY)

### 5.1 Core Architecture Principles
- **Target Engine**: Unity 2022.3 LTS or Unity 6 LTS.
- **Rendering Pipeline**: Universal Render Pipeline (URP) with Forward+ or 2D/Forward renderer tailored for mobile fill-rate optimization.
- **Pattern**: Component-based with State Machine for characters (`IState` / Hierarchical State Machine), Event-driven messaging for decoupling UI and audio, and ScriptableObjects for all content definitions.

```
┌─────────────────────────────────────────────────────────────┐
│                       GameManager                           │
│  (Scene Loader, Game State Machine, Persistence, Audio)     │
└──────────────┬───────────────────────────────┬──────────────┘
               │                               │
┌──────────────▼─────────────┐   ┌─────────────▼──────────────┐
│       Input Module         │   │      Save/Data System      │
│  New Input System (Touch,  │   │  Encrypted JSON + Hash     │
│  Gamepad, Keyboard Mock)   │   │  Atomic Dual-File Write    │
└──────────────┬─────────────┘   └─────────────┬──────────────┘
               │                               │
┌──────────────▼─────────────┐   ┌─────────────▼──────────────┐
│     Player Entity          │   │      Level / Combat        │
│  - Movement Controller     │   │  - Enemy Spawner / Waves   │
│  - Combat State Machine    │   │  - Boss AI State Machine   │
│  - Stats & Equipment       │   │  - Hitbox / Hurtbox System │
│  - Animation & VFX Driver  │   │  - Audio Event Emitter     │
└────────────────────────────┘   └────────────────────────────┘
```

### 5.2 Key Subsystems

1. **Input Abstraction**:
   - Unity's *New Input System* package (`com.unity.inputsystem`).
   - Abstraction layer (`IInputService`): generates uniform action vectors whether input is received from on-screen touchscreen canvas, Bluetooth gamepad, or PC keyboard.
2. **Hitbox & Combat Resolution**:
   - Physics2D triggers or customized box overlaps with precise layer masks (`PlayerHurtbox`, `EnemyHurtbox`, `PlayerHitbox`, `EnemyHitbox`, `Projectile`).
   - Damage data packet: `struct DamageContext { int damage; Vector2 knockback; DamageType type; bool causesStagger; float hitStopDuration; }`.
3. **Enemy AI**:
   - Modular State Machine: `PatrolState`, `ChaseState`, `AttackWindupState`, `ComboState`, `StunState`, `RecoveryState`.
   - Boss Phases: Multi-phase behavior trees triggered by HP thresholds (Phase 1: Basic attacks -> Phase 2: Enraged aura + aerial beam bombardment -> Phase 3: Ultimate desperation attack).
4. **Local Save & Anti-Corruption Strategy**:
   - `SaveData` schema serialized to JSON with a schema version header.
   - Dual-file atomic write: write to `save_slot_1.tmp`, verify checksum (SHA-256), rename/replace `save_slot_1.dat`.
   - Backup retention: keep `save_slot_1.bak` from the previous valid session. If primary file fails JSON deserialization or checksum validation, automatically roll back to `.bak` and notify user.
5. **Asset & Memory Management**:
   - Unity Addressables system (`com.unity.addressables`) for stage environments and boss assets to ensure app startup RAM stays below 350 MB on low-end devices (e.g. 2 GB Android phones).

---

## SECTION 6: ART, AUDIO & ASSET PIPELINE

### 6.1 Asset Pipeline Tiers
1. **Tier 1: Whitebox / Prototype (Phase 2)**:
   - Built-in geometric shapes (capsules, cubes), Unity standard sprite placeholders, debug hit-boxes.
   - Rapid iteration on jump velocity, combo timings, cancel windows, and hitbox feel.
2. **Tier 2: Production Stylized 2.5D (Phase 3–5)**:
   - Custom low-to-mid poly 3D character models (6,000–12,000 tris per hero/boss).
   - Cel-shaded flat/toon ramp textures (1024x1024 diffuse + emission for auras).
   - Animation rigs using human-IK or custom skeleton with blend trees.
3. **Tier 3: Final Polish (Phase 6–7)**:
   - Particle VFX using URP Visual Effect Graph or Shuriken Particle System.
   - Custom UI icon sheets, responsive vector fonts (SDF via TextMeshPro).
   - High-fidelity original audio: punchy hit impacts, energy swooshes, voice grunts, and orchestrated synth-rock combat music.

### 6.2 Mobile Technical Budgets
- **Frame Rate Target**: 60 FPS rock-solid on mid-range devices (Snapdragon 778G / Helio G99 / iPhone 11+); 30 FPS fallback on low-end.
- **Draw Calls**: ≤ 75 draw calls per frame (SRP batching + dynamic batching).
- **Triangle Budget**: ≤ 100,000 triangles visible in frustum.
- **VRAM / Texture Budget**: Max 256 MB active texture memory in any combat scene.
- **Audio Budget**: Max 16 concurrent voices; sound effects compressed in Vorbis/ADPCM.

---

## SECTION 7: PRODUCTION ROADMAP & ESTIMATES

| Phase | Objective | Tangible Deliverables | Dependencies | Estimated Effort |
|---|---|---|---|---|
| **Phase 1: Discovery & Pre-Production** *(Current)* | Environment validation, architecture design, IP definition | Technical Architecture, PRD, GDD, Risk Register | Project Charter | **1 Person-Week** |
| **Phase 2: Core Combat Prototype** | Validate game feel, movement, attack timing on PC/Android | Standalone executable/APK with Whitebox hero, dummy AI, combo chain, dodge | Toolchain setup (Unity installed) | **2 Person-Weeks** |
| **Phase 3: Vertical Slice** | Prove full aesthetic & loop in one polished arena | 1 Boss fight, complete hero cel-shaded model, aura burst, sound FX, UI HUD, 60 FPS | Prototype approval, original 3D art/VFX | **4 Person-Weeks** |
| **Phase 4: Systems & Framework** | Complete meta systems for expansion | Multi-slot save/load, settings menu, audio manager, dialogue runner, skill tree | Vertical slice validation | **3 Person-Weeks** |
| **Phase 5: Campaign Production** | Build Chapter 1 & 2 content | 4 Stages, 8 enemy types, 2 major bosses, quest progression, cutscenes | Systems framework | **6 Person-Weeks** |
| **Phase 6: Polish & Content Completion** | Full campaign balancing and juice | Screen shake, haptic feedback, balancing curve, localization hooks | Campaign stages | **3 Person-Weeks** |
| **Phase 7: QA, Compatibility & Optimization** | Device profiling, bug fixing, offline validation | 0 P0/P1 bugs, profiling logs on low-end Android & iOS, stress tests | Complete content | **3 Person-Weeks** |
| **Phase 8: Store Compliance & Release** | Store submission assets & build certification | Signed Android AAB, iOS Xcode archive, privacy policy, store listing | QA sign-off, Store accounts | **2 Person-Weeks** |

*Total Baseline Estimate: ~24 Person-Weeks (assumes dedicated solo/lead engineering and art delivery pipelines).*

---

## SECTION 8: IN-GAME PURCHASES & MONETIZATION ARCHITECTURE (PROPOSED)

### 8.1 Business Model & Monetization Ethics
- **Model**: Free-to-Play (F2P) installation with a complete, self-contained single-player offline campaign.
- **Fair Play Guarantees**:
  - **Zero Paywalls**: 100% of the main campaign story, boss encounters, standard skill trees, and core endings are beatable through player skill and earned progression alone.
  - **No Predatory Mechanics**: Zero pay-to-win stat walls, no energy/stamina timers blocking campaign access, no loot boxes with undisclosed probabilities, and no manipulative countdowns targeting minors.
  - **Offline Integrity**: Real-time network connectivity is required **only** during the active transaction window (store communication, receipt validation). All purchased entitlements (skins, auras, character unlocks, passes) are cached cryptographically in the local save container so the player can play completely offline thereafter.

### 8.2 Proposed In-Game Economy & Currency Structure

```
┌─────────────────────────────────────────────────────────────┐
│                    DUAL-CURRENCY MODEL                      │
├──────────────────────────────┬──────────────────────────────┤
│  SOFT CURRENCY: Aether Shards│ PREMIUM: Astral Crystals     │
│  - Earned via combat, quests,│ - Earned via major milestones│
│    combos, exploration       │ - Purchased via IAP packs    │
│  - Used for standard skill   │ - Used for premium cosmetics,│
│    nodes, gear tuning, craft │   character skins, aura VFX  │
└──────────────────────────────┴──────────────────────────────┘
```

1. **Soft Currency ("Aether Shards")**:
   - Acquired freely through gameplay: defeating enemies, achieving S-rank combo ratings, completing bounties, discovering exploration chests.
   - Spent on: standard character level-ups, martial arts skill tree unlocks, consumable in-run health draughts.
2. **Premium Currency ("Astral Crystals")**:
   - Primary monetization vector. Can also be earned in modest amounts via achievement milestones and saga completions.
   - Spent on: visual customization, unique aura shaders, voice packs, cosmetic character costumes, convenience inventory expansions.
   - Strictly separated in code: `EarnedAstralCrystals` vs. `PurchasedAstralCrystals` for financial audit and compliance tracking.

### 8.3 Configurable Product Catalog (PROPOSED — ALL PRICES & SKUS UNAPPROVED PLACEHOLDERS)

> [!IMPORTANT]
> **No Subscriptions, Live Billing, or Real Store Products are Implemented in this Task.**
> All SKUs and prices below are strictly provisional proposals for architectural planning and UI mock flow validation.

| Product ID | Product Name | Type | Base Quantity | Bonus Crystals | Total Astral Crystals | Proposed Placeholder Price | Purpose & Progression Impact |
|---|---|---|---|---|---|---|---|
| `cur_astral_tier1` | **Pouch of Astral Crystals** | Consumable | 100 | 0 | 100 | *$0.99 USD (Unapproved)* | Micro-transaction tier; optional small cosmetic spend. |
| `cur_astral_tier2` | **Sack of Astral Crystals** | Consumable | 500 | 50 (+10%) | 550 | *$4.99 USD (Unapproved)* | Entry cosmetic tier; unlocks a costume or weapon aura. |
| `cur_astral_tier3` | **Chest of Astral Crystals** | Consumable | 1,000 | 150 (+15%) | 1,150 | *$9.99 USD (Unapproved)* | Mid tier; multiple cosmetic unlocks. |
| `cur_astral_tier4` | **Vault of Astral Crystals** | Consumable | 2,000 | 400 (+20%) | 2,400 | *$19.99 USD (Unapproved)* | Collector tier; high cosmetic value bundle. |
| `pack_starter_disciple` | **Disciple Starter Bundle** | Non-Consumable | N/A (1-Time) | 100 | 100 (+Solar Gi Outfit) | *$1.99 USD (Unapproved)* | One-time early supporter pack; cosmetic gi + 5,000 Aether Shards. |

*Note: Subscriptions are explicitly excluded from the current roadmap to avoid recurring billing friction, complex compliance, and mandatory account systems.*

### 8.4 Technical Architecture for In-Game Purchases & Inventory

#### A. Architecture Overview & Decoupled Interfaces
The economy and billing systems are strictly partitioned into two separate interfaces: `ICurrencyInventoryService` (gameplay progression & wallet) and `IPurchaseService` (store transaction processing):

```csharp
public interface ICurrencyInventoryService
{
    int AetherShards { get; }          // Soft currency (earned only)
    int AstralCrystals { get; }        // Premium currency (earned + purchased)
    bool HasPurchasedPackage(string packageId);
    
    void AddAetherShards(int amount, string sourceReason);
    bool SpendAetherShards(int amount, string sinkReason);
    
    void CreditAstralCrystals(int amount, string transactionId, bool isPurchased);
    bool SpendAstralCrystals(int amount, string sinkReason);
}

public interface IPurchaseService
{
    void Initialize(List<ProductCatalogItem> catalog, Action<bool> onInitialized);
    void PurchaseProduct(string productId, Action<PurchaseTransactionResult> onComplete);
    void RestorePurchases(Action<RestoreResult> onComplete);
}
```

#### B. Mock Shop Flow & State Handling (Development Phase)
During prototype and early development, `MockPurchaseService` is used exclusively. It simulates the exact store states without network calls or credit cards:
- **`Initiated`**: User clicks purchase; UI disables inputs and shows spinner.
- **`Success` (90% mock branch / explicit debug button)**: Simulates receipt issuance, generates a synthetic `TransactionID`, credits `ICurrencyInventoryService`, and triggers save.
- **`Cancelled`**: User closes native purchase sheet without buying; UI returns to normal state with zero balance change.
- **`Pending / Deferred`**: Simulates parental controls (Apple Ask to Buy / Google Play Pending); UI shows "Waiting for approval" and polls or listens for resolution.
- **`Failed`**: Simulates network timeout or payment decline; displays a friendly, non-crashing localized error modal.

#### C. Idempotency & Duplicate Prevention
- Every platform transaction delivers an immutable `TransactionID` (Google Order ID / Apple Transaction Identifier).
- Before granting any resources, the transaction ID is checked against `SaveData.ProcessedTransactionHistory`.
- If the ID is already present, the transaction is marked as acknowledged (`ConfirmPendingPurchase`) and silently dismissed, guaranteeing zero duplicate currency grants even if the store re-delivers the receipt upon app launch.

#### D. Platform Purchase Verification, Proof of Payment & Offline Behavior
- **Security Standard**: **We never rely on a hardcoded client-side secret or local HMAC alone as legitimate proof of payment.**
- **Verification Plan**:
  1. *Online Validation Window*: When connected, receipts are validated against Google Play Developer API or Apple App Store Server API (via lightweight serverless endpoint or obfuscated native cross-check).
  2. *Offline Gameplay*: **Zero daily connectivity is required for the single-player campaign.** Once purchased crystals or unlocks are credited, they exist in the player's local save.
  3. *Local Tamper Deterrent*: The local save file uses atomic dual-file writes and an integrity checksum to prevent casual hex-editing of save files on rooted/jailbroken devices.
  4. *Reconnection Reconciliation*: When reconnecting online, the client queries any pending unacknowledged transactions and synchronizes safely with the store server.

#### E. Refund & Revocation Handling
- If a user requests and receives a platform refund (via Google Play Voided Purchases API or Apple App Store Server Notifications):
  - On next verified connection, the server/client reconciliation identifies the revoked `TransactionID`.
  - The granted `AstralCrystals` are deducted. If crystals were already spent, the balance adjusts gracefully (or locks the cosmetic item associated with the purchase) rather than corrupting the save file or soft-locking the player's campaign progression.


---

## SECTION 9: REVISED PRODUCTION ROADMAP & EFFORT ESTIMATES

| Phase | Objective | Tangible Deliverables | Dependencies | Estimated Effort |
|---|---|---|---|---|
| **Phase 1: Discovery & Technical Planning** *(Current)* | Environment validation, architecture design, PRD, IAP design | Complete Charter & Planning Documents, `.gitignore` | Project Charter | **1 Person-Week** |
| **Phase 2: Core Combat Prototype** | Validate game feel, movement, attack timing on PC/Android | Standalone executable/APK with Whitebox hero, dummy AI, combo chain, dodge | Toolchain setup (Unity installed) | **2 Person-Weeks** |
| **Phase 3: Vertical Slice** | Prove full aesthetic & loop in one polished arena | 1 Boss fight, complete hero cel-shaded model, aura burst, sound FX, UI HUD, 60 FPS | Prototype approval, original 3D art/VFX | **4 Person-Weeks** |
| **Phase 4: Systems Framework & Mock IAP** | Complete meta systems, save/load, and modular IAP abstraction | Multi-slot save/load, settings menu, audio manager, dialogue runner, `IPurchaseService` with `MockPurchaseService` | Vertical slice validation | **4 Person-Weeks** *(+1 Wk for IAP architecture)* |
| **Phase 5: Campaign Production** | Build Chapter 1 & 2 content | 4 Stages, 8 enemy types, 2 major bosses, quest progression, cutscenes | Systems framework | **6 Person-Weeks** |
| **Phase 6: Polish & Content Completion** | Full campaign balancing, cosmetic shop UI, juice | Screen shake, haptic feedback, balancing curve, in-game cosmetic preview shop | Campaign stages | **3 Person-Weeks** |
| **Phase 7: QA, Optimization & Store Sandbox Testing** | Device profiling, Google/Apple Sandbox IAP testing, offline validation | 0 P0/P1 bugs, successful Sandbox purchases & restorations on test devices, memory profiling | Complete content | **4 Person-Weeks** *(+1 Wk for IAP Sandbox)* |
| **Phase 8: Store Compliance, Billing & Release** | Live store billing certification, metadata, signed builds | Signed Android AAB, iOS Xcode archive, Google Play Data Safety, Apple Privacy Labels, store listing | QA sign-off, Store accounts, Banking setup | **2 Person-Weeks** |

*Revised Baseline Estimate: **~26 Person-Weeks** (reflects addition of modular billing architecture, sandbox testing, and store financial certification).*

---

## SECTION 10: RELEASE READINESS & MONETIZATION PREREQUISITES

### 10.1 Android (Google Play) Billing & Release
- **Google Play Developer Account**: Active organization or individual developer registration.
- **Merchant Account Setup**: Completed Payments Profile with bank verification, tax forms (W-8BEN / local tax registration), and currency payout settings.
- **Google Play Billing Setup**: Product IDs created in Play Console matching in-game constants (`cur_astral_tier1`, etc.) with configured tax categories and localized pricing.
- **Google License Testers**: Whitelist of developer and QA Google accounts enabled for zero-cost sandbox testing.
- **Data Safety Declaration**: Disclose billing transaction handling and financial data processing requirements.

### 10.2 iOS (Apple App Store) In-App Purchase & Release
- **Apple Developer Program**: Active team enrollment ($99/year).
- **Paid Applications Agreement**: Signed in App Store Connect with valid banking, routing, and tax certificates submitted.
- **App Store In-App Purchases Configuration**: Non-consumables and consumables registered in App Store Connect with localized screenshots, descriptions, and pricing tiers.
- **Sandbox Testing**: TestFlight builds linked to Apple Sandbox Tester accounts.
- **App Privacy Details**: Complete Apple's App Privacy nutrition labels declaring purchase history collection for fulfillment.

---

## SECTION 11: REVISED RISK REGISTER

| Risk ID | Category | Risk Description | Severity | Likelihood | Mitigation Strategy | Owner |
|---|---|---|---|---|---|---|
| **R-01** | Legal / IP | Using trademarked "Dragon Ball" assets/names causes immediate store ban and legal claims. | **CRITICAL** | HIGH | Commit strictly to the original "Aether Surge" IP. Zero copyrighted assets. | Product Owner / Lead Dev |
| **R-02** | Hardware / Space | Drive C: has only 7.76 GB free; installing toolchains will exhaust system disk. | **HIGH** | HIGH | Mandate all Unity, Android SDK, and tool installations to Drive D: (48 GB free). | Lead Dev |
| **R-03** | Engine Tooling | Unity Editor and Android SDK are not installed on host machine. | **HIGH** | CERTAIN | Install Unity Hub & Unity 2022.3 LTS with Android Build Support on Drive D:. | Lead Dev / PM |
| **R-04** | iOS Toolchain | Host machine is Windows; native iOS compilation cannot be performed locally. | **HIGH** | HIGH | Setup cloud build pipeline (GitHub Actions macOS / Unity Cloud Build) or dedicated Mac runner. | PM / Lead Dev |
| **R-05** | Save Corruption | Sudden mobile battery depletion or OS process kill during save write destroys progress. | **HIGH** | MEDIUM | Dual-file atomic write (.tmp -> checksum -> .dat) with rollback to previous session (.bak). | Lead Dev |
| **R-06** | Combat Feel | Touchscreen controls feel sluggish, inaccurate, or lack tactile impact. | **HIGH** | MEDIUM | Early Phase 2 mobile playtesting with customizable touch button bounds and subtle haptic feedback. | Lead Dev |
| **R-07** | Performance | High particle counts and cel-shader passes drop frame rate below 30 FPS on low-end phones. | **MEDIUM** | HIGH | Strict draw-call budget (≤75), texture atlasing, low-poly geometry, quality scaling options in settings. | Lead Dev |
| **R-08** | Monetization / Fraud | Modified APKs or fake receipt injection spoofing in-app purchases. | **HIGH** | MEDIUM | Cryptographic receipt validation (embedded public key cross-check + optional serverless verify); atomic entitlement hashing. | Lead Dev |
| **R-09** | Store Rejection | App rejected by Apple/Google review for non-compliant IAP, missing restore button, or subscription terms. | **HIGH** | MEDIUM | Enforce standard "Restore Purchases" button, clear pricing disclosures, two-step purchase confirmation modal. | Lead Dev / PM |
| **R-10** | Offline Sync Desync | User purchases on one device, plays offline on another, causing currency or cosmetic discrepancies. | **MEDIUM** | MEDIUM | Clear UI disclaimer that offline saves remain local; provide manual "Re-sync Purchases" button when reconnected. | Lead Dev |

