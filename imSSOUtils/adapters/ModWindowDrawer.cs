using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using ImGuiNET;
using imSSOUtils.mod;
using imSSOUtils.mod.option.@static;

namespace imSSOUtils.adapters
{
    /// <summary>
    /// Makes drawing pre-made mod windows easier.
    /// </summary>
    internal readonly struct ModWindowDrawer
    {
        #region Variables
        /// <summary>
        /// A collection of all the mods.
        /// </summary>
        private static List<KeyValuePair<ModAccessor, string>> collection;

        /// <summary>
        /// Button scales.
        /// </summary>
        private const float buttonSingle = 289, buttonOptions = 250;

        /// <summary>
        /// The scale of the windows.
        /// </summary>
        private static readonly Vector2 scale = new(305, 220);

        /// <summary>
        /// Determines whether we have cached <see cref="collection"/> or not.
        /// </summary>
        private static bool hasCached;
        #endregion

        /// <summary>
        /// Begin drawing the core components.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="shouldDisplay"></param>
        public static void begin(string title, ref bool shouldDisplay)
        {
            if (!ImGui.Begin(title, ref shouldDisplay, ImGuiWindowFlags.NoResize))
            {
                ImGui.End();
                return;
            }

            // ! By caching this, we save loads of processing power as it's only done once rather than every tick.
            if (!hasCached)
            {
                collection = ModOperator.get_mods().ToList();
                hasCached = true;
            }

            ImGui.SetWindowSize(scale);
            for (var i = 0; i < collection.Count; i++)
            {
                var entry = collection[i];
                if (entry.Value != title) continue;
                var accessor = entry.Key;
                if (ImGui.Button(accessor.name.Replace('_', ' '),
                    new Vector2(ModOption.has_options(accessor.mod) ? buttonOptions : buttonSingle,
                        ImGui.GetItemRectSize().Y)))
                    entry.Key.mod.on_trigger();
                ModOption.list_options(accessor.mod, accessor.name);
            }

            ImGui.End();
        }
    }
}