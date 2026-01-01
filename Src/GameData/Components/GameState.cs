using DungeonAdventures.Src.GameData.Entities;
using DungeonAdventures.Src.GameEngine;
using static DungeonAdventures.Src.GameData.Entities.PlayerData; // Correct namespace for AdventureData

namespace DungeonAdventures.Src.GameData.Components
{
	



	public class GameState
	{
		
		// 1. Data registration
		public static readonly string[] DataFolders = { "profiles", "items", "weapons", "gems", "bosses", "quests", "dungeons", "adventures", "settings" }; public static string BossPath => GetGlobalPath("bosses");
		// 2. Paths
		public static string GemPath => GetGlobalPath("gems");
		public static string MasterPath = Path.GetFullPath(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "DungeonAdventures", "Data"));
		// 3. Properties (ONLY ONE DEFINITION EACH)
		public static loadPlayer CurrentPlayer { get; set; } = null;
		public static AdventureData? CurrentAdventure { get; set; } = null;
		public static BossData? CurrentBoss { get; set; } = null;

		// 4. System States
		public static bool IsDevMode { get; set; } = true;
		public static string BuildVersion { get; set; } = "2.0.5";
		public static bool StateDirty { get; set; } = false;
		public static string ActiveSavePath { get; set; } = "";
		public static string DevIteration { get; set; } = "ALPHA-V2";

		public static string? CurrentLocation { get; set; } = null; // Current active scene/map
		public static bool SceneChangeRequested { get; set; } = false; // Flag to request a scene change


		// ---------------------------------------


		public static void EnsureDirectories()
		{
			if (!Directory.Exists(MasterPath)) Directory.CreateDirectory(MasterPath);
			foreach (var folder in DataFolders)
			{
				string subPath = Path.Combine(MasterPath, folder.ToLower());
				if (!Directory.Exists(subPath)) Directory.CreateDirectory(subPath);
			}
		}
		

		public static void Sync()
		{
			if (CurrentPlayer == null || string.IsNullOrWhiteSpace(CurrentPlayer.PlayerName)) return;

			string safeName = CurrentPlayer.PlayerName.Replace(" ", "_").ToLower();
			string fileName = $"{safeName}.json";

			// Calling SaveGame.SaveData() to centralize saving logic
			// The 'profiles' folder should be the target subfolder.
			SaveGame.SaveData(fileName, CurrentPlayer, "profiles");
		}

		public static string GetActiveProfileFolder()
		{
			return Path.Combine(MasterPath, "profiles");
		}

		public static string GetGlobalPath(string folder) => Path.Combine(MasterPath, folder.ToLower());
	}
}