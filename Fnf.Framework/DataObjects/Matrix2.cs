using OpenTK;
using System;

namespace Fnf.Framework
{
    /// <summary>
    /// A simple 2 by 2 matrix grid
    /// </summary>
    public struct Matrix2
    {
        // Matrix rows
        public Vector2 r1;
        public Vector2 r2;

        // Matrix columns (based on rows)
        public Vector2 c1
        {
            get => new Vector2(r1.x, r2.x);
            set => (r1.x, r2.x) = (value.x, value.y);
        }
        public Vector2 c2
        {
            get => new Vector2(r1.y, r2.y);
            set => (r1.y, r2.y) = (value.x, value.y);
        }

        /// <summary>
        /// Used to send the matrix data using a uniform to a shader
        /// </summary>
        public float[] ToColumnMajorFloatArray()
        {
            return new float[] {
                c1.x, c1.y,
                c2.x, c2.y,
            };
        }

        /// <summary>
        /// A matrix with no effect whatsoever
        /// </summary>
        public static Matrix2 Identity = new Matrix2()
        {
            r1 = new Vector2(1, 0),
            r2 = new Vector2(0, 1)
        };


        // Some operators to make life simple
        public static Vector2 operator *(Matrix2 matrix, Vector2 vector)
        {
            return matrix.c1 * vector.x +
                   matrix.c2 * vector.y;
        }

        /*public static Matrix3 operator *(Matrix3 m1, Matrix3 m2)
        {
            return new Matrix2()
            {
                c1 = new Vector2(
                    Vector2.DotProduct(m1.r1, m2.c1),
                    Vector2.DotProduct(m1.r2, m2.c1))
                c2 = new Vector2(
                    Vector2.DotProduct(m1.r1, m2.c2),
                    Vector2.DotProduct(m1.r2, m2.c2))
            };
        }*/
    }
}