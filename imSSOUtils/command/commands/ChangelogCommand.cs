using System.Drawing;
using imSSOUtils.window.windows;

namespace imSSOUtils.command.commands
{
    /// <summary>
    /// Displays the current SSOUtils version changelog.
    /// </summary>
    internal struct ChangelogCommand : ICommand
    {
        /// <summary>
        /// Execute the command.
        /// </summary>
        /// <param name="args"></param>
        void ICommand.push(string[] args)
        {
            var color = Color.White;
            var empty = string.Empty;
            ConsoleWindow.send_input("ImGui.Separator", empty, color);
            ConsoleWindow.send_input("Additions / Changes", empty, color);
            ConsoleWindow.send_input("ImGui.Separator", empty, color);
            ConsoleWindow.send_input("ImGui.Bullet Renamed \"Sky Modifier\" to \"Render Modifier\"", empty, color);
            ConsoleWindow.send_input("ImGui.Bullet Fixed several crash issues", empty, color);
            ConsoleWindow.send_input(
                "ImGui.Bullet Cinematica updated with a \"Smooth\" button and a higher max-speed value of 200", empty,
                color);
            ConsoleWindow.send_input(
                "ImGui.Bullet Updated the font used in \"Extra Information\" and \"Alpine Editor\"", empty, color);
            ConsoleWindow.send_input("ImGui.Bullet Cinematica now supports a higher max speed value", empty, color);
            ConsoleWindow.send_input("ImGui.Bullet Updated Save Manager", empty, color);
            ConsoleWindow.send_input("ImGui.Separator", empty, color);
            ConsoleWindow.send_input("Alpine / Scripting", empty, color);
            ConsoleWindow.send_input("ImGui.Separator", empty, color);
            ConsoleWindow.send_input(
                "ImGui.Bullet Fixed an issue where long script-code would still execute, even when removed", empty,
                color);
            ConsoleWindow.send_input(
                "ImGui.Bullet You can now use \"is\" (results in ==) and \"is not\" (results in !=) whilst scripting",
                empty, color);
        }
    }
}