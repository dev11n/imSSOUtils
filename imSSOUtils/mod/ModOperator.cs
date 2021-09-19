using System;
using System.Collections.Generic;
using System.Linq;

namespace imSSOUtils.mod
{
    /// <summary>
    /// Basically the mod manager.
    /// </summary>
    internal struct ModOperator
    {
        #region Variables
        /// <summary>
        /// A list of all raw mods.
        /// </summary>
        private static readonly IEnumerable<Type> mods_raw = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(t => typeof(IMod).IsAssignableFrom(t) && t != typeof(IMod));

        /// <summary>
        /// A list of all mods.
        /// </summary>
        private static readonly Dictionary<ModAccessor, string> mods = new();
        #endregion

        /// <summary>
        /// Get all mods.
        /// </summary>
        public static Dictionary<ModAccessor, string> get_mods() => mods;

        /// <summary>
        /// Caches mods from <see cref="mods_raw"/> into <see cref="mods"/>.
        /// </summary>
        public static void cache_mods()
        {
            var raw = mods_raw.ToList();
            const char split = '.';
            for (var i = 0; i < raw.Count; i++)
            {
                var mod_raw = raw[i];
                var core = Activator.CreateInstance(mod_raw) as IMod;
                var tmp = mod_raw.Namespace?.Split(split);
                mods.Add(new ModAccessor(mod_raw.Name, core), tmp[^1]);
                core?.on_initialize();
            }
        }

        /// <summary>
        /// Tries to find a mod by its name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static IMod get_mod(string name)
        {
            foreach (var mod in mods)
                if (mod.Key.name == name)
                    return mod.Key.mod;
            return null;
        }

        /// <summary>
        /// Disposes all timers.
        /// </summary>
        public static void dispose_timers()
        {
            foreach (var mod in mods) mod.Key.mod.modTimer?.Dispose();
        }
    }
}