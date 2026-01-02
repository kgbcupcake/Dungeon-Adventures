using System;
using System.Threading;
using System.Collections.Generic;
using DungeonAdventures.Src.Utilities;
using Pastel;

namespace DungeonAdventures
{
    public static class LoadingSequence
    {
        // Constants for hex colors
        private const string VEIN_RED = "#FF0000";
        private const string NUCLEAR_WASTE_GREEN = "#00FF00";
        private const string DARK_GRAY = "#A9A9A9"; // Approximating ConsoleColor.DarkGray
        private const string GREEN = "#00FF00"; // Approximating ConsoleColor.Green
        private const string WHITE = "#FFFFFF"; // Default white for text

        // Hard-coded dimensions as requested (84x24 for drawing area, RenderService is 85x25)
        private const int DRAW_WIDTH = 84;
        private const int DRAW_HEIGHT = 24;

        // Corruption characters (from existing Run method in file)
        private static readonly char[] CORRUPTION_CHARS = " !\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{{|}}~".ToCharArray();
        private static readonly Random _random = new Random();

        // The DUMGERAGN_LOGO and IMMUTABLE_ICONS from the file, they are not used in the new logic.
        // Keeping them here for now, but they will not be drawn by the new logic.
        // These can be removed if they are truly unused, but for now, maintaining file structure.
        private static readonly string[] DUMGERAGN_LOGO = new string[]
        {
            "  ██████╗ ██╗   ██╗███╗   ███╗ ██████╗ ███████╗██████╗  █████╗  ██████╗ ███╗   ██╗",
            "  ██╔══██╗██║   ██║████╗ ████║██╔════╝ ██╔════╝██╔══██╗██╔══██╗██╔════╝ ████╗  ██║",
            "  ██║  ██║██║   ██║██╔████╔██║██║  ███╗█████╗  ██████╔╝███████║██║  ███╗██╔██╗ ██║",
            "  ██║  ██║██║   ██║██║╚██╔╝██║██║   ██║██╔══╝  ██╔══██╗██╔══██║██║   ██║██║╚██╗██║",
            "  ██████╔╝╚██████╔╝██║ ╚═╝ ██║╚██████╔╝███████╗██║  ██║██║  ██║╚██████╔╝██║ ╚████║",
            "  ╚═════╝  ╚═════╝ ╚═╝     ╚═╝ ╚═════╝  ╚══════╝╚═╝  ╚═╝╚═════╝ ╚═════╝ ╚═╝  ╚═══╝"
        };
        private static readonly string[] IMMUTABLE_ICONS = { "💀", "⸸", "☣", "☢" };


        public static void Run()
        {
            RenderService.Initialize(); // Initialize RenderService
            Console.CursorVisible = false; // Keep cursor invisible

            // Loading messages from the user-provided logic
            List<string> scaryLabels = new List<string>
            {
                "Initializing existential dread...",
                "Calibrating entropy levels...",
                "Consulting ancient prophecies...",
                "Brewing potions of uncertainty...",
                "Summoning forgotten algorithms...",
                "Unraveling spacetime paradoxes...",
                "Feeding the dungeon's guardians...",
                "Polishing the skulls of heroes...",
                "Aligning cosmic horror...",
                "Loading... or are we?",
                "Reconfiguring reality anchors...",
                "Spawning untold terrors...",
                "Just kidding... mostly.",
                "Analyzing your life choices...",
                "Preparing inevitable demise...",
                "Good luck, you'll need it.",
                "Wait, what was that sound?",
                "They are coming.",
                "Run.",
                "There is no escape."
            };

            DateTime lastGlitchTime = DateTime.UtcNow;
            TimeSpan glitchInterval = TimeSpan.FromMilliseconds(500); // How often glitches can occur

            for (int i = 0; i <= 100; i++)
            {
                RenderService.ClearFrameBuffer(); // Clear buffer for new frame

                // 90% Background Noise: Fill 90% of the 84x24 buffer with random red/green noise.
                for (int y = 0; y < DRAW_HEIGHT; y++)
                {
                    for (int x = 0; x < DRAW_WIDTH; x++)
                    {
                        if (_random.Next(1, 101) <= 90) // 90% chance to draw noise
                        {
                            char noiseChar = CORRUPTION_CHARS[_random.Next(CORRUPTION_CHARS.Length)];
                            string noiseColor = _random.Next(0, 2) == 0 ? VEIN_RED : NUCLEAR_WASTE_GREEN;
                            RenderService.WriteToBuffer(x, y, noiseChar, noiseColor);
                        }
                    }
                }

                // Remove ASCII Art: Use the single line text [ D U M G E R A G N // SYSTEM_ERROR ] centered at the top.
                string systemErrorText = "[ D U M G E R A G N // SYSTEM_ERROR ]";
                RenderService.DrawCentered(0, systemErrorText, DARK_GRAY); // Centered at y=0, Dark Gray

                // Display a random scary message (centered horizontally, original Y position was ConsoleHeight / 2 - 2)
                string currentLabel = scaryLabels[_random.Next(scaryLabels.Count)];
                RenderService.DrawCentered(DRAW_HEIGHT / 2 - 2, currentLabel, VEIN_RED);

                // Progress bar
                int barWidth = 50;
                int barY = DRAW_HEIGHT / 2; // Original barY, but now relative to DRAW_HEIGHT

                // Calculate center X for the bar. Ensure it's centered within DRAW_WIDTH.
                // barWidth (50) + 2 for brackets + 3 for "% ". Total 55.
                int barTotalDisplayWidth = barWidth + 2 + 3; // [....] XX%
                int barStartX = (DRAW_WIDTH - barTotalDisplayWidth) / 2;

                RenderService.WriteToBuffer(barStartX, barY, "[", GREEN);
                int progressChars = (int)((double)i / 100 * barWidth);
                for (int k = 0; k < barWidth; k++)
                {
                    char progressChar = (k < progressChars) ? '█' : '░';
                    RenderService.WriteToBuffer(barStartX + 1 + k, barY, progressChar, GREEN);
                }
                RenderService.WriteToBuffer(barStartX + 1 + barWidth, barY, "]", GREEN);
                RenderService.WriteToBuffer(barStartX + 1 + barWidth + 2, barY, string.Format("{0}%", i), GREEN);

                // Glitch effect (random characters appearing briefly)
                if (_random.Next(0, 100) < 5 && (DateTime.UtcNow - lastGlitchTime) > glitchInterval) // 5% chance per frame
                {
                    int numGlitchChars = _random.Next(10, 50);
                    for (int g = 0; g < numGlitchChars; g++)
                    {
                        int x = _random.Next(0, DRAW_WIDTH);
                        int y = _random.Next(0, DRAW_HEIGHT);
                        char glitchChar = CORRUPTION_CHARS[_random.Next(CORRUPTION_CHARS.Length)]; // Use existing corruption chars
                        
                        // Random glitch color (approximating original random ConsoleColor)
                        string glitchColor;
                        switch (_random.Next(0, 5)) // Randomly pick from a few colors
                        {
                            case 0: glitchColor = VEIN_RED; break;
                            case 1: glitchColor = NUCLEAR_WASTE_GREEN; break;
                            case 2: glitchColor = DARK_GRAY; break;
                            case 3: glitchColor = WHITE; break;
                            case 4: glitchColor = GREEN; break;
                            default: glitchColor = WHITE; break;
                        }
                        RenderService.WriteToBuffer(x, y, glitchChar, glitchColor);
                    }
                    lastGlitchTime = DateTime.UtcNow;
                }

                RenderService.Present(); // Push the buffer to the screen
                Thread.Sleep(50); // Simulate work
            }
            
            Console.CursorVisible = true; // Restore cursor visibility at the end
        }
    }
}
