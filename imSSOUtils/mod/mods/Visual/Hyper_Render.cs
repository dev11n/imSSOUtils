using System.Diagnostics;
using imSSOUtils.cache.visual;
using imSSOUtils.window.windows;

namespace imSSOUtils.mod.mods.Visual
{
    internal class Hyper_Render : IMod
    {
        protected internal override void on_trigger()
        {
            // WIP
            if (!Debugger.IsAttached)
            {
                Text.show_white_message("Hyper Render isn't ready yet!");
                return;
            }

            Viewport.toggle_filters(true);
            Viewport.set_filter(10);
            Viewport.set_intensity(.54f);
            ExtraInfoWindow.log("[debug] hyper_render -> on_trigger");
            Window.show_generic_window("Hyper Render",
                "Hyper Render is now activated. Please keep in mind that this is just a test!");
        }

        protected internal override void on_initialize() { }
    }
}