using DungeonAdventures.Src.GameData;
using DungeonAdventures.Src.GameData.Components;
using DungeonAdventures.Src.GameEngine;
using ImGuiNET;
using System.Numerics;
using System.Text.Json;
using System.Text.RegularExpressions;
using static DungeonAdventures.Src.GameData.Entities.PlayerData;

namespace DungeonAdventures.Src.Interfaces
{
	public partial class DevGuiRenderer
	{

		private static readonly Dictionary<string, Vector4> _colorMap = new();
		private string _configPath = Path.Combine(GameState.MasterPath, "settings", "gui_config.json");

		private static void ParseAndRenderColoredText(string text)
		{
			var regex = new Regex(@"\[(.*?)\](.[^\[]*)");
			var matches = regex.Matches(text);

			if (matches.Count == 0)
			{
				ImGui.TextUnformatted(text);
				return;
			}

			int lastIndex = 0;
			foreach (Match match in matches)
			{
				// Render text before the colored segment
				if (match.Index > lastIndex)
				{
					ImGui.TextUnformatted(text.Substring(lastIndex, match.Index - lastIndex));
					ImGui.SameLine(0, 0); // Keep on the same line without spacing
				}

				string colorName = match.Groups[1].Value;
				string segmentText = match.Groups[2].Value;

				if (_colorMap.TryGetValue(colorName, out Vector4 color))
				{
					ImGui.TextColored(color, segmentText);
				}
				else
				{
					// If color not found, render as uncolored text
					ImGui.TextUnformatted(segmentText);
				}
				ImGui.SameLine(0, 0); // Keep on the same line without spacing
				lastIndex = match.Index + match.Length;
			}

			// Render any remaining text after the last colored segment
			if (lastIndex < text.Length)
			{
				ImGui.TextUnformatted(text.Substring(lastIndex));
			}
		}

		private void SaveTheme()
		{
			try
			{
				var config = new ThemeConfig
				{
					FontColor = new float[] { color1.X, color1.Y, color1.Z, color1.W },
					//BgColor = new float[] { TgbColor.X, TgbColor.Y, TgbColor.Z, TgbColor.W },
					BorderColor = new float[] { BorderColor.X, BorderColor.Y, BorderColor.Z, BorderColor.W },
					ChildBgColor = new float[] { ChildBgColor.X, ChildBgColor.Y, ChildBgColor.Z, ChildBgColor.W }, // NEW
					WindowBorderSize = sd1,
					FrameBorderSize = sd2,
					ChildBorderSize = sd4,
					WindowRounding = sd5,
					FrameRounding = sd6,
					ChildRounding = sd7,
					WindowSize = new float[] { sd3.X, sd3.Y }
				};
				File.WriteAllText(_configPath, JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true }));
			}
			catch (Exception ex) { DevLog.Write($"Save Error: {ex.Message}", "ERROR"); }
		}

		//private void LoadTheme()
		//{
		//	if (!File.Exists(_configPath)) return;
		//	try
		//	{
		//		var config = JsonSerializer.Deserialize<ThemeConfig>(File.ReadAllText(_configPath)) ?? new ThemeConfig();
		//		if (config != null)
		//		{
		//			color1 = new Vector4(config.FontColor[0], config.FontColor[1], config.FontColor[2], config.FontColor[3]);
		//			TgbColor = new Vector4(config.BgColor[0], config.BgColor[1], config.BgColor[2], config.BgColor[3]);
		//			BorderColor = new Vector4(config.BorderColor[0], config.BorderColor[1], config.BorderColor[2], config.BorderColor[3]);

		//			if (config.ChildBgColor != null) // Safety check for old config files
		//				ChildBgColor = new Vector4(config.ChildBgColor[0], config.ChildBgColor[1], config.ChildBgColor[2], config.ChildBgColor[3]);

		//			sd1 = config.WindowBorderSize;
		//			sd2 = config.FrameBorderSize;
		//			sd4 = config.ChildBorderSize;
		//			sd5 = config.WindowRounding;
		//			sd6 = config.FrameRounding;
		//			sd7 = config.ChildRounding;
		//			sd3 = new Vector2(config.WindowSize[0], config.WindowSize[1]);
		//		}
		//	}
		//	catch (Exception ex) { DevLog.Write($"Reload Error: {ex.Message}", "ERROR"); }
		//}




























		private void RenderDeletePopup()
		{
			if (_showDeleteConfirm) ImGui.OpenPopup("Confirm Destructive Action");

			if (ImGui.BeginPopupModal("Confirm Destructive Action", ref _showDeleteConfirm, ImGuiWindowFlags.AlwaysAutoResize))
			{
				ImGui.TextColored(new Vector4(1, 0, 0, 1), "WARNING: THIS CANNOT BE UNDONE");
				ImGui.Text($"Permanently delete profile: {Path.GetFileName(_profileToDelete)}?");

				if (ImGui.Button("YES, DELETE IT", new Vector2(150, 30)))
				{
					if (File.Exists(_profileToDelete)) File.Delete(_profileToDelete);
					_showDeleteConfirm = false;
				}
				ImGui.SameLine();
				if (ImGui.Button("NO, CANCEL", new Vector2(150, 30)))
				{
					_showDeleteConfirm = false;
				}
				ImGui.EndPopup();
			}
		}
	



	private void RenderFullJsonEditor()
		{
			if (_showEditPopup) ImGui.OpenPopup("Raw Source JSON Editor");

			if (ImGui.BeginPopupModal("Raw Source JSON Editor", ref _showEditPopup))
			{
				ImGui.Text($"Editing: {Path.GetFileName(_profileToEdit)}");
				ImGui.InputTextMultiline("##editor", ref _jsonEditorBuffer, 100000, new Vector2(800, 500));

				if (ImGui.Button("APPLY & OVERWRITE FILE", new Vector2(300, 35)))
				{
					File.WriteAllText(_profileToEdit, _jsonEditorBuffer);
					_showEditPopup = false;
				}
				ImGui.SameLine();
				if (ImGui.Button("DISCARD CHANGES", new Vector2(300, 35)))
				{
					_showEditPopup = false;
				}
				ImGui.EndPopup();
			}
		}
		private void RenderDataManager()
		{
			// --- FIX: Point to the new nested Saves structure ---
			string path = GameState.GetActiveProfileFolder();

			// Ensure the folder exists so ImGui doesn't crash trying to scan it
			if (!Directory.Exists(path)) Directory.CreateDirectory(path);
			var files = Directory.GetFiles(path, "*.json");

			ImGui.Text("Filter Profiles:"); ImGui.SameLine();
			ImGui.InputText("##profileSearch", ref _profileSearchFilter, 100);
			ImGui.Separator();

			// --- CREATE NEW DATA PROFILE ---
			if (ImGui.Button("+++ CREATE NEW DATA PROFILE +++", new Vector2(-1, 35)))
			{
				_tempNewPlayer = new loadPlayer
				{
					PlayerName = "Unnamed Hero",
					PlayerClass = "Warrior", // Added for theme consistency
					Health = 100,
					HitPoints = 100,
					Coins = 500,
					Level = 1,
					Abilities = new Abilities { Strength = 10, Dexterity = 10, Constitution = 10, Intelligence = 10, Wisdom = 10, Charisma = 10, Luck = 10, Perception = 10 },
					Inventory = new List<ItemData>
			{
				new ItemData { Name = "Standard Issue Blade", Type = "Weapon", Value = 50, Rarity = ItemRarity.Common },
				new ItemData { Name = "Ration Pack", Type = "Consumable", Value = 10, Rarity = ItemRarity.Common }
			}
				};
				_showCreatePopup = true;
			}

			ImGui.Separator();

			for (int i = 0; i < files.Length; i++)
			{
				ImGui.PushID(i); // Prevents the ID Conflict you saw earlier

				ImGui.Text(Path.GetFileNameWithoutExtension(files[i]));
				ImGui.SameLine(ImGui.GetWindowWidth() - 100);

				//if (ImGui.Button("DELETE", new Vector2(80, 0)))
				//{
				//	_fileToDelete = files[i];
				//	_showDeleteConfirm = true;
				//}

				ImGui.PopID();
				ImGui.Separator();
			}

			// Confirmation Popup Logic
			if (_showDeleteConfirm)
			{
				ImGui.OpenPopup("Delete Confirmation");
			}

			if (ImGui.BeginPopupModal("Delete Confirmation", ref _showDeleteConfirm, ImGuiWindowFlags.AlwaysAutoResize))
			{
				ImGui.Text($"Are you sure you want to permanently delete:\n{Path.GetFileName(_fileToDelete)}?");
				ImGui.Separator();

				if (ImGui.Button("YES", new Vector2(120, 0)))
				{
					File.Delete(_fileToDelete);
					DevLog.Write($"Permanently deleted {_fileToDelete}", "WARNING");
					_showDeleteConfirm = false;
					ImGui.CloseCurrentPopup();
				}
				ImGui.SameLine();
				if (ImGui.Button("CANCEL", new Vector2(120, 0)))
				{
					_showDeleteConfirm = false;
					ImGui.CloseCurrentPopup();
				}
				ImGui.EndPopup();
			}
			// --- PROFILE LISTING ---
			if (ImGui.BeginChild("ProfileScrollRegion", new Vector2(0, 300), ImGuiChildFlags.Borders))
			{
				foreach (var file in files)
				{
					string fileName = Path.GetFileName(file);

					// Filter logic
					if (!string.IsNullOrEmpty(_profileSearchFilter) &&
						fileName.IndexOf(_profileSearchFilter, StringComparison.OrdinalIgnoreCase) < 0) continue;

					// UI Feedback for currently loaded player
					if (fileName == _currentLoadedFileName)
						ImGui.TextColored(new Vector4(0, 1, 0, 1), $"[ACTIVE] {fileName}");
					else
						ImGui.Text(fileName);

					ImGui.SameLine(ImGui.GetWindowWidth() - 220);

					// LOAD BUTTON
					if (ImGui.SmallButton($"LOAD##{file}"))
					{
						try
						{
							string json = File.ReadAllText(file);
							var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
							var deserializedPlayer = JsonSerializer.Deserialize<loadPlayer>(json, options);
							if (deserializedPlayer != null)
							{
								GameState.CurrentPlayer = deserializedPlayer;
							}
							else
							{
								throw new InvalidOperationException("Deserialized player data was null.");
							}

							_currentLoadedFileName = fileName;
							// Update the active path so the Load Screen knows where this came from
							GameState.ActiveSavePath = Path.GetDirectoryName(path);

							_injectionMsg = $"[SUCCESS] Injected {fileName} into GameState.";
							_injectionSuccess = true;
							_showInjectionPopup = true;
						}
						catch (Exception ex)
						{
							_injectionMsg = $"Load Failed: {ex.Message}";
							_injectionSuccess = false;
							_showInjectionPopup = true;
						}
					}

					ImGui.SameLine();
					if (ImGui.SmallButton($"EDIT##{file}"))
					{
						_profileToEdit = file;
						_jsonEditorBuffer = File.ReadAllText(file);
						_showEditPopup = true;
					}

					ImGui.SameLine();
					if (ImGui.SmallButton($"DEL##{file}"))
					{
						_profileToDelete = file;
						_showDeleteConfirm = true;
					}
				}
				ImGui.EndChild();
			}
		}

		private void RefreshLibrary(string category)
		{
			try
			{
				string path = GameState.GetGlobalPath(category);
				if (category == "attachments")
					_availableAttachments = LoadGame.LoadAllFromFolder<AttachmentData>(path);
				else if (category == "weapons")
					_availableWeapons = LoadGame.LoadAllFromFolder<WeaponData>(path);

				DevLog.Write($"Refreshed {category} library from disk.", "SUCCESS");
			}
			catch (Exception ex)
			{
				DevLog.Write($"Library Refresh Failed: {ex.Message}", "ERROR");
			}
		}


		private void RenderCreateProfilePopup()
		{
			if (_showCreatePopup)
			{
				Vector2 viewportSize = ImGui.GetMainViewport().Size;
				// Adjusted size to fit all the attribute and inventory options
				ImGui.SetNextWindowSize(new Vector2(700, 600), ImGuiCond.Appearing);
				ImGui.SetNextWindowPos(viewportSize / 2, ImGuiCond.Appearing, new Vector2(0.5f, 0.5f));
				ImGui.OpenPopup("Profile Creation Forge");
			}

			if (ImGui.BeginPopupModal("Profile Creation Forge", ref _showCreatePopup))
			{
				ImGui.TextColored(new Vector4(0, 1, 1, 1), "PLAYER DATA INITIALIZATION");
				ImGui.Separator();

				// 1. File and Identity
				ImGui.Columns(2, "create_identity", false);
				ImGui.Text("File Name (.json):");
				ImGui.InputText("##fname", ref _newProfileName, 64);

				ImGui.NextColumn();
				ImGui.Text("Hero Name:");
				string pName = _tempNewPlayer.PlayerName ?? "";
				if (ImGui.InputText("##pname", ref pName, 64)) _tempNewPlayer.PlayerName = pName;
				ImGui.Columns(1);

				// 2. Class and Vitals
				ImGui.Spacing();
				if (ImGui.Combo("Initial Class", ref _selectedClassIndex, _availableClasses, _availableClasses.Length))
				{
					_tempNewPlayer.PlayerClass = _availableClasses[_selectedClassIndex];
				}

				ImGui.Columns(2, "create_vitals", false);
				int hp = _tempNewPlayer.Health;
				if (ImGui.DragInt("Starting HP", ref hp, 1, 1, 9999)) { _tempNewPlayer.Health = hp; _tempNewPlayer.HitPoints = hp; }

				ImGui.NextColumn();
				int coins = _tempNewPlayer.Coins;
				if (ImGui.DragInt("Starting Gold", ref coins, 10, 0, 100000)) _tempNewPlayer.Coins = coins;
				ImGui.Columns(1);

				// 3. Attribute Matrix (The STR, DEX, etc. you had)
				ImGui.Separator();
				ImGui.TextColored(new Vector4(1, 1, 0, 1), "Core Attributes");
				ImGui.Columns(3, "create_attribs", false);

				int str = _tempNewPlayer.Abilities.Strength; if (ImGui.DragInt("STR", ref str)) _tempNewPlayer.Abilities.Strength = str;
				int dex = _tempNewPlayer.Abilities.Dexterity; if (ImGui.DragInt("DEX", ref dex)) _tempNewPlayer.Abilities.Dexterity = dex;
				int con = _tempNewPlayer.Abilities.Constitution; if (ImGui.DragInt("CON", ref con)) _tempNewPlayer.Abilities.Constitution = con;

				ImGui.NextColumn();
				int intl = _tempNewPlayer.Abilities.Intelligence; if (ImGui.DragInt("INT", ref intl)) _tempNewPlayer.Abilities.Intelligence = intl;
				int wis = _tempNewPlayer.Abilities.Wisdom; if (ImGui.DragInt("WIS", ref wis)) _tempNewPlayer.Abilities.Wisdom = wis;
				int cha = _tempNewPlayer.Abilities.Charisma; if (ImGui.DragInt("CHA", ref cha)) _tempNewPlayer.Abilities.Charisma = cha;

				ImGui.NextColumn();
				int lck = _tempNewPlayer.Abilities.Luck; if (ImGui.DragInt("LUCK", ref lck)) _tempNewPlayer.Abilities.Luck = lck;
				int per = _tempNewPlayer.Abilities.Perception; if (ImGui.DragInt("PER", ref per)) _tempNewPlayer.Abilities.Perception = per;
				ImGui.Columns(1);

				// 4. Starting Inventory Preview
				ImGui.Separator();
				ImGui.Text("Starting Equipment:");
				if (ImGui.BeginChild("StartingInv", new Vector2(0, 100), ImGuiChildFlags.Borders))
				{
					foreach (var item in _tempNewPlayer.Inventory)
					{
						ImGui.Text(item.ToString());
					}
					ImGui.EndChild();
				}

				// 5. Finalize Logic
				ImGui.Spacing();
				ImGui.Separator();
				if (ImGui.Button("FINALIZE & SAVE TO DISK", new Vector2(-1, 40)))
				{
					try
					{
						string dir = GameState.GetActiveProfileFolder();
						if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

						// Standardize the name to include _stats.json so the Main Menu sees it!
						string fullPath = Path.Combine(dir, $"{_newProfileName}_stats.json");
						string json = JsonSerializer.Serialize(_tempNewPlayer, new JsonSerializerOptions { WriteIndented = true });
						File.WriteAllText(fullPath, json);

						DevLog.Write($"Profile '{_newProfileName}' forged successfully.", "SYSTEM");
						_showCreatePopup = false;
					}
					catch (Exception ex)
					{
						DevLog.Write($"Forge Failed: {ex.Message}", "ERROR");
					}
				}

				if (ImGui.Button("ABANDON FORGE (CANCEL)", new Vector2(-1, 30)))
				{
					_showCreatePopup = false;
				}

				ImGui.EndPopup();
			}
		}

		private void RenderInjectionStatus()
		{
			if (!_showInjectionPopup) return;

			Vector2 displaySize = ImGui.GetMainViewport().Size;
			ImGui.SetNextWindowPos(new Vector2(displaySize.X / 2, displaySize.Y / 2), ImGuiCond.Appearing, new Vector2(0.5f, 0.5f));
			ImGui.SetNextWindowSize(new Vector2(600, 450));

			if (ImGui.BeginPopupModal("System Notification", ref _showInjectionPopup, ImGuiWindowFlags.AlwaysAutoResize))
			{
				if (ImGui.IsWindowAppearing())
				{
					DevLog.Write("[!] SYNC in progress. Review the report in the GUI window and press 'Acknowledge'.", "SYSTEM");
				}

				ImGui.TextColored(_injectionSuccess ? new Vector4(0, 1, 0.8f, 1) : new Vector4(1, 0, 0, 1),
					_injectionSuccess ? "--- DATA INJECTION REPORT ---" : "--- INJECTION FAILED ---");
				ImGui.Separator();

				if (ImGui.BeginChild("FolderReport", new Vector2(0, -65), ImGuiChildFlags.Borders))
				{
					// Use the new helper function to parse and render colored text
					ParseAndRenderColoredText(_injectionMsg);
					ImGui.EndChild();
				}

				ImGui.SetCursorPosY(ImGui.GetWindowHeight() - 55);
				if (ImGui.Button("ACKNOWLEDGE", new Vector2(-1, 45)))
				{
					_showInjectionPopup = false;
					// --- THE CLEANUP & NEW SUCCESS MESSAGE ---
					DevLog.Write("========================================", "SYSTEM");
					DevLog.Write("        INJECTION COMPLETE!             ", "SYSTEM");
					DevLog.Write("========================================", "SYSTEM");
					DevGuiRenderer.NeedsMenuRedraw = true; // This will trigger the main menu to redraw
					ImGui.CloseCurrentPopup(); // Explicitly close the popup
				}
				ImGui.EndPopup();
			}
		}



		private void RenderGuiCustomizer()
		{
			ImGui.TextColored(new Vector4(1f, 1f, 0.4f, 1), "INTERFACE STYLING ENGINE");
			ImGui.Separator();

			ImGui.Columns(2, "customizer_cols", false);

			// Left Column: Colors
			ImGui.Text("Color Palettes");
			ImGui.ColorEdit4("Font Color", ref color1);
			ImGui.ColorEdit4("Background", ref TgbColor);
			ImGui.ColorEdit4("Border Color", ref BorderColor);
			ImGui.ColorEdit4("Child Background", ref ChildBgColor);
			ImGui.NextColumn();

			// Right Column: Borders & Sizing
			ImGui.Text("Borders & Rounding");
			ImGui.SliderFloat("Win Border", ref sd1, 0f, 10f);
			ImGui.SliderFloat("Win Round", ref sd5, 0f, 20f);
			ImGui.Separator();
			ImGui.SliderFloat("Frame Border", ref sd2, 0f, 10f);
			ImGui.SliderFloat("Frame Round", ref sd6, 0f, 20f);
			ImGui.Separator();
			ImGui.SliderFloat("Child Border", ref sd4, 1f, 10f);
			ImGui.SliderFloat("Child Round", ref sd7, 1f, 20f);

			ImGui.Columns(1);
			ImGui.Separator();

			ImGui.Text("Global Window Resolution");
			ImGui.SliderFloat2("##winsize", ref sd3, 100, 1920);

			// --- SAVE AND CONTROL BUTTONS ---
			if (ImGui.Button("SAVE THEME TO DISK", new Vector2(ImGui.GetContentRegionAvail().X / 2 - 5, 40)))
			{
				SaveTheme();
				_injectionSuccess = true;
				_injectionMsg = "GUI configuration written to gui_config.json successfully.";
				_showInjectionPopup = true;
			}
			ImGui.SameLine();
			if (ImGui.Button("RELOAD FROM DISK", new Vector2(ImGui.GetContentRegionAvail().X, 40)))
			{
				//LoadTheme();
			}

			if (ImGui.Button("RESET TO DEFAULT THEME", new Vector2(-1, 30)))
			{
				color1 = new Vector4(1, 1, 1, 1);
				TgbColor = new Vector4(0.1f, 0.1f, 0.1f, 1);
				BorderColor = new Vector4(0, 1, 1, 1);
				sd1 = 1.0f; sd2 = 1.0f; sd4 = 1.0f; // Borders
				sd5 = 5.0f; sd6 = 4.0f; sd7 = 4.0f; // Rounding
				sd3 = new Vector2(900, 700);
			}

			ImGui.Separator();
			ImGui.TextColored(new Vector4(0.5f, 0.8f, 0.5f, 1), "Developer Information:");
			ImGui.Text($"Game Version: {GameVersion}");
			ImGui.Text($"Last Save Sync: {_lastSavedTime}");
		}
	}
}
