namespace imSSOUtils.adapters
{
    /// <summary>
    /// Short "code generator".
    /// </summary>
    internal readonly struct PXShort
    {
        public static string p_if(string statement, string code) => $"if ({statement}) {{\n{code}\n}}";
        public static string p_else_if(string statement, string code) => $" else if ({statement}) {{\n{code}\n}}";
    }
}