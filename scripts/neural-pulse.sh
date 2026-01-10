#!/usr/bin/env bash

# Color Definitions
CYAN='\033[0;36m'
GREEN='\033[0;32m'
MAGENTA='\033[0;35m'
RED='\033[0;31m'
NC='\033[0m'

echo -e "${MAGENTA}📡 INITIATING VENTURE PULSE...${NC}"

# Check if the Game Engine is built
if [ ! -f "./bin/Dungeon-Adventures.exe" ] && [ ! -f "./bin/Dungeon-Adventures" ]; then
    echo -e "${RED}❌ OFFLINE: Game Engine not found. Run './scripts/install.sh' first.${NC}"
    exit 1
fi

echo -e "${CYAN}╔══════════════════════╦═══════════════╦════════════╗${NC}"
echo -e "${CYAN}║ ASSET CATEGORY       ║ SIZE / COUNT  ║ STATUS     ║${NC}"
echo -e "${CYAN}╠══════════════════════╬═══════════════╬════════════╣${NC}"

# 1. Check Maps/Levels
if [ -d "./Assets/Maps" ]; then
    map_count=$(find ./Assets/Maps -name "*.json" | wc -l)
    printf "${CYAN}║${NC} %-20s ${CYAN}║${NC} %-13s ${CYAN}║${NC} ${GREEN}STABLE${NC}     ${CYAN}║${NC}\n" "World Maps" "$map_count Files"
else
    printf "${CYAN}║${NC} %-20s ${CYAN}║${NC} %-13s ${CYAN}║${NC} ${RED}MISSING${NC}    ${CYAN}║${NC}\n" "World Maps" "0 Files"
fi

# 2. Check Textures/Sprites (Console Renderers often use .txt or .ans files)
if [ -d "./Assets/Art" ]; then
    art_size=$(du -sh ./Assets/Art | cut -f1)
    printf "${CYAN}║${NC} %-20s ${CYAN}║${NC} %-13s ${CYAN}║${NC} ${GREEN}STABLE${NC}     ${CYAN}║${NC}\n" "Visual Assets" "$art_size"
else
    printf "${CYAN}║${NC} %-20s ${CYAN}║${NC} %-13s ${CYAN}║${NC} ${RED}MISSING${NC}    ${CYAN}║${NC}\n" "Visual Assets" "0 KB"
fi

# 3. Check Save Data integrity
if [ -d "./Saves" ]; then
    save_count=$(find ./Saves -name "*.dat" | wc -l)
    printf "${CYAN}║${NC} %-20s ${CYAN}║${NC} %-13s ${CYAN}║${NC} ${GREEN}STABLE${NC}     ${CYAN}║${NC}\n" "Player Memory" "$save_count Saves"
else
    printf "${CYAN}║${NC} %-20s ${CYAN}║${NC} %-13s ${CYAN}║${NC} ${GREEN}NEW GAME${NC}   ${CYAN}║${NC}\n" "Player Memory" "0 Saves"
fi

echo -e "${CYAN}╚══════════════════════╩═══════════════╩════════════╝${NC}"
echo -e "${GREEN}✨ VENTURE LINK STABLE // ENGINE NOMINAL${NC}"