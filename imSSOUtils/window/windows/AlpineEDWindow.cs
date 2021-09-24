using System.Drawing;
using System.Numerics;
using ImGuiNET;
using imSSOUtils.adapters;
using static ImGuiNET.ImGuiWindowFlags;

namespace imSSOUtils.window.windows
{
    /// <summary>
    /// Alpine Window
    /// </summary>
    internal class AlpineEDWindow : IWindow
    {
        #region Variables
        /// <summary>
        /// The window size.
        /// </summary>
        private readonly Vector2 size = new(767, 377);

        /// <summary>
        /// A buffer that holds the current data of InputText.
        /// </summary>
        public static string content = string.Empty;
        #endregion

        /// <summary>
        /// Draw the text area.
        /// </summary>
        private void draw_text_area()
        {
            ImGui.SameLine(-1);
            ImGui.SetCursorPosY(ImGui.GetCursorPosY() - 4);
            ImGui.InputTextMultiline(string.Empty, ref content, 9000, new Vector2(765, 314));
            format();
        }

        /// <summary>
        /// Draw the "Push into Game" button.
        /// </summary>
        private void draw_button()
        {
            // Not a pretty way of moving it to the side, but it works.
            ImGui.NewLine();
            ImGui.SameLine(-1);
            if (!ImGui.Button("Push into Game", new Vector2(765, 25))) return;
            if (content.Contains('/') || content.Contains('.') && !content.Contains(".scene") &&
                !content.Contains(".pxo") && !content.Contains(".pte") &&
                !content.EndsWith("f);"))
            {
                ConsoleWindow.send_input(
                    "Found PXScript code, this isn't supported. Please switch to Alpine Script!", "[alpine]",
                    Color.OrangeRed);
                return;
            }

            MemoryAdapter.direct_call(content);
        }

        /// <summary>
        /// Format to Alpine Script.
        /// </summary>
        private void format()
        {
            var txt = content;
            foreach (var set in Alpine.sets)
            {
                if (!txt.Contains(set.Value)) continue;
                ImGui.SetWindowFocus("Misc");
                txt = txt.Replace(set.Value, set.Key);
                content = txt;
                ImGui.SetWindowFocus("Alpine Editor");
                ImGui.SetKeyboardFocusHere();
            }
        }

        /// <summary>
        /// Draw the window.
        /// </summary>
        protected internal override void draw()
        {
            if (!ImGui.Begin(identifier, ref shouldDisplay, NoResize | NoScrollbar | NoScrollWithMouse))
            {
                ImGui.End();
                return;
            }

            ImGui.SetWindowSize(size);
            draw_text_area();
            draw_button();
            ImGui.End();
        }

        protected internal override void initialize() => identifier = "Alpine Editor";
    }
}