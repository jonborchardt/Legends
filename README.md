# Legends

Olympic Manager MVP — a WebGL game on GitHub Pages. Player manages a fantasy athletic team and watches a 3D replay of a simulated Dragon Egg Relay event.

No backend. No multiplayer. No cloud saves. All state in browser localStorage.

## Deployment Setup (one-time)

Unity Personal licenses can't be exported, so CI activation requires a manual flow:

- [ ] **Step 1** — Add three secrets in repo Settings → Secrets and variables → Actions:
  - `UNITY_EMAIL` — your Unity account email
  - `UNITY_PASSWORD` — your Unity account password
- [ ] **Step 2** — Go to repo Settings → Pages → Source: **Deploy from a branch** → Branch: `gh-pages` / `/ (root)` → Save

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
