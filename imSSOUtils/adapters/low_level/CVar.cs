using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static System.String;
using static imSSOUtils.registers.InternalRegister;

namespace imSSOUtils.adapters.low_level
{
    public readonly struct CVar
    {
        #region Variables
        /// <summary>
        /// Addresses for CVars.
        /// </summary>
        public static readonly List<string> directAddresses01 = new(), directAddresses02 = new();

        /// <summary>
        /// Addresses for accessing a variables data.
        /// </summary>
        private static string directAddress01 = Empty, directAddress02 = Empty;

        /// <summary>
        /// Determines whether we have cached a specific CVar or not.
        /// </summary>
        public static bool hasCached01, hasCached02;
        #endregion

        /// <summary>
        /// Read from CVar
        /// </summary>
        /// <returns></returns>
        public static string read_cvar_string() => hasCached01
            ? MemoryAdapter.head.get_consult().Memory.read_string(directAddress01, 128)
            : "NOT CACHED!";

        /// <summary>
        /// Read from CVar
        /// </summary>
        /// <returns></returns>
        public static int read_cvar_int()
        {
            try
            {
                // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
                Convert.ToInt32(read_cvar_string());
            }
            catch (Exception)
            {
                // bug: we have to ignore this once, then continue as cvar isn't instant.
                // bug: this should really just be fixed by reading cvar_string twice and ignoring the first result.
            }

            return hasCached01 ? Convert.ToInt32(read_cvar_string()) : -1;
        }

        /// <summary>
        /// Write to CVar
        /// </summary>
        /// <param name="data">Data to be written</param>
        /// <param name="type">The type (String, Int, etc) - Case-Sensitive</param>
        public static void write_cvar(string data, string type) =>
            MemoryAdapter.head.inject_code($"global/TempString.SetData{type}({data});");

        /// <summary>
        /// Cache CVar
        /// </summary>
        public static async Task cache_cvar()
        {
            if (hasCached01) return;
            // ? Do this twice because SSO has brain issues.
            for (var i = 0; i < 4; i++)
            {
                directAddresses01.Clear();
                foreach (var address in await MemoryAdapter.head.aob_scan(direct, true))
                    directAddresses01.Add($"0x{address:X}");
                foreach (var address in directAddresses01)
                    if (MemoryAdapter.head.get_consult().Memory.read_string(address).StartsWith(direct_raw))
                        directAddress01 = address;
            }

            hasCached01 = directAddress01.Length > 2;
        }

        public static async Task setup_cvar()
        {
            MemoryAdapter.head.inject_code(
                $"global/QuestCollectCompleteWindow/Script/sText.GlobalAccessShortcut(\"TempString\");\nglobal/TempString.SetDataString(\"{direct_raw}\");");
            await Task.Delay(300);
            await cache_cvar();
        }
    }
}