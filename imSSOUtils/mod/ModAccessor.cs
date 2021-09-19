namespace imSSOUtils.mod
{
    /// <summary>
    /// Used for displaying mods easier.
    /// </summary>
    internal readonly struct ModAccessor
    {
        /// <summary>
        /// The name of the mod.
        /// </summary>
        public readonly string name;

        /// <summary>
        /// The mod itself.
        /// </summary>
        public readonly IMod mod;

        /// <summary>
        /// Creates a new instance of <see cref="ModAccessor"/>.
        /// </summary>
        public ModAccessor(string name, IMod mod)
        {
            this.name = name;
            this.mod = mod;
        }
    }
}