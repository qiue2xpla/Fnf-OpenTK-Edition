namespace Fnf.Framework
{
    public static class MathFunctions
    {
        public static float Lerp(float a, float b, float t) => a + (b - a) * t;
    }
}
