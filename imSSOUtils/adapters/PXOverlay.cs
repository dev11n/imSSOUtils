using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using static imSSOUtils.adapters.ProcessAdapter;
using static imSSOUtils.adapters.WinAPI;
using static imSSOUtils.registers.BaseRegister;

namespace imSSOUtils.adapters
{
    /// <summary>
    /// Make sure the overlay stays only visible in-game.
    /// </summary>
    internal struct PXOverlay
    {
        #region Variables
        /// <summary>
        /// A timer for checking windows.
        /// </summary>
        public static Timer checkWindows;

        /// <summary>
        /// Last window title.
        /// </summary>
        private static string lastTitle;

        /// <summary>
        /// Indicates whether we are in SSO or not.
        /// </summary>
        public static bool inSSO;
        #endregion

        /// <summary>
        /// Get the active window title.
        /// </summary>
        private static string active_title()
        {
            const int nChar = 256;
            var ss = new StringBuilder(nChar);
            var handle = GetForegroundWindow();
            return GetWindowText(handle, ss, nChar) > 0 ? ss.ToString() : string.Empty;
        }

        /// <summary>
        /// Start checking for the foreground process.
        /// </summary>
        public static void begin_check()
        {
            if (Debugger.IsAttached)
            {
                inSSO = true;
                return;
            }

            checkWindows = new Timer(_ =>
            {
                if (!IsProcessAlive(runtime)) Environment.Exit(0);
                IntPtr currentHandle = GetForegroundWindow(), ssoHandle = GetProcess(runtime).MainWindowHandle;
                var currentTitle = active_title();
                inSSO = currentHandle == ssoHandle || lastTitle is runtime_title;
                if (currentTitle is not "Overlay") lastTitle = currentTitle;
            }, null, 0, 550);
        }
    }
}