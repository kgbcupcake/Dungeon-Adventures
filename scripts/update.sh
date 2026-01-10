#!/usr/bin/env bash
set -e

# --- [PATH CALIBRATION] ---
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
ROOT_DIR="$(cd "$SCRIPT_DIR/.." && pwd)"
csproj=$(find "$ROOT_DIR" -name "Dungeon-Adventures.csproj" | head -n 1)
version=$(grep -oP '(?<=<Version>).*?(?=</Version>)' "$csproj" | tr -d '\r' | xargs)

# 1. Metadata extraction
csproj=$(find "$ROOT_DIR" -name "Dungeon-Adventures.csproj" | head -n 1)
version=$(grep -oP '(?<=<Version>).*?(?=</Version>)' "$csproj" | tr -d '\r' | xargs)

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

# --- [PHASE 0: SYSTEM DIAGNOSTICS] ---
clear
echo -e "${CYAN}🔍 ANALYZING VENTURE ENVIRONMENT...${NC}"
sleep 0.5

# Check for required tools
for tool in dotnet git gh zip; do
    if ! command -v $tool &> /dev/null; then
        echo -e "${RED}❌ CRITICAL FAILURE: $tool is not installed.${NC}"
        exit 1
    fi
    echo -e "  ${GRAY}» $tool: ${GREEN}ONLINE${NC}"
done

# Environment variables check
echo -e "  ${GRAY}» ROOT_DIR: ${CYAN}$ROOT_DIR${NC}"
echo -e "  ${GRAY}» TARGET_VERSION: ${MAGENTA}v$version${NC}"

# --- [PHASE A: VENTURE PRE-FLIGHT] ---

cd "$ROOT_DIR"

# 1. Git Guard (Safety Check)
echo -ne "\n${CYAN}🛡️  [GUARD] CHECKING FOR UNSTABLE CODE... ${NC}"
if [[ -n $(git status -s) ]]; then
    echo -e "${RED}UNSTABLE${NC}"
    echo -e "${YELLOW}---------------------------------------------------------${NC}"
    git status -s
    echo -e "${YELLOW}---------------------------------------------------------${NC}"
    echo -e "${RED}❌ UPLINK BLOCKED: You have uncommitted changes.${NC}"
    exit 1
fi
echo -e "${GREEN}SECURE${NC}"

# 2. Integrity & Health Check
if [ -f "$SCRIPT_DIR/check-health.sh" ]; then
    echo -e "${CYAN}🏥 [HEALTH] RUNNING NUNIT DIAGNOSTICS...${NC}"
    bash "$SCRIPT_DIR/check-health.sh"
fi

# 3. Version Collision Check (GitHub)
echo -ne "${CYAN}🛰️  [CLOUD] SCANNING FOR VERSION COLLISIONS... ${NC}"
if gh release view "v$version" --repo "kgbcupcake/Dungeon-Adventures" &>/dev/null; then
    echo -e "${YELLOW}COLLISION DETECTED${NC}"
    echo -e "${YELLOW}⚠️  v$version already exists on GitHub.${NC}"
    read -p "Overwrite existing release assets and notes? (y/n): " overwrite
    if [[ $overwrite != [yY] ]]; then exit 1; fi
else
    echo -e "${GREEN}CLEAR${NC}"
fi

# 4. Venture Log Preview (The Spicy Menu)
if [ -f "$ROOT_DIR/CHANGELOG.md" ]; then
    echo -e "\n${CYAN}╔═══════════════════════════════════════════════════════╗${NC}"
    echo -e "${CYAN}║              📡 VENTURE LOG PREVIEW (v$version)           ║${NC}"
    echo -e "${CYAN}╚═══════════════════════════════════════════════════════╝${NC}"
    if command -v glow &> /dev/null; then 
        glow "$ROOT_DIR/CHANGELOG.md"
    else 
        echo -e "${GRAY}"
        cat "$ROOT_DIR/CHANGELOG.md"
        echo -e "${NC}"
    fi
    echo -e "${CYAN}─────────────────────────────────────────────────────────${NC}"
    read -p "Proceed with this Changelog? (y/n): " log_confirm
    if [[ $log_confirm != [yY] ]]; then exit 1; fi
fi

# 5. README Sync
if [ -f "$ROOT_DIR/README.md" ]; then
    echo -ne "📄 [SYNC] UPDATING README VERSION BADGE... "
    sed -i "s/version-v[0-9.]*-cyan/version-v$version-cyan/g" "$ROOT_DIR/README.md"
    echo -e "${GREEN}DONE${NC}"
fi

# 6. Final Uplink Confirmation
echo ""
read -p "⚠️  PROCEED WITH FULL VENTURE UPLINK? (y/n): " confirm
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

# 1. FORGE: WINDOWS (Installer)
echo -ne " ${MAGENTA}[1/5] 🛠️  FORGING WINDOWS ENGINE...      ${NC}"
dotnet publish "$csproj" -c Release -r win-x64 --self-contained true -o ./dist/win --nologo >/dev/null 2>&1
if command -v iscc &> /dev/null; then
    iscc "$ROOT_DIR/installer.iss" /DAppVersion="$version" >/dev/null 2>&1
    winPkg="./Dungeon-Adventures-v$version-Installer.exe"
    mv "$ROOT_DIR/dist/Output/setup.exe" "$winPkg"
    echo -e "${GREEN}EXE READY${NC}"
else
    cd ./dist/win && zip -r "../../Dungeon-Adventures-v$version-Win.zip" . >/dev/null 2>&1 && cd ../..
    winPkg="./Dungeon-Adventures-v$version-Win.zip"
    echo -e "${YELLOW}ZIP READY${NC}"
fi

# 2. FORGE: LINUX (Portable)
echo -ne " ${MAGENTA}[2/5] 🐧 FORGING LINUX ENGINE...        ${NC}"
dotnet publish "$csproj" -c Release -r linux-x64 --self-contained true -o ./dist/linux --nologo >/dev/null 2>&1
tar -czf "Dungeon-Adventures-v$version-Linux.tar.gz" -C ./dist/linux .
linPkg="./Dungeon-Adventures-v$version-Linux.tar.gz"
echo -e "${GREEN}TAR.GZ READY${NC}"

# 3. Git Sync
echo -ne " ${YELLOW}[3/4] 🧠 WRITING VENTURE MEMORY...      ${NC}"
git add . >/dev/null 2>&1
git commit -m "Venture Update: v$version" --quiet >/dev/null 2>&1 || true
git tag -f "v$version" -m "Game Release v$version" >/dev/null 2>&1
echo -e "${GREEN}LOCALIZED${NC}"

# --- [PHASE A: VENTURE PRE-FLIGHT] ---
# 2. Integrity & Health Check (Triggers your 100+ lines of logic)
if [ -f "$SCRIPT_DIR/check-health.sh" ]; then
    echo -e "${CYAN}🏥 [HEALTH] RUNNING DIAGNOSTICS...${NC}"
    bash "$SCRIPT_DIR/check-health.sh" || exit 1
fi


# 4. CLOUD UPLINK (The Multi-Asset Release)
echo -ne " ${BLUE}[4/5] 🛰️  UPLINKING TO GITHUB...        ${NC}"
gh release create "v$version" "$winPkg" "$linPkg" \
    --title "Venture v$version" \
    --notes-file CHANGELOG.md \
    --repo "kgbcupcake/Dungeon-Adventures" \
    --clobber >/dev/null 2>&1
echo -e "${GREEN}STABLE${NC}"

# --- [PHASE C: FINALIZATION] ---

echo ""
echo -e "${CYAN}─────────────────────────────────────────────────────────${NC}"
echo -e " ${BG_GREEN} ✨ SUCCESS: VENTURE STATE STABLE [v$version] ✨ ${NC}"
echo -e " ${GRAY}⚡ Build is archived and live on GitHub.${NC}"
echo -e " ${GRAY}⚡ Current Binary: $pkgPath${NC}"
echo -e "${CYAN}─────────────────────────────────────────────────────────${NC}"

# Cleanup
echo -ne " 🧹 [CLEANUP] PURGING ARTIFACTS... "
if [ -d "./dist" ]; then rm -rf ./dist; fi
# Optional: remove local zip/exe if you only want it on GitHub
# rm -f ./*.zip ./*.exe 
echo -e "${GREEN}CLEAN${NC}\n"