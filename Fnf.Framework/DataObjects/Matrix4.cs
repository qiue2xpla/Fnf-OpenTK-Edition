using System;

namespace Fnf.Framework
{
    /// <summary>
    /// A simple 4 by 4 matrix grid
    /// </summary>
    public struct Matrix4
    {
        // Matrix rows
        public Vector4 r1;
        public Vector4 r2;
        public Vector4 r3;
        public Vector4 r4;

        // Matrix columns (based on rows)
        public Vector4 c1
        {
            get => new Vector4(r1.x, r2.x, r3.x, r4.x);
            set => (r1.x, r2.x, r3.x, r4.x) = (value.x, value.y, value.z, value.w);
        }
        public Vector4 c2
        {
            get => new Vector4(r1.y, r2.y, r3.y, r4.y);
            set => (r1.y, r2.y, r3.y, r4.y) = (value.x, value.y, value.z, value.w);
        }
        public Vector4 c3
        {
            get => new Vector4(r1.z, r2.z, r3.z, r4.z);
            set => (r1.z, r2.z, r3.z, r4.z) = (value.x, value.y, value.z, value.w);
        }
        public Vector4 c4
        {
            get => new Vector4(r1.w, r2.w, r3.w, r4.w);
            set => (r1.w, r2.w, r3.w, r4.w) = (value.x, value.y, value.z, value.w);
        }

        /// <summary>
        /// Used to send the matrix data using a uniform to a shader
        /// </summary>
        public float[] ToColumnMajorFloatArray()
        {
            return new float[] {
                c1.x, c1.y, c1.z, c1.w,
                c2.x, c2.y, c2.z, c2.w,
                c3.x, c3.y, c3.z, c3.w,
                c4.x, c4.y, c4.z, c4.w
            };
        }

        /// <summary>
        /// A matrix with no effect whatsoever
        /// </summary>
        public static Matrix4 Identity = new Matrix4()
        {
            r1 = new Vector4(1, 0, 0, 0),
            r2 = new Vector4(0, 1, 0, 0),
            r3 = new Vector4(0, 0, 1, 0),
            r4 = new Vector4(0, 0, 0, 1)
        };

        /// <summary>
        /// Returns a matrix that applies transformation by the given arguments
        /// </summary>
        /*public static Matrix3 Transform(Vector2 position, float rotationInRadian, Vector2 scale) // M = T * R * S
        {
            return Translation(position) * Rotation(rotationInRadian) * Scale(scale);
        }*/

        /// <summary>
        /// Returns a matrix that inverses a transformation by the given arguments
        /// </summary>
        /*public static Matrix3 InverseTransform(Vector2 pos, float rotationInRadian, Vector2 scale) // M^-1 = S^-1 * R^-1 * T^-1
        {
            return Scale(Vector2.One / scale) * Rotation(-rotationInRadian) * Translation(Vector2.NegativeOne * pos);
        }*/

        /// <summary>
        /// Returns a matrix that translates by the given <seealso cref="Vector3"/>
        /// </summary>
        public static Matrix4 Translation(Vector3 translation)
        {
            return new Matrix4() 
            { 
                r1 = new Vector4(1, 0, 0, translation.x),
                r2 = new Vector4(0, 1, 0, translation.y),
                r3 = new Vector4(0, 0, 1, translation.z),
                r4 = new Vector4(0, 0, 0, 1            )
            };
        }

        /// <summary>
        /// Returns a matrix that scales by the given <seealso cref="Vector3"/>
        /// </summary>
        public static Matrix4 Scale(Vector3 scale)
        {
            return new Matrix4()
            {
                r1 = new Vector4(scale.x, 0      , 0      , 0),
                r2 = new Vector4(0      , scale.y, 0      , 0),
                r3 = new Vector4(0      , 0      , scale.z, 0),
                r4 = new Vector4(0      , 0      , 0      , 1)
            };
        }

        public static Matrix4 RotateX(float radianAngle)
        {
            float sin = (float)Math.Sin(radianAngle);
            float cos = (float)Math.Cos(radianAngle);
            return new Matrix4()
            {
                r1 = new Vector4(1, 0  , 0   , 0),
                r2 = new Vector4(0, cos, -sin, 0),
                r3 = new Vector4(0, sin, cos , 0),
                r4 = new Vector4(0, 0  , 0   , 1)
            };
        }

        public static Matrix4 RotateY(float radianAngle)
        {
            float sin = (float)Math.Sin(radianAngle);
            float cos = (float)Math.Cos(radianAngle);
            return new Matrix4()
            {
                r1 = new Vector4(cos , 0, sin, 0),
                r2 = new Vector4(0   , 1, 0  , 0),
                r3 = new Vector4(-sin, 0, cos, 0),
                r4 = new Vector4(0   , 0, 0  , 1)
            };
        }

        public static Matrix4 RotateZ(float radianAngle)
        {
            float sin = (float)Math.Sin(radianAngle);
            float cos = (float)Math.Cos(radianAngle);
            return new Matrix4()
            {
                r1 = new Vector4(cos, -sin, 0, 0),
                r2 = new Vector4(sin, cos , 0, 0),
                r3 = new Vector4(0  , 0   , 1, 0),
                r4 = new Vector4(0  , 0   , 0, 1)
            };
        }

        public static Matrix4 NewChatGPTPerspectiveFieldOfView(float fovy, float aspect, float zNear, float zFar)
        {
            float f = 1f / (float)Math.Tan(fovy / 2f);

            return new Matrix4()
            {
                r1 = new Vector4(f / aspect, 0, 0                               , 0                                    ),
                r2 = new Vector4(0         , f, 0                               , 0                                    ),
                r3 = new Vector4(0         , 0, -(zFar + zNear) / (zFar - zNear), -(2f * zFar * zNear) / (zFar - zNear)),
                r4 = new Vector4(0         , 0, -1                              , 0                                    )
            };
        }

        public static Matrix4 LookAt(Vector3 eye, Vector3 target, Vector3 up)
        {
            Vector3 zaxis = (eye - target).normalized();                   // camera forward (points backwards)
            Vector3 xaxis = Vector3.CrossProduct(up, zaxis).normalized();  // right vector
            Vector3 yaxis = Vector3.CrossProduct(zaxis, xaxis);            // corrected up vector

            return new Matrix4()
            {
                r1 = new Vector4(xaxis.x, xaxis.y, xaxis.z, -Vector3.DotProduct(xaxis, eye)),
                r2 = new Vector4(yaxis.x, yaxis.y, yaxis.z, -Vector3.DotProduct(yaxis, eye)),
                r3 = new Vector4(zaxis.x, zaxis.y, zaxis.z, -Vector3.DotProduct(zaxis, eye)),
                r4 = new Vector4(0, 0, 0, 1)
            };
        }

        // Some operators to make life simple
        public static Vector4 operator *(Matrix4 matrix, Vector4 vector)
        {
            return matrix.c1 * vector.x + 
                   matrix.c2 * vector.y + 
                   matrix.c3 * vector.z +
                   matrix.c4 * vector.w;
        }

        public static Matrix4 operator *(Matrix4 m1, Matrix4 m2)
        {
            return new Matrix4()
            {
                c1 = m1 * m2.c1,
                c2 = m1 * m2.c2,
                c3 = m1 * m2.c3,
                c4 = m1 * m2.c4
            };
        }
    }
}