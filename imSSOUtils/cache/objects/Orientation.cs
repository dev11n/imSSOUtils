using System.Numerics;
using imSSOUtils.adapters;
using imSSOUtils.adapters.low_level;

namespace imSSOUtils.cache.objects
{
    /// <summary>
    /// Allows for access to an objects orientation.
    /// </summary>
    internal readonly struct Orientation
    {
        /// <summary>
        /// Try and get the rotation of an object.
        /// </summary>
        /// <param name="gObject"></param>
        /// <param name="axis"></param>
        /// <returns></returns>
        public static float get_rotation(string gObject, char axis)
        {
            CVar.write_cvar01($"{gObject}::GetRotation{axis}()", "Float");
            return CVar.read_cvar01_float();
        }

        /// <summary>
        /// Try and get the rotation of an object.
        /// </summary>
        /// <param name="gObject"></param>
        /// <returns></returns>
        public static Vector3 get_rotation(string gObject)
        {
            CVar.write_cvar01($"{gObject}::GetRotationX()", "Float");
            var x = CVar.read_cvar01_float();
            CVar.write_cvar01($"{gObject}::GetRotationY()", "Float");
            var y = CVar.read_cvar01_float();
            CVar.write_cvar01($"{gObject}::GetRotationZ()", "Float");
            var z = CVar.read_cvar01_float();
            return new Vector3(x, y, z);
        }

        /// <summary>
        /// Adjusts and aligns the object to match the ground surface.
        /// </summary>
        /// <param name="gObject"></param>
        public static void align_to_terrain(string gObject) =>
            MemoryAdapter.direct_call($"{gObject}::AdjustAndAlignToGround();");
    }
}