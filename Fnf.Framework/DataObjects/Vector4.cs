using System;

namespace Fnf.Framework
{
    public struct Vector4
    {
        public float x, y, z, w;

        public Vector4(float sharedValue)
        {
            x = sharedValue;
            y = sharedValue;
            z = sharedValue;
            w = sharedValue;
        }

        public Vector4(float x, float y, float z, float w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        public static float DotProduct(Vector4 a, Vector4 b)
        {
            Vector4 v = a * b;
            return v.x + v.y + v.z + v.w;
        }

        public Vector3 ToEuclidean()
        {
            return new Vector3(x, y, z);
        }

        public override string ToString()
        {
            return $"({x}, {y}, {z}, {w})";
        }

        public static Vector4 operator *(Vector4 vector, float value)
        {
            return new Vector4(vector.x * value, vector.y * value, vector.z * value, vector.w * value);
        }

        public static Vector4 operator *(Vector4 vector1, Vector4 vector2)
        {
            return new Vector4(vector1.x * vector2.x, vector1.y * vector2.y, vector1.z * vector2.z, vector1.w * vector2.w);
        }

        public static Vector4 operator +(Vector4 vector1, Vector4 vector2)
        {
            return new Vector4(vector1.x + vector2.x, vector1.y + vector2.y, vector1.z + vector2.z, vector1.w + vector2.w);
        }
    }
}