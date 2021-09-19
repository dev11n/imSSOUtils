using imSSOUtils.adapters;

namespace imSSOUtils.window.windows.modding
{
    /// <summary>
    /// All pre-made misc mods.
    /// </summary>
    internal class MiscWindow : IWindow
    {
        /// <summary>
        /// Draw the window.
        /// </summary>
        protected internal override void draw() => ModWindowDrawer.begin(identifier, ref shouldDisplay);

        protected internal override void initialize() => identifier = "Misc";
    }
}