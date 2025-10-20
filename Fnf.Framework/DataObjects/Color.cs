namespace Fnf.Framework
{
    /// <summary>
    /// Holds onto 4 <seealso cref="float"/> values (red, green, blue, alpha) for coloring
    /// </summary>
    public struct Color
    {
        // Some colors for ease of use
        public static Color Transparent => new Color(1, 1, 1, 0);
        public static Color White => new Color(1, 1, 1, 1);
        public static Color Black => new Color(0, 0, 0, 1);
        public static Color Red => new Color(1, 0, 0, 1);
        public static Color Green => new Color(0, 1, 0, 1);
        public static Color Blue => new Color(0, 0, 1, 1);

        public float r, g, b, a;

        /// <summary>
        /// Create a color with the rgb being set to the same value and the alpha seperatly
        /// </summary>
        public Color(float value, float alpha = 1) => (r, g, b, a) = (value, value, value, alpha);

        /// <summary>
        /// Create a color while setting all its values
        /// </summary>
        public Color(float r, float g, float b, float a = 1) => (this.r, this.g, this.b, this.a) = (r, g, b, a);

        /// <summary>
        /// Create a color from <see cref="int"/> (argb) value
        /// </summary>
        public Color(int intColor)
        {
            const int mask = 0b11111111;
            b = (intColor & mask) / 255f;
            g = ((intColor >> 8) & mask) / 255f;
            r = ((intColor >> 16) & mask) / 255f;
            a = ((intColor >> 24) & mask) / 255f;
        }

        /// <summary>
        /// Converts this <see cref="Color"/> to an <see cref="int"/> (argb) value
        /// </summary>
        public int ToArgbInt()
        {
            return ((byte)(a * 255) << 24) | ((byte)(r * 255) << 16) | ((byte)(g * 255) << 8) | (byte)(b * 255);
        }

        /// <summary>
        /// Lerp two colors with a value
        /// </summary>
        public static Color Lerp(Color color0, Color color1, float t)
        {
            return new Color(
                MathUtility.Lerp(color0.r, color1.r, t),
                MathUtility.Lerp(color0.g, color1.g, t),
                MathUtility.Lerp(color0.b, color1.b, t),
                MathUtility.Lerp(color0.a, color1.a, t));
        }

        public static Color operator *(Color c1, Color c2)
        {
            return new Color(
                c1.r * c2.r,
                c1.g * c2.g,
                c1.b * c2.b,
                c1.a * c2.a);
        }
    }
}