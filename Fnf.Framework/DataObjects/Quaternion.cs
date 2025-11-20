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

        public static Quaternion FormAngle(Vector3 axis, float angleInRadian)
        {
            float half = angleInRadian / 2;
            float sin = (float)Math.Sin(half);
            return new Quaternion((float)Math.Cos(half), axis.x * sin, axis.y * sin, axis.z * sin);
        }

        public static Quaternion operator *(Quaternion a, Quaternion b)
        {
            return new Quaternion(
                a.w * b.w - a.x * b.x - a.y * b.y - a.z * b.z,
                a.w * b.x + a.x * b.w + a.y * b.z - a.z * b.y,
                a.w * b.y - a.x * b.z + a.y * b.w + a.z * b.x,
                a.w * b.z + a.x * b.y - a.y * b.x + a.z * b.w);
        }

        public static Quaternion Idendity = new Quaternion(1, 0, 0, 0);
    }
}