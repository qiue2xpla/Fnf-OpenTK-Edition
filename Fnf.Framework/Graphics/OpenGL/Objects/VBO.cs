using OpenTK.Graphics.OpenGL;
using System;

namespace Fnf.Framework.Graphics
{
    public static class VBO
    {
        public static int GenerateVBO()
        {
            return GL.GenBuffer();
        }

        public static void Use(int id)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, id);
        }

        public static void Resize(int sizeInBytes)
        {
            GL.BufferData(BufferTarget.ArrayBuffer, sizeInBytes, IntPtr.Zero, BufferUsageHint.DynamicDraw);
        }

        public static void SetData(float[] data, int startIndex = 0)
        {
            GL.BufferSubData(BufferTarget.ArrayBuffer, new IntPtr(startIndex * sizeof(float)), data.Length * sizeof(float), data);
        }
    }
}