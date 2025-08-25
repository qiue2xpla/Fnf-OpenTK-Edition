using System.Collections.Generic;
using System.Threading.Tasks;

namespace Fnf.Framework.TrueType.Rasterization
{
    public static class Rasterizer
    {
        public static void RasterizeGlyph(FastBitmap bitmap, Glyph glyph, Point position, Size size, Color hitColor)
        {
            GlyphMetrics metrics = glyph.Metrics;

            Parallel.For(0, size.height, y =>
            {
                float ry = map(y + 0.5f, size.height, 0, metrics.MinY, metrics.MaxY);

                List<float> TotalRoots = new List<float>();
                for (int i = 0; i < glyph.Curves.Length; i++)
                {
                    float p0 = glyph.Curves[i].p0.y - ry;
                    float p2 = glyph.Curves[i].p2.y - ry;

                    if (p0 == p2) continue;

                    if (p0 > 0 || p2 < 0) //if downward curve
                    {
                        if (p0 <= 0 && p2 < 0) continue; // Under The Line
                        if (p0 >= 0 && p2 > 0) continue; // Above The Line
                    }
                    else
                    {
                        if (p0 < 0 && p2 <= 0) continue; // Under The Line
                        if (p0 > 0 && p2 >= 0) continue; // Above The Line
                    }

                    (float r0, float r1) = glyph.Curves[i].GetRoots(ry);

                    if (r0 >= 0 && r0 <= 1)
                    {
                        TotalRoots.Add(glyph.Curves[i].LerpX(r0));
                        continue;
                    }

                    if (r1 >= 0 && r1 <= 1)
                    {
                        TotalRoots.Add(glyph.Curves[i].LerpX(r1));
                        continue;
                    }
                }

                for (int x = 0; x < size.width; x++)
                {
                    float rx = map(x + 0.5f, 0, size.width, metrics.MinX, metrics.MaxX);
               
                    int times = 0;
                    for (int i = 0; i < TotalRoots.Count; i++) if (TotalRoots[i] > rx) times++;

                    bool isInside = times % 2 != 0;
                    if(isInside) bitmap.SetPixel(position.x + x, position.y + y, hitColor);
                }
            });

            float map(float x, float in_min, float in_max, float out_min, float out_max)
            {
                return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
            }
        }
    }
}