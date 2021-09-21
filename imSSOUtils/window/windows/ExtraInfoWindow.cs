using System.Numerics;
using ImGuiNET;
using imSSOUtils.adapters;
using imSSOUtils.adapters.low_level;
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
        private readonly Vector2 pos = new(0, 200);
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
            ImGui.Text(identifier);
            var horsePos = PXInternal.get_horse_position();
            ImGui.Text(
                $"Current Horse Position: {horsePos.X}, {horsePos.Y}, {horsePos.Z}");
            if (CVar.hasCached01)
                ImGui.Text(CVar.read_cvar01_string());
            if (CVar.hasCached02)
                ImGui.Text(CVar.read_cvar02_string());

            ImGui.Text($"Modding state: {(MemoryAdapter.is_enabled() ? "Enabled" : "Corrupted")} (alpine_v2_exp)");
            ImGui.End();
        }

        protected internal override void initialize() => identifier = "SSOUtils - Information";
    }
}