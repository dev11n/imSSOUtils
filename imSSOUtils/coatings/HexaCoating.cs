using System.Numerics;
using DriverProgram.adapters;
using ImGuiNET;

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
            black = ColourAdapter.rgba_to_frgba(0, 0, 0, 0);

        /// <summary>
        /// Max alpha for colours.
        /// </summary>
        private const float max_alpha = 255;
        #endregion

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
            colours[(int) ImGuiCol.TitleBg] = orange;
            colours[(int) ImGuiCol.TitleBgActive] = orange;
            colours[(int) ImGuiCol.TitleBgCollapsed] = ColourAdapter.rgba_to_frgba(0, 0, 0, 130);
            // ? Buttons
            colours[(int) ImGuiCol.Button] = gray;
            colours[(int) ImGuiCol.ButtonActive] = gray;
            colours[(int) ImGuiCol.ButtonHovered] = ColourAdapter.rgba_to_frgba(41, 40, 41, 255);
            // ? Separator
            colours[(int) ImGuiCol.Separator] = light_gray;
            colours[(int) ImGuiCol.SeparatorActive] = light_gray;
            colours[(int) ImGuiCol.SeparatorHovered] = ColourAdapter.rgba_to_frgba(76, 76, 76, 255);
            // ? Frame Background
            colours[(int) ImGuiCol.FrameBg] = gray_alt;
            colours[(int) ImGuiCol.FrameBgActive] = gray_alt;
            colours[(int) ImGuiCol.FrameBgHovered] = ColourAdapter.rgba_to_frgba(37, 36, 37, 255);
            // ? Header
            colours[(int) ImGuiCol.Header] = black;
            colours[(int) ImGuiCol.HeaderActive] = black;
            colours[(int) ImGuiCol.HeaderHovered] = ColourAdapter.rgba_to_frgba(46, 46, 46, 255);
            // ? Checkbox
            colours[(int) ImGuiCol.CheckMark] = ColourAdapter.rgba_to_frgba(255, 101, 53, 255);
            // ? Tabs
            colours[(int) ImGuiCol.Tab] = orange;
            colours[(int) ImGuiCol.TabActive] = orange;
            colours[(int) ImGuiCol.TabHovered] = orange_alt;
            // ? Sliders
            colours[(int) ImGuiCol.SliderGrab] = orange;
            colours[(int) ImGuiCol.SliderGrabActive] = orange_alt;
        }
    }
}