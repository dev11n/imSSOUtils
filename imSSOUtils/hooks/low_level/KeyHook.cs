using System.Collections.Generic;
using Coroutine;
using imClickable;
using imSSOUtils.adapters;
using imSSOUtils.adapters.low_level;
using imSSOUtils.mod.mods.Visual;
using imSSOUtils.window.windows.modding.low_level;

namespace imSSOUtils.hooks.low_level
{
    /// <summary>
    /// Basic key checking without WinShi.. Forms and WPF.
    /// </summary>
    internal readonly struct KeyHook
    {
        #region Variables
        /// <summary>
        /// The delay for unlocking key checks.
        /// </summary>
        private static readonly Wait resetDelay = new(5);

        /// <summary>
        /// Determines whether key checks have been locked or not.
        /// </summary>
        private static bool locked;
        #endregion

        /// <summary>
        /// Enable the checks.
        /// </summary>
        /// <returns></returns>
        public static IEnumerator<Wait> plug()
        {
            var wait = new Wait(Overlay.OnRender);
            var playerWindow = Program.get_by_name("Player") as LL_PlayerStatsWindow;
            // 0x43 = G
            while (true)
            {
                yield return wait;
                if (!NativeMethods.IsKeyPressed(0x43) || locked || !PXOverlay.inSSO ||
                    playerWindow.shouldDisplay || !UI_Enhancements.isRunning) continue;
                playerWindow.open_window();
                locked = true;
                CoroutineHandler.InvokeLater(resetDelay, () => locked = false);
            }
        }

        /// <summary>
        /// Resets the value of CVar_01.
        /// </summary>
        private static void reset_cvar() => CVar.write_cvar01(" ", "String");
    }
}