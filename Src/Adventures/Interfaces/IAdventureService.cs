using System;
using System.Collections.Generic;
using static System.Console;
using DungeonAdventures.Src.Utilities.UI;
using DungeonAdventures.Src.GameData;

namespace DungeonAdventures.Src.Adventures.Interfaces
{

	public interface IAdventureService
	{
		// Change from 'Quest' or 'GetLoadDukesquest' to this:
		List<DungeonAdventures.Src.GameData.AdventureData> LoadAllQuests();
	}
}