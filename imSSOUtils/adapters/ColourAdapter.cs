using System.Numerics;

namespace imSSOUtils.adapters
{
    /// <summary>
    /// Colour utilities.
    /// </summary>
    internal struct ColourAdapter
    {
        /// <summary>
        /// Convert RGB to Float RGB.
        /// </summary>
        public static Vector3 rgb_to_frgb(Vector3 rgb)
        {
            const short divide = 255;
            return new Vector3(rgb.X / divide, rgb.Y / divide, rgb.Z / divide);
        }

        /// <summary>
        /// Convert RGB to Float RGB.
        /// </summary>
        public static Vector3 rgb_to_frgb(float r, float g, float b)
        {
            const short divide = 255;
            return new Vector3(r / divide, g / divide, b / divide);
        }

        /// <summary>
        /// Convert RGBA to Float RGBA.
        /// </summary>
        public static Vector4 rgba_to_frgba(Vector4 rgba)
        {
            const short divide = 255;
            return new Vector4(rgba.X / divide, rgba.Y / divide, rgba.Z / divide, rgba.W);
        }

        /// <summary>
        /// Convert RGBA to Float RGBA.
        /// </summary>
        public static Vector4 rgba_to_frgba(float r, float g, float b, float alpha)
        {
            const short divide = 255;
            return new Vector4(r / divide, g / divide, b / divide, alpha);
        }
    }
}