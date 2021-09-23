using System.Numerics;
using ImGuiNET;
using imSSOUtils.adapters;
using static ImGuiNET.ImGuiCol;

namespace imSSOUtils.coatings
{
    /// <summary>
    /// A modern coating - WIP
    /// </summary>
    internal struct ModernCoating
    {
        #region Variables
        private static readonly Vector4 transparent_inactive = ColourAdapter.rgba_to_frgba(0, 0, 0, .65f),
            transparent_active = ColourAdapter.rgba_to_frgba(0, 0, 0, .8f),
            transparent_collapsed = ColourAdapter.rgba_to_frgba(0, 0, 0, .95f),
            transparent_alt = ColourAdapter.rgba_to_frgba(27, 27, 27, .95f);

        /// <summary>
        /// Max alpha for colours.
        /// </summary>
        private const float max_alpha = 1;
        #endregion

        /// <summary>
        /// "Plug" / Enable the coating.
        /// </summary>
        public static void plug()
        {
            var style = ImGui.GetStyle();
            var colours = style.Colors;

            // ? Set the border colour
            colours[(int) Border] = ColourAdapter.rgba_to_frgba(0, 255, 214, 1);
            style.WindowBorderSize = 1;

            // ? Align the title to the middle
            const float middle = .5f;
            style.WindowTitleAlign = new Vector2(middle, middle);

            // ? Frame Size
            style.FramePadding = new Vector2(8, 6);

            // ? Title
            colours[(int) TitleBg] = transparent_inactive;
            colours[(int) TitleBgActive] = transparent_active;
            colours[(int) TitleBgCollapsed] = transparent_collapsed;
            // ? Buttons
            colours[(int) Button] = transparent_inactive;
            colours[(int) ButtonActive] = transparent_active;
            colours[(int) ButtonHovered] = transparent_alt;
            // ? Separator
            colours[(int) Separator] = transparent_inactive;
            colours[(int) SeparatorActive] = transparent_active;
            colours[(int) SeparatorHovered] = transparent_collapsed;
            // ? Frame Background
            colours[(int) FrameBg] = transparent_inactive;
            colours[(int) FrameBgActive] = transparent_active;
            colours[(int) FrameBgHovered] = transparent_collapsed;
            // ? Header
            colours[(int) Header] = transparent_inactive;
            colours[(int) HeaderActive] = transparent_active;
            colours[(int) HeaderHovered] = transparent_collapsed;
            // ? Checkbox
            colours[(int) CheckMark] = transparent_collapsed;
            // ? Tabs
            colours[(int) Tab] = transparent_inactive;
            colours[(int) TabActive] = transparent_active;
            colours[(int) TabHovered] = transparent_collapsed;
            // ? Sliders
            colours[(int) SliderGrab] = transparent_alt;
            colours[(int) SliderGrabActive] = transparent_alt;
        }
    }
}