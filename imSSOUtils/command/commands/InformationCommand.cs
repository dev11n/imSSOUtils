using System.Diagnostics;
using System.Drawing;
using imSSOUtils.window.windows;

namespace imSSOUtils.command.commands
{
    /// <summary>
    /// Displays the current SSOUtils version along with other info.
    /// </summary>
    internal struct InformationCommand : ICommand
    {
        #region Variables
        /// <summary>
        /// The current version.
        /// </summary>
        public const string version = "master_release_031";
        #endregion

        /// <summary>
        /// Execute the command.
        /// </summary>
        /// <param name="args"></param>
        void ICommand.push(string[] args)
        {
            var color = Color.White;
            ConsoleWindow.send_input("ImGui.Separator", string.Empty, color);
            ConsoleWindow.send_input("Version", string.Empty, color);
            ConsoleWindow.send_input("ImGui.Separator", string.Empty, color);
            ConsoleWindow.send_input($"ImGui.Bullet ssoutils-{version}", string.Empty, color);
            ConsoleWindow.send_input("ImGui.Bullet alpine v2 (experimental)", string.Empty, color);
            ConsoleWindow.send_input("ImGui.Separator", string.Empty, color);
            ConsoleWindow.send_input("Assembly", string.Empty, color);
            ConsoleWindow.send_input("ImGui.Separator", string.Empty, color);
            ConsoleWindow.send_input("ImGui.Bullet .NET 5.0", string.Empty, color);
            ConsoleWindow.send_input("ImGui.Bullet C# 9.0", string.Empty, color);
            ConsoleWindow.send_input("ImGui.Bullet WIAP", string.Empty, color);
            if (!Debugger.IsAttached) return;
            ConsoleWindow.send_input("a debugger is attached", "[debugger]", Color.OrangeRed);
        }
    }
}