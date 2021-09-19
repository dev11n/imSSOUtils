using System.Drawing;
using imSSOUtils.window.windows;

namespace imSSOUtils.command.commands
{
    /// <summary>
    /// Simple help command.
    /// </summary>
    internal struct HelpCommand : ICommand
    {
        /// <summary>
        /// Execute the command.
        /// </summary>
        /// <param name="args"></param>
        void ICommand.push(string[] args)
        {
            var color = Color.White;
            ConsoleWindow.send_input("ImGui.Separator", string.Empty, color);
            ConsoleWindow.send_input("Software", string.Empty, color);
            ConsoleWindow.send_input("ImGui.Separator", string.Empty, color);
            ConsoleWindow.send_input("ImGui.Bullet information - Displays information about SSOUtils", string.Empty,
                color);
            ConsoleWindow.send_input("ImGui.Bullet changelog - Displays the current changelog", string.Empty, color);
            ConsoleWindow.send_input("ImGui.Bullet lscm - Shows all custom categories and the mods in them",
                string.Empty, color);
            ConsoleWindow.send_input("ImGui.Separator", string.Empty, color);
            ConsoleWindow.send_input("Advanced", string.Empty, color);
            ConsoleWindow.send_input("ImGui.Separator", string.Empty, color);
            ConsoleWindow.send_input(
                "ImGui.Bullet save_mod [name] [category] | Saves the code from the script editor as a custom mod",
                string.Empty, color);
            ConsoleWindow.send_input("ImGui.Bullet copy_pos | Copies your current horse position to the clipboard",
                string.Empty, color);
            ConsoleWindow.send_input("ImGui.Bullet styling (...) | Allows you to add custom buttons etc to custom mods",
                string.Empty, color);
        }
    }
}