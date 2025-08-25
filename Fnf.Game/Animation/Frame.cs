using Fnf.Framework;
using System;

namespace Fnf.Game
{
    public struct Frame
    {
        public string name;
        public Vector2[] verts;
        public Vector2[] coords;

        internal Frame(SubTexture subTexture, int textureWidth, int textureHeight)
        {
            name = subTexture.name;

            // Calculate texture coordinate
            Vector2 textureSize = new Vector2(textureWidth, textureHeight);
            Vector2 min = new Vector2(subTexture.x, subTexture.y) / textureSize;
            Vector2 max = min + new Vector2(subTexture.width, subTexture.height) / textureSize;

            coords = new Vector2[4]
            {
                new Vector2(max.x, min.y),
                new Vector2(min.x, min.y),
                new Vector2(min.x, max.y),
                new Vector2(max.x, max.y),
            };

            // Calculate vertex coordinates

            // For some reason its a negative number
            float left = Math.Abs(subTexture.frameX);
            float top = Math.Abs(subTexture.frameY);
            float right = subTexture.frameWidth - subTexture.width - left;
            float bottom = subTexture.frameHeight - subTexture.height - top;
            Vector2 offset = new Vector2(left - right, bottom - top) / 2;
            Vector2 vertex = new Vector2(subTexture.width, subTexture.height) / 2;

            verts = new Vector2[4]
            {
                new Vector2( vertex.x,  vertex.y) + offset,
                new Vector2(-vertex.x,  vertex.y) + offset,
                new Vector2(-vertex.x, -vertex.y) + offset,
                new Vector2( vertex.x, -vertex.y) + offset
            };
        }
    }
}
