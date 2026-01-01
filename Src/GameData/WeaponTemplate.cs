using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using DungeonAdventures.Src.GameData;
using DungeonAdventures.Src.GameData.Components;
using DungeonAdventures.Src.Utilities.UI;
using static System.Console;

namespace DungeonAdventures.Src.GameEngine.GameData
{
	public class WeaponTemplate
	{
		public string ID { get; set; } = Guid.NewGuid().ToString();
		public string Name { get; set; } = "New Weapon";
		public int Damage { get; set; } = 10;
		public string Description { get; set; } = "No Description";
		public ItemRarity Rarity { get; set; } = ItemRarity.Common;

		public static List<WeaponTemplate> LoadWeaponTemplates()
		{
			string path = Path.Combine(GameState.MasterPath, "Saves", "weapons", "master_weapons.json");
			if (!File.Exists(path)) return new List<WeaponTemplate>();
			try
			{
				return JsonSerializer.Deserialize<List<WeaponTemplate>>(File.ReadAllText(path)) ?? new List<WeaponTemplate>();
			}
			catch { return new List<WeaponTemplate>(); }
		}

		public static void SaveToMaster(WeaponTemplate weapon)
		{
			string path = Path.Combine(GameState.MasterPath, "Saves", "weapons", "master_weapons.json");
			var list = LoadWeaponTemplates();
			list.Add(weapon);
			File.WriteAllText(path, JsonSerializer.Serialize(list, new JsonSerializerOptions { WriteIndented = true }));
		}
	}
}