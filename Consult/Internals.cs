using System;
using System.Runtime.InteropServices;

namespace CNLibrary
{
    /// <summary>
    /// Basic Windows API functions
    /// </summary>
    public struct Internals
    {
        #region Enums
        /// <summary>
        /// Scaled-down Process Access Flags enum, only keeping relevant values.
        /// </summary>
        [Flags]
        public enum MinProcessAccessFlags : uint
        {
            All = 2035711,
            VirtualMemoryOperation = 8,
            VirtualMemoryRead = 16,
            VirtualMemoryWrite = 32,
        }
        #endregion
        #region RPM
        /// <summary>
        /// Read from a process' memory
        /// </summary>
        /// <param name="hProcess">The process handle</param>
        /// <param name="lpBaseAddress">Address to read from</param>
        /// <param name="lpBuffer">Buffer / Output variable</param>
        /// <param name="nSize">Amount of bytes to be read</param>
        /// <param name="lpNumberOfBytesRead">Outputs the amount of bytes read (set to "out _" if you don't need it)</param>
        /// <returns></returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool ReadProcessMemory(IntPtr hProcess, UIntPtr lpBaseAddress, byte[] lpBuffer, int nSize,
            out IntPtr lpNumberOfBytesRead);

        /// <summary>
        /// Read from a process' memory
        /// </summary>
        /// <param name="hProcess">The process handle</param>
        /// <param name="lpBaseAddress">Address we should read from</param>
        /// <param name="lpBuffer">Buffer / Output variable</param>
        /// <param name="nSize">Amount of bytes to be read</param>
        /// <param name="lpNumberOfBytesRead">Outputs the amount of bytes read (set to "out _" if you don't need it)</param>
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool ReadProcessMemory(IntPtr hProcess, UIntPtr lpBaseAddress, byte[] lpBuffer, long nSize,
            out IntPtr lpNumberOfBytesRead);

        /// <summary>
        /// Read from a process' memory
        /// </summary>
        /// <param name="hProcess">The process handle</param>
        /// <param name="lpBaseAddress">Address we should read from</param>
        /// <param name="lpBuffer">Buffer / Output variable</param>
        /// <param name="nSize">Amount of bytes to be read</param>
        /// <param name="lpNumberOfBytesRead">Outputs the amount of bytes read (set to "out _" if you don't need it)</param>
        [DllImport("kernel32.dll")] internal static extern bool ReadProcessMemory(IntPtr hProcess,
            UIntPtr lpBaseAddress, byte[] lpBuffer, UIntPtr nSize, IntPtr lpNumberOfBytesRead);
        #endregion
        #region WPM
        /// <summary>
        /// Writes to a process' memory
        /// </summary>
        /// <param name="hProcess">The process handle</param>
        /// <param name="lpBaseAddress">Address we should write to</param>
        /// <param name="lpBuffer">What we should write to the address</param>
        /// <param name="nSize">Size of the type buffer type</param>
        /// <param name="lpNumberOfBytesWritten">Outputs the amount of bytes read (set to IntPtr.Zero if you don't need it)</param>
        [DllImport("kernel32.dll")]
        public static extern bool WriteProcessMemory(IntPtr hProcess, UIntPtr lpBaseAddress, byte[] lpBuffer,
            UIntPtr nSize, IntPtr lpNumberOfBytesWritten);
        #endregion
        #region General
        /// <summary>
        /// Tries to open the specified process
        /// </summary>
        /// <param name="minProcessAccess">The type of access you want</param>
        /// <param name="bInheritHandle">Whether or not we should inherit the handle (recommended value is false)</param>
        /// <param name="processId">The target Process ID</param>
        /// <returns></returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr OpenProcess(MinProcessAccessFlags minProcessAccess, bool bInheritHandle,
            int processId);

        [DllImport("kernel32.dll", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsWow64Process(IntPtr process, out bool wow64Process);
        #endregion
    }
}