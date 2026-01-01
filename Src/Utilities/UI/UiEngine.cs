using DungeonAdventures.Src.GameData.Components;
using Pastel;
using System.Text.RegularExpressions;
using static DungeonAdventures.Src.GameData.Entities.PlayerData;
using static System.Console;

namespace DungeonAdventures.Src.Utilities.UI
{
	public static class UiEngine
	{
		public const string BorderColor = "#8B0000"; // Dark Red for ominous borders
		public const int BoxWidth = 46;

		// --- SPICY THEME COLORS ---
		private const string DungeonPurple = "#2E0E4E";
		private const string TorchGold = "#FFAB00";
		private const string NeonGreen = "#00FF41"; // High-contrast Matrix green
		private const string DimGrey = "#777777";   // Severe retro grey
		private const string LockedRed = "#440000"; // Dark "locked" red

		public static string StripAnsi(string text)
		{
			if (string.IsNullOrEmpty(text)) return "";
			return Regex.Replace(text, @"\x1B\[[0-9;]*[a-zA-Z]", "");
		}
		
		public static string PadAnsiStringWithCenter(string text, int totalWidth)
		{
			if (string.IsNullOrEmpty(text)) return new string(' ', totalWidth);
			int cleanLength = UiEngine.StripAnsi(text).Length;
			int padding = (totalWidth - cleanLength) / 2;
			string leftPad = new string(' ', padding > 0 ? padding : 0);
			string rightPad = new string(' ', totalWidth - (padding + cleanLength) > 0 ? totalWidth - (padding + cleanLength) : 0);
			return leftPad + text + rightPad;
		}

		public static void DrawCentered(string text)
		{
			string cleanText = StripAnsi(text);
			int centerX = (WindowWidth / 2) - (cleanText.Length / 2);
			if (centerX < 0) centerX = 0;
			SetCursorPosition(centerX, CursorTop);
			WriteLine(text);
		}

		public static void DrawCentered(string text, int row)
		{
			string cleanText = StripAnsi(text);
			SetCursorPosition(Math.Max(0, (WindowWidth / 2) - (cleanText.Length / 2)), row);
			Write(text);
		}

		public static void DrawCentered(string text, int row, int clearWidth = 50)
		{
			string clean = StripAnsi(text);
			int startPos = WindowWidth / 2 - clean.Length / 2;
			int clearStart = WindowWidth / 2 - clearWidth / 2;

			SetCursorPosition(Math.Max(0, clearStart), row);
			Write(new string(' ', clearWidth));

			SetCursorPosition(Math.Max(0, startPos), row);
			Write(text);
		}

		public static void DrawLoadingScreen(string taskName, int durationMs)
		{
			CursorVisible = false;
			int width = 40;
			int centerX = WindowWidth / 2;
			int centerY = WindowHeight / 2;

			for (int i = 0; i <= 100; i += 10)
			{
				Clear();
				DrawCentered(taskName.Pastel(TorchGold), centerY - 1);

				SetCursorPosition(centerX - (width / 2), centerY + 1);
				string bar = new string('█', (i * width) / 100);
				string empty = new string('░', width - ((i * width) / 100));
				Write(bar.Pastel(DungeonPurple) + empty.Pastel("#333333"));

				Thread.Sleep(durationMs / 10);
			}
			Clear();
		}

		public static void DrawBoxLine(string content, int row, int left)
		{
			SetCursorPosition(left, row);
			Write("║".Pastel(BorderColor));
			SetCursorPosition(left + 1, row);
			Write(new string(' ', BoxWidth - 2));
			string cleanContent = StripAnsi(content);
			int contentStart = left + 1 + (BoxWidth - 2) / 2 - cleanContent.Length / 2;
			SetCursorPosition(contentStart, row);
			Write(content);
			SetCursorPosition(left + BoxWidth - 1, row);
			Write("║".Pastel(BorderColor));
		}

		public static void DrawBoxBorder(int row, int left, bool isTop)
		{
			SetCursorPosition(left, row);
			string leftCap = isTop ? "╔" : "╚";
			string rightCap = isTop ? "╗" : "╝";
			Write((leftCap + new string('═', BoxWidth - 2) + rightCap).Pastel(BorderColor));
		}

		public static void DrawDynamicFrame(string title, List<string> lines, string hint = "", int boxWidth = 66, int startY = 11, int selectedIndex = -1, int previewHp = -1, int previewCoins = -1)
		{
			CursorVisible = false;
			SetCursorPosition(0, 0);
			BackgroundColor = ConsoleColor.DarkRed; // Dark Red background
			ForegroundColor = ConsoleColor.Yellow; // Yellow foreground
			int displayHp = (previewHp != -1) ? previewHp : (GameState.CurrentPlayer?.Health ?? 100);
			int displayCoins = (previewCoins != -1) ? previewCoins : (GameState.CurrentPlayer?.Coins ?? 50);
			string leftSection = $" {title}";
			string rightSection;
			if (title == "MAIN MENU")
			{
				rightSection = $"HP: {displayHp} | COINS: {displayCoins} ";
			}
			else
			{
				rightSection = $"TIME: {DateTime.Now:HH:mm} | HP: {displayHp} | COINS: {displayCoins} ";
			}
			int fillerCount = WindowWidth - leftSection.Length - rightSection.Length;
			Write(leftSection + new string(' ', Math.Max(0, fillerCount)) + rightSection);
			ResetColor();

			int startX = (WindowWidth / 2) - (boxWidth / 2);
			int fixedHeight = lines.Count;
			string borderColor = BorderColor;

			SetCursorPosition(startX, startY);
			Write(("╔" + new string('═', boxWidth - 2) + "╗").Pastel(borderColor));

			for (int i = 0; i < fixedHeight; i++)
			{
				string content = (lines != null && i < lines.Count) ? lines[i] : "";
				DrawCenteredBoxLine(content, startY + 1 + i, startX, boxWidth, borderColor);
			}

			SetCursorPosition(startX, startY + fixedHeight + 1);
			Write(("╚" + new string('═', boxWidth - 2) + "╝").Pastel(borderColor));

			if (!string.IsNullOrEmpty(hint)) DrawCentered(hint.Pastel("#555555"), startY + fixedHeight + 3);
			UiFunctions.DisplayFooter();
		}

		public static void DrawCenteredBoxLine(string content, int row, int x, int width, string borderColor)
		{
			SetCursorPosition(x, row);
			Write($"{"║".Pastel(borderColor)}{content}{"║".Pastel(borderColor)}");
		}


		public static void DrawAttributeBox(loadPlayer player, int x, int y, int selectedStat = -1)
		{
			string bColor = BorderColor;
			SetCursorPosition(x, y);
			WriteLine("╔══════════ ATTRIBUTES ══════════╗".Pastel(bColor));
			DrawStatLine("Strength", player.Abilities.Strength, x, y + 1, selectedStat == 0, "#00FF00", "#FFFFFF", bColor);
			DrawStatLine("Dexterity", player.Abilities.Dexterity, x, y + 2, selectedStat == 1, "#00FF00", "#FFFFFF", bColor);
			DrawStatLine("Intelligence", player.Abilities.Intelligence, x, y + 3, selectedStat == 2, "#00FF00", "#FFFFFF", bColor);
			DrawStatLine("Constitution", player.Abilities.Constitution, x, y + 4, selectedStat == 3, "#00FF00", "#FFFFFF", bColor);
			DrawStatLine("Perception", player.Abilities.Perception, x, y + 5, selectedStat == 4, "#00FF00", "#FFFFFF", bColor);
			DrawStatLine("Luck", player.Abilities.Luck, x, y + 6, selectedStat == 5, "#00FF00", "#FFFFFF", bColor);
			DrawStatLine("Wisdom", player.Abilities.Wisdom, x, y + 7, selectedStat == 6, "#00FF00", "#FFFFFF", bColor);
			DrawStatLine("Charisma", player.Abilities.Charisma, x, y + 8, selectedStat == 7, "#00FF00", "#FFFFFF", bColor);
			SetCursorPosition(x, y + 9);
			WriteLine("╠════════════════════════════════╣".Pastel(bColor));
			SetCursorPosition(x, y + 10);
			WriteLine($"║ Health: {player.Health.ToString().PadRight(23).Pastel("#FF4500")}║");
			SetCursorPosition(x, y + 11);
			WriteLine("╚════════════════════════════════╝".Pastel(bColor));
		}

		private static void DrawStatLine(string label, int value, int x, int y, bool isSelected, string aCol, string iCol, string bColor)
		{
			// Hide cursor during draw to prevent the "white flicker"
			Console.CursorVisible = false;

			SetCursorPosition(x, y);
			string color = isSelected ? aCol : iCol;

			// PADDING FIX: Ensure the total length of 'text' matches your frame width exactly
			// If your box is 34 characters wide, use PadRight accordingly.
			string text = $" {label}:".PadRight(15) + value.ToString().PadLeft(5);

			// If the item is selected, we can add the > < arrows back inside the line
			if (isSelected)
			{
				text = $"> {label}: {value} <".PadRight(20).PadLeft(25); // Adjust numbers to fit your box
			}

			Write($"{"║".Pastel(bColor)}{text.Pastel(color)}{"║".Pastel(bColor)}");

			// Move cursor to 0,0 after drawing so it's not visible on the menu
			SetCursorPosition(0, 0);
		}

		public static bool GetArrowChoice(string prompt, string option1, string option2)
		{
			int selected = 0;
			while (true)
			{
				Clear();
				WriteLine($"\n {prompt.Pastel("#FFD700")}\n");
				WriteLine($" {(selected == 0 ? " > ".Pastel("#00FF00") : "    ")} {option1}");
				WriteLine($" {(selected == 1 ? " > ".Pastel("#00FF00") : "    ")} {option2}");
				var key = ReadKey(true).Key;
				if (key == ConsoleKey.UpArrow) selected = 0;
				else if (key == ConsoleKey.DownArrow) selected = 1;
				else if (key == ConsoleKey.Enter) return selected == 0;
			}
		}



		public static void DrawV2Footer()
		{
			int row = WindowHeight - 1;
			SetCursorPosition(0, row);

			BackgroundColor = ConsoleColor.DarkCyan;
			ForegroundColor = ConsoleColor.White;
			Write(" Build_MoDe ");
			ResetColor();

			// MATCHED TO YOUR GameState.cs: Uses IsDevMode
			string devHint = GameState.IsDevMode ? " [F8: GUI ACTIVE] ".Pastel("#FFD700") : "";
			string systemMsg = $"[ SYSTEM: STABLE ]{devHint}";

			DrawCentered(systemMsg.Pastel("#3A96DD"), row);

			// Using your BuildVersion from GameState
			string ver = $"V.{GameState.BuildVersion}";
			SetCursorPosition(WindowWidth - ver.Length - 1, row);
			Write(ver.Pastel("#FFD700"));
		}





	}
}