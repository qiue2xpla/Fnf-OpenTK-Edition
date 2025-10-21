using System.Collections.Generic;
using System.Threading.Tasks;

namespace Fnf.Framework.TrueType.Rendering
{
    public static class GlyphRenderer
    {
        public static void Render(Map<float> map, Glyph glyph, Rectangle rect)
        {
            Rectangle glyphBounds = glyph.metrics.bounds;

            Parallel.For(0, rect.size.height, RenderLine);

            void RenderLine(int y)
            {
                float ry = MathUtility.Map(y + 0.5f, 0, rect.size.height - 1, glyphBounds.top, glyphBounds.bottom);

                List<float> TotalRoots = new List<float>();
                for (int i = 0; i < glyph.curves.Length; i++)
                {
                    float p0 = glyph.curves[i].p0.y - ry;
                    float p2 = glyph.curves[i].p2.y - ry;

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

                    (float r0, float r1) = glyph.curves[i].GetRoots(ry);

                    if (r0 >= 0 && r0 <= 1)
                    {
                        TotalRoots.Add(glyph.curves[i].LerpX(r0));
                        continue;
                    }

                    if (r1 >= 0 && r1 <= 1)
                    {
                        TotalRoots.Add(glyph.curves[i].LerpX(r1));
                        continue;
                    }
                }

                TotalRoots.Sort();

                for (int x = 0; x < rect.size.width; x++)
                {
                    float rx = MathUtility.Map(x + 0.5f, 0, rect.size.width - 1, glyphBounds.left, glyphBounds.right);

                    int times = 0;
                    for (int i = 0; i < TotalRoots.Count; i++) if (TotalRoots[i] < rx) times++;

                    bool isInside = times % 2 != 0;
                    map[rect.position.x + x, rect.position.y + y] = isInside ? 1 : 0;
                }
            }
        }
    }
}