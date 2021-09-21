using System;
using System.Diagnostics;
using imSSOUtils.adapters;
using imSSOUtils.window.windows.modding.low_level;

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
                LL_PlayerStatsWindow.fetch_updates();
            }
            catch (Exception e)
            {
                Program.write_crash(e);
            }
        }
    }
}