using System;
using System.Diagnostics;
using System.Drawing;
using imSSOUtils.adapters;
using imSSOUtils.registers;
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
                //LL_PlayerStatsWindow.fetch_updates();
                MemoryAdapter.replace_all(LowLevelRegister.start_player_sheet,
                    " global/TempString.SetDataString(\"OP CS OPEN SHEET\");                  ");
                ConsoleWindow.send_input("done replacing", "[developer]", Color.White);
            }
            catch (Exception e)
            {
                Program.write_crash(e);
            }
        }
    }
}