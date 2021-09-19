using System.Collections.Generic;
using imSSOUtils.mod;
using imSSOUtils.mod.option.@static;
using Newtonsoft.Json.Linq;

namespace imSSOUtils.customs
{
    /// <summary>
    /// CMod structure.
    /// </summary>
    internal class CMod
    {
        #region Variables
        /// <summary>
        /// All options.
        /// </summary>
        internal readonly Dictionary<ModOption, IMod> options = new();

        /// <summary>
        /// Checkbox values (if any checkbox is present).
        /// </summary>
        internal readonly Dictionary<string, bool> checkboxes = new();
        #endregion

        /// <summary>
        /// JSON Output of the mod.
        /// </summary>
        public readonly JObject raw;

        /// <summary>
        /// Strings.
        /// </summary>
        public readonly string name, code, category;

        /// <summary>
        /// Create a new CMod instance.
        /// </summary>
        public CMod(JObject raw, string name, string code, string category)
        {
            this.raw = raw;
            this.name = name;
            this.code = code;
            this.category = category;
        }
    }
}