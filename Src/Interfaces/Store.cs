using System;
using System.Collections.Generic;
using System.Threading;
using DungeonAdventures.Src.GameData.Components;
using DungeonAdventures.Src.Utilities.UI;
using Pastel;
using static System.Console;
using static DungeonAdventures.Src.GameData.Entities.PlayerData;

// FIX: This now matches the namespace your Game Engine is calling
namespace DungeonAdventures.Src.GameEngine.Interfaces
{
	public class Store
	{
		// Colors for the Retro Dungeon Theme
		private static string dungeonPurple = "#2E0E4E";
		private static string torchGold = "#FFAB00";

		public static void LoadShop()
		{
			// 1. SPICY ENTRANCE
			UiEngine.DrawLoadingScreen("UNPACKING CARGO...", 1000);

			var p = GameState.CurrentPlayer;
			UpdateAndRun(p);
		}

		private static void UpdateAndRun(loadPlayer p)
		{
			bool shopping = true;
			while (shopping)
			{
				Clear();
				UiFunctions.TitleBar();

				int potionP = 20 + 10 * p.Potion;
				int armorP = 100 * (p.ArmorValue + 1);
				int weaponP = 100 * (p.WeaponValue + 1);
				int lockP = 50 + 10 * p.Lockpicks;
				int difP = 300 + 100 * p.Mods;

				// 2. BUILD THE MENU
				string[] shopLines = {
					"╔══════════════════════════════════════════╗".Pastel(dungeonPurple),
					BuildLine("(P)otion", potionP, torchGold, dungeonPurple),
					BuildLine("(W)eapon Upgrade", weaponP, torchGold, dungeonPurple),
					BuildLine("(A)rmor Upgrade", armorP, torchGold, dungeonPurple),
					BuildLine("(L)ockpicks", lockP, torchGold, dungeonPurple),
					BuildLine("(D)ifficulty Mod", difP, torchGold, dungeonPurple),
					"║                                          ║".Pastel(dungeonPurple),
					"║ " + "(E)xit to Town Square".PadRight(40) + " ║".Pastel(dungeonPurple),
					"╚══════════════════════════════════════════╝".Pastel(dungeonPurple)
				};

				foreach (var line in shopLines) UiEngine.DrawCentered(line);
				UiFunctions.DisplayFooter();

				// 3. INPUT HANDLING
				ConsoleKeyInfo key = ReadKey(true);
				switch (key.Key)
				{
					case ConsoleKey.P: TryBuy("Potion", potionP, p); break;
					case ConsoleKey.W: TryBuy("Weapon", weaponP, p); break;
					case ConsoleKey.A: TryBuy("Armor", armorP, p); break;
					case ConsoleKey.L: TryBuy("Lockpick", lockP, p); break;
					case ConsoleKey.D: TryBuy("Difficulty", difP, p); break;
					case ConsoleKey.E:
						UiEngine.DrawLoadingScreen("RETURNING TO TOWN...", 800);
						shopping = false;
						break;
				}
			}
		}

		private static void TryBuy(string item, int cost, loadPlayer p)
		{
			if (p.Coins >= cost)
			{
				p.Coins -= cost;
				if (item == "Potion") p.Potion++;
				else if (item == "Weapon") p.WeaponValue++;
				else if (item == "Armor") p.ArmorValue++;
				else if (item == "Difficulty") p.Mods++;
				else if (item == "Lockpick") p.Lockpicks++;

				UiEngine.DrawCentered($"{"SUCCESS:".Pastel("#00FF00")} Bought {item}!");
				Thread.Sleep(800);
			}
			else
			{
				UiEngine.DrawCentered($"{"FAILED:".Pastel("#880808")} Not enough gold!");
				Thread.Sleep(1000);
			}
		}

		private static string BuildLine(string label, int price, string priceColor, string borderColor)
		{
			string priceStr = price.ToString();
			string leftText = label.PadRight(18) + "Cost: ";
			int paddingNeeded = 40 - (18 + 6 + priceStr.Length);
			string spaces = new string(' ', paddingNeeded);

			return "║ ".Pastel(borderColor) +
				   leftText +
				   priceStr.Pastel(priceColor) +
				   spaces +
				   " ║".Pastel(borderColor);
		}
	}
}