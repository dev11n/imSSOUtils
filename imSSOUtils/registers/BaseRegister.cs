namespace imSSOUtils.registers
{
    /// <summary>
    /// An internal register with commonly used values.
    /// </summary>
    public struct BaseRegister
    {
        /// <summary>
        /// Constant strings which represents various common names related to processes.
        /// </summary>
        public const string sentry_rt_name = "crashpad_handler", runtime = "SSOClient", runtime_title = "Star Stable";

        /*
         * Legacy Hex:
         * noFall = "48 6F 72 73 65 4D 6F 76 65 54 6F 4C 61 73 74 56 61 6C 69 64 50 6F 73 69 74 69 6F 6E 28 29 3B"
         * mp_highlight = "67 6C 6F 62 61 6C 2F 4D 61 70 57 69 6E 64 6F 77 2F 48 69 67 68 6C 69 67 68 74 4D 6F 64 65 2E 53 74 61 72 74 28 29 3B"
         * loginMenu_legacy = "67 6C 6F 62 61 6C 2F 4C 6F 67 69 6E 4D 65 6E 75 2E 53 74 61 72 74 28 29 3B"
         */

        public static bool useV2AlpineInject = false;

        /// <summary>
        /// HEX strings.
        /// </summary>
        // ! Readonly instead of const so obfuscators obfuscate it
        public static readonly string mp =
            "67 6C 6F 62 61 6C 2F 4D 61 70 57 69 6E 64 6F 77 2E 53 74 61 72 74 28 29 3B 0D 0A 7D 0D 0A 0D 0A 2F 2F 20 4D";
    }
}