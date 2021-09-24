using System.Globalization;
using imSSOUtils.adapters;

namespace imSSOUtils.cache.visual
{
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
    }
}