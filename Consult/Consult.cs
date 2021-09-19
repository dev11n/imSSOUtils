using System;
using System.Diagnostics;

namespace CNLibrary
{
    /// <summary>
    /// A minimalistic work-in-progress memory library.
    /// </summary>
    public class Consult
    {
        #region Variables
        /// <summary>
        /// The current <see cref="IntPtr"/> of the opened process.
        /// </summary>
        internal static IntPtr currentHandle { get; private set; }

        /// <summary>
        /// Accesses the <see cref="Memory"/> structure.
        /// </summary>
        public Memory Memory { get; } = new();
        #endregion

        /// <summary>
        /// Try to open the specified process.
        /// </summary>
        /// <param name="process">The process you want to open for <c>ProcessAccessFlags.All</c></param>
        /// <param name="access">The access flags you want to use</param>
        /// <returns>A boolean for whether if the process was opened successfully or not</returns>
        public bool open_process(Process process,
            Internals.MinProcessAccessFlags access = Internals.MinProcessAccessFlags.All)
        {
            currentHandle = Internals.OpenProcess(access, false, process.Id);
            Utils.cache_modules(process.ProcessName);
            return currentHandle != IntPtr.Zero;
        }
    }
}