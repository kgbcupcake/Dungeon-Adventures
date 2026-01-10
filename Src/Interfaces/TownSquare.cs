using DungeonAdventures.Src.Utilities.UI;
using static System.Console;
using DungeonAdventures.Src.GameData.Components;
using DungeonAdventures.Src.Interfaces;

namespace DungeonAdventures.Src.GameEngine.Interfaces
{

	internal class TownSquare
	{
		public static void MainTownSquare()
		{
			// Mirroring MainMenu's structure
			bool isRunning = true;
			int selectedIndex = 0;
			string[] options =
			{
				"STORE",
				"QUESTS",
				"DUNGEON",
                "BLACK SMITH",
				"MED CLINIC",
				"PLAYER STATS",
				"EXIT"
			};

			while (isRunning)
			{
				RedrawMenu:
				CursorVisible = false;

				UiFunctions.TitleBar();
				UiFunctions.DisplayFooter();
				// 1. Create a "Processed" list to center the text and fix the borders
				List<string> centeredOptions = new List<string>();
				int internalWidth = 92; // The magic number for your 94-width reality

				foreach (var opt in options)
				{
					// Get visual length (even if you add colors later)
					int visualLength = UiEngine.StripAnsi(opt).Length;
					int leftSpace = (internalWidth / 2) - (visualLength / 2);
					int rightSpace = internalWidth - visualLength - leftSpace;

					// Create a perfectly padded line to overwrite the ghost borders
					centeredOptions.Add(new string(' ', leftSpace) + opt + new string(' ', rightSpace));
				}

				// 2. Pass the CENTERED list to the frame
				UiEngine.DrawDynamicFrame(
					title: "THE FORGOTTEN OUTPOST",
					lines: centeredOptions, // Use the new list
					hint: "Use arrow keys to navigate",
					boxWidth: 94, // Ensure this is explicitly set to your anchor
					selectedIndex: selectedIndex
				);

				while (!Console.KeyAvailable)
				{
					if (DevGuiRenderer.NeedsMenuRedraw)
					{
						goto RedrawMenu;
					}

					Thread.Sleep(10);
				}

				ConsoleKey key = ReadKey(true).Key;

				if (key == ConsoleKey.Escape) { isRunning = false; continue; }
				if (key == ConsoleKey.F12)
				{
					GameState.IsDevMode = !GameState.IsDevMode;
					DevGuiRenderer.NeedsMenuRedraw = true;
					continue;
				}

				switch (key)
				{
					case ConsoleKey.UpArrow:
						selectedIndex = (selectedIndex == 0) ? options.Length - 1 : selectedIndex - 1;
						break;
					case ConsoleKey.DownArrow:
						selectedIndex = (selectedIndex == options.Length - 1) ? 0 : selectedIndex + 1;
						break;
					case ConsoleKey.Enter:
						switch (options[selectedIndex])
						{
							case "STORE":
								Store.LoadShop();
								break;
							case "QUESTS":
								// QuestSystem.StartTestQuest();
								break;
							case "DUNGEON":
								// new DungeonGame().Start();
								goto RedrawMenu;
							case "BLACK SMITH":
								UiFunctions.QuickAlert("The Blacksmith is closed for repairs!");
								break;
							case "MED CLINIC":
								UiFunctions.QuickAlert("The Mage is out healing others!");
								break;
							case "PLAYER STATS":
								Stats.PlayerStats();
								break;
							case "EXIT":
								Environment.Exit(0);
								isRunning = false;
								break;
						}
						break;
				}
			}
		}



	}







}
