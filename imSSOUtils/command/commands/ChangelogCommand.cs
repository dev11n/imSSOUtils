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
            ConsoleWindow.send_input("ImGui.Bullet Updated Data Viewer", empty, color);
            ConsoleWindow.send_input(
                "ImGui.Bullet The \"Custom Mods\" Window can now be resized\n- Suggested by: biseukis", empty, color);
            ConsoleWindow.send_input("ImGui.Separator", empty, color);
            ConsoleWindow.send_input("Alpine / Scripting", empty, color);
            ConsoleWindow.send_input("ImGui.Separator", empty, color);
            ConsoleWindow.send_input(
                "ImGui.Bullet Fixed an issue where long script-code would still execute, even when removed", empty,
                color);
            ConsoleWindow.send_input(
                "ImGui.Bullet \"is\" and \"is not\" have been removed from Alpine due to formatting issues", empty,
                color);
            ConsoleWindow.send_input(
                "ImGui.Bullet OTFE updated to v1.2 which aims to fix:\n- Random crashes\n- Issues finding CVar Addresses",
                empty,
                color);
        }
    }
}