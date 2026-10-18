# DRAGON BALL ADVENTURE — Windows Playtest Guide

**Version:** Prototype Playable Slice (Build v1.0.0-whitebox)  
**Target Platform:** Windows x64 Standalone  
**Executable Path:** `D:\Dragon-Ball-Adventure\Builds\Windows\DragonBallAdventure.exe`  
**Engine:** Unity 6 LTS `6000.0.85f1`  
**Date:** 2026-10-18  

---

## 1. Quick Start

1. Open Windows File Explorer and navigate to:  
   `D:\Dragon-Ball-Adventure\Builds\Windows`
2. Double-click **`DragonBallAdventure.exe`**.
3. The game window will launch in 1280x720 windowed mode.

---

## 2. Keyboard & Mouse Controls

| Action | Primary Key | Alternative Key | Notes |
|---|---|---|---|
| **Move Left / Right** | `A` / `D` | `Left` / `Right` Arrow | Responsive acceleration with instant turnaround |
| **Jump** | `Space` | `W` / `Up` Arrow | Variable jump height with coyote time |
| **Dodge / Air Dash** | `Left Shift` | `C` | Invulnerability window; limit 1 air-dash per jump |
| **Normal Attack (Combo)** | `J` | `Z` | 4-hit melee combo chain with input buffering |
| **Ki Skill (Beam Blast)** | `K` | `X` | Fires projectile (costs 40 Ki) |
| **Charge Ki (Energy)** | `L` (Hold) | `V` (Hold) | Stand still to gather Ki (25 Ki/sec) |
| **Restart Match** | `R` | Click UI Button | Resets Player, Enemy, HP, and Match State instantly |
| **Virtual On-Screen UI** | Left Mouse Click | Touch Screen | Click virtual joystick and touch action buttons directly |

---

## 3. What to Test in this Prototype

1. **Locomotion & Jumping:** Move left and right, jump over platforms, test falling off edges and landing smoothly.
2. **Air Dash:** Jump into the air and press `Left Shift` to perform an air-dash. Confirm you can only dash once before touching ground again.
3. **4-Hit Combo:** Press `J` repeatedly to execute the 4-hit attack chain against the Training Dummy (yellow) or the Enemy (red).
4. **Ki Charging & Beam Skill:** Hold `L` to fill your energy meter to 100. Press `K` to unleash the energy beam projectile.
5. **Enemy AI & Combat:** Approach the Enemy (red). Observe it detect you, chase, perform melee attacks, and pause for cooldown.
6. **Victory / Defeat Conditions:**
   - Deplete Enemy HP (150) to trigger **"VICTORY!"**.
   - Let Enemy deplete Player HP (100) to trigger **"DEFEAT!"**.
   - Confirm player combat controls are locked once the round ends.
7. **Instant Restart:** Press `R` or click "RESTART MATCH (R)" to instantly reset the arena for repeated play without restarting the `.exe`.

---

## 4. Current Limitations (Whitebox Stage)

- **Visuals:** Pure geometric whitebox (Cyan = Player, Red = Enemy, Yellow = Dummy). 2D anime sprite animations for Goku and enemies are planned for upcoming milestones.
- **Audio:** Completely silent; sound effects and music are pending integration.
- **Story & Dialogue:** Standalone combat arena testing; Mount Paozu narrative progression is detailed in `docs/TASK_06B_PRODUCT_DIRECTION_AND_PLAYTEST.md`.
