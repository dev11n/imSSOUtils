using System;
using System.Collections.Generic;
using System.Numerics;
using ImGuiNET;
using imSSOUtils.adapters;

namespace imSSOUtils.mod.option.@static
{
    /// <summary>
    /// Adds support for options in mods.
    /// </summary>
    internal class ModOption
    {
        #region Variables
        /// <summary>
        /// Custom start strings.
        /// </summary>
        public const string buttonStart = "button_",
            checkboxStart = "checkbox_",
            inputTextStart = "inputText_",
            floatSliderStart = "floatSlider_",
            buttonInvokeStart = "buttonInvoke_";

        /// <summary>
        /// All checkboxes.
        /// </summary>
        public static readonly Dictionary<string, bool> checkboxes = new();

        /// <summary>
        /// All input texts.
        /// </summary>
        public static readonly Dictionary<string, byte[]> inputTexts = new();

        /// <summary>
        /// All float sliders.
        /// </summary>
        public static readonly Dictionary<string, float> f_sliders = new();

        /// <summary>
        /// Button size.
        /// </summary>
        private static readonly Vector2 size = new(289, 25);
        #endregion

        /// <summary>
        /// List all options for a specific mod.
        /// </summary>
        /// <param name="mod"></param>
        /// <param name="modName"></param>
        public static void list_options(IMod mod, string modName)
        {
            var json = mod.raw;
            ImGui.SameLine();
            if (!ImGui.CollapsingHeader($"Options##{modName}")) return;
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
                if (name.StartsWith(floatSliderStart))
                {
                    var split = property.Value.ToString().Split('|');
                    draw_float_slider(name, Convert.ToSingle(split[0]), Convert.ToSingle(split[1]));
                }
            }

            ImGui.NewLine();
        }

        /// <summary>
        /// Check whether a specific custom mod has any options.
        /// </summary>
        /// <param name="mod"></param>
        /// <returns></returns>
        public static bool has_options(IMod mod)
        {
            var text = mod.raw.ToString();
            return text.Contains(buttonStart) || text.Contains(checkboxStart) || text.Contains(inputTextStart) ||
                   text.Contains(floatSliderStart) || text.Contains(buttonInvokeStart);
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

        /// <summary>
        /// Draw a new InputText.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="minValue"></param>
        /// <param name="maxValue"></param>
        private static void draw_float_slider(string text, float minValue, float maxValue)
        {
            if (!f_sliders.ContainsKey(text)) f_sliders.Add(text, 0);
            var current = f_sliders[text];
            ImGui.SliderFloat(text.Replace(floatSliderStart, string.Empty).Replace('_', ' '), ref current, minValue,
                maxValue);
            f_sliders[text] = current;
        }
    }
}