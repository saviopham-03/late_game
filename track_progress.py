#!/usr/bin/env python3

import subprocess
import sys
from collections import defaultdict
from pathlib import Path

# ============================================================
# Configuration
# ============================================================

# File extensions to count.
# Add more if you want to track shaders, JavaScript, etc.
EXTENSIONS = {
    ".cs",
    # ".shader",
    # ".hlsl",
    # ".cginc",
}

# Directories to ignore.
IGNORED_DIRS = {
    ".git",
    "Library",
    "Temp",
    "Logs",
    "obj",
    "Build",
    "Builds",
    "UserSettings",
    "MemoryCaptures",
}

# Optional: set these to None to analyze the entire Git history.
SINCE = None
UNTIL = None

# ============================================================
# Git helpers
# ============================================================

def run_git(args):
    result = subprocess.run(
        ["git"] + args,
        stdout=subprocess.PIPE,
        stderr=subprocess.PIPE,
        text=True,
        encoding="utf-8",
    )

    if result.returncode != 0:
        print("Git error:")
        print(result.stderr)
        sys.exit(1)

    return result.stdout


def is_counted_file(path):
    p = Path(path)

    # Check extension.
    if p.suffix.lower() not in EXTENSIONS:
        return False

    # Check ignored directories.
    for part in p.parts:
        if part in IGNORED_DIRS:
            return False

    return True


# ============================================================
# Main analysis
# ============================================================

def main():
    print("Analyzing Git history...\n")

    # Make sure we're inside a Git repository.
    try:
        run_git(["rev-parse", "--is-inside-work-tree"])
    except SystemExit:
        print("This directory is not a Git repository.")
        sys.exit(1)

    args = [
        "log",
        "--all",
        "--numstat",
        "--format=AUTHOR:%aN",
    ]

    if SINCE:
        args.append(f"--since={SINCE}")

    if UNTIL:
        args.append(f"--until={UNTIL}")

    output = run_git(args)

    stats = defaultdict(lambda: {
        "added": 0,
        "deleted": 0,
        "files": set(),
    })

    current_author = None

    for line in output.splitlines():

        # New commit/author.
        if line.startswith("AUTHOR:"):
            current_author = line[7:].strip()
            continue

        if not current_author:
            continue

        parts = line.split("\t")

        # numstat normally looks like:
        # additions    deletions    filename
        if len(parts) != 3:
            continue

        added, deleted, filename = parts

        # Binary files sometimes show "-" instead of numbers.
        if not added.isdigit() or not deleted.isdigit():
            continue

        if not is_counted_file(filename):
            continue

        stats[current_author]["added"] += int(added)
        stats[current_author]["deleted"] += int(deleted)
        stats[current_author]["files"].add(filename)

    # ========================================================
    # Display results
    # ========================================================

    if not stats:
        print("No matching contributions found.")
        return

    results = []

    for author, data in stats.items():
        added = data["added"]
        deleted = data["deleted"]
        net = added - deleted
        files = len(data["files"])

        results.append(
            (author, added, deleted, net, files)
        )

    # Sort by lines added.
    results.sort(key=lambda x: x[1], reverse=True)

    print(
        f"{'Developer':<25}"
        f"{'Added':>12}"
        f"{'Deleted':>12}"
        f"{'Net':>12}"
        f"{'Files':>10}"
    )

    print("-" * 71)

    for author, added, deleted, net, files in results:
        print(
            f"{author:<25}"
            f"{added:>12,}"
            f"{deleted:>12,}"
            f"{net:>12,}"
            f"{files:>10}"
        )

    print("\nFiles counted:")
    for extension in sorted(EXTENSIONS):
        print(f"  {extension}")

    if SINCE:
        print(f"\nSince: {SINCE}")

    if UNTIL:
        print(f"Until: {UNTIL}")


if __name__ == "__main__":
    main()
