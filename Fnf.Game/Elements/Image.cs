using Fnf.Framework.Graphics;
using Fnf.Framework;
using System;

namespace Fnf.Game
{
    public class Image : GameObject, IRenderable
    {
        public bool isRenderable { get; set; } = true;
        public float width, height;
        public Color color = Color.White;

        int texture;

        public Image(string imagePath)
        {
            texture = Texture.GenerateFromPath(imagePath, false);
            Size size = Texture.GetTextureSize(texture);
            (width, height) = (size.width, size.height);
        }

        public void Render()
        {
            Texture.Use(texture);
            OpenGL.BeginDrawing(DrawMode.Quads);
            OpenGL.Color4(color);

            Vector2 wv = new Vector2(width, height) / 2;

            OpenGL.TextureCoord(1, 0);
            Pixel2( wv.x,  wv.y);
            OpenGL.TextureCoord(0, 0);
            Pixel2(-wv.x,  wv.y);
            OpenGL.TextureCoord(0, 1);
            Pixel2(-wv.x, -wv.y);
            OpenGL.TextureCoord(1, 1);
            Pixel2( wv.x, -wv.y);

            OpenGL.EndDrawing();
            Texture.Use(OpenGL.NULL);

            void Pixel2(float x, float y)
            {
                OpenGL.Pixel2((GetObjectWorldlTransformMatrix() * new Vector3(x, y, 1)).ToEuclidean());
            }
        }
    }
}
