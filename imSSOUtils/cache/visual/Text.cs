using imSSOUtils.adapters;

namespace imSSOUtils.cache.visual
{
    /// <summary>
    /// Display text messages on the screen
    /// </summary>
    internal readonly struct Text
    {
        /// <summary>
        /// Displays a basic middle-aligned text message with white colour.
        /// </summary>
        /// <param name="text"></param>
        public static void show_white_message(string text) => MemoryAdapter.direct_call(
            $"Game->GUI_RescueRanch_InfoText::SetViewText(\"{text}\");\n" +
            "Game->GUI_RescueRanch_InfoText::Start();\n" +
            "Game->GUI_RescueRanch_InfoText->Duration::Start();\n" +
            "Game->ReportWindow::SetScaleX(0);");
    }
}