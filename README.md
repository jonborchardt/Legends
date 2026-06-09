# Legends

Olympic Manager MVP — a WebGL game on GitHub Pages. Player manages a fantasy athletic team and watches a 3D replay of a simulated Dragon Egg Relay event.

No backend. No multiplayer. No cloud saves. All state in browser localStorage.

## Development

The project uses **Programmatic Scene Construction (PSC)**: all UI and 3D objects are built entirely in C#. The Unity Editor is used only to run Play mode and build WebGL — never to place objects or wire up inspectors.

**To change any UI or game object:**
1. Edit the relevant `*Screen.cs`, `*Factory.cs`, or generator script
2. Hit Play — changes are live immediately

**Generated artifacts** (AnimatorController, etc.) rebuild automatically whenever scripts recompile and before every build. To force a rebuild: `Tools > Legends > Build All`. To change animation states, edit `Assets/Editor/AthleteControllerGenerator.cs` — the controller regenerates on the next compile.

**Key entry points:**

| What | Where |
|---|---|
| Main menu, team hub, results screens | `Assets/Scripts/UI/Screens/` |
| UI layout helpers (buttons, panels, etc.) | `Assets/Scripts/UI/UIFactory.cs` |
| Colours and font sizes | `Assets/Scripts/UI/UIStyle.cs` |
| 3D athlete visual creation | `Assets/Scripts/Replay/AthleteFactory.cs` |
| Race simulation | `Assets/Scripts/Simulation/DragonEggRelaySimulator.cs` |
| Scene/screen navigation | `Assets/Scripts/Services/SceneFlow.cs` |

---

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
