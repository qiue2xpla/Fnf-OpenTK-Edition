using System;

namespace Fnf.Framework.TrueType
{
    public struct Curve
    {
        public Vector2 p0;
        public Vector2 p1;
        public Vector2 p2;

        public Curve(Vector2 p0, Vector2 p1, Vector2 p2)
        {
            this.p0 = p0;
            this.p1 = p1;
            this.p2 = p2;
        }

        public Vector2 Lerp(float t)
        {
            return new Vector2(LerpX(t), LerpY(t));
        }

        public float LerpX(float t)
        {
            (float a, float b, float c) = (p0.x, p1.x, p2.x);
            return (a + c - b * 2) * (t * t) + (b - a) * 2 * t + a;
        }

        public float LerpY(float t)
        {
            (float a, float b, float c) = (p0.y, p1.y, p2.y);
            return (a + c - b * 2) * (t * t) + (b - a) * 2 * t + a;
        }

        public (float a, float b) GetRoots(float y)
        {
            float ay = p0.y - y;
            float by = p1.y - y;
            float cy = p2.y - y;

            float a = ay + cy - by * 2;
            float b = (by - ay) * 2;
            float c = ay;

            if (Math.Abs(a) < 0.0001)
            {
                if (b != 0)
                    return (-c / b, float.NaN);
                else
                    return (float.NaN, float.NaN);
            }
            else
            {
                const float epsilon = 1e-5f;

                float discriminant = b * b - 4 * a * c;

                if (discriminant > -epsilon)
                {
                    float s = (float)Math.Sqrt(Math.Max(0, discriminant));
                    return ((-b + s) / (2 * a), (-b - s) / (2 * a));
                }

            }

            return (float.NaN, float.NaN);
        }

        internal (float a, float b, float c) CalculateYCoefficients(float y = 0)
        {
            float a = p0.y + p2.y - p1.y * 2;
            float b = (p1.y - p0.y) * 2;
            float c = p0.y - y;

            return (a, b, c);
        }

        internal (Vector2 a, Vector2 b, Vector2 c) CalculateVectorCoefficients(Vector2 pos = default)
        {
            Vector2 a = p0 + p2 - p1 * 2;
            Vector2 b = (p1 - p0) * 2;
            Vector2 c = p0 - pos;

            return (a, b, c);
        }

        internal bool isSplitable()
        {
            float min = Math.Min(p0.y, p2.y);
            float max = Math.Max(p0.y, p2.y);
            return !(p1.y >= min && p1.y <= max);
        }

        internal (Curve a, Curve b) Split()
        {
            (Vector2 a, Vector2 b, Vector2 c) = CalculateVectorCoefficients();

            float r = -b.y / (2 * a.y);
            Vector2 T = a * r * r + b * r + c;

            float lambdaA = (T.y - p0.y) / b.y;
            Vector2 p1A = p0 + b * lambdaA;

            float lambdaB = (T.y - p2.y) / (2 * a.y + b.y);
            Vector2 p1B = p2 + (a * 2 + b) * lambdaB;

            return (new Curve(p0, p1A, T), new Curve(T, p1B, p2));
        }
    }
}