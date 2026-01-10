#!/bin/bash

# --- 💠 NEURAL UPLINK: PROMETHEUS PROTOCOL ---
# Project: Dungeon Adventures | Author: kgbcupcake
# Status: Production Grade

# Professional Color Palette
CYAN='\033[0;36m'
GOLD='\033[0;33m'
GREEN='\033[0;32m'
RED='\033[0;31m'
GRAY='\033[0;90m'
NC='\033[0m' 

# 1. THE LOGO
clear
echo -e "${GOLD}"
echo "  ⚔️  DUNGEON-ADVENTURES : UPLINK"
echo "  ------------------------------"
echo -e "${NC}"

# 2. PRE-FLIGHT DIAGNOSTICS
echo -ne "${CYAN}[SYSTEM] Running Logic Guard... ${NC}"
./scripts/check-health.sh > /dev/null 2>&1

if [ $? -eq 0 ]; then
    echo -e "${GREEN}PASS${NC}"
else
    echo -e "${RED}FAIL${NC}"
    echo -e "${RED}>> Aborting uplink: NUnit diagnostics detected a fault.${NC}"
    exit 1
fi

# 3. BUILD & PACK
echo -ne "${CYAN}[SYSTEM] Forging Binaries...    ${NC}"
./scripts/update.sh > /dev/null 2>&1
echo -e "${GREEN}DONE${NC}"

# 4. SMART COMMIT
echo -e "${GRAY}------------------------------${NC}"
git status -s
echo -e "${GRAY}------------------------------${NC}"
read -p "📝 Enter Deployment Note: " msg
if [ -z "$msg" ]; then msg="System Maintenance: $(date +'%Y-%m-%d')"; fi

# 5. THE PUSH (Modified to ignore binaries)
echo -e "${CYAN}[SYSTEM] Igniting Venture Uplink...${NC}"
# Use a specific add if you want to be safe, or rely on .gitignore
git add . 
git commit -m "🚀 $msg" --quiet
git push origin master --quiet

# 6. AUTOMATED RELEASE (The "Spiciest" Part)
# Requires GitHub CLI (gh) installed
VERSION=$(grep -oP '(?<=<Version>)[^<]+' *.csproj | head -1)
echo -e "${CYAN}[SYSTEM] Creating Release v$VERSION...${NC}"
gh release create "v$VERSION" ./dist/*.nupkg --title "Release v$VERSION" --notes "$msg"

echo -e "\n${GREEN}✔ UPLINK COMPLETE: System is Stable.${NC}"
echo -e "${GOLD}View at: https://github.com/kgbcupcake/Dungeon-Adventures/releases${NC}"