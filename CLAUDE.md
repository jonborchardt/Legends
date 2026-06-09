# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project

Olympic Manager MVP — a WebGL game on GitHub Pages. Player manages a fantasy athletic team and watches a 3D replay of a simulated Dragon Egg Relay event. Unity project targeting WebGL, no backend.

## Active Refactor: Programmatic Scene Construction (PSC)

The project is undergoing a **Code-First Authoring** refactor (also called Programmatic Scene
Construction). The goal: C# code is the sole authoritative source for all scene structure, UI
layout, and runtime objects. The Unity Editor is a build host only — not an authoring tool.

**End state: when the refactor is complete, the Unity Editor is never required for development.**
All future work is written as C# and takes effect on the next Play/build. No inspector, no scene
hierarchy, no prefab editing mode — ever.

**The "never edit in editor" rule** — for any UI, prefab, or scene content change:
1. Edit the relevant `*Screen.cs`, `*Factory.cs`, or generator script
2. Hit Play — the change is live immediately
3. AnimatorController only: run `Tools > Legends > Regenerate Athlete Controller`

Refactor plan: [Plans/refactor/MASTER.md](Plans/refactor/MASTER.md)

PSC rules (in addition to the non-negotiables below):

| Rule | Constraint |
|---|---|
| No editor-authored UI | All canvas/UI hierarchy built by `UIFactory` + `UIScreen.Build()` |
| No .prefab files (runtime objects) | AthleteCard, ResultRow, Athlete built by factory classes |
| No `[SerializeField]` drag-and-drop | All references assigned in code |
| No inspector-assigned materials | Materials created with `new Material(Shader.Find(...))` |
| No hand-edited .asset files | EventConfig created by `EventConfigFactory`; AnimatorController by generator script |
| No hand-placed GameObjects in scenes | Scenes contain only `SceneBootstrap`; everything else is spawned |

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

Training, recruitment, seasons, equipment, economy, multiple events, physics-based egg handling, character creation, online services, analytics, multiplayer, Addressables, ECS, DI frameworks.

If any proposed implementation requires these: reject it.

## Assets

See [synty asset info](CLAUDE_ASSETS.md) for models, animations, effects, and UI elements before creating placeholders. See memory for details.
