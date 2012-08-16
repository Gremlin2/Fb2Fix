using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace FB2Fix.CommandLine
{
    public static class NativeMethods
    {
        private const int STD_OUTPUT_HANDLE = -11;
		private static bool isRunningOnMono;
		
		static NativeMethods()
		{
			isRunningOnMono = (Type.GetType ("Mono.Runtime") != null);
		}

        private struct COORD
        {
            internal short x;
            internal short y;

            private COORD(short x, short y)
            {
                this.x = x;
                this.y = y;
            }
        }

        private struct SMALL_RECT
        {
            internal short Left;
            internal short Top;
            internal short Right;
            internal short Bottom;

            private SMALL_RECT(short Left, short Top, short Right, short Bottom)
            {
                this.Left = Left;
                this.Top = Top;
                this.Right = Right;
                this.Bottom = Bottom;
            }
        }

        private struct CONSOLE_SCREEN_BUFFER_INFO
        {
            internal COORD dwSize;
            internal COORD dwCursorPosition;
            internal short wAttributes;
            internal SMALL_RECT srWindow;
            internal COORD dwMaximumWindowSize;
        }

        [DllImport("kernel32.dll", EntryPoint="GetStdHandle", SetLastError=true, CharSet=CharSet.Auto, CallingConvention=CallingConvention.StdCall)]
        private static extern int GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll", EntryPoint="GetConsoleScreenBufferInfo", SetLastError=true, CharSet=CharSet.Auto, CallingConvention=CallingConvention.StdCall)]
        private static extern int GetConsoleScreenBufferInfo(int hConsoleOutput, ref CONSOLE_SCREEN_BUFFER_INFO lpConsoleScreenBufferInfo);

        /// <summary>
        /// Returns the number of columns in the current console window
        /// </summary>
        /// <returns>Returns the number of columns in the current console window</returns>
        public static int GetConsoleWindowWidth()
        {
            int screenWidth;
            CONSOLE_SCREEN_BUFFER_INFO csbi = new CONSOLE_SCREEN_BUFFER_INFO();

            int rc;
            rc = GetConsoleScreenBufferInfo(GetStdHandle(STD_OUTPUT_HANDLE), ref csbi);
            screenWidth = csbi.dwSize.x;
            return screenWidth;
        }

        public static bool IsRunningOnWindows
        {
            get
            {
                // check for non-Unix platforms - see FAQ for more details
                // http://www.mono-project.com/FAQ:_Technical#How_to_detect_the_execution_platform_.3F
                int platform = (int) Environment.OSVersion.Platform;

                return ((platform != 4) && (platform != 128));
            }
        }
        
        public static bool IsRunningOnMono
        {
            get
            {
                return isRunningOnMono;
            }
        }
    }
}
