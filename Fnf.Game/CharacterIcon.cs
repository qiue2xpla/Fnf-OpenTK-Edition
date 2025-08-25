using Fnf.Framework.Graphics;
using Fnf.Framework;

namespace Fnf.Game
{
    public class CharacterIcon : MovableObject, IRenderable
    {
        public bool isRenderable { get; set; } = true;
        public bool isNormal = true;
        public Vector2 size;
        int texture;
        Vector2 topRight;

        Vector2[] verts = new Vector2[4] {new Vector2(1,1), new Vector2(-1,1), new Vector2(-1,-1),new Vector2(1,-1)};

        public CharacterIcon(string name)
        {
            texture = Texture.GenerateFromPath(name + ".png", out Size dim);
            size = new Vector2(dim.width / 2, dim.height);
            topRight = size / 2;
        }

        public void Render()
        {
            if (!isRenderable) return;

            Texture.Use(texture);

            OpenGL.BeginDrawing(DrawMode.Quads);

            float offset = isNormal ? 0 : 0.5f;

            OpenGL.TextureCoord(0.5f + offset, 0);
            OpenGL.Pixel2((topRight * globalScale * verts[0]).Rotate(globalRotation) + globalPosition);
            OpenGL.TextureCoord(0    + offset, 0);
            OpenGL.Pixel2((topRight * globalScale * verts[1]).Rotate(globalRotation) + globalPosition);
            OpenGL.TextureCoord(0    + offset, 1);
            OpenGL.Pixel2((topRight * globalScale * verts[2]).Rotate(globalRotation) + globalPosition);
            OpenGL.TextureCoord(0.5f + offset, 1);
            OpenGL.Pixel2((topRight * globalScale * verts[3]).Rotate(globalRotation) + globalPosition);

            OpenGL.EndDrawing();

            Texture.Use(OpenGL.NULL);
        }
    }
}