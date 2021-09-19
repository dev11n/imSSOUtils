using System.Drawing;
using System.Threading;
using imSSOUtils.adapters;
using imSSOUtils.window.windows;

namespace imSSOUtils.command.commands
{
    /// <summary>
    /// Enables modding.
    /// </summary>
    internal struct ReadyCommand : ICommand
    {
        /// <summary>
        /// Execute the command.
        /// </summary>
        /// <param name="args"></param>
        void ICommand.push(string[] args)
        {
            if (MemoryAdapter.is_enabled()) return;
            ConsoleWindow.send_input("starting (reflections)", "[alpine internal]", Color.White);
            new Thread(async () => await MemoryAdapter.patch_memory()).Start();
        }
    }
}