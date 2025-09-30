using System;

namespace Fnf.Framework.Graphics
{
    /// <summary>
    /// Contains some helpfull drawing functions
    /// </summary>
    public static class Gizmos
    {
        private static int cubeVAO;
        private static int roundShader;

        public static void DrawPoints(params Vector2[] points)
        {
            OpenGL.BeginDrawing(DrawMode.Points);

            for (int i = 0; i < points.Length; i++)
            {
                OpenGL.Pixel2(points[i]);
            }

            OpenGL.EndDrawing();
        }

        public static void DrawCube(Vector2 position, Vector2 size, float rotation)
        {
            OpenGL.BeginDrawing(DrawMode.Quads);

            Vector2 topright = size / 2;

            OpenGL.Pixel2(position + topright.Rotate(rotation));
            OpenGL.Pixel2(position + new Vector2(-topright.x, topright.y).Rotate(rotation));
            OpenGL.Pixel2(position - topright.Rotate(rotation));
            OpenGL.Pixel2(position + new Vector2(topright.x, -topright.y).Rotate(rotation));

            OpenGL.EndDrawing();
        }

        public static void DrawRoundQuad(GUI gui, Color color, float radius, float smoothness)
        {
            Vector2 size = new Vector2(gui.width, gui.height);

            Matrix3 mat = Matrix3.Scale(Window.PixelToViewport(1, 1)) * gui.WorldlTransformMatrix() * Matrix3.Scale(size / 2);

            Shader.Use(roundShader);

            Shader.UniformMat(roundShader, "transform", mat);

            Shader.Color(roundShader, "col", color);
            Shader.Uniform2(roundShader, "rect", size);
            Shader.Uniform1(roundShader, "radius", radius);
            Shader.Uniform1(roundShader, "smoothness", smoothness);

            VAO.Use(cubeVAO);

            OpenGL.DrawArrays(DrawMode.Triangles, 6);

            VAO.Use(OpenGL.NULL);
            Shader.Use(OpenGL.NULL);
        }

        static Gizmos()
        {
            cubeVAO = VAO.GenerateVAO();
            int vbo = VBO.GenerateVBO();

            float[] data = new float[] {
                 1,  1,
                -1,  1,
                -1, -1,
                -1, -1,
                 1, -1,
                 1,  1,
            };

            VBO.Use(vbo);
            VBO.Resize(data.Length * sizeof(float));
            VBO.SetData(data);

            VAO.Use(cubeVAO);

            VertexAttrib2f.UseAttrib();

            VAO.Use(OpenGL.NULL);
            VBO.Use(OpenGL.NULL);

            roundShader = Shader.GenerateShaderFromResource("round");
        }
    }
}