using System.Collections.Generic;
using System.Numerics;
using ImGuiNET;
using imSSOUtils.adapters;
using imSSOUtils.customs;

namespace imSSOUtils.mod.option.dynamic
{
    /// <summary>
    /// Adds support for options in custom mods.
    /// </summary>
    internal class CModOption
    {
        #region Variables
        /// <summary>
        /// Custom options start strings.
        /// </summary>
        public const string buttonStart = "button_", checkboxStart = "checkbox_", inputTextStart = "inputText_";

        /// <summary>
        /// All checkboxes.
        /// </summary>
        public static readonly Dictionary<string, bool> checkboxes = new();

        /// <summary>
        /// All input texts.
        /// </summary>
        public static readonly Dictionary<string, byte[]> inputTexts = new();

        /// <summary>
        /// Button size.
        /// </summary>
        private static readonly Vector2 size = new(289, 25);
        #endregion

        /// <summary>
        /// Clean everything.
        /// </summary>
        public static void Dispose()
        {
            checkboxes.Clear();
            inputTexts.Clear();
        }

        /// <summary>
        /// List all options for a specific custom mod.
        /// </summary>
        /// <param name="mod"></param>
        public static void list_options(CMod mod)
        {
            var json = mod.raw;
            ImGui.SameLine();
            if (!ImGui.CollapsingHeader($"Options##{mod.name}")) return;
            ImTools.CentreText("Options");
            foreach (var property in json.Properties())
            {
                var name = property.Name;
                if (name.StartsWith(buttonStart))
                    draw_button(name, property.Value.ToString());
                if (name.StartsWith(checkboxStart))
                    draw_checkbox(name);
                if (name.StartsWith(inputTextStart))
                    draw_inputtext(name);
            }

            ImGui.NewLine();
        }

        /// <summary>
        /// Check whether a specific custom mod has any options.
        /// </summary>
        /// <param name="mod"></param>
        /// <returns></returns>
        public static bool has_options(CMod mod)
        {
            var text = mod.raw.ToString();
            return text.Contains(buttonStart) || text.Contains(checkboxStart) || text.Contains(inputTextStart);
        }

        /// <summary>
        /// Draw a new button.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="code"></param>
        private static void draw_button(string text, string code)
        {
            var name = text.Replace(buttonStart, string.Empty).Replace('_', ' ');
            if (ImGui.Button(name, size)) MemoryAdapter.direct_call(code);
        }

        /// <summary>
        /// Draw a new checkbox.
        /// </summary>
        /// <param name="text"></param>
        private static void draw_checkbox(string text)
        {
            var name = text.Replace(checkboxStart, string.Empty).Replace('_', ' ');
            if (!checkboxes.ContainsKey(text)) checkboxes.Add(text, false);
            var current = checkboxes[text];
            ImGui.Checkbox(name, ref current);
            checkboxes[text] = current;
        }

        /// <summary>
        /// Draw a new InputText.
        /// </summary>
        /// <param name="text"></param>
        private static void draw_inputtext(string text)
        {
            if (!inputTexts.ContainsKey(text)) inputTexts.Add(text, new byte[100]);
            var current = inputTexts[text];
            ImGui.InputText(text.Replace(inputTextStart, string.Empty).Replace('_', ' '), current, 100);
            inputTexts[text] = current;
        }
    }
}