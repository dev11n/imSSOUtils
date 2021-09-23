using System.Numerics;
using System.Text;
using ImGuiNET;
using imSSOUtils.adapters;
using static ImGuiNET.ImGuiWindowFlags;

namespace imSSOUtils.window.windows
{
    /// <summary>
    /// A window dedicated towards saving various things in mods that simply cannot access SSOUtils itself in form of a button.
    /// </summary>
    internal class SaveHubWindow : IWindow
    {
        #region Variables
        /// <summary>
        /// Sizes in <see cref="Vector2"/>.
        /// </summary>
        // Button: 253 + 20 is to match the window size relatively well.
        // Button: >> 1 is the same as / 2.
        private readonly Vector2 windowSize = new(270 + 20, 150), buttonSize = new(((253 + 20) >> 1) - 1, 25);

        /// <summary>
        /// A buffer that temporarily holds the current data of InputText for Sky Modifier.
        /// </summary>
        private readonly byte[] data = new byte[100];
        #endregion

        /// <summary>
        /// Draw the window.
        /// </summary>
        protected internal override void draw()
        {
            if (!ImGui.Begin(identifier, ref shouldDisplay, NoResize))
            {
                ImGui.End();
                return;
            }

            ImGui.SetWindowSize(windowSize);
            ImTools.CentreText("SAVE MANAGER - PREVIEW");
            ImGui.Separator();
            draw_sky_section();
        }

        /// <summary>
        /// Draws the save sky section.
        /// </summary>
        private void draw_sky_section()
        {
            if (!ImGui.CollapsingHeader("Sky Modifier")) return;
            ImGui.PushItemWidth(253 + 20);
            ImGui.InputText(string.Empty, data, 100);
            ImGui.PopItemWidth();
            if (ImGui.Button("Save", buttonSize))
                FileAdapter.create_sky_preset(Encoding.UTF8.GetString(data).Replace("\u0000", string.Empty));
            ImGui.SameLine(146.3f);
            if (ImGui.Button("Load", buttonSize))
                FileAdapter.load_sky_preset(Encoding.UTF8.GetString(data).Replace("\u0000", string.Empty));
        }

        protected internal override void initialize() => identifier = "Save Manager";
    }
}