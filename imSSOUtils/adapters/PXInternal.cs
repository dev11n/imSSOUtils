using System;
using System.Drawing;
using System.Numerics;
using CNLibrary;
using imSSOUtils.adapters.low_level;
using imSSOUtils.window.windows;

namespace imSSOUtils.adapters
{
    /// <summary>
    /// Quick access to game internal stuff.
    /// </summary>
    internal struct PXInternal
    {
        #region Variables
        /// <summary>
        /// Web Bytes
        /// </summary>
        public static string dc01_bytes, dc02_bytes, dc03_bytes, bypass_bytes;

        /// <summary>
        /// Web Addresses
        /// </summary>
        public static string dc01, dc02, dc03, bypass;

        /// <summary>
        /// Pointer addresses converted into real ones.
        /// </summary>
        private static UIntPtr xReal, yReal, zReal;

        /// <summary>
        /// Web Addresses
        /// </summary>
        private static string x, y, z;

        /// <summary>
        /// Web Addresses
        /// </summary>
        public static string state;
        #endregion
        #region Enums
        /// <summary>
        /// Horse states
        /// </summary>
        public enum HorseState
        {
            JUMPING = 7
        }
        #endregion

        /// <summary>
        /// Get the current horse position.
        /// </summary>
        /// <returns>A vector for the X, Y and Z coordinates.</returns>
        public static Vector3 get_horse_position()
        {
            var mem = MemoryAdapter.head.get_consult().Memory;
            return new Vector3(mem.read_float(xReal), mem.read_float(yReal), mem.read_float(zReal));
        }

        /// <summary>
        /// Cache everything that's needed.
        /// </summary>
        public static void cache_pointers()
        {
            // ? Web
            x = WebAdapter.GetAPIValue("pos_x").ToString();
            y = WebAdapter.GetAPIValue("pos_y").ToString();
            z = WebAdapter.GetAPIValue("pos_z").ToString();
            dc01 = WebAdapter.GetAPIValue("disconnect1").ToString();
            dc01_bytes = WebAdapter.GetAPIValue("disconnect1a").ToString();
            dc02 = WebAdapter.GetAPIValue("disconnect2").ToString();
            dc02_bytes = WebAdapter.GetAPIValue("disconnect2a").ToString();
            dc03 = WebAdapter.GetAPIValue("disconnect3").ToString();
            dc03_bytes = WebAdapter.GetAPIValue("disconnect3a").ToString();
            bypass = WebAdapter.GetAPIValue("bypass").ToString();
            bypass_bytes = WebAdapter.GetAPIValue("bypassa").ToString();
            state = WebAdapter.GetAPIValue("trigger").ToString();
            ConsoleWindow.send_input("cached pointers", "[system]", Color.White);
        }

        /// <summary>
        /// Convert X Y Z to an unsigned integer pointer.
        /// </summary>
        public static void convert()
        {
            // ? Convert
            xReal = Utils.get_real_address(x);
            yReal = Utils.get_real_address(y);
            zReal = Utils.get_real_address(z);
        }

        /// <summary>
        /// Change the horses position.
        /// </summary>
        /// <param name="pos">The new position.</param>
        public static void set_horse_position(Vector3 pos)
        {
            var mem = MemoryAdapter.head.get_consult().Memory;
            mem.write_float(x, pos.X);
            mem.write_float(y, pos.Y);
            mem.write_float(z, pos.Z);
        }

        /// <summary>
        /// Get the current horse state.
        /// </summary>
        /// <returns></returns>
        public static HorseState get_horse_state() =>
            (HorseState) MemoryAdapter.head.get_consult().Memory.read_int(state);

        /// <summary>
        /// Get the child count of a specific object.
        /// </summary>
        /// <param name="gObject">The object</param>
        /// <returns></returns>
        public static int get_child_count(string gObject)
        {
            try
            {
                CVar.write_cvar01($"{gObject}::GetChildCount()", "Int");
                return CVar.read_cvar01_int();
            }
            catch (Exception)
            {
                return 0;
            }
        }

        /// <summary>
        /// Get a child's name from a specific objects index.
        /// </summary>
        /// <param name="gObject"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static string get_child_name(string gObject, int index)
        {
            // ! Disable printing of errors as it'll get spammy, even though it fixes it automatically
            MemoryAdapter.print_autofix_errors(false);
            MemoryAdapter.direct_call($"{gObject}::SetChildGlobalAccessShortcut(\"DVVa\", {index});");
            CVar.write_cvar01("Game->DVVa::GetName()", "String");
            // ! Enable again
            MemoryAdapter.print_autofix_errors(true);
            return CVar.read_cvar01_string();
        }
    }
}