namespace imSSOUtils.registers
{
    /// <summary>
    /// This register is for low-level windows which aims to replace SSOs built-in windows.
    /// </summary>
    internal readonly struct LowLevelRegister
    {
        // start_player_sheet -> HEX = Event-driven
        public static readonly string start_player_sheet =
            "74 68 69 73 2e 52 75 6e 53 63 72 69 70 74 28 22 43 68 61 72 61 63 74 65 72 53 68 65 65 74 55 70 64 61 74 65 22 29";
    }
}