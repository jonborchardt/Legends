#!/usr/bin/env python3
"""Simple local server for Unity WebGL builds.
Run: python serve.py
Then open: http://localhost:8080
"""

import http.server
import os
import sys

PORT = 8080
BUILD_DIR = os.path.join(os.path.dirname(__file__), "Build")


class UnityHandler(http.server.SimpleHTTPRequestHandler):
    def __init__(self, *args, **kwargs):
        super().__init__(*args, directory=BUILD_DIR, **kwargs)

    def end_headers(self):
        # Required for Unity WebGL SharedArrayBuffer support
        self.send_header("Cross-Origin-Opener-Policy", "same-origin")
        self.send_header("Cross-Origin-Embedder-Policy", "require-corp")
        super().end_headers()

    def log_message(self, format, *args):
        print(f"  {self.address_string()} {format % args}")


if __name__ == "__main__":
    port = int(sys.argv[1]) if len(sys.argv) > 1 else PORT
    os.chdir(BUILD_DIR)
    with http.server.HTTPServer(("", port), UnityHandler) as httpd:
        print(f"Serving Unity build at http://localhost:{port}")
        print("Press Ctrl+C to stop.\n")
        try:
            httpd.serve_forever()
        except KeyboardInterrupt:
            print("\nStopped.")
