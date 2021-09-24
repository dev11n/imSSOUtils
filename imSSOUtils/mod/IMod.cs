using System.Threading;
using imSSOUtils.adapters;
using imSSOUtils.mod.option.@static;
using Newtonsoft.Json.Linq;
using static System.String;
using static System.Text.Encoding;

namespace imSSOUtils.mod
{
    /// <summary>
    /// Basic mod interface.
    /// </summary>
    internal abstract class IMod
    {
        #region Variables
        /// <summary>
        /// JSON options.
        /// </summary>
        internal readonly JObject raw = new();

        /// <summary>
        /// A safe-to-use mod timer which can be used for Update simulations, etc.
        /// </summary>
        internal Timer modTimer;
        #endregion

        /// <summary>
        /// Trigger the mod.
        /// </summary>
        protected internal abstract void on_trigger();

        /// <summary>
        /// Called when the mod has had its instance created.
        /// </summary>
        protected internal abstract void on_initialize();

        protected void add_button(string identifier, string code) => raw.Add($"button_{identifier}", code);

        protected void add_checkbox(string identifier) => raw.Add($"checkbox_{identifier}", Empty);

        protected void add_input_text(string identifier) => raw.Add($"inputText_{identifier}", Empty);

        protected void add_float_slider(string identifier, float minValue, float maxValue, float defaultValue = 0)
        {
            var fixedIdentifier = $"floatSlider_{identifier}";
            raw.Add(fixedIdentifier, $"{minValue}|{maxValue}");
            ModOption.f_sliders[fixedIdentifier] = defaultValue is 0 ? minValue : defaultValue;
        }

        protected void add_int_slider(string identifier, int minValue, int maxValue, int defaultValue = 0)
        {
            var fixedIdentifier = $"intSlider_{identifier}";
            raw.Add(fixedIdentifier, $"{minValue}|{maxValue}");
            ModOption.i_sliders[fixedIdentifier] = defaultValue is 0 ? minValue : defaultValue;
        }

        protected void add_centre_text(string text)
        {
            var fixedIdentifier = $"centreText_{text}";
            raw.Add(fixedIdentifier, Empty);
        }

        protected bool get_checkbox_value(string identifier)
        {
            var fixedIdentifier = $"checkbox_{identifier}";
            return ModOption.checkboxes.ContainsKey(fixedIdentifier) && ModOption.checkboxes[fixedIdentifier];
        }

        protected float get_float_slider_value(string identifier)
        {
            var fixedIdentifier = $"floatSlider_{identifier}";
            return ModOption.f_sliders.ContainsKey(fixedIdentifier) ? ModOption.f_sliders[fixedIdentifier] : 0;
        }

        protected int get_int_slider_value(string identifier)
        {
            var fixedIdentifier = $"intSlider_{identifier}";
            return ModOption.i_sliders.ContainsKey(fixedIdentifier) ? ModOption.i_sliders[fixedIdentifier] : 0;
        }

        protected string get_input_text_value(string identifier)
        {
            var fixedIdentifier = $"inputText_{identifier}";
            return ModOption.inputTexts.ContainsKey(fixedIdentifier)
                ? UTF8.GetString(ModOption.inputTexts[fixedIdentifier]).Replace("\u0000", Empty)
                : Empty;
        }

        /// <summary>
        /// Execute mod code.
        /// </summary>
        /// <param name="code">The code to be executed.</param>
        protected void alpine_execute(string code) =>
            MemoryAdapter.direct_call(code);
    }
}