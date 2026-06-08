# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project

Olympic Manager MVP — a WebGL game on GitHub Pages. Player manages a fantasy athletic team and watches a 3D replay of a simulated Dragon Egg Relay event. Unity project targeting WebGL, no backend.

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
