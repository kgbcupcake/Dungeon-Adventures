using DungeonAdventures.Src.GameData.Components;
using DungeonAdventures.Src.Utilities.UI;
using System.Text.Json;
using static DungeonAdventures.Src.GameData.Entities.PlayerData;
using static System.Console;

namespace DungeonAdventures.Src.GameEngine.Interfaces
{
	public class CharacterLoadScreen
	{
		public static bool ShowLoadMenu()
		{
			string activeFolder = GameState.GetActiveProfileFolder();
			if (!Directory.Exists(activeFolder)) Directory.CreateDirectory(activeFolder);

			string[] files = Directory.GetFiles(activeFolder, "*.json");
			List<string> displayNames = files.Select(f => Path.GetFileNameWithoutExtension(f).Replace("_", " ").ToUpper()).ToList();

			int selectedIndex = 0;
			bool navigating = true;

			while (navigating)
			{
				// DYNAMIC PREVIEW: Peek inside the file for the UI header
				loadPlayer preview = new loadPlayer();
				if (files.Length > 0)
				{
					try
					{
						string json = File.ReadAllText(files[selectedIndex]);
						preview = JsonSerializer.Deserialize<loadPlayer>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new loadPlayer();
					}
					catch { preview.PlayerName = "CORRUPT DATA"; }
				}

				string title = displayNames.Count > 0
					? $"HERO: {preview.PlayerName.ToUpper()} | LVL: {preview.Level} | XP: {preview.Experience}/{preview.ExperienceToLevel}"
					: "CHARACTER SELECTION: EMPTY";

				Clear();
				UiEngine.DrawDynamicFrame(
					title,
					displayNames.Count > 0 ? displayNames : new List<string> { "NO SAVES FOUND" },
					"ARROWS to move | ENTER to select | ESC to cancel",
					boxWidth: 70, // Explicitly set a reasonable width
					startY: 5,   // Set a reasonable starting Y position
					selectedIndex: selectedIndex, // Correctly map selectedIndex
					previewHp: preview.Health,    // Correctly map previewHp
					previewCoins: preview.Coins   // Correctly map previewCoins
				);

				var key = ReadKey(true).Key;
				if (key == ConsoleKey.Escape) return false;
				if (displayNames.Count == 0) continue;

				switch (key)
				{
					case ConsoleKey.UpArrow: selectedIndex = (selectedIndex == 0) ? displayNames.Count - 1 : selectedIndex - 1; break;
					case ConsoleKey.DownArrow: selectedIndex = (selectedIndex == displayNames.Count - 1) ? 0 : selectedIndex + 1; break;
					case ConsoleKey.Enter:
						GameState.CurrentPlayer = preview; // Use the one we already deserialized
						UiFunctions.LoadSaveProgress();
						UiFunctions.ShowSaveLoadedIcon(preview.PlayerName);
						Thread.Sleep(800);
						TownSquare.MainTownSquare();
						return true;
				}
			}
			return false;
		}
	}
}