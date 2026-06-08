# Legends

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
