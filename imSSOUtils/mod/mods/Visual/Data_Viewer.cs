using System;
using imSSOUtils.adapters;

namespace imSSOUtils.mod.mods.Visual
{
    /// <summary>
    /// Displays data from the game, such as integers and strings.
    /// </summary>
    internal class Data_Viewer : IMod
    {
        /// <summary>
        /// Not used in this mod, instead we are using the options.
        /// </summary>
        protected internal override void on_trigger() { }

        /// <summary>
        /// Initialize all the options.
        /// </summary>
        protected internal override void on_initialize()
        {
            try
            {
                add_input_text("Script");
                
            }
            catch (Exception)
            {
                // ignored
            }
        }
    }
}