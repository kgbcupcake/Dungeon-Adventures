#!/usr/bin/env bash
set -e

# --- [PATH CALIBRATION] ---
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
ROOT_DIR="$(cd "$SCRIPT_DIR/.." && pwd)"

# Color Definitions
CYAN='\033[0;36m'
GREEN='\033[0;32m'
MAGENTA='\033[0;35m'
GRAY='\033[0;90m'
NC='\033[0m'

# 1. Metadata extraction - Hardcoded to the Reality Anchor
csproj="$ROOT_DIR/Dungeon-Adventures.csproj"

if [ ! -f "$csproj" ]; then
    echo -e "\033[0;31m❌ ERROR: Dungeon-Adventures.csproj not found at $ROOT_DIR\033[0m"
    exit 1
fi

# This ensures we only grab the FIRST match to avoid the double-version bug
current_version=$(grep -m 1 -oP '(?<=<Version>).*?(?=</Version>)' "$csproj" | tr -d '\r' | xargs)

echo -e "${MAGENTA}🧠 VENTURE VERSION BUMPER INITIATED...${NC}"
echo -e "${CYAN}Current Version: v$current_version${NC}"

# 2. Split version into parts
IFS='.' read -r major minor patch <<< "$current_version"

# 3. Increment patch
new_patch=$((patch + 1))
new_version="$major.$minor.$new_patch"

# 4. Write back to .csproj
sed -i "s/<Version>$current_version<\/Version>/<Version>$new_version<\/Version>/" "$csproj"

echo -e "${GREEN}✨ SUCCESS: Venture evolved to v$new_version${NC}"
echo -e "${GRAY}Note: Run 'da-ship' to uplink this version.${NC}"