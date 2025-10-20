using OpenTK.Graphics.OpenGL;

namespace Fnf.Framework.Graphics
{
    /// <summary>
    /// VAO or Vertex Array Object is used to contain buffers
    /// </summary>
    public static class VAO
    {
        /// <summary>
        /// Generates a new <see cref="VAO"/>
        /// </summary>
        public static int Generate(bool bind = false)
        {
            int vao = GL.GenVertexArray();
            if (bind) Bind(vao);
            return vao;
        }

        /// <summary>
        /// Binds the Vertex Array Buffer using its id
        /// </summary>
        public static void Bind(int id)
        {
            GL.BindVertexArray(id);
        }

        /// <summary>
        /// Sets the attribute data usage and the buffer to use to the currently bound array buffer
        /// </summary>
        /// <param name="attrib">The location of the attribute set in the shader</param>
        /// <param name="componentCount">The components present in the given data ( float=1, Vec2=2, etc... )</param>
        /// <param name="type">The type of the data</param>
        /// <param name="stride">The length of the data for a given vertex in bytes</param>
        /// <param name="offset">The offset in the vertex data in bytes</param>
        public static void VertexAttribPointer(int attrib, int componentCount, VertexAttribPointerType type, int stride, int offset)
        {
            var convertedType = (OpenTK.Graphics.OpenGL.VertexAttribPointerType)type;
            GL.VertexAttribPointer(attrib, componentCount, convertedType, false, stride, offset);
        }

        /// <summary>
        /// Every vertex attribute is disabled by default so it has to be enabled if used
        /// </summary>
        /// <param name="attrib">The location of the attribute set in the shader</param>
        public static void EnableVertexAttribArray(int attrib)
        {
            GL.EnableVertexAttribArray(attrib);
        }

        public enum VertexAttribPointerType
        {
            SByte = 5120,
            Byte = 5121,
            Short = 5122,
            UShort = 5123,
            Int = 5124,
            UInt = 5125,
            Float = 5126,
            Double = 5130,
        }
    }
}