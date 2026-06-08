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

## Deployment Setup (one-time)

Unity Personal licenses can't be exported, so CI activation requires a manual flow:

- [ ] **Step 1** — Commit and push `.github/workflows/activate.yml` and `.github/workflows/deploy.yml` to `main`
- [ ] **Step 2** — Go to repo → Actions → "Acquire Activation File" → Run workflow → download the `.alf` artifact
- [ ] **Step 3** — Go to [license.unity3d.com/manual](https://license.unity3d.com/manual), upload the `.alf`, download the `.ulf` file
- [ ] **Step 4** — Add three secrets in repo Settings → Secrets and variables → Actions:
  - `UNITY_LICENSE` — paste the full contents of the `.ulf` file
  - `UNITY_EMAIL` — your Unity account email
  - `UNITY_PASSWORD` — your Unity account password
- [ ] **Step 5** — Go to repo Settings → Pages → Source: **Deploy from a branch** → Branch: `gh-pages` / `/ (root)` → Save

After Step 4, any push to `main` will build and deploy automatically. `activate.yml` can be deleted after Step 3.

---

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
