using System;
using System.IO;
using System.Numerics;
using imSSOUtils.cores;
using imSSOUtils.customs;
using imSSOUtils.mod.option.@static;
using Newtonsoft.Json.Linq;
using static System.Convert;

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
        private const string cmods = "CMods", sliderPresets = "Slider Presets", objects = "Object Presets";
        #endregion

        /// <summary>
        /// Initialize everything.
        /// </summary>
        public static void initialize()
        {
            setup_cmods();
            setup_slider_presets();
            setup_object_presets();
        }

        /// <summary>
        /// Create a new (or if already present, override) slider preset.
        /// </summary>
        /// <param name="name">The presets name</param>
        public static void create_slider_preset(string name)
        {
            try
            {
                var file = $"{sliderPresets}\\{name}.spp";
                if (File.Exists(file)) File.Delete(file);
                File.Create(file).Close();
                var result = string.Empty;
                foreach (var entry in ModOption.f_sliders) result += $"{entry.Key}|{entry.Value}\n";
                foreach (var entry in ModOption.i_sliders) result += $"{entry.Key}|{entry.Value}\n";
                File.WriteAllText(file, result);
            }
            catch (Exception e)
            {
                Program.write_crash(e);
            }
        }

        /// <summary>
        /// Create a new (or if already present, override) object preset.
        /// </summary>
        /// <param name="name">The presets name</param>
        public static void create_object_preset(string name)
        {
            try
            {
                var file = $"{objects}\\{name}.spo";
                if (File.Exists(file)) File.Delete(file);
                File.Create(file).Close();
                var result = string.Empty;
                var spawnPos = SpawnerCore.spawnerPosition;
                var sObjects = SpawnerCore.objects;
                result += $"{spawnPos.X},{spawnPos.Y},{spawnPos.Z}\n";
                for (var i = 0; i < sObjects.Count; i++)
                {
                    var current = sObjects[i];
                    Vector3 currentPos = current.worldSpacePosition,
                        currentScale = current.scale;
                    var currentIdentifier = current.identifier;
                    /*
                     * Structure:
                     * 0 -> Identifier
                     * 1 -> PosX
                     * 2 -> PosY
                     * 3 -> PosZ
                     * 4 -> ScaleX
                     * 5 -> ScaleY
                     * 6 -> ScaleZ
                     * 7 = RotX
                     * 8 = RotY
                     * 9 = RotZ
                     */
                    result +=
                        $"{currentIdentifier},{currentPos.X},{currentPos.Y},{currentPos.Z},{currentScale.X},{currentScale.Y},{currentScale.Z}\n";
                }

                File.WriteAllText(file, result);
            }
            catch (Exception e)
            {
                Program.write_crash(e);
            }
        }

        /// <summary>
        /// Load a object preset.
        /// </summary>
        /// <param name="name">The presets name</param>
        public static void load_object_preset(string name)
        {
            try
            {
                var file = $"{objects}\\{name}.spo";
                if (!File.Exists(file) || SpawnerCore.isInitialized) return;
                const char splitCar = ',';
                var lines = File.ReadAllLines(file);
                var firstLine = lines[0].Split(splitCar);
                SpawnerCore.deactivate(true);
                SpawnerCore.spawnerPosition =
                    new Vector3(ToSingle(firstLine[0]), ToSingle(firstLine[1]), ToSingle(firstLine[2]));
                for (var i = 1; i < lines.Length; i++)
                {
                    var split = lines[i].Split(splitCar);
                    /*
                     * Structure:
                     * 0 -> Identifier
                     * 1 -> PosX
                     * 2 -> PosY
                     * 3 -> PosZ
                     * 4 -> ScaleX
                     * 5 -> ScaleY
                     * 6 -> ScaleZ
                     * 7 = RotX
                     * 8 = RotY
                     * 9 = RotZ
                     */
                    SpawnerCore.objects.Add(new PXFileObject(split[0],
                        new Vector3(ToSingle(split[1]), ToSingle(split[2]), ToSingle(split[3])),
                        new Vector3(ToSingle(split[4]), ToSingle(split[5]), ToSingle(split[6]))));
                }
            }
            catch (Exception e)
            {
                Program.write_crash(e);
            }
        }

        /// <summary>
        /// Load a slider preset.
        /// </summary>
        /// <param name="name">The presets name</param>
        public static void load_slider_preset(string name)
        {
            try
            {
                var file = $"{sliderPresets}\\{name}.spp";
                if (!File.Exists(file)) return;
                const char splitCar = '|';
                var lines = File.ReadAllLines(file);
                for (var i = 0; i < lines.Length; i++)
                {
                    var split = lines[i].Split(splitCar);
                    if (split[0].StartsWith("float"))
                        ModOption.f_sliders[split[0]] = ToSingle(split[1]);
                    else if (split[0].StartsWith("int"))
                        ModOption.i_sliders[split[0]] = ToInt32(split[1]);
                }
            }
            catch (Exception e)
            {
                Program.write_crash(e);
            }
        }

        /// <summary>
        /// Setup the Slider Presets directory
        /// </summary>
        private static void setup_slider_presets()
        {
            if (!Directory.Exists(sliderPresets)) Directory.CreateDirectory(sliderPresets);
        }

        /// <summary>
        /// Setup the Object Presets directory
        /// </summary>
        private static void setup_object_presets()
        {
            if (!Directory.Exists(objects)) Directory.CreateDirectory(objects);
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