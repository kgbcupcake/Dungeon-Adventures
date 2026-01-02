using System.Runtime.InteropServices;

namespace DungeonAdventures.csproj.Src.Utilities.UI
{
    public static class ScreenInfo
    {
        // Declare the GetSystemMetrics function from user32.dll
        [DllImport("user32.dll")]
        private static extern int GetSystemMetrics(int nIndex);

        // Define constants for the metrics we want to retrieve
        private const int SM_CXSCREEN = 0; // Screen width
        private const int SM_CYSCREEN = 1; // Screen height

        public static (int Width, int Height) GetScreenDimensions()
        {
            int screenWidth = GetSystemMetrics(SM_CXSCREEN);
            int screenHeight = GetSystemMetrics(SM_CYSCREEN);
            return (screenWidth, screenHeight);
        }
    }
}
