using DungeonAdventures.Src.GameData.Components;
using DungeonAdventures.Src.Interfaces; // Added for DevLog
using System.Text;
using static DungeonAdventures.Src.GameData.Entities.PlayerData;

namespace DungeonAdventures.Src.GameEngine
{
	/// <summary>
	/// Streamlined Conductor: Manages game data strictly in the User Documents folder.
	/// This prevents MSBuild from seeing JSON files as resources and crashing.
	/// </summary>
	public class Conductor
	{
		private readonly string _masterPath = GameState.MasterPath;
		public Conductor()
		{
			InitializeDirectories();
		}

		private void InitializeDirectories()
		{
			GameState.EnsureDirectories(); // Simply call GameState.EnsureDirectories()
		}


		// Fixes the Player Creator issue
		public void CreateNewPlayer(string name, string heroClass)
		{
			var newHero = new loadPlayer
			{
				PlayerName = name,
				PlayerClass = heroClass,
				Level = 1,
				Health = 100,
				HitPoints = 100
			};

			GameState.CurrentPlayer = newHero;
			GameState.Sync(); // Immediately writes to /profiles/
			DevGuiRenderer.DevLog.Write($"[CONDUCTOR] Hero {name} initialized and saved.", "SYSTEM");
		}

		/// <summary>
		/// Requests a scene change and updates GameState.
		/// The actual scene loading/unloading should be handled by the main game loop.
		/// </summary>
		/// <param name="newScenePath">The path or identifier of the new scene to load.</param>
		public void SwitchMap(string newScenePath)
		{
			// Basic validation: ensure the path is not empty
			if (string.IsNullOrWhiteSpace(newScenePath))
			{
				Console.WriteLine("[Conductor] Error: newScenePath cannot be empty.");
				return;
			}

			// In a real game, here you might:
			// 1. Trigger an event for scene unloading
			// 2. Perform any necessary cleanup for the current scene
			Console.WriteLine($"[Conductor] Disposing of current scene (placeholder)...");

			// Update GameState to reflect the requested new location
			GameState.CurrentLocation = newScenePath;
			GameState.SceneChangeRequested = true;

			// In a real game, here you might:
			// 1. Trigger an event for scene loading
			// 2. Load assets for the new scene
			Console.WriteLine($"[Conductor] Initializing new scene '{newScenePath}' (placeholder)...");
		}
        
        		public void LoadAdventure(DungeonAdventures.Src.GameData.AdventureData adventureData)
                {
                    if (adventureData == null)
                    {
                        DevGuiRenderer.DevLog.Write("[Conductor] Error: Attempted to load a null adventure.", "ERROR");
                        return;
                    }
        
                    if (string.IsNullOrWhiteSpace(adventureData.ScenePath))
                    {
                        DevGuiRenderer.DevLog.Write($"[Conductor] Warning: Adventure '{adventureData.MapName}' has no ScenePath. Setting CurrentLocation to 'UNKNOWN'.", "WARNING");
                        GameState.CurrentLocation = "UNKNOWN"; // Fallback
                    }
                    else
                    {
                        GameState.CurrentLocation = adventureData.ScenePath;
                        DevGuiRenderer.DevLog.Write($"[Conductor] Loading adventure '{adventureData.MapName}'. Setting CurrentLocation to '{adventureData.ScenePath}'.", "SYSTEM");
                    }
                    
                    GameState.SceneChangeRequested = true;
                    GameState.CurrentAdventure = adventureData; // Assuming CurrentAdventure exists or needs to be added to GameState
                }
        
        		/// <summary>
        		/// Generic method to export any data object to a specified category and filename.
        		/// This serves as the primary interface for the UI to save data to disk.
        		/// </summary>
        		/// <typeparam name="T">The type of data to export.</typeparam>
        		/// <param name="data">The data object to export.</param>
        		/// <param name="category">The subfolder category within the master path (e.g., "profiles", "items").</param>
        		/// <param name="fileName">The name of the file (without extension) to save the data to.</param>
        		public void ExportData<T>(T data, string category, string fileName)
        		{
        			try
        			{

        				SaveGame.ExportToMaster(data, category, fileName);
        				DevGuiRenderer.DevLog.Write($"[CONDUCTOR] Successfully exported '{fileName}' to category '{category}'.", "INFO");
        			}
        			catch (Exception ex)
        			{
        				DevGuiRenderer.DevLog.Write($"[CONDUCTOR ERROR] Failed to export '{fileName}' to category '{category}': {ex.Message}", "ERROR");
        			}
        		}
        	}
        }
        