using OpenTK.Graphics.OpenGL;

namespace Fnf.Framework.Graphics
{
    public static class VertexAttrib2f2f    
    {
        const int ComponentCount = 2;
        const VertexAttribPointerType type = VertexAttribPointerType.Float;
        const int stride = 4 * sizeof(float);

        public static void UseAttrib()
        {
            UseAttrib0();
            UseAttrib1();
        }

        public static void UseAttrib0()
        {
            GL.VertexAttribPointer(0, ComponentCount, type, false, stride, 0);
            GL.EnableVertexAttribArray(0);
        }

        public static void UseAttrib1()
        {
            int offset = 2 * sizeof(float);
            GL.VertexAttribPointer(1, ComponentCount, type, false, stride, offset);
            GL.EnableVertexAttribArray(1);
        }
    }
}