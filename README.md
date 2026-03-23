# Puzzle Level Test Case — Unity Project

> **Inside Unity:** the same documentation is also available as a **TextAsset** at [`Assets/Documentation/ProjectReadMe.txt`](Assets/Documentation/ProjectReadMe.txt) (visible in the Project window).

A **grid-based puzzle prototype** built in Unity: players swipe stars on a board, match them to color-aligned gates, avoid obstacles, and beat a per-level timer. Progression (locked / active / completed levels) is stored in **PlayerPrefs**. UI and core systems are wired with **Zenject (Extenject)**.

---

## Requirements

| Item | Version / notes |
|------|------------------|
| **Unity Editor** | **2022.3.19f1** (LTS) — other 2022.3.x may work |
| **TextMesh Pro** | Included / imported via Unity |
| **Packages / plugins in repo** | **Zenject**, **DOTween** (Demigiant), standard UGUI |

Open **`Assets/Scenes/GamePlayScene.unity`** as the main gameplay entry (configure your Scene Build Index if you ship builds).

---

## High-Level Architecture

The project follows a **layered, state-driven** layout:

1. **Composition root (Zenject)** — Scene / project installers bind `ScriptableObject` data, services (`ILevelService`, `IUIService`), and core `MonoBehaviour` systems as singletons where appropriate.
2. **Game state machine** — `GameStateFactory` switches between **menu**, **playing**, and **end-of-level** states. Each state is a `MonoBehaviour` under a shared root prefab; only the active state’s GameObject is enabled.
3. **Data layer** — `LevelDatabase` references an ordered list of `LevelData` assets plus prefabs for stars, gates, and obstacles. Runtime never hardcodes level layout.
4. **Presentation** — `UIManager` resolves screen prefabs from `UIScreenData`, instantiates once per screen type, and shows/hides roots.
5. **Gameplay loop** — `LevelSpawner` builds the visual grid and entities from `LevelData`. `GameplayController` owns input, grid logic, and win detection.

```
┌─────────────────────────────────────────────────────────────┐
│                    Zenject Installers                        │
│  (Game, Level, UI, LevelSpawner, GameplayController)       │
└──────────────────────────┬──────────────────────────────────┘
                           ▼
┌─────────────────────────────────────────────────────────────┐
│  GameStateFactory  ──►  Start / Playing / End states        │
└──────────────────────────┬──────────────────────────────────┘
                           ▼
        ┌──────────────────┼──────────────────┐
        ▼                  ▼                  ▼
   UIManager         LevelSpawner      GameplayController
   + screens         + GridVisualizer   (input + rules)
        │                  │
        └──────────┬───────┘
                   ▼
            LevelManager (prefs + LevelDatabase)
```

---

## Folder Map (game code)

| Path | Role |
|------|------|
| `Assets/Scripts/Core/` | Level spawning, grid visuals, gameplay input/rules, camera fit |
| `Assets/Scripts/Core/GameStates/` | State machine entries and `GameStateFactory` |
| `Assets/Scripts/UI/` | Screens, `UIManager`, `LevelManager`, level buttons |
| `Assets/Scripts/Installers/` | Zenject `MonoInstaller` bindings |
| `Assets/Scripts/LevelEditor/` | Scene-placed element components (`StarElement`, `GateElement`, `ObstacleElement`) |
| `Assets/Scripts/Editor/` | `LevelSaverEditor` — save/load level layouts between scene and assets |
| `Assets/Scripts/Enums/` | Shared enums (`ColorType`, `LevelStatus`, `GateOrientation`) |
| `Assets/ScriptableObjects/` | `LevelData` / `LevelDatabase` / `UIScreenData` assets |
| `Assets/Prefabs/` | UI screen prefabs and gameplay prefabs |
| `Assets/Scenes/` | `GamePlayScene`, `LevelEditorScene` |

---

## Important Classes (what they do)

### Composition & flow

| Class | Responsibility |
|-------|----------------|
| **`GameInstaller`** | Instantiates the **game state root** prefab that holds all `GameState` behaviours. |
| **`LevelInstaller`** | Binds **`LevelDatabase`** instance and creates **`LevelManager`** (`ILevelService`). |
| **`UIInstaller`** | Binds **`UIManager`** from prefab (`IUIService`). |
| **`LevelSpawnerInstaller`**, **`GameplayControllerInstaller`** | Register **`LevelSpawner`** and **`GameplayController`** as scene services. |

### State machine

| Class | Responsibility |
|-------|----------------|
| **`GameStateFactory`** | Registers state objects, exposes **`ChangeState<T>()`**, holds **selected level index** and **last run outcome** (time, completed flag) for the end screen. |
| **`GameState`** | Abstract base: **`Enter()` / `Exit()`** toggle the state’s GameObject; injects **`IUIService`**. |
| **`StartGameState`** | Shows **`StartScreen`**, listens for level pick and play; navigates to **`PlayingGameState`**. |
| **`PlayingGameState`** | Shows **`GameScreen`**, spawns level via **`LevelSpawner`**, runs **`GameplayController`**; pause opens **`SettingsScreen`**; win/fail → **`EndGameState`**. |
| **`EndGameState`** | Shows **`EndScreen`** with win/lose UI and **star rating**; can save progress on success. |

### Data

| Class | Responsibility |
|-------|----------------|
| **`LevelData`** | ScriptableObject: **grid size**, **time limit**, lists of **stars / gates / obstacles** (designer-authored). |
| **`LevelDatabase`** | Ordered **list of `LevelData`**, plus **prefabs** used by **`LevelSpawner`** and the level editor. |
| **`UIScreenData`** | List of **`UIView`** prefabs the **`UIManager`** can instantiate by type. |

### Gameplay & world

| Class | Responsibility |
|-------|----------------|
| **`LevelSpawner`** | Clears previous level, generates **grid background/lines** (`GridVisualizer`), **instantiates** entities from **`LevelData`**, **fits** the main orthographic camera to the board. |
| **`GridVisualizer`** | Builds **sprite-based** background, cell lines, and border under gameplay. |
| **`GameplayController`** | **Coroutine-driven input**: tap a star, swipe to move or **match a same-color gate**; blocks obstacles and other stars; fires **`OnAllStarsMatched`** when no stars remain. Uses **DOTween** for motion. |
| **`StarElement`**, **`GateElement`**, **`ObstacleElement`** | Level pieces with **color** and visuals; used in editor and at runtime. |

### UI & progression

| Class | Responsibility |
|-------|----------------|
| **`UIManager`** | **`Show<T>()** / **`Hide<T>()`**: resolves prefab from **`UIScreenData`**, Zenject-instantiates once per `T`, caches instances. |
| **`UIView`** | Base **show/hide** via `GameObject.SetActive`. |
| **`StartScreen`** | **Pooled** level buttons; **play** CTA; events for selection and start. |
| **`GameScreen`** | **Countdown timer** (with optional urgent styling), pause button, level label. |
| **`SettingsScreen`** | Pause overlay: **resume** / **home**. |
| **`EndScreen`** | Win vs lose panels; **star rating** from remaining time vs limit; continue / replay. |
| **`LevelManager`** | Implements **`ILevelService`**: **`LevelCount`** from database, **`GetLevelStatus`** / **`CompleteLevel`** via **PlayerPrefs**, **`CurrentLevelIndex`**. |
| **`LevelButtonView`** | Single level entry on the start screen (locked / active / completed / selected look). |

### Editor tooling

| Class | Responsibility |
|-------|----------------|
| **`LevelSaverEditor`** | Editor window: **save** scene-placed stars/gates/obstacles into a **`LevelData`** asset, **load** asset back into the scene, optional **auto-reload** when the SO changes. |

---

## Gameplay Summary

- **Start screen**: One button per entry in **`LevelDatabase.levels`** (not per loose `LevelData` file in the project folder — assets must be **assigned to the database list**).
- **Playing**: Swipe from a **star** in a cardinal direction — slide to an empty cell, or enter a **gate** if its **color matches** the star.
- **Win**: All stars cleared (matched through gates).
- **Lose**: Timer hits zero.
- **Progress**: `LevelManager` writes **per-level status** and **current level index** to PlayerPrefs.

---

## Third-Party Assets (in repo)

- **[Zenject / Extenject](https://github.com/modesttree/Zenject)** — dependency injection and scene installers.  
- **[DOTween](http://dotween.demigiant.com/)** — tweens for UI and star movement.  
- **TextMesh Pro** — UI text.

---

## License / attribution

Game logic and scene setup are project-specific. **Zenject** and **DOTween** retain their respective licenses; see vendor folders under `Assets/Zenject` and `Assets/Plugins/Demigiant`.
