using System;
using System.Diagnostics;
using System.Drawing;
using imSSOUtils.adapters;
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
            if (!MemoryAdapter.is_enabled() || !Debugger.IsAttached) return;
            try
            {
                ConsoleWindow.send_input($"count: {PXInternal.get_child_count("CurrentHorse")}", "[developer]",
                    Color.White);
            }
            catch (Exception e)
            {
                Program.write_crash(e);
            }
        }
    }
}