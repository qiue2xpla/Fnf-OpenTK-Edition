using Fnf.Framework.Graphics;
using Fnf.Framework;
using System;

namespace Fnf.Game
{
    public class Image : MovableObject, IRenderable, IDisposable
    {
        public bool isRenderable { get; set; } = true;
        public Size size;

        int texture;

        public Image(string imagePath)
        {
            texture = Texture.GenerateFromPath(imagePath, false);
            size = Texture.GetTextureSize(texture);
        }

        public void Render()
        {
            Vector2 v = size.ToVector2() / 2 * globalScale;

            Texture.Use(texture);
            OpenGL.BeginDrawing(DrawMode.Quads);

            OpenGL.TextureCoord(1, 0);
            OpenGL.Pixel2(v.Rotate(globalRotation) + globalPosition);
            OpenGL.TextureCoord(0, 0);
            OpenGL.Pixel2((v * new Vector2(-1, 1)).Rotate(globalRotation) + globalPosition);
            OpenGL.TextureCoord(0, 1);
            OpenGL.Pixel2((v * new Vector2(-1,-1)).Rotate(globalRotation) + globalPosition);
            OpenGL.TextureCoord(1, 1);
            OpenGL.Pixel2((v * new Vector2( 1,-1)).Rotate(globalRotation) + globalPosition);

            OpenGL.EndDrawing();
            Texture.Use(OpenGL.NULL);
        }

        public void Dispose()
        {
            Texture.Destroy(texture);
            texture = OpenGL.NULL;
        }
    }
}
