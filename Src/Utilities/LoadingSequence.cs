using System;
using System.Text;
using System.Threading;
using Pastel;

namespace DungeonAdventures.Src.Utilities
{
    public static class LoadingSequence
    {
        private const int CONSOLE_WIDTH = 85;
        private const int CONSOLE_HEIGHT = 25;
        private const string VEIN_RED = "#FF0000";
        private const string NUCLEAR_WASTE_GREEN = "#00FF00";

        private static readonly string[] CORRUPTION_CHARS = { " ", "░", "▒", "▓", "█" };
        private static readonly Random _random = new Random();

        public static void Run()
        {
            RenderService.Initialize();

            for (int i = 0; i < 20; i++)
            {
                RenderService.ClearFrameBuffer();
                DrawImmutableSeed();
                DrawAsciiCorruption();
                RenderService.Present();
                Thread.Sleep(800);
            }
        }

        private static void DrawImmutableSeed()
        {
            string logo = "DUMGERAGN";
            string[] icons = { "💀", "⸸", "☣", "☢" };

            RenderService.DrawCentered(5, logo);
            RenderService.WriteToBuffer(2, 2, icons[0]);
            RenderService.WriteToBuffer(CONSOLE_WIDTH - 3, 2, icons[1]);
            RenderService.WriteToBuffer(2, CONSOLE_HEIGHT - 2, icons[2]);
            RenderService.WriteToBuffer(CONSOLE_WIDTH - 3, CONSOLE_HEIGHT - 2, icons[3]);
        }

        private static void DrawAsciiCorruption()
        {
            int corruptionAmount = (int)(CONSOLE_WIDTH * CONSOLE_HEIGHT * 0.5);
            for (int i = 0; i < corruptionAmount; i++)
            {
                int x = _random.Next(0, CONSOLE_WIDTH);
                int y = _random.Next(0, CONSOLE_HEIGHT);
                string corruptionChar = CORRUPTION_CHARS[_random.Next(0, CORRUPTION_CHARS.Length)];
                string color = _random.Next(0, 2) == 0 ? VEIN_RED : NUCLEAR_WASTE_GREEN;
                RenderService.WriteToBuffer(x, y, corruptionChar.Pastel(color));
            }
        }
    }
}
