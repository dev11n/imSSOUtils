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
            ConsoleWindow.send_input("ImGui.Bullet Added a new command, write \"styling\" to see the usage(s) of it",
                empty, color);
            ConsoleWindow.send_input(
                "ImGui.Bullet Added an \"Experiments\" section to the \"SSOUtils\" window, use experiments at your own risk",
                empty, color);
            ConsoleWindow.send_input("ImGui.Bullet Fixed an issue where custom textboxes displayed incorrectly", empty,
                color);
            ConsoleWindow.send_input(
                "ImGui.Bullet Added the ability to close sub-windows. To open them again, see SSOUtils -> Window States",
                empty, color);
            ConsoleWindow.send_input("ImGui.Bullet Added a new mod, \"Sky Modifier\" - PREVIEW", empty, color);
            ConsoleWindow.send_input(
                "ImGui.Bullet Removed the \"XP\" Mod, you now have to find your own mod in order to spawn XP", empty,
                color);
            ConsoleWindow.send_input(
                "ImGui.Bullet Added a new mod, \"STFU FadeUp\" which tries to eliminate all fade up/down screens",
                empty, color);
            ConsoleWindow.send_input(
                "ImGui.Bullet Added a new mod, \"MediaMode\" which lets you spoof your player and horse name (only visible for you)",
                empty, color);
            ConsoleWindow.send_input("ImGui.Bullet Added a new window, \"Save Manager\" - PREVIEW", empty, color);
            ConsoleWindow.send_input(
                "ImGui.Bullet Added a new mod, \"Fast Time\" - PREVIEW - Speeds up the time of day cycle\nTo toggle it off, simply press \"Fast Time\" again",
                empty, color);
            ConsoleWindow.send_input("ImGui.Bullet Removed the options from \"NoWarnings\"", empty, color);
            ConsoleWindow.send_input(
                "ImGui.Bullet Improved the overall performance by ~20%%, especially in crowded areas", empty, color);
            ConsoleWindow.send_input("ImGui.Separator", empty, color);
            ConsoleWindow.send_input("Alpine / Scripting", empty, color);
            ConsoleWindow.send_input("ImGui.Separator", empty, color);
            ConsoleWindow.send_input(
                "ImGui.Bullet Switched modding techniques; from minimap -> OTF (On the Fly) - PREVIEW\nThis means that mod code will execute directly as you press the button associated with it",
                empty, color);
            ConsoleWindow.send_input(
                "ImGui.Bullet OTF Modding (if executed in parallel which may become available in custom scripts in the future) can\nexecute up to 2k lines of script code without issues. Only limit is Star Stable's PXScript itself and\nhow fast it keeps up",
                empty, color);
        }
    }
}