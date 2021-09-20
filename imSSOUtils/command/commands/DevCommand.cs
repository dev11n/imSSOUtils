using System;
using System.Drawing;
using imSSOUtils.adapters;
using imSSOUtils.adapters.extensions;
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
                ConsoleWindow.send_input($"value: {CVar.read_cvar01_string()}", "[developer]", Color.White);
                foreach (var address in CVar.directAddresses02)
                {
                    ConsoleWindow.send_input($"value: {MemoryAdapter.head.get_consult().Memory.read_string(address).GetUntilOrEmpty("\");")}",
                        "[developer]", Color.White);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ConsoleWindow.send_input(e.ToString(), "[fatal error]", Color.White);
            }
        }
    }
}