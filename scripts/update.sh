#!/usr/bin/env bash
set -e

# --- [PATH CALIBRATION] ---
# Ensures the script knows where it is and where the root project is
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
ROOT_DIR="$(cd "$SCRIPT_DIR/.." && pwd)"

# 1. Metadata extraction (Looking for Dungeon-Adventures project)
csproj=$(find "$ROOT_DIR" -name "Dungeon-Adventures.csproj" | head -n 1)
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

# --- [PHASE A: VENTURE PRE-FLIGHT] ---

# 1. Git Guard (Safety Check)
cd "$ROOT_DIR"
if [[ -n $(git status -s) ]]; then
    echo -e "${RED}❌ GIT GUARD: Uncommitted changes detected.${NC}"
    git status -s
    echo -e "${YELLOW}Please commit your changes before shipping.${NC}"
    exit 1
fi

# 2. Integrity & Health Check
# Ensure this script exists in your /scripts folder to run your NUnit tests
if [ -f "$SCRIPT_DIR/check-health.sh" ]; then
    bash "$SCRIPT_DIR/check-health.sh"
fi

# 3. Version Collision Check (GitHub)
if command -v gh &> /dev/null; then
    if gh release view "v$version" --repo "kgbcupcake/Dungeon-Adventures" &>/dev/null; then
        echo -e "${YELLOW}⚠️  COLLISION: v$version already exists on GitHub.${NC}"
        read -p "Overwrite existing release assets and notes? (y/n): " overwrite
        if [[ $overwrite != [yY] ]]; then
            echo -e "${RED}❌ UPLINK ABORTED. Update your .csproj version first.${NC}"
            exit 1
        fi
    fi
fi

# 4. Venture Log Preview (Changelog)
if [ -f "$ROOT_DIR/CHANGELOG.md" ]; then
    echo -e "${CYAN}╔═══════════════════════════════════════════════════════╗${NC}"
    echo -e "${CYAN}║             📡 VENTURE LOG PREVIEW (v$version)           ║${NC}"
    echo -e "${CYAN}╚═══════════════════════════════════════════════════════╝${NC}"
    if command -v glow &> /dev/null; then 
        glow "$ROOT_DIR/CHANGELOG.md"
    else 
        cat "$ROOT_DIR/CHANGELOG.md"
    fi
    echo -e "${CYAN}─────────────────────────────────────────────────────────${NC}"
    read -p "Proceed with this Changelog? (y/n): " log_confirm
    if [[ $log_confirm != [yY] ]]; then exit 1; fi
fi

# 5. Integrated README Sync (Badge Update)
if [ -f "$ROOT_DIR/README.md" ]; then
    echo -ne " 📄 [SYNC] UPDATING README VERSION BADGE... "
    # Updates the shields.io badge version
    sed -i "s/version-v[0-9.]*-cyan/version-v$version-cyan/g" "$ROOT_DIR/README.md"
    echo -e "${GREEN}DONE${NC}"
fi

# 6. Final Uplink Confirmation
read -p "⚠️ PROCEED WITH VENTURE UPLINK? (y/n): " confirm
if [[ $confirm != [yY] ]]; then
    echo -e "${RED}❌ UPLINK ABORTED BY USER.${NC}"
    exit 1
fi

clear
echo -e "${CYAN}╔═══════════════════════════════════════════════════════╗${NC}"
echo -e "${CYAN}║${BG_CYAN}    ⚔️  DUNGEON-ADVENTURES // VENTURE UPLINK // v$version  ${NC}${CYAN}║${NC}"
echo -e "${CYAN}╚═══════════════════════════════════════════════════════╝${NC}"
echo ""

# --- [PHASE B: UPLINK SEQUENCE] ---

# Ensure we are in Root for dotnet and git commands
cd "$ROOT_DIR"

# 1. Build & Forge (Publishing as a standalone app)
echo -ne " ${MAGENTA}[1/4] 🛠️  FORGING GAME ENGINE...       ${NC}"
# We use publish instead of pack because this is a game, not a tool
dotnet publish -c Release -r win-x64 --self-contained true -o ./dist/win --nologo >/dev/null 2>&1
echo -e "${GREEN}ONLINE${NC}"

# 2. Package for Distribution (Zipping the folder)
echo -ne " ${MAGENTA}[2/4] 📦 COMPRESSING WORLD ASSETS...   ${NC}"
# This creates the zip file that players actually download
cd ./dist/win && zip -r "../../Dungeon-Adventures-v$version.zip" . >/dev/null 2>&1 && cd ../..
echo -e "${GREEN}ZIPPED${NC}"

# 3. Git Sync
echo -ne " ${YELLOW}[3/4] 🧠 WRITING VENTURE MEMORY...     ${NC}"
git add . >/dev/null 2>&1
git commit -m "Venture Update: v$version" --quiet >/dev/null 2>&1 || true
git tag -f "v$version" -m "Game Release v$version" >/dev/null 2>&1
echo -e "${GREEN}LOCALIZED${NC}"

# 4. Cloud Uplink
echo -ne " ${BLUE}[4/4] 🛰️  UPLINKING TO GITHUB...       ${NC}"
git push origin master --force --quiet >/dev/null 2>&1
git push origin "v$version" --force --quiet >/dev/null 2>&1
echo -e "${GREEN}STABLE${NC}"

# 5. GitHub Release (Using Changelog Logic)
echo -ne " 🌐 [RELEASE] GENERATING GLOBAL RELEASE... "
pkgPath="./Dungeon-Adventures-v$version.zip"
UPLINK_STATUS="SUCCESS"

if [ -f "CHANGELOG.md" ]; then
    NOTES_ARG="--notes-file CHANGELOG.md"
else
    NOTES_ARG="--notes 'Venture Update: v$version'"
fi

if command -v gh &> /dev/null; then
    if gh release create "v$version" "$pkgPath" --title "Venture v$version" $NOTES_ARG --repo "kgbcupcake/Dungeon-Adventures" 2>/dev/null || \
       gh release upload "v$version" "$pkgPath" --repo "kgbcupcake/Dungeon-Adventures" --clobber >/dev/null 2>&1; then
        echo -e "${GREEN}PUBLISHED${NC}"
    else
        echo -e "${RED}FAIL${NC}"
        UPLINK_STATUS="FAILED"
    fi
else
    echo -e "${YELLOW}SKIPPED (GH CLI NOT FOUND)${NC}"
fi

# 6. Final Status Report
echo ""
echo -e "${CYAN}─────────────────────────────────────────────────────────${NC}"
if [ "$UPLINK_STATUS" == "SUCCESS" ]; then
    echo -e " ${BG_GREEN} ✨ SUCCESS: VENTURE STATE STABLE [v$version] ✨ ${NC}"
    echo -e " ${GRAY}⚡ Build is archived and live on GitHub.${NC}"
else
    echo -e " ${RED} ⚠️  UPLINK INCOMPLETE: LOCAL UPDATED / CLOUD FAILED ${NC}"
fi
echo ""

# 7. Local Cleanup
echo -ne " 🧹 [CLEANUP] PURGING ARTIFACTS...        "
sleep 2 
if [ -d "./dist" ]; then
    rm -rf ./dist
    rm -f ./*.zip
fi
echo -e "${GREEN}CLEAN${NC}"
echo ""