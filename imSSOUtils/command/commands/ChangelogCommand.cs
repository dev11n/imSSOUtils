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
            ConsoleWindow.send_input("ImGui.Bullet Added a new mod, \"UI Enhancements\" - Alpha", empty, color);
            ConsoleWindow.send_input("ImGui.Bullet Added internal support for reading in-game data", empty, color);
            ConsoleWindow.send_input(
                "ImGui.Bullet Added a new window, \"Data Viewer\" - Enter for example: Game->MoorlandStable to see the child objects of it",
                empty, color);
            ConsoleWindow.send_input("ImGui.Bullet Added a global font which is used by default, \"Comfortaa\"", empty,
                color);
            ConsoleWindow.send_input("ImGui.Bullet Improved overall performance", empty, color);
            ConsoleWindow.send_input("ImGui.Bullet SSOUtils is now x64, supporting Star Stable", empty, color);
            ConsoleWindow.send_input("ImGui.Bullet Cinematica now supports a higher max speed value", empty, color);
            ConsoleWindow.send_input("ImGui.Bullet Updated Save Manager", empty, color);
            ConsoleWindow.send_input(
                "ImGui.Bullet Removed the mod \"Still Camera\" due to it being extremely inefficient and outdated",
                empty, color);
            ConsoleWindow.send_input("ImGui.Separator", empty, color);
            ConsoleWindow.send_input("Alpine / Scripting", empty, color);
            ConsoleWindow.send_input("ImGui.Separator", empty, color);
            ConsoleWindow.send_input(
                "ImGui.Bullet OTFE Updated to v1.1, adding support for:\n- x64\n- Reading game-data\n- Faster response times",
                empty, color);
            ConsoleWindow.send_input(
                "ImGui.Bullet Added a new function to scripting, \"ForceKill();\" which can be used to forcefully kill an object\nPlease note that the specific object you kill won't be available until you restart!\nCrashes may also occur.",
                empty, color);
        }
    }
}