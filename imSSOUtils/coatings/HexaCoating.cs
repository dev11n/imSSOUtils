using System.Numerics;
using ImGuiNET;
using imSSOUtils.adapters;
using static ImGuiNET.ImGuiCol;

namespace imSSOUtils.coatings
{
    /// <summary>
    /// An orange coating for SSOUtils.
    /// </summary>
    internal struct HexaCoating
    {
        #region Variables
        /// <summary>
        /// Frequently used colours.
        /// </summary>
        private static readonly Vector4 orange = ColourAdapter.rgba_to_frgba(255, 101, 53, max_alpha),
            orange_alt = ColourAdapter.rgba_to_frgba(255, 115, 71, 255),
            gray = ColourAdapter.rgba_to_frgba(31, 30, 31, max_alpha),
            light_gray = ColourAdapter.rgba_to_frgba(70, 70, 70, max_alpha),
            gray_alt = ColourAdapter.rgba_to_frgba(37, 36, 37, max_alpha),
            black = Vector4.Zero;

        /// <summary>
        /// Max alpha for colours.
        /// </summary>
        private const float max_alpha = 255;
        #endregion

        /// <summary>
        /// "Plug" / Enable the coating.
        /// </summary>
        public static void plug()
        {
            var style = ImGui.GetStyle();
            var colours = style.Colors;

            // ? Disable the border
            style.WindowBorderSize = 0;

            // ? Align the title to the middle
            const float middle = .5f;
            style.WindowTitleAlign = new Vector2(middle, middle);

            // ? Frame Size
            style.FramePadding = new Vector2(8, 6);

            // ? Title
            colours[(int) TitleBg] = orange;
            colours[(int) TitleBgActive] = orange;
            colours[(int) TitleBgCollapsed] = ColourAdapter.rgba_to_frgba(0, 0, 0, 130);
            // ? Buttons
            colours[(int) Button] = gray;
            colours[(int) ButtonActive] = gray;
            colours[(int) ButtonHovered] = ColourAdapter.rgba_to_frgba(41, 40, 41, 255);
            // ? Separator
            colours[(int) Separator] = light_gray;
            colours[(int) SeparatorActive] = light_gray;
            colours[(int) SeparatorHovered] = ColourAdapter.rgba_to_frgba(76, 76, 76, 255);
            // ? Frame Background
            colours[(int) FrameBg] = gray_alt;
            colours[(int) FrameBgActive] = gray_alt;
            colours[(int) FrameBgHovered] = ColourAdapter.rgba_to_frgba(37, 36, 37, 255);
            // ? Header
            colours[(int) Header] = black;
            colours[(int) HeaderActive] = black;
            colours[(int) HeaderHovered] = ColourAdapter.rgba_to_frgba(46, 46, 46, 255);
            // ? Checkbox
            colours[(int) CheckMark] = ColourAdapter.rgba_to_frgba(255, 101, 53, 255);
            // ? Tabs
            colours[(int) Tab] = orange;
            colours[(int) TabActive] = orange;
            colours[(int) TabHovered] = orange_alt;
            // ? Sliders
            colours[(int) SliderGrab] = orange;
            colours[(int) SliderGrabActive] = orange_alt;
        }
    }
}