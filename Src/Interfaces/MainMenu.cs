using DungeonAdventures.Src.Interfaces;
using DungeonAdventures.Src.Game.Interfaces;
using DungeonAdventures.Src.Utilities;
using DungeonAdventures.Src.Utilities.UI;
using DungeonAdventures.Src.GameData; // Added for ThemeConfig
using System.Text.Json; // Added for JsonSerializer usage consistency
using System.Drawing; // Added for ThemeConfig usage consistency
using Pastel;
using System.Runtime.InteropServices;
using static System.Console;
using DungeonAdventures.Src.GameData.Components;

namespace DungeonAdventures.Src.GameEngine.Interfaces
{
	internal class MainMenu
	{
		private static DevGuiRenderer? _myDevRenderer;

		public static void Show()
		{
			GameState.EnsureDirectories();
			var conductor = new Conductor();
			bool isMirrorHealthy = SaveGame.RunSanityCheck();
			CursorVisible = true; // Make cursor visible for retro/creepy feel

			bool isRunning = true;
			int selectedIndex = 0;
			string[] options = { "START NEW GAME", "LOAD SAVE", "CREDITS", "EXIT" };

			// --- DRAW TITLE ART ONCE ---
			RenderService.ClearFrameBuffer();
			RenderService.Present();
			string titleArt = @"
  ██████╗ ██╗   ██╗███╗   ███╗ ██████╗ ███████╗██████╗  █████╗  ██████╗ ███╗   ██╗
  ██╔══██╗██║   ██║████╗ ████║██╔════╝ ██╔════╝██╔══██╗██╔══██╗██╔════╝ ████╗  ██║
  ██║  ██║██║   ██║██╔████╔██║██║  ███╗█████╗  ██████╔╝███████║██║  ███╗██╔██╗ ██║
  ██║  ██║██║   ██║██║╚██╔╝██║██║   ██║██╔══╝  ██╔══██╗██╔══██║██║   ██║██║╚██╗██║
  ██████╔╝╚██████╔╝██║ ╚═╝ ██║╚██████╔╝███████╗██║  ██║██║  ██║╚██████╔╝██║ ╚████║
  ╚═════╝  ╚═════╝ ╚═╝     ╚═╝ ╚═════╝ ╚══════╝╚═╝  ╚═╝╚═╝  ╚═╝ ╚═════╝ ╚═╝  ╚═══╝
 
<--------------------------------------------------------------------------------->
<                  D U N G E O N   A D V E N T U R E S - R E B O R N              >
<--------------------------------------------------------------------------------->
".Pastel("#8B0000"); // Dark Red
			string[] titleLines = titleArt.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
			for (int i = 0; i < titleLines.Length; i++)
			{
				if (string.IsNullOrWhiteSpace(titleLines[i])) continue;
				UiEngine.DrawCentered(titleLines[i], i + 2);
			}

			// --- ATMOSPHERIC BACKGROUND TEXT ---
			UiEngine.DrawCentered("--------------------------------------------".Pastel("#4A0000"), 10);
			UiEngine.DrawCentered("SYSTEM // CORRUPTED // REALITY // FRAGMENTED".Pastel("#4A0000"), 11);
			UiEngine.DrawCentered("--------------------------------------------".Pastel("#4A0000"), 18);
			UiEngine.DrawCentered("ERROR // SECTOR // UNSTABLE // ENTITY".Pastel("#4A0000"), 19);
			UiEngine.DrawCentered("--------------------------------------------".Pastel("#4A0000"), 20);


			while (isRunning)
			{
			RedrawMenu:
				bool saveExists = CheckForSaves();

				UiFunctions.TitleBar();
				RenderMenu(options, selectedIndex, saveExists);

				while (!KeyAvailable)
				{
					// if (DevGuiRenderer.NeedsMenuRedraw)
					// {
					// 	DevGuiRenderer.NeedsMenuRedraw = false;
					// 	goto RedrawMenu;
					// }

					UpdateIdleAnimation(saveExists);

					Thread.Sleep(50);
				}

				ConsoleKeyInfo keyInfo = ReadKey(true);

				if (HandleGlobalHotkeys(keyInfo)) continue;

				switch (keyInfo.Key)
				{
					case ConsoleKey.UpArrow:
						if (selectedIndex > 0)
						{
							selectedIndex--;
						}
						break;
					case ConsoleKey.DownArrow:
						if (selectedIndex < options.Length - 1)
						{
							selectedIndex++;
						}
						break;
					case ConsoleKey.Enter:
						isRunning = ExecuteSelection(options[selectedIndex], saveExists);
						if (!isRunning) return;

						RenderService.ClearFrameBuffer();
						RenderService.Present();
						break;
				}
				RenderMenu(options, selectedIndex, saveExists);
			}
		}

		private static void RenderMenu(string[] options, int selectedIndex, bool saveExists)
		{
			const int boxWidth = 42;
			const int startY = 12;

			var menuItems = new List<string>();

			for (int i = 0; i < options.Length; i++)
			{
				menuItems.Add(BuildMenuItem(i, selectedIndex, saveExists, options));
			}

			UiEngine.DrawDynamicFrame("MAIN MENU", menuItems, "Use Arrows & Enter", boxWidth: boxWidth, startY: startY);
		}
		private static string BuildMenuItem(int index, int selectedIndex, bool saveExists, string[] options)
		{
			if (index >= options.Length) return "";

			string option = options[index];
			bool isSelected = (index == selectedIndex);

			// Normalize the string to catch "Load Save", "LOAD SAVE ", etc.
			if (option.Trim().ToUpper() == "LOAD SAVE" && !saveExists)
			{
				return UiEngine.PadAnsiStringWithCenter("🔒 LOAD SAVE 🔒".Pastel("#8B0000"), 40); // Darker red
			}

			if (isSelected)
			{
				string selectedColor = ((int)(DateTime.Now.TimeOfDay.TotalMilliseconds / 250) % 2 == 0) ? "#FFD700" : "#FFA500"; // Gold flicker
				return UiEngine.PadAnsiStringWithCenter($"> {option} <".Pastel(selectedColor), 40);
			}

			return UiEngine.PadAnsiStringWithCenter(option.Pastel("#4A0000"), 40); // Very dark red/brown
		}
		private static bool HandleGlobalHotkeys(ConsoleKeyInfo keyInfo)
		{
			if (keyInfo.Key == ConsoleKey.F8)
			{
				// Only allow the GUI to toggle if the Master Switch (IsDevMode) is on
				if (GameState.IsDevMode && _myDevRenderer != null)
				{
					_myDevRenderer.Toggle(); // Assuming Toggle() handles visibility
				}
				return true;
			}

			if (keyInfo.Key == ConsoleKey.F12)
			{
				GameState.IsDevMode = !GameState.IsDevMode;
				if (GameState.IsDevMode && _myDevRenderer == null)
				{
					if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
					{
						_myDevRenderer = new DevGuiRenderer();
						_ = Task.Run(async () => await _myDevRenderer.Start());
					}
				}
				return true;
			}

			if (keyInfo.Key == ConsoleKey.F10)
			{
				GameState.Sync();
				return true;
			}

			return false;
		}

		private static void UpdateIdleAnimation(bool saveExists)
		{
			string status = saveExists ? "[ SAVES DETECTED ]" : "[ NO SAVES FOUND ]";
			bool statusVisible = ((int)(DateTime.Now.TimeOfDay.TotalSeconds * 2.0) % 2 == 0);
			// Row 22 ensures it stays below the spicy frame
			UiEngine.DrawCentered(statusVisible ? status.Pastel(saveExists ? "#00FF00" : "#FF0000") : "                     ", 22);
		}

		private static bool CheckForSaves()
		{
			string activeFolder = GameState.GetActiveProfileFolder();
			if (Directory.Exists(activeFolder))
			{
				return Directory.GetFiles(activeFolder, "*.json").Length > 0;
			}
			return false;
		}

		private static bool ExecuteSelection(string selection, bool saveExists)
		{
			switch (selection)
			{
				case "START NEW GAME":
					RenderService.ClearFrameBuffer();
					RenderService.Present();
					CharacterCreation.Start();
					if (GameState.CurrentPlayer != null) // Check if character was successfully created
					{
						UiFunctions.TitleBar(); // Update title bar to established state
					}
					return false; // Exit menu to enter game loop

				case "LOAD SAVE":
					if (saveExists)
					{
						RenderService.ClearFrameBuffer();
						RenderService.Present(); // Clear the main menu before showing load screen
						if (CharacterLoadScreen.ShowLoadMenu()) // Show the load menu
						{
							// If a character was loaded, update the title bar
							UiFunctions.TitleBar();
							return false; // Exit MainMenu.Show() to proceed to game (or wherever CharacterLoadScreen leads)
						}
					}
					return true; // Keep menu running if no save exists or user cancels load

				case "CREDITS":
					ShowCredits();
					return true; // Keep menu running

				case "EXIT":
					Environment.Exit(0);
					return false;

				default:
					return true;
			}
		}

		private static void ShowCredits()
		{
			RenderService.ClearFrameBuffer();
			RenderService.Present();
			UiEngine.DrawCentered("CREATED BY: YOUR NAME".Pastel("#FFAB00"), 10);
			UiEngine.DrawCentered("V2 ENGINE POWERED BY C# & IMGUI", 12);
			UiEngine.DrawCentered("Press any key to return...", 16);
			ReadKey(true);
			RenderService.ClearFrameBuffer();
			RenderService.Present();
		}
	}
}