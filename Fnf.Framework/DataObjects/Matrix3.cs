using OpenTK;
using System;

namespace Fnf.Framework
{
    /// <summary>
    /// A simple 3 by 3 matrix grid
    /// </summary>
    public struct Matrix3
    {
        // Matrix rows
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

        /// <summary>
        /// Used to send the matrix data using a uniform to a shader
        /// </summary>
        public float[] ToColumnMajorFloatArray()
        {
            return new float[] {
                c1.x, c1.y, c1.z,
                c2.x, c2.y, c2.z,
                c3.x, c3.y, c3.z,
            };
        }

        /// <summary>
        /// A matrix with no effect whatsoever
        /// </summary>
        public static Matrix3 Identity = new Matrix3()
        {
            r1 = new Vector3(1, 0, 0),
            r2 = new Vector3(0, 1, 0),
            r3 = new Vector3(0, 0, 1)
        };

        /// <summary>
        /// Returns a matrix that applies transformation by the given arguments
        /// </summary>
        public static Matrix3 Transform(Vector2 position, float rotationInRadian, Vector2 scale) // M = T * R * S
        {
            return Translation(position) * Rotation(rotationInRadian) * Scale(scale);
        }

        /// <summary>
        /// Returns a matrix that inverses a transformation by the given arguments
        /// </summary>
        public static Matrix3 InverseTransform(Vector2 pos, float rotationInRadian, Vector2 scale) // M^-1 = S^-1 * R^-1 * T^-1
        {
            return Scale(Vector2.One / scale) * Rotation(-rotationInRadian) * Translation(Vector2.NegativeOne * pos);
        }

        /// <summary>
        /// Returns a matrix that translates by the given <seealso cref="Vector2"/>
        /// </summary>
        public static Matrix3 Translation(Vector2 translation)
        {
            return new Matrix3() 
            { 
                r1 = new Vector3(1, 0, translation.x),
                r2 = new Vector3(0, 1, translation.y),
                r3 = new Vector3(0, 0, 1            )
            };
        }

        /// <summary>
        /// Returns a matrix that scales by the given <seealso cref="Vector2"/>
        /// </summary>
        public static Matrix3 Scale(Vector2 scale)
        {
            return new Matrix3()
            {
                r1 = new Vector3(scale.x, 0      , 0),
                r2 = new Vector3(0      , scale.y, 0),
                r3 = new Vector3(0      , 0      , 1)
            };
        }

        /// <summary>
        /// Returns a matrix that rotates by the given radian angle
        /// </summary>
        public static Matrix3 Rotation(float radianAngle)
        {
            return new Matrix3()
            {
                r1 = new Vector3((float)Math.Cos(radianAngle), -(float)Math.Sin(radianAngle), 0),
                r2 = new Vector3((float)Math.Sin(radianAngle),  (float)Math.Cos(radianAngle), 0),
                r3 = new Vector3(0                           , 0                            , 1)
            };
        }

        // Some operators to make life simple

        /// <summary>
        /// Converts the Vector2 to Vector3 and back to Vector2
        /// </summary>
        public static Vector2 operator *(Matrix3 matrix, Vector2 vector)
        {
            return new Vector2(
                matrix.c1.x * vector.x + matrix.c2.x * vector.y + matrix.c3.x,
                matrix.c1.y * vector.x + matrix.c2.y * vector.y + matrix.c3.y);
        }

        public static Vector3 operator *(Matrix3 matrix, Vector3 vector)
        {
            return matrix.c1 * vector.x + 
                   matrix.c2 * vector.y + 
                   matrix.c3 * vector.z;
        }

        public static Matrix3 operator *(Matrix3 m1, Matrix3 m2)
        {
            return new Matrix3()
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