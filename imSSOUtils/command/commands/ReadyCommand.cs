using System.Drawing;
using System.Threading;
using CNLibrary;
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
            Utils.set_64(true);
            ConsoleWindow.send_input($"starting (reflections) | x64 >> {Utils.is_64()}", "[alpine internal]",
                Color.White);
            new Thread(async () => await MemoryAdapter.patch_memory()).Start();
        }
    }
}