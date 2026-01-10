#!/usr/bin/env bash
set -e

# --- [PATH CALIBRATION] ---
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
ROOT_DIR="$(cd "$SCRIPT_DIR/.." && pwd)"

# Color Definitions
CYAN='\033[0;36m'
GREEN='\033[0;32m'
MAGENTA='\033[0;35m'
YELLOW='\033[1;33m'
GRAY='\033[0;90m'
RED='\033[0;31m'
NC='\033[0m'

# 1. Metadata extraction
csproj="$ROOT_DIR/Dungeon-Adventures.csproj"
if [ ! -f "$csproj" ]; then
    echo -e "${RED}❌ ERROR: Dungeon-Adventures.csproj not found.${NC}"
    exit 1
fi

# Extract current version
current_version=$(grep -m 1 -oP '(?<=<Version>).*?(?=</Version>)' "$csproj" | tr -d '\r' | xargs)

echo -e "${MAGENTA}🧠 VENTURE VERSION BUMPER INITIATED...${NC}"
echo -e "${CYAN}  » Current State: v$current_version${NC}"

# 2. Safety Check: Don't bump if the workspace is a mess
cd "$ROOT_DIR"
if [[ -n $(git status -s | grep -v "Dungeon-Adventures.csproj") ]]; then
    echo -e "${YELLOW}⚠️  WARNING: You have other uncommitted changes.${NC}"
    echo -e "${GRAY}It is recommended to commit your game logic before bumping versions.${NC}"
    read -p "Continue anyway? (y/n): " cont
    if [[ $cont != [yY] ]]; then exit 1; fi
fi

# 3. Logic: Incrementing the Patch
IFS='.' read -r major minor patch <<< "$current_version"
new_patch=$((patch + 1))
new_version="$major.$minor.$new_patch"

# 4. Write to File
sed -i "s/<Version>$current_version<\/Version>/<Version>$new_version<\/Version>/" "$csproj"

# 5. --- [THE MISSING LINK] ---
# We must commit this change so the 'update.sh' script passes the Git Guard
echo -ne " ${CYAN}📝 [GIT] LOGGING VERSION EVOLUTION... ${NC}"
git add "$csproj" > /dev/null
git commit -m "release: evolve to v$new_version" --quiet > /dev/null
echo -e "${GREEN}COMMITTED${NC}"

# 6. Final Report
echo -e "\n${GREEN}✨ SUCCESS: Venture evolved to v$new_version${NC}"
echo -e "${MAGENTA}🚀 READY FOR UPLINK.${NC}"
echo -e "${GRAY}---------------------------------------------------------${NC}"