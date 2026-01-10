using System;
using System.Text;
using Pastel;

namespace DungeonAdventures.csproj.Src.Utilities.UI
{
    public static class RenderService
    {
        private struct Pixel
        {
            public char Character;
            public string Color; // Hex color string
        }

        private static Pixel[] _frameBuffer;
        private const int CONSOLE_WIDTH = 85;
        private const int CONSOLE_HEIGHT = 25;

        static RenderService()
        {
            _frameBuffer = new Pixel[CONSOLE_WIDTH * CONSOLE_HEIGHT];
            ConsoleManager.SetupConsole(CONSOLE_WIDTH, CONSOLE_HEIGHT);
        }

        public static void ClearFrameBuffer()
        {
            // As per "Total Frame Reset", fill with empty space for now.
            for (int i = 0; i < _frameBuffer.Length; i++)
            {
                _frameBuffer[i].Character = ' ';
                _frameBuffer[i].Color = "#FFFFFF"; // Default to white
            }
        }

        public static void WriteToBuffer(int x, int y, char character, string color = "#FFFFFF")
        {
            if (x >= 0 && x < CONSOLE_WIDTH && y >= 0 && y < CONSOLE_HEIGHT)
            {
                int index = y * CONSOLE_WIDTH + x;
                _frameBuffer[index].Character = character;
                _frameBuffer[index].Color = color;
            }
        }

        public static void WriteToBuffer(int x, int y, string text, string color = "#FFFFFF")
        {
            for (int i = 0; i < text.Length; i++)
            {
                WriteToBuffer(x + i, y, text[i], color);
            }
        }

        public static void Present()
        {
            Console.SetCursorPosition(0, 0);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < _frameBuffer.Length; i++)
            {
                sb.Append(_frameBuffer[i].Character.ToString().Pastel(_frameBuffer[i].Color));
            }
            Console.Write(sb.ToString());
        }

        // Add method to draw centered text, respecting the 85-width offset
        public static void DrawCentered(int y, string text, string color = "#FFFFFF")
        {
            int startX = (CONSOLE_WIDTH - text.Length) / 2;
            WriteToBuffer(startX, y, text, color);
        }
    }
}
