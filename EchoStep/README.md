# ECHO STEP — Unity 2D Project

> "You cannot move forward until you learn to use your past selves as tools."

## Quick Start

1. **Create a new Unity 2D project** (Unity 2021.3+ recommended)
2. Copy the `Assets/` folder contents into your project's `Assets/`
3. Create scenes for each level (see Level Design below)
4. Set up prefabs as described in Prefab Setup
5. Hit Play!

## Project Structure

```
Assets/
├── Scripts/
│   ├── PlayerController.cs      — Hold-to-charge, release-to-jump
│   ├── EchoManager.cs           — Spawns & tracks echoes (max 5)
│   ├── Echo.cs                  — Echo behavior: replay, platform, boost, shockwave
│   ├── ShockwaveEffect.cs       — Upward launch when two echoes collide
│   ├── ResonanceCrystal.cs      — Pickup: +1 echo charge
│   ├── PressurePlate.cs         — Activated by player or echo landing
│   ├── TimedPressurePlate.cs    — Must be hit within time window
│   ├── MovingPlatform.cs        — Moves when activated
│   ├── Hazard.cs                — Kill zone / moving hazard
│   ├── LevelGoal.cs             — End-of-level trigger
│   ├── EncoreRating.cs          — D-to-S rating calculator
│   ├── EchoChargeUI.cs          — HUD: 5 dots showing echo charges
│   ├── ChargeBarUI.cs           — Visual jump charge indicator
│   ├── GameManager.cs           — Level flow, restart, transitions
│   └── CameraFollow.cs          — Smooth follow camera
├── Prefabs/                     — (create from scene objects)
├── Scenes/                      — One scene per level
└── UI/                          — UI canvases
```

## Prefab Setup

### Player
- Sprite (small circle/character)
- `Rigidbody2D` (Dynamic, gravity scale 3, freeze rotation Z)
- `CircleCollider2D`
- `PlayerController.cs`
- Tag: **Player**
- Layer: **Player**

### Echo (Prefab)
- Sprite (same shape as player, semi-transparent)
- `Rigidbody2D` (Kinematic)
- `BoxCollider2D` (slightly wider than player — acts as platform)
- `Echo.cs`
- Tag: **Echo**
- Layer: **Echo**

### Shockwave (Prefab)
- Particle effect or expanding circle sprite
- `ShockwaveEffect.cs`
- `CircleCollider2D` (Trigger)
- Auto-destroys after 0.5s

### Resonance Crystal
- Glowing sprite
- `CircleCollider2D` (Trigger)
- `ResonanceCrystal.cs`

### Pressure Plate
- Flat sprite
- `BoxCollider2D` (Trigger)
- `PressurePlate.cs` or `TimedPressurePlate.cs`

### Moving Platform
- Sprite
- `Rigidbody2D` (Kinematic)
- `BoxCollider2D`
- `MovingPlatform.cs`

### Hazard
- Sprite (spikes/energy)
- `BoxCollider2D` (Trigger)
- `Hazard.cs`

### Level Goal
- Sprite (portal/door)
- `BoxCollider2D` (Trigger)
- `LevelGoal.cs`

## Level Design Guide

| Level | Echoes | New Mechanic | Key Idea |
|-------|--------|-------------|----------|
| 1     | 5      | Echo as platform | Jump, land, ride your echo up |
| 2     | 5      | Pressure plates | Echo lands on plate → opens door |
| 3     | 5      | Two plates | Plan landing order for two echoes |
| 4     | 5      | Timed plates (1.5s window) | Control jump distance precisely |
| 5     | 3      | Moving hazard + limited echoes | Precision & economy |

## Controls

| Input | Action |
|-------|--------|
| Hold Space / Left Click / Tap | Charge jump |
| Release | Jump (short hold = low, long hold = high & far) |

## Encore Rating

| Grade | Criteria |
|-------|----------|
| S | 80%+ echo use, 2+ shockwaves, 0 wasted landings |
| A | 60%+ echo use, 1+ shockwave |
| B | 40%+ echo use |
| C | Completed level |
| D | Completed but inefficient |
