using DungeonAdventures.Src.Game.Encounters;
using Pastel;
using static System.Console;
using DungeonAdventures.Src.Utilities.UI;
using DungeonAdventures.Src.GameData.Components;

namespace DungeonAdventures.Src.Adventures.Encounters.CumstomEncounters
{
	internal static class Bosses
	{
		public static void Wizardboss()
		{
			// Accessing the player from GameState
			var player = GameState.CurrentPlayer;

			// Using your health variable
			if (player.Health > 15)
			{
				Clear();
				WriteLine("The door rips open! A tall man with a long beard looks up from a large tome...");
				WriteLine("Dark Wizard: You dare interrupt my research? You shall be turned to ash!");
				ReadKey(true);

				// Calling the updated Combat method
				MainEncounter.Combat(false, "Dark Wizard Raider".Pastel("#D60B18"), 5, 10);
			}
			else
			{
				WriteLine("You hear a powerful humming behind the door, but you are too weak to enter...");
				ReadKey(true);
			}
		}

		/// <summary>
		/// Rat Boss encounter logic
		/// </summary>
		public static void RatBoss()
		{
			var player = GameState.CurrentPlayer;

			Clear();
			// Using your Functions header for consistency
			UiEngine.DrawCentered("BOSS ENCOUNTER".Pastel("#D60B18"), 10);

			SetCursorPosition(5, 10);
			WriteLine("A massive, plague-ridden rat emerges from the shadows!");
			WriteLine("Can you handle the Rat King?".Pastel("#D60B18"));
			ReadKey(true);

			// High stakes boss stats
			MainEncounter.Combat(false, "Rat King".Pastel("#D60B18"), 15, 25);

			// Post-boss logic
			if (player.Health > 0)
			{
				Clear();
				WriteLine("Rat King: Squeeeee! (You have defeated me...)");
				WriteLine("You feel a sense of peace return to the sewers.");
				ReadKey(true);

				// Transitioning back to the town square after victory
				// TownSquare.MainTownsquare(); 
			}
		}
	}
}