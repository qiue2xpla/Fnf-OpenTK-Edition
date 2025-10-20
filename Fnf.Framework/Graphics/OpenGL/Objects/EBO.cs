using OpenTK.Graphics.OpenGL;
using System;

namespace Fnf.Framework.Graphics
{
    public static class EBO
    {
        public static int Generate()
        {
            return GL.GenBuffer();
        }

        public static int Generate(int uintCount)
        {
            int id = GL.GenBuffer();
            Use(id);
            Resize(uintCount);
            return id;
        }

        public static void Use(int id)
        {
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, id);
        }

        public static void Resize(int uintCount)
        {
            GL.BufferData(BufferTarget.ElementArrayBuffer, uintCount * sizeof(uint), IntPtr.Zero, BufferUsageHint.DynamicDraw);
        }

        public static void SetData(uint[] indices)
        {
            GL.BufferSubData(BufferTarget.ElementArrayBuffer, IntPtr.Zero, indices.Length * sizeof(uint), indices);
        }
    }
}