using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using imSSOUtils.window.windows;
using static System.String;
using static imSSOUtils.adapters.MemoryAdapter;
using static imSSOUtils.registers.InternalRegister;

namespace imSSOUtils.adapters.low_level
{
    /// <summary>
    /// "Custom Variable" - Modify a variable in SSO in order to fetch in-game data externally.
    /// </summary>
    internal readonly struct CVar
    {
        #region Variables
        /// <summary>
        /// Addresses for accessing a variables data.
        /// </summary>
        public static string directAddress01 = Empty, directAddress02 = Empty;

        /// <summary>
        /// Determines whether we have cached a specific CVar or not.
        /// </summary>
        public static bool hasCached01, hasCached02, hasCachedAll;
        #endregion

        /// <summary>
        /// Read from CVar
        /// </summary>
        /// <returns></returns>
        public static string read_cvar01_string()
        {
            try
            {
                return hasCached01 ? head.get_consult().Memory.read_string(directAddress01, 128) : Empty;
            }
            catch (Exception)
            {
                return Empty;
            }
        }

        /// <summary>
        /// Read from CVar
        /// </summary>
        /// <returns></returns>
        public static string read_cvar02_string() => hasCached02
            ? head.get_consult().Memory.read_string(directAddress02, 128)
            : "NOT CACHED!";

        /// <summary>
        /// Read from CVar
        /// </summary>
        /// <returns></returns>
        public static int read_cvar01_int()
        {
            try
            {
                // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
                Convert.ToInt32(read_cvar01_string());
            }
            catch (Exception)
            {
                // bug: we have to ignore this once, then continue as cvar isn't instant.
                // bug: this should really just be fixed by reading cvar_string twice and ignoring the first result.
            }

            return hasCached01 ? Convert.ToInt32(read_cvar01_string()) : -1;
        }

        /// <summary>
        /// Read from CVar
        /// </summary>
        /// <returns></returns>
        public static uint read_cvar01_uint()
        {
            try
            {
                // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
                Convert.ToUInt32(read_cvar01_string());
            }
            catch (Exception)
            {
                // bug: we have to ignore this once, then continue as cvar isn't instant.
                // bug: this should really just be fixed by reading cvar_string twice and ignoring the first result.
            }

            return hasCached01 ? Convert.ToUInt32(read_cvar01_string()) : 0;
        }

        /// <summary>
        /// Read from CVar
        /// </summary>
        /// <returns></returns>
        public static int read_cvar02_int()
        {
            try
            {
                // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
                Convert.ToInt32(read_cvar02_string());
            }
            catch (Exception)
            {
                // bug: we have to ignore this once, then continue as cvar isn't instant.
                // bug: this should really just be fixed by reading cvar_string twice and ignoring the first result.
            }

            return hasCached02 ? Convert.ToInt32(read_cvar02_string()) : -1;
        }

        /// <summary>
        /// Write to CVar
        /// </summary>
        /// <param name="data">Data to be written</param>
        /// <param name="type">The type to write (Int, String, etc) | Case-sensitive</param>
        /// <param name="pauseThread">For how long the thread should be paused after writing. Set to 0 for no delay.</param>
        public static void write_cvar01(string data, string type, int pauseThread = 10)
        {
            direct_call($"Game->TempString::SetData{type}({data});");
            // If C# 9.0+ is active on all cheats, switch the if-check out to "if (pauseThread is not 0)" as it's easier to read.
            if (pauseThread != 0) Thread.Sleep(pauseThread);
        }

        /// <summary>
        /// Write to CVar
        /// <para>This uses direct writing and can't easily retrieve in-game data like CVar_01.</para>
        /// <para>Only use this for writing whether a mod has been executed or not (0/1).</para>
        /// </summary>
        /// <param name="data">Data to be written</param>
        public static void write_cvar02(string data) =>
            head.get_consult().Memory.write_string(directAddress02, data);

        /// <summary>
        /// Cache CVar
        /// </summary>
        private static async Task cache_cvar01()
        {
            if (hasCached01) return;
            for (var i = 0; i < 4; i++)
            {
                foreach (var address in await head.aob_scan(direct, true))
                {
                    var fixedAddress = $"0x{address:X}";
                    if (head.get_consult().Memory.read_string(fixedAddress, 128) == direct_raw)
                    {
                        directAddress01 = fixedAddress;
                        hasCached01 = true;
                        ConsoleWindow.send_input("cached cvar_01", "[cvar]", Color.White);
                        return;
                    }

                    if (fixedAddress is "" or {Length: < 3})
                        ConsoleWindow.send_input("CAN'T FIND CVAR_01!", "[fatal]", Color.OrangeRed);
                }
            }

            hasCached01 = false;
        }

        /// <summary>
        /// Cache CVar
        /// </summary>
        private static async Task cache_cvar02()
        {
            if (hasCached02) return;
            for (var i = 0; i < 4; i++)
                foreach (var address in await head.aob_scan(direct02, true))
                {
                    var fixedAddress = $"0x{address:X}";
                    if (head.get_consult().Memory.read_string(fixedAddress) == direct02_raw)
                    {
                        directAddress02 = fixedAddress;
                        hasCached02 = true;
                        ConsoleWindow.send_input("cached cvar_02", "[cvar]", Color.White);
                        return;
                    }

                    if (fixedAddress is "" or {Length: < 3})
                        ConsoleWindow.send_input("CAN'T FIND CVAR_02!", "[fatal]", Color.OrangeRed);
                }

            hasCached02 = false;
        }

        /// <summary>
        /// Setup CVar.
        /// </summary>
        public static async Task setup_cvar()
        {
            ConsoleWindow.send_input("caching cvar_01, please do not move", "[cvar]", Color.White);
            for (var i = 0; i < 2; i++)
                // ! We have to set a GAS (GlobalAccessShortcut) so it reads the object of "TempString" once, then it gets cached properly in memory
                direct_call(
                    $"Game->QuestCollectCompleteWindow->Script->sText::GlobalAccessShortcut(\"TempString\");\nGame->TempString::SetDataString(\"{direct_raw}\");",
                    false);
            await Task.Delay(300);
            await cache_cvar01();
            await Task.Delay(50);
            ConsoleWindow.send_input("caching cvar_02, please do not move", "[cvar]", Color.White);
            for (var i = 0; i < 2; i++)
                direct_call(
                    $"Game->CSIInspectView->FailedMessageData::GlobalAccessShortcut(\"TempString2\");\nGame->TempString2::SetDataString(\"{direct02_raw}\");",
                    false);
            await Task.Delay(300);
            await cache_cvar02();
            ConsoleWindow.send_input("finished cvar caching", "[cvar]", Color.White);
            hasCachedAll = hasCached01 && hasCached02;
            if (!hasCachedAll)
                Program.write_crash(
                    new ExternalException($"Invalid CVar states! p_01: {hasCached01} | p_02: {hasCached02}"));
        }
    }
}