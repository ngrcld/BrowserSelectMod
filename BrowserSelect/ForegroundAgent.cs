﻿using System;
using System.Runtime.InteropServices;

namespace BrowserSelectMod
{
    //=============================================================================================================
    // acquired from:
    // http://stackoverflow.com/a/11065126/1461004
    // and
    // http://stackoverflow.com/q/9099479/1461004
    class ForegroundAgent
    //=============================================================================================================
    {
        private const int SW_RESTORE = 9;
        private const int GWL_STYLE = -16;
        private const UInt32 WS_MAXIMIZE = 0x1000000;

        //-------------------------------------------------------------------------------------------------------------
        [DllImport("user32.dll")]
        private static extern int SetForegroundWindow(IntPtr hWnd);
        //-------------------------------------------------------------------------------------------------------------


        //-------------------------------------------------------------------------------------------------------------
        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        //-------------------------------------------------------------------------------------------------------------

        //-------------------------------------------------------------------------------------------------------------
        [DllImport("user32.dll", SetLastError = true)]
        static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        //-------------------------------------------------------------------------------------------------------------

        //-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// sets window state of given hWnd to Restore if it is not maximized
        /// also gives said window focus (setForegroundWindow)
        /// </summary>
        /// <param name="hwnd">hWnd of window to be restored</param>
        public static void RestoreWindow(int hwnd)
        //-------------------------------------------------------------------------------------------------------------
        {

            var hWnd = new IntPtr(hwnd);
            int style = GetWindowLong(hWnd, GWL_STYLE);

            if ((style & WS_MAXIMIZE) != WS_MAXIMIZE)
            {
                ShowWindow(hWnd, SW_RESTORE);
            }

            SetForegroundWindow(hWnd);
        }
    }
}
