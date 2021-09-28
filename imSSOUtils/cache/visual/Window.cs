using imSSOUtils.adapters;

namespace imSSOUtils.cache.visual
{
    /// <summary>
    /// Dynamically customise certain aspects of a window
    /// </summary>
    internal readonly struct Window
    {
        /// <summary>
        /// Show a generic text window.
        /// <para>Note: certain characters like "," doesn't display</para>
        /// </summary>
        /// <param name="headline"></param>
        /// <param name="text"></param>
        public static void show_generic_window(string headline, string text) => MemoryAdapter.direct_call(
            $"\nGame->GenericRequestWindow2->Headline::SetViewText(\"{headline}\");\n" +
            $"\nGame->GenericRequestWindow2->Message::SetViewText(\"{text}\");\n" +
            "Game->GenericRequestWindow2::Start();\n" +
            "Game->ReportWindow::SetScaleX(0);");
    }
}