using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading;
using imSSOUtils.adapters;
using imSSOUtils.window.windows.modding;

namespace imSSOUtils.cores
{
    /// <summary>
    /// The core of Cinematica.
    /// </summary>
    internal readonly struct CinematicaCore
    {
        #region Variables
        /// <summary>
        /// All position points.
        /// </summary>
        public static readonly List<Vector3> positions = new();

        /// <summary>
        /// Last visited point.
        /// </summary>
        private static Vector3 lastPos = Vector3.Zero;

        /// <summary>
        /// Common booleans.
        /// </summary>
        private static bool isPlaying, isBuilding;

        /// <summary>
        /// The Cinematica directory.
        /// </summary>
        private const string dir = "Cinematica";

        /// <summary>
        /// The current travel speed.
        /// </summary>
        private static float speed = 0.0009f;

        /// <summary>
        /// Timers for various operations.
        /// </summary>
        private static Timer build, play;
        #endregion

        /// <summary>
        /// Move towards a Vector.
        /// <para>Source: https://github.com/Unity-Technologies/UnityCsReference/blob/master/Runtime/Export/Math/Vector3.cs</para>
        /// </summary>
        private static Vector3 MoveTowards(Vector3 current, Vector3 target, float maxDistanceDelta)
        {
            float toVector_x = target.X - current.X,
                toVector_y = target.Y - current.Y,
                toVector_z = target.Z - current.Z;
            var sqdist = toVector_x * toVector_x + toVector_y * toVector_y + toVector_z * toVector_z;
            if (sqdist is 0 || (maxDistanceDelta >= 0 && sqdist <= maxDistanceDelta * maxDistanceDelta)) return target;
            var dist = (float) Math.Sqrt(sqdist);

            return new Vector3(current.X + toVector_x / dist * maxDistanceDelta,
                current.Y + toVector_y / dist * maxDistanceDelta,
                current.Z + toVector_z / dist * maxDistanceDelta);
        }

        /// <summary>
        /// Modifies the movement speed.
        /// </summary>
        /// <param name="newSpeed">The new speed.</param>
        public static void set_speed(float newSpeed)
        {
            try
            {
                speed = newSpeed / 10000;
            }
            catch (Exception)
            {
                Debug.WriteLine("Exception caught! Resetting the value");
                CinematicaWindow.currentSpeedValue = 1;
            }
        }

        /// <summary>
        /// Clears all positions as long as no path is being built, and no cinematic is running.
        /// </summary>
        public static void clear()
        {
            if (isBuilding || isPlaying) return;
            positions.Clear();
        }

        /// <summary>
        /// Determines whether it's safe to modify positions or not.
        /// </summary>
        public static bool can_modify() => !isBuilding && !isPlaying;

        /// <summary>
        /// Start logging positions.
        /// </summary>
        public static void build_path()
        {
            if (!MemoryAdapter.is_enabled()) return;
            if (isBuilding)
            {
                build.Dispose();
                isBuilding = false;
                return;
            }

            isBuilding = true;
            // ? Start tracking
            build = new Timer(_ =>
            {
                var pos = PXInternal.get_horse_position();
                if (Vector3.Distance(pos, lastPos) <= 5) return;
                positions.Add(pos);
                lastPos = pos;
            }, null, 0, CinematicaWindow.currentThresholdValue);
        }

        /// <summary>
        /// Save the current set of points.
        /// </summary>
        /// <param name="name">The save name.</param>
        public static void save_current(string name)
        {
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            var file = $"{dir}\\{name}.cna";
            if (File.Exists(file)) File.Delete(file);
            File.Create(file).Close();
            var print = string.Empty;
            /*
             * Instead of constantly calling File.Append*, we can just save what should be saved into a temporary
             * string and then write the string to the file directly, saving a lot of function-calls.
             */
            for (var i = 0; i < positions.Count; i++)
            {
                var point = positions[i];
                print += $"{point.X},{point.Y},{point.Z}\n";
            }

            File.WriteAllText(file, print);
        }

        /// <summary>
        /// Load a Cinematica files points.
        /// </summary>
        /// <param name="name">The save name.</param>
        public static void load_cna(string name)
        {
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
                return;
            }

            var file = $"{dir}\\{name}.cna";
            if (!File.Exists(file)) return;
            clear();
            const char comma = ',';
            var lines = File.ReadAllLines(file);
            for (var i = 0; i < lines.Length; i++)
            {
                var split = lines[i].Split(comma);
                positions.Add(new Vector3(Convert.ToSingle(split[0]), Convert.ToSingle(split[1]),
                    Convert.ToSingle(split[2])));
            }
        }

        /// <summary>
        /// Start the cinematic.
        /// </summary>
        public static void start_path()
        {
            if (!MemoryAdapter.is_enabled() || isBuilding || isPlaying || positions is {Count: < 2}) return;
            // ? Teleport to the first point
            PXInternal.set_horse_position(positions.FirstOrDefault());
            // ? Start the cinematic
            play = new Timer(_ =>
            {
                // ! Safety check to prevent issues
                if (isPlaying) return;
                isPlaying = true;
                var pos = PXInternal.get_horse_position();
                foreach (var point in positions)
                {
                    while (Vector3.Distance(PXInternal.get_horse_position(), point) >= 5)
                    {
                        pos = MoveTowards(pos, point, speed);
                        PXInternal.set_horse_position(pos);
                    }
                }

                isPlaying = false;
                play.Dispose();
            }, null, 0, 1000);
        }
    }
}