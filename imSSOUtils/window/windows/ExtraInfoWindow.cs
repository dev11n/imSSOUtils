using System.Numerics;
using imClickable;
using ImGuiNET;
using imSSOUtils.adapters;
using imSSOUtils.adapters.low_level;
using imSSOUtils.cores;
using static ImGuiNET.ImGuiWindowFlags;

namespace imSSOUtils.window.windows
{
    /// <summary>
    /// Displays additional information in the top corner.
    /// </summary>
    internal class ExtraInfoWindow : IWindow
    {
        #region Variables
        /// <summary>
        /// The window position.
        /// </summary>
        private readonly Vector2 pos = new(0, 150);

        /// <summary>
        /// The log text.
        /// </summary>
        private static string log_text = string.Empty;
        #endregion

        /// <summary>
        /// Draw the window.
        /// </summary>
        protected internal override void draw()
        {
            if (!Program.showExtraInfo || !MemoryAdapter.is_enabled()) return;
            ImGui.SetNextWindowPos(pos);
            ImGui.SetNextWindowBgAlpha(0);
            ImGui.Begin(identifier, NoInputs | NoCollapse | NoTitleBar | AlwaysAutoResize | NoResize);
            ImGui.PushFont(ImGuiController.inconsolata_Regular);
            ImGui.Text(identifier);
            var horsePos = PXInternal.get_horse_position();
            ImGui.Text($"Current Horse Position: {horsePos.X}, {horsePos.Y}, {horsePos.Z}");
            ImGui.Text(
                $"Modding state: {(MemoryAdapter.is_enabled() && CVar.hasCachedAll ? "Enabled" : "Corrupted")} (alpine_v2_exp)");
            //ImGui.Text(SpawnerCore.closestObject);
            if (log_text is {Length: >= 5})
            {
                ImGui.NewLine();
                ImGui.Text(log_text);
            }

            ImGui.PopFont();
            ImGui.End();
        }

        /// <summary>
        /// Write text to the side of the screen.
        /// </summary>
        /// <param name="text"></param>
        public static void log(string text) => log_text += $"{text}\n";

        protected internal override void initialize() => identifier = "SSOUtils - Information";
    }
}