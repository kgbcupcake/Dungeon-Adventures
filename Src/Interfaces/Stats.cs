using DungeonAdventures.Src.GameData.Components;
using Pastel;
using static System.Console;


namespace DungeonAdventures.Src.GameEngine.Interfaces
{
	internal class Stats
	{

		public static void PlayerStats()
		{
			var p = GameState.CurrentPlayer;

			// 1. Data Calculation
			int maxLevelXp = p.ExperienceToLevel;
			int currentXp = p.Experience;
			int playerLevel = p.Level;

			// Calculate progress for a 10-block bar
			double xpPercent = (double)currentXp / Math.Max(1, maxLevelXp);
			int filledSlots = Math.Clamp((int)(xpPercent * 10), 0, 10);

			// 2. Build the UI Components
			string filledPart = new string('■', filledSlots).Pastel("#00FFFF");
			string emptyPart = new string('-', 10 - filledSlots).Pastel("#333333");

			// Colored Brackets
			string openB = "[".Pastel("#FFD700");
			string closeB = "]".Pastel("#FFD700");
			string xpBar = openB + filledPart + emptyPart + closeB;

			string xpString = $"{p.Experience}/{p.ExperienceToLevel}";

			Clear();

			// 3. Layout and Color Settings
			int screenWidth = 85;
			int boxWidth = 44;
			int center = screenWidth / 2;
			int startX = center - (boxWidth / 2);

			string bColor = "#125874"; // Border Blue
			string vColor = "#FFD700"; // Value Gold
			string lvlColor = "#BC1DBC"; // Level Purple

			// 4. Build the Stats List Dynamically
			List<string> statLines = new List<string>();

			statLines.Add("╔══════════════════════════════════════════╗".Pastel(bColor));
			statLines.Add(BuildStatLine("Level", playerLevel.ToString(), lvlColor, bColor));
			statLines.Add(BuildStatLine("Health", $"{p.Health}/{p.HitPoints}", vColor, bColor));
			statLines.Add(BuildStatLine("Coins", p.Coins.ToString(), vColor, bColor));
			statLines.Add(BuildStatLine("XP", xpString, vColor, bColor));
			statLines.Add(BuildBarLine("Progress", xpBar, bColor));

			statLines.Add("║                                          ║".Pastel(bColor));

			// --- DYNAMIC ABILITIES: Only show if they are > 0 ---

			if (p.Abilities.Strength > 0)
				statLines.Add(BuildStatLine("Strength", p.Abilities.Strength.ToString(), vColor, bColor));

			if (p.Abilities.Dexterity > 0)
				statLines.Add(BuildStatLine("Dexterity", p.Abilities.Dexterity.ToString(), vColor, bColor));

			if (p.Abilities.Intelligence > 0)
				statLines.Add(BuildStatLine("Intelligence", p.Abilities.Intelligence.ToString(), vColor, bColor));

			if (p.Abilities.Wisdom > 0)
				statLines.Add(BuildStatLine("Wisdom", p.Abilities.Wisdom.ToString(), vColor, bColor));

			if (p.Abilities.Perception > 0)
				statLines.Add(BuildStatLine("Perception", p.Abilities.Perception.ToString(), vColor, bColor));

			if (p.Abilities.Luck > 0)
				statLines.Add(BuildStatLine("Luck", p.Abilities.Luck.ToString(), vColor, bColor));

			// These will stay hidden because Marie has 0 in them
			if (p.Abilities.Charisma > 0)
				statLines.Add(BuildStatLine("Charisma", p.Abilities.Charisma.ToString(), vColor, bColor));

			if (p.Abilities.Constitution > 0)
				statLines.Add(BuildStatLine("Constitution", p.Abilities.Constitution.ToString(), vColor, bColor));

			statLines.Add("╚══════════════════════════════════════════╝".Pastel(bColor));
			statLines.Add("           PRESS ANY KEY TO RETURN           ");

			// 5. Draw Header
			string header = $"{p.PlayerClass.ToUpper()}: {p.PlayerName}";
			int headerPadding = header.Length / 2;
			SetCursorPosition(Math.Max(0, center - headerPadding), 4);
			WriteLine(header.Pastel(vColor));

			// 6. Draw the Box
			int row = 7;
			foreach (string line in statLines)
			{
				SetCursorPosition(startX, row++);
				WriteLine(line);
			}

			ReadKey(true);
		}

		private static string BuildStatLine(string label, string value, string valColor, string borderColor)
		{
			string leftWall = "║ ".Pastel(borderColor);
			string rightWall = " ║".Pastel(borderColor);
			string labelPart = (label + ":").PadRight(15);

			int remainingSpace = 40 - (15 + value.Length);
			string padding = new string(' ', Math.Max(0, remainingSpace));

			return leftWall + labelPart + value.Pastel(valColor) + padding + rightWall;
		}

		private static string BuildBarLine(string label, string bar, string borderColor)
		{
			string leftWall = "║ ".Pastel(borderColor);
			string rightWall = " ║".Pastel(borderColor);
			string labelPart = (label + ":").PadRight(15);

			// Visible length of [■■■-------] is always 12
			string padding = new string(' ', 13);

			return leftWall + labelPart + bar + padding + rightWall;
		}

		public static void CheckLevelUp()
		{
			var p = GameState.CurrentPlayer;
			if (p.Level > p.Mods)
			{
				p.Mods = p.Level;
				WriteLine("\n" + "!!! THE WORLD GROWS MORE DANGEROUS !!!".Pastel("#FF0000"));
				WriteLine($"Difficulty Scale increased to Rank {p.Mods}!".Pastel("#FF4500"));
				p.Health = p.HitPoints;
				WriteLine("Your health has been restored!".Pastel("#00FF00"));
				ReadKey(true);
			}
		}
	}
}