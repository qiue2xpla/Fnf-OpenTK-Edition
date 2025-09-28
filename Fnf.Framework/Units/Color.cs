namespace Fnf.Framework
{
    public struct Color
    {
        public byte r, g, b, a;

        public Color(int color)
        {
            const int mask = 0b11111111;

            b = (byte)(color & mask);
            g = (byte)((color >> 8) & mask);
            r = (byte)((color >> 16) & mask);
            a = (byte)((color >> 24) & mask);
        }

        public Color(byte v)
        {
            r = v;
            g = v;
            b = v;
            a = 255;
        }

        public Color(byte r, byte g, byte b)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            a = 255;
        }

        public Color(byte r, byte g, byte b, byte a)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = a;
        }


        public int ToArgb()
        {
            return (a << 24) | (r << 16) | (g << 8) | b;
        }

        public static Color Lerp(Color color0, Color color1, float t)
        {
            byte r = (byte)lerp(color0.r, color1.r);
            byte g = (byte)lerp(color0.g, color1.g);
            byte b = (byte)lerp(color0.b, color1.b);
            byte a = (byte)lerp(color0.a, color1.a);

            return new Color(r, g, b, a);

            float lerp(float value0, float value1) => value0 + (value1 - value0) * t;
        }

        public static Color Transparent => new Color(255, 255, 255, 0);
        public static Color White => new Color(255, 255, 255, 255);
        public static Color Black => new Color(0, 0, 0, 255);
        public static Color Green => new Color(0, 255, 0, 255);
        public static Color Red => new Color(255, 0, 0, 255);
    }
}