using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Numerics;
using System.Threading;
using imSSOUtils.adapters;
using imSSOUtils.cache.objects;
using imSSOUtils.window.windows;

namespace imSSOUtils.command.commands
{
    /// <summary>
    /// Developer command.
    /// </summary>
    internal class DevCommand : ICommand
    {
        private Vector3 mainPos;
        private readonly Dictionary<Vector3, string> objectPos = new();

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
                    var vc = Orientation.get_rotation("CurrentHorse");
                    ConsoleWindow.send_input($"rot: {vc.X}, {vc.Y}, {vc.Z}", "[dev]", Color.White);
                }).Start();
            }
            catch (Exception e)
            {
                Program.write_crash(e);
            }
        }
    }
}