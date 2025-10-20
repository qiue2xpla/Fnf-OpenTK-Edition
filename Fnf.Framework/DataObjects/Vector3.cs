using System;

namespace Fnf.Framework
{
    public struct Vector3
    {
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

        public float lenght()
        {
            return (float)Math.Sqrt(x*x + y*y + z*z);
        }

        public Vector3 normalized()
        {
            return this / lenght();
        }

        public Vector2 ToEuclidean()
        {
            return new Vector2(x, y);
        }

        public override string ToString()
        {
            return $"({x}, {y}, {z})";
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

        public static Vector3 operator /(Vector3 vector, float v)
        {
            return new Vector3(vector.x - v, vector.y - v, vector.z - v);
        }

        public static Vector3 Zero => new Vector3(0);


        public static Vector3 UnitY => new Vector3(0, 1, 0);
    }
}