# WHITEBOX COMBAT PROTOTYPE SPECIFICATION
**Project**: Aether Surge (Mobile Offline Action RPG)  
**Phase**: Phase 2 — Core Combat Whitebox Prototype  
**Status**: SPECIFICATION & TEST PLAN (PROPOSED)  
**Target Engine**: Unity 2022.3 LTS (URP)  

---

## 1. SCOPE & OBJECTIVE
The whitebox combat prototype is an isolated, testable development slice designed to validate:
- Tight, responsive 2.5D character locomotion and physics.
- Input abstraction and input buffering (150ms buffer window).
- 4-hit light combo attack chain with fluid branch windows.
- Invulnerability-frame (i-frame) dodge roll and air-dash mechanics.
- Energy charging stance and projectile skill release.
- Combat dummy hit reaction, knockback, floating combat text, and hit-stop (frame freeze).
- Safe `Time.timeScale` management ensuring hit-stop recovers cleanly during pause, reset, or interruption.
- Architecture decoupled from billing, online servers, or heavy production art.

---

## 2. INPUT ABSTRACTION ARCHITECTURE

### 2.1 Uniform Input Interface
Input is abstracted via `IInputService` using Unity's New Input System (`com.unity.inputsystem`). The gameplay controller never checks `Input.GetKey` or raw hardware directly.

```csharp
public interface IInputService
{
    Vector2 MoveVector { get; }
    bool JumpTriggered { get; }
    bool DashTriggered { get; }
    bool AttackTriggered { get; }
    bool SkillTriggered { get; }
    bool ChargeHeld { get; }
    bool PauseTriggered { get; }
}
```

### 2.2 Input Mapping & Bindings

| Action | Keyboard / Mouse (Prototype) | Gamepad (Standard XInput) | Mobile Touchscreen (Future) |
|---|---|---|---|
| **Move Left / Right** | `A` / `D` or Left / Right Arrows | Left Analog Stick / D-Pad | Floating Virtual Joystick (Left) |
| **Jump** | `Space` / `W` | Button South (`A` / Cross) | Virtual Jump Button (Right lower) |
| **Dodge / Dash** | `Left Shift` / `K` | Button East (`B` / Circle) | Virtual Dash Button (Right mid) |
| **Light Attack** | `J` / Left Mouse Button | Button West (`X` / Square) | Primary Attack Button (Right large) |
| **Energy Charge** | `C` (Hold) | Left Bumper (`LB` / `L1`) | Virtual Charge Button (Hold) |
| **Energy Skill** | `U` / Right Mouse Button | Button North (`Y` / Triangle) | Skill Button 1 |
| **Pause / Reset** | `Escape` / `P` | `Start` / Options | Pause Icon (Top right) |

### 2.3 Input Buffering & Queue System
- **Buffer Duration**: 150 milliseconds.
- **Mechanism**: When an action button (Attack, Dodge, Jump, Skill) is pressed during an un-cancellable animation recovery or hit-reaction frame, the request is stored in `InputBuffer`.
- **Consumption**: The moment the active state enters its `CancelWindow` or returns to `Idle/Move`, the queued action executes immediately, eliminating "dropped inputs" common in mobile and fast-paced action games.

---

## 3. COMBAT STATE MACHINE & LOCOMOTION

### 3.1 Hierarchical State Machine (HSM)
The player controller operates on an explicit state machine:

```
                  ┌───────────────┐
                  │   Idle/Walk   │◄──────────────┐
                  └───┬───────▲───┘               │
          Move/Jump   │       │ Recovery          │
          ┌───────────┘       └───────────┐       │
          ▼                               │       │
    ┌───────────┐                   ┌─────┴───────┴┐
    │ Air/Jump  │                   │  Dodge/Roll  │ (i-frames 0-200ms)
    └─────┬─────┘                   └──────────────┘
          │ Attack                        ▲
          ▼                               │ Dodge trigger
    ┌───────────┐                         │
    │ Attack 1  ├──Combo Chain─► Attack 2 ├──Combo Chain─► Attack 3 ──► Attack 4 (Knockback)
    └─────┬─────┘                │        │
          │ Charge Hold          └────────┘
          ▼
    ┌───────────┐
    │  Charging │ (Continuous Energy regen + visual aura pulse)
    └───────────┘
```

### 3.2 State Details & Mechanics

1. **Locomotion**:
   - Ground movement speed: 7.5 m/s. Smooth acceleration and deceleration curves.
   - Jump: Variable height jump (tap for hop, hold for maximum height 3.8m).
   - Air-Dash: 1 horizontal air-dash permitted per airborne cycle before touching ground.
2. **Dodge Roll (I-Frames)**:
   - Duration: 400ms total.
   - **Invulnerability Window**: First 200ms (frames 0–12 at 60 FPS).
   - During i-frames, player's hurtbox layer mask ignores all `EnemyHitbox` overlap queries.
   - Has a 250ms cooldown before another dodge can be initiated.
3. **Four-Hit Light Attack Chain**:
   - **Attack 1 (Jab)**: 120ms startup, 60ms active hitbox, 150ms recovery. Deals 20 damage, minor flinch.
   - **Attack 2 (Cross)**: 100ms startup, 60ms active hitbox, 160ms recovery. Deals 25 damage, minor flinch.
   - **Attack 3 (High Kick / Sweep)**: 140ms startup, 80ms active hitbox, 200ms recovery. Deals 35 damage, pulls target forward.
   - **Attack 4 (Aura Fist Finisher)**: 220ms startup, 100ms active hitbox, 350ms recovery. Deals 60 damage, triggers heavy knockback (`Vector2(8f, 3.5f)`) and ground dust burst.
4. **Energy Charging**:
   - Hold Charge button: Player enters charging stance, anchoring velocity to zero.
   - Energy regeneration: 25 Aether Energy / second (Max pool: 100).
   - Emits a growing aura scaling effect and ground shockwave particle.
5. **Placeholder Energy Skill (*Astral Beam*)**:
   - Consumes 30 Aether Energy.
   - Fires a fast linear projectile (`speed = 22 m/s`, damage = 55).
   - Penetrates or explodes on dummy contact with area-of-effect stagger.

---

## 4. COMBAT DUMMY & FEEDBACK MECHANICS

### 4.1 Training Dummy Specification
- **Health**: 500 HP (with auto-regeneration toggle after 3s of no damage).
- **Hurtbox & Collision**: Dynamic 2D capsule hurtbox responding to physical impulse.
- **States**: `Idle`, `HitReaction_Light`, `HitReaction_Heavy`, `AirborneJuggle`, `Knockdown`, `GetUp`.
- **Feedback**:
  - Material flash: Shader swaps to solid white for 60ms upon taking damage.
  - Floating Combat Text (FCT): Displays damage numbers with dynamic pop-up scale and color coding (White = Normal, Yellow = Combo finisher/Skill).
  - Pushback / Knockback: Velocity impulse scaled by attack type.

### 4.2 Safe Hit-Stop & Time-Scale Controller
Hit-stop freezes game time for 40ms–80ms upon solid impact to create visceral weight.
- **Failure Mode in Traditional Implementations**: Calling `Time.timeScale = 0` via coroutine can leave the game permanently frozen if the scene reloads, the game pauses, or the attacking entity is destroyed during the freeze.
- **Safe Hit-Stop Implementation**:
  ```csharp
  public class HitStopManager : MonoBehaviour
  {
      private static HitStopManager _instance;
      private Coroutine _activeHitStopCoroutine;

      public void TriggerHitStop(float durationRealtime, float frozenScale = 0.05f)
      {
          if (_activeHitStopCoroutine != null)
          {
              StopCoroutine(_activeHitStopCoroutine);
              Time.timeScale = 1.0f; // Ensure clean baseline
          }
          _activeHitStopCoroutine = StartCoroutine(DoHitStop(durationRealtime, frozenScale));
      }

      private IEnumerator DoHitStop(float durationRealtime, float frozenScale)
      {
          Time.timeScale = frozenScale;
          yield return new WaitForSecondsRealtime(durationRealtime);
          Time.timeScale = 1.0f;
          _activeHitStopCoroutine = null;
      }

      private void OnDisable()
      {
          Time.timeScale = 1.0f; // Failsafe on scene transition or object disable
      }
  }
  ```

---

## 5. REPEATABLE TEST PLAN & ACCEPTANCE TESTS

### 5.1 Automated Unit & Integration Tests (Unity Test Framework)

| Test ID | Target System | Assertion / Pass Condition |
|---|---|---|
| `TEST-01-MOVE` | `PlayerLocomotion` | Applying +1.0 horizontal input moves player position on X-axis within expected speed bounds. |
| `TEST-02-JUMP` | `PlayerLocomotion` | Jump input applies upward vertical velocity; vertical velocity clamps at peak and returns to ground. |
| `TEST-03-COMBO-WINDOW` | `CombatStateMachine` | Consecutive attack inputs within branch window transition sequentially: Attack 1 -> 2 -> 3 -> 4. |
| `TEST-04-COMBO-TIMEOUT`| `CombatStateMachine` | Waiting >250ms after Attack 1 without input resets state machine to Idle; next attack starts at Attack 1. |
| `TEST-05-IFRAMES` | `HitboxResolver` | Attack collision occurring during first 200ms of DodgeRoll records 0 damage to Player. |
| `TEST-06-HITSTOP-RESET`| `HitStopManager` | Force-disabling or resetting `HitStopManager` mid-freeze guarantees `Time.timeScale` returns strictly to `1.0f`. |
| `TEST-07-ENERGY-CHARGE`| `EnergyController` | Holding charge increases Energy by 25 units/sec up to max 100; releasing halts regeneration. |
| `TEST-08-SKILL-COST` | `EnergyController` | Attempting to cast Astral Beam with <30 Energy fails; casting with ≥30 Energy deducts 30 and spawns projectile. |

### 5.2 Manual Repeatable Test Script

1. **Locomotion & Jumping**:
   - Press `A`/`D`: Confirm smooth left/right movement with instant directional flipping.
   - Tap `Space`: Confirm short hop; Hold `Space`: Confirm full 3.8m jump height.
   - In air, press `Shift`: Verify horizontal air-dash executes once and cannot be re-triggered until landing.
2. **Combo Chaining**:
   - Press `J` rhythmically 4 times: Observe Attack 1 (jab), Attack 2 (cross), Attack 3 (kick), Attack 4 (slam).
   - Check that dummy flashes white on each hit and is launched backward across the arena on Attack 4.
3. **Dodge & I-Frames**:
   - Configure dummy to emit a repeating stationary hazard projectile.
   - Roll through the projectile: Verify zero damage during the first half of the roll.
   - Roll late or early: Verify damage is registered when vulnerable.
4. **Hit-Stop & Interruption Stress Test**:
   - Attack the dummy to trigger hit-stop; press `Escape` (Pause) immediately during the freeze.
   - Resume game: Verify `Time.timeScale` is exactly `1.0f` and game does not remain slow-motion.
