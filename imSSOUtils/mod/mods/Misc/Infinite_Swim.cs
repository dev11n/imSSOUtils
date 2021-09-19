using imSSOUtils.adapters;

namespace imSSOUtils.mod.mods.Misc
{
    /// <summary>
    /// Kills off the swimming time limit.
    /// </summary>
    internal class Infinite_Swim : IMod
    {
        /// <summary>
        /// Execute the mod.
        /// </summary>
        protected internal override void on_trigger()
        {
            alpine_execute("Game->SwimActionBar::ForceKill();");
            PXInternal.show_white_message("Swimming limit has been removed. Have fun!");
        }

        /// <summary>
        /// Called when the instance has been created.
        /// </summary>
        protected internal override void on_initialize() { }
    }
}