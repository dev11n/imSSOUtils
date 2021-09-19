namespace imSSOUtils.mod.mods.Movement
{
    /// <summary>
    /// Basic physics flight mod for the horse.
    /// </summary>
    internal class Flight : IMod
    {
        /// <summary>
        /// Execute the mod.
        /// </summary>
        protected internal override void on_trigger() =>
            alpine_execute("CurrentHorse::SetRelativeLinearVelocity(0, 7, 45);");

        /// <summary>
        /// Called when the instance has been created.
        /// </summary>
        protected internal override void on_initialize() { }
    }
}