using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;

namespace CNLibrary
{
    public struct Utils
    {
        #region Variables
        /// <summary>
        /// All the modules.
        /// </summary>
        private static readonly Dictionary<string, IntPtr> modules = new();

        /// <summary>
        /// The process collected from <see cref="cache_modules"/>.
        /// </summary>
        private static Process theProc;

        /// <summary>
        /// Determines whether we are using x64 functionality or not.
        /// </summary>
        private static bool win64;
        #endregion

        /// <summary>
        /// Returns the value of <see cref="win64"/>
        /// </summary>
        /// <returns></returns>
        public static bool is_64() => win64;

        /// <summary>
        /// Forces x64 functionality to be used.
        /// <para>EXPERIMENTAL</para>
        /// </summary>
        /// <param name="_64"></param>
        public static void set_64(bool _64) => win64 = _64;

        /// <summary>
        /// Cache modules from a specific process.
        /// <para>If the process isn't found, the function returns.</para>
        /// </summary>
        /// <param name="processName">The process name.</param>
        // ? Source: Memory.dll
        internal static void cache_modules(string processName)
        {
            if (Process.GetProcessesByName(processName).Length is 0) return;
            modules.Clear();
            theProc = Process.GetProcessesByName(processName)[0];
            for (var i = 0; i < theProc.Modules.Count; i++)
            {
                var Module = theProc.Modules[i];
                if (!string.IsNullOrEmpty(Module.ModuleName) && !modules.ContainsKey(Module.ModuleName))
                    modules.Add(Module.ModuleName, Module.BaseAddress);
            }
        }

        /// <summary>
        /// Get a byte-array from a string. Such as: "0x00, 0x00, 0x00" can be converted into a byte-array.
        /// </summary>
        /// <param name="input">The input which should contain an array of bytes.</param>
        /// <param name="separator">The separator to be used. If it's formatted like "0x00 0x00" then it's space.</param>
        public static byte[] get_bytes(string input, char separator) =>
            Array.ConvertAll(input.Replace("0x", string.Empty).Split(separator), b => Convert.ToByte(b, 16));

        /// <summary>
        /// Convert code with offsets to the real address.
        /// <para>For example: Notepad.exe+000000,0x00,0x00,0x00 will be converted to the real address.</para>
        /// <para>This should only be run after the process has been opened.</para>
        /// <para>For best performance, cache the result if you plan on reusing it a lot of times.</para>
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        // ? Source: Memory.dll
        public static UIntPtr get_real_address(string code)
        {
            if (is_64()) return get_real_address_64(code);
            var mainModule = theProc.MainModule;
            if (mainModule is null) return UIntPtr.Zero;
            var nameCopy = code;
            const char comma = ',', plus = '+';
            const short size = 8;
            // If nameCopy is just empty, return just a 0 in UIntPtr
            if (nameCopy is "") return UIntPtr.Zero;

            // If nameCopy contains spaces, remove them
            if (nameCopy.Contains(' ')) nameCopy = nameCopy.Replace(" ", string.Empty);

            // There's no offsets, convert the address to UIntPtr and return it
            if (!nameCopy.Contains(plus) && !nameCopy.Contains(comma))
                return new UIntPtr(Convert.ToUInt32(nameCopy, 16));

            // Copy nameCopy as there's offsets in it
            var updatedOffsets = nameCopy;

            // Offsets found! (name)+(obj) (...)
            if (nameCopy.Contains(plus)) updatedOffsets = nameCopy[(nameCopy.IndexOf(plus) + 1)..];

            // Create a new byte-array with a size of 8
            var memoryAddress = new byte[size];

            // Does it contain a comma?
            if (updatedOffsets.Contains(comma))
            {
                // Create a list which will store the offsets
                var offsetsList = new List<int>();

                // Get all the items, split by a comma
                var split = updatedOffsets.Split(comma);
                // Loop through all the offsets
                foreach (var currentOffset in split)
                {
                    // A copy of currentOffset, which then replaces any "0x" with an empty string
                    var currentOffsetCopy = currentOffset.Replace("0x", string.Empty);
                    // An integer that's going to store the value of "currentOffsetCopy"
                    int preParse;
                    if (!currentOffset.Contains('-'))
                        // It doesn't contain a minus, so parse it directly
                        preParse = int.Parse(currentOffsetCopy, NumberStyles.AllowHexSpecifier);
                    else
                    {
                        // It contains a minus, replace it with an empty string, parse it and then change the value
                        currentOffsetCopy = currentOffsetCopy.Replace("-", "");
                        preParse = int.Parse(currentOffsetCopy, NumberStyles.AllowHexSpecifier);
                        preParse *= -1;
                    }

                    // Add the offset to the offsetsList
                    offsetsList.Add(preParse);
                }

                // All offsets converted into an integer-list
                var offsets = offsetsList.ToArray();

                // Check if nameCopy contains "base" or "main"
                if (nameCopy.Contains("base") || nameCopy.Contains("main"))
                    /*
                     * It did, so read the BaseAddress combined with the main offset [0] / First -> (address)
                     * Then output it to memoryAddress
                     */
                    Internals.ReadProcessMemory(theProc.Handle, (UIntPtr) ((int) mainModule.BaseAddress + offsets[0]),
                        memoryAddress,
                        (UIntPtr) size, IntPtr.Zero);
                // If nameCopy doesn't contain "base" or "main", then make sure it doesn't contain them again but only plus before continuing
                else if (!nameCopy.Contains("base") && !nameCopy.Contains("main") && nameCopy.Contains(plus))
                {
                    /*
                     * Split by plus and get the first entry, in this case: the module name
                     * An example: ProcessName.exe+Address
                     * Taking 0 as the index would return "ProcessName.exe" as its the first entry
                     */
                    var moduleName = nameCopy.Split(plus)[0];
                    // This will hold the parsed module/process value (string) 
                    var parsedModule = IntPtr.Zero;
                    // Make all alphabetic characters lowercase
                    var lower = moduleName.ToLower();
                    // If the moduleName doesn't contain ".dll", ".exe" nor ".exe", continue
                    if (!lower.Contains(".dll") && !lower.Contains(".exe") && !lower.Contains(".bin"))
                    {
                        // Copy moduleName into moduleTarget
                        var moduleTarget = moduleName;
                        // Replace "0x" with an empty string
                        moduleTarget = moduleTarget.Replace("0x", string.Empty);
                        // Parse the value of moduleTarget into parsedModule
                        parsedModule = (IntPtr) int.Parse(moduleTarget, NumberStyles.HexNumber);
                    }
                    else
                    {
                        try
                        {
                            // Set the value of parsedModule to what it is in modules -> Index -> moduleName
                            parsedModule = modules[moduleName];
                        }
                        catch
                        {
                            // An error has occured, print information
                            Debug.WriteLine($"Module {moduleName[0]} was not found in the module list!");
                            Debug.WriteLine($"Modules: {string.Join(comma, modules)}");
                        }
                    }

                    // Read the process memory, with a baseAddress of parsedModule + offsets[0] (a.k.a the first offset)
                    Internals.ReadProcessMemory(theProc.Handle, (UIntPtr) ((int) parsedModule + offsets[0]),
                        memoryAddress,
                        (UIntPtr) size,
                        IntPtr.Zero);
                }
                else
                    // It didn't contain a plus, so read the first offset and output it to memoryAddress
                    Internals.ReadProcessMemory(theProc.Handle, (UIntPtr) (offsets[0]), memoryAddress, (UIntPtr) size,
                        IntPtr.Zero);

                // Convert the value of memoryAddress to an unsigned integer
                var uint32_memAdd = BitConverter.ToUInt32(memoryAddress, 0);
                // The new base address to use
                var newBaseAdd = (UIntPtr) 0;

                // Loop over all offsets
                for (short i = 1; i < offsets.Length; i++)
                {
                    // Set the value of newBaseAdd a new unsigned integer pointer with the value of uint32_memAdd + the current offset
                    newBaseAdd = new UIntPtr(Convert.ToUInt32(uint32_memAdd + offsets[i]));
                    // Read the process memory with newBaseAdd as the base address, then output it to memoryAddress
                    Internals.ReadProcessMemory(theProc.Handle, newBaseAdd, memoryAddress, (UIntPtr) size, IntPtr.Zero);
                    // Set the value of uint32_memAdd to an unsigned integer version of memoryAddress
                    uint32_memAdd = BitConverter.ToUInt32(memoryAddress, 0);
                }

                // Return the new base address
                return newBaseAdd;
            }

            // Brackets to allow for variables of the same name as above, without modifying their values
            {
                // Convert the value of updatedOffsets to a new integer
                var trueCode = Convert.ToInt32(updatedOffsets, 16);
                // A lowercase version of nameCopy
                var lower = nameCopy.ToLower();
                // The new base address to be used
                var baseAddress = IntPtr.Zero;
                // If lower contains "base" or "main", set the value of baseAddress to the mainModule's Base Address
                if (lower.Contains("base") || lower.Contains("main"))
                    baseAddress = mainModule.BaseAddress;
                // If it doesn't contain "base" nor "main", but it contains a plus, then continue
                else if (!nameCopy.ToLower().Contains("base") && !nameCopy.ToLower().Contains("main") &&
                         nameCopy.Contains("+"))
                {
                    // Get the module name
                    var moduleName = nameCopy.Split('+')[0];
                    // A lowercase version of moduleName
                    var moduleLower = moduleName.ToLower();
                    // Make sure moduleLower doesn't contain ".dll", ".exe" or ".bin" before continuing
                    if (!moduleLower.Contains(".dll") && !moduleLower.Contains(".exe") && !moduleLower.Contains(".bin"))
                    {
                        // A copy of moduleName
                        var moduleCopy = moduleName;
                        // Replace "0x" in moduleCopy with an empty string
                        moduleCopy = moduleCopy.Replace("0x", string.Empty);
                        // Set the value of baseAddress to a integer-parsed version of moduleCopy
                        baseAddress = (IntPtr) int.Parse(moduleCopy, NumberStyles.HexNumber);
                    }
                    // If it contains either "base" or "main" and no plus
                    else
                    {
                        try
                        {
                            // Set the value of baseAddress to what it is in modules -> Index -> moduleName
                            baseAddress = modules[moduleName];
                        }
                        catch
                        {
                            // An error has occured, print information
                            Debug.WriteLine($"Module {moduleName[0]} was not found in module list!");
                            Debug.WriteLine($"Modules: {string.Join(comma, modules)}");
                        }
                    }
                }
                else
                    // Otherwise just split by plus and get the first entry
                    baseAddress = modules[nameCopy.Split(plus)[0]];

                // Finally, return the result.
                return (UIntPtr) ((int) baseAddress + trueCode);
            }
        }

        /// <summary>
        /// <para>x64 overload of <c>get_real_address</c></para>
        /// Convert code with offsets to the real address.
        /// <para>For example: Notepad.exe+000000,0x00,0x00,0x00 will be converted to the real address.</para>
        /// <para>This should only be run after the process has been opened.</para>
        /// <para>For best performance, cache the result if you plan on reusing it a lot of times.</para>
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        // ? Source: Memory.dll
        public static UIntPtr get_real_address_64(string code)
        {
            var mainModule = theProc.MainModule;
            if (mainModule is null) return UIntPtr.Zero;
            var nameCopy = code;
            const char comma = ',', plus = '+';
            const short size = 16;
            // If nameCopy is just empty, return just a 0 in UIntPtr
            if (nameCopy is "") return UIntPtr.Zero;

            // If nameCopy contains spaces, remove them
            if (nameCopy.Contains(' ')) nameCopy = nameCopy.Replace(" ", string.Empty);

            // There's no offsets, convert the address to UIntPtr and return it
            if (!nameCopy.Contains(plus) && !nameCopy.Contains(comma))
                return new UIntPtr(Convert.ToUInt64(nameCopy, 16));

            // Copy nameCopy as there's offsets in it
            var updatedOffsets = nameCopy;

            // Offsets found! (name)+(obj) (...)
            if (nameCopy.Contains(plus)) updatedOffsets = nameCopy[(nameCopy.IndexOf(plus) + 1)..];

            // Create a new byte-array with a size of 8
            var memoryAddress = new byte[size];

            // Does it contain a comma?
            if (updatedOffsets.Contains(comma))
            {
                // Create a list which will store the offsets
                var offsetsList = new List<long>();

                // Get all the items, split by a comma
                var split = updatedOffsets.Split(comma);
                // Loop through all the offsets
                foreach (var currentOffset in split)
                {
                    // A copy of currentOffset, which then replaces any "0x" with an empty string
                    var currentOffsetCopy = currentOffset.Replace("0x", string.Empty);
                    // An integer that's going to store the value of "currentOffsetCopy"
                    long preParse;
                    if (!currentOffset.Contains('-'))
                        // It doesn't contain a minus, so parse it directly
                        preParse = int.Parse(currentOffsetCopy, NumberStyles.AllowHexSpecifier);
                    else
                    {
                        // It contains a minus, replace it with an empty string, parse it and then change the value
                        currentOffsetCopy = currentOffsetCopy.Replace("-", "");
                        preParse = long.Parse(currentOffsetCopy, NumberStyles.AllowHexSpecifier);
                        preParse *= -1;
                    }

                    // Add the offset to the offsetsList
                    offsetsList.Add(preParse);
                }

                // All offsets converted into an integer-list
                var offsets = offsetsList.ToArray();

                // Check if nameCopy contains "base" or "main"
                if (nameCopy.Contains("base") || nameCopy.Contains("main"))
                    /*
                     * It did, so read the BaseAddress combined with the main offset [0] / First -> (address)
                     * Then output it to memoryAddress
                     */
                    Internals.ReadProcessMemory(theProc.Handle, (UIntPtr) ((long) mainModule.BaseAddress + offsets[0]),
                        memoryAddress,
                        (UIntPtr) size, IntPtr.Zero);
                // If nameCopy doesn't contain "base" or "main", then make sure it doesn't contain them again but only plus before continuing
                else if (!nameCopy.Contains("base") && !nameCopy.Contains("main") && nameCopy.Contains(plus))
                {
                    /*
                     * Split by plus and get the first entry, in this case: the module name
                     * An example: ProcessName.exe+Address
                     * Taking 0 as the index would return "ProcessName.exe" as its the first entry
                     */
                    var moduleName = nameCopy.Split(plus)[0];
                    // This will hold the parsed module/process value (string) 
                    var parsedModule = IntPtr.Zero;
                    // Make all alphabetic characters lowercase
                    var lower = moduleName.ToLower();
                    // If the moduleName doesn't contain ".dll", ".exe" nor ".exe", continue
                    if (!lower.Contains(".dll") && !lower.Contains(".exe") && !lower.Contains(".bin"))
                    {
                        // Copy moduleName into moduleTarget
                        var moduleTarget = moduleName;
                        // Replace "0x" with an empty string
                        moduleTarget = moduleTarget.Replace("0x", string.Empty);
                        // Parse the value of moduleTarget into parsedModule
                        parsedModule = (IntPtr) long.Parse(moduleTarget, NumberStyles.HexNumber);
                    }
                    else
                    {
                        try
                        {
                            // Set the value of parsedModule to what it is in modules -> Index -> moduleName
                            parsedModule = modules[moduleName];
                        }
                        catch
                        {
                            // An error has occured, print information
                            Debug.WriteLine($"Module {moduleName[0]} was not found in the module list!");
                            Debug.WriteLine($"Modules: {string.Join(comma, modules)}");
                        }
                    }

                    // Read the process memory, with a baseAddress of parsedModule + offsets[0] (a.k.a the first offset)
                    Internals.ReadProcessMemory(theProc.Handle, (UIntPtr) ((long) parsedModule + offsets[0]),
                        memoryAddress,
                        (UIntPtr) size,
                        IntPtr.Zero);
                }
                else
                    // It didn't contain a plus, so read the first offset and output it to memoryAddress
                    Internals.ReadProcessMemory(theProc.Handle, (UIntPtr) (offsets[0]), memoryAddress, (UIntPtr) size,
                        IntPtr.Zero);

                // Convert the value of memoryAddress to an unsigned integer
                var int64_memAdd = BitConverter.ToInt64(memoryAddress, 0);
                // The new base address to use
                var newBaseAdd = (UIntPtr) 0;

                // Loop over all offsets
                for (var i = 1; i < offsets.Length; i++)
                {
                    // Set the value of newBaseAdd a new unsigned integer pointer with the value of uint32_memAdd + the current offset
                    newBaseAdd = new UIntPtr(Convert.ToUInt64(int64_memAdd + offsets[i]));
                    // Read the process memory with newBaseAdd as the base address, then output it to memoryAddress
                    Internals.ReadProcessMemory(theProc.Handle, newBaseAdd, memoryAddress, (UIntPtr) size, IntPtr.Zero);
                    // Set the value of uint32_memAdd to an unsigned integer version of memoryAddress
                    int64_memAdd = BitConverter.ToInt64(memoryAddress, 0);
                }

                // Return the new base address
                return newBaseAdd;
            }

            // Brackets to allow for variables of the same name as above, without modifying their values
            {
                // Convert the value of updatedOffsets to a new integer
                var trueCode = Convert.ToInt64(updatedOffsets, 16);
                // A lowercase version of nameCopy
                var lower = nameCopy.ToLower();
                // The new base address to be used
                var baseAddress = IntPtr.Zero;
                // If lower contains "base" or "main", set the value of baseAddress to the mainModule's Base Address
                if (lower.Contains("base") || lower.Contains("main"))
                    baseAddress = mainModule.BaseAddress;
                // If it doesn't contain "base" nor "main", but it contains a plus, then continue
                else if (!nameCopy.ToLower().Contains("base") && !nameCopy.ToLower().Contains("main") &&
                         nameCopy.Contains("+"))
                {
                    // Get the module name
                    var moduleName = nameCopy.Split('+')[0];
                    // A lowercase version of moduleName
                    var moduleLower = moduleName.ToLower();
                    // Make sure moduleLower doesn't contain ".dll", ".exe" or ".bin" before continuing
                    if (!moduleLower.Contains(".dll") && !moduleLower.Contains(".exe") && !moduleLower.Contains(".bin"))
                    {
                        // A copy of moduleName
                        var moduleCopy = moduleName;
                        // Replace "0x" in moduleCopy with an empty string
                        moduleCopy = moduleCopy.Replace("0x", string.Empty);
                        // Set the value of baseAddress to a integer-parsed version of moduleCopy
                        baseAddress = (IntPtr) long.Parse(moduleCopy, NumberStyles.HexNumber);
                    }
                    // If it contains either "base" or "main" and no plus
                    else
                    {
                        try
                        {
                            // Set the value of baseAddress to what it is in modules -> Index -> moduleName
                            baseAddress = modules[moduleName];
                        }
                        catch
                        {
                            // An error has occured, print information
                            Debug.WriteLine($"Module {moduleName[0]} was not found in module list!");
                            Debug.WriteLine($"Modules: {string.Join(comma, modules)}");
                        }
                    }
                }
                else
                    // Otherwise just split by plus and get the first entry
                    baseAddress = modules[nameCopy.Split(plus)[0]];

                // Finally, return the result.
                return (UIntPtr) ((long) baseAddress + trueCode);
            }
        }
    }
}