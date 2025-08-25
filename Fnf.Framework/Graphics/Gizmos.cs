using System;

namespace Fnf.Framework.Graphics
{
    public static class Gizmos
    {
        private static int roundShader, vao;

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

        public static void DrawRoundQuad(Vector2 position, Vector2 scale, float width, float height, float rotation, float radius, float smoothness, Color color)
        {
            Vector2 dim = new Vector2(width, height);
            Vector2 idk = dim * scale / 2;
            float ratioUI = idk.y / idk.x;

            Shader.Use(roundShader);

            // Vertex shader
            Shader.Uniform2(roundShader, "position", Window.PixelToViewport(position));
            Shader.Uniform2(roundShader, "size", Window.PixelToViewport(new Vector2(width, height) / 2) * scale);
            Shader.Uniform1(roundShader, "rotation", -rotation / 180 * (float)Math.PI);
            Shader.Uniform1(roundShader, "ratioGRID", Window.GridSize.slobe);

            // Fragment shader
            Shader.Uniform4(roundShader, "col", color.r / 255f, color.g / 255f, color.b / 255f, color.a / 255f);
            Shader.Uniform1(roundShader, "radius", 2 * radius / (ratioUI > 1 ? width : height));
            Shader.Uniform1(roundShader, "ratioUI", height / width);
            Shader.Uniform1(roundShader, "smoothness", 2 * smoothness / (ratioUI > 1 ? width : height));

            VAO.Use(vao);

            OpenGL.DrawArrays(DrawMode.Triangles, 6);

            VAO.Use(-1);
            Shader.Use(OpenGL.NULL);
        }

        internal static void LoadGizmoz()
        {
            vao = VAO.GenerateVAO();
            int vbo = VBO.GenerateVBO();

            float[] data = new float[] {
                 1,  1,  1,  1, 
                -1,  1, -1,  1,
                -1, -1, -1, -1,
                -1, -1, -1, -1, 
                 1, -1,  1, -1,
                 1,  1,  1,  1 
            };

            VBO.Use(vbo);
            VBO.Resize(data.Length * sizeof(float));
            VBO.SetData(data);

            VAO.Use(vao);

            VertexAttrib2f2f.UseAttrib();

            VAO.Use(-1);
            VBO.Use(-1);

            roundShader = Shader.GenerateShaderFromResource("round");
        }
    }
}