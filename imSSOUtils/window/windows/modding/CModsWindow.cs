using System.Linq;
using System.Numerics;
using ImGuiNET;
using imSSOUtils.adapters;
using imSSOUtils.mod.option.dynamic;

namespace imSSOUtils.window.windows.modding
{
    /// <summary>
    /// All custom-made mods.
    /// </summary>
    internal class CModsWindow : IWindow
    {
        #region Variables
        /// <summary>
        /// Button scales.
        /// </summary>
        private const float buttonSingle = 289, buttonOptions = 250;

        /// <summary>
        /// The scale of this window.
        /// </summary>
        private readonly Vector2 scale = new(305, 220);

        /// <summary>
        /// Determines whether we have done the initial scaling or not.
        /// </summary>
        private bool hasScaledOnce;
        #endregion

        /// <summary>
        /// Draw the window.
        /// </summary>
        protected internal override void draw()
        {
            if (!ImGui.Begin(identifier, ref shouldDisplay))
            {
                ImGui.End();
                return;
            }

            if (!hasScaledOnce)
            {
                ImGui.SetWindowSize(scale);
                hasScaledOnce = true;
            }

            var collection = Alpine.get_cmods_categories().ToList();
            if (collection.Count is not 0) ImGui.BeginTabBar("Categories");
            for (var i = 0; i < collection.Count; i++)
            {
                var cat = collection[i];
                if (!ImGui.BeginTabItem(cat)) continue;
                var mods = Alpine.get_cmods(cat);
                for (var j = 0; j < mods.Count; j++)
                {
                    var mod = mods[j];
                    if (ImGui.Button(mod.name.Replace('_', ' '),
                        new Vector2(ImGui.GetWindowWidth() - (CModOption.has_options(mod) ? 55 : 16), 25)))
                        MemoryAdapter.direct_call(mod.code);
                    CModOption.list_options(mod);
                }

                ImGui.EndTabItem();
            }

            if (collection.Count is not 0) ImGui.EndTabBar();
            ImGui.End();
        }

        protected internal override void initialize() => identifier = "Custom Mods";
    }
}