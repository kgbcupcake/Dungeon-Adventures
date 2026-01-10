#!/usr/bin/env bash
set -e

# --- [PATH CALIBRATION] ---
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
ROOT_DIR="$(cd "$SCRIPT_DIR/.." && pwd)"

# Color Definitions
CYAN='\033[0;36m'
MAGENTA='\033[0;35m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
GREEN='\033[0;32m'
RED='\033[0;31m'
GRAY='\033[0;90m'
NC='\033[0m' 
BG_RED='\033[41;37m'

# Prevents the health check from triggering the update loop
if [ "$HEALTH_INTERNAL_RUN" == "true" ]; then exit 0; fi
export HEALTH_INTERNAL_RUN="true"

clear
echo -e "${MAGENTA}"
cat << "EOF"
    D I A G N O S T I C   M O N I T O R
    ═══════════════════════════════════
EOF
echo -e "${NC}"

# --- [PHASE 1: DEPENDENCY AUDIT] ---
echo -e "${CYAN}📡 PHASE 1: SYSTEM ENVIRONMENT AUDIT${NC}"

# Comprehensive tool check loop
tools=("dotnet" "git" "gh" "zip" "sed" "grep" "tar") 

for tool in "${tools[@]}"; do
    # ... your existing tool loop ...
done

# NEW: Specific check for Inno Setup (Windows Installer Compiler)
echo -ne " ${GRAY}» Sub-system: ${NC}iscc (Inno)... "
if command -v iscc &> /dev/null; then
    echo -e "${GREEN}ONLINE (EXE Support)${NC}"
else
    echo -e "${YELLOW}OFFLINE (ZIP Fallback Only)${NC}"
fi

# --- [PHASE 2: FILESYSTEM & INTEGRITY] ---
echo -e "\n${CYAN}📂 PHASE 2: FILESYSTEM INTEGRITY${NC}"

# Verify Reality Anchors
anchors=("Dungeon-Adventures.csproj" "Program.cs" "README.md")
for file in "${anchors[@]}"; do
    echo -ne " ${GRAY}» Anchor: ${NC}$file... "
    if [ -f "$ROOT_DIR/$file" ]; then
        echo -e "${GREEN}VERIFIED${NC}"
    else
        echo -e "${RED}MANIFEST ERROR${NC}"
        echo -e "${YELLOW}Expected $file in $ROOT_DIR${NC}"
        exit 1
    fi
done

# --- [PHASE 3: NEURAL LOGIC TESTS] ---
echo -e "\n${CYAN}🧠 PHASE 3: LOGIC & UNIT DIAGNOSTICS${NC}"

# Re-linking dependencies
echo -ne " ${GRAY}» Re-linking Neuro-Dependencies... ${NC}"
dotnet restore "$ROOT_DIR/Dungeon-Adventures.csproj" --nologo -v quiet > /dev/null 2>&1
echo -e "${GREEN}DONE${NC}"

# Run NUnit Test Suite
echo -e " ${GRAY}» Initiating Combat & Logic Simulations (NUnit)...${NC}"
echo -e "${GRAY}---------------------------------------------------------${NC}"

# Temporarily allow failure to capture test results
set +e 
dotnet test "$ROOT_DIR" --nologo --verbosity minimal
TEST_RESULT=$?
set -e

echo -e "${GRAY}---------------------------------------------------------${NC}"
if [ $TEST_RESULT -eq 0 ]; then
    echo -e " ${GRAY}» Simulations: ${GREEN}STABLE${NC}"
else
    echo -e " ${GRAY}» Simulations: ${RED}CRITICAL LOGIC FAILURE${NC}"
    echo -e "${BG_RED} ❌ UPLINK ABORTED: Fix failing tests before shipping. ${NC}"
    exit 1
fi

# --- [PHASE 4: CLOUD ALIGNMENT] ---
echo -e "\n${CYAN}🛰️  PHASE 4: CLOUD ALIGNMENT CHECK${NC}"

# Git Status Audit
echo -ne " ${GRAY}» Git Workspace: ${NC}"
if [[ -n $(git status -s) ]]; then
    echo -e "${YELLOW}DIRTY (Uncommitted Changes Found)${NC}"
else
    echo -e "${GREEN}CLEAN${NC}"
fi

# GitHub CLI Auth Check
echo -ne " ${GRAY}» Cloud Authentication: ${NC}"
if gh auth status &> /dev/null; then
    echo -e "${GREEN}AUTHORIZED${NC}"
else
    echo -e "${YELLOW}UNAUTHORIZED${NC}"
fi

# --- [PHASE 5: FINAL REPORT] ---
echo -e "\n${CYAN}─────────────────────────────────────────────────────────${NC}"
echo -e " ${BG_GREEN} ✅ DIAGNOSTICS PASSED: ALL SYSTEMS STABLE ${NC}"
echo -e " ${GRAY}The engine is cleared for deployment.${NC}"
echo -e "${CYAN}─────────────────────────────────────────────────────────${NC}\n"

export HEALTH_INTERNAL_RUN="false"