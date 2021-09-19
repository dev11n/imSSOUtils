using System.Numerics;
using ImGuiNET;
using imSSOUtils.adapters;
using imSSOUtils.adapters.low_level;

namespace imSSOUtils.window.windows
{
    /// <summary>
    /// Displays additional information in the top corner.
    /// </summary>
    internal class ExtraInfoWindow : IWindow
    {
        /// <summary>
        /// Draw the window.
        /// </summary>
        protected internal override void draw()
        {
            if (!Program.showExtraInfo || !MemoryAdapter.is_enabled()) return;
            ImGui.SetNextWindowPos(Vector2.Zero);
            ImGui.SetNextWindowBgAlpha(.9f);
            ImGui.Begin(
                identifier,
                ImGuiWindowFlags.NoInputs |
                ImGuiWindowFlags.NoCollapse |
                ImGuiWindowFlags.NoTitleBar |
                ImGuiWindowFlags.AlwaysAutoResize |
                ImGuiWindowFlags.NoResize);
            ImGui.Text(identifier);
            var pos = PXInternal.get_horse_position();
            ImGui.Text(
                $"Current Horse Position: {pos.X}, {pos.Y}, {pos.Z}");
            if (CVar.hasCached01)
                ImGui.Text(CVar.read_cvar_string());

            ImGui.Text("Modding state: Enabled (alpine_v2_exp)");
            ImGui.End();
        }

        protected internal override void initialize() => identifier = "SSOUtils - Information";
    }
}