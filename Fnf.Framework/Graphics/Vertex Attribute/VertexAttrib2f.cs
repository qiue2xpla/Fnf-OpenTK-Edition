using OpenTK.Graphics.OpenGL;

namespace Fnf.Framework.Graphics
{
    public static class VertexAttrib2f   
    {
        const int ComponentCount = 2;
        const VertexAttribPointerType type = VertexAttribPointerType.Float;
        const int stride = 2 * sizeof(float);

        public static void UseAttrib()
        {
            GL.VertexAttribPointer(0, ComponentCount, type, false, stride, 0);
            GL.EnableVertexAttribArray(0);
        }
    }
}