namespace DungeonAdventures.Src.GameData
{
	// Note: If you want Gems to have a Name, Value, and Weight, 
	// you should inherit from ItemData like your WeaponData does.
	public class GemData : ItemData
	{
		// REMOVED: The JsonConverter attribute that was causing the crash
		public List<EffectType> Attributes { get; set; } = new List<EffectType>();
		public float Power { get; set; } = 0f;

		public GemData() : base()
		{
			Type = "Gem";
			Rarity = ItemRarity.Uncommon;
		}
	}
}