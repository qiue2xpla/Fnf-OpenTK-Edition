using OpenTK;
using System;

namespace Fnf.Framework
{
    /// <summary>
    /// 3 by 3 matrix grid and functions to do alot of things
    /// </summary>
    public struct Matrix3x3
    {
        /// <summary>
        /// A matrix with no effect whatsoever
        /// </summary>
        public static Matrix3x3 Identity = new Matrix3x3() { c1 = new Vector3(1, 0, 0), c2 = new Vector3(0, 1, 0), c3 = new Vector3(0, 0, 1) };
         
        // The matrix rows
        public Vector3 r1;
        public Vector3 r2;
        public Vector3 r3;

        // Matrix columns (based on rows)
        public Vector3 c1
        {
            get => new Vector3(r1.x, r2.x, r3.x);
            set => (r1.x, r2.x, r3.x) = (value.x, value.y, value.z);
        }
        public Vector3 c2
        {
            get => new Vector3(r1.y, r2.y, r3.y);
            set => (r1.y, r2.y, r3.y) = (value.x, value.y, value.z);
        }
        public Vector3 c3
        {
            get => new Vector3(r1.z, r2.z, r3.z);
            set => (r1.z, r2.z, r3.z) = (value.x, value.y, value.z);
        }

        public static Matrix3x3 CreateTransformMatrix(Vector2 pos, float rotationInRadian, Vector2 scale) // M = T * R * S
        {
            return CreateTranslationMatrix(pos) * CreateRotationMatrix(rotationInRadian) * CreateScaleMatrix(scale);
        }

        public static Matrix3x3 CreateInverseTransformMatrix(Vector2 pos, float rotationInRadian, Vector2 scale) // M^-1 = S^-1 * R^-1 * T^-1
        {
            return CreateScaleMatrix(Vector2.One / scale) * CreateRotationMatrix(-rotationInRadian) * CreateTranslationMatrix(Vector2.NegativeOne * pos);
        }

        public static Matrix3x3 CreateTranslationMatrix(Vector2 vector2)
        {
            Matrix3x3 T = new Matrix3x3() 
            { 
                r1 = new Vector3(1, 0, vector2.x),
                r2 = new Vector3(0, 1, vector2.y),
                r3 = new Vector3(0, 0, 1)
            };
            return T;
        }

        public static Matrix3x3 CreateScaleMatrix(Vector2 vector2)
        {
            Matrix3x3 S = new Matrix3x3()
            {
                r1 = new Vector3(vector2.x, 0        , 0),
                r2 = new Vector3(0        , vector2.y, 0),
                r3 = new Vector3(0        , 0        , 1)
            };
            return S;
        }

        public static Matrix3x3 CreateRotationMatrix(float radianAngle)
        {
            Matrix3x3 R = new Matrix3x3()
            {
                r1 = new Vector3((float)Math.Cos(radianAngle), -(float)Math.Sin(radianAngle), 0),
                r2 = new Vector3((float)Math.Sin(radianAngle),  (float)Math.Cos(radianAngle), 0),
                r3 = new Vector3(0                           , 0                            , 1)
            };
            return R;
        }

        public static Vector3 operator *(Matrix3x3 matrix, Vector3 vector)
        {
            return matrix.c1 * vector.x + 
                   matrix.c2 * vector.y + 
                   matrix.c3 * vector.z;
        }

        public static Matrix3x3 operator *(Matrix3x3 m1, Matrix3x3 m2)
        {
            return new Matrix3x3()
            {
                c1 = new Vector3(
                    Vector3.DotProduct(m1.r1, m2.c1),
                    Vector3.DotProduct(m1.r2, m2.c1),
                    Vector3.DotProduct(m1.r3, m2.c1)),
                c2 = new Vector3(
                    Vector3.DotProduct(m1.r1, m2.c2),
                    Vector3.DotProduct(m1.r2, m2.c2),
                    Vector3.DotProduct(m1.r3, m2.c2)),
                c3 = new Vector3(
                    Vector3.DotProduct(m1.r1, m2.c3),
                    Vector3.DotProduct(m1.r2, m2.c3),
                    Vector3.DotProduct(m1.r3, m2.c3))
            };
        }
    }
}