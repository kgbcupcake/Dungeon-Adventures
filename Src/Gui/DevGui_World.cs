using DungeonAdventures.Src.GameData;
using DungeonAdventures.Src.GameEngine;
using ImGuiNET;
using System.Numerics;
using System;
using System.Collections.Generic;
using System.Linq;
using DungeonAdventures.Src.GameData.Entities;
using DungeonAdventures.Src.GameData.Components;

namespace DungeonAdventures.Src.Interfaces
{
	public partial class DevGuiRenderer
	{
		private List<AdventureData> _availableAdventures = new List<AdventureData>();
		private string _adventureSearchFilter = "";

		// --- WARP TAB ---
		public void RenderWarpTab()
		{
			ImGui.TextColored(new Vector4(0, 1, 1, 1), "Level Transition Controller");
			ImGui.Separator();
			ImGui.InputText("Target Room ID", ref _targetRoomID, 64);
			ImGui.DragFloat3("Warp Coords", ref _warpCoords);

			ImGui.Spacing();
			if (ImGui.Button("LAUNCH WARP CONTROL WINDOW", new Vector2(-1, 40)))
			{
				_showWarpMenu = true;
			}

			ImGui.Separator();
			ImGui.Text("Active Scene Info:");
			ImGui.Text($"Current File: {_currentLoadedFileName}");
		}

		public void RenderWarpMenu()
		{
			ImGui.SetNextWindowSize(new Vector2(500, 400), ImGuiCond.FirstUseEver);
			bool isExpanded = true;

			if (ImGui.Begin("Warp Menu: Room Navigator", ref isExpanded))
			{
				_showWarpMenu = isExpanded;

				ImGui.TextColored(new Vector4(1, 1, 0, 1), "SEARCHABLE WARP LOCATIONS");
				ImGui.Separator();

				if (ImGui.Button("Refresh Warp Locations", new Vector2(-1, 30)))
				{
					_availableAdventures = LoadGame.LoadAllFromFolder<AdventureData>("adventures");
				}

				ImGui.InputText("Search Filter", ref _adventureSearchFilter, 256);
				ImGui.Separator();

				if (_availableAdventures != null && _availableAdventures.Any())
				{
					var filtered = _availableAdventures
						.Where(a => string.IsNullOrEmpty(_adventureSearchFilter) ||
									(a.MapName?.Contains(_adventureSearchFilter, StringComparison.OrdinalIgnoreCase) ?? false))
						.ToList();

					// FIX: ImGui 1.91.x uses ImGuiChildFlags.Borders (PLURAL)
					if (ImGui.BeginChild("WarpList", new Vector2(0, -ImGui.GetFrameHeightWithSpacing()), ImGuiChildFlags.Borders, ImGuiWindowFlags.None))
					{
						foreach (var adv in filtered)
						{
							ImGui.PushID(adv.GUID.ToString());

							if (ImGui.Selectable($"{adv.MapName}##{adv.GUID}")) { /* Selection Logic */ }

							ImGui.SameLine(ImGui.GetWindowWidth() - 90);
							if (ImGui.Button("WARP", new Vector2(80, 0)))
							{
								if (_conductor != null) _conductor.SwitchMap(adv.ScenePath ?? "");
							}

							ImGui.PopID();
						}
						ImGui.EndChild();
					}
				}
			}
			ImGui.End(); // This End() belongs to ImGui.Begin("Warp Menu: Room Navigator"...)
		}

		// --- BOSS LAB ---
		private void RenderBossLab()
		{
			// Get the enum names once per frame for the combo box
			string[] auraNames = Enum.GetNames(typeof(GlobalAura));

			ImGui.PushStyleColor(ImGuiCol.ChildBg, new Vector4(0.12f, 0.05f, 0.05f, 1.0f));
			ImGui.BeginChild("BossHeader", new Vector2(-1, 50), ImGuiChildFlags.Borders);
			ImGui.TextColored(new Vector4(1, 0.2f, 0.2f, 1), " [ FORGE: ENTITY ARCHITECT ]");
			ImGui.SameLine();
			ImGui.TextDisabled("| Direct Disk Injection Mode");
			ImGui.EndChild();
			ImGui.PopStyleColor();

			ImGui.Columns(2, "BossSplit", true);

			if (ImGui.CollapsingHeader("CORE DATA", ImGuiTreeNodeFlags.DefaultOpen))
			{
				ImGui.Text("Boss Name");
				ImGui.InputText("##bname", ref _bossName, 100);

				ImGui.Text("Health");
				ImGui.DragFloat("##bhp", ref _bossHP, 100, 100, 1000000, "%.0f");

				ImGui.Text("Damage");
				ImGui.DragFloat("##batk", ref _bossDamage, 5, 1, 50000, "%.0f");

				ImGui.Text("Speed");
				ImGui.SliderFloat("##bspd", ref _bossSpeed, 0.1f, 20f, "%.1f");

				ImGui.Text("Level");
				ImGui.DragInt("##blvl", ref _bossLevel, 1, 1, 999);
			}

			if (ImGui.CollapsingHeader("ENVIRONMENTAL AURA", ImGuiTreeNodeFlags.DefaultOpen))
			{
				// Using the persistent variable from DevGui_Vars.cs
				ImGui.Combo("Active Aura", ref _bossAuraIndex, auraNames, auraNames.Length);
			}

			ImGui.NextColumn();

			if (ImGui.CollapsingHeader("TRAIT INJECTION", ImGuiTreeNodeFlags.DefaultOpen))
			{
				ImGui.BeginChild("Traits", new Vector2(-1, 200), ImGuiChildFlags.Borders);
				foreach (TraitType trait in Enum.GetValues(typeof(TraitType)))
				{
					if (trait == TraitType.None) continue;
					bool hasTrait = _bossTraits.Contains(trait);
					if (ImGui.Checkbox(trait.ToString(), ref hasTrait))
					{
						if (hasTrait) _bossTraits.Add(trait);
						else _bossTraits.Remove(trait);
					}
				}
				ImGui.EndChild();
			}

			ImGui.Spacing();

			// MATERIALIZE LOGIC
			ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.4f, 0.1f, 0.1f, 1));
			if (ImGui.Button("MATERIALIZE TO DISK", new Vector2(-1, 40)))
			{
				try
				{
					BossData newBoss = new BossData
					{
						Name = _bossName,
						Health = (int)_bossHP,
						Damage = (int)_bossDamage,
						Speed = _bossSpeed,
						Level = _bossLevel,
						// Cast the persistent index back to the GlobalAura enum
						Aura = (GlobalAura)_bossAuraIndex,
						Description = "Custom forged entity.",
						Attributes = new List<TraitType>(_bossTraits)
					};

					string fileName = _bossName.Replace(" ", "_").ToLower();

					// This saves it to /Data/bosses/your_name.json
					SaveGame.ExportToMaster(newBoss, "bosses", fileName);

					_injectionMsg = $"SUCCESS: {fileName}.json saved to /bosses/";
					_injectionSuccess = true;
					_showInjectionPopup = true;
				}
				catch (Exception ex)
				{
					_injectionMsg = $"ERROR: {ex.Message}";
					_injectionSuccess = false;
					_showInjectionPopup = true;
					MaterializeBoss();
				}
			}
			ImGui.PopStyleColor();

			ImGui.Columns(1);
		}

		private void MaterializeBoss()
		{
			var newBoss = new BossData
			{
				Name = _bossName,
				Health = (int)_bossHP,
				Damage = (int)_bossDamage,
				Speed = _bossSpeed,
				Level = _bossLevel,
				Aura = (GlobalAura)_bossAuraIndex,
				Attributes = new List<TraitType>(_bossTraits)
			};

			// Use the central saving utility
			string fileName = $"{_bossName.Replace(" ", "_")}.json";
			SaveGame.ExportToMaster(newBoss, "bosses", fileName);

			// Refresh the engine state
			GameState.Sync();
			DevLog.Write($"BOSS MATERIALIZED: {fileName} at {GameState.BossPath}", "LAB");
		}



	}
}