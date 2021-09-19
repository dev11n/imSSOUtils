using System.Drawing;
using System.Linq;
using imSSOUtils.adapters;
using imSSOUtils.window.windows;

namespace imSSOUtils.command.commands
{
    /// <summary>
    /// List CMods command.
    /// </summary>
    internal struct LSCMCommand : ICommand
    {
        /// <summary>
        /// Execute the command.
        /// </summary>
        /// <param name="args"></param>
        void ICommand.push(string[] args)
        {
            var color = Color.White;
            var cat = Alpine.get_cmods_categories();
            if (cat.Count() is 0)
            {
                ConsoleWindow.send_input("no categories or custom mods were found", "[lscm]", color);
                return;
            }

            foreach (var category in Alpine.get_cmods_categories())
            {
                var cmods = Alpine.get_cmods(category);
                ConsoleWindow.send_input("ImGui.Separator", string.Empty, color);
                ConsoleWindow.send_input(category, string.Empty, color);
                ConsoleWindow.send_input("ImGui.Separator", string.Empty, color);
                for (short j = 0; j < cmods.Count; j++)
                    ConsoleWindow.send_input($"ImGui.Bullet {cmods[j].name}", string.Empty, color);
            }
        }
    }
}