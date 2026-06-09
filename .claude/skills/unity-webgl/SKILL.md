---
trigger: Always when the build target is WebGL; any async/await, threading, or native plugin question; "why does X work in editor but not in the browser"; build size or texture compression questions; anything touching file I/O or Application.persistentDataPath.
---
# unity-webgl

## Purpose
Hard platform constraints for WebGL builds. These are non-negotiable overrides that any other skill must defer to. Covers threading, async, memory, browser I/O, build settings, and GitHub Pages deployment.

## Rules (enforce)
- No `Thread`, `Task.Run`, or `Parallel.*` — WebGL is single-threaded.
- `async/await` only on the main thread; use `UnityWebRequest` or Addressables for async I/O, not `System.IO`.
- No `System.IO.File.*` — use `PlayerPrefs` or IndexedDB via JS interop.
- Texture format must be `DXT1`/`DXT5` (desktop) or `ETC2` (mobile) — uncompressed textures balloon download size.
- `Application.platform == RuntimePlatform.WebGLPlayer` guards for any platform-specific path.
- `Addressables.WaitForCompletion()` is **forbidden** on WebGL — always `await` the handle.
- `UnityEngine.Networking.UnityWebRequest` for any HTTP; no `HttpClient`.

## Rules (avoid)
- `System.Threading.Thread` — silently no-ops or crashes on WebGL.
- `System.IO.File`, `Directory`, `Path.GetTempPath`.
- Any native (`.dll`/`.so`) plugin not built for WebGL's Emscripten target.
- `Debug.Log` spam in builds — it crosses the JS boundary and has real overhead; strip in release.
- `WWW` (deprecated) — use `UnityWebRequest`.
- `Task.Run(() => heavyCompute())` — runs synchronously on WebGL, blocking the frame.

## Patterns

### Correct async on WebGL main thread
```csharp
// Coroutine-based (safest, widest Unity version support)
private IEnumerator LoadAthleteAssets()
{
    var handle = Addressables.LoadAssetAsync<GameObject>(AddressKeys.AthleteCharacter);
    yield return handle;
    if (handle.Status != AsyncOperationStatus.Succeeded) yield break;
    SpawnAthlete(handle.Result);
}

// async/await variant (Unity 2021+, main thread only)
private async void LoadAthleteAssetsAsync()
{
    var handle = Addressables.LoadAssetAsync<GameObject>(AddressKeys.AthleteCharacter);
    await handle.Task;                      // awaits on main thread — OK
    if (handle.Status != AsyncOperationStatus.Succeeded) return;
    SpawnAthlete(handle.Result);
}
```

### JS interop (calling JS from C#)
```csharp
// In a .jslib plugin:
// mergeInto(LibraryManager.library, { LogEvent: function(name) { console.log(name); } });

[DllImport("__Internal")]
private static extern void LogEvent(string eventName);

private void TrackStart()
{
#if UNITY_WEBGL && !UNITY_EDITOR
    LogEvent("replay_started");
#endif
}
```

### File I/O alternative
```csharp
// No System.IO on WebGL. Use PlayerPrefs for small data.
PlayerPrefs.SetString("legends_save_v1", json);
PlayerPrefs.Save();   // must call explicitly on WebGL
```

## Build settings checklist
- Compression: Brotli (requires server headers) or Gzip
- Texture compression: DXT for desktop WebGL
- Code stripping level: Medium or High
- Strip engine code: enabled
- Exception support: None (performance) or Explicitly Thrown Only
- `Development Build` off for release

## GitHub Pages deployment
```yaml
# .github/workflows/deploy.yml
- name: Deploy to GitHub Pages
  uses: JamesIves/github-pages-deploy-action@v4
  with:
    folder: Build/WebGL/WebGL   # Unity build output folder
    branch: gh-pages
```
Ensure `index.html` is at repo root or configure `baseUrl` in GitHub Pages settings.

## Debugging browser-only issues
- Open browser DevTools Console — Unity logs appear there.
- Network tab: check asset load failures (404s for Addressables catalog, etc.).
- Memory tab: profile if the tab crashes — usually compressed textures or large AudioClips.
- `UNITY_WEBGL && !UNITY_EDITOR` guards let you add browser-only diagnostics.

## Anti-patterns
- `new Thread(() => Simulate()).Start()` — freezes or crashes in browser.
- `File.WriteAllText(path, json)` — throws `NotSupportedException` on WebGL.
- `handle.WaitForCompletion()` — blocks the JS event loop; tab appears frozen.
- Uncompressed `Texture2D` assets in WebGL build — 4 MB texture → 4 MB download.
