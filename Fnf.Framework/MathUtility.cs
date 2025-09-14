namespace Fnf.Framework
{
    public static class MathUtility
    {
        public static float Lerp(float a, float b, float t) => a + (b - a) * t;
        public static Vector2 Lerp(Vector2 a, Vector2 b, float t) => a + (b - a) * t;

        public static float Clamp(float a, float max, float min)
        {
            if (a > max) return max;
            if (a < min) return min;
            return a;
        }

        public static int Clamp(int a, int max, int min)
        {
            if (a > max) return max;
            if (a < min) return min;
            return a;
        }

        public static int WrapClamp(int a, int max, int min)
        {
            if (a > max) return min;
            if (a < min) return max;
            return a;
        }
    }
}
