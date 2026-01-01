using DungeonAdventures.Src.Adventures.Interfaces;
using Newtonsoft.Json;
using static System.Console;

namespace DungeonAdventures.Src.Adventures.Services
{
	public class AdventureService : IAdventureService
	{
		// Use the FULL path in the return type to satisfy the Interface perfectly
		public List<DungeonAdventures.Src.GameData.AdventureData> LoadAllQuests()
		{
			// Explicitly use the GameData version to avoid "Shadowing" errors
			var quests = new List<DungeonAdventures.Src.GameData.AdventureData>();

			var basePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "adventures");

			if (!Directory.Exists(basePath))
			{
				Directory.CreateDirectory(basePath);
				return quests;
			}

			string[] questFiles = Directory.GetFiles(basePath, "*.json");

			foreach (var questFile in questFiles)
			{
				try
				{
					string json = File.ReadAllText(questFile);
					// Deserialize specifically into the GameData class
					var quest = JsonConvert.DeserializeObject<DungeonAdventures.Src.GameData.AdventureData>(json);
					if (quest != null) quests.Add(quest);
				}
				catch (Exception ex)
				{
					WriteLine($"Failed to load {questFile}: {ex.Message}");
				}
			}
			return quests;
		}
	}
}