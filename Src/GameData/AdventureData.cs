namespace DungeonAdventures.Src.GameData
{
	[Serializable]
	public class AdventureData
	{
		// The display name used by the Warp Menu Search Bar
		public string MapName { get; set; } = "New Adventure";

		// The internal file path for Conductor.SwitchMap
		public string ScenePath { get; set; } = "Maps/DefaultLevel.json";

		// Metadata for the UI list
		public string Difficulty { get; set; } = "Normal";
		public int RecommendedLevel { get; set; } = 1;

		// To allow the Search Bar to look for "Fire" or "Dungeon" tags
		public List<string> Tags { get; set; } = new List<string>();

		public string? Title { get; set; }
		public string? DescriptionD { get; set; }
		public string? GUID { get; set; }
		public int CompletionXPReward { get; set; }
		public int CompletionGold { get; set; }
		public int MaxLevel { get; set; }
		public int MinimumLevel { get; set; }
		public string? StartRoomID { get; set; } // New property for the starting room

		// --- UI AND MODDING ADDITIONS ---
		// This lets friends set a specific color for their quest title
		public string? TitleColor { get; set; }

		// This allows the UI to know which ASCII art to display
		public string? ArtHeaderName { get; set; }

		// A list of custom "Stages" (Story text and Enemy links)
		public List<QuestStage>? Stages { get; set; }
	}

	public class QuestStage
	{
		public string? StoryText { get; set; }
		public string? EnemyId { get; set; } // Link to your hardcoded Bosses or Custom JSONs
		public string? HexColor { get; set; } // Color for this specific part of the story
	}
}
