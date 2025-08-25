using Fnf.Framework.Graphics;
using Fnf.Framework;

namespace Fnf.Game
{
    public class Background : MovableObject, IRenderable
    {
        public bool isRenderable { get; set; } = true;
        public Size textureSize;

        int texture;

        public Background(string imagePath)
        {
            texture = Texture.GenerateFromPath(imagePath, out textureSize);
        }

        public void Render()
        {
            Texture.Use(texture);
            OpenGL.BeginDrawing(DrawMode.Quads);

            Vector2 v = textureSize.ToVector2() / 2 * globalScale;

            OpenGL.TextureCoord(1, 0);
            OpenGL.Pixel2(v.Rotate(globalRotation));
            OpenGL.TextureCoord(0, 0);
            OpenGL.Pixel2((v * new Vector2(-1, 1)).Rotate(globalRotation));
            OpenGL.TextureCoord(0, 1);
            OpenGL.Pixel2((v * new Vector2(-1,-1)).Rotate(globalRotation));
            OpenGL.TextureCoord(1, 1);
            OpenGL.Pixel2((v * new Vector2( 1,-1)).Rotate(globalRotation));

            OpenGL.EndDrawing();

            Texture.Use(OpenGL.NULL);
        }
    }
}
