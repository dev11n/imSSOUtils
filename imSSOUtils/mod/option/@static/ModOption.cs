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
    internal readonly struct ModOption
    {
        #region Variables
        /// <summary>
        /// Custom start strings.
        /// </summary>
        public const string buttonStart = "button_",
            checkboxStart = "checkbox_",
            inputTextStart = "inputText_",
            floatSliderStart = "floatSlider_",
            buttonInvokeStart = "buttonInvoke_",
            centreTextStart = "centreText_",
            intSliderStart = "intSlider_";

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
        /// All int sliders.
        /// </summary>
        public static readonly Dictionary<string, int> i_sliders = new();

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
                if (name.StartsWith(centreTextStart))
                    draw_centred_text(name);
                if (name.StartsWith(floatSliderStart))
                {
                    var split = property.Value.ToString().Split('|');
                    draw_float_slider(name, Convert.ToSingle(split[0]), Convert.ToSingle(split[1]));
                }

                if (name.StartsWith(intSliderStart))
                {
                    var split = property.Value.ToString().Split('|');
                    draw_int_slider(name, Convert.ToInt32(split[0]), Convert.ToInt32(split[1]));
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
                   text.Contains(floatSliderStart) || text.Contains(buttonInvokeStart) || text.Contains(centreTextStart)
                   || text.Contains(intSliderStart);
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
            if (ImGui.Checkbox(name, ref current))
                checkboxes[text] = current;
        }

        /// <summary>
        /// Draw a new centred text control.
        /// </summary>
        /// <param name="text"></param>
        private static void draw_centred_text(string text)
        {
            var name = text.Replace(centreTextStart, string.Empty).Replace('_', ' ');
            ImTools.CentreText(name);
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
        /// Draw a new float slider.
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

        /// <summary>
        /// Draw a new int slider.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="minValue"></param>
        /// <param name="maxValue"></param>
        private static void draw_int_slider(string text, int minValue, int maxValue)
        {
            if (!i_sliders.ContainsKey(text)) i_sliders.Add(text, 0);
            var current = i_sliders[text];
            ImGui.SliderInt(text.Replace(floatSliderStart, string.Empty).Replace('_', ' '), ref current, minValue,
                maxValue);
            i_sliders[text] = current;
        }
    }
}