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
        private readonly string[] test = new[] {"YES", "NO", "MAYBE"};

        /// <summary>
        /// Execute the command.
        /// </summary>
        /// <param name="args"></param>
        void ICommand.push(string[] args)
        {
            if (!MemoryAdapter.is_enabled()) return;
            try
            {
                CVar.write_cvar02(test[new Random().Next(0, test.Length)]);
                ConsoleWindow.send_input($"value: {CVar.read_cvar01_string()}", "[developer]", Color.White);
                ConsoleWindow.send_input($"value: {CVar.read_cvar02_string()}", "[developer]", Color.White);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ConsoleWindow.send_input(e.ToString(), "[fatal error]", Color.White);
            }
        }
    }
}