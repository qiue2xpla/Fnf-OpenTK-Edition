using OpenTK;
using System;

namespace Fnf.Framework
{
    public struct Matrix3x3
    {
        public static Matrix3x3 Identity = new Matrix3x3() { c1 = new Vector3(1, 0, 0), c2 = new Vector3(0, 1, 0), c3 = new Vector3(0, 0, 1) };

        public Vector3 r1;
        public Vector3 r2;
        public Vector3 r3;

        public Vector3 c1
        {
            get => new Vector3(r1.x, r2.x, r3.x);
            set
            {
                r1.x = value.x;
                r2.x = value.y;
                r3.x = value.z;
            }
        }
        public Vector3 c2
        {
            get => new Vector3(r1.y, r2.y, r3.y);
            set
            {
                r1.y = value.x;
                r2.y = value.y;
                r3.y = value.z;
            }
        }
        public Vector3 c3
        {
            get => new Vector3(r1.z, r2.z, r3.z);
            set
            {
                r1.z = value.x;
                r2.z = value.y;
                r3.z = value.z;
            }
        }

        public static Matrix3x3 CreateTranslationMatrix(Vector2 vector2)
        {
            return new Matrix3x3() 
            { 
                r1 = new Vector3(1, 0, vector2.x),
                r2 = new Vector3(0, 1, vector2.y),
                r3 = new Vector3(0, 0, 1)
            };
        }

        public static Matrix3x3 CreateScaleMatrix(Vector2 vector2)
        {
            return new Matrix3x3()
            {
                r1 = new Vector3(vector2.x, 0        , 0),
                r2 = new Vector3(0        , vector2.y, 0),
                r3 = new Vector3(0        , 0        , 1)
            };
        }

        public static Matrix3x3 CreateRotationMatrix(float radianAngle)
        {
            return new Matrix3x3()
            {
                r1 = new Vector3((float)Math.Cos(radianAngle), -(float)Math.Sin(radianAngle), 0),
                r2 = new Vector3((float)Math.Sin(radianAngle),  (float)Math.Cos(radianAngle), 0),
                r3 = new Vector3(0                           , 0                            , 1)
            };
        }

        public static Vector3 operator *(Matrix3x3 matrix, Vector3 vector)
        {
            return matrix.c1 * vector.x + 
                   matrix.c2 * vector.y + 
                   matrix.c3 * vector.z;
        }

        public static Matrix3x3 operator *(Matrix3x3 m1, Matrix3x3 m2)
        {
            Vector3 col1 = new Vector3(
                Vector3.DotProduct(m1.r1, m2.c1),
                Vector3.DotProduct(m1.r2, m2.c1),
                Vector3.DotProduct(m1.r3, m2.c1));

            Vector3 col2 = new Vector3(
                Vector3.DotProduct(m1.r1, m2.c2),
                Vector3.DotProduct(m1.r2, m2.c2),
                Vector3.DotProduct(m1.r3, m2.c2));

            Vector3 col3 = new Vector3(
                Vector3.DotProduct(m1.r1, m2.c3),
                Vector3.DotProduct(m1.r2, m2.c3),
                Vector3.DotProduct(m1.r3, m2.c3));


            return new Matrix3x3()
            {
                c1 = col1,
                c2 = col2,
                c3 = col3
            };
        }
    }
}