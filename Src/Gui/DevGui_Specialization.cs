using DungeonAdventures.Src.GameData;
using DungeonAdventures.Src.GameData.Components;
using DungeonAdventures.Src.GameEngine;
using ImGuiNET;
using System.Numerics;

namespace DungeonAdventures.Src.Interfaces
{
	public partial class DevGuiRenderer
	{
		// --- GEM ARCHITECT FIELDS ---
		// These variables are declared in DevGui_Vars.cs, aligning usage here.

		// --- BOSS LAB FIELDS ---
		// These variables are declared in DevGui_Vars.cs, aligning usage here.

		private void RenderQuestDesigner()
		{
			ImGui.TextColored(new Vector4(1, 1, 0, 1), " [ MISSION CONTROL: QUEST ARCHITECT ]");
			ImGui.Separator();

			ImGui.InputText("Quest Title", ref _qTitle, 100);
			ImGui.InputTextMultiline("Objective", ref _qObjective, 500, new Vector2(-1, 60));

			ImGui.Columns(2, "rewardCols", false);
			ImGui.DragInt("XP Reward", ref _qXPReward, 10, 0, 10000);
			ImGui.NextColumn();
			ImGui.DragInt("Gold Reward", ref _qGoldReward, 5, 0, 5000);
			ImGui.Columns(1);

			ImGui.Spacing();

			// SPICY: Using the same industrial green for the action button
			ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.2f, 0.4f, 0.2f, 1.0f));
			if (ImGui.Button("PUBLISH QUEST TO DATA VAULT", new Vector2(-1, 35)))
			{
				var quest = new QuestData
				{
					Title = _qTitle,
					Description = _qObjective,
					Rewards = new Reward
					{
						ExperiencePoints = _qXPReward,
						Gold = _qGoldReward
					}
				};

				// Use the centralized utility instead of manual File.WriteAllText
				string safeTitle = _qTitle.Replace(" ", "_");
				SaveGame.ExportToMaster(quest, "quests", safeTitle);

				GameState.Sync();
				_injectionSuccess = true;
				_injectionMsg = $"[SUCCESS] Mission '{_qTitle}' verified and archived.";
				_showInjectionPopup = true;
			}
			ImGui.PopStyleColor();
		}

		private void RenderDungeonBuilder()
		{
			ImGui.TextColored(new Vector4(1, 0.5f, 0, 1), "DUNGEON BUILDER");
			ImGui.Separator();

			ImGui.InputText("Dungeon Name", ref _dName, 100);
			ImGui.SliderInt("Floor Count", ref _dFloors, 1, 100);
			ImGui.Checkbox("Hardcore Mode", ref _dIsHardcore);

			if (ImGui.Button("Inject Dungeon into Master", new Vector2(-1, 30)))
			{
				try
				{
					// Creating a dynamic object since the specific class definition is missing
					var dungeon = new { Name = _dName, Floors = _dFloors, Hardcore = _dIsHardcore };

					_conductor.ExportData(dungeon, "dungeons", _dName.Replace(" ", "_"));

					GameState.Sync();
					_injectionSuccess = true;
					_injectionMsg = $"[SUCCESS] Dungeon '{_dName}' Injected.";
					_showInjectionPopup = true;
				}
				catch (Exception ex) 
				{ 
					DevLog.Write($"Dungeon Injection Failed: {ex.Message}", "ERROR");
					_injectionSuccess = false;
					_injectionMsg = $"[ERROR] Dungeon Injection Failed: {ex.Message}";
					_showInjectionPopup = true;
				}
			}
		}

		private void RenderGemAlchemy()
		{
			_pulseTimer += 0.05f;
			float pulseScale = 1.0f + (MathF.Sin(_pulseTimer) * (_gPower / 600f));

			ImGui.TextColored(new Vector4(0.5f, 0.8f, 1.0f, 1), " [ INDUSTRIAL FORGE: CRYSTAL SYNTHESIS ]");
			ImGui.Separator();

			// Use columns to push the preview window far to the right
			ImGui.Columns(2, "GemLayout", false);
			ImGui.SetColumnWidth(0, 400); // Give the data column plenty of space

			// --- COLUMN 1: EXPANDED CORE DATA ---
			ImGui.TextDisabled("MATERIAL SPECIFICATIONS");

			ImGui.SetNextItemWidth(250);
			ImGui.InputText("Designation", ref _gName, 100);

			ImGui.SetNextItemWidth(250);
			ImGui.Combo("Purity Grade", ref _gRarity, _rarityNames, _rarityNames.Length);

			ImGui.SetNextItemWidth(250);
			ImGui.SliderFloat("Resonance", ref _gPower, 0.0f, 100.0f, "%.1f %%");

			ImGui.Spacing();
			ImGui.TextDisabled("MARKET & USAGE");

			ImGui.SetNextItemWidth(120);
			ImGui.DragInt("Market Value", ref _gValue, 10, 0, 100000, "%d Gold");

			ImGui.SameLine();
			ImGui.SetNextItemWidth(120);
			ImGui.DragFloat("Mass (kg)", ref _gWeight, 0.01f, 0.01f, 5.0f, "%.2f");

			ImGui.SetNextItemWidth(120);
			ImGui.DragInt("Level Req", ref _gLevelReq, 1, 1, 100);

			ImGui.NextColumn(); // MOVE TO FAR RIGHT

			// --- COLUMN 2: THE PREVIEW ---
			ImGui.SetCursorPosX(ImGui.GetCursorPosX() + 50); // Additional nudge to the right
			ImGui.BeginGroup();
			ImGui.TextDisabled("   AURA MONITOR");

			Vector4 pColor = GetGemColor();
			ImGui.PushStyleColor(ImGuiCol.Button, pColor);

			float boxSize = 90f * pulseScale;
			// Padding logic to keep the pulsing box centered in its area
			float pad = (100f - boxSize) / 2f;
			ImGui.SetCursorPos(new Vector2(ImGui.GetCursorPos().X + pad, ImGui.GetCursorPos().Y + pad));

			ImGui.Button("##gemPulse", new Vector2(boxSize, boxSize));
			ImGui.PopStyleColor();

			if (ImGui.IsItemHovered())
				ImGui.SetTooltip($"Current Resonance: {_gPower}%\nStatus: Operational");

			ImGui.EndGroup();

			ImGui.Columns(1); // Reset columns
			ImGui.Separator();

			// --- ATTRIBUTES SECTION ---
			ImGui.TextDisabled("ELEMENTAL EXTRACTION");
			if (ImGui.BeginChild("AttrScroll", new Vector2(0, 110), ImGuiChildFlags.Borders))
			{
				ImGui.Columns(4, "attrCols", false); // 4 columns makes it more compact
				foreach (var effect in _allEffects)
				{
					if (effect == EffectType.None) continue;
					bool isSelected = _gAttributes.Contains(effect);
					if (ImGui.Checkbox(effect.ToString(), ref isSelected))
					{
						if (isSelected) _gAttributes.Add(effect);
						else _gAttributes.Remove(effect);
					}
					ImGui.NextColumn();
				}
				ImGui.EndChild();
			}

			ImGui.Spacing();

			// --- ACTION BAR ---
			if (ImGui.Button("GENERATE RANDOM SEED", new Vector2(180, 35)))
			{
				Random r = new Random();
				_gName = $"Shard_{r.Next(1000, 9999)}";
				_gPower = (float)r.NextDouble() * 100f;
				_gValue = (int)(_gPower * r.Next(5, 15));
				_gWeight = (float)r.NextDouble() * 0.5f;
			}

			ImGui.SameLine();

			ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.2f, 0.5f, 0.2f, 1.0f));
			if (ImGui.Button("FABRICATE & EXPORT", new Vector2(200, 35)))
			{
				PerformGemInjection(_rarityNames);
			}
			ImGui.PopStyleColor();
		}

		// Clean helper for the preview color
		private Vector4 GetGemColor()
		{
			if (_gAttributes.Contains(EffectType.Fire)) return new Vector4(1, 0.2f, 0.2f, 1);
			if (_gAttributes.Contains(EffectType.Ice)) return new Vector4(0.2f, 0.5f, 1, 1);
			if (_gAttributes.Contains(EffectType.Poison)) return new Vector4(0.2f, 1, 0.2f, 1);
			if (_gAttributes.Contains(EffectType.Electric)) return new Vector4(1, 1, 0.2f, 1);
			return new Vector4(0.4f, 0.4f, 0.4f, 1);
		}

		private void PerformGemInjection(string[] rarityNames)
		{
			if (string.IsNullOrWhiteSpace(_gName)) return;

			try
			{
				if (Enum.TryParse<ItemRarity>(rarityNames[_gRarity], true, out var parsedRarity))
				{
					// Now populating the new Core Data fields
					var gemData = new GemData
					{
						Name = _gName,
						Rarity = parsedRarity,
						Power = _gPower,
						Attributes = new List<EffectType>(_gAttributes),
						Type = "Gem",
						Value = _gValue,      // Saved
						Weight = _gWeight,    // Saved
						LevelReq = _gLevelReq // Saved
					};

					string safeFileName = _gName.Replace(" ", "_");
					SaveGame.ExportToMaster(gemData, "gems", safeFileName);

					GameState.Sync();
					_injectionSuccess = true;
					_injectionMsg = $"SUCCESS: '{_gName}' saved to {GameState.GemPath}";
				}
			}
			catch (Exception ex)
			{
				_injectionMsg = $"FORGE ERROR: {ex.Message}";
				_injectionSuccess = false;
			}
			_showInjectionPopup = true;
		}









	}
}