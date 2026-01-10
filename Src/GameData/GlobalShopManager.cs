using DungeonAdventures.Src.GameEngine; // Required for LoadGame

namespace DungeonAdventures.Src.GameData
{
    public static class GlobalShopManager
    {
        public static List<ItemData> Items { get; set; } = new List<ItemData>();

        public static void LoadAllFromMaster()
        {
            Items = LoadGame.LoadAllFromFolder<ItemData>("shop_stock");
        }
    }
}