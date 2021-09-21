using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Threading;
using ImGuiNET;
using imSSOUtils.adapters;
using static ImGuiNET.ImGuiInputTextFlags;
using static ImGuiNET.ImGuiWindowFlags;

namespace imSSOUtils.window.windows.modding
{
    internal class DataViewerWindow : IWindow
    {
        #region Variables
        /// <summary>
        /// All tree nodes.
        /// </summary>
        private static readonly Dictionary<string, string> treeNodes = new();

        /// <summary>
        /// A buffer that holds the current data of InputText.
        /// </summary>
        private static readonly byte[] data = new byte[100];

        /// <summary>
        /// The window size.
        /// </summary>
        private readonly Vector2 size = new(767, 420);
        #endregion

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
            draw_centre();
            ImGui.PushItemWidth(750);
            ImTools.CentreText("Everything you write is automatically converted to Alpine!");
            if (ImGui.InputText(string.Empty, data, 100, EnterReturnsTrue))
            {
                // ! Remove all "00" bytes, aka \u0000
                var input = Alpine.proc_frm_string(Encoding.UTF8.GetString(data).Replace("\u0000", string.Empty));
                get_child_objects(input);
                // Clear the buffer which also clears the input.
                Array.Clear(data, 0, data.Length);
            }
        }

        /// <summary>
        /// Add all child objects.
        /// </summary>
        /// <param name="input"></param>
        private void get_child_objects(string input) => new Thread(() =>
        {
            var count = PXInternal.get_child_count(input);
            for (var i = 0; i < count; i++)
                add_node(input, $"{i + 1} / {count}: {PXInternal.get_child_name(input, i)}");
        }).Start();

        /// <summary>
        /// Add a value to a specific tree.
        /// <para>If the specific tree doesn't exist, it'll be created.</para>
        /// </summary>
        /// <param name="node"></param>
        /// <param name="value"></param>
        private void add_node(string node, string value)
        {
            if (!treeNodes.ContainsKey(node)) treeNodes.Add(node, value);
            treeNodes[node] = treeNodes[node] += $"{value}\n";
        }

        /// <summary>
        /// Draw the centre content (everything above the input text).
        /// </summary>
        private void draw_centre()
        {
            ImGui.BeginChild("Centre", new Vector2(750, 335));
            foreach (var tree in treeNodes)
            {
                if (ImGui.TreeNode(tree.Key))
                {
                    var split = tree.Value.Split('\n');
                    for (var i = 0; i < split.Length; i++) ImGui.Text(split[i]);
                    ImGui.TreePop();
                }
            }

            ImGui.EndChild();
        }

        /// <summary>
        /// Assign the identifier.
        /// </summary>
        protected internal override void initialize() => identifier = "Data Viewer";
    }
}