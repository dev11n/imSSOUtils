using System;
using System.Runtime.InteropServices;
using System.Text;

namespace imSSOUtils.adapters
{
    /// <summary>
    /// Windows API calls
    /// </summary>
    internal struct WinAPI
    {
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool GetWindowRect(IntPtr hWnd, ref Dimensions dim);

        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern int GetWindowText(IntPtr hwnd, StringBuilder ss, int count);

        #region Structures
        [StructLayout(LayoutKind.Sequential)]
        public struct Dimensions
        {
            public int left, top, right, bottom;
        }
        #endregion
    }
}