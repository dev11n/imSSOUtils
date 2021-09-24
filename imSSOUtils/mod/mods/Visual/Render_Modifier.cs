using System.Threading;
using imSSOUtils.adapters;

namespace imSSOUtils.mod.mods.Visual
{
    internal class Render_Modifier : IMod
    {
        #region Variables
        /// <summary>
        /// Timer used for updating the ambience.
        /// </summary>
        private Timer updateTimer;
        #endregion

        /// <summary>
        /// Execute the mod.
        /// </summary>
        protected internal override void on_trigger()
        {
            var value = get_checkbox_value("Realtime_View");
            if (value)
            {
                updateTimer?.Dispose();
                updateTimer = new Timer(_ => update_ambience(), null, 0, 150);
            }
            else update_ambience();
        }

        /// <summary>
        /// Updates the ambience (sky, fog, etc)
        /// </summary>
        private void update_ambience()
        {
            if (!get_checkbox_value("Realtime_View")) updateTimer?.Dispose();
            const string cR = "floatSlider_Colour_R",
                cG = "floatSlider_Colour_G",
                cB = "floatSlider_Colour_B",
                cA = "floatSlider_Colour_R",
                hR = "floatSlider_Horizon_R",
                hG = "floatSlider_Horizon_G",
                hB = "floatSlider_Horizon_B",
                hA = "floatSlider_Horizon_A",
                cMix = "floatSlider_Colour_Mix",
                fMix = "floatSlider_Fog_Mix",
                fNear = "floatSlider_Fog_Near",
                fFar = "floatSlider_Fog_Far",
                useFilters = "checkbox_Use_Filters",
                filterIndex = "intSlider_Index",
                filterFade = "floatSlider_Fade";
            MemoryAdapter.direct_call($"Game->Sky::SetSkyModifierSkyColor({cR},{cG},{cB},{cA});\n" +
                                      $"Game->Sky::SetSkyModifierHorisonColor({hR},{hG},{hB},{hA});\n" +
                                      $"Game->Sky::SetSkyModifierColorMix({cMix});\n" +
                                      $"Game->Sky::SetSkyModifierFogMix({fMix});\n" +
                                      $"Game->Sky::SetSkyModifierFogNear({fNear});\n" +
                                      $"Game->Sky::SetSkyModifierFogFar({fFar});\n" +
                                      "Game->Sky::StartSkyModifier(10::0f);\n" +
                                      $"Game->PostEffectHandler::SetEnableLUT({useFilters});\n" +
                                      $"Game->PostEffectHandler::SetFilter({filterIndex});\n" +
                                      $"Game->PostEffectHandler::SetFilterFade({filterFade});\n");
        }

        /// <summary>
        /// Initialize options.
        /// </summary>
        protected internal override void on_initialize()
        {
            // ? Checkboxes
            add_checkbox("Realtime_View");
            // ? Text
            add_centre_text("SKY AND FOG");
            // ? Sky Colour RGBA
            add_float_slider("Colour_R", 0, 1);
            add_float_slider("Colour_G", 0, 1);
            add_float_slider("Colour_B", 0, 1);
            add_float_slider("Colour_A", 0, 1);
            // ? Sky Horizon RGBA
            add_float_slider("Horizon_R", 0, 1);
            add_float_slider("Horizon_G", 0, 1);
            add_float_slider("Horizon_B", 0, 1);
            add_float_slider("Horizon_A", 0, 1);
            // ? Sky Colour Mix
            add_float_slider("Colour_Mix", 0, 1);
            // ? Fog
            add_float_slider("Fog_Mix", 0, 1);
            add_float_slider("Fog_Near", -1500, 1500);
            add_float_slider("Fog_Far", -1500, 1500);
            // ? Text
            add_centre_text("FILTERS");
            // ? Checkboxes
            add_checkbox("Use_Filters");
            // ? Filter Index
            add_int_slider("Index", 0, 10);
            // ? Float Sliders
            add_float_slider("Fade", 0, 1);
        }
    }
}