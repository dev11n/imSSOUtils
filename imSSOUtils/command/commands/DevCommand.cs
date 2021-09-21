using System;
using System.Diagnostics;
using imSSOUtils.adapters;
using imSSOUtils.adapters.low_level;

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
                CVar.write_cvar01("global/MoorlandStable.GetName()", "String");
            }
            catch (Exception e)
            {
                Program.write_crash(e);
            }
        }
    }
}