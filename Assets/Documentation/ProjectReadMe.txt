Puzzle Level Test Case — Unity Project
======================================

A grid-based puzzle prototype built in Unity: players swipe stars on a board, match them to color-aligned gates, avoid obstacles, and beat a per-level timer. Progression (locked / active / completed levels) is stored in PlayerPrefs. UI and core systems are wired with Zenject (Extenject).

Note: A Markdown copy also lives at the repository root as README.md (for GitHub / IDEs).


Requirements
------------
• Unity Editor: 2022.3.19f1 (LTS) — other 2022.3.x may work
• TextMesh Pro: included / imported via Unity
• Packages in repo: Zenject, DOTween (Demigiant), standard UGUI

Open Assets/Scenes/GamePlayScene.unity as the main gameplay entry.


High-Level Architecture
-----------------------
1. Composition root (Zenject) — installers bind ScriptableObject data, services (ILevelService, IUIService), and core MonoBehaviour systems.
2. Game state machine — GameStateFactory switches between menu, playing, and end-of-level states.
3. Data layer — LevelDatabase references an ordered list of LevelData assets plus prefabs for stars, gates, and obstacles.
4. Presentation — UIManager resolves screen prefabs from UIScreenData.
5. Gameplay loop — LevelSpawner builds the world from LevelData; GameplayController owns input, grid logic, and win detection.

Flow (text diagram):
  Zenject Installers → GameStateFactory → Start / Playing / End states
                    → UIManager + LevelSpawner + GameplayController
                    → LevelManager (prefs + LevelDatabase)


Folder Map (game code)
----------------------
Assets/Scripts/Core/           — Level spawning, grid, gameplay, camera fit
Assets/Scripts/Core/GameStates/ — State machine + GameStateFactory
Assets/Scripts/UI/             — Screens, UIManager, LevelManager, buttons
Assets/Scripts/Installers/     — Zenject MonoInstaller bindings
Assets/Scripts/LevelEditor/    — StarElement, GateElement, ObstacleElement
Assets/Scripts/Editor/         — LevelSaverEditor (scene ↔ LevelData)
Assets/Scripts/Enums/          — ColorType, LevelStatus, GateOrientation
Assets/ScriptableObjects/      — LevelData, LevelDatabase, UIScreenData assets
Assets/Prefabs/               — UI and gameplay prefabs
Assets/Scenes/                — GamePlayScene, LevelEditorScene


Important Classes (short)
-------------------------
Installers:
  GameInstaller — spawns game state root prefab
  LevelInstaller — LevelDatabase + LevelManager (ILevelService)
  UIInstaller — UIManager (IUIService)
  LevelSpawnerInstaller, GameplayControllerInstaller — core services

States:
  GameStateFactory — ChangeState<T>(), session data (selected level, outcome)
  GameState — Enter/Exit toggles GameObject
  StartGameState — StartScreen → play
  PlayingGameState — GameScreen, spawn level, GameplayController, pause
  EndGameState — EndScreen, stars, save on win

Data:
  LevelData — grid size, timer, star/gate/obstacle lists
  LevelDatabase — ordered levels + spawn prefabs
  UIScreenData — UIView prefab list for UIManager

Gameplay:
  LevelSpawner — clear/spawn level, GridVisualizer, camera fit
  GridVisualizer — board background and lines
  GameplayController — tap + swipe input, gate matching, win when no stars
  StarElement, GateElement, ObstacleElement — level pieces

UI:
  UIManager — Show/Hide screens by type
  UIView — base active toggle
  StartScreen — level buttons (pool), play
  GameScreen — countdown timer, pause
  SettingsScreen — resume / home
  EndScreen — win/lose, star rating
  LevelManager — PlayerPrefs progression, LevelCount from database
  LevelButtonView — one start-menu level slot

Editor:
  LevelSaverEditor — Tools menu: save/load level between scene and LevelData


Gameplay Summary
----------------
• Start screen: one button per slot in LevelDatabase.levels (assign every LevelData in the database asset).
• Playing: swipe from a star — move to empty cell or match a same-color gate.
• Win: all stars gone. Lose: time runs out.
• Progress: LevelManager + PlayerPrefs.


Third-Party
-----------
Zenject / Extenject — DI
DOTween — animation
TextMesh Pro — UI text

See Assets/Zenject and Assets/Plugins/Demigiant for vendor licenses.
