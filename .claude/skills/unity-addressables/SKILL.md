---
trigger: Any Resources.Load usage introduced or questioned; loading meshes, audio, or texture assets at runtime; "how do I load X asset at runtime"; build size or memory questions involving assets.
---
# unity-addressables

## Purpose
Correct Addressables usage for WebGL: async loading, release discipline, no `Resources.Load` at runtime except for legacy cases that must be explicitly justified.

## Rules (enforce)
- `Addressables.LoadAssetAsync<T>` for runtime asset loads; release with `Addressables.Release` when done.
- Assets referenced by **address string** from a typed `AddressKeys` constants file — not magic strings scattered in code.
- Address constants live in a single `AddressKeys.cs` per logical group.
- `Resources.Load` requires an inline comment explaining why Addressables cannot be used.
- Async load results must handle the `null` / failed case explicitly.
- Never call `WaitForCompletion()` on WebGL — always await via coroutine or `async/await`.

## Rules (avoid)
- `Resources.LoadAll` — enumerate via Addressables label instead.
- Loading and immediately abandoning the handle (memory leak).
- `WaitForCompletion()` on WebGL — blocks the main thread, can hang the tab.
- Mixing Addressables and `Resources.Load` for the same asset type.

## Patterns

### Address constants
```csharp
// AddressKeys.cs — one file per logical group.
namespace Legends.Data.Assets
{
    public static class AddressKeys
    {
        public const string AthleteCharacter  = "athlete_character";
        public const string AthleteController = "athlete_controller";
        public const string FinishLineBurst   = "fx_finish_burst";
    }
}
```

### Async load + release (coroutine)
```csharp
private IEnumerator LoadAndSpawn()
{
    var handle = Addressables.LoadAssetAsync<GameObject>(AddressKeys.AthleteCharacter);
    yield return handle;

    if (handle.Status != AsyncOperationStatus.Succeeded)
    {
        Debug.LogError($"Failed to load {AddressKeys.AthleteCharacter}");
        FallbackToCapsule();
        yield break;
    }

    _instance = Object.Instantiate(handle.Result, parent);
    // Cache handle so we can release it on cleanup.
    _characterHandle = handle;
}

private void OnDestroy()
{
    if (_characterHandle.IsValid())
        Addressables.Release(_characterHandle);
}
```

### Load all assets with a label
```csharp
var handle = Addressables.LoadAssetsAsync<Sprite>("athlete_portraits", OnPortraitLoaded);
yield return handle;
// Release when all portraits are no longer needed.
Addressables.Release(handle);
```

### Migration: Resources.Load → Addressables
```csharp
// Before (forbidden at runtime — requires asset in Resources/ folder):
var mesh = Resources.Load<GameObject>("Meshes/PolygonSyntyCharacter");

// After:
// 1. Mark asset as Addressable in the Inspector (group: Athletes, address: "athlete_character").
// 2. Add AddressKeys.AthleteCharacter = "athlete_character".
// 3. Load async:
var handle = Addressables.LoadAssetAsync<GameObject>(AddressKeys.AthleteCharacter);
yield return handle;
```

## When Resources.Load is acceptable
- Editor-only utility code (`#if UNITY_EDITOR`).
- A generated asset (e.g., `Athlete.controller`) loaded only for a brief editor test; must be migrated before shipping.
- Always add `// TODO: migrate to Addressables` comment.

## Anti-patterns
- `Addressables.LoadAssetAsync<T>(key).Result` — synchronous access, same as `WaitForCompletion`.
- Loading a handle, using the result, discarding the handle reference — leaks memory.
- `"Meshes/PolygonSyntyCharacter"` as a magic string in `AthleteFactory.cs` — use `AddressKeys`.

## Examples
See `examples/` for annotated code.
