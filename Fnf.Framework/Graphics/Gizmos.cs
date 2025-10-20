using System;

namespace Fnf.Framework.Graphics
{
    /// <summary>
    /// Draws some primitive shapes to make designing faster and easier
    /// </summary>
    public static class Gizmos
    {
        private static int cubeVAO;
        private static int roundShader;

        static Gizmos()
        {
            cubeVAO = VAO.Generate();
            int vbo = VBO.GenerateVBO();
            VAO.Bind(cubeVAO);
            VBO.Use(vbo);
            VBO.Resize(12 * sizeof(float));
            VBO.SetData(new float[] { 1,  1, -1,  1, -1, -1, -1, -1, 1, -1, 1,  1 });
            VertexAttrib2f.UseAttrib();
            VBO.Use(0);
            VAO.Bind(0);

            roundShader = Shader.GenerateShaderFromResource("round");
        }

        /// <summary>
        /// Draws points on the given positions
        /// </summary>
        public static void DrawPoints(float size, params Vector2[] points)
        {
            OpenTK.Graphics.OpenGL.GL.PointSize(size);

            OpenGL.BeginDrawing(DrawMode.Points);

            for (int i = 0; i < points.Length; i++)
            {
                OpenGL.Pixel2(points[i]);
            }

            OpenGL.EndDrawing();
        }

        /// <summary>
        /// Draws a quad that has round corners
        /// </summary>
        public static void DrawRoundQuad(GUI gui, Color color, float radius, float smoothness)
        {
            Vector2 size = new Vector2(gui.width, gui.height);

            Matrix3 mat = Matrix3.Scale(Window.PixelToViewport(1, 1)) * gui.WorldlTransformMatrix() * Matrix3.Scale(size / 2);

            Shader.Bind(roundShader);

            Shader.UniformMat(roundShader, "transform", mat);

            Shader.Color(roundShader, "col", color);
            Shader.Uniform2(roundShader, "rect", size);
            Shader.Uniform4(roundShader, "radius", radius, radius, radius, radius);
            Shader.Uniform1(roundShader, "smoothness", smoothness);

            VAO.Bind(cubeVAO);

            OpenGL.DrawArrays(DrawMode.Triangles, 6);

            VAO.Bind(0);
            Shader.Bind(0);
        }
    }
}