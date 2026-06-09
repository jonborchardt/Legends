# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project

Olympic Manager MVP — a WebGL game on GitHub Pages. Player manages a fantasy athletic team and watches a 3D replay of a simulated Dragon Egg Relay event. Unity project targeting WebGL, no backend.

## Status

MVP code complete. Full player loop is implemented and working in Play mode:
Main Menu → Team Hub (view roster) → simulate Dragon Egg Relay → Replay3D (3D replay) → Results → persist to localStorage.

**Pending before live deployment (manual steps):**
- Open Unity → switch Build Target to WebGL → confirm build succeeds
- GitHub Pages: repo Settings → Pages → source: `gh-pages` branch / root
- Smoke test the deployed URL after first CI run

## Architecture: Programmatic Scene Construction (PSC)

The project uses **Code-First Authoring** (Programmatic Scene Construction). C# code is the sole authoritative source for all scene structure, UI layout, and runtime objects. The Unity Editor is a build host only — not an authoring tool.

**The "never edit in editor" rule** — for any UI, prefab, or scene content change:
1. Edit the relevant `*Screen.cs`, `*Factory.cs`, or generator script
2. Hit Play — the change is live immediately

Generated artifacts (AnimatorController, etc.) rebuild automatically on script recompile and before every build via `LegendsBuildTools`. To force a rebuild: `Tools > Legends > Build All`.

PSC rules:

| Rule | Constraint |
|---|---|
| No editor-authored UI | All canvas/UI hierarchy built by `UIFactory` + `UIScreen.Build()` |
| No .prefab files (runtime objects) | AthleteCard, ResultRow, Athlete built by factory classes |
| No `[SerializeField]` drag-and-drop | All references assigned in code |
| No inspector-assigned materials | Materials created with `new Material(Shader.Find(...))` |
| No hand-edited .asset files | EventConfig created by `EventConfigFactory`; AnimatorController by generator script |
| No hand-placed GameObjects in scenes | Scenes contain only a single bootstrap MonoBehaviour; everything else is spawned |

## Key Files

| What you want to change | File to edit |
|---|---|
| Main menu layout / buttons | [Assets/Scripts/UI/Screens/MainMenuScreen.cs](Assets/Scripts/UI/Screens/MainMenuScreen.cs) |
| Team hub layout / athlete cards | [Assets/Scripts/UI/Screens/TeamHubScreen.cs](Assets/Scripts/UI/Screens/TeamHubScreen.cs), [AthleteCardFactory.cs](Assets/Scripts/UI/Factories/AthleteCardFactory.cs) |
| Results screen / row layout | [Assets/Scripts/UI/Screens/ResultsScreen.cs](Assets/Scripts/UI/Screens/ResultsScreen.cs), [ResultRowFactory.cs](Assets/Scripts/UI/Factories/ResultRowFactory.cs) |
| UI colours / font sizes | [Assets/Scripts/UI/UIStyle.cs](Assets/Scripts/UI/UIStyle.cs) |
| UI layout primitives (buttons, panels, scroll views) | [Assets/Scripts/UI/UIFactory.cs](Assets/Scripts/UI/UIFactory.cs) |
| 3D athlete visual / material | [Assets/Scripts/Replay/AthleteFactory.cs](Assets/Scripts/Replay/AthleteFactory.cs) |
| Athlete animation states | [Assets/Editor/AthleteControllerGenerator.cs](Assets/Editor/AthleteControllerGenerator.cs) → regenerate |
| Event config (segments, timing) | [Assets/Scripts/Simulation/EventConfigFactory.cs](Assets/Scripts/Simulation/EventConfigFactory.cs) |
| Scene navigation routes | [Assets/Scripts/Services/SceneFlow.cs](Assets/Scripts/Services/SceneFlow.cs) |
| Replay camera behaviour | [Assets/Scripts/Replay/ReplayCameraController.cs](Assets/Scripts/Replay/ReplayCameraController.cs) |

## Non-Obvious Implementation Notes

**SceneFlow delegate pattern** — `SceneFlow` (Legends.Services) exposes `static Action` fields (`GoToMainMenuHandler`, etc.) that `ScreenRegistry` (Legends.UI) registers at startup via `[RuntimeInitializeOnLoadMethod]`. This indirection exists because Services cannot reference UI types without creating a circular assembly dependency.

**EventConfigFactory lives in Legends.Simulation** — not Legends.Services, because `EventConfig`/`EventConfigData` are defined in that assembly and keeping it there avoids a cross-assembly reference.

**Addressables for athlete assets** — `AthleteFactory` loads the character mesh and `AnimatorController` via Addressables (address keys in `AddressKeys.cs`). In-editor it falls back to `AssetDatabase` paths for fast iteration; the async path is only active in builds. `Addressables.WaitForCompletion()` is forbidden on WebGL — `ReplaySceneController` awaits the load before calling `Spawn`.

## Scenes

Only two scenes exist in the build:

| Scene | Purpose |
|---|---|
| `Boot.unity` | Entry point. ScreenRegistry auto-bootstraps; BootController triggers main menu. |
| `Replay3D.unity` | 3D replay. ReplaySceneController wires up ReplayDirector and camera in code. |

All UI screens (MainMenu, TeamHub, Results) are canvas GameObjects built in code by ScreenRegistry — no separate scene files.

## Architectural Non-Negotiables

These rules are absolute. Never violate them:

| Rule                                | Constraint                                                           |
| ----------------------------------- | -------------------------------------------------------------------- |
| Simulation owns truth               | Replay consumes output — never rolls dice                            |
| All state serializable              | Plain C# objects — no MonoBehaviour state                            |
| No SceneManager outside SceneFlow   | All transitions go through SceneFlow                                 |
| No Animator outside AthleteAnimator | All animation calls through AthleteAnimator                          |
| SceneFlow is routing only           | No save logic, no parameters, no game state access                   |
| Frames are positions only           | ReplayFrame has no AnimState                                         |
| EventConfigData is pure C#          | EventConfig (ScriptableObject) wraps it                              |
| No EventType string literals        | Use `EventTypes.*` constants from Legends.Data                       |
| Assembly deps are one-way           | Data ← Services/Simulation ← Replay/UI                               |
| GameSession is the scene handoff    | Set LatestResult before navigating; ReplayDirector reads it on Start |

## Deferred — Do Not Build

Training, recruitment, seasons, equipment, economy, multiple events, physics-based egg handling, character creation, online services, analytics, multiplayer, ECS, DI frameworks.

If any proposed implementation requires these: reject it.

## Skills

Project-local skills in `.claude/skills/` — loaded automatically by the harness:

| Skill | When it applies |
|---|---|
| `unity-csharp` | Any C# file creation or edit; foundation for all other skills |
| `unity-scene-construction` | Adding screens, scenes, or major features; architectural decisions |
| `unity-runtime-ui` | Any UI screen, panel, button, or layout change |
| `unity-prefabs` | Factory classes, `Instantiate`, spawning repeated objects |
| `unity-animation` | Animator states, transitions, `AthleteAnimator`, controller generator |
| `unity-addressables` | Runtime asset loading; any `Resources.Load` question |
| `unity-local-storage` | Save/load, `ISaveService`, `GameState`, schema migration |
| `unity-webgl` | Threading, async, file I/O, build settings, GitHub Pages deploy |
| `unity-testing` | Writing or reviewing any test; simulation or data-transform logic |

## Assets

See [synty asset info](CLAUDE_ASSETS.md) for models, animations, effects, and UI elements before creating placeholders. See memory for details.
