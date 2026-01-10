using DungeonAdventures.Src.Game.MainInterfaces;
using DungeonAdventures.Src.Utilities.UI;
using Pastel;
using static System.Console;
using DungeonAdventures.Src.GameData;
using DungeonAdventures.Src.GameData.Components;
using static DungeonAdventures.Src.GameData.Entities.PlayerData;

namespace DungeonAdventures.Src.Game.Interfaces
{
	internal class CharacterCreation
	{
		public static void Start()
		{
			if (GameState.CurrentPlayer == null) GameState.CurrentPlayer = new loadPlayer();
			var player = GameState.CurrentPlayer;

			int screenWidth = Console.WindowWidth;
			Clear();

			// Logic is now self-contained; no more external UiFunctions calls here
			string subHeader = "--- NEW CHARACTER ---";
			string prompt = "Name your hero: ";

			SetCursorPosition((screenWidth / 2) - (subHeader.Length / 2), 5);
			WriteLine(subHeader.Pastel("#125874"));

			int promptX = (screenWidth / 2) - (prompt.Length / 2);
			SetCursorPosition(promptX, 7);
			Write(prompt.Pastel("#FFD700"));

			CursorVisible = true;
			ForegroundColor = ConsoleColor.Cyan;
			string? nameInput = ReadLine();
			ResetColor();

			player.PlayerName = string.IsNullOrWhiteSpace(nameInput) ? "Hero" : nameInput;
			CursorVisible = false;

			string[] coreArchetypes = { "Warrior", "Rogue", "Mage" };
			int selectedIndex = 0;
			bool pickingBase = true;

			int boxW = 60;
			int boxX = (screenWidth / 2) - (boxW / 2);
			int boxY = 4;

			while (pickingBase)
			{
				Clear();

				DrawOuterBox(boxX, boxY, boxW, 12, "#125874");

				string greeting = $"Greetings, {player.PlayerName}";
				SetCursorPosition((screenWidth / 2) - (greeting.Length / 2), boxY + 1);
				Write(greeting.Pastel("#FFD700"));

				for (int i = 0; i < coreArchetypes.Length; i++)
				{
					// 1. Build the raw text first to calculate the true visual center
					string rawText = (i == selectedIndex) ? $"[ {coreArchetypes[i].ToUpper()} ]" : $"  {coreArchetypes[i]}  ";

					// 2. Calculate the X position based on the REAL length of the text
					// We use screenWidth (94) to ensure it stays anchored to your console size
					int visualLength = rawText.Length;
					int classX = (screenWidth / 2) - (visualLength / 2);

					// 3. Set the position and THEN apply the color
					SetCursorPosition(classX, boxY + 4 + i);

					if (i == selectedIndex)
						Write(rawText.Pastel("#00FF00")); // Highlighted green
					else
						Write(rawText.Pastel("#555555")); // Dimmed gray
				}


				string hint = "[UP/DOWN] to browse | [ENTER] to specialize";
				SetCursorPosition((screenWidth / 2) - (hint.Length / 2), 17);
				Write(hint.Pastel("#333333"));

				var key = ReadKey(true).Key;
				if (key == ConsoleKey.UpArrow) selectedIndex = (selectedIndex == 0) ? coreArchetypes.Length - 1 : selectedIndex - 1;
				if (key == ConsoleKey.DownArrow) selectedIndex = (selectedIndex == coreArchetypes.Length - 1) ? 0 : selectedIndex + 1;
				if (key == ConsoleKey.Enter) pickingBase = false;
			}

			player.PlayerClass = coreArchetypes[selectedIndex];

			CharSubMenus.ChooseSubClass();

			InitializeAbilities(player);
			PointDistribution(player, screenWidth);
			AssignStarterKit(player);

			Clear();
			int centerX = (screenWidth / 2); // Change (80 / 2) to (screenWidth / 2)

			// Use StripAnsi to get the REAL center of the welcome message
			string welcomeMsg = $"Welcome to the world, {player.PlayerName.Pastel("#00FF00")}!";
			int welcomeVisualLength = UiEngine.StripAnsi(welcomeMsg).Length;
			SetCursorPosition(centerX - (welcomeVisualLength / 2), 8);
			WriteLine(welcomeMsg);

			// Center the journey message
			string journeyMsg = $"Your journey as a {player.PlayerClass.Pastel("#FFD700")} begins now...";
			int journeyVisualLength = UiEngine.StripAnsi(journeyMsg).Length;
			SetCursorPosition(centerX - (journeyVisualLength / 2), 10);
			WriteLine(journeyMsg.Pastel("#DCDCDC"));

			// Center the "Starter gear" message too
			string gearMsg = "Starter gear has been added to your pack.";
			SetCursorPosition(centerX - (gearMsg.Length / 2), 12);
			WriteLine(gearMsg);

			SetCursorPosition(centerX - 12, 14);
			WriteLine("\n    --- INITIAL INVENTORY ---".Pastel("#125874"));

			int itemRow = 16;
			foreach (var item in player.Inventory)
			{
				SetCursorPosition(centerX - 10, itemRow++);
				WriteLine($"- {item.Name}".Pastel("#DCDCDC"));
			}

			Thread.Sleep(2000);
			GameState.Sync();

			// Animation kept here as it's the final transition
			UiFunctions.ShowCreationAnimation(player.PlayerName, player.PlayerClass);
		}

		private static void DrawOuterBox(int x, int y, int w, int h, string color)
		{
			string topBorder = "╔" + new string('═', w - 2) + "╗";
			string bottomBorder = "╚" + new string('═', w - 2) + "╝";
			string side = "║";

			SetCursorPosition(x, y);
			Write(topBorder.Pastel(color));
			for (int i = 1; i < h - 1; i++)
			{
				SetCursorPosition(x, y + i);
				Write(side.Pastel(color));
				SetCursorPosition(x + w - 1, y + i);
				Write(side.Pastel(color));
			}
			SetCursorPosition(x, y + h - 1);
			Write(bottomBorder.Pastel(color));
		}

		private static void PointDistribution(loadPlayer player, int screenWidth)
		{
			int pointsLeft = 10;
			string[] statNames = { "Strength", "Dexterity", "Intelligence", "Constitution", "Perception", "Luck", "Wisdom", "Charisma" };
			int selectedStat = 0;
			bool picking = true;

			while (picking)
			{
				string classDesc = "Distribute your attribute points to define your hero.";
				string? pClass = player.PlayerClass;

				if (pClass == "Paladin" || pClass == "Berserker" || pClass == "Knight")
					classDesc = "Focus on Strength and Constitution for a powerful front-line fighter.";
				else if (pClass == "Assassin" || pClass == "Ranger" || pClass == "Thief")
					classDesc = "Focus on Dexterity and Luck to strike fast and evade danger.";
				else if (pClass == "Warlock" || pClass == "Druid" || pClass == "Wizard")
					classDesc = "Focus on Intelligence and Wisdom to master the arcane arts.";

				Clear();
				// Building the header locally to maintain alignment
				WriteLine("=== Build your Hero ===".Pastel("#FFD700"));
				WriteLine(new string('-', screenWidth).Pastel("#125874"));

				SetCursorPosition(4, 3);
				WriteLine(" CLASS OVERVIEW ".Pastel("#000000").PastelBg("#125874"));
				SetCursorPosition(4, 4);
				WriteLine($"{classDesc}".Pastel("#DCDCDC"));

				player.Health = 100 + (player.Abilities.Constitution * 5);
				player.HitPoints = player.Health;

				SetCursorPosition(4, 7);
				WriteLine($"Path: {player.PlayerClass.Pastel("#FFD700")}");

				string progressBar = new string('■', Math.Max(0, 10 - pointsLeft)).Pastel("#00FF00") +
									 new string('?', Math.Max(0, pointsLeft)).Pastel("#333333");

				SetCursorPosition(4, 8);
				WriteLine($"Points Spent: [{progressBar}] | {pointsLeft} Remaining");

				for (int i = 0; i < statNames.Length; i++)
				{
					SetCursorPosition(8, 11 + i);
					int currentVal = GetStatValueByIndex(player, i);

					// Build the string first, THEN color it to keep alignment perfect
					string line = (i == selectedStat)
						? $"> {statNames[i].PadRight(15)} [{currentVal}] <"
						: $"  {statNames[i].PadRight(15)} [{currentVal}]  ";

					Write(i == selectedStat ? line.Pastel("#00FF00") : line.Pastel("#555555"));
				}

				// The Attributes box is handled by UiEngine as per your setup
				int attributeBoxX = 52;
				UiEngine.DrawAttributeBox(player, attributeBoxX, 11, selectedStat);

				var key = ReadKey(true).Key;
				if (key == ConsoleKey.UpArrow) selectedStat = (selectedStat == 0) ? statNames.Length - 1 : selectedStat - 1;
				if (key == ConsoleKey.DownArrow) selectedStat = (selectedStat == statNames.Length - 1) ? 0 : selectedStat + 1;
				if (key == ConsoleKey.RightArrow && pointsLeft > 0) { AdjustStatByIndex(player, selectedStat, 1); pointsLeft--; }
				if (key == ConsoleKey.LeftArrow && GetStatValueByIndex(player, selectedStat) > 0) { AdjustStatByIndex(player, selectedStat, -1); pointsLeft++; }
				if (key == ConsoleKey.Enter && pointsLeft == 0) picking = false;
			}
		}

		private static int GetStatValueByIndex(loadPlayer p, int index) => index switch
		{
			0 => p.Abilities.Strength,
			1 => p.Abilities.Dexterity,
			2 => p.Abilities.Intelligence,
			3 => p.Abilities.Constitution,
			4 => p.Abilities.Perception,
			5 => p.Abilities.Luck,
			6 => p.Abilities.Wisdom,
			7 => p.Abilities.Charisma,
			_ => 0
		};

		private static void AdjustStatByIndex(loadPlayer p, int index, int amount)
		{
			if (index == 0) p.Abilities.Strength += amount;
			else if (index == 1) p.Abilities.Dexterity += amount;
			else if (index == 2) p.Abilities.Intelligence += amount;
			else if (index == 3) p.Abilities.Constitution += amount;
			else if (index == 4) p.Abilities.Perception += amount;
			else if (index == 5) p.Abilities.Luck += amount;
			else if (index == 6) p.Abilities.Wisdom += amount;
			else if (index == 7) p.Abilities.Charisma += amount;
		}

		private static void InitializeAbilities(loadPlayer player)
		{
			player.Abilities.Strength = 0; player.Abilities.Dexterity = 0; player.Abilities.Intelligence = 0; player.Abilities.Constitution = 0;
			player.Abilities.Perception = 0; player.Abilities.Luck = 0; player.Abilities.Wisdom = 0; player.Abilities.Charisma = 0;

			switch (player.PlayerClass)
			{
				case "Paladin":
				case "Knight": player.Abilities.Strength = 5; player.Abilities.Constitution = 3; break;
				case "Berserker": player.Abilities.Strength = 8; break;
				case "Assassin":
				case "Thief": player.Abilities.Dexterity = 5; player.Abilities.Luck = 3; break;
				case "Wizard":
				case "Warlock": player.Abilities.Intelligence = 8; break;
				case "Druid": player.Abilities.Intelligence = 4; player.Abilities.Wisdom = 4; break;
			}
		}

		private static void AssignStarterKit(loadPlayer player)
		{
			player.Inventory.Clear();
			switch (player.PlayerClass)
			{
				case "Paladin":
				case "Knight":
					player.Coins += 25;
					player.Inventory.Add(new ItemData("Rusty Mace", 15, 5.0f, 1, "Weapon", ItemRarity.Common, Enum.TryParse<EffectType>("None", true, out var effect1) ? effect1 : EffectType.None, 100, 0, false, 0));
					player.Inventory.Add(new ItemData("Small Shield", 20, 8.0f, 1, "Armor", ItemRarity.Common, Enum.TryParse<EffectType>("None", true, out var effect2) ? effect2 : EffectType.None, 150, 0, false, 0));
					break;
				case "Berserker":
					player.Abilities.Strength += 2;
					player.Inventory.Add(new ItemData("Chipped Axe", 12, 7.0f, 1, "Weapon", ItemRarity.Common, Enum.TryParse<EffectType>("None", true, out var effect3) ? effect3 : EffectType.None, 80, 0, false, 0));
					break;
				case "Assassin":
				case "Thief":
					player.Abilities.Dexterity += 2;
					player.Inventory.Add(new ItemData("Dull Dagger", 10, 1.0f, 1, "Weapon", ItemRarity.Common, Enum.TryParse<EffectType>("None", true, out var effect4) ? effect4 : EffectType.None, 60, 0, false, 0));
					player.Inventory.Add(new ItemData("Smoke Bomb", 25, 0.5f, 3, "Consumable", ItemRarity.Uncommon, Enum.TryParse<EffectType>("Blind", true, out var effect5) ? effect5 : EffectType.None, 0, 1, true, 0));
					break;
				case "Wizard":
				case "Warlock":
					player.Coins += 10;
					player.Inventory.Add(new ItemData("Old Staff", 20, 3.0f, 1, "Weapon", ItemRarity.Common, Enum.TryParse<EffectType>("Magic", true, out var effect6) ? effect6 : EffectType.None, 100, 0, false, 0));
					player.Inventory.Add(new ItemData("Mana Potion", 50, 0.2f, 2, "Consumable", ItemRarity.Common, Enum.TryParse<EffectType>("Restore", true, out var effect7) ? effect7 : EffectType.None, 0, 1, true, 0));
					break;
				case "Druid":
					player.Abilities.Wisdom += 2;
					player.Inventory.Add(new ItemData("Wooden Branch", 5, 2.0f, 1, "Weapon", ItemRarity.Common, Enum.TryParse<EffectType>("None", true, out var effect8) ? effect8 : EffectType.None, 40, 0, false, 0));
					break;
			}
		}
	}
}