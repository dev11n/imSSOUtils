using System;
using System.IO;
using imSSOUtils.customs;
using imSSOUtils.mod.option.@static;
using Newtonsoft.Json.Linq;

namespace imSSOUtils.adapters
{
    /// <summary>
    /// Everything related to files.
    /// </summary>
    public struct FileAdapter
    {
        #region Variables
        /// <summary>
        /// All directories.
        /// </summary>
        private const string cmods = "CMods", skyPresets = "Sky Presets";
        #endregion

        /// <summary>
        /// Initialize everything.
        /// </summary>
        public static void initialize()
        {
            setup_cmods();
            setup_sky_presets();
        }

        /// <summary>
        /// Create a new (or if already present, override) sky preset.
        /// </summary>
        /// <param name="name">The presets name</param>
        public static void create_sky_preset(string name)
        {
            try
            {
                var file = $"{skyPresets}\\{name}.skp";
                if (File.Exists(file)) File.Delete(file);
                File.Create(file).Close();
                var result = string.Empty;
                foreach (var entry in ModOption.f_sliders) result += $"{entry.Key}|{entry.Value}\n";
                File.WriteAllText(file, result);
            }
            catch (Exception e)
            {
                Program.write_crash(e);
            }
        }

        /// <summary>
        /// Load a sky preset.
        /// </summary>
        /// <param name="name">The presets name</param>
        public static void load_sky_preset(string name)
        {
            try
            {
                var file = $"{skyPresets}\\{name}.skp";
                if (!File.Exists(file)) return;
                const char splitCar = '|';
                var lines = File.ReadAllLines(file);
                for (var i = 0; i < lines.Length; i++)
                {
                    var split = lines[i].Split(splitCar);
                    ModOption.f_sliders[split[0]] = Convert.ToSingle(split[1]);
                }
            }
            catch (Exception e)
            {
                Program.write_crash(e);
            }
        }

        /// <summary>
        /// Setup the Sky Presets directory
        /// </summary>
        private static void setup_sky_presets()
        {
            if (!Directory.Exists(skyPresets)) Directory.CreateDirectory(skyPresets);
        }

        /// <summary>
        /// Setup the CMods directory and files
        /// </summary>
        public static void setup_cmods()
        {
            try
            {
                if (!Directory.Exists(cmods)) Directory.CreateDirectory(cmods);
                var files = Directory.GetFiles(cmods, "*.*", SearchOption.AllDirectories);
                for (var i = 0; i < files.Length; i++)
                {
                    var file = files[i];
                    var parsed = JObject.Parse(File.ReadAllText(file));
                    var mod = new CMod(parsed, parsed["name"]?.ToString(), parsed["code"]?.ToString(),
                        parsed["category"]?.ToString());
                    if (!Alpine.cmods_list.Contains(mod))
                        Alpine.cmods_list.Add(mod);
                }
            }
            catch (Exception e)
            {
                Program.write_crash(e);
            }
        }

        /// <summary>
        /// Create a new custom mod, overriding existing ones.
        /// </summary>
        /// <param name="name">The name of the mod</param>
        /// <param name="code">The Alpine Script code of the mod</param>
        /// <param name="category">A custom category the mod should be in</param>
        public static void create_cmod(string name, string code, string category)
        {
            try
            {
                var directory = $"{cmods}\\{category}";
                var fullPath = $"{directory}\\{name}.json";
                if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);
                if (File.Exists(fullPath)) File.Delete(fullPath);
                var json = new JObject
                {
                    {"name", name},
                    {"category", category},
                    {"code", code}
                };
                File.Create(fullPath).Close();
                File.WriteAllText(fullPath, json.ToString());
                Alpine.cmods_list.Add(new CMod(json, name, code, category));
            }
            catch (Exception e)
            {
                Program.write_crash(e);
            }
        }

        /// <summary>
        /// Creates a new (or override) custom mod from a JSON object.
        /// </summary>
        /// <param name="saveJSON"></param>
        public static void create_cmod(JObject saveJSON)
        {
            try
            {
                var directory = $"{cmods}\\{saveJSON["category"]}";
                var fullPath = $"{directory}\\{saveJSON["name"]}.json";
                if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);
                if (File.Exists(fullPath)) File.Delete(fullPath);
                File.Create(fullPath).Close();
                File.WriteAllText(fullPath, saveJSON.ToString());
                Alpine.refresh_cmods();
            }
            catch (Exception e)
            {
                Program.write_crash(e);
            }
        }
    }
}