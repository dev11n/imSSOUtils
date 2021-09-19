using System.Threading;
using imSSOUtils.adapters;

namespace imSSOUtils.mod.mods.Visual
{
    /// <summary>
    /// Clone the selected horse (must be purchasable)
    /// </summary>
    internal class Horse_Clone : IMod
    {
        #region Variables
        /// <summary>
        /// Determine whether this is running or not.
        /// </summary>
        private bool isRunning;
        #endregion

        /// <summary>
        /// Execute the mod.
        /// </summary>
        protected internal override void on_trigger()
        {
            if (isRunning)
            {
                isRunning = false;
                modTimer.Dispose();
            }
            else
                modTimer = new Timer(_ =>
                {
                    isRunning = true;
                    alpine_execute(PXShort.p_if("Game->GameUI::GetActive() == 1",
                        "SetHorseAppearance(Game->SelectedHorseForSale::GetHorseDataID());"));
                }, null, 0, 1500);
        }

        /// <summary>
        /// Called when the instance has been created.
        /// </summary>
        protected internal override void on_initialize() { }
    }
}