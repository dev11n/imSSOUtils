using imSSOUtils.adapters;
using imSSOUtils.cache;
using imSSOUtils.window.windows;

namespace imSSOUtils.linker
{
    /// <summary>
    /// Holds the actions for several in-game events.
    /// </summary>
    internal readonly struct EventLinker
    {
        /// <summary>
        /// Called whenever the player has leveled up.
        /// </summary>
        public static void on_player_level_up()
        {
            // ? Log
            ExtraInfoWindow.log("player leveled up!");
            // ? Display a custom level-up message
            PXInternal.show_white_message($"Level {Player.level} reached!");
            // ? Update the player stats
            Player.update_currencies();
            Player.update_level();
            Player.update_skills();
        }
    }
}