using System.Threading;
using imSSOUtils.adapters;
using static imSSOUtils.adapters.PXInternal.HorseState;

namespace imSSOUtils.mod.mods.Movement
{
    /// <summary>
    /// Extends the jumping length via Physics::SetRelativeLinearVelocity
    /// </summary>
    internal class Longjump : IMod
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
                    if (PXInternal.get_horse_state() is not JUMPING) return;
                    alpine_execute("CurrentHorse::SetRelativeLinearVelocity(0, floatSlider_Vertic._Force, floatSlider_Horiz._Force);");
                }, null, 0, 200);
        }

        /// <summary>
        /// Called when the instance has been created.
        /// </summary>
        protected internal override void on_initialize()
        {
            add_float_slider("Vertic._Force", .1f, 20, 10);
            add_float_slider("Horiz._Force", 2, 40, 16);
        }
    }
}