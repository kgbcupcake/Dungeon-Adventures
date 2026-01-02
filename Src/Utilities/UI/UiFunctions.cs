// Ignore Spelling: Tle

using DungeonAdventures.Src.GameData.Components;
using Pastel;
using System.Runtime.InteropServices;
using System.Text;

// ADD THIS LINE TO FIX THE ERRORS:
using static System.Console;

namespace DungeonAdventures.Src.Utilities.UI
{
	public class UiFunctions
	{
		#region//ConsoleSize
		public static void ConsoleSize()
		{
			int width = 100; // Slightly wider than standard to fit the matrix side-car
			int height = 30;

			if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
			{
				try
				{
					// 1. Set the Window Size first
					Console.SetWindowSize(85, 25);

					// 2. FORCE the Buffer to match the Window exactly (This kills the scroll bar)
					Console.SetBufferSize(85, 25);
				}
				catch { /* Fallback for small screens */ }
			}
			Console.CursorVisible = false;
		}


		#endregion
		#region//TitleBar
		public static void TitleBar()
				{
					// 1. FORCE UTF-8: Essential for Ubuntu/Linux to render ⚔️
					Console.OutputEncoding = System.Text.Encoding.UTF8;
		
					// We save the cursor position first so we don't disrupt the menu
					int currentCursorLeft = Console.CursorLeft;
					int currentCursorTop = Console.CursorTop;
		
					try
					{
						Console.SetCursorPosition(0, 0);
						// Wipe the top line with spaces to act as a physical eraser
						Console.Write(new string(' ', Console.WindowWidth));
		
						Random rnd = new Random();
						// Check if player is established (e.g., after character creation or loading a game)
						if (GameState.CurrentPlayer != null)
						{
					// 2. SET THE TAB TITLE: Use the property for better cross-platform support
					string modeLabel = GameState.IsDevMode ? "DEVELOPER" : "STABLE";
					Console.Title = $"⚔️ {modeLabel} MODE V2 ⚔️";

				}
				else
						{
							// --- Glitchy, Vague Title Bar (Player Not Established) ---
							string[] glitchTitles = {
								"S̷Y̷S̷T̵E̵M̴ ̶E̵R̷R̴O̷R̴",
								"D̷A̷T̵A̷ ̶C̶O̵R̷R̴U̷P̷T̸E̵D̸",
								"U̷N̵K̵N̷O̵W̷N̵ ̶E̵N̵T̸I̵T̸Y̵",
								"P̷R̷O̷T̸O̵C̸O̷L̵ ̶F̷A̷I̸L̴U̷R̷E̵"
							};
		
							string[] glitchChars = { "!", "@", "#", "$", "%", "^", "&", "*", "(", ")", "-", "=", "+", "[", "]", "{", "}", ";", ":", "'", "\"", "\\", "|", "<", ">", ",", ".", "/", "?", "`", "~" };
							string[] scaryColors = { "#FF0000", "#8B0000", "#4B0082", "#800000" }; // Red, Dark Red, Indigo, Maroon
		
							// Flickering Console.Title
							Console.Title = glitchTitles[rnd.Next(glitchTitles.Length)].Pastel(scaryColors[rnd.Next(scaryColors.Length)]);
		
							// Row 0 Display - Dynamic and Glitchy
							string glitchText = "";
							int glitchLength = rnd.Next(15, 30);
							for(int i = 0; i < glitchLength; i++)
							{
								glitchText += glitchChars[rnd.Next(glitchChars.Length)].Pastel(scaryColors[rnd.Next(scaryColors.Length)]);
							}
							UiEngine.DrawCentered(glitchText, 0);
						}
					}
					finally
					{
						// Always restore the cursor position
						Console.SetCursorPosition(currentCursorLeft, currentCursorTop);
					}
				}
				#endregion
		#region//Footer
		public static void DisplayFooter(bool isVisible = true)
		{
			int width = WindowWidth;
			int height = WindowHeight;
			int footerRow = height - 1;

			// Save cursor to prevent menu flicker
			int origLeft = CursorLeft;
			int origTop = CursorTop;

			try
			{
				// 1. Setup Data
				string modeLabel = GameState.IsDevMode ? " DEV " : " BUILD ";
				string modeStatus = GameState.IsDevMode ? " F8 FOR GUI " : " STABLE ";
				string versionText = GameState.IsDevMode ? $"ITER: {GameState.DevIteration}" : $"V {GameState.BuildVersion}";
				string accentColor = GameState.IsDevMode ? "#FF4500" : "#125874";

				// 2. Build the "Powerline" Badge (Left Side)
				string leftBadge = modeLabel.PastelBg(accentColor).Pastel("#FFFFFF") +
								  modeStatus.PastelBg("#333333").Pastel(accentColor);

				// 3. Build the Version Info (Right Side)
				string rightInfo = $" {versionText} ".Pastel("#888888").PastelBg("#1A1A1A");

				// 4. Calculate Center Padding
				// We subtract the raw character length (ignoring color codes) from the total width
				int rawLength = modeLabel.Length + modeStatus.Length + versionText.Length + 2;
				int paddingSize = width - rawLength;
				string padding = new string(' ', Math.Max(0, paddingSize)).PastelBg("#1A1A1A");

				// 5. Execution: One single move and one single write
				SetCursorPosition(0, footerRow);
				Write(leftBadge + padding + rightInfo);

				// Restore cursor to the menu area
				SetCursorPosition(origLeft, origTop);
			}
			catch { /* Resize Safety */ }
		}
		#endregion
		#region//ShowSaveLoadedIcon
		public static void ShowSaveLoadedIcon(string playerName)
		{
			Console.Clear();
			// V2 Integration
			UiFunctions.TitleBar();
			UiFunctions.DisplayFooter();

			int centerX = Console.WindowWidth / 2;
			int startY = 5;

			// --- SIDE DATA PILLARS (Dynamic Width) ---
			string[] sideDecor = { " [CON-4] ", " [TRN-R] ", " [LOG-V] ", " [SYS-8] " };
			for (int i = 0; i < sideDecor.Length; i++)
			{
				// Use Console.WindowWidth - 10 to keep them relative to the screen edge
				Console.SetCursorPosition(2, 7 + (i * 3));
				Console.Write(sideDecor[i].Pastel("#125874"));

				Console.SetCursorPosition(Console.WindowWidth - 11, 7 + (i * 3));
				Console.Write(sideDecor[i].Pastel("#125874"));
			}

			// 1. TOP ACCENTS
			string topAccent = "◢◤".Pastel("#00FF00") + " —————————————————————————————————————— " + "◥◣".Pastel("#00FF00");
			Console.SetCursorPosition(Math.Max(0, centerX - (UiEngine.StripAnsi(topAccent).Length / 2)), startY - 2);
			Console.Write(topAccent);

			// 2. 3D ICON
			string icon = @"
      _______________________
     /                      /|
    /      RECONSTRUCT     / |
   /        COMPLETE      /  |
  /______________________/   |
  |                      |   |
  |   [ VESSEL ONLINE ]  |   |
  |______________________|  /";

			string[] lines = icon.Split('\n', StringSplitOptions.RemoveEmptyEntries);
			int currentY = startY;
			foreach (string line in lines)
			{
				Console.SetCursorPosition(centerX - 13, currentY++);
				string color = (currentY % 2 == 0) ? "#00FF00" : "#008800";
				Console.Write(line.Pastel(color));
			}

			// 3. PLAYER DATA BLOCK
			currentY += 1;
			string bracketL = ">> [".Pastel("#00FFFF");
			string bracketR = "] <<".Pastel("#00FFFF");
			string nameDisp = playerName.ToUpper().Pastel("#FFD700");
			string fullRow = $"{bracketL} {nameDisp} {bracketR}";

			// Using your new UiEngine helper to calculate centering
			int textLen = UiEngine.StripAnsi(fullRow).Length;
			Console.SetCursorPosition(centerX - (textLen / 2), currentY);
			Console.WriteLine(fullRow);

			// 4. METRICS
			currentY += 3;
			string[] metrics = { "INTEGRITY: 100%", "BIO-SYNC: STABLE", "CHRONO-LINK: ACTIVE" };
			foreach (var m in metrics)
			{
				Console.SetCursorPosition(centerX - (m.Length / 2), currentY++);
				Console.Write(m.Pastel("#333333"));
			}

			// 5. THE PULSE PROMPT
			string prompt = "— PRESS ANY KEY TO ENTER THE VOID —";
			int promptY = 20;

			if (OperatingSystem.IsWindows())
			{
				Console.Beep(600, 50); Console.Beep(900, 50); Console.Beep(1200, 100);
			}

			while (!Console.KeyAvailable)
			{
				UiEngine.DrawCentered(prompt.Pastel("#FFFFFF"), promptY, 50);
				Thread.Sleep(500);
				if (Console.KeyAvailable) break;
				UiEngine.DrawCentered(prompt.Pastel("#444444"), promptY, 50);
				Thread.Sleep(250);
			}

			Console.ReadKey(true);
			Clear();
		}
		#endregion
		#region//LoadSaveProgress
		public static void LoadSaveProgress()
		{
			Console.Clear();

			int centerX = 85 / 2;
			int centerY = 12;
			int barWidth = 40;
			Random rnd = new Random();

			// --- RAIN CONFIG ---
			// Columns on the left (5, 10, 15) and right (70, 75, 80)
			int[] rainCols = { 5, 10, 15, 70, 75, 80 };
			int[] rainY = new int[rainCols.Length];
			for (int r = 0; r < rainY.Length; r++) rainY[r] = rnd.Next(2, 22);

			// 1. PHASE ONE: RAPID DATA SCAN
			for (int i = 0; i < 15; i++)
			{
				Console.SetCursorPosition(centerX - 20, centerY);
				string noise = $"SCANNING_SECTOR_{rnd.Next(1000, 9999)} >> 0x{rnd.Next(100, 999)}FF{rnd.Next(10, 99)}";
				Console.Write(noise.Pastel("#ed0410"));
				Thread.Sleep(90);
			}

			string[] steps = {
		"INITIALIZING HANDSHAKE",
		"DECRYPTING SOUL-FILE",
		"STITCHING TEMPORAL VECTORS",
		"RECONSTRUCTING VESSEL"
	};

			// 2. PHASE TWO: THE PROGRESS BAR + RAIN
			for (int i = 0; i < steps.Length; i++)
			{
				Console.SetCursorPosition(centerX - 25, centerY - 2);
				Console.Write(new string(' ', 50));
				Console.SetCursorPosition(centerX - (steps[i].Length / 2), centerY - 2);
				Console.Write(steps[i].Pastel("#00FFFF"));

				for (int p = 0; p <= 10; p++)
				{
					// --- UPDATE BINARY RAIN ---
					for (int r = 0; r < rainCols.Length; r++)
					{
						// Erase old bit
						Console.SetCursorPosition(rainCols[r], rainY[r]);
						Console.Write(" ");

						// Increment and wrap Y
						rainY[r]++;
						if (rainY[r] > 21) rainY[r] = 2;

						// Draw new bit
						Console.SetCursorPosition(rainCols[r], rainY[r]);
						string bit = rnd.Next(2).ToString();
						// Alternate between bright cyan and deep teal for depth
						string color = rnd.Next(3) == 0 ? "#00FFFF" : "#004444";
						Console.Write(bit.Pastel(color));
					}

					// --- UPDATE PROGRESS BAR ---
					float totalPercent = ((i * 10) + p) / 40f;
					int filledWidth = (int)(totalPercent * barWidth);

					Console.SetCursorPosition(centerX - (barWidth / 2), centerY);
					string bar = new string('█', filledWidth).Pastel("#00FFFF");
					string background = new string('░', barWidth - filledWidth).Pastel("#222222");
					string percentText = $" {(int)(totalPercent * 100)}% ".Pastel("#FFFFFF");

					Console.Write($"{bar}{background}{percentText}");

					Thread.Sleep(rnd.Next(30, 80));
				}
			}

			// 3. FINAL LOCK-ON
			Console.SetCursorPosition(centerX - 10, centerY + 2);
			Console.Write("SYNC SUCCESSFUL".Pastel("#00FF00"));

			if (OperatingSystem.IsWindows())
			{
				Console.Beep(1200, 100);
				Console.Beep(1500, 150);
			}
			Thread.Sleep(800);
		}
		#endregion
		#region//StartGameLoading
		public static void StartGameLoading()
		{
			// --- STEP 1: DIMENSIONS & SCROLLBAR KILLER ---
			const int width = 84;
			const int height = 24;

			try
			{
				Console.SetWindowSize(width + 1, height + 1);
				Console.SetBufferSize(width + 1, height + 1);
			}
			catch { }

			Console.Clear();
			Console.CursorVisible = false;

			int centerX = width / 2;
			int centerY = height / 2;
			int barWidth = 40;
			Random rnd = new Random();

			// --- STEP 2: INITIAL BURST ---
			for (int j = 0; j < 5; j++)
			{
				Console.SetCursorPosition(0, 0);
				StringBuilder burst = new StringBuilder();
				for (int y = 0; y < height; y++)
				{
					for (int x = 0; x < width; x++)
					{
						if (rnd.Next(0, 100) < 15)
						{
							burst.Append(((char)rnd.Next(33, 126)).ToString().Pastel(rnd.Next(2) == 0 ? "#FF0000" : "#00FF00"));
						}
						else burst.Append(" ");
					}
				}
				Console.Write(burst.ToString());
				Thread.Sleep(300);
			}

			// --- STEP 3: THE MAIN LOOP (Rain + Skulls + Nukes) ---
			string[] scaryLabels = {
		"SYSTEM INTEGRITY: CRITICAL...",
		"REALITY ANCHORS: DEGRADED...",
		"INITIATING UNSTABLE PROTOCOL...",
		"WARNING: ENTITY DETECTED...",
		"S̴Y̷S̷T̵E̵M̴ ̶C̶O̵R̷R̴U̷P̷T̸E̵D̸...",
		"F̸A̴I̸L̴U̶R̷E̵ ̸T̸O̵ ̷C̸O̷N̵N̷E̵C̷T̵..."
	};
			string[] scaryColors = { "#FF0000", "#8B0000", "#4B0082", "#2F4F4F" };
			int labelIndex = 0;

			for (int i = 0; i <= 100; i++)
			{
				// --- THE RAIN ENGINE (Background) ---
				Console.SetCursorPosition(0, 0);
				StringBuilder rainFrame = new StringBuilder();
				for (int y = 0; y < height; y++)
				{
					for (int x = 0; x < width; x++)
					{
						if (rnd.Next(0, 100) < 10)
						{
							rainFrame.Append(((char)rnd.Next(33, 126)).ToString().Pastel("#004400"));
						}
						else rainFrame.Append(" ");
					}
				}
				Console.Write(rainFrame.ToString());

				// --- THE UI OVERLAY ---

				// We use double quotes "" to avoid the "too many characters" error
				// 1. TOP PATTERN: RED SKULLS + ERROR MESSAGE
				string skullIcon = "☠"; // This is the standard Unicode skull, it tints perfectly
				string skullMsg = " [ FATAL_MEMORY_LEAK ] ";
				// Combine first, THEN apply color to the whole thing
				string topContent = $"{skullIcon}{skullMsg}{skullIcon}";

				// Use the glitchRed logic you have, but apply it to the FINAL string
				string glitchRed = rnd.Next(0, 10) > 8 ? "#4A0000" : "#FF0000";
				UiEngine.DrawCentered(topContent.Pastel(glitchRed), centerY - 5);

				// 2. DYNAMIC LABEL (The one you already had)
				if (i % 15 == 0) labelIndex = rnd.Next(scaryLabels.Length);
				UiEngine.DrawCentered(scaryLabels[labelIndex].Pastel(scaryColors[rnd.Next(scaryColors.Length)]), centerY - 2);

				// 3. PROGRESS BAR
				int barLeft = centerX - (barWidth / 2);
				Console.SetCursorPosition(barLeft, centerY);
				int progressBlocks = (int)((i / 100.0) * barWidth);
				string filled = new string('█', progressBlocks);
				string empty = new string('░', barWidth - progressBlocks);
				Console.Write(filled.Pastel("#5A057A") + empty.Pastel("#333333") + $" {i}%".Pastel("#FFFFFF"));

				// 4. BOTTOM PATTERN: NUKES + RADIATION WARNING
				string nukeIcon = "☢";
				string nukeMsg = " [ CORE_MELTDOWN_IMMINENT ] ";
				string bottomContent = nukeIcon + nukeMsg + nukeIcon;
				UiEngine.DrawCentered(bottomContent.Pastel(scaryColors[rnd.Next(scaryColors.Length)]), centerY + 3);

				// --- SPEED CONTROL ---
				int baseDelay = 120; // Slower rain and progress
				if (i < 30) Thread.Sleep(baseDelay + 100);
				else if (i < 80) Thread.Sleep(baseDelay);
				else Thread.Sleep(baseDelay + 150);

				if (rnd.Next(0, 70) == 0 && i < 90) i += rnd.Next(2, 8);
			}

			// --- STEP 4: FINAL MESSAGE ---
			Console.Clear();
			UiEngine.DrawCentered("T̷H̷E̵ ̸G̷A̷T̷E̵S̷ ̷A̶R̸E̵ ̷O̷P̷E̶N̸...".Pastel("#FF0000"), centerY);
			Thread.Sleep(2500);

			Console.ResetColor();
			Console.Clear();
		}
		#endregion
		#region//ShowCreationAnimation
		public static void ShowCreationAnimation(string playerName, string playerClass)
		{
			// --- DYNAMIC THEME ---
			string themeColor = playerClass switch
			{
				"Warrior" or "Paladin" or "Berserker" => "#FF4500",
				"Mage" or "Wizard" or "Warlock" => "#00FFFF",
				"Rogue" or "Assassin" => "#9370DB",
				_ => "#00FF00"
			};

			string[] steps = {
		$"Analyzing {playerName}'s Potential...",
		$"Forging the {playerClass} Soul-Core...",
		"Calibrating Attribute Matrix...",
		"Manifesting Physical Vessel...",
		"Injecting Starter Gear into Pack...",
		"Syncing Chrono-Link to World..."
	};

			Console.Clear();
			TitleBar();
			UiEngine.DrawCentered($"SYSTEM: CHARACTER MANIFESTATION".Pastel(themeColor), 4);
			DisplayFooter();

			int startY = 7;
			int centerX = (Console.WindowWidth / 2) - 20;

			for (int i = 0; i < steps.Length; i++)
			{
				Console.SetCursorPosition(centerX, startY + i);
				Console.Write("[ ".Pastel("#555555") + "WAIT".Pastel("#FFD700") + " ] ".Pastel("#555555") + steps[i]);

				// We removed the SaveData logic from here. 
				// The data is already synced by GameState.Sync() before this starts.

				Thread.Sleep(new Random().Next(300, 600));

				Console.SetCursorPosition(centerX, startY + i);
				Console.Write("[ ".Pastel("#555555") + "DONE".Pastel("#00FF00") + " ] ".Pastel("#555555") + steps[i]);
			}

			// --- FINAL FEEDBACK ---
			string checksum = Guid.NewGuid().ToString("N").Substring(0, 12).ToUpper();
			Console.SetCursorPosition(centerX, startY + steps.Length + 1);
			Console.WriteLine($"» HASH VERIFIED: {checksum}".Pastel("#555555"));

			Console.SetCursorPosition(centerX, startY + steps.Length + 2);
			Console.WriteLine("» UPLINK COMPLETE: LOCAL & MASTER FULLY SYNCED...".Pastel(themeColor));

			Thread.Sleep(1200);
		}
		#endregion
		#region//ProgressBar
		public static void ProgressBar()
		{
			int barWidth = 50;
			int topPadding = 12;
			int leftPadding = (85 / 2) - (barWidth / 2); // Result: 17

			Console.CursorVisible = false;

			// 1. Define the plain text for math
			string plainLabel = "LOADING PROFILE...";
			// 2. Define the colored version for display
			string coloredLabel = plainLabel.Pastel("#A020F0");

			// 3. Center using the PLAIN length (18 characters)
			int labelLeft = (85 / 2) - (plainLabel.Length / 2);
			Console.SetCursorPosition(labelLeft, topPadding - 2);
			Console.Write(coloredLabel);

			for (int i = 0; i <= 100; i++)
			{
				// Force the cursor back to the exact start of the bar area
				Console.SetCursorPosition(leftPadding, topPadding);

				int progressBlocks = (int)((i / 100.0) * barWidth);
				string filled = new string('█', progressBlocks);
				string empty = new string('░', barWidth - progressBlocks);

				// We draw the bar using Pastel, but since we use SetCursorPosition 
				// every loop, it can't "drift" to the left.
				Console.Write(filled.Pastel("#5A057A") + empty.Pastel("#333333"));

				// Add the percentage at the end
				Console.ForegroundColor = ConsoleColor.White;
				Console.Write($" {i}%");

				Thread.Sleep(25);
			}

			Console.ResetColor();
			Console.CursorVisible = true;
		}
		#endregion
		#region//QuickAlert

		public static void QuickAlert(string message)
		{
			int x = (85 / 2) - (message.Length / 2);
			int y = 12;
			Console.Clear();
			// Center it
			Console.SetCursorPosition((85 / 2) - (message.Length / 2), 12);
			Console.WriteLine(message.Pastel("#72071C"));
			Console.SetCursorPosition((85 / 2) - 10, y + 2);
			Console.Write("Press any key...".Pastel("#555555"));

			// This stops the code right here until a key is pressed!
			Console.ReadKey(true);

		}

		#endregion
		#region//AreaLoadingScreen
		public static void AreaLoadingScreen(string areaName, string hexColor)
		{
			Console.Clear();
			Console.CursorVisible = false;

			int barWidth = 40; // Slightly narrower for a "smaller" area feel
			int centerX = 85 / 2;
			int centerY = 25 / 2;

			// 1. Label (e.g., "ENTERING THE GENERAL STORE...")
			string plainLabel = $"ENTERING THE {areaName.ToUpper()}...";
			int labelLeft = centerX - (plainLabel.Length / 2);

			Console.SetCursorPosition(labelLeft, centerY - 2);
			Console.Write(plainLabel.Pastel(hexColor));

			// 2. The Bar
			for (int i = 0; i <= 100; i++)
			{
				int barLeft = centerX - (barWidth / 2);
				Console.SetCursorPosition(barLeft, centerY);

				int progressBlocks = (int)((i / 100.0) * barWidth);
				string filled = new string('█', progressBlocks);
				string empty = new string('░', barWidth - progressBlocks);

				// Use the hexColor passed into the method for the bar!
				Console.Write(filled.Pastel(hexColor) + empty.Pastel("#333333"));

				Console.ForegroundColor = ConsoleColor.White;
				Console.Write($" {i}%");

				Thread.Sleep(15); // Stores usually load fast!

			}

			Thread.Sleep(400);
			Console.Clear();
			//Store.LoadShop();
			Console.CursorVisible = true;
		}


		#endregion















	}
}
