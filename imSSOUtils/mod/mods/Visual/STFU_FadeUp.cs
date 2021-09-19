using imSSOUtils.adapters;

namespace imSSOUtils.mod.mods.Visual
{
    /// <summary>
    /// Tries to kill off most fade up / fade down transitions.
    /// </summary>
    internal class STFU_FadeUp : IMod
    {
        #region Variables
        /// <summary>
        /// Most fading windows.
        /// </summary>
        private readonly string[] windows =
        {
            "Game->FadeDownFadeUp::Delete();", "Game->FadeUp::Delete();", "Game->FadeUpLong::Delete();",
            "Game->FadeDown::Delete();", "Game->FadeUpGreen::Delete();", "Game->FadeUpFastTransparent::Delete();",
            "Game->FadeToBlack::Delete();", "Game->FadeDownCatched::Delete();", "Game->FadeDownIntro::Delete();",
            "Game->FadeDawnUpIntro::Delete();", "Game->FadeDownOutro::Delete();", "Game->FadedScreen::Delete();",
            "Game->FadeDownGoToMainMenu::Delete();", "Game->FadeDownLongWithText::Delete();",
            "Game->FadeUpWithLaterText::Delete();",
            "Game->FadeDownFastFadeUp::Delete();", "Game->FadeDownFadeUp100::Delete();",
            "Game->FadeDownDisconnectedExit::Delete();"
        };
        #endregion

        /// <summary>
        /// Execute the mod.
        /// </summary>
        protected internal override void on_trigger()
        {
            for (var i = 0; i < windows.Length; i++)
            {
                alpine_execute(windows[i]);
                PXInternal.show_white_message($"{i + 1} / {windows.Length}");
            }

            PXInternal.show_white_message("Completed!");
        }

        /// <summary>
        /// Called when the instance has been created.
        /// </summary>
        protected internal override void on_initialize() { }
    }
}