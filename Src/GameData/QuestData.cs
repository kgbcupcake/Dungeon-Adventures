namespace DungeonAdventures.Src.GameData
{
    public class QuestData
    {
        public int ID { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<QuestObjective> Objectives { get; set; } = new List<QuestObjective>();
        public Reward Rewards { get; set; } = new Reward();
        public bool IsCompleted { get; set; } = false;
    }

    public class QuestObjective
    {
        public string Description { get; set; } = string.Empty;
        public int TargetCount { get; set; } // e.g., 5 for "Kill 5 boars"
        public int CurrentCount { get; set; } = 0;
        // We might need a TargetID later, e.g., the ID of the monster to kill
    }

    public class Reward
    {
        public int ExperiencePoints { get; set; }
        public int Gold { get; set; }
        // We could add a List<Item> here later
    }
}
