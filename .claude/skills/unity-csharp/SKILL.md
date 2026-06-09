---
trigger: Any C# file creation or significant edit in a Unity project; baseline for all other Unity skills.
---
# unity-csharp

## Purpose
Foundation skill. Enforces idiomatic C# patterns for Unity: assembly definitions, namespace conventions, data model discipline, and MonoBehaviour hygiene. All other skills inherit from these rules.

## Rules (enforce)
- One class per file; filename matches class name.
- `namespace` matches folder path (`Legends.Data`, `Legends.UI`, `Legends.Simulation`, etc.).
- Plain C# classes for data/simulation state — no `MonoBehaviour` unless Unity lifecycle (`Awake`, `Start`, `Update`, `OnDestroy`) is genuinely required.
- No `public` fields; use properties with `{ get; private set; }` or constructor parameters.
- Assembly definitions (`.asmdef`) per logical layer; deps flow one-way: `Data ← Services/Simulation ← Replay/UI`.
- No `static` mutable state outside explicit singletons (e.g., `ScreenRegistry`).
- No `string` magic constants for type/event names — use typed constants or enums (`EventTypes.*`).
- Split classes that exceed ~200 lines of substantive logic.

## Rules (avoid)
- `MonoBehaviour` as a data container (serialized fields holding game state).
- `FindObjectOfType` at runtime — wire dependencies in code at construction time.
- `GetComponent` in `Update` — cache in `Awake`/`Start`.
- Coroutines for pure data transforms — use plain methods or `async/await` (with WebGL constraints from `unity-webgl`).
- `[SerializeField]` as a substitute for constructor/code wiring.
- God classes — one responsibility per class.

## Patterns

### Data model (plain C#)
```csharp
namespace Legends.Data
{
    public sealed class SegmentResult
    {
        public int AthleteId { get; }
        public float TimeSeconds { get; }

        public SegmentResult(int athleteId, float timeSeconds)
        {
            AthleteId = athleteId;
            TimeSeconds = timeSeconds;
        }
    }
}
```

### MonoBehaviour shell (thin wrapper)
```csharp
// MonoBehaviour only because Unity lifecycle is required here.
public sealed class ReplaySceneController : MonoBehaviour
{
    private ReplayDirector _director;

    private void Start()
    {
        _director = new ReplayDirector(GameSession.LatestResult);
        _director.Begin();
    }
}
```

### Assembly layout
```
Legends.Data          (pure C#, no Unity deps beyond primitives)
  ↓
Legends.Services      (interfaces, SceneFlow, ScreenRegistry)
Legends.Simulation    (DragonEggRelaySimulator, EventConfigFactory)
  ↓
Legends.Replay        (AthleteFactory, ReplayDirector, camera)
Legends.UI            (UIFactory, UIScreen subclasses)
```

## Anti-patterns
- `public float speed;` — use `public float Speed { get; private set; }` or constructor.
- `FindObjectOfType<GameManager>()` — pass the dependency at construction time.
- `void Update() { GetComponent<Rigidbody>().velocity = ...; }` — cache the component.
- One 500-line class doing simulation + UI + persistence — split it.

## Examples
See `examples/` for annotated code snippets.
