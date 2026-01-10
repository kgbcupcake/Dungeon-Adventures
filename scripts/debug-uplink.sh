#!/usr/bin/env bash
# No 'set -e' - we want to see every crash point.

# 1. Metadata Extraction
csproj=$(find . -name "Dungeon-Adventures.csproj" | head -n 1)
version=$(grep -oP '(?<=<Version>).*?(?=</Version>)' "$csproj" | tr -d '\r' | xargs)

# Color Definitions
CYAN='\033[0;36m'
MAGENTA='\033[0;35m'
YELLOW='\033[1;33m'
GREEN='\033[0;32m'
RED='\033[0;31m'
NC='\033[0m'

clear
echo -e "${MAGENTA}🛠️  VENTURE DEBUG UPLINK INITIATED...${NC}"
echo -e "${CYAN}Targeting Version: v$version${NC}\n"

# 2. Forge Check
echo -e "${YELLOW}[1/3] 📦 TESTING GAME FORGE (WIN-X64)...${NC}"
# Removed 'quiet' so you see the actual compiler errors
dotnet publish -c Release -r win-x64 --self-contained true -o ./dist/win --nologo

if [ $? -eq 0 ]; then 
    echo -e "${GREEN}✅ FORGE SUCCESSFUL. Archiving assets...${NC}"
    zip -r "Dungeon-Adventures-v$version.zip" ./dist/win
else
    echo -e "${RED}❌ FORGE FAILED. Check the dotnet output above.${NC}"
fi

# 3. Connection Check
echo -e "\n${YELLOW}[2/3] 🌐 VERIFYING CLOUD PERMISSIONS...${NC}"
if ! command -v gh &> /dev/null; then
    echo -e "${RED}❌ ERROR: GitHub CLI (gh) not found.${NC}"
else
    gh auth status
fi

# 4. Uplink Test
echo -e "\n${YELLOW}[3/3] 🚀 ATTEMPTING RELEASE UPLOAD...${NC}"
pkgPath="./Dungeon-Adventures-v$version.zip"

if [ -f "$pkgPath" ]; then
    # Note: --clobber replaces any existing file with the same name
    gh release create "v$version" "$pkgPath" \
        --title "Debug: Venture v$version" \
        --notes "Automated Debug Uplink" \
        --repo "kgbcupcake/Dungeon-Adventures" \
        --clobber
    
    if [ $? -eq 0 ]; then
        echo -e "${GREEN}✨ UPLINK STABLE: Release is live on GitHub.${NC}"
    else
        echo -e "${RED}❌ UPLINK FAILED: Check your repository permissions.${NC}"
    fi
else
    echo -e "${RED}❌ ABORTED: Package not found at $pkgPath${NC}"
fi

echo -e "\n${MAGENTA}🏁 DIAGNOSTICS COMPLETE.${NC}"