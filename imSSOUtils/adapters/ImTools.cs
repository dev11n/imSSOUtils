using ImGuiNET;

namespace imSSOUtils.adapters
{
    /// <summary>
    /// Easier way to use certain ImGui features.
    /// </summary>
    internal struct ImTools
    {
        /// <summary>
        /// Starts a tooltip when hovering over a control.
        /// <para>Place this under the control you want the tooltip to display for.</para>
        /// </summary>
        /// <param name="text">The text to be displayed.</param>
        public static void ApplyTooltip(string text)
        {
            if (!ImGui.IsItemHovered()) return;
            ImGui.BeginTooltip();
            ImGui.SetTooltip(text);
            ImGui.EndTooltip();
        }

        /// <summary>
        /// Centre text.
        /// </summary>
        /// <param name="text">The text.</param>
        public static void CentreText(string text)
        {
            float windowWidth = ImGui.GetWindowSize().X, textWidth = ImGui.CalcTextSize(text).X;
            ImGui.SetCursorPosX((windowWidth - textWidth) * .5f);
            ImGui.Text(text);
        }
    }
}