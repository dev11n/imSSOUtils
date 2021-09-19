using System;
using System.Drawing;
using System.Numerics;
using System.Runtime.InteropServices;

namespace imClickable
{
    /// <summary>
    /// This class allow user to access Win32 API functions.
    /// </summary>
    public static class NativeMethods
    {
        private const int GWL_EXSTYLE = -20, WS_EX_LAYERED = 0x80000, WS_EX_TRANSPARENT = 0x20;

        private const int SW_HIDE = 0, SW_SHOW = 5;

        private static IntPtr GWL_EXSTYLE_CLICKABLE = IntPtr.Zero, GWL_EXSTYLE_NOT_CLICKABLE = IntPtr.Zero;

        private const int KEY_PRESSED = 0x8000;

        /// <summary>
        /// Gets a value indicating whether the overlay is clickable or not.
        /// </summary>
        internal static bool IsClickable { get; private set; } = true;

        /// <summary>
        /// Allows the SDL2Window to become transparent.
        /// </summary>
        /// <param name="handle">
        /// Veldrid window handle in IntPtr format.
        /// </param>
        internal static void InitTransparency(IntPtr handle)
        {
            GWL_EXSTYLE_CLICKABLE = GetWindowLongPtr(handle, GWL_EXSTYLE);
            GWL_EXSTYLE_NOT_CLICKABLE = new IntPtr(GWL_EXSTYLE_CLICKABLE.ToInt64() | WS_EX_LAYERED | WS_EX_TRANSPARENT);
            var margins = Margins.FromRectangle(new Rectangle(-1, -1, -1, -1));
            DwmExtendFrameIntoClientArea(handle, ref margins);
        }

        /// <summary>
        /// Enables (clickable) / Disables (not clickable) the SDL2Window keyboard/mouse inputs.
        /// NOTE: This function depends on InitTransparency being called when the SDL2Winhdow was created.
        /// </summary>
        /// <param name="handle">Veldrid window handle in IntPtr format.</param>
        /// <param name="WantClickable">Set to true if you want to make the window clickable otherwise false.</param>
        internal static void SetOverlayClickable(IntPtr handle, bool WantClickable)
        {
            switch (IsClickable)
            {
                case false when WantClickable:
                    SetWindowLongPtr(handle, GWL_EXSTYLE, GWL_EXSTYLE_CLICKABLE);
                    SetFocus(handle);
                    IsClickable = true;
                    break;
                case true when !WantClickable:
                    SetWindowLongPtr(handle, GWL_EXSTYLE, GWL_EXSTYLE_NOT_CLICKABLE);
                    IsClickable = false;
                    break;
            }
        }

        /// <summary>
        /// Allows showing/hiding the console window.
        /// </summary>
        internal static void SetConsoleWindow(bool visible)
        {
            var handle = GetConsoleWindow();
            ShowWindow(handle, visible ? SW_SHOW : SW_HIDE);
        }

        /// <summary>
        /// Returns the current mouse position w.r.t the window in Vector2 format.
        /// Also, returns Zero in case of any errors.
        /// </summary>
        /// <param name="hWnd"></param>
        /// <returns></returns>
        internal static Vector2 GetCursorPosition(IntPtr hWnd)
        {
            if (!GetCursorPos(out var lpPoint)) return Vector2.Zero;
            ScreenToClient(hWnd, ref lpPoint);
            return lpPoint;
        }

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetCursorPos(out POINT lpPoint);

        [DllImport("user32.dll")]
        private static extern bool ScreenToClient(IntPtr hWnd, ref POINT lpPoint);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr SetFocus(IntPtr hWnd);

        [DllImport("dwmapi.dll")]
        private static extern int DwmExtendFrameIntoClientArea(IntPtr hWnd, ref Margins pMarInset);

        [DllImport("user32.dll", EntryPoint = "GetWindowLongPtr")]
        private static extern IntPtr GetWindowLongPtr(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr")]
        private static extern IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll", EntryPoint = "ShowWindow", SetLastError = true)]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [StructLayout(LayoutKind.Sequential)]
        private struct Margins
        {
            private int left, right, top, bottom;

            public static Margins FromRectangle(Rectangle rectangle)
            {
                var margins = new Margins
                {
                    left = rectangle.Left,
                    right = rectangle.Right,
                    top = rectangle.Top,
                    bottom = rectangle.Bottom,
                };
                return margins;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        internal readonly struct POINT
        {
            public static implicit operator Vector2(POINT p) => new(p.X, p.Y);
            public static implicit operator Point(POINT p) => new(p.X, p.Y);
            private readonly int X, Y;
        }
    }
}