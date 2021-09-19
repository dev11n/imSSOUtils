using imSSOUtils.adapters;

namespace imSSOUtils.window.windows.modding
{
    /// <summary>
    /// All pre-made visual mods.
    /// </summary>
    internal class VisualWindow : IWindow
    {
        /// <summary>
        /// Draw the window.
        /// </summary>
        protected internal override void draw() => ModWindowDrawer.begin(identifier, ref shouldDisplay);

        protected internal override void initialize() => identifier = "Visual";
    }
}