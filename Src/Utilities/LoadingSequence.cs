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

        private static readonly char[] CORRUPTION_CHARS = " !\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{{|}}~".ToCharArray();
        private static readonly Random _random = new Random();

        // The DUMGERAGN ASCII logo as a static array
        private static readonly string[] DUMGERAGN_LOGO = new string[]
        {
            "  ██████╗ ██╗   ██╗███╗   ███╗ ██████╗ ███████╗██████╗  █████╗  ██████╗ ███╗   ██╗",
            "  ██╔══██╗██║   ██║████╗ ████║██╔════╝ ██╔════╝██╔══██╗██╔══██╗██╔════╝ ████╗  ██║",
            "  ██║  ██║██║   ██║██╔████╔██║██║  ███╗█████╗  ██████╔╝███████║██║  ███╗██╔██╗ ██║",
            "  ██║  ██║██║   ██║██║╚██╔╝██║██║   ██║██╔══╝  ██╔══██╗██╔══██║██║   ██║██║╚██╗██║",
            "  ██████╔╝╚██████╔╝██║ ╚═╝ ██║╚██████╔╝███████╗██║  ██║██║  ██║╚██████╔╝██║ ╚████║",
            "  ╚═════╝  ╚═════╝ ╚═╝     ╚═╝ ╚═════╝  ╚══════╝╚═╝  ╚═╝╚═╝  ╚═╝ ╚═════╝ ╚═╝  ╚═══╝"
        };
        private static readonly string[] IMMUTABLE_ICONS = { "💀", "⸸", "☣", "☢" };


        public static void Run()
        {
            RenderService.Initialize();

            for (int iteration = 0; iteration < 20; iteration++) // 20 iterations for Hardware Agony
            {
                RenderService.ClearFrameBuffer(); // Fills buffer with spaces

                // Step 1: Generate 90% corruption noise across the entire buffer
                for (int y = 0; y < CONSOLE_HEIGHT; y++)
                {
                    for (int x = 0; x < CONSOLE_WIDTH; x++)
                    {
                        if (_random.Next(1, 101) <= 90) // 90% chance to corrupt
                        {
                            char corruptionChar = CORRUPTION_CHARS[_random.Next(CORRUPTION_CHARS.Length)];
                            string color = _random.Next(0, 2) == 0 ? VEIN_RED : NUCLEAR_WASTE_GREEN;
                            RenderService.WriteToBuffer(x, y, corruptionChar, color);
                        }
                    }
                }

                // Step 2: Print the Immutable Seed (DUMGERAGN logo and icons) on top
                // This ensures the logo is never corrupted and always visible
                int logoStartY = (CONSOLE_HEIGHT / 2) - (DUMGERAGN_LOGO.Length / 2);
                for (int i = 0; i < DUMGERAGN_LOGO.Length; i++)
                {
                    RenderService.DrawCentered(logoStartY + i, DUMGERAGN_LOGO[i], VEIN_RED);
                }

                RenderService.WriteToBuffer(2, 2, IMMUTABLE_ICONS[0], NUCLEAR_WASTE_GREEN);
                RenderService.WriteToBuffer(CONSOLE_WIDTH - 3, 2, IMMUTABLE_ICONS[1], NUCLEAR_WASTE_GREEN);
                RenderService.WriteToBuffer(2, CONSOLE_HEIGHT - 2, IMMUTABLE_ICONS[2], NUCLEAR_WASTE_GREEN);
                RenderService.WriteToBuffer(CONSOLE_WIDTH - 3, CONSOLE_HEIGHT - 2, IMMUTABLE_ICONS[3], NUCLEAR_WASTE_GREEN);


                RenderService.Present(); // Single Console.Write call
                Thread.Sleep(800);      // Hardware Agony cycle
            }
        }
    }
}