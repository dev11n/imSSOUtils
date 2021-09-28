using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace imSSOUtils.adapters.low_level
{
    /// <summary>
    /// "Unsafe" scripting - Alpha
    /// <para>Different from Alpine, this aims to support software-level features.</para>
    /// </summary>
    internal readonly struct UNSFScript
    {
        /// <summary>
        /// Unsafe script URL.
        /// </summary>
        private static readonly string url = "https://meadowriders.com/burst/unsf.scr";

        /// <summary>
        /// All UNSF pre-set functions, etc.
        /// </summary>
        private static readonly Dictionary<string, string> functions = new();

        /// <summary>
        /// Start UNSF.
        /// </summary>
        public static async Task begin()
        {
            var content = await WebAdapter.DownloadContent(url);
            const char newLine = '\n', functionSplit = ',';
            var lines = content.Split(newLine);
            for (var i = 0; i < lines.Length; i++)
            {
                var line = lines[i];
                if (!line.Contains(functionSplit)) return;
                var set = line.Split(functionSplit);
                Debug.WriteLine($"add {line}");
                functions.Add(set[0], set[1]);
            }
        }

        /// <summary>
        /// Convert from UNSF -> PXScript.
        /// </summary>
        /// <param name="unsf"></param>
        /// <returns></returns>
        public static string convert_to_pxscript(string unsf)
        {
            var content = unsf;
            foreach (var set in functions) content = content.Replace(set.Key, set.Value);
            return content;
        }
    }
}