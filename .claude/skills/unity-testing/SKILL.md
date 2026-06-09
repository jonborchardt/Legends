---
trigger: Writing or reviewing any test file; "how do I test X"; adding simulation logic, data transforms, or save/load; CI setup for Unity projects.
---
# unity-testing

## Purpose
Testing strategy for a PSC Unity project: pure C# tests for simulation and data transformation (NUnit, no Unity runner), PlayMode tests only when Unity lifecycle is the actual subject, EditMode tests for editor-only logic.

## Rules (enforce)
- Simulation (`DragonEggRelaySimulator`, `EventConfigFactory`) is tested with plain NUnit `[Test]` — no `[UnityTest]`, no `IEnumerator`.
- Test assemblies have their own `.asmdef` with `testPlatforms` specified: `EditMode` for pure C# logic, `PlayMode` only when truly needed.
- Tests are deterministic — seed any RNG under test; never depend on `Time.time` or wall clock.
- Test data is constructed inline or via builders — no JSON fixtures unless testing serialization itself.
- Each test class maps to one system under test — no catch-all `Tests.cs`.

## Rules (avoid)
- `[UnityTest]` / `IEnumerator` for logic that doesn't need a frame boundary.
- `MonoBehaviour` instances in tests for non-lifecycle logic — extract to plain C# and test that.
- `Resources.Load` in tests — inject dependencies; don't hit the asset database.
- PlayMode tests for UI layout correctness — too brittle, too slow.
- `Assert.IsTrue(x == "foo")` — use `Assert.AreEqual("foo", x)` for useful failure messages.

## Patterns

### Pure NUnit test (no Unity runner)
```csharp
[TestFixture]
public sealed class DragonEggRelaySimulatorTests
{
    [Test]
    public void Simulate_ReturnsOneResultPerAthlete()
    {
        var config = EventConfigFactory.CreateDefault();
        var team   = TeamBuilder.WithAthletes(4);
        var rng    = new System.Random(42);   // seeded — deterministic

        var result = new DragonEggRelaySimulator(rng).Simulate(config, team);

        Assert.AreEqual(4, result.AthleteResults.Count);
    }
}
```

### Test data builder pattern
```csharp
public static class TeamBuilder
{
    public static TeamData WithAthletes(int count)
    {
        var athletes = Enumerable.Range(0, count)
            .Select(i => new AthleteData(id: i, name: $"Athlete{i}", speed: 1f))
            .ToList();
        return new TeamData(athletes);
    }
}
```

### Serialization round-trip test
```csharp
[Test]
public void SaveLoad_RoundTrips_GameState()
{
    var original = new GameState { TeamName = "Blazers", Version = 1 };
    var svc      = new PlayerPrefsSaveService();

    svc.Save(original);
    var loaded = svc.Load();

    Assert.AreEqual("Blazers", loaded.TeamName);
    Assert.AreEqual(1, loaded.Version);
}
```

### Test assembly .asmdef
```json
{
  "name": "Legends.Tests.EditMode",
  "references": ["Legends.Data", "Legends.Simulation", "Legends.Services"],
  "includePlatforms": ["Editor"],
  "optionalUnityReferences": ["TestAssemblies"]
}
```

## Test pyramid
```
Unit (pure C#, NUnit)     — simulation, data transforms, serialization, migration
     ↓ most tests here
EditMode integration      — editor-only tools (generators, build pipeline)
     ↓ few tests here
PlayMode                  — only when Unity lifecycle (Awake/Start/coroutine) is the subject
     ↓ rarely here
```

## What NOT to test
- UI layout output from factory methods — verify via code review.
- Generated `.controller` / `.asset` artifacts — the generator itself is the spec.
- `SceneFlow` transitions — integration test via manual play testing.

## Anti-patterns
- `yield return new WaitForSeconds(1f);` in a simulator test — seed RNG and call synchronously.
- `[UnityTest] IEnumerator TestSave()` — `PlayerPrefsSaveService` is pure C#; use `[Test]`.
- `Assert.IsTrue(result.Count == 4)` — use `Assert.AreEqual(4, result.Count)` so NUnit shows expected vs. actual on failure.

## Examples
See `examples/` for annotated snippets.
