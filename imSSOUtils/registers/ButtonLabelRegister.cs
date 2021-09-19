namespace imSSOUtils.registers
{
    /// <summary>
    /// Button variables for dynamically changing the text.
    /// </summary>
    internal struct ButtonLabelRegister
    {
        public const string pointStart = "Start Point Saving",
            pointEnd = "Stop Point Saving";

        public static string currentPointText = pointStart;
    }
}