# Mindlock

A 2D top-down escape-room puzzle game made in Unity. Wake up locked in a lab, explore the rooms, collect items, crack codes and fix the faulty equipment before time runs out.

## Gameplay

- Explore connected rooms and interact with objects (doors, computers, pipes, boilers, keypads).
- Pick up items and manage them in your inventory.
- Solve puzzles: enter codes on a keypad, use the computer console, repair the pipes and deal with a gas leak.
- Follow on-screen objectives for each room while a time bar keeps the pressure on.
- Atmosphere through animated sprites, lighting effects (monitor pulses, alarm lights, wall shadows) and sound.

## Controls

| Key | Action |
|---|---|
| Arrow keys / WASD | Move |
| E | Interact (doors, items, computers, pipes) |
| Enter | Confirm in the computer console |
| Esc | Pause menu / close panels |

## Built with

- Unity **6000.2.7f2** (Unity 6) with the Universal Render Pipeline (2D)
- C#
- TextMesh Pro

## Project structure

```
Mindlock/
├─ Assets/
│  ├─ Animations/   monitor and character animations
│  ├─ Audio/        songs and sound effects
│  ├─ MainMenu/     main menu assets
│  ├─ Scenes/       Main Menu, Loading Screen, game rooms
│  ├─ Script/       gameplay scripts (player, doors, puzzles, UI)
│  └─ ...           textures, videos, settings
├─ Packages/
└─ ProjectSettings/
```

## Getting started

1. Install [Unity Hub](https://unity.com/download) and Unity **6000.2.7f2**.
2. Clone the repository:
   ```bash
   git clone https://github.com/JustMeGoro/unity-mindlock.git
   ```
3. In Unity Hub choose **Add project from disk** and select the `Mindlock` folder.
4. Open the `MainMenu` scene and press **Play**.

## Status

In development (beta). Rooms 1 and 2 are playable.
