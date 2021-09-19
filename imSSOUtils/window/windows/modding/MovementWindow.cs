using imSSOUtils.adapters;

namespace imSSOUtils.window.windows.modding
{
    /// <summary>
    /// All pre-made movement mods.
    /// </summary>
    internal class MovementWindow : IWindow
    {
        /// <summary>
        /// Draw the window.
        /// </summary>
        protected internal override void draw() => ModWindowDrawer.begin(identifier, ref shouldDisplay);

        protected internal override void initialize() => identifier = "Movement";
    }
}