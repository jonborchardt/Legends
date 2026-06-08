# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project

Olympic Manager MVP — a WebGL game on GitHub Pages. Player manages a fantasy athletic team and watches a 3D replay of a simulated Dragon Egg Relay event. Unity project targeting WebGL, no backend.

## All Work Flows Through /Plans

Every implementation task has a corresponding plan file. Before writing any code:

1. Read [Plans/MASTER.md](Plans/MASTER.md) to understand project status and what is unlocked.
2. Find the relevant sub-sub-plan (e.g. `Plans/4.3.1-Load-And-Spawn.md`) and read it fully.
3. Implement exactly what the plan specifies — no more, no less.
4. After completing a task, update the Status column in `Plans/MASTER.md` to `✅ done`.

Do not implement anything that does not have a plan file. If a task seems necessary but has no plan, say so and stop.

## Plan Hierarchy

```
Plans/MASTER.md              ← overall status tracker and dependency graph
Plans/1-Foundation.md        ← chunk overview with sub-plan index
Plans/1.1-Folder-Structure.md  ← mid-level plan
Plans/1.1.1-Create-Directories.md  ← atomic task (implement from this)
```

Always read the atomic sub-sub-plan before implementing. The chunk and mid-level files provide context but the atomic file is the spec.

## Architectural Non-Negotiables

These rules are absolute. Never violate them:

| Rule | Constraint |
|---|---|
| Simulation owns truth | Replay consumes output — never rolls dice |
| All state serializable | Plain C# objects — no MonoBehaviour state |
| No SceneManager outside SceneFlow | All transitions go through SceneFlow |
| No Animator outside AthleteAnimator | All animation calls through AthleteAnimator |
| SceneFlow is routing only | No save logic, no parameters, no game state access |
| Frames are positions only | ReplayFrame has no AnimState |
| EventConfigData is pure C# | EventConfig (ScriptableObject) wraps it |
| No EventType string literals | Use `EventTypes.*` constants from Legends.Data |
| Assembly deps are one-way | Data ← Services/Simulation ← Replay/UI |
| GameSession is the scene handoff | Set LatestResult before navigating; ReplayDirector reads it on Start |

## Deferred — Do Not Build

Training, recruitment, seasons, equipment, economy, multiple events, physics-based egg handling, character creation, online services, analytics, multiplayer, Addressables, ECS, DI frameworks.

If any proposed implementation requires these: reject it.

## Assets

Always check `Assets/Asset Packs/Synty/` for models, animations, effects, and UI elements before creating placeholders. See memory for details.
