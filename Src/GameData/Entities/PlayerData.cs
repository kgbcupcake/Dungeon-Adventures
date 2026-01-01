namespace DungeonAdventures.Src.GameData.Entities
{
	public class PlayerData
	{

		public class loadPlayer
		{
			private Random rand = new Random();

			public string PlayerName { get; set; } = "Unnamed Hero";
			public string PlayerClass { get; set; } = "Warrior";
			public int Level { get; set; }
			private int _health = 100;
			public int Health
			{
				get => _health;
				set => _health = Math.Clamp(value, 0, HitPoints);
			}
			public int HitPoints { get; set; } = 100;
			public int Experience { get; set; } = 0;
			public int ExperienceToLevel { get; set; } = 100;
			public int Damage { get; set; } = 10;
			public int WeaponValue { get; set; } = 0; public int ArmorValue { get; set; }
			public int WeaponHealth { get; set; }
			public int ArmorHealth { get; set; }
			public int Coins { get; set; } = 50;
			public int Mods { get; set; } = 0;
			public int Potion { get; set; } = 5;
			public int Lockpicks { get; set; } = 0;

			public Abilities Abilities { get; set; } = new Abilities();

			public List<ItemData> Inventory { get; set; } = new List<ItemData>();
			public List<WeaponData> Weapons { get; set; } = new List<WeaponData>();

			// --- NEW IMPLEMENTATION: SOCKET STAT CALCULATION ---
			// This ensures your Damage/Health values factor in socketed gems dynamically
			public float GetTotalDamage()
			{
				float baseDmg = Damage;
				// Sum up all damage-related gems in all inventory items
				float gemBonus = Inventory
					.SelectMany(i => i.SocketedGems)
					.SelectMany(g => g.Attributes.Select(attr => new { Gem = g, Attribute = attr }))
					.Where(x => x.Attribute == EffectType.Fire || x.Attribute == EffectType.Ice || x.Attribute == EffectType.Poison || x.Attribute == EffectType.Electric)
					.Sum(x => x.Gem.Power);

				return baseDmg + gemBonus;
			}

			public float GetTotalHealth()
			{
				float baseHealth = Health;
				// Sum up all health-related gems in all inventory items
				float gemBonus = Inventory
					.SelectMany(i => i.SocketedGems)
					.SelectMany(g => g.Attributes.Select(attr => new { Gem = g, Attribute = attr }))
					.Where(x => x.Attribute == EffectType.Restorative || x.Attribute == EffectType.Defense) // Assuming these affect health
					.Sum(x => x.Gem.Power);

				return baseHealth + gemBonus;
			}

			public int GetPower() => rand.Next(Mods + 1, 2 * Mods + 3);
			public int GetCoins() => rand.Next(10 * Mods + 10, 15 * Mods + 50);
			public int GetHealth() => rand.Next(Mods + 2, 2 * Mods + 5);
		}




		public class Abilities
		{
			public int Strength { get; set; } = 0;
			public int Dexterity { get; set; } = 0;
			public int Intelligence { get; set; } = 0;
			public int Wisdom { get; set; } = 0;
			public int Charisma { get; set; } = 0;
			public int Constitution { get; set; } = 0;
			public int Perception { get; set; } = 0;
			public int Luck { get; set; } = 0;
		}









	}
}
