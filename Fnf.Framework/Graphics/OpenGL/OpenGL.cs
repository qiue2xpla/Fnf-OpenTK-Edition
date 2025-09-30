using OpenTK.Graphics.OpenGL;

namespace Fnf.Framework.Graphics
{
    public static class OpenGL
    {
        public const int NULL = 0;

        public static void DrawArrays(DrawMode mode, int dataStrideCount)
        {
            GL.DrawArrays((PrimitiveType)mode, 0, dataStrideCount);
        }
        public static void BeginDrawing(DrawMode mode) => GL.Begin((PrimitiveType)mode);
        public static void EndDrawing() => GL.End();

        public static void Color3(float r, float g, float b) => GL.Color3(r, g, b);

        public static void Color4(float r, float g, float b, float a) => GL.Color4(r, g, b, a);
        public static void Color4(Color color) => GL.Color4(color.r, color.g, color.b, color.a);
        public static void Color4(Color color, float a) => GL.Color4(color.r, color.g, color.b, a);

        public static void LineWidth(float width) => GL.LineWidth(width);
        public static void PointSize(float size) => GL.PointSize(size);

        public static void TextureCoord(Vector2 vector) => GL.TexCoord2(vector.x,vector.y);
        public static void TextureCoord(float x, float y) => GL.TexCoord2(x,y);

        public static void Vertex2(float x, float y) => GL.Vertex2(x, y);
        public static void Vertex2(Vector2 vector) => GL.Vertex2(vector.x, vector.y);

        public static void Pixel2(float x, float y) => GL.Vertex2(Window.PixelToViewportHorizontal(x), Window.PixelToViewportVertical(y));
        public static void Pixel2(Vector2 vector) => Vertex2(Window.PixelToViewport(vector));
    }
}