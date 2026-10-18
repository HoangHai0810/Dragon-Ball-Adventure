# Dragon Ball Adventure — Playtest README

## Quick Start

1. **Launch:** Double-click `Builds\Windows\DragonBallAdventure.exe`
2. **Window:** Starts in 1280×720 windowed mode

---

## Controls

### Keyboard

| Action | Keys |
|--------|------|
| Move | `A`/`D` or `←`/`→` |
| Jump | `Space` or `W` |
| Attack (4-hit combo) | `J` or `Z` or Left Mouse |
| Skill (Astral Beam) | `K` or `X` or `U` or Right Mouse |
| Energy Charge | `L` or `C` or `V` (hold) |
| Dodge (ground) / Air-Dash | `Left Shift` or `Left Ctrl` or `I` |
| Restart Match | `R` |
| Pause | `Escape` or `P` |

### Gamepad

| Action | Button |
|--------|--------|
| Move | Left Stick |
| Jump | South (A/X) |
| Attack | West (X/□) |
| Skill | North (Y/△) |
| Charge | Left Shoulder / Trigger |
| Dodge | East (B/○) |
| Pause | Start |

### Mobile Touch (iOS/Android)

Virtual joystick (left) + action buttons (right). Buttons auto-hide on desktop.

---

## What to Test

### Movement
- [x] Walk left/right — smooth and responsive at 8.5 speed
- [x] Jump — crisp, 13.5 force with variable height (release Space early for short hop)
- [x] Coyote time — brief grace period when walking off edges
- [x] Air dash — one mid-air dash per airborne cycle (resets on landing)
- [x] Ground dodge — invulnerable roll with cooldown

### Combat
- [x] **4-hit combo** — J/Z for rapid melee chain, each hit produces a blue slash VFX (gold on hit 4)
- [x] **Attack lunge** — player slides forward with each swing
- [x] **Astral Beam** — K/X fires a cyan energy projectile that damages the enemy
- [x] **Energy charge** — Hold L/C/V while grounded, player glows yellow and pulses
- [x] **Damage numbers** — floating yellow text on each hit

### Enemy Behavior
- [x] Enemy patrols when player is far away
- [x] Enemy chases when player enters detection range (10 units)
- [x] Enemy stops at safe distance (2.2 units) — no more crowding
- [x] **Attack telegraph** — orange/yellow flash during windup (0.3s)
- [x] **Attack impact** — enemy lunges forward to strike
- [x] **Recovery window** — enemy turns grey after attacking (punish opportunity!)
- [x] Enemy takes knockback from player attacks
- [x] Enemy HP decreases and is shown on HUD

### Match Flow
- [x] Defeat enemy → Victory banner appears
- [x] Player defeated → Defeat banner appears
- [x] Press R → full match restart (HP, positions, energy all reset)

---

## Known Limitations

- All characters are placeholder capsules (no sprites/animations yet)
- Sound effects not implemented
- Single arena only
- No save/load or progression

---

## Build Info

| Field | Value |
|-------|-------|
| Commit | `1ec742c` |
| Engine | Unity 6 LTS `6000.0.85f1` |
| Platform | Windows x64 |
| Resolution | 1280×720 (windowed) |
