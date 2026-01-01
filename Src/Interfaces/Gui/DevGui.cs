using ClickableTransparentOverlay;
using DungeonAdventures.Src.GameData;
using DungeonAdventures.Src.GameData.Components;
using DungeonAdventures.Src.GameEngine;
using ImGuiNET;
using System.Numerics;
using System.Drawing; // For Point, needed for Screen.PrimaryScreen.Bounds


namespace DungeonAdventures.Src.Interfaces
{
	public partial class DevGuiRenderer : Overlay
	{
		private Conductor _conductor = new Conductor();
		private bool _showCreateProfileWindow = false; // New boolean for persistent player creator window
		private bool _isTempPlayerInitialized = false; // New boolean to track temp player initialization
		private int _currentTab = 0; // 0 = Weapons, 1 = Gems, 2 = Bosses

		private IntPtr _bioIcon = IntPtr.Zero; // Example: Icon for Bio-Engineering tab
		private IntPtr _forgeIcon = IntPtr.Zero; // Example: Icon for Industrial Forge tab
		private IntPtr _worldIcon = IntPtr.Zero; // Example: Icon for World Control tab
		private IntPtr _profileThumbnail = IntPtr.Zero; // Dynamic image for loaded player profile
		private IntPtr _weaponPreview = IntPtr.Zero; // Dynamic image for selected weapon
		private IntPtr _alchemyPreview = IntPtr.Zero; // Dynamic image for selected alchemy item
		private IntPtr _gemPreview = IntPtr.Zero; // Dynamic image for selected gem



		public DevGuiRenderer()
		{
			GameVersion = "Alpha 0.0.1";
			DevName = "DevConsole v6.0 - System Dashboard";
			_jsonEditorBuffer = string.Empty;
			Console.SetOut(new ImGuiLogRedirector());
			LoadTheme();

		}

		public static class DevLog
		{
			public static List<string> Buffer = new List<string>();
			private static int _maxLines = 200;
			public static readonly object _lock = new object();
		

			public static void Write(string message, string type = "INFO")
			{
				if (string.IsNullOrWhiteSpace(message)) return;
				lock (_lock)
				{
					string timestamp = DateTime.Now.ToString("HH:mm:ss");
					string formattedLine = message.StartsWith("[")
						? $"[{timestamp}] {message}"
						: $"[{timestamp}] [{type}] {message}";
					Buffer.Add(formattedLine);
					if (Buffer.Count > _maxLines) Buffer.RemoveAt(0);
				}
			}
		}
		protected override void Render()
		{
			// --- 1. SYSTEM GATEKEEPER ---
			bool open = this.IsVisible;
			if (!GameState.IsDevMode || !open) return;

			// --- 2. THE SPICY VISUAL OVERHAUL ---
			ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, 8.0f);
			ImGui.PushStyleVar(ImGuiStyleVar.FrameRounding, 4.0f);
			ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 2.0f);
			ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(12, 10));

			ImGui.PushStyleColor(ImGuiCol.WindowBg, new Vector4(0.04f, 0.04f, 0.05f, 0.95f));
			ImGui.PushStyleColor(ImGuiCol.Border, new Vector4(0, 0.8f, 0.8f, 1.0f));
			ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.9f, 0.9f, 1.0f, 1.0f));
			// This forces the 'Invisible Box' to be the size of your whole monitor
			this.Position = new Point(0, 0);
			this.Size = new Size(1920, 1080); // Set this to your actual monitor resolution
											  // --- 3. THE MOBILITY FIX ---
											  // 'Appearing' ensures it starts at 100,100 but lets you move it ANYWHERE without cutting off.
			ImGui.SetNextWindowSize(new Vector2(1250, 850), ImGuiCond.Appearing);
			ImGui.SetNextWindowPos(new Vector2(100, 100), ImGuiCond.Appearing);

			// Make sure we have a TitleBar so you can grab it!
			bool isVisible = ImGui.Begin(DevName, ref open, ImGuiWindowFlags.MenuBar);

			if (isVisible)
			{
				// TOP MENU
				if (ImGui.BeginMenuBar())
				{
					ImGui.TextColored(new Vector4(0, 1, 1, 1), " [ CORE LINK: ACTIVE ] ");
					ImGui.Separator();
					if (ImGui.MenuItem(" MASTER SYNC ")) { GameState.Sync(); }
					ImGui.EndMenuBar();
				}

				// --- DUAL COLUMN LAYOUT ---
				ImGui.Columns(2, "MainLayout", true);
				ImGui.SetColumnWidth(0, ImGui.GetWindowWidth() - 360);

				// LEFT COLUMN: FUNCTIONAL HUBS
				// Using '###' in IDs stops the 'assertion failed: id != 0' crash
				if (ImGui.BeginTabBar("ControlHubs###main_tabs", ImGuiTabBarFlags.Reorderable))
				{
					if (ImGui.BeginTabItem(" [🧬] BIO-ENGINEERING : ###bio_tab"))
					{
						RenderFullStatEditor();
						ImGui.EndTabItem();
					}

					if (ImGui.BeginTabItem(" [🛠] INDUSTRIAL FORGE : ###forge_tab"))
					{
						// Put your sub-tabs or forge logic here
						RenderSpicyForgeSubTabs();
						ImGui.EndTabItem();
					}

					if (ImGui.BeginTabItem(" [🗺] WORLD FABRICATOR : ###world_tab"))
					{
						RenderWorldForgePanel();
						ImGui.EndTabItem();
					}
					ImGui.EndTabBar();
				}

				ImGui.NextColumn();

				// --- RIGHT COLUMN: TERMINAL & ARCHIVE ---
				ImGui.TextColored(new Vector4(0, 1, 1, 1), " [💻] TERMINAL_OPS :");

				if (ImGui.Button(" INJECT_SYSTEM : F10 ###inj_btn", new Vector2(-1, 50)))
				{
					DevLog.Write("Manual Injection sequence initiated...", "CRITICAL");
				}

				ImGui.Separator();
				ImGui.TextColored(new Vector4(0.5f, 0.5f, 1.0f, 1.0f), " [🗄] NEURAL_ARCHIVE :");
				RenderArchiveBrowser_Flat();

				ImGui.Separator();
				ImGui.TextColored(new Vector4(1, 0.8f, 0, 1), " [📋] SYSTEM_LOG :");
				RenderDevLog();

				ImGui.Columns(1);
			}

			// --- 4. THE STACK RECOVERY ---
			ImGui.End();
			ImGui.PopStyleColor(3);
			ImGui.PopStyleVar(4);

			// --- 5. DAEMONS ---
			this.IsVisible = open;
			HandleMasterHotkeys(ref open);
			RenderDeletePopup();
			RenderFullJsonEditor();
			RenderInjectionStatus();
		}
		private void RenderSpicyForgeSubTabs()
		{
			if (ImGui.BeginTabBar("ForgeTabs"))
			{
				if (ImGui.BeginTabItem(" [⚔️] WEAPON LAB : "))
				{
					RenderAdvancedWeaponLab();
					ImGui.EndTabItem();
				}
				if (ImGui.BeginTabItem(" [💎] GEM ALCHEMY : "))
				{
					RenderGemAlchemy();
					ImGui.EndTabItem();
				}
				if (ImGui.BeginTabItem(" [🧪] ITEM CREATOR : "))
				{
					RenderItemCreator();
					ImGui.EndTabItem();
				}
				ImGui.EndTabBar();
			}
		}

		private void RenderWorldForgePanel()
		{
			ImGui.TextColored(new Vector4(0, 1, 1, 1), " [🌍] REALITY_STITCHER :");
			ImGui.Separator();

			// Use Buttons instead of Sliders/Radios for "Spicy" stability
			if (ImGui.Button(" [⚡] GENERATE DUNGEON : ", new Vector2(-1, 35))) { /* Logic */ }
			if (ImGui.Button(" [💀] SUMMON BOSS : ", new Vector2(-1, 35))) { /* Logic */ }
			if (ImGui.Button(" [🌪] REWRITE BIOME : ", new Vector2(-1, 35))) { /* Logic */ }

			ImGui.Spacing();
			ImGui.TextDisabled(" CURRENT_SECTOR : [FRAGMENT_01] ");
		}



		private void RenderArchiveBrowser_Flat()
		{
			string playerPath = Path.Combine(GameState.MasterPath, "players");
			if (!Directory.Exists(playerPath))
			{
				ImGui.TextColored(new Vector4(1, 0, 0, 1), " [!] PATH_NOT_FOUND : NULL");
				return;
			}

			string[] files = Directory.GetFiles(playerPath, "*.json");

			// ONLY ONE LOOP ALLOWED HERE
			foreach (var file in files)
			{
				string shortName = Path.GetFileNameWithoutExtension(file);
				bool isActive = GameState.CurrentPlayer?.PlayerName == shortName;

				// CRITICAL FIX: We push a unique string prefix to prevent the id != 0 error
				ImGui.PushID($"row_{file}");

				string icon = isActive ? ">> [🧬] " : "   [👤] ";

				// We use ### to separate the spicy label from the permanent ID
				string spicyLabel = $"{icon}{shortName.ToUpper()} : ###{file}";

				if (isActive) ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0, 1, 1, 1));

				if (ImGui.Selectable(spicyLabel, isActive, ImGuiSelectableFlags.None, new Vector2(0, 22)))
				{
					LoadPlayerFromFile(file);
				}

				if (isActive) ImGui.PopStyleColor();

				// Right-Click Options
				if (ImGui.BeginPopupContextItem())
				{
					if (ImGui.MenuItem(" [🔄] RE-LINK : ")) { LoadPlayerFromFile(file); }
					ImGui.EndPopup();
				}

				// MUST POP THE ID BEFORE NEXT ITERATION
				ImGui.PopID();
				ImGui.Spacing();
			}
		}





		/// <summary>
		/// end of Gui
		/// </summary>
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
}