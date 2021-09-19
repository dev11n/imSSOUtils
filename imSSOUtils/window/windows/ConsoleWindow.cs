using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Text;
using DriverProgram.adapters;
using ImGuiNET;
using imSSOUtils.command;
using imSSOUtils.customs;
using static ImGuiNET.ImGuiWindowFlags;

namespace imSSOUtils.window.windows
{
    /// <summary>
    /// Console Window
    /// </summary>
    internal class ConsoleWindow : IWindow
    {
        #region Variables
        /// <summary>
        /// Console messages.
        /// </summary>
        private static readonly List<ConsoleMessage> messages = new();

        /// <summary>
        /// A buffer that temporarily holds the current data of InputText.
        /// </summary>
        private static readonly byte[] data = new byte[100];
        #endregion

        /// <summary>
        /// Draw the console.
        /// </summary>
        private static void draw_log()
        {
            ImGui.SetWindowSize(new Vector2(767, 403));
            ImGui.BeginChild("Log", new Vector2(750, 335));
            try
            {
                foreach (var msg in messages)
                {
                    string text = msg.text, prefix = msg.system_prefix;
                    var system = msg.system;
                    if (!system) ImGui.TextColored(msg.colour, $"# {text}");
                    else
                    {
                        if (!text.StartsWith("ImGui."))
                            ImGui.TextColored(msg.colour, prefix is {Length: 0} ? text : $"{prefix} {text}");
                        else
                        {
                            // Support for calling functions after "ImGui."
                            // Like ImGui.Bullet calls ImGui.Bullet();
                            var spaceSplit = text.Split(' ');
                            var split = spaceSplit[0].Split('.');
                            var function = split[1];
                            typeof(ImGui).GetMethod(function)?.Invoke(null, null);
                            if (spaceSplit is not {Length: >= 2}) continue;
                            ImGui.TextColored(msg.colour, text.Replace($"ImGui.{function} ", string.Empty));
                        }
                    }
                }
            }
            catch (Exception)
            {
                // ignored
            }

            ImGui.EndChild();
        }

        /// <summary>
        /// Send a message to the console.
        /// </summary>
        public static void send_input(string text, float r, float g, float b, float alpha = 1) =>
            messages.Add(new ConsoleMessage(text, ColourAdapter.rgba_to_frgba(r, g, b, alpha), false, string.Empty));

        /// <summary>
        /// Send a message to the console.
        /// <para>Overload for system messages.</para>
        /// </summary>
        public static void send_input(string text, string prefix, float r, float g, float b, float alpha = 1) =>
            messages.Add(new ConsoleMessage(text, ColourAdapter.rgba_to_frgba(r, g, b, alpha), true, prefix));

        /// <summary>
        /// Send a message to the console.
        /// <para>Overload for system messages.</para>
        /// </summary>
        public static void send_input(string text, Color colour, float alpha = 1) =>
            messages.Add(new ConsoleMessage(text, ColourAdapter.rgba_to_frgba(colour.R, colour.G, colour.B, alpha),
                false, string.Empty));

        /// <summary>
        /// Send a message to the console.
        /// <para>Overload for system messages.</para>
        /// </summary>
        public static void send_input(string text, string prefix, Color colour, float alpha = 1) =>
            messages.Add(new ConsoleMessage(text, ColourAdapter.rgba_to_frgba(colour.R, colour.G, colour.B, alpha),
                true, prefix));

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

            draw_log();
            ImGui.PushItemWidth(750);
            if (ImGui.InputText(string.Empty, data, 100, ImGuiInputTextFlags.EnterReturnsTrue))
            {
                // ! Remove all "00" bytes, aka \u0000
                var input = Encoding.UTF8.GetString(data).Replace("\u0000", string.Empty);
                send_input(input, Color.White);
                CommandOperator.push_command(input.Split(' '));
                // Clear the buffer which also clears the input.
                Array.Clear(data, 0, data.Length);
            }

            ImGui.PopItemWidth();
            ImGui.End();
        }

        protected internal override void initialize() => identifier = "Console";
    }
}