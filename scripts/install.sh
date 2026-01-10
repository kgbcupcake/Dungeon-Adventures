#!/bin/bash
set -e

# 1. Metadata extraction
VERSION=$(grep -oPm1 "(?<=<Version>)[^<]+" Dungeon-Adventures.csproj)

# Color Definitions
CYAN='\033[0;36m'
MAGENTA='\033[0;35m'
GREEN='\033[0;32m'
RED='\033[0;31m'
NC='\033[0m' 
BG_BLUE='\033[44;30m'
BG_GREEN='\033[42;37m'

clear
echo -e "${CYAN}"
cat << "EOF"
  _____                                     
 |  __ \                                    
 | |  | |_   _ _ __   __ _  ___  ___  _ __  
 | |  | | | | | '_ \ / _` |/ _ \/ _ \| '_ \ 
 | |__| | |_| | | | | (_| |  __/ (_) | | | |
 |_____/ \__,_|_| |_|\__, |\___|\___/|_| |_|
                      __/ |                 
                     |___/                  
EOF
echo -e "${NC}"
echo -e " ${BG_BLUE} ⚔️  DUNGEON-ADVENTURES VENTURE DOWNLINK v$VERSION ${NC}"
echo ""

# 2. Cloud Sync (Safety Toggle)
echo -ne " ${CYAN}📡 [1/4] SYNCING WORLD DATA...        ${NC}"
# git fetch origin master --quiet > /dev/null 2>&1
# git reset --hard origin/master --quiet > /dev/null 2>&1
echo -e "${GREEN}LOCAL SOURCE PRESERVED${NC}"

# 3. Clean & Build with Spinner
echo -ne " ${CYAN}📦 [2/4] FORGING THE ENGINE...        ${NC}"

# Target the 'win' subdirectory for clarity
(rm -rf ./bin && dotnet publish -c Release -r win-x64 --self-contained true -o ./bin --nologo -v quiet > /dev/null 2>&1) & 
pid=$! 

spinner=( '⠋' '⠙' '⠹' '⠸' '⠼' '⠴' '⠦' '⠧' '⠇' '⠏' )
while kill -0 $pid 2>/dev/null; do
    for i in "${spinner[@]}"; do
        echo -ne "\r ${CYAN}📦 [2/4] FORGING THE ENGINE...        ${MAGENTA}$i${NC}"
        sleep 0.1
    done
done
wait $pid 
echo -e "\r ${CYAN}📦 [2/4] FORGING THE ENGINE...        ${GREEN}DONE  ${NC}"

# 4. Binary Readiness
echo -ne " ${CYAN}💉 [3/4] PREPARING EXECUTABLES...      ${NC}"
# Check for both Linux and Windows binary names just in case
if [ -f "./bin/Dungeon-Adventures.exe" ]; then
    chmod +x ./bin/Dungeon-Adventures.exe > /dev/null 2>&1
    echo -e "${GREEN}WIN-READY${NC}"
elif [ -f "./bin/Dungeon-Adventures" ]; then
    chmod +x ./bin/Dungeon-Adventures > /dev/null 2>&1
    echo -e "${GREEN}LINUX-READY${NC}"
else
    echo -e "${RED}FAILED (NOT FOUND)${NC}"
fi

# 5. Clean Up
echo -ne " ${CYAN}🧹 [4/4] PURGING TEMPORARY LOGIC...   ${NC}"
dotnet clean --nologo -v quiet > /dev/null 2>&1
echo -e "${GREEN}CLEAN${NC}"

# 6. Packaging
echo -ne " ${MAGENTA}🎁 [BONUS] ARCHIVING ASSETS...        ${NC}"
zip -r "Dungeon-Adventures-v$VERSION-Portable.zip" ./bin > /dev/null 2>&1
echo -e "${GREEN}ZIP CREATED${NC}"

echo ""
echo -e " ${BG_GREEN} ✅ DOWNLINK SUCCESSFUL. VENTURE STATE: STABLE v$VERSION ${NC}"
echo ""