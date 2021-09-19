using System.Drawing;
using ImGuiNET;
using imSSOUtils.adapters;
using imSSOUtils.window.windows;

namespace imSSOUtils.command.commands
{
    /// <summary>
    /// Copies the horse coordinates to the clipboard.
    /// </summary>
    internal struct Copy_PosCommand : ICommand
    {
        /// <summary>
        /// Execute the command.
        /// </summary>
        /// <param name="args"></param>
        void ICommand.push(string[] args)
        {
            if (!MemoryAdapter.is_enabled()) return;
            var pos = PXInternal.get_horse_position();
            ImGui.SetClipboardText($"{pos.X}, {pos.Y}, {pos.Z}");
            ConsoleWindow.send_input("copied position to your clipboard!", "[system]", Color.Lime);
        }
    }
}