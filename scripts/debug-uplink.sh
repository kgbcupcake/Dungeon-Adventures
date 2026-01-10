#!#!/usr/bin/env bash
# No 'set -e' here so we can see all errors without the script crashing
# No '>/dev/null' so you see everything

# 1. Version Check
# Updated to look for the Dungeon-Adventures project file
csproj=$(find . -name "Dungeon-Adventures.csproj" | head -n 1)
version=$(grep -oP '(?<=<Version>).*?(?=</Version>)' "$csproj")
echo "🔍 DIAGNOSING VENTURE VERSION: $version"

# 2. Build Check
# Games are 'published' as executables rather than 'packed' as libraries
echo "📦 TESTING GAME PUBLISH (WIN-X64)..."
dotnet publish -c Release -r win-x64 --self-contained true -o ./dist/win --nologo
if [ $? -ne 0 ]; then 
    echo "❌ BUILD ERROR DETECTED"
else
    echo "✅ Build Successful. Compressing artifacts..."
    # Zip the build for easier GitHub Release downloading
    zip -r "Dungeon-Adventures-v$version.zip" ./dist/win > /dev/null
fi

# 3. GitHub CLI Check
echo "🌐 TESTING GITHUB CONNECTION..."
if ! command -v gh &> /dev/null; then
    echo "❌ ERROR: GitHub CLI (gh) is not installed or not in PATH."
else
    echo "✅ GitHub CLI found. Attempting to verify auth..."
    gh auth status
fi

# 4. Release Test
echo "🚀 ATTEMPTING VENTURE UPLOAD..."
pkgPath="./Dungeon-Adventures-v$version.zip"
# Updated to point to the Dungeon-Adventures repository
gh release create "v$version" "$pkgPath" --title "Venture v$version" --notes "Debug Uplink" --repo "kgbcupcake/Dungeon-Adventures" --overwrite

echo "🏁 DIAGNOSTICS COMPLETE."