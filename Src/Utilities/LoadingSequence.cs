using System;
using System.Threading;
using System.Collections.Generic;
using DungeonAdventures.Src.Utilities;
using Pastel; // This is already in the file.

namespace DungeonAdventures
{
    public static class LoadingSequence
    {
        // Constants for hex colors (from UiFunctions.cs's StartGameLoading and prompt)
        private const string VEIN_RED = "#FF0000"; // Red
        private const string DARK_RED = "#8B0000"; // From UiFunctions (Dark Red)
        private const string INDIGO = "#4B0082";   // From UiFunctions (Indigo)
        private const string MAROON = "#800000";   // From UiFunctions (Maroon)
        private const string INDIAN_RED = "#A52A2A"; // From UiFunctions (IndianRed)
        private const string DIM_GRAY = "#696969";  // From UiFunctions (DimGray)
        private const string GRAY = "#404040";    // From UiFunctions (Gray)
        private const string DARK_SLATE_GRAY = "#2F4F4F"; // From UiFunctions (DarkSlateGray)

        private const string NUCLEAR_WASTE_GREEN = "#00FF00"; // Green for noise
        private const string WHITE = "#FFFFFF"; // Default white for text
        private const string DARK_GRAY = "#A9A9A9"; // For the SYSTEM_ERROR text

        // Hard-coded dimensions as requested (84x24 for drawing area, RenderService is 85x25)
        private const int DRAW_WIDTH = 84;
        private const int DRAW_HEIGHT = 24;

        // Corruption characters
        private static readonly char[] CORRUPTION_CHARS = " !\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{{|}}~".ToCharArray();
        private static readonly Random _random = new Random();

        // The DUMGERAGN_LOGO and IMMUTABLE_ICONS from the original file, not used in the new logic.
        // Keeping them for now as they were part of the existing file.
        private static readonly string[] DUMGERAGN_LOGO = new string[]
        {
            "  ██████╗ ██╗   ██╗███╗   ███╗ ██████╗ ███████╗██████╗  █████╗  ██████╗ ███╗   ██╗",
            "  ██╔══██╗██║   ██║████╗ ████║██╔════╝ ██╔════╝██╔══██╗██╔══██╗██╔════╝ ████╗  ██║",
            "  ██║  ██║██║   ██║██╔████╔██║██║  ███╗█████╗  ██████╔╝███████║██║  ███╗██╔██╗ ██║",
            "  ██║  ██║██║   ██║██║╚██╔╝██║██║   ██║██╔══╝  ██╔══██╗██╔══██║██║   ██║██║╚██╗██║",
            "  ██████╔╝╚██████╔╝██║ ╚═╝ ██║╚██████╔╝███████╗██║  ██║██║  ██║╚██████╔╝██║ ╚████║",
            "  ╚═════╝  ╚═════╝ ╚═╝     ╚═╝ ╚═════╝  ╚══════╝╚═╝  ╚═╝╚═════╝ ╚═════╝ ╚═════╝  ╚═══╝"
        };
        private static readonly string[] IMMUTABLE_ICONS = { "💀", "⸸", "☣", "☢" };


        public static void Run()
        {
            
            Console.CursorVisible = false;

            // Scary labels from UiFunctions.cs
            string[] scaryLabels = {
                "SYSTEM INTEGRITY: CRITICAL...",
                "REALITY ANCHORS: DEGRADED...",
                "INITIATING UNSTABLE PROTOCOL...",
                "WARNING: ENTITY DETECTED...",
                "ERROR: DIMENSIONAL FRACTURE...",
                "LOADING NIGHTMARE PROTOCOL...",
                "S̴Y̷S̷T̵E̵M̴ ̶C̶O̵R̷R̴U̷P̷T̸E̵D̸...",
                "F̸A̴I̸L̴U̶R̷E̵ ̸T̸O̵ ̷C̸O̷N̷N̷E̷C̷T̵ ̶T̷O̵ ̸S̵E̸R̷V̶E̵R̶..."
            };

            // Scary colors from UiFunctions.cs
            string[] scaryColors = { VEIN_RED, DARK_RED, INDIGO, MAROON, INDIAN_RED, DIM_GRAY, GRAY, DARK_SLATE_GRAY };

            // --- Initial Static Burst / Glitch Effect (from UiFunctions.cs) ---
            for (int j = 0; j < 5; j++) // Short, jarring flashes
            {
                RenderService.ClearFrameBuffer();
                for (int y = 0; y < DRAW_HEIGHT; y++)
                {
                    for (int x = 0; x < DRAW_WIDTH; x++)
                    {
                        if (_random.Next(0, 100) < 30) // 30% chance for a character
                        {
                            char c = (char)_random.Next(33, 126); // Printable ASCII
                            string color = _random.Next(2) == 0 ? VEIN_RED : NUCLEAR_WASTE_GREEN; // Red/Green glitch
                            RenderService.WriteToBuffer(x, y, c, color);
                        }
                        else
                        {
                            RenderService.WriteToBuffer(x, y, ' ', WHITE); // Draw space in default color
                        }
                    }
                }
                RenderService.Present();
                Thread.Sleep(_random.Next(100, 200)); // Slower flashes
            }
            // No RenderService.ClearFrameBuffer() here, as it will be cleared at the start of the main loop.

            // --- Corrupted Loading Sequence ---
            int labelIndex = 0;

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

                // --- UNSETTLING BACKGROUND PATTERN (from UiFunctions.cs) ---
                // Top pattern - dynamically changing characters and faster flicker
                string topPatternChar = (_random.Next(2) == 0) ? "💀" : "⸸";
                if (_random.Next(0, 5) == 0) topPatternChar = ((char)_random.Next(33, 126)).ToString(); // Random ASCII for glitch
                string topPatternLine = new string(topPatternChar[0], 30);
                RenderService.DrawCentered(DRAW_HEIGHT / 2 - 5, topPatternLine, scaryColors[_random.Next(scaryColors.Length)]);

                // Bottom pattern - dynamically changing characters and faster flicker
                string bottomPatternChar = (_random.Next(2) == 0) ? "☣" : "☢";
                if (_random.Next(0, 5) == 0) bottomPatternChar = ((char)_random.Next(33, 126)).ToString(); // Random ASCII for glitch
                string bottomPatternLine = new string(bottomPatternChar[0], 30);
                RenderService.DrawCentered(DRAW_HEIGHT / 2 + 3, bottomPatternLine, scaryColors[_random.Next(scaryColors.Length)]);


                // Update label sporadically (from UiFunctions.cs)
                if (i == 0 || _random.Next(0, 10) == 0) // Change label at start and more frequently
                {
                    labelIndex = _random.Next(0, scaryLabels.Length);
                    string currentLabel = scaryLabels[labelIndex];
                    string labelColor = scaryColors[_random.Next(scaryColors.Length)];
                    RenderService.DrawCentered(DRAW_HEIGHT / 2 - 2, currentLabel, labelColor);
                }

                // Progress bar (from UiFunctions.cs)
                int barWidth = 50; // Re-declare as local variable to match previous logic
                int barLeft = (DRAW_WIDTH / 2) - (barWidth / 2); // Centered horizontally
                int barY = DRAW_HEIGHT / 2; // Vertical center

                int progressBlocks = (int)((i / 100.0) * barWidth);
                string filled = new string('█', progressBlocks);
                string empty = new string('░', barWidth - progressBlocks);

                RenderService.WriteToBuffer(barLeft, barY, filled, INDIGO); // Use INDIGO as per original prompt
                RenderService.WriteToBuffer(barLeft + filled.Length, barY, empty, GRAY); // Use GRAY for empty bar

                RenderService.WriteToBuffer(barLeft + barWidth + 2, barY, string.Format("{0}%", i), WHITE); // Percentage


                // Erratic speed variation (from UiFunctions.cs)
                if (i < 20) Thread.Sleep(_random.Next(50, 100));
                else if (i < 70) Thread.Sleep(_random.Next(10, 30));
                else Thread.Sleep(_random.Next(40, 80));

                // Small chance of a "glitch jump" in progress (from UiFunctions.cs)
                if (_random.Next(0, 50) == 0 && i < 90)
                {
                    i += _random.Next(5, 15); // Jump forward
                    if (i > 100) i = 100;
                }

                RenderService.Present(); // Push the buffer to the screen
            }

            // --- Final Unsettling Message (from UiFunctions.cs) ---
            RenderService.ClearFrameBuffer(); // Clear one last time for the final message
            string finalMessage = "T̷H̷E̵ ̸G̷A̷T̷E̵S̷ ̷A̶R̸E̵ ̷O̷P̷E̶N̸...".Pastel(VEIN_RED);
            RenderService.DrawCentered(DRAW_HEIGHT / 2, finalMessage, VEIN_RED);
            RenderService.Present();
            Thread.Sleep(1500); // Hold the message

            // Cleanup
            Console.CursorVisible = true;
        }
    }
}