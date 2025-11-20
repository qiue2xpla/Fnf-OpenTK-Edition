using System;
using System.Diagnostics.Contracts;

namespace Fnf.Framework
{
    public struct Vector3
    {
        public static Vector3 Zero => new Vector3(0);
        public static Vector3 One => new Vector3(1);
        public static Vector3 UnitX => new Vector3(1, 0, 0);
        public static Vector3 UnitY => new Vector3(0, 1, 0);
        public static Vector3 UnitZ => new Vector3(0, 1, 1);

        public float x, y, z;

        public Vector3(float sharedValue)
        {
            x = sharedValue;
            y = sharedValue;
            z = sharedValue;
        }

        public Vector3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public float lenght()
        {
            return (float)Math.Sqrt(x*x + y*y + z*z);
        }

        public Vector3 normalized()
        {
            return this / lenght();
        }

        public Vector4 ExtendW(float value)
        {
            return new Vector4(x, y, z, value);
        }

        public override string ToString()
        {
            return $"({x}, {y}, {z})";
        }

        public static float DotProduct(Vector3 vector1, Vector3 vector2)
        {
            Vector3 v = vector1 * vector2;
            return v.x + v.y + v.z;
        }

        public static Vector3 CrossProduct(Vector3 a, Vector3 b)
        {
            return new Vector3(
                a.y * b.z - a.z * b.y,
                a.z * b.x - a.x * b.z,
                a.x * b.y - a.y * b.x
            );
        }

        public static Vector3 operator *(Vector3 vector, float value)
        {
            return new Vector3(vector.x * value, vector.y * value, vector.z * value);
        }

        public static Vector3 operator *(Vector3 vector1, Vector3 vector2)
        {
            return new Vector3(vector1.x * vector2.x, vector1.y * vector2.y, vector1.z * vector2.z);
        }

        public static Vector3 operator +(Vector3 vector1, Vector3 vector2)
        {
            return new Vector3(vector1.x + vector2.x, vector1.y + vector2.y, vector1.z + vector2.z);
        }

        public static Vector3 operator -(Vector3 vector1, Vector3 vector2)
        {
            return new Vector3(vector1.x - vector2.x, vector1.y - vector2.y, vector1.z - vector2.z);
        }

        public static Vector3 operator -(Vector3 v)
        {
            return new Vector3(-v.x, -v.y, -v.z);
        }

        public static Vector3 operator /(Vector3 vector1, Vector3 vector2)
        {
            return new Vector3(vector1.x - vector2.x, vector1.y - vector2.y, vector1.z - vector2.z);
        }

        public static Vector3 operator /(Vector3 vector, float v)
        {
            return new Vector3(vector.x - v, vector.y - v, vector.z - v);
        }

        public static bool operator ==(Vector3 a, Vector3 b)
        {
            return a.x == b.x && a.y == b.y && a.z == b.z;
        }

        public static bool operator !=(Vector3 a, Vector3 b)
        {
            return !(a == b);
        }
    }
}