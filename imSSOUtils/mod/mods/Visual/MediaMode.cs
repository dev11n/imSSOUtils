namespace imSSOUtils.mod.mods.Visual
{
    /// <summary>
    /// A mod aimed for recording / streaming, works by spoofing certain game aspects.
    /// </summary>
    internal class MediaMode : IMod
    {
        /// <summary>
        /// Execute the mod.
        /// </summary>
        protected internal override void on_trigger() => alpine_execute(
            "CurrentPlayerName::SetDataString(\"inputText_Player_Name\");\nCurrentHorseName::SetDataString(\"inputText_Horse_Name\");");

        /// <summary>
        /// Called when the instance has been created.
        /// </summary>
        protected internal override void on_initialize()
        {
            add_input_text("Player_Name");
            add_input_text("Horse_Name");
        }
    }
}