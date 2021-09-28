using System.Diagnostics;
using System.Numerics;
using System.Text;
using ImGuiNET;
using imSSOUtils.adapters;
using static ImGuiNET.ImGuiWindowFlags;
using static imSSOUtils.cores.SpawnerCore;

namespace imSSOUtils.window.windows.modding.low_level
{
    /// <summary>
    /// Spawner Window.
    /// </summary>
    internal class LL_SpawnerWindow : IWindow
    {
        #region Variables
        /// <summary>
        /// A buffer that temporarily holds the current data of InputText.
        /// </summary>
        private static readonly byte[] data_ls = new byte[32], data_fo = new byte[128];

        /// <summary>
        /// The current object scale.
        /// </summary>
        private static Vector3 currentScale = Vector3.One;
        #endregion

        /// <summary>
        /// Draw all the controls.
        /// </summary>
        private void draw_controls()
        {
            // Regions to tidy it up visually
            if (ImGui.CollapsingHeader("Core"))
            {
                #region Spawn Object
                if (isInitialized && ImGui.Button("Spawn Object")) on_spawn_click();
                #endregion
                #region Clear
                if (isInitialized) ImGui.SameLine();
                if (ImGui.Button("Clear Objects")) on_clear_click();
                ImTools.ApplyTooltip("Clears all placed objects.");
                #endregion
                #region FO Identifier
                ImGui.InputText("FileObject Identifier", data_fo, 128);
                #endregion
                #region Scaling
                ImGui.SliderFloat3("Scale", ref currentScale, 0.1f, 10);
                ImTools.ApplyTooltip(
                    "The objects scale (X Y Z), default is 1.\nIf you need a precise scale, CTRL + LEFT CLICK to write your own value");
                #endregion
            }

            if (ImGui.CollapsingHeader("Misc"))
            {
                #region Load Save
                var buttonSize = new Vector2(103, 25);
                ImGui.InputText("Name", data_ls, 32);
                ImTools.ApplyTooltip("Enter your saved Objects Preset name");
                if (ImGui.Button("Save", buttonSize))
                    FileAdapter.create_object_preset(Encoding.UTF8.GetString(data_ls).Replace("\u0000", string.Empty));
                ImGui.SameLine(113);
                if (ImGui.Button("Load", buttonSize))
                    load(Encoding.UTF8.GetString(data_ls).Replace("\u0000", string.Empty));
                #endregion
            }

            if (ImGui.Button($"{(isInitialized ? "Reset" : "Initialize")}###HeadButton",
                new Vector2(450, 25))) on_head_click();
            ImTools.ApplyTooltip(isInitialized
                ? "Deactivates the spawner.\n" +
                  "Please be aware that starting it back up will reset all objects!"
                : "Activates the spawner.\n" +
                  "Please be aware that starting it back up will reset all objects!");
        }

        /// <summary>
        /// Called when the user presses on the Spawn button.
        /// </summary>
        private void on_spawn_click() =>
            spawn(Encoding.UTF8.GetString(data_fo).Replace("\u0000", string.Empty), currentScale);

        /// <summary>
        /// Called when the user presses on the Head (Initialize/Reset) button.
        /// </summary>
        private void on_head_click()
        {
            if (isInitialized) deactivate(false);
            else activate();
        }

        /// <summary>
        /// Called when the user presses on the Clear button.
        /// </summary>
        private void on_clear_click() => deactivate(true);

        /// <summary>
        /// Draw the window.
        /// </summary>
        protected internal override void draw()
        {
            if (!ImGui.Begin($"Spawner - #{get_objects_count()} Objects###Spawner", ref shouldDisplay,
                AlwaysAutoResize))
            {
                ImGui.End();
                return;
            }

            if (!Debugger.IsAttached)
            {
                shouldDisplay = false;
                return;
            }

            ImTools.CentreText("THIS IS STILL IN PREVIEW");
            draw_controls();
        }

        protected internal override void initialize() => identifier = "Spawner";
    }
}