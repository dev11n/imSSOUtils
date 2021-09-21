using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
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
                ConsoleWindow.send_input("loading, this might take a while so have patience", "[developer]",
                    Color.White);
                new Thread(() =>
                {
                    for (var i = 0; i < 20; i++)
                    {
                        ConsoleWindow.send_input($"value: {PXInternal.get_child_name("CurrentHorse", i)}",
                            "[developer]",
                            Color.White);
                    }

                    ConsoleWindow.send_input("finished!", "[developer]", Color.White);
                }).Start();
            }
            catch (Exception e)
            {
                Program.write_crash(e);
            }
        }
    }
}