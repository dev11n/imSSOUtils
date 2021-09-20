using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using imSSOUtils.adapters.low_level;
using imSSOUtils.mod.option.dynamic;
using imSSOUtils.mod.option.@static;
using imSSOUtils.window.windows;
using OTFE;
using static imSSOUtils.adapters.PXInternal;
using static imSSOUtils.registers.BaseRegister;
using Point = Veldrid.Point;

namespace imSSOUtils.adapters
{
    /// <summary>
    /// Fairly basic and straight forward memory adapter.
    /// </summary>
    internal struct MemoryAdapter
    {
        #region Variables
        /// <summary>
        /// OTFE Head.
        /// </summary>
        public static readonly OTFEHead head = new();

        /// <summary>
        /// <see cref="Random"/> instance.
        /// </summary>
        private static readonly Random rand = new();

        /// <summary>
        /// Window dimensions.
        /// </summary>
        private static WinAPI.Dimensions dim;

        /// <summary>
        /// Timer responsible for syncing the window position.
        /// </summary>
        public static Timer syncPosition;
        #endregion

        /// <summary>
        /// Check if the process is elevated (running as an Administrator).
        /// </summary>
        /// <returns>true if it is.</returns>
        public static bool is_elevated() => head.get_memory_dll().IsAdmin();

        /// <summary>
        /// Checks whether modding is enabled or not.
        /// </summary>
        /// <returns></returns>
        public static bool is_enabled() => head.is_enabled();

        /// <summary>
        /// Try and sync the size and position to SSO.
        /// </summary>
        private static void sync_position(Process process) =>
            syncPosition = new Timer(_ =>
            {
                if (!ProcessAdapter.IsProcessAlive(runtime)) Environment.Exit(0);
                WinAPI.GetWindowRect(process.MainWindowHandle, ref dim);
                dim.left += 10;
                dim.top += 31;
                Program.set_size(new Point(dim.left, dim.top),
                    new Point(dim.right - dim.left - 7, dim.bottom - dim.top));
            }, null, 0, 15);

        /// <summary>
        /// Prepare everything.
        /// </summary>
        public static async Task patch_memory()
        {
            cache_pointers();
            await head.begin(dc01, dc01_bytes, dc02, dc02_bytes, dc03, dc03_bytes, bypass, bypass_bytes, state);
            convert();
            var ssoClient = Process.GetProcessesByName(runtime).FirstOrDefault();
            sync_position(ssoClient);
            // ! Cache cvar
            show_white_message("Caching CVar...");
            await CVar.setup_cvar();
            show_white_message("SSOUtils loaded successfully. Have fun and keep the experience fair for everyone!");
        }

        /// <summary>
        /// Formats CMods and their dynamic variable values.
        /// </summary>
        private static string dynamic_formatting(string input)
        {
            var processed = input;
            // ? Dynamic Mods
            foreach (var checkbox in CModOption.checkboxes)
                processed = processed.Replace(checkbox.Key, checkbox.Value ? "1" : "0");
            foreach (var inputText in CModOption.inputTexts)
                processed = processed.Replace(inputText.Key,
                    Encoding.UTF8.GetString(inputText.Value).Replace("\u0000", string.Empty));
            // ? Static Mods
            foreach (var checkbox in ModOption.checkboxes)
                processed = processed.Replace(checkbox.Key, checkbox.Value ? "1" : "0");
            foreach (var inputText in ModOption.inputTexts)
                processed = processed.Replace(inputText.Key,
                    Encoding.UTF8.GetString(inputText.Value).Replace("\u0000", string.Empty));
            foreach (var floatSlider in ModOption.f_sliders)
                processed = processed.Replace(floatSlider.Key,
                    floatSlider.Value.ToString(CultureInfo.InvariantCulture).Replace(".", "::"));
            return processed;
        }

        /// <summary>
        /// Inject code and execute it directly.
        /// </summary>
        public static void direct_call(string newString)
        {
            if (CVar.hasCachedAll) CVar.write_cvar02("0");
            // CVar_02 is modified again inside the string below if everything has been cached.
            var code = Alpine.proc_frm_string(dynamic_formatting(newString)) +
                       "\nglobal/ReportWindow.SetScaleX(0.0);" + (CVar.hasCachedAll
                           ? "\nglobal/CSIInspectView/FailedMessageData.SetDataString(\"1\");"
                           : string.Empty);
            head.inject_code(code);
            if (CVar.hasCachedAll)
            {
                // Check 5 times if executing the mod failed (a.k.a the entire code failed to execute)
                for (var i = 0; i < 5; i++)
                {
                    // If its 1, return and exit the loop.
                    if (CVar.read_cvar02_int() is 1) return;
                    // Failed (its 0 / false, a.k.a it failed executing in one way or another), try and fix it
                    ConsoleWindow.send_input($"failed executing mod, trying to recover ({i + 1}/5)", "[alpine internal]",
                        Color.OrangeRed);
                    code += $"\n// {head.get_random_string(10)}";
                    head.inject_code(code);
                }

                if (CVar.read_cvar02_int() is not 1) return;
                ConsoleWindow.send_input("failed executing mod, please check your code and try again (5/5)",
                    "[alpine internal]", Color.OrangeRed);
            }
        }
    }
}