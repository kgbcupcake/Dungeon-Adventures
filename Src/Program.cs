using DungeonAdventures.Src.GameEngine.Interfaces;
using DungeonAdventures.Src.Utilities.UI;
using System.Text;
using DungeonAdventures.Src.GameData.Components;

// 1. Establish Environment
// This ensures Ubuntu/WSL renders the swords ⚔️ instead of ??
Console.OutputEncoding = Encoding.UTF8;

// Let the VS/WSL "path dump" finish
Thread.Sleep(100);

// 2. Configure UI
UiFunctions.ConsoleSize();
Console.Write("\x1b[2J\x1b[3J\x1b[H");

// Ensure game directories are set up
GameState.EnsureDirectories();

// 3. Set the Spicy Title
// Using your TitleBar method ensures the swords are set and Row 0 is wiped
UiFunctions.TitleBar();

// 4. Loading Sequence
UiFunctions.StartGameLoading();

// 5. Launch Game
MainMenu.Show();