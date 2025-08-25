using OpenTK.Graphics.OpenGL;

namespace Fnf.Framework.Graphics
{
    public static class VAO
    {
        public static int GenerateVAO()
        { 
            return GL.GenVertexArray(); 
        }

        public static void Use(int id)
        {
            GL.BindVertexArray(id);
        }
    }
}