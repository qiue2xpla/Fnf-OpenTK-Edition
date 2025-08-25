using System;
using System.Drawing.Printing;

namespace Fnf.Framework
{
    public struct Vector2
    {
        public float x, y;

        public Vector2(float v)
        {
            x = v;
            y = v;
        }

        public Vector2(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public Vector2 Rotate(float degree)
        {
            float radian = -degree / 180 * (float)Math.PI;
            float rx = (float)(x * Math.Cos(radian) - y * Math.Sin(radian));
            float ry = (float)(x * Math.Sin(radian) + y * Math.Cos(radian));
            return new Vector2(rx, ry);
        }

        public override string ToString()
        {
            return $"({x}, {y})";
        }

        public static Vector2 Max(Vector2 a, Vector2 b)
        {
            return new Vector2(Math.Max(a.x, b.x), Math.Max(a.y, b.y));
        }

        public static Vector2 Min(Vector2 a, Vector2 b)
        {
            return new Vector2(Math.Min(a.x, b.x), Math.Min(a.y, b.y));
        }

        public static Vector2 operator +(Vector2 v1, Vector2 v2) => new Vector2(v1.x + v2.x, v1.y + v2.y);
        public static Vector2 operator -(Vector2 v1, Vector2 v2) => new Vector2(v1.x - v2.x, v1.y - v2.y);
        public static Vector2 operator *(Vector2 v1, Vector2 v2) => new Vector2(v1.x * v2.x, v1.y * v2.y);
        public static Vector2 operator /(Vector2 v1, Vector2 v2) => new Vector2(v1.x / v2.x, v1.y / v2.y);

        public static Vector2 operator *(Vector2 v1, float v2) => new Vector2(v1.x * v2, v1.y * v2);
        public static Vector2 operator /(Vector2 v1, float v2) => new Vector2(v1.x / v2, v1.y / v2);

        public static Vector2 One => new Vector2(1);
        public static Vector2 Zero => new Vector2(0);
    }
}