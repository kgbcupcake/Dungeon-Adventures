using DungeonAdventures.Src.Utilities.UI;
using Pastel;
using System;

namespace DungeonAdventures.Src.Utilities.GameArt
{
	#region// Main Menu Art
	public static class MainMenuArt
	{
		public static void DrawMainHeader()
		{
			// 1. Create a dynamic separator based on current window width
			// Subtracting 2 ensures we don't hit the edge and cause a scrollbar
			string separator = "<" + new string('-', Console.WindowWidth - 2) + ">";
			string titleText = "D U N G E O N   A D V E N T U R E S - R E B O R N";

			// 2. Set the color and draw
			// We use line 1 and 3 for separators, and line 2 for the text
			Console.ForegroundColor = ConsoleColor.DarkRed;

			UiEngine.DrawCentered(separator, 1);
			UiEngine.DrawCentered(titleText.Pastel("#8B0000"), 2);
			UiEngine.DrawCentered(separator, 3);

			Console.ResetColor();
		}
	}
	#endregion
	#region// Game Over Art
	public static class GameOver 
	{
		public static readonly string[] CynicalSkull = new string[]
		{
			"         YOU WERE JUST RENTED DATA         ",
			"___________________________________________",
			"         oooo$$$$$$$$oooo                  ",
			"      oo$$$$$$$$$$$$$$$$$$oo               ",
			"     od$$$$$$$$$$$$$$$$$$$$$bo             ",
			"     oo$$$$$$$$$$$$$$$$$$$$$$oo            ",
			"     $$$$$$$$$$$$$$$$$$$$$$$$$$            ",
			"     $$$$$$$$$$$$$$$$$$$$$$$$$$            ",
			"     $$$   $$$$$$$$$$$$   $$$$$            ",
			"     \"$$$   $$$$$$$$$$   $$$$$\"            ",
			"      \"$$$oooo$$$$$$$oooo$$$\"              ",
			"        \"$$$$$$$$$$$$$$$$$\"                ",
			"    oooo$$$$$$$$$$$$$$$$$$oooo             ",
			"   $$$$$$$$\"\"$$$$$$$$$??$$$$$$$            ",
			"    \"\"$$$$\"  \"$$$$$$$\"  \"$$$$\"             ",
			"       \"$$$    \"$$$\"    $$$\"               ",
			"___________________________________________",
			"          [ REALITY TERMINATED ]           "
		};
	}


	#endregion


}