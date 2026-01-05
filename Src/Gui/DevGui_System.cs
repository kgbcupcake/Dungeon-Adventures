using ClickableTransparentOverlay;
using DungeonAdventures.Src.GameData;
using DungeonAdventures.Src.GameData.Components;
using DungeonAdventures.Src.GameData.Entities;
using DungeonAdventures.Src.GameEngine;
using ImGuiNET;
using System.Collections;
using System.Numerics;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DungeonAdventures.Src.Interfaces
{
	// --- SYSTEM & REFLECTION ENGINE ---
	public partial class DevGuiRenderer : Overlay
	{
		private int _newPlayerClassIndex = 0;

		// --- THE REFLECTION ENGINE (PROPERTY EDITOR) ---
		public void RenderPropertyEditor(object obj)
		{
			if (obj == null)
			{
				ImGui.TextColored(new Vector4(1, 0, 0, 1), "[!] NULL REFERENCE: Object target is missing.");
				return;
			}

			var properties = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

			foreach (var prop in properties)
			{
				if (prop.GetIndexParameters().Length > 0 || !prop.CanRead) continue;

				string propName = prop.Name;
				object value = null;

				try { value = prop.GetValue(obj); }
				catch { ImGui.Text($"{propName}: [READ ERROR]"); continue; }

				// 1. PUSH ID
				ImGui.PushID($"{propName}_{obj.GetHashCode()}");

				// 2. TYPE CHECKS (FLATTENED)
				if (prop.PropertyType == typeof(string))
				{
					string s = (string)value ?? "";
					if (ImGui.InputText(propName, ref s, 256) && prop.CanWrite)
						prop.SetValue(obj, s);
				}
				else if (prop.PropertyType == typeof(int))
				{
					int i = (int)(value ?? 0);
					if (ImGui.DragInt(propName, ref i) && prop.CanWrite)
						prop.SetValue(obj, i);
				}
				else if (prop.PropertyType == typeof(float))
				{
					float f = (float)(value ?? 0f);
					if (ImGui.DragFloat(propName, ref f) && prop.CanWrite)
						prop.SetValue(obj, f);
				}
				else if (prop.PropertyType.IsEnum)
				{
					string[] names = Enum.GetNames(prop.PropertyType);
					int index = Array.IndexOf(names, value?.ToString() ?? "");
					if (ImGui.Combo(propName, ref index, names, names.Length) && prop.CanWrite)
					{
						if (Enum.TryParse(prop.PropertyType, names[index], out object enumValue))
							prop.SetValue(obj, enumValue);
					}
				}
				else if (value is IList list) // Now reachable!
				{
					if (ImGui.TreeNode($"{propName} [Count: {list.Count}]"))
					{
						for (int i = 0; i < list.Count; i++)
						{
							ImGui.PushID($"Idx_{i}");
							if (ImGui.TreeNode($"Index [{i}]"))
							{
								RenderPropertyEditor(list[i]);
								ImGui.TreePop();
							}
							ImGui.PopID();
						}
						ImGui.TreePop();
					}
				}
				else if (prop.PropertyType.IsClass && value != null)
				{
					if (ImGui.TreeNode(propName))
					{
						RenderPropertyEditor(value);
						ImGui.TreePop();
					}
				}
				else
				{
					ImGui.Text($"{propName}: {value?.ToString() ?? "null"}");
				}

				// 3. GUARANTEED POP ID
				ImGui.PopID();
			}
		}

		//_currentLoadedFileName
		private void RenderFullStatEditor()
		{
			// --- STATE 1: CREATION MODE (If no player exists in memory) ---
			if (GameState.CurrentPlayer == null)
			{
				ImGui.TextColored(new Vector4(1, 1, 0, 1), " [ PROFILE NOT DETECTED ]");
				ImGui.Separator();
				ImGui.Text("Enter details to eject a new biometric profile to disk:");

				ImGui.InputText("Hero Name", ref _pNameBuffer, 64);
				ImGui.Combo("Archetype", ref _selectedClassIndex, _availableClasses, _availableClasses.Length);

				ImGui.Spacing();
				if (ImGui.Button("GENERATE & EJECT NEW PROFILE", new Vector2(-1, 50)))
				{

					_currentTab = 0;
					GameState.CurrentPlayer = null; // Clearing this triggers the "Creation Wizard" we built
					var newHero = new PlayerData.loadPlayer()
					{
						PlayerName = _pNameBuffer,
						PlayerClass = _availableClasses[_selectedClassIndex],
						Level = 1,
						HitPoints = 100,
						Health = 100,
						Coins = 50,
						Abilities = new PlayerData.Abilities() // MUST initialize sub-objects!
						{
							Strength = 0,
							Dexterity = 0,
							Constitution = 0,
							Intelligence = 0,
							Wisdom = 0,
							Charisma = 0,
							Luck = 0,
							Perception = 0
						}
					};

					// 2. Set as the active player and eject to folder
					GameState.CurrentPlayer = newHero;
					_conductor.ExportData(newHero, "players", _pNameBuffer.Replace(" ", "_"));

					// 3. Refresh GameState so the editor below sees it
					GameState.Sync();

					_injectionMsg = $"PROFILE CREATED: {newHero.PlayerName} has been manifested.";
					_injectionSuccess = true;
					_showInjectionPopup = true;
				}
				return; // Stop here; wait for player to exist before showing stats
			}

			// --- STATE 2: EDITOR MODE (The active profile) ---
			var p = GameState.CurrentPlayer;

			// Header Banner
			if (ImGui.BeginChild("HeroBanner", new Vector2(0, 60), ImGuiChildFlags.Borders))
			{
				ImGui.Columns(3, "hero_cols", false);
				ImGui.SetWindowFontScale(1.2f);
				ImGui.TextColored(new Vector4(0, 1, 1, 1), $"SOURCE: {_currentLoadedFileName}");
				ImGui.NextColumn();
				ImGui.TextColored(new Vector4(1, 1, 0, 1), $"HERO: {p.PlayerName}");
				ImGui.NextColumn();
				ImGui.TextColored(new Vector4(1, 0.5f, 0, 1), $"CLASS: {p.PlayerClass}");
				ImGui.Columns(1);
				ImGui.SetWindowFontScale(1.0f);
				ImGui.EndChild();
			}

			ImGui.Separator();

			if (_godMode) p.Health = p.HitPoints;

			// Vital Statistics Section
			if (ImGui.CollapsingHeader("Vital Statistics", ImGuiTreeNodeFlags.DefaultOpen))
			{
				ImGui.Columns(2, "vitals_cols", false);
				int hp = p.Health; if (ImGui.DragInt("Current HP", ref hp, 1, 0, 99999)) p.Health = hp;
				int mhp = p.HitPoints; if (ImGui.DragInt("Max HP", ref mhp, 1, 0, 99999)) p.HitPoints = mhp;
				int gold = p.Coins; if (ImGui.InputInt("Coin Purse", ref gold, 100, 1000)) p.Coins = gold;

				ImGui.NextColumn();
				int lvl = p.Level; if (ImGui.InputInt("Character Level", ref lvl)) p.Level = lvl;
				int xp = p.Experience; if (ImGui.DragInt("Total EXP", ref xp, 10)) p.Experience = xp;
				ImGui.Checkbox("GOD MODE (Auto-Heal)", ref _godMode);
				ImGui.Columns(1);
			}

			// Attribute Matrix Section
			if (ImGui.CollapsingHeader("Attribute Matrix", ImGuiTreeNodeFlags.DefaultOpen))
			{
				ImGui.TextColored(new Vector4(0.6f, 0.6f, 1.0f, 1), "Core DNA Scaling");
				if (ImGui.Combo("Change Archetype", ref _selectedClassIndex, _availableClasses, _availableClasses.Length))
				{
					p.PlayerClass = _availableClasses[_selectedClassIndex];
				}

				ImGui.Separator();
				ImGui.Columns(3, "attrib_matrix", false);

				int str = p.Abilities.Strength; if (ImGui.DragInt("STR", ref str)) p.Abilities.Strength = str;
				int dex = p.Abilities.Dexterity; if (ImGui.DragInt("DEX", ref dex)) p.Abilities.Dexterity = dex;
				int con = p.Abilities.Constitution; if (ImGui.DragInt("CON", ref con)) p.Abilities.Constitution = con;

				ImGui.NextColumn();
				int intl = p.Abilities.Intelligence; if (ImGui.DragInt("INT", ref intl)) p.Abilities.Intelligence = intl;
				int wis = p.Abilities.Wisdom; if (ImGui.DragInt("WIS", ref wis)) p.Abilities.Wisdom = wis;
				int cha = p.Abilities.Charisma; if (ImGui.DragInt("CHA", ref cha)) p.Abilities.Charisma = cha;

				ImGui.NextColumn();
				int lck = p.Abilities.Luck; if (ImGui.DragInt("LUCK", ref lck)) p.Abilities.Luck = lck;
				int per = p.Abilities.Perception; if (ImGui.DragInt("PER", ref per)) p.Abilities.Perception = per;

				ImGui.Columns(1);
				ImGui.Spacing();

				if (ImGui.Button("LEVEL UP ALL (MAX)", new Vector2(180, 30)))
				{
					p.Abilities.Strength = 100; p.Abilities.Dexterity = 100; p.Abilities.Constitution = 100;
					p.Abilities.Intelligence = 100; p.Abilities.Wisdom = 100; p.Abilities.Charisma = 100;
					p.Abilities.Luck = 100; p.Abilities.Perception = 100;
				}
				ImGui.SameLine();
				if (ImGui.Button("RESET TO BASE", new Vector2(180, 30)))
				{
					p.Abilities.Strength = 10; p.Abilities.Dexterity = 10; p.Abilities.Constitution = 10;
					p.Abilities.Intelligence = 10; p.Abilities.Wisdom = 10; p.Abilities.Charisma = 10;
					p.Abilities.Luck = 10; p.Abilities.Perception = 10;
				}

				ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.1f, 0.4f, 0.1f, 1.0f));
				if (ImGui.Button("SYNCHRONIZE BIOMETRICS TO DISK", new Vector2(-1, 40)))
				{
					try
					{
						GameState.Sync();
						_lastSavedTime = DateTime.Now.ToString("HH:mm:ss");
						_injectionSuccess = true;
						_injectionMsg = $"[SUCCESS] {p.PlayerName} secured at {_lastSavedTime}";
						_showInjectionPopup = true;
					}
					catch (Exception ex)
					{
						_injectionMsg = $"SYNC ERROR: {ex.Message}";
						_injectionSuccess = false;
						_showInjectionPopup = true;
					}
				}
				ImGui.PopStyleColor();
			}
		}






























		public void Toggle()
		{
			this.IsVisible = !this.IsVisible;
		}

		private void RenderDevLog()
		{
			// --- ROW 1: Controls ---
			ImGui.Columns(3, "log_btns_flat", false);
			if (ImGui.Button("CLEAR ALL : ###clear", new Vector2(-1, 25)))
			{
				lock (DevLog._lock) { DevLog.Buffer.Clear(); }
			}
			ImGui.NextColumn();
			if (ImGui.Button(_logPaused ? "RESUME : ###res" : "PAUSE : ###pau", new Vector2(-1, 25)))
			{
				_logPaused = !_logPaused;
			}
			ImGui.NextColumn();
			if (ImGui.Button("EXPORT : ###exp", new Vector2(-1, 25)))
			{
				/* Export Logic */
			}
			ImGui.Columns(1);

			ImGui.Spacing();
			ImGui.InputTextWithHint("###log_filter", "SEARCH_TAGS :", ref _logFilter, 100);
			ImGui.Separator();

			// --- ROW 3: The Terminal Buffer ---
			// If BeginChild is called, EndChild MUST follow it before the Main Window's End()
			if (ImGui.BeginChild("TerminalBuffer", new Vector2(0, 350), ImGuiChildFlags.Borders, ImGuiWindowFlags.HorizontalScrollbar))
			{
				string[] snapshot;
				lock (DevLog._lock) { snapshot = DevLog.Buffer.ToArray(); }

				foreach (var line in snapshot)
				{
					if (!string.IsNullOrEmpty(_logFilter) && line.IndexOf(_logFilter, StringComparison.OrdinalIgnoreCase) < 0)
						continue;

					// Push a unique hash ID for every line to prevent the 'id != 0' error
					ImGui.PushID(line.GetHashCode());

					if (line.Contains("[ERROR]")) ImGui.TextColored(new Vector4(1.0f, 0.3f, 0.3f, 1), line);
					else if (line.Contains("[SUCCESS]")) ImGui.TextColored(new Vector4(0.3f, 1.0f, 0.3f, 1), line);
					else ImGui.TextUnformatted(line);

					ImGui.PopID();
				}

				if (!_logPaused) ImGui.SetScrollHereY(1.0f);

				ImGui.EndChild(); // This closes the 'True' path
			}
			else
			{
				// If BeginChild returned false, ImGui STILL expects an EndChild in many versions of 1.9x
				ImGui.EndChild();
			}

			ImGui.TextDisabled($"STATUS_ENTRIES : {DevLog.Buffer.Count}");
		}

		private void HandleMasterHotkeys(ref bool open)
		{
			// F12 Master Toggle
			if ((GetAsyncKeyState(0x7B) & 0x8000) != 0 && (DateTime.Now - _lastToggleTime).TotalMilliseconds > 400)
			{
				GameState.IsDevMode = !GameState.IsDevMode;
				_lastToggleTime = DateTime.Now;
				if (!GameState.IsDevMode) this.IsVisible = false;
				NeedsMenuRedraw = true;
			}

			// F8 Visibility Toggle
			if (GameState.IsDevMode && (GetAsyncKeyState(0x77) & 0x8000) != 0 && (DateTime.Now - _lastF8ToggleTime).TotalMilliseconds > 400)
			{
				this.IsVisible = !this.IsVisible;
				_lastF8ToggleTime = DateTime.Now;
			}
		}

		private string _archiveSearch = ""; // Add this to your class variables at the top

		private void RenderArchiveBrowser()
		{
			if (ImGui.BeginChild("ArchiveBrowserInternal", new Vector2(0, 0), ImGuiChildFlags.None))
			{
				// --- HEADER & SEARCH ---
				ImGui.TextColored(new Vector4(0, 1, 1, 1), " [ NEURAL ARCHIVE ]");
				ImGui.SameLine(ImGui.GetContentRegionAvail().X - 150);
				ImGui.SetNextItemWidth(150);
				ImGui.InputTextWithHint("##search", "Filter...", ref _archiveSearch, 64);
				ImGui.Separator();

				string playerPath = Path.Combine(GameState.MasterPath, "players");

				if (!Directory.Exists(playerPath))
				{
					ImGui.TextColored(new Vector4(1, 0.4f, 0.4f, 1), ">> PATH OFFLINE");
				}
				else
				{
					string[] files = Directory.GetFiles(playerPath, "*.json");

					foreach (var file in files)
					{
						string shortName = Path.GetFileNameWithoutExtension(file);

						// Search Filter
						if (!string.IsNullOrEmpty(_archiveSearch) &&
							!shortName.Contains(_archiveSearch, StringComparison.OrdinalIgnoreCase))
							continue;

						bool isActive = GameState.CurrentPlayer?.PlayerName == shortName;
						ImGui.PushID(file);

						// --- SPICY ROW ---
						if (isActive)
						{
							ImGui.PushStyleColor(ImGuiCol.Header, new Vector4(0, 0.3f, 0.3f, 0.4f));
							ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0, 1, 1, 1));
						}

						if (ImGui.Selectable($"##{shortName}", isActive, ImGuiSelectableFlags.AllowDoubleClick, new Vector2(0, 25)))
						{
							LoadPlayerFromFile(file);
						}

						ImGui.SameLine(10);
						ImGui.Text(isActive ? $">> {shortName.ToUpper()}" : $"   {shortName}");

						if (isActive) ImGui.PopStyleColor(2);

						// Manual Red Text Button
						ImGui.SameLine(ImGui.GetWindowWidth() - 40);
						ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(1, 0, 0, 0.6f));
						ImGui.Text("[X]");
						ImGui.PopStyleColor();

						if (ImGui.IsItemClicked()) ImGui.OpenPopup("VaporizeConfirm");

						// --- MODALS & POPUPS ---
						if (ImGui.BeginPopupModal("VaporizeConfirm", ref _isDeleteOpen, ImGuiWindowFlags.AlwaysAutoResize))
						{
							ImGui.TextColored(new Vector4(1, 0, 0, 1), "CONFIRM DATA PURGE?");
							ImGui.Text($"Target: {shortName}");
							if (ImGui.Button("PURGE", new Vector2(100, 0))) { _conductor.DeletePlayerProfile(file); ImGui.CloseCurrentPopup(); }
							ImGui.SameLine();
							if (ImGui.Button("CANCEL", new Vector2(100, 0))) { ImGui.CloseCurrentPopup(); }
							ImGui.EndPopup();
						}

						ImGui.PopID();
					}
				}
			}
			//ImGui.EndChild(); // GUARANTEED closure for OpsSidebar safety
		}
		private void LoadPlayerFromFile(string path)
		{
			DevLog.Write($"Profile clicked: {path}", "DEBUG");
			_injectionMsg = $"DEBUG: Profile clicked, path: {path}";
			_injectionSuccess = true;
			_showInjectionPopup = true;

			// // Original logic commented out for testing
			// var loadedPlayer = _conductor.LoadPlayerProfile(path);

			// if (loadedPlayer != null)
			// {
			// 	GameState.CurrentPlayer = loadedPlayer;
			// 	_injectionMsg = $"LOADED: {loadedPlayer.PlayerName} active.";
			// 	_injectionSuccess = true;
			// }
			// else
			// {
			// 	_injectionMsg = $"LOAD ERROR: Failed to load player from {path}.";
			// 	_injectionSuccess = false;
			// }
			// _showInjectionPopup = true;
		}




	}
}