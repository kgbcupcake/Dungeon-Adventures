[Setup]
AppName=Dungeon-Adventures
AppVersion=1.0.1
DefaultDirName={autopf}\DungeonAdventures
DefaultGroupName=Dungeon-Adventures
UninstallDisplayIcon={app}\Dungeon-Adventures.exe
Compression=lzma2
SolidCompression=yes
OutputDir=./dist/installer

[Files]
; Grab everything from the 'publish' folder
Source: "dist\win\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{group}\Dungeon-Adventures"; Filename: "{app}\Dungeon-Adventures.exe"
Name: "{userdesktop}\Dungeon-Adventures"; Filename: "{app}\Dungeon-Adventures.exe"