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
Console.WriteLine("Press 'S' to skip loading, or any other key to view the loading sequence...");
// Wait for a key press. This will block until a key is pressed.
ConsoleKeyInfo key = Console.ReadKey(true); // 'true' means don't display the key

if (key.Key == ConsoleKey.S)
{
    Console.WriteLine("Loading sequence skipped.");
    // Clear the console to prepare for MainMenu
    Console.Clear();
}
else
{
    // User pressed another key or just wanted to proceed with loading
    UiFunctions.StartGameLoading();
}

// 5. Launch Game
MainMenu.Show();