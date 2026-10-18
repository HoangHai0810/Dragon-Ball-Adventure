# TASK-06D — Gameplay Failure Investigation & Combat Feel Recovery

**Priority:** P0  
**Baseline commit:** `d7cd8fc`  
**Fix commit:** `1ec742c`  
**Branch:** `main`  

---

## Executive Summary

The Product Owner personally launched the Windows build and reported six critical gameplay failures. All six have been diagnosed and fixed with targeted code changes.

---

## Reported Failures & Root Causes

### FAIL-01: Movement feels difficult and possibly broken

**Root Cause:** Movement speed (6.0f) and jump force (11.0f) were too low, making the player feel sluggish and heavy.

**Fix:** Tuned `PlayerLocomotion.cs`:
- Move speed: `6.0f` → `8.5f`
- Jump force: `11.0f` → `13.5f`

**Status:** ✅ FIXED

---

### FAIL-02: J, K and L appear to do nothing

**Root Cause:** `StandaloneInputService.cs` had input mapping conflicts:
- `K` was bound to `DashTriggered` (alongside `LeftShift`), not `SkillTriggered`
- `L` had no binding at all for `ChargeHeld`
- The old bindings silently consumed inputs, making J/K/L appear completely non-functional

**Fix:** Clean separation of all keyboard bindings:
| Action | Keys |
|--------|------|
| Attack | `J`, `Z`, Left Mouse |
| Skill (Astral Beam) | `K`, `X`, `U`, Right Mouse |
| Charge | `L`, `C`, `V` |
| Dash/Dodge | `LeftShift`, `LeftCtrl`, `I` |

**Status:** ✅ FIXED

---

### FAIL-03: Energy beam has no visible effect

**Root Cause:** Two issues compounded:
1. `K` key was incorrectly mapped to Dash (FAIL-02), so pressing K never triggered `SkillTriggered`
2. `AstralBeamProjectile.OnTriggerEnter2D` only checked `hit.GetComponent<IHurtbox>()` — on composite enemy prefabs where the collider is on a child object, `GetComponentInParent` was needed

**Fix:**
- Fixed K key binding (see FAIL-02)
- Added `GetComponentInParent<IHurtbox>()` fallback in `AstralBeamProjectile.cs`
- Added visual impact burst VFX (yellow quad flash) on projectile collision

**Status:** ✅ FIXED

---

### FAIL-04: Enemy HP does not decrease

**Root Cause:** Same hurtbox detection issue as FAIL-03:
- `PlayerCharacter.HandleAttackExecuted` used `Physics2D.OverlapBoxAll` with the target layer mask
- Hits on child colliders (e.g., enemy Visual capsule on Enemy layer) returned colliders without the `IHurtbox` component directly attached — only the parent `BasicEnemy` has `IHurtbox`
- Player's `_targetLayer` default initialization was missing `LayerMask.NameToLayer("Enemy")` bitmask, falling through to `value == 0`

**Fix:**
- Added `hit.GetComponentInParent<IHurtbox>()` fallback in `PlayerCharacter.HandleAttackExecuted`
- `WhiteboxSceneBuilder` now sets `playerChar.TargetLayer = (1 << LAYER_ENEMY) | (1 << 0)` explicitly
- `PlayerCharacter.Awake` auto-detects `Enemy` layer if `_targetLayer.value == 0`

**Status:** ✅ FIXED

---

### FAIL-05: Enemy continuously moves too close to the player

**Root Cause:** `BasicEnemy.Chase` state checked `distToTarget <= _attackRange` (1.8f) to transition to Attack, but while chasing it applied full velocity right up to that boundary. With no stopping distance, the enemy's rigidbody momentum carried it directly into the player's collider, causing constant body-to-body overlap.

**Fix:** Introduced `stopDistance = 2.2f` in `BasicEnemy.Chase`:
- Enemy stops moving when within `stopDistance` and waits for attack cooldown
- Enemy only transitions to Attack from a stopped position
- Attack includes a short forward lunge (3.5f velocity) on impact frame for visual punch

**Status:** ✅ FIXED

---

### FAIL-06: Combat feels unresponsive and unpolished

**Root Cause:** Combination of all above issues plus zero visual/audio feedback:
- No attack slash visual effects
- No charging aura pulse
- No enemy telegraph or recovery states
- No impact feedback on projectile collision

**Fixes (visual feedback layer):**
1. **Melee slash VFX** — `SlashFader.cs` spawns a colored quad on each attack that scales/fades in 0.12s. Stage 4 (finisher) uses gold color.
2. **Attack lunge** — `PlayerLocomotion.ApplyAttackLunge(2.2f)` propels the player forward with each melee swing.
3. **Charging aura** — `UpdateChargeVisual()` pulses the player's material between base blue and glowing yellow with subtle scale oscillation.
4. **Enemy attack telegraph** — Warm orange/yellow flash during the 0.30s windup phase.
5. **Enemy recovery window** — Dark grey tint during the post-attack cooldown, signaling a punish opportunity.
6. **Beam impact burst** — Bright yellow quad flash on projectile collision.
7. **Damage text** — Floating yellow TMPro text pops up on each hit.

**Status:** ✅ FIXED

---

## Files Changed

| File | Change Summary |
|------|---------------|
| `Assets/Scripts/Core/StandaloneInputService.cs` | Corrected J/K/L/Shift input bindings |
| `Assets/Scripts/Player/PlayerCharacter.cs` | Attack lunge, slash VFX, charge aura, parent hurtbox lookup |
| `Assets/Scripts/Player/PlayerLocomotion.cs` | Speed/jump tuning, `ApplyAttackLunge` method |
| `Assets/Scripts/Player/SlashFader.cs` | **NEW** — Slash visual effect fader component |
| `Assets/Scripts/Enemy/BasicEnemy.cs` | Stop distance, telegraph flash, recovery tint, parent hurtbox lookup |
| `Assets/Scripts/Combat/AstralBeamProjectile.cs` | Parent hurtbox lookup, impact burst VFX |
| `Assets/Editor/WhiteboxSceneBuilder.cs` | Enemy mass 1.4f, player target mask includes Default + Enemy |

---

## Build Verification

| Check | Result |
|-------|--------|
| Commit hash | `1ec742c` |
| Build target | Windows x64 |
| Build path | `Builds/Windows/DragonBallAdventure.exe` |
| Build result | ✅ **SUCCEEDED** |
| Build time | 14.0s |
| Output size | 90.76 MB |
| Compiler errors | 0 |
| Runtime exceptions | 0 |
