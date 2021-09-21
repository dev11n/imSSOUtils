using System.Diagnostics;

namespace imSSOUtils.adapters
{
    /// <summary>
    /// Extremely basic process utilities.
    /// </summary>
    internal struct ProcessAdapter
    {
        /// <summary>
        /// Check if a Process is active.
        /// </summary>
        /// <param name="name">The Process name (Case-sensitive)</param>
        /// <returns>True if it's active, false if it isn't.</returns>
        public static bool IsProcessAlive(string name) =>
            Process.GetProcessesByName(name).Length is not 0;

        /// <summary>
        /// Get the first found Process specified by it's name.
        /// </summary>
        /// <param name="name">The Process name.</param>
        /// <returns>The Process if any is found.</returns>
        public static Process GetProcess(string name) => Process.GetProcessesByName(name)[0];
    }
}