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
        /// Sizes.
        /// </summary>
        private readonly Vector2 main = new(767, 420),
            child = new(750, 335);
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

            ImGui.SetWindowSize(main);
            draw_centre();
            ImGui.PushItemWidth(750);
            ImTools.CentreText("Everything you write has to be in PXScript!");
            if (!ImGui.InputText(string.Empty, data, 100, EnterReturnsTrue)) return;
            // ! Remove all "00" bytes, aka \u0000
            var fixedString = Encoding.UTF8.GetString(data).Replace("\u0000", string.Empty);
            var input = format(fixedString);
            get_child_objects(input, fixedString);
            // Clear the buffer which also clears the input.
            Array.Clear(data, 0, data.Length);
        }

        /// <summary>
        /// Add all child objects.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="rawInput"></param>
        private void get_child_objects(string input, string rawInput) => new Thread(() =>
        {
            var count = PXInternal.get_child_count(rawInput);
            if (treeNodes.ContainsKey(rawInput)) treeNodes.Remove(rawInput);
            for (var i = 0; i < count; i++)
            {
                var clone = i + 1;
                add_node(rawInput, $"{clone} / {count}: {PXInternal.get_child_name(input, i)}");
            }
        }).Start();

        /// <summary>
        /// Format to Alpine Script.
        /// </summary>
        private string format(string content)
        {
            var txt = content;
            foreach (var set in Alpine.sets)
            {
                if (!txt.Contains(set.Value)) continue;
                ImGui.SetWindowFocus("Misc");
                txt = txt.Replace(set.Value, set.Key);
            }

            return txt;
        }

        /// <summary>
        /// Add a value to a specific tree.
        /// <para>If the specific tree doesn't exist, it'll be created.</para>
        /// </summary>
        /// <param name="node"></param>
        /// <param name="value"></param>
        private void add_node(string node, string value)
        {
            if (!treeNodes.ContainsKey(node)) treeNodes.Add(node, $"{value}\n");
            else treeNodes[node] = treeNodes[node] += $"{value}\n";
        }

        /// <summary>
        /// Draw the centre content (everything above the input text).
        /// </summary>
        private void draw_centre()
        {
            ImGui.BeginChild("Centre", child);
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