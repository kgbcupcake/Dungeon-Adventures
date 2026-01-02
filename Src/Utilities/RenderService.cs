using System;
using System.Text;

namespace DungeonAdventures.Src.Utilities
{
    public static class RenderService
    {
        private static StringBuilder _frameBuffer = new StringBuilder();
        private const int CONSOLE_WIDTH = 85;
        private const int CONSOLE_HEIGHT = 25;

        public static void Initialize()
        {
            // Ensure ConsoleManager has been called to set up console dimensions
            ConsoleManager.SetupConsole(CONSOLE_WIDTH, CONSOLE_HEIGHT);
        }

        public static void ClearFrameBuffer()
        {
            // As per "Total Frame Reset", fill with empty space for now.
            // Actual immutable seed will be placed by the caller (LoadingSequence)
            _frameBuffer.Clear();
            for (int i = 0; i < CONSOLE_WIDTH * CONSOLE_HEIGHT; i++)
            {
                _frameBuffer.Append(' ');
            }
        }

        public static void WriteToBuffer(int x, int y, char character)
        {
            if (x >= 0 && x < CONSOLE_WIDTH && y >= 0 && y < CONSOLE_HEIGHT)
            {
                int index = y * CONSOLE_WIDTH + x;
                if (index < _frameBuffer.Length)
                {
                    _frameBuffer[index] = character;
                }
            }
        }

        public static void WriteToBuffer(int x, int y, string text)
        {
            for (int i = 0; i < text.Length; i++)
            {
                WriteToBuffer(x + i, y, text[i]);
            }
        }


        public static void Present()
        {
            Console.SetCursorPosition(0, 0);
            // Do not use Console.Write(_frameBuffer.ToString()) directly due to potential performance and flushing issues
            // Instead, write character by character or in chunks if performance becomes an issue
            // For now, let's assume direct write is acceptable for stringbuilder content
            Console.Write(_frameBuffer.ToString());
        }

        // Add method to draw centered text, respecting the 85-width offset
        public static void DrawCentered(int y, string text)
        {
            int startX = (CONSOLE_WIDTH - text.Length) / 2;
            WriteToBuffer(startX, y, text);
        }
    }
}
