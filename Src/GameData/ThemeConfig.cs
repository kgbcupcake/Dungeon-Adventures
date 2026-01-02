namespace DungeonAdventures.Src.GameData
{
    public class ThemeConfig
    {
        public float[] FontColor { get; set; } = new float[] { 1f, 1f, 1f, 1f };
        public float[] TgbColor { get; set; } = new float[] { 0.1f, 0.1f, 0.1f, 1f };
        public float[] WindowBgColor { get; set; } = new float[] { 0.1f, 0.1f, 0.1f, 1f };
        public float[] BorderColor { get; set; } = new float[] { 0f, 1f, 1f, 1f };
        public float[] ChildBgColor { get; set; } = new float[] { 0.08f, 0.08f, 0.12f, 1f };
        public float WindowBorderSize { get; set; } = 1.0f;
        public float FrameBorderSize { get; set; } = 1.0f;
        public float ChildBorderSize { get; set; } = 1.0f;
        public float WindowRounding { get; set; } = 5.0f;
        public float FrameRounding { get; set; } = 4.0f;
        public float ChildRounding { get; set; } = 4.0f;
        public float[] WindowSize { get; set; } = new float[] { 900f, 700f };
    }
}
