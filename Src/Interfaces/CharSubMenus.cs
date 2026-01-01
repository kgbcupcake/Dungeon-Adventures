using DungeonAdventures.Src.GameData.Components;
using DungeonAdventures.Src.Utilities.UI;
using Pastel;
using System;
using System.Collections.Generic;
using static System.Console;

namespace DungeonAdventures.Src.Game.MainInterfaces
{
	internal class CharSubMenus
	{
		public static void ChooseSubClass()
		{
			var player = GameState.CurrentPlayer;
			string[] subClasses; string[] descriptions; string flavorColor;

			switch (player.PlayerClass)
			{
				case "Warrior":
					subClasses = new string[] { "Paladin", "Berserker", "Knight" }; flavorColor = "#FF4500";
					descriptions = new string[] { "Holy defender.", "Bloodthirsty brute.", "Master of armor." };
					break;
				case "Rogue":
					subClasses = new string[] { "Assassin", "Ranger", "Thief" }; flavorColor = "#9370DB";
					descriptions = new string[] { "Deadly crits.", "Master of bow.", "Expert thief." };
					break;
				case "Mage":
					subClasses = new string[] { "Warlock", "Druid", "Wizard" }; flavorColor = "#00FFFF";
					descriptions = new string[] { "Life drain.", "Nature balance.", "Glass cannon." };
					break;
				default: return;
			}

			int selected = 0;
			while (true)
			{
				Clear();

				// 1. Prepare the list of options so the Box expands to fit them
				List<string> evolutionOptions = new List<string>();
				for (int i = 0; i < subClasses.Length; i++)
				{
					evolutionOptions.Add(subClasses[i]);
				}

				// 2. Draw the Massive Frame using the options list
				// This ensures the box is tall enough to hold everything
				UiEngine.DrawDynamicFrame($"{player.PlayerClass.ToUpper()} EVOLUTION: {player.PlayerName.ToUpper()}",
										 evolutionOptions,
										 "[UP/DOWN] to browse | [ENTER] to evolve",
										 boxWidth: 70, // Explicitly set a reasonable width
										 selectedIndex: selected);

				// 3. Draw the Sub-Header (Greetings/Choose Path) ABOVE the box
				UiEngine.DrawCentered($"Choose your path, {player.PlayerName}".Pastel(flavorColor), 6);

				// 4. Draw the Attributes/Description BELOW the options but INSIDE the frame area
				// Since DrawDynamicFrame with 3 options is about 6 rows high, we place these relative to startY
				int contentBaseY = 15;
				UiEngine.DrawCentered(" ATTRIBUTES ".Pastel("#000000").PastelBg(flavorColor), contentBaseY);
				UiEngine.DrawCentered(descriptions[selected].Pastel("#DCDCDC"), contentBaseY + 2);

				var key = ReadKey(true).Key;
				if (key == ConsoleKey.UpArrow) selected = (selected == 0) ? subClasses.Length - 1 : selected - 1;
				else if (key == ConsoleKey.DownArrow) selected = (selected == subClasses.Length - 1) ? 0 : selected + 1;
				else if (key == ConsoleKey.Enter) { player.PlayerClass = subClasses[selected]; break; }
			}
		}
	}
}