namespace imSSOUtils.mod.mods.Misc
{
    /// <summary>
    /// Speeds up the game tick-speed.
    /// </summary>
    internal class GlobalSpeed : IMod
    {
        /// <summary>
        /// Execute the mod.
        /// </summary>
        protected internal override void on_trigger() => alpine_execute("->World::SetWorldTimeMul(1::5);");

        /// <summary>
        /// Called when the instance has been created.
        /// </summary>
        protected internal override void on_initialize() => add_button("Reset", "->World::SetWorldTimeMul(1);");
    }
}