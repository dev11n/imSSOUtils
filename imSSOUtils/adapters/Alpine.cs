using System.Collections.Generic;
using System.IO;
using System.Net;
using imSSOUtils.adapters.extensions;
using imSSOUtils.customs;

namespace imSSOUtils.adapters
{
    /// <summary>
    /// Alpine Script structure.
    /// <para>Alpine just replaces text to make it easier for people to script their own stuff.</para>
    /// </summary>
    internal struct Alpine
    {
        /// <summary>
        /// The URL for all presets.
        /// </summary>
        // Making this a constant exposes it when obfuscated, unless the obfuscator has support for it.
        private static readonly string url = "https://meadowriders.com/burst/filemanager.lpc";

        /// <summary>
        /// Dictionaries.
        /// </summary>
        internal static readonly Dictionary<string, string> sets = new();

        /// <summary>
        /// Lists.
        /// </summary>
        public static readonly List<CMod> cmods_list = new();

        /// <summary>
        /// A static readonly <see cref="WebClient"/> instance.
        /// </summary>
        private static readonly WebClient client = new();

        /// <summary>
        /// Custom directories
        /// </summary>
        private const string cmods = "CMods", skyPresets = "Sky Presets";

        /// <summary>
        /// Initialize Alpine.
        /// </summary>
        public static void initialize_ascript()
        {
            var lines = client.DownloadString(url).Split('\n');
            const char comma = ',';
            const string comment_start = "/>", comment_end = "</";
            for (var i = 0; i < lines.Length; i++)
            {
                var line = lines[i];
                // If there's no comma, or it's not a Alpine comment (/> Hello! </), skip over this entry to prevent errors.
                if (!line.Contains(comma) || line.StartsWith(comment_start) && line.EndsWith(comment_end)) continue;
                // Splitting worked when we didn't have commas in actual functions, so this way works better.
                var index = line.IndexOf(comma);
                var preset = line.Substring(0, index);
                sets.Add(preset, line.Replace(preset + comma, string.Empty));
            }
        }

        /// <summary>
        /// Checks for "::ForceKill();", if it's found then Stop, Delete and Kill is added.
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        private static string force_kill(string code)
        {
            var result = string.Empty;
            var split = code.Split('\n');
            for (var i = 0; i < split.Length; i++)
            {
                var line = split[i];
                if (!line.Contains("ForceKill();"))
                {
                    result += split.Length - i != 1 ? line + "\n" : line;
                    continue;
                }

                var until = line.GetUntilOrEmpty("::ForceKill();");
                var finished = $"{until}::Stop();\n{until}::Delete();\n{until}::Kill();";
                result += split.Length - i != 1 ? finished + "\n" : finished;
            }

            return result;
        }

        /// <summary>
        /// Format the string to match AScript.
        /// </summary>
        public static string proc_frm_string(string content)
        {
            var newContent = force_kill(content);
            foreach (var set in sets)
                // New fix: !Regex.Match(newContent, @$"\b{set.Key}\b").Success && set.Key.Length > 3
                // The new fix has to be tested and improved a lot more before its implemented.
                newContent = newContent.Replace(set.Key, set.Value);
            if (content.Contains("Spectate")) newContent += "global/IntroCam1.Stop();";
            return newContent;
        }

        /// <summary>
        /// Refresh the entire CMods list.
        /// </summary>
        public static void refresh_cmods()
        {
            cmods_list.Clear();
            FileAdapter.setup_cmods();
        }

        /// <summary>
        /// Get all the categories.
        /// </summary>
        public static IEnumerable<string> get_cmods_categories()
        {
            var categories = new List<string>();
            var dirs = Directory.GetDirectories(cmods);
            for (var i = 0; i < dirs.Length; i++) categories.Add(new DirectoryInfo(dirs[i]).Name);
            return categories;
        }

        /// <summary>
        /// Get all the custom mods in a specific category.
        /// </summary>
        public static List<CMod> get_cmods(string category)
        {
            var modsInCategory = new List<CMod>();
            var mods = cmods_list;
            for (var i = 0; i < mods.Count; i++)
            {
                var mod = mods[i];
                if (mod.category == category) modsInCategory.Add(mod);
            }

            return modsInCategory;
        }

        /// <summary>
        /// Get a custom mod by its name.
        /// </summary>
        public static CMod get_cmod(string name)
        {
            var mods = cmods_list;
            for (var i = 0; i < mods.Count; i++)
            {
                var mod = mods[i];
                if (mod.name == name) return mod;
            }

            return null;
        }
    }
}