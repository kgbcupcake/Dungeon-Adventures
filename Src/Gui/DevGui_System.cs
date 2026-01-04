using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace DungeonAdventures.Src.Interfaces
{
	public partial class DevGuiRenderer
	{

		private void RenderDevLog()
		{
			ImGui.Text("Developer Log");
			ImGui.Separator();
			
			if (ImGui.Button("Clear Log"))
			{
				DevLog.Clear();
			}
			ImGui.SameLine();
			ImGui.Checkbox("Pause Log", ref _logPaused);
			ImGui.SameLine();
			ImGui.InputText("Filter", ref _logFilter, 100);

			ImGui.BeginChild("LogRegion", new Vector2(0, -1), ImGuiChildFlags.None, ImGuiWindowFlags.HorizontalScrollbar);
			
			lock (DevLog._lock)
			{
				foreach (var line in DevLog.Buffer)
				{
					if (!string.IsNullOrEmpty(_logFilter) && !line.Contains(_logFilter, StringComparison.OrdinalIgnoreCase))
						continue;

					Vector4 color = new Vector4(1, 1, 1, 1);
					if (line.Contains("[ERROR]")) color = new Vector4(1, 0, 0, 1);
					else if (line.Contains("[WARN]")) color = new Vector4(1, 1, 0, 1);
					else if (line.Contains("[SUCCESS]")) color = new Vector4(0, 1, 0, 1);

					ImGui.TextColored(color, line);
				}
			}

			if (!_logPaused && ImGui.GetScrollY() >= ImGui.GetScrollMaxY())
			{
				ImGui.SetScrollHereY(1.0f);
			}

			ImGui.EndChild();
		}

	}
}
