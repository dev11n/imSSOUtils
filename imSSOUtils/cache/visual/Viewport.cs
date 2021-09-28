using System.Globalization;
using imSSOUtils.adapters;

namespace imSSOUtils.cache.visual
{
    /// <summary>
    /// Viewport-based functions
    /// </summary>
    internal readonly struct Viewport
    {
        /// <summary>
        /// Toggle filters (LUT) on and off.
        /// </summary>
        /// <param name="enabled"></param>
        public static void toggle_filters(bool enabled) =>
            MemoryAdapter.direct_call($"Game->PostEffectHandler::SetEnableLUT({(enabled ? 1 : 0)});");

        /// <summary>
        /// Changes the current filter intensity.
        /// </summary>
        /// <param name="intensity">The intensity (max 1)</param>
        public static void set_intensity(float intensity) =>
            MemoryAdapter.direct_call(
                $"Game->PostEffectHandler::SetFilterFade({intensity.ToString(CultureInfo.InvariantCulture).Replace(".", "::")});");

        /// <summary>
        /// Changes the filter.
        /// </summary>
        /// <param name="index">The filter index</param>
        public static void set_filter(int index) =>
            MemoryAdapter.direct_call($"Game->PostEffectHandler::SetFilter({index});");

        /// <summary>
        /// Draws the current location.
        /// </summary>
        /// <returns></returns>
        public static void draw_current_location()
        {
            const string code =
                "if (Game->GlobalTempStringData::GetDataString() != Game->LocationNameMiniMap::GetViewText()) >>\n" +
                "Game->InfoTextWindow3::SetViewText(Game->LocationNameMiniMap::GetViewText());\n" +
                "Game->InfoTextWindow3::SetViewTextColor(1, 1, 1, 1);\n" + // Float RGBA: White
                "Game->InfoTextWindow3::Start();\n<<\n" +
                "Game->GlobalTempStringData::SetDataString(Game->LocationNameMiniMap::GetViewText());";
            MemoryAdapter.direct_call(code);
        }
    }
}