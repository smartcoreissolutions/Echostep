# ECHO STEP — Unity Project Setup Guide

## 📂 Project Structure

```
Unity/EchoStep/
└── Assets/
    ├── Scripts/
    │   ├── GameConstants.cs      — All tuning values
    │   ├── GameManager.cs        — State machine, level loading
    │   ├── InputHelper.cs        — Unified input (mouse/touch/keyboard/gamepad)
    │   ├── Player/
    │   │   └── PlayerController.cs  — Charge-jump, ground detection, squash/stretch
    │   ├── Echo/
    │   │   ├── EchoManager.cs       — Echo spawning, shockwave detection
    │   │   ├── EchoController.cs    — Individual echo lifecycle
    │   │   └── ShockwaveController.cs — Expanding ring effect
    │   ├── Level/
    │   │   ├── LevelData.cs         — Per-level config
    │   │   ├── PressurePlate.cs     — Activates on contact
    │   │   ├── TimedPlate.cs        — Time-window challenge
    │   │   ├── DoorController.cs    — Slide-open door
    │   │   └── Crystal.cs           — Echo charge pickup
    │   ├── Hazards/
    │   │   └── MovingHazard.cs      — Patrolling danger
    │   ├── UI/
    │   │   └── UIManager.cs         — Title/HUD/rating/gameover screens
    │   ├── Audio/
    │   │   └── SFXManager.cs        — Procedural sound effects
    │   └── Camera/
    │       ├── CameraController.cs  — Smooth follow + screen shake
    │       └── ParallaxBackground.cs — Star field
    ├── Prefabs/     — (you'll create these)
    ├── Scenes/      — Main scene
    ├── Materials/   — (sprites, shaders)
    └── Fonts/       — Orbitron, Share Tech Mono
```

---

## 🚀 Step-by-Step Setup

### 1. Create Unity Project
- Open Unity Hub → **New Project** → **2D (URP)** or **2D Core**
- Name: `EchoStep`
- Copy all files from `Unity/EchoStep/Assets/Scripts/` into your project's `Assets/Scripts/`

### 2. Install TextMeshPro
- When prompted, click **Import TMP Essentials**
- Or: Window → TextMeshPro → Import TMP Essential Resources

### 3. Set Up Tags & Layers

**Tags** (Edit → Project Settings → Tags and Layers):
- `Player`
- `Echo`
- `Goal`
- `Hazard`
- `Crystal`

**Layers**:
- Layer 6: `Ground`
- Layer 7: `Echo`
- Layer 8: `Player`

**Physics2D Layer Collision** (Edit → Project Settings → Physics 2D):
- Player collides with: Ground, Echo, Default
- Echo collides with: Ground, Player, Default
- Uncheck Echo ↔ Echo collision

### 4. Import Fonts (Optional but recommended)
- Download [Orbitron](https://fonts.google.com/specimen/Orbitron) and [Share Tech Mono](https://fonts.google.com/specimen/Share+Tech+Mono)
- Import .ttf files into `Assets/Fonts/`
- Create TMP Font Assets: Window → TextMeshPro → Font Asset Creator

### 5. Create the Player Prefab

1. Create empty GameObject → name it **"Player"**
2. Set tag to **Player**, layer to **Player**
3. Add components:
   - `Rigidbody2D` (Gravity Scale: 0, Freeze Rotation Z: ✓, Collision Detection: Continuous)
   - `BoxCollider2D` (Size: 0.28 x 0.36)
   - `PlayerController` script
4. Create child sprites:
   - **Body**: Sprite (white square), Scale 0.28 x 0.36, Color #00DDFF
   - **EyeLeft**: Sprite (white square), Scale 0.06 x 0.05, Position (-0.04, 0.04, 0)
   - **EyeRight**: Sprite (white square), Scale 0.06 x 0.05, Position (0.04, 0.04, 0)
   - **Arrow**: Sprite (triangle/arrow), Position (0.22, 0, 0), Color #00FFFF88
5. Create child ParticleSystems:
   - **JumpParticles**: Burst 10, Start Color cyan, Start Size 0.05, Duration 0.3
   - **LandParticles**: Burst 8, Start Color gray, Start Size 0.04, Gravity 3
   - **ChargeParticles**: Looping, Rate 15, Start Color orange, Orbit around center
   - **TrailParticles**: Looping, Rate 20, Start Color cyan, Start Size 0.03
6. Assign all references in `PlayerController` inspector
7. Save as prefab in `Assets/Prefabs/`

### 6. Create the Echo Prefab

1. Create empty → name **"Echo"**
2. Tag: **Echo**, Layer: **Echo**
3. Add components:
   - `Rigidbody2D` (Kinematic, Freeze Rotation Z: ✓)
   - `BoxCollider2D` (Size: 0.28 x 0.36, Used By Effector: ✓)
   - `PlatformEffector2D` (Use One Way: ✓, Surface Arc: 170)
   - `EchoController` script
4. Child sprites:
   - **Body**: White square, Scale 0.28 x 0.36, Color #0099CC (will be set by script)
   - **EyeLeft/Right**: Same as player but smaller
5. Child: **ReplayParticles** (ParticleSystem, Burst 8, cyan)
6. Save as prefab

### 7. Create the Shockwave Prefab

1. Create empty → name **"Shockwave"**
2. Add `ShockwaveController` script
3. Child: **Ring** sprite (circle outline), initial scale 0.1
4. Save as prefab

### 8. Build Level Prefabs

Each level is a prefab containing platforms, hazards, plates, doors, crystals, and a goal zone.

#### Level 1: "Step on your echoes"
```
Platforms (create Sprites with BoxCollider2D, Layer: Ground):
  - (0, 5.2)    size 3.0 x 0.4
  - (5, 5.2)    size 2.0 x 0.4
  - (9, 4.0)    size 2.0 x 0.4
  - (13, 2.8)   size 2.0 x 0.4

Goal Zone (BoxCollider2D Trigger, Tag: Goal):
  - (14, 2.2)   size 0.6 x 0.6

Crystal (Tag: Crystal, CircleCollider2D Trigger):
  - (7, 4.6)

LevelData:
  - playerSpawn: (1, 5.2)
  - maxEchoes: 5
  - echoCharges: 5
  - levelMessage: "Level 1: Step on your echoes"
```

#### Level 2: "Your echoes can trigger plates"
```
Platforms:
  - (0, 5.2)    size 3.5 x 0.4
  - (5.5, 5.2)  size 1.0 x 0.4
  - (8.5, 5.2)  size 3.0 x 0.4

Pressure Plate at (5.7, 5.1) → links to Door
Door at (8.3, 4.2) size 0.3 x 1.0
Goal at (10.5, 4.6)

LevelData: playerSpawn (1, 5.2), message "Level 2: Your echoes can trigger plates"
```

#### Level 3: "Plan your landing order"
```
Platforms:
  - (0, 5.2)    size 3.0 x 0.4
  - (4, 4.8)    size 1.2 x 0.4
  - (6.5, 4.4)  size 1.2 x 0.4
  - (9, 5.2)    size 3.0 x 0.4

Two Pressure Plates → Two Doors
Crystal at (5.6, 3.8)
Goal at (11, 4.6)
```

#### Level 4: "Time your echoes carefully"
```
Platforms:
  - (0, 5.2)    size 3.0 x 0.4
  - (4.5, 4.8)  size 1.5 x 0.4
  - (8, 5.2)    size 3.0 x 0.4

Timed Plate at (4.8, 4.7), timeWindow: 2.5
Door at (7.8, 4.2)
Goal at (10, 4.6)
```

#### Level 5: "Farewell to past selves"
```
Platforms:
  - (0, 5.2)    size 2.5 x 0.4
  - (4, 4.8)    size 1.0 x 0.4
  - (6.5, 4.2)  size 1.0 x 0.4
  - (9, 3.6)    size 1.0 x 0.4
  - (11.5, 3.0) size 2.0 x 0.4

Moving Hazards (Tag: Hazard):
  - pointA (3, 4.9) → pointB (6, 4.9), speed 1.5
  - pointA (8, 3.8) → pointB (10.5, 3.8), speed 1.8

Crystal at (7.5, 3.5)
Goal at (12.5, 2.4)

LevelData: maxEchoes 3, echoCharges 3
```

### 9. Set Up the Main Scene

1. **Create scene** → Save as `Assets/Scenes/MainScene`
2. **GameManager** (empty GO):
   - Add `GameManager` script
   - Assign levelPrefabs array (5 levels)
   - Assign player, echoManager, uiManager, cameraController references
3. **Player** (instance or spawned by GameManager)
4. **EchoManager** (empty GO):
   - Add `EchoManager` script
   - Assign echo & shockwave prefabs
5. **SFXManager** (empty GO):
   - Add `SFXManager` script
6. **Main Camera**:
   - Add `CameraController` script (target = Player)
   - Add `ParallaxBackground` script
   - Background color: #060610
   - Orthographic Size: 5
7. **Canvas** (Screen Space - Overlay):
   - Add `UIManager` script
   - Create child panels: TitlePanel, HUDPanel, RatingPanel, GameOverPanel
   - Build UI elements per the UIManager fields
   - Use Orbitron Bold for titles, Share Tech Mono for body text
   - Color scheme: #00DDFF (cyan), #060610 (dark bg), #FFD700 (gold accents)

### 10. Configure Physics

Edit → Project Settings → Physics 2D:
- Gravity: (0, -18)
- Default Contact Offset: 0.01

### 11. Build Settings

- Add `MainScene` to Build Settings
- Target: PC/Mac/Linux Standalone, WebGL, Android, iOS — all work
- For WebGL: Player Settings → Resolution: 960 x 540, set template to Minimal

---

## 🎮 How It Plays (Same as HTML Version)

1. **One button** — tap/click/space
2. **Tap** (< 150ms) = flip facing direction
3. **Hold** = charge jump power
4. **Release** = jump at 62° angle in facing direction
5. Each landing spawns an **echo** that replays your jump after 2s
6. Echoes become **one-way platforms** you can stand on
7. Two echoes colliding create a **shockwave** that launches you
8. Reach the **gold goal zone** to complete each level
9. Get rated S/A/B/C/D based on echo utilization

---

## 🎨 Visual Polish Checklist

- [x] Squash/stretch on player (jump/land)
- [x] Eye blink animation
- [x] Parallax star field
- [x] Screen shake (jump, land, shockwave, death)
- [x] Particle effects (jump, land, charge, echo spawn, crystal, shockwave)
- [x] Procedural sound effects (no audio files needed)
- [x] Glowing platforms (edge highlights)
- [x] Pulsing hazards
- [x] Bobbing crystals
- [x] Echo scan-line effect (use shader or overlay sprite)
- [x] Goal beacon rays
- [x] Charge ring with sparks

---

## 📦 Optional Enhancements

- **Post Processing**: Add Bloom (URP) for glow effects
- **Shader Graph**: Create custom echo hologram shader with scan lines
- **Cinemachine**: Replace CameraController with Cinemachine for smoother follow
- **DOTween**: Smoother UI animations (grade scale-in, panel transitions)
- **ProBuilder**: If you want 3D platforms instead of 2D sprites
