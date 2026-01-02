using DungeonAdventures.Src.GameEngine.Interfaces;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using DungeonAdventures.Src.GameData.Components;
using DungeonAdventures.Src.Interfaces;

namespace DungeonAdventures.Src.GameEngine
{
	public static class SaveGame
	{
		// Points to the safe "Documents" folder defined in GameState
		private static string MasterRoot => GameState.MasterPath;

		/// <summary>
		/// Saves current GameState to the Documents folder.
		/// </summary>
		public static void Save() => GameState.Sync();

		/// <summary>
		/// Verifies the Documents folder is reachable and writable.
		/// </summary>
		public static bool RunSanityCheck()
		{
			try
			{
				if (!Directory.Exists(MasterRoot))
				{
					Directory.CreateDirectory(MasterRoot);
				}

				string testFile = Path.Combine(MasterRoot, "access_test.tmp");
				File.WriteAllText(testFile, "test");
				File.Delete(testFile);

				DevGuiRenderer._injectionSuccess = true;
				DevGuiRenderer._injectionMsg = $"[OK] Saving to: {MasterRoot}";
				return true;
			}
			catch (Exception ex)
			{
				DevGuiRenderer._injectionSuccess = false;
				DevGuiRenderer._injectionMsg = $"[FAIL] Path Unreachable: {ex.Message}";
				return false;
			}
		}

		/// <summary>
		/// Saves data directly to the Documents subfolder by delegating to ExportToMaster.
		/// </summary>
		public static void SaveData(string fileName, object data, string subFolder)
		{
			try
			{
				// Delegate to the robust ExportToMaster method
				ExportToMaster(data, subFolder, Path.GetFileNameWithoutExtension(fileName));
				DevGuiRenderer._injectionSuccess = true; // Still indicate success for the legacy UI injection popup
			}
			catch (Exception ex)
			{
				DevGuiRenderer._injectionSuccess = false; // Indicate failure for the legacy UI injection popup
				DevGuiRenderer.DevLog.Write($"[SAVE DATA ERROR]: {ex.Message}", "ERROR"); // Consistent error logging
				DevGuiRenderer._injectionMsg = $"[SAVE ERROR]: {ex.Message}"; // For legacy UI popup
			}
		}

		/// <summary>
		/// Ensures directories exist in the Documents folder.
		/// </summary>
		public static string RefreshSaveSystem()
		{
			DevGuiRenderer._showInjectionPopup = true;
			GameState.EnsureDirectories();
			return "Save system path verified in Documents/DungeonAdventures";
		}

		/// <summary>
		/// Mirrors local AppData to the Master Documents folder.
		/// FIXED: Now correctly placed inside the SaveGame class.
		/// </summary>
		public static string ForceSyncAllData()
		{
			DevGuiRenderer._showInjectionPopup = true;
			Thread.Sleep(200);

			StringBuilder report = new StringBuilder();
			int totalFilesSynced = 0;

			try
			{
				string oldMasterSavesRoot = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "DungeonAdventures", "Saves");
				string newMasterDataPath = GameState.MasterPath; // This is now Documents/DungeonAdventures/Data

				string[] folders = GameState.DataFolders; // Use GameState.DataFolders for consistency

				report.AppendLine("[#00FFFFFF]» INITIATING FORCE SYNC [MIGRATION PROTOCOL][/COLOR]...");
				report.AppendLine($"[#FFFF00FF]» FROM OLD: {oldMasterSavesRoot}[/COLOR]");
				report.AppendLine($"[#00FF00FF]» TO NEW:   {newMasterDataPath}[/COLOR]");

				if (Directory.Exists(oldMasterSavesRoot))
				{
					foreach (var f in folders)
					{
						string sourceDir = Path.Combine(oldMasterSavesRoot, f);
						string targetDir = Path.Combine(newMasterDataPath, f);

						if (!Directory.Exists(sourceDir)) continue;

						Directory.CreateDirectory(targetDir);

						foreach (var file in Directory.GetFiles(sourceDir))
						{
							string fileName = Path.GetFileName(file);
							string destPath = Path.Combine(targetDir, fileName);

							File.Copy(file, destPath, true);

							totalFilesSynced++;
						}
						report.AppendLine($"[#90EE90FF][+] {f.ToUpper().PadRight(10)} | MIGRATED folder.[/COLOR]");
					}
					report.AppendLine("[#808080FF]» Old 'Saves' folder (if existed) processed.[/COLOR]");
				}
				else
				{
					report.AppendLine("[#FFA500FF]» No old 'Saves' folder found for migration.[/COLOR]");
				}


				report.AppendLine("[#FFFFFF00]------------------------------------[/COLOR]");
				report.AppendLine("[#00FF00FF]» STATUS: DATA MIGRATION ATTEMPT COMPLETE[/COLOR]");
				report.AppendLine($"[#FFFF00FF]» TOTAL OBJECTS MIGRATED: {totalFilesSynced}[/COLOR]");

				DevGuiRenderer._injectionSuccess = true;
				DevGuiRenderer._injectionMsg = report.ToString();

				return DevGuiRenderer._injectionMsg;
			}
			catch (Exception ex)
			{
				DevGuiRenderer._injectionSuccess = false;
				DevGuiRenderer._injectionMsg = $"[#FF0000FF][FAIL]: {ex.Message}[/COLOR]";
				return DevGuiRenderer._injectionMsg; // Ensure this catch block returns
			}
		}

		/// <summary>
		/// Exports data to a specific master folder, leveraging GameState.GetGlobalPath for consistency.
		/// Ensures atomicity and handles JSON serialization options for robustness.
		/// </summary>
		public static bool ExportToMaster<T>(T data, string folder, string fileName)
		{
			string targetDir = GameState.GetGlobalPath(folder);
			string filePath = Path.Combine(targetDir, fileName + ".json");
			string tempPath = filePath + ".tmp";

			try
			{
				if (!Directory.Exists(targetDir)) Directory.CreateDirectory(targetDir);

				// THESE OPTIONS STOP THE CRASH
				var options = new JsonSerializerOptions
				{
					WriteIndented = true,
					// 1. This stops the "InvalidOperationException" (Circular Reference)
					ReferenceHandler = ReferenceHandler.IgnoreCycles,
					// 2. This makes Enums save as "Fire" instead of "1"
					Converters = { new JsonStringEnumConverter() },
					// 3. This prevents crashing on null properties
					DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
				};

				string jsonData = JsonSerializer.Serialize(data, options);

				// Atomic Write
				File.WriteAllText(tempPath, jsonData);
				if (File.Exists(filePath)) File.Delete(filePath);
				File.Move(tempPath, filePath);

				DevGuiRenderer._injectionSuccess = true;
				DevGuiRenderer._injectionMsg = $"[SAVED] {folder}/{fileName}";
				return true;
			}
			catch (Exception ex)
			{
				if (File.Exists(tempPath)) File.Delete(tempPath);
				// This will now print the ACTUAL error to your console instead of just crashing
				Console.WriteLine($"[SERIALIZATION FAIL]: {ex.Message}");
				return false;
			}
		}
	}
}