using System;

namespace Fnf.Framework
{
    public struct Quaternion
    {
        public float w, x, y, z;

        public Quaternion(float w, float x, float y, float z)
        {
            this.w = w;
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public Quaternion conjugate()
        {
            return new Quaternion(w, -x, -y, -z);
        }

        public Vector3 rotate(Vector3 v)
        {
            Quaternion qv = new Quaternion(0, v.x, v.y, v.z);
            Quaternion res = this * qv * conjugate();
            return new Vector3(res.x, res.y, res.z);
        }

        public static Quaternion Inverse(Quaternion q)
        {
            float magSq = q.x * q.x + q.y * q.y + q.z * q.z + q.w * q.w;

            if (magSq == 0f) return Identity;

            return new Quaternion(
                -q.x / magSq,
                -q.y / magSq,
                -q.z / magSq,
                 q.w / magSq
            );
        }

        public static Vector3 ToEuler(Quaternion q)
        {
            // Extract values for readability
            float x = q.x;
            float y = q.y;
            float z = q.z;
            float w = q.w;

            Vector3 euler = new Vector3();

            // X rotation (pitch)
            float sinX = 2f * (w * x + y * z);
            float cosX = 1f - 2f * (x * x + y * y);
            euler.x = (float)Math.Atan2(sinX, cosX);

            // Y rotation (yaw)
            float sinY = 2f * (w * y - z * x);
            sinY = MathUtility.Clamp(sinY, 1, -1); // avoid NaN
            euler.y = (float)Math.Asin(sinY);

            // Z rotation (roll)
            float sinZ = 2f * (w * z + x * y);
            float cosZ = 1f - 2f * (y * y + z * z);
            euler.z = (float)Math.Atan2(sinZ, cosZ);

            return euler;
        }

        public static Quaternion FromEuler(Vector3 euler)
        {
            // Half angles
            float cx = (float)Math.Cos(euler.x * 0.5f);
            float sx = (float)Math.Sin(euler.x * 0.5f);

            float cy = (float)Math.Cos(euler.y * 0.5f);
            float sy = (float)Math.Sin(euler.y * 0.5f);

            float cz = (float)Math.Cos(euler.z * 0.5f);
            float sz = (float)Math.Sin(euler.z * 0.5f);

            Quaternion q;
            q.x = sx * cy * cz - cx * sy * sz;
            q.y = cx * sy * cz + sx * cy * sz;
            q.z = cx * cy * sz - sx * sy * cz;
            q.w = cx * cy * cz + sx * sy * sz;

            return q;
        }

        public static Quaternion FormAngle(Vector3 axis, float angleInRadian)
        {
            float half = angleInRadian / 2;
            float sin = (float)Math.Sin(half);
            return new Quaternion((float)Math.Cos(half), axis.x * sin, axis.y * sin, axis.z * sin);
        }

        public static Quaternion Normalize(Quaternion q)
        {
            float len = (float)Math.Sqrt(q.x * q.x + q.y * q.y + q.z * q.z + q.w * q.w);

            if (len < 0.000001f)
                return Quaternion.Identity; // avoid divide-by-zero

            float inv = 1.0f / len;

            return new Quaternion(q.x * inv, q.y * inv, q.z * inv, q.w * inv);
        }

        public static Quaternion operator *(Quaternion a, Quaternion b)
        {
            return new Quaternion(
                a.w * b.w - a.x * b.x - a.y * b.y - a.z * b.z,
                a.w * b.x + a.x * b.w + a.y * b.z - a.z * b.y,
                a.w * b.y - a.x * b.z + a.y * b.w + a.z * b.x,
                a.w * b.z + a.x * b.y - a.y * b.x + a.z * b.w);
        }

        public static bool operator ==(Quaternion a, Quaternion b)
        {
            return a.w == b.w && a.x == b.x && a.y == b.y && a.z == b.z;
        }

        public static bool operator !=(Quaternion a, Quaternion b)
        {
            return !(a == b);
        }

        public static Quaternion Identity = new Quaternion(1, 0, 0, 0);
    }
}