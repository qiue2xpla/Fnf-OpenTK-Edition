using System.Numerics;
using System;

namespace Fnf.Framework
{
    public class Frame
    {
        public Vector2[] verts;
        public Vector2[] coords;

        public Frame(SubTexture subTexture, int textureWidth, int textureHeight)
        {
            SetCoords(subTexture, textureWidth, textureHeight);
            SetVerts(subTexture);
        }

        void SetCoords(SubTexture data, float tw, float th)
        {
            float x = data.x;
            float y = data.y;
            float w = data.width;
            float h = data.height;

            float minX = x / tw;
            float maxX = (x + w) / tw;
            float minY = y / th;
            float maxY = (y + h) / th;

            coords = new Vector2[]
            {
                new Vector2(maxX, minY),
                new Vector2(minX, minY),
                new Vector2(minX, maxY),
                new Vector2(maxX, maxY)
            };
        }

        void SetVerts(SubTexture subTexture)
        {
            Vector2 offset = GetOffset(subTexture);
            float x = subTexture.width / 2f;
            float y = subTexture.height / 2f;

            verts = new Vector2[4]
            {
                new Vector2( x,  y),
                new Vector2(-x,  y),
                new Vector2(-x, -y),
                new Vector2( x, -y)
            };

            for (int i = 0; i < 4; i++)
            {
                verts[i] += offset;
            }
        }

        Vector2 GetOffset(SubTexture data)
        {
            float left = Math.Abs(data.frameX);
            float top = Math.Abs(data.frameY);
            float right = data.frameWidth - data.width - left;
            float bottom = data.frameHeight - data.height - top;

            Vector2 offset = new Vector2
            (
                (left - right) / 2,
                (bottom - top) / 2
            );

            return offset;
        }
    }
}
