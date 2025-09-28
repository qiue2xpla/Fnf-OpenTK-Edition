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
    }
}