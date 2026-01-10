using System;
using System.Runtime.InteropServices;

namespace DungeonAdventures.csproj.Src.Utilities.UI
{
    public static class ConsoleManager
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool SetConsoleWindowInfo(IntPtr hConsoleOutput, bool bAbsolute, ref SMALL_RECT lpConsoleWindow);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool SetConsoleScreenBufferSize(IntPtr hConsoleOutput, COORD dwSize);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool SetCurrentConsoleFontEx(IntPtr consoleOutput, bool maximumWindow, ref CONSOLE_FONT_INFO_EX consoleFont);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool GetCurrentConsoleFontEx(IntPtr consoleOutput, bool maximumWindow, ref CONSOLE_FONT_INFO_EX consoleFont);

        private const int STD_OUTPUT_HANDLE = -11;
        private const uint ENABLE_VIRTUAL_TERMINAL_PROCESSING = 0x0004;

        [StructLayout(LayoutKind.Sequential)]
        public struct COORD
        {
            public short X;
            public short Y;

            public COORD(short X, short Y)
            {
                this.X = X;
                this.Y = Y;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SMALL_RECT
        {
            public short Left;
            public short Top;
            public short Right;
            public short Bottom;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct CONSOLE_FONT_INFO_EX
        {
            public uint cbSize;
            public uint nFont;
            public COORD dwFontSize;
            public int FontFamily;
            public int FontWeight;
            public char FaceName; // Should be fixed-size array of 32 chars
        }

        public static void SetupConsole(int width, int height)
        {
            IntPtr handle = GetStdHandle(STD_OUTPUT_HANDLE);
            if (handle == IntPtr.Zero)
            {
                return;
            }

            // Set console mode for virtual terminal processing (enables ANSI escape codes)
            uint mode;
            if (GetConsoleMode(handle, out mode))
            {
                mode |= ENABLE_VIRTUAL_TERMINAL_PROCESSING;
                SetConsoleMode(handle, mode);
            }

            // Set window size
            SMALL_RECT rect = new SMALL_RECT { Left = 0, Top = 0, Right = (short)(width - 1), Bottom = (short)(height - 1) };
            SetConsoleWindowInfo(handle, true, ref rect);

            // Set buffer size
            COORD bufferSize = new COORD((short)width, (short)height);
            SetConsoleScreenBufferSize(handle, bufferSize);

            // Try setting window size again after buffer size is set
            SetConsoleWindowInfo(handle, true, ref rect);
            
            // Attempt to get current font info to preserve other settings, then update size
            CONSOLE_FONT_INFO_EX fontInfo = new CONSOLE_FONT_INFO_EX();
            fontInfo.cbSize = (uint)Marshal.SizeOf(fontInfo);
            
            // Get current font info (this can sometimes fail, so we handle it)
            if (GetCurrentConsoleFontEx(handle, false, ref fontInfo))
            {
                // Set the font size to make characters fit within the window, assuming default font
                // A common default font width for an 85 character wide console would be 8 pixels.
                // A common default font height for a 25 character high console would be 16-20 pixels.
                // This is a heuristic and might need adjustment based on system defaults.
                fontInfo.dwFontSize = new COORD(8, 16); // Example: 8x16 font size for 85x25 (adjust as needed)
                SetCurrentConsoleFontEx(handle, false, ref fontInfo);
            }
            
            // Set window title for flavor
            Console.Title = "DUMGERAGN // ACTIVE";

            // Hide the cursor
            Console.CursorVisible = false;
        }

        public static void ClearConsoleBuffer()
        {
            // Do not use Console.Clear() as per Hostile Workflow: "Total Frame Reset"
            // This method would typically be responsible for iterating through the buffer
            // and overwriting characters, which will be handled by RenderService.
            // For now, we'll ensure cursor is at top-left for RenderService.
            Console.SetCursorPosition(0, 0);
        }
    }
}
