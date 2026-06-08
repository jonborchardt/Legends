# Legends

Olympic Manager MVP — a WebGL game on GitHub Pages. Player manages a fantasy athletic team and watches a 3D replay of a simulated Dragon Egg Relay event.

No backend. No multiplayer. No cloud saves. All state in browser localStorage.

## Implementation Roadmap

All planned work lives in [Plans/](Plans/). Start with [Plans/MASTER.md](Plans/MASTER.md) for the full task index, dependency graph, and current status.

```
Plans/MASTER.md              ← start here
Plans/1-Foundation.md        ← chunk overviews
Plans/1.1.1-Create-Directories.md  ← atomic task specs (implement from these)
```

Chunks in order: Foundation → Data Layer → Simulation → Replay & Animation → UI & Screens → Deployment.

## Running the WebGL Build Locally

Unity WebGL builds require a web server (not a file:// URL) due to browser security restrictions around `SharedArrayBuffer`.

**Prerequisites:** Python 3

```bash
python serve.py
```

Then open [http://localhost:8080](http://localhost:8080) in your browser.

To use a different port:

```bash
python serve.py 3000
```

The server sets the required `Cross-Origin-Opener-Policy` and `Cross-Origin-Embedder-Policy` headers so the build loads correctly.
