using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
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
            if (!MemoryAdapter.is_enabled() || !Debugger.IsAttached) return;
            try
            {
                new Thread(() =>
                {
                    ConsoleWindow.send_input($"last value: {CVar.read_cvar02_string()}", "[developer]", Color.White);
                    CVar.write_cvar02("HEYYY" + new Random().Next(0, 4214));
                    ConsoleWindow.send_input($"current value: {CVar.read_cvar02_string()}", "[developer]", Color.White);
                }).Start();
            }
            catch (Exception e)
            {
                Program.write_crash(e);
            }
        }
    }
}