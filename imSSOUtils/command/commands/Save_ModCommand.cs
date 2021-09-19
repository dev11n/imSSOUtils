using System.Drawing;
using imSSOUtils.adapters;
using imSSOUtils.window.windows;

namespace imSSOUtils.command.commands
{
    /// <summary>
    /// Saves a custom mod.
    /// </summary>
    internal struct Save_ModCommand : ICommand
    {
        /// <summary>
        /// Execute the command.
        /// </summary>
        /// <param name="args"></param>
        void ICommand.push(string[] args)
        {
            if (args is not {Length: 3})
            {
                ConsoleWindow.send_input("not enough arguments! save [name] [category]", "[cmod]", Color.OrangeRed);
                return;
            }

            FileAdapter.create_cmod(args[1], AlpineEDWindow.content, args[2]);
            ConsoleWindow.send_input($"saved cmod as \"{args[1]}\"!", "[cmod]", Color.Lime);
        }
    }
}