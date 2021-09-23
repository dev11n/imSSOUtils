namespace imSSOUtils.registers
{
    /// <summary>
    /// This register is for low-level windows which aims to replace SSOs built-in windows.
    /// </summary>
    internal readonly struct LowLevelRegister
    {
        // Most of these hex-values are event-driven.
        public static readonly string start_player_sheet =
                "74 68 69 73 2e 52 75 6e 53 63 72 69 70 74 28 22 43 68 61 72 61 63 74 65 72 53 68 65 65 74 55 70 64 61 74 65 22 29",
            on_player_level_up =
                "67 6c 6f 62 61 6c 2f 4c 65 76 65 6c 55 70 54 65 78 74 57 69 6e 64 6f 77 31 2e 53 65 74 56 69 65 77 54 65 78 74 28 22 4c 45 56 45 4c 55 50 22 29 3b";
    }
}