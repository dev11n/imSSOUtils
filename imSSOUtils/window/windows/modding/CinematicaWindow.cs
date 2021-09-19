using System.Numerics;
using System.Text;
using ImGuiNET;
using imSSOUtils.adapters;
using imSSOUtils.cinematica;
using imSSOUtils.registers;
using static ImGuiNET.ImGuiWindowFlags;

namespace imSSOUtils.window.windows.modding
{
    /// <summary>
    /// Cinematica Window.
    /// </summary>
    internal class CinematicaWindow : IWindow
    {
        #region Variables
        /// <summary>
        /// Current control values.
        /// </summary>
        // ! Have to make it static otherwise the value resets after dragging.
        public static int currentSpeedValue = 1, currentThresholdValue = 200;

        /// <summary>
        /// A buffer that temporarily holds the current data of InputText.
        /// </summary>
        private static readonly byte[] data = new byte[32];
        #endregion

        // ? Everything is split up into their own functions to make it more easy-to-read.

        /// <summary>
        /// Draw all the controls.
        /// </summary>
        private void draw_controls()
        {
            var canModify = CinematicaCore.can_modify();
            // Regions to tidy it up visually
            if (ImGui.CollapsingHeader("Core"))
            {
                #region Toggle Saving
                var current = ButtonLabelRegister.currentPointText;
                const string start = ButtonLabelRegister.pointStart, end = ButtonLabelRegister.pointEnd;
                if (ImGui.Button($"{current}###ToggleSaving")) // ToggleSaving is the permanent ID of the button.
                {
                    on_saving_toggle();
                    current = current == start ? end : start;
                }

                ButtonLabelRegister.currentPointText = current;
                #endregion
                #region Clear and Enhancements
                ImGui.SameLine();
                if (ImGui.Button("Clear Points") && canModify) on_clear_click();
                ImTools.ApplyTooltip("Clears all currently logged points.");
                ImGui.SameLine();
                if (ImGui.Button("Enhancements")) on_enhance_click();
                ImTools.ApplyTooltip("Disables your horses skin and enables NoFall.");
                #endregion
                #region Speed Adjustments
                if (ImGui.SliderInt("Speed", ref currentSpeedValue, 1, 20)) on_slider_drag(currentSpeedValue);
                ImTools.ApplyTooltip(
                    "Speed of how fast the current cinematic should be played at.\nHigher values may cause random crashes or lag issues.");
                #endregion
            }

            if (ImGui.CollapsingHeader("Misc"))
            {
                #region Load Save
                var buttonSize = new Vector2(103, 25);
                ImGui.InputText("Name", data, 32);
                ImTools.ApplyTooltip("Enter your saved Cinematica name");
                if (ImGui.Button("Save", buttonSize) && canModify)
                    CinematicaCore.save_current(Encoding.UTF8.GetString(data).Replace("\u0000", string.Empty));
                ImGui.SameLine(113);
                if (ImGui.Button("Load", buttonSize) && canModify)
                    CinematicaCore.load_cna(Encoding.UTF8.GetString(data).Replace("\u0000", string.Empty));
                #endregion
                #region Threshold
                if (canModify)
                {
                    ImGui.SliderInt("Threshold Value", ref currentThresholdValue, 50, 5000);
                    ImTools.ApplyTooltip("This is the update frequency for points in milliseconds.\n" +
                                         "The higher the value, the higher the precision.\n" +
                                         "Please be aware that low values may cause lag and big file sizes!\n" +
                                         "CTRL + LEFT CLICK to write a value");
                }
                #endregion
            }

            if (ImGui.Button("Start Cinematic", new Vector2(450, 25))) on_start_cinematic_click();
            ImTools.ApplyTooltip("Start the cinematic.");
        }

        /// <summary>
        /// Called when the user presses on the ToggleSaving button.
        /// </summary>
        private void on_saving_toggle() => CinematicaCore.build_path();

        /// <summary>
        /// Called when the user presses on the Start Cinematic button.
        /// </summary>
        private void on_start_cinematic_click() => CinematicaCore.start_path();

        /// <summary>
        /// Called when the user presses on the Clear Points button.
        /// </summary>
        private void on_clear_click() => CinematicaCore.clear();

        /// <summary>
        /// Called when the user presses on the Enhancements button.
        /// </summary>
        private void on_enhance_click()
        {
            if (!MemoryAdapter.is_enabled()) return;
            MemoryAdapter.direct_call("CurrentHorse::SetHorseMaxFallTime(9999);\nCurrentHorse->Skin::Stop();");
        }

        /// <summary>
        /// Called when the speed slider has had its value changed.
        /// </summary>
        /// <param name="value">The current value</param>
        private void on_slider_drag(int value)
        {
            if (currentSpeedValue is < 1 or > 20) currentSpeedValue = 1;
            CinematicaCore.set_speed(value);
        }

        /// <summary>
        /// Draw the window.
        /// </summary>
        protected internal override void draw()
        {
            if (!ImGui.Begin($"Cinematica - #{CinematicaCore.positions.Count} Points###Cinematica", ref shouldDisplay,
                AlwaysAutoResize))
            {
                ImGui.End();
                return;
            }

            draw_controls();
        }

        protected internal override void initialize() => identifier = "Cinematica";
    }
}