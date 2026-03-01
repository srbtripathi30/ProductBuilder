#!/usr/bin/env bash
# post-edit.sh
# Runs after every Edit or Write tool call.
# - Updates "Last Modified" date in CLAUDE.md
# - Prints a reminder if a backend (.cs) file was changed and dotnet watch is not running

set -euo pipefail

PROJECT_ROOT="/Users/saurabhkumar/Docs/Projects/ProductBuilder"
CLAUDE_MD="$PROJECT_ROOT/CLAUDE.md"

# Parse file_path from tool-input JSON on stdin
INPUT=$(cat)
FILE_PATH=$(echo "$INPUT" | jq -r '.file_path // ""' 2>/dev/null || true)

[ -z "$FILE_PATH" ] && exit 0

# ── 1. Update Last Modified date in CLAUDE.md ─────────────────────────────────
TODAY=$(date +"%Y-%m-%d")

if grep -q "^> Last modified:" "$CLAUDE_MD" 2>/dev/null; then
  sed -i '' "s|^> Last modified:.*|> Last modified: $TODAY|" "$CLAUDE_MD"
fi

# ── 2. Backend change → warn if dotnet watch is not running ───────────────────
if [[ "$FILE_PATH" == *"ProductBuilder.API"* && "$FILE_PATH" == *.cs ]]; then
  if ! pgrep -f "dotnet.*watch" > /dev/null 2>&1; then
    echo "⚠️  Backend file changed: $(basename "$FILE_PATH")"
    echo "   Server is NOT running with 'dotnet watch run'."
    echo "   Start it with:"
    echo "     cd ProductBuilder.API/src/ProductBuilder.API && dotnet watch run --urls http://localhost:5000"
    echo "   (Auto-restarts on every .cs change — no manual restarts needed)"
  fi
fi
