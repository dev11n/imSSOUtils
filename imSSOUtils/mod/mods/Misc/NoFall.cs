namespace imSSOUtils.mod.mods.Misc
{
    /// <summary>
    /// NoFall for the horse.
    /// </summary>
    internal class NoFall : IMod
    {
        /// <summary>
        /// Execute the mod.
        /// </summary>
        protected internal override void on_trigger() => alpine_execute("CurrentHorse::SetHorseMaxFallTime(9999);");

        /// <summary>
        /// Called when the instance has been created.
        /// </summary>
        protected internal override void on_initialize() { }
    }
}