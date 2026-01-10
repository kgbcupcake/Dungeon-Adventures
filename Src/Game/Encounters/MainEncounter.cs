using DungeonAdventures.Src.GameData.Components;
using DungeonAdventures.Src.Utilities.UI;
using System;
using System.Collections.Generic;
using System.Threading;
using static DungeonAdventures.Src.GameData.Entities.PlayerData;
using static System.Console;

namespace DungeonAdventures.Src.Game.Encounters
{
    public static class MainEncounter
    {
        static readonly Random rand = new Random();

        public static void FirstEncounter() { Combat(false, "Human Rogue", 1, 4); }

        public static void BasicFightEncounter()
        {
            var lines = new List<string> { "Raider: You Think You Can Defeat me?" };
            UiEngine.DrawDynamicFrame("Encounter", lines, "Press any key to continue...");
            ReadKey(true);
            Combat(true, "", 0, 0);
        }

		public static void Combat(bool random, string name, int power, int health)
		{
			var player = GameState.CurrentPlayer;
			string n = random ? GetName() : name;
			int p = random ? rand.Next(1, 5) : power;
			int h = random ? rand.Next(10, 21) : health;

			while (h > 0 && player.Health > 0)
			{
				var lines = new List<string>
				{
					$"Enemy: {n}",
					$"HP: {h}",
					$"Power: {p}",
					"--------------------",
					$"Player: {player.PlayerName}",
					$"HP: {player.Health}",
					$"Potions: {player.Potion} | Attack Power: {player.WeaponValue}"
				};

				UiEngine.DrawDynamicFrame("COMBAT", lines, "[A]ttack    [D]efend    [H]eal");

				char input = ReadKey(true).KeyChar;

				if (input == 'a')
				{
					// 1. Calculate damage
					int playerAttack = rand.Next(1, player.WeaponValue + 1) + (player.Abilities.Strength / 2);
					int enemyAttack = rand.Next(1, p + 1);

					int newPlayerHp = player.Health - enemyAttack;
					int newEnemyHp = h - playerAttack;

					// 2. Show damage preview
					var previewLines = new List<string>
					{
						$"{n} attacks you for {enemyAttack} damage!",
						$"You attack {n} for {playerAttack} damage!",
						"",
						"Press any key to continue..."
					};
					                    UiEngine.DrawDynamicFrame("COMBAT", previewLines, "", -1, newPlayerHp, -1);
					                    ReadKey(true);
					// 3. Apply damage
					player.Health = newPlayerHp;
					h = newEnemyHp;
				}
				else if (input == 'h')
				{
					if (player.Potion > 0)
					{
						int healAmount = 25;
						player.Health += healAmount;
						if (player.Health > 100) player.Health = 100;
						player.Potion--;

						var healLines = new List<string>
						{
							$"You used a potion and healed for {healAmount} HP.",
							"",
							"Press any key to continue..."
						};
						            UiEngine.DrawDynamicFrame("COMBAT", healLines, "", -1, player.Health, -1);
												ReadKey(true);					}
					else
					{
						var noPotionLines = new List<string>
						{
							"You are out of potions!",
							"",
							"Press any key to continue..."
						};
						UiEngine.DrawDynamicFrame("COMBAT", noPotionLines, "");
						ReadKey(true);
					}
				}
				// Defend logic can be added here
			}

			HandlePostCombat(player, n);
		}

		private static void HandlePostCombat(loadPlayer player, string n)
		{
			if (player.Health <= 0)
			{
				var lines = new List<string>
				{
					"You have been slain...",
					"Game Over."
				};
				                UiEngine.DrawDynamicFrame("DEFEAT", lines, "Press any key to exit...");
				                ReadKey(true);				Environment.Exit(0);
			}
			else
			{
				int reward = rand.Next(5, 20);
				int xpGained = rand.Next(30, 60);
				player.Coins += reward;
				player.Experience += xpGained;

				var lines = new List<string>
				{
					$"You defeated the {n}!",
					"",
					$"Found: {reward} Coins",
					$"Gained: {xpGained} XP"
				};
				UiEngine.DrawDynamicFrame("VICTORY", lines, "Press any key to continue...");
				ReadKey(true);

				if (player.Experience >= player.ExperienceToLevel)
				{
					LevelUp(player);
				}
			}
		}

		public static void LevelUp(loadPlayer player)
		{
			player.Level++;
			player.Experience -= player.ExperienceToLevel;
			player.ExperienceToLevel = (int)(player.ExperienceToLevel * 1.5);
			
			int points = 2;
			string[] stats = { "Strength", "Dexterity", "Intelligence", "Constitution", "Perception", "Luck", "Wisdom", "Charisma" };
			int selected = 0;

			while (points > 0)
			{
				var lines = new List<string>
				{
					$"You are now Level {player.Level}!",
					$"Points to spend: {points}",
					""
				};
				for (int i = 0; i < stats.Length; i++)
				{
					lines.Add(stats[i]);
				}

				UiEngine.DrawDynamicFrame("LEVEL UP", lines, "Use arrow keys and Enter to select.", selected + 3);

				var key = ReadKey(true).Key;
				if (key == ConsoleKey.UpArrow) selected = (selected == 0) ? stats.Length - 1 : selected - 1;
				if (key == ConsoleKey.DownArrow) selected = (selected == stats.Length - 1) ? 0 : selected + 1;
				if (key == ConsoleKey.Enter)
				{
					AdjustStat(player, selected);
					points--;
					player.Health = 100 + (player.Abilities.Constitution * 5);
				}
			}
		}

		private static void AdjustStat(loadPlayer p, int index)
		{
			if (index == 0) p.Abilities.Strength++;
			else if (index == 1) p.Abilities.Dexterity++;
			else if (index == 2) p.Abilities.Intelligence++;
			else if (index == 3) p.Abilities.Constitution++;
			else if (index == 4) p.Abilities.Perception++;
			else if (index == 5) p.Abilities.Luck++;
			else if (index == 6) p.Abilities.Wisdom++;
			else if (index == 7) p.Abilities.Charisma++;
		}

        public static string GetName()
        {
            string[] names = { "Skeleton", "Zombie", "Cultist", "Grave Robber" };
            return names[rand.Next(names.Length)];
        }
    }
}
