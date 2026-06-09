---
trigger: Any persistence, save, load, or settings question; modifying ISaveService, PlayerPrefsSaveService, or GameState; "how do I save X" or "data isn't persisting between sessions".
---
# unity-local-storage

## Purpose
Correct save/load on WebGL using `PlayerPrefs` (backed by IndexedDB in the browser). Covers serialization strategy (JSON via Newtonsoft), versioning, migration, and the `ISaveService` abstraction pattern.

## Rules (enforce)
- All save/load goes through `ISaveService` — never `PlayerPrefs` directly in game logic.
- `GameState` is a plain C# class serializable by Newtonsoft JSON — no `[SerializeField]`.
- Save key is versioned (`"legends_save_v1"`) — increment on breaking schema changes; add a migration step.
- `PlayerPrefs.Save()` called explicitly after every write (not guaranteed on WebGL quit).
- `Exists()` checked before `Load()` — `null` from `Load` means "no save", not an error.
- Keep JSON payload under 64 KB per key (IndexedDB per-key limits in some browsers).

## Rules (avoid)
- `System.IO.File` for storage (blocked on WebGL — see `unity-webgl`).
- Storing large binary blobs in `PlayerPrefs`.
- Storing `UnityEngine` types (`Vector3`, `Color`) directly — convert to plain value types first.
- Multiple save keys for one logical save slot — one JSON blob per slot.
- Silent `catch (JsonException)` — log and return `null` so callers handle it.

## Patterns

### ISaveService interface
```csharp
public interface ISaveService
{
    bool   Exists();
    void   Save(GameState state);
    GameState Load();   // returns null if no save
    void   Delete();
}
```

### PlayerPrefsSaveService implementation
```csharp
public sealed class PlayerPrefsSaveService : ISaveService
{
    private const string Key = "legends_save_v1";

    public bool Exists() => PlayerPrefs.HasKey(Key);

    public void Save(GameState state)
    {
        var json = JsonConvert.SerializeObject(state);
        PlayerPrefs.SetString(Key, json);
        PlayerPrefs.Save();   // required on WebGL
    }

    public GameState Load()
    {
        if (!Exists()) return null;
        try
        {
            return JsonConvert.DeserializeObject<GameState>(PlayerPrefs.GetString(Key));
        }
        catch (JsonException ex)
        {
            Debug.LogError($"Save data corrupt: {ex.Message}");
            return null;
        }
    }

    public void Delete() { PlayerPrefs.DeleteKey(Key); PlayerPrefs.Save(); }
}
```

### Serializable GameState (plain C#)
```csharp
public sealed class GameState
{
    public int    Version      { get; set; } = 1;
    public string TeamName     { get; set; }
    public List<AthleteRecord> Athletes { get; set; } = new();
}

// No Vector3, no Color — use plain types:
public sealed class AthleteRecord
{
    public int    Id    { get; set; }
    public string Name  { get; set; }
    public float  Speed { get; set; }   // not a Unity type
}
```

### Schema migration (v1 → v2)
```csharp
// In PlayerPrefsSaveService.Load():
var raw = JObject.Parse(PlayerPrefs.GetString(Key));
int version = raw.Value<int>("Version");
if (version < 2)
    raw["NewField"] = "default_value";   // add field with default
return raw.ToObject<GameState>();
```

## Anti-patterns
- `PlayerPrefs.SetString("save", json)` directly in a screen class — use `ISaveService`.
- `PlayerPrefs.SetString("legends_save", ...)` without calling `PlayerPrefs.Save()` — data may be lost on WebGL tab close.
- Storing `Color primaryColor` as a `GameState` field — serialize as `float r, g, b, a` instead.
- Swallowing `JsonException` silently — callers will receive non-null corrupt data.

## Examples
See `examples/` for annotated code.
