using System;
using System.Drawing;
using imSSOUtils.adapters;
using imSSOUtils.adapters.low_level;
using imSSOUtils.window.windows;

namespace imSSOUtils.command.commands
{
    /// <summary>
    /// Developer command.
    /// </summary>
    internal class DevCommand : ICommand
    {
        /// <summary>
        /// Execute the command.
        /// </summary>
        /// <param name="args"></param>
        void ICommand.push(string[] args)
        {
            if (!MemoryAdapter.is_enabled()) return;
            try
            {
                ConsoleWindow.send_input($"value: {CVar.read_cvar_string()}", "[developer]", Color.White);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ConsoleWindow.send_input(e.ToString(), "[fatal error]", Color.White);
            }
        }
    }
}