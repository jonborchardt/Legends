---
trigger: Any mention of .prefab, Instantiate, or "can we make a prefab for X"; creating or editing a factory class; spawning repeated objects (cards, rows, athletes).
---
# unity-prefabs

## Purpose
Teaches when prefabs are appropriate vs. when runtime construction is better. In PSC projects, prefabs are used for **authored assets** (meshes, FX) and **editor tooling** — never for runtime UI or game logic objects.

## Rules (enforce)
- Runtime UI and logic objects are **never prefabs** — they are built by factory classes (`AthleteCardFactory`, `ResultRowFactory`, etc.).
- `Instantiate` on authored 3D assets (meshes, FX) is acceptable — load via Addressables, not `Resources.Load`.
- Factories are `static` classes that return `GameObject` (or a typed wrapper); they hold no state.
- Every `Instantiate` call must include a clearly named parent `Transform` argument.

## Rules (avoid)
- `.prefab` files for UI panels, cards, rows, or any screen element.
- `Instantiate` on a `Resources.Load` result at runtime — use Addressables.
- Prefab variants as a substitute for parameterised factory methods.
- `[SerializeField]` references to prefab assets on MonoBehaviours that could be resolved in code.

## Patterns

### When to use prefabs (authored 3D/FX assets)
```
Assets/Models/PodiumStand.prefab       ← OK: authored mesh, spawned by AthleteFactory
Assets/VFX/FinishLineBurst.prefab      ← OK: authored FX, loaded via Addressables
Assets/Editor/GeneratorPreview.prefab  ← OK: editor-only tooling
```

### When NOT to use prefabs (UI, logic objects)
```
❌ Assets/Prefabs/AthleteCard.prefab    — use AthleteCardFactory.Create() instead
❌ Assets/Prefabs/ResultRow.prefab      — use ResultRowFactory.Create() instead
❌ Assets/Prefabs/ConfirmModal.prefab   — build in code via UIFactory
```

### Factory pattern
```csharp
public static class AthleteCardFactory
{
    public static GameObject Create(AthleteData data, Transform parent)
    {
        var root  = UIFactory.CreatePanel("AthleteCard", parent);
        var name  = UIFactory.CreateText(data.Name, root.transform, UIStyle.FontSizes.Body);
        var score = UIFactory.CreateText(data.Score.ToString(), root.transform, UIStyle.FontSizes.Body);
        return root;
    }
}
```

### Acceptable Instantiate pattern (3D mesh from Addressables)
```csharp
// Load via Addressables, then Instantiate with explicit parent.
var handle = Addressables.LoadAssetAsync<GameObject>(AddressKeys.AthleteCharacter);
await handle.Task;
var instance = Object.Instantiate(handle.Result, spawnPoint.position, Quaternion.identity, stageParent);
```

## Anti-patterns
- `var card = Instantiate(cardPrefab);` — no parent argument; orphaned in scene root.
- `var mesh = Resources.Load<GameObject>("Meshes/Athlete"); Instantiate(mesh);` — use Addressables.
- Creating a prefab variant for each athlete skin instead of a parameterised factory.

## Examples
See `examples/` for annotated snippets.
