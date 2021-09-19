namespace imSSOUtils.window
{
    /// <summary>
    /// Interface which connects all windows and calls the draw() function afterwards.
    /// </summary>
    internal abstract class IWindow
    {
        #region Variables
        /// <summary>
        /// Determines whether this window should be displayed or not.
        /// </summary>
        protected internal bool shouldDisplay = true;

        /// <summary>
        /// The window identifier so it can be found easier via <see cref="Program.get_by_name"/>
        /// </summary>
        protected internal string identifier;
        #endregion

        /// <summary>
        /// Draw the window.
        /// </summary>
        protected internal abstract void draw();

        /// <summary>
        /// Initialize the window.
        /// </summary>
        protected internal abstract void initialize();
    }
}