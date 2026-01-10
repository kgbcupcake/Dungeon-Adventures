using DungeonAdventures.Src.GameData;
using DungeonAdventures.Src.GameEngine;
using ImGuiNET;
using System.Numerics;
using System;
using System.Collections.Generic;
using System.Linq;
using DungeonAdventures.Src.GameData.Components;

namespace DungeonAdventures.Src.Interfaces
{
	public partial class DevGuiRenderer
	{
		public void RenderAdventureCreator()
		{
			// --- TOP BANNER ---
			ImGui.PushStyleColor(ImGuiCol.ChildBg, new Vector4(0.12f, 0.12f, 0.15f, 1.0f));
			if (ImGui.BeginChild("HeaderBanner", new Vector2(-1, 50), ImGuiChildFlags.Borders))
			{
				ImGui.SetCursorPosY(15);
				ImGui.TextColored(new Vector4(0.4f, 0.8f, 1f, 1), "   [ WORLD ARCHITECT MODE ]");

				ImGui.SameLine(ImGui.GetContentRegionAvail().X - 110);
				if (string.IsNullOrWhiteSpace(_aMapName) || _aMapName == "New Adventure")
					ImGui.TextColored(new Vector4(1, 0.4f, 0, 1), "DRAFTING");
				else
					ImGui.TextColored(new Vector4(0, 1, 0.5f, 1), "VALID");

				ImGui.EndChild();
			}
			ImGui.PopStyleColor();

			ImGui.Spacing();
			ImGui.Columns(2, "ArchitectLayout", true);
			ImGui.SetColumnWidth(0, ImGui.GetWindowWidth() * 0.62f);

			if (ImGui.CollapsingHeader("1. IDENTITY", ImGuiTreeNodeFlags.DefaultOpen))
			{
				ImGui.Indent();
				ImGui.Text("Adventure Name");
				ImGui.InputText("##advname", ref _aMapName, 100);
				ImGui.Text("Unity Scene Path");
				ImGui.InputText("##scenepath", ref _aScenePath, 256);
				ImGui.Unindent();
			}

			if (ImGui.CollapsingHeader("2. REWARDS", ImGuiTreeNodeFlags.DefaultOpen))
			{
				ImGui.Indent();
				ImGui.Columns(2, "reward_cols", false);
				ImGui.Text("Difficulty");
				ImGui.Combo("##diff", ref _aDifficulty, _difficulties, _difficulties.Length);
				ImGui.Text("Min Level");
				ImGui.DragInt("##minlvl", ref _aRecommendedLevel, 1, 1, 99);
				ImGui.ProgressBar(_aRecommendedLevel / 99f, new Vector2(-1, 5), "");
				ImGui.NextColumn();
				ImGui.TextColored(new Vector4(1, 0.84f, 0, 1), "Gold");
				ImGui.DragInt("##gold", ref _iValue, 100, 0, 1000000);
				ImGui.Columns(1);
				ImGui.Unindent();
			}

			if (ImGui.CollapsingHeader("3. TAGS"))
			{
				ImGui.Indent();
				ImGui.InputText("New Tag", ref _newAdventureTag, 50);
				ImGui.SameLine();
				if (ImGui.Button("ADD"))
				{
					if (!string.IsNullOrWhiteSpace(_newAdventureTag) && !_aTags.Contains(_newAdventureTag))
						_aTags.Add(_newAdventureTag);
					_newAdventureTag = "";
				}
				ImGui.BeginChild("TagBox", new Vector2(-1, 60), ImGuiChildFlags.Borders);
				for (int i = 0; i < _aTags.Count; i++)
				{
					if (ImGui.Button(_aTags[i] + " x##" + i)) { _aTags.RemoveAt(i); break; }
					ImGui.SameLine();
				}
				ImGui.EndChild();
				ImGui.Unindent();
			}

			ImGui.Spacing();
			ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.1f, 0.5f, 0.2f, 1f));
			if (ImGui.Button("FABRICATE TO DISK", new Vector2(-1, 45)))
			{
				ExecuteAdventureFabrication();
			}
			ImGui.PopStyleColor();

			ImGui.NextColumn();
			ImGui.TextColored(new Vector4(1, 1, 0, 1), "LIVE PREVIEW");
			ImGui.Separator();
			if (ImGui.BeginChild("JsonPreview", new Vector2(-1, -1), ImGuiChildFlags.Borders))
			{
				ImGui.Text("{");
				ImGui.Indent();
				PrintJsonLine("Title", _aMapName, new Vector4(0.4f, 0.8f, 1f, 1f));
				PrintJsonLine("Scene", _aScenePath, new Vector4(0.4f, 0.8f, 1f, 1f));

				string tagJoin = string.Join(", ", _aTags.Select(t => "\"" + t + "\""));
				ImGui.TextColored(new Vector4(0.7f, 0.7f, 0.9f, 1f), "\"Tags\": ");
				ImGui.SameLine();
				ImGui.TextColored(new Vector4(1, 1, 1, 1), "[" + tagJoin + "]");

				ImGui.Unindent();
				ImGui.Text("}");
				ImGui.EndChild();
			}
			ImGui.Columns(1);
			if (ImGui.Button("Inject Adventure into Master", new Vector2(-1, 30)))
			{
				try
				{
					var adventure = new AdventureData
					{
						MapName = _aMapName,
						ScenePath = _aScenePath,
						Difficulty = _difficulties[_aDifficulty],
						RecommendedLevel = _aRecommendedLevel
					};

					_conductor.ExportData(adventure, "adventures", _aMapName.Replace(" ", "_"));

					GameState.Sync();
					_injectionSuccess = true;
					_injectionMsg = $"[SUCCESS] Adventure '{_aMapName}' Injected.";
					_showInjectionPopup = true;
				}
				catch (Exception ex) { DevLog.Write($"Injection Error: {ex.Message}", "CRITICAL"); }
			}
		}




		

		private void ExecuteAdventureFabrication()
		{
			if (string.IsNullOrWhiteSpace(_aMapName) || string.IsNullOrWhiteSpace(_aScenePath))
			{
				_injectionMsg = "VALIDATION ERROR";
				_showInjectionPopup = true;
				return;
			}

			try
			{
				var exportData = new AdventureData
				{
					GUID = Guid.NewGuid().ToString(),
					Title = _aMapName,
					MapName = _aMapName,
					ScenePath = _aScenePath,
					DescriptionD = _testDialogue,
					MinimumLevel = _aRecommendedLevel,
					CompletionGold = _iValue,
					Tags = new List<string>(_aTags),
					Stages = new List<QuestStage> {
						new QuestStage { StoryText = "Arrival...", HexColor = "#62D2FF" }
					}
				};

				_conductor.ExportData(exportData, "adventures", _aMapName.Replace(" ", "_").ToLower());
				GameState.Sync();
				_injectionSuccess = true;
				_injectionMsg = "SUCCESS";
			}
			catch (Exception ex) { _injectionMsg = "ERROR: " + ex.Message; }
			_showInjectionPopup = true;
		}

		private void PrintJsonLine(string key, string val, Vector4 color)
		{
			ImGui.TextColored(new Vector4(0.7f, 0.7f, 0.9f, 1f), "\"" + key + "\": ");
			ImGui.SameLine();
			ImGui.TextColored(color, "\"" + val + "\",");
		}
	}
}