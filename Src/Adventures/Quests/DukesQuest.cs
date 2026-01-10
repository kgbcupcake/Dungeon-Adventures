
using DungeonAdventures.Src.Adventures.Interfaces;
using DungeonAdventures.Src.Adventures.Services;
using DungeonAdventures.Src.Game.Encounters;
using DungeonAdventures.Src.GameData;
using DungeonAdventures.Src.GameData.Components;
using DungeonAdventures.Src.Utilities.UI;
using Pastel;
using static System.Console;

namespace DungeonAdventures.Src.Adventures.Quests
{
	internal class DukesQuest
	{
		private IAdventureService adventureServices;
		
		public DukesQuest(IAdventureService AdventureServices)
		{
			adventureServices = AdventureServices;

		}

		public static string Indent(int count)
		{
			return "".PadLeft(count);
		}

		public void DukesMainQuest()
		{
			// 1. Fetch the list and find Duke's data specifically
			var allQuests = adventureServices.LoadAllQuests();
			var loadDukesquest = allQuests.FirstOrDefault(q => q.Title.Contains("Duke"))
								 ?? new AdventureData { Title = "Dukes Quest", DescriptionD = "Default Description" };

			// Shortcut for the player
			var p = GameState.CurrentPlayer;

			// 2. UI Header (Matching your Hall of Heroes style)
			Clear();
			UiFunctions.TitleBar(); // This handles the top title bar
			UiEngine.DrawCentered($"HERO: {p?.PlayerName?.ToUpper() ?? "UNKNOWN"} | LVL: {p?.Level ?? 0}", 0); // Display the text on line 0
			// gameArt.DukesTitle(); // Uncomment when ready

			string? q1 = "Quest One:".Pastel("#82282E");
			string? dec = "Description:".Pastel("#82282E");

			WriteLine(q1 + " You Have Chosen ".Pastel("#154871") + loadDukesquest.Title.Pastel("#A02DA3"));
			WriteLine("Reward: ".Pastel("#FFD700") + loadDukesquest.CompletionXPReward + " XP");
			WriteLine(dec + " " + loadDukesquest.DescriptionD.Pastel("#154875"));
			WriteLine(new string('─', 60).Pastel("#333333"));

			// 3. Choice Logic
			bool tookTorch = UiEngine.GetArrowChoice(
				"As you enter the Sewers you notice there are no lights... Do you take a torch?",
				"Yes, I need the light.",
				"No, I'll trust my instincts."
			);

			if (tookTorch)
			{
				Clear();
				UiFunctions.TitleBar(); // This handles the top title bar
				UiEngine.DrawCentered("INVENTORY UPDATED", 0); // Display the text on line 0
				WriteLine(" You have successfully taken the torch off of the wall!".Pastel("#00FF00"));
				WriteLine(" This light source won't last forever though...");
				// p.Inventory.Add("Torch"); // Use your new loadPlayer inventory!
			}
			else
			{
				Clear();
				WriteLine(" You decide to traverse the darkness... It is eerily quiet.".Pastel("#555555"));
			}

			// 4. Story Progression
			WriteLine("\nAs you continue your way down into the Sewers you come across a pile of bones...");
			WriteLine("A figure stands in the distance. You whistle to get their attention.");
			WriteLine("The figure turns... and charges!".Pastel("#FF0000"));
			WriteLine("\n[ Press any key to FIGHT ]");
			ReadKey(true);

			// Combat Scene
			MainEncounter.FirstEncounter();

			// After combat logic
			if (p != null && p.Health > 0)
			{
				WriteLine("\nAs you Continue your way into the sewers you come across another Raider");
				// Next encounter logic...
			}
		}

	}
}
