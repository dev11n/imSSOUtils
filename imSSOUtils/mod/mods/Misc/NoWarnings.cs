namespace imSSOUtils.mod.mods.Misc
{
    /// <summary>
    /// Erases all closed area warnings and most invisible collision walls.
    /// </summary>
    internal class NoWarnings : IMod
    {
        /// <summary>
        /// Execute the mod.
        /// </summary>
        protected internal override void on_trigger() => alpine_execute(
            "Game->ForcePlayerFromRestrictedArea::ForceKill();\nGame->ForcePlayerBackToMoorland::ForceKill();\nGSpace->WorldBlockCollsion::ForceKill();");

        /// <summary>
        /// Called when the instance has been created.
        /// </summary>
        protected internal override void on_initialize() { }
    }
}