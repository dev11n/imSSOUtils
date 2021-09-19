using System.Threading;
using imSSOUtils.adapters;

namespace imSSOUtils.mod.mods.Visual
{
    /// <summary>
    /// WIP - Improves SSOs current UI
    /// </summary>
    internal class UI_Enhancements : IMod
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
                    PXInternal.draw_current_location();
                }, null, 0, 2000);
        }

        /// <summary>
        /// Called when the instance has been created.
        /// </summary>
        protected internal override void on_initialize() { }
    }
}