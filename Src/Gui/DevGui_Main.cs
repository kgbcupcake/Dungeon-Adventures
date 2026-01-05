using ClickableTransparentOverlay;
using DungeonAdventures.Src.GameEngine;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DungeonAdventures.Src.Interfaces
{
    public partial class DevGuiRenderer : Overlay
    {
		private readonly Conductor conductor = new Conductor();
		public DevGuiRenderer()
		{
			GameVersion = "Alpha 0.0.1";
			DevName = "DevConsole v6.0 - System Dashboard";
			_jsonEditorBuffer = string.Empty;
			conductor = new Conductor();
			Console.SetOut(new ImGuiLogRedirector());
			LoadTheme();

		}
	}
































	public class ImGuiLogRedirector : System.IO.TextWriter
	{
		private System.IO.TextWriter _originalConsole;
		public override System.Text.Encoding Encoding => System.Text.Encoding.UTF8;
		public ImGuiLogRedirector() { _originalConsole = Console.Out; }

		public static void ClearAll()
		{
			try { Console.Clear(); } catch { }
			lock (DevGuiRenderer.DevLog._lock) { DevGuiRenderer.DevLog.Buffer.Clear(); }
		}

		public override void Write(string? value)
		{
			_originalConsole.Write(value);
			if (!string.IsNullOrEmpty(value)) DevGuiRenderer.DevLog.Write(value, "LOG");
		}

		public override void WriteLine(string? value)
		{
			_originalConsole.WriteLine(value);
			if (value != null) DevGuiRenderer.DevLog.Write(value, "SYSTEM");
		}
	}
}