#!/usr/bin/env bash
# Use 'set -e' to stop if any command fails, but we'll manage the Git/GH errors manually
set -e

# 1. Metadata extraction
csproj=$(find . -name "Dungeon-Adventures.csproj" | head -n 1)
version=$(sed -n "s/.*<Version>\(.*\)<\/Version>.*/\1/p" "$csproj" | tr -d "\r" | xargs)

# Color Definitions
CYAN='\033[0;36m'
MAGENTA='\033[0;35m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
GREEN='\033[0;32m'
RED='\033[0;31m'
GRAY='\033[0;90m'
NC='\033[0m' 
BG_CYAN='\033[46;30m'
BG_GREEN='\033[42;37m'

# Check if we are already in a loop
if [ "$GMA_INTERNAL_RUN" == "true" ]; then
    exit 0
fi
export GMA_INTERNAL_RUN="true"

# Health check - ensure check-health.sh doesn't call update.sh!
if [ -f "./check-health.sh" ]; then
    bash ./check-health.sh
fi

read -p "⚠️ PROCEED WITH UPLINK? (y/n): " confirm
if [[ $confirm != [yY] ]]; then
    echo -e "${RED}❌ UPLINK ABORTED BY User.${NC}"
    exit 1
fi

clear
echo -e "${CYAN}╔═══════════════════════════════════════════════════════╗${NC}"
echo -e "${CYAN}║${BG_CYAN}    ⚔️  DUNGEON-ADVENTURES // RELEASE v$version        ${NC}${CYAN}║${NC}"
echo -e "${CYAN}╚═══════════════════════════════════════════════════════╝${NC}"
echo ""

# 2. Build & Test (Changed 'pack' to 'publish' for a standalone game)
echo -ne " ${MAGENTA}[1/6] 🛠️  FORGING GAME BINARIES...      ${NC}"
# Publishing as a self-contained executable is better for games than a NuGet pack
dotnet publish -c Release -r win-x64 --self-contained true -o ./dist/win --nologo >/dev/null 2>&1
echo -e "${GREEN}ONLINE${NC}"

# 3. Git Sync
echo -ne " ${YELLOW}[2/6] 🧠 WRITING VENTURE MEMORY...     ${NC}"
git add . >/dev/null 2>&1
git commit -m "Venture Update: v$version" --allow-empty --quiet >/dev/null 2>&1 || true
git tag -f "v$version" -m "Dungeon Release v$version" >/dev/null 2>&1
echo -e "${GREEN}LOCALIZED${NC}"

# 4. Cloud Uplink
echo -ne " ${BLUE}[3/6] 🛰️  UPLINKING TO GITHUB...       ${NC}"
git push origin master --force --quiet >/dev/null 2>&1
git push origin "v$version" --force --quiet >/dev/null 2>&1
echo -e "${GREEN}STABLE${NC}"

# 5. Global Inject (Local Update)
echo -ne " ${CYAN}[4/6] 💉 INJECTING WORKSPACE...        ${NC}"
dotnet tool update -g --add-source ./dist Dungeon-Adventures --verbosity minimal >/dev/null 2>&1
echo -e "${GREEN}ACTIVE${NC}"

# 6. GitHub Release (Smart Fallback)
echo -ne " ${MAGENTA}[5/6] 🌐 GENERATING GLOBAL RELEASE...   ${NC}"
pkgPath="./dist/Dungeon-Adventures .$version.nupkg"
UPLINK_STATUS="SUCCESS"

if command -v gh &> /dev/null; then
    # Create or Upload Fallback
    if gh release create "v$version" "$pkgPath" --title "Multi-Tool v$version" --notes "Neural Update" --repo "kgbcupcake/GM-Architect" 2>/dev/null || \
       gh release upload "v$version" "$pkgPath" --repo "kgbcupcake/GM-Architect" --clobber >/dev/null 2>&1; then
        echo -e "${GREEN}PUBLISHED${NC}"
    else
        echo -e "${RED}FAIL${NC}"
        UPLINK_STATUS="FAILED"
    fi
else
    echo -e "${YELLOW}SKIPPED${NC}"
fi

# 7. Local Cleanup
echo -ne " ${GRAY}[6/6] 🧹 PURGING ARTIFACTS...           ${NC}"
sleep 2
rm -rf ./dist/*.nupkg
echo -e "${GREEN}CLEAN${NC}"

echo ""
echo -e "${CYAN}─────────────────────────────────────────────────────────${NC}"
if [ "$UPLINK_STATUS" == "FAILED" ]; then
    echo -e " ${RED} ❌ UPLINK INCOMPLETE: LOCAL UPDATED / CLOUD FAILED ${NC}"
    echo -e " ${YELLOW} Check 'gh auth status' to fix cloud connection.${NC}"
else
    echo -e " ${BG_GREEN} ✨ SUCCESS: LOGIC STATE STABLE [v$version] ✨ ${NC}"
    echo -e " ${GRAY} ⚡ Global command 'gma' is now optimized.${NC}"
fi
echo ""