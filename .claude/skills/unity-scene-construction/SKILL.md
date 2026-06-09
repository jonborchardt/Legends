---
trigger: Adding a new screen, scene, or major feature; questions about scene structure, bootstrap, or object lifecycle; architectural decisions.
---
# unity-scene-construction

## Purpose
PSC master skill (Programmatic Scene Construction). Synthesises `unity-csharp`, `unity-runtime-ui`, `unity-prefabs`, and `unity-animation` into a complete workflow: bootstrap pattern, screen registry, factory classes, code-generated assets. The editor is a build host only — all scene structure is produced by C# code.

## Rules (enforce)
- Only two real Unity scenes: `Boot.unity` and `EventReplay.unity`. All other "screens" are `UIScreen` subclasses built in code.
- Every scene has exactly one bootstrap `MonoBehaviour`; no other hand-placed GameObjects.
- `ScreenRegistry` is the sole router for UI screens; `SceneFlow` is the sole router for scene transitions.
- `SceneFlow` contains no game state, no save logic, no parameters — routing only.
- `GameSession` is the data handoff between scenes: set before navigate, read on `Start`.
- Generated artifacts (`AnimatorController`, `EventConfig`) are rebuilt via `LegendsBuildTools` before every build (`Tools > Legends > Build All`).
- `DontDestroyOnLoad` used only for `ScreenRegistry` and explicit singletons registered there.

## Rules (avoid)
- New `.unity` scene files for UI screens.
- `SceneManager.LoadScene` outside `SceneFlow`.
- `GameObject.Find` or `FindObjectOfType` in production code.
- `[SerializeField]` as a substitute for constructor/code wiring.
- Storing scene-transition data anywhere except `GameSession`.
- `DontDestroyOnLoad` on ad-hoc objects.

## Patterns

### Two-scene rule
```
Boot.unity         — BootController bootstrap → builds UI canvas → shows MainMenuScreen
EventReplay.unity  — ReplaySceneController bootstrap → builds replay stage → plays result
```
All other "screens" (TeamHub, Results, etc.) are canvas GameObjects activated/deactivated inside Boot.

### Bootstrap pattern
```csharp
// Single MB in Boot scene. Everything else is spawned from here.
public sealed class BootController : MonoBehaviour
{
    private void Start()
    {
        ScreenRegistry.Instance.RegisterAll();
        ScreenRegistry.Instance.ShowScreen<MainMenuScreen>();
    }
}
```

### Screen lifecycle
1. `UIScreen.Build()` — constructs the `GameObject` hierarchy (called once on register).
2. `UIScreen.Show()` — `SetActive(true)` + populate data.
3. `UIScreen.Hide()` — `SetActive(false)`.
Never destroy and re-create a screen; reuse the existing GameObject.

### SceneFlow: routing only
```csharp
// SceneFlow has NO knowledge of game state or save data.
public static class SceneFlow
{
    public static void GoToReplay()  => SceneManager.LoadScene("EventReplay");
    public static void GoToBoot()    => SceneManager.LoadScene("Boot");
}
```

### GameSession: scene handoff
```csharp
// Set before navigating; read in Start of the receiving scene.
GameSession.LatestResult = simulationResult;
SceneFlow.GoToReplay();
```

### Factory class pattern
```csharp
// Static factory; no state; returns a fully wired GameObject.
public static class AthleteCardFactory
{
    public static GameObject Create(AthleteData data, Transform parent)
    {
        var root = UIFactory.CreatePanel("AthleteCard", parent);
        // ... build hierarchy
        return root;
    }
}
```

## Anti-patterns
- Adding a third Unity scene for a new screen.
- `SceneFlow.GoToReplay(simulationResult)` — no parameters on SceneFlow; use GameSession.
- `FindObjectOfType<ScreenRegistry>()` — use `ScreenRegistry.Instance`.
- `[SerializeField] public BootController boot;` — wire in code.

## Examples
See `examples/` for annotated snippets of each pattern.
