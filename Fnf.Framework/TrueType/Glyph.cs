using Fnf.Framework.TrueType.Parsing.Tables;
using Fnf.Framework.TrueType.Parsing;
using System.Collections.Generic;
using System;

namespace Fnf.Framework.TrueType
{
    public struct Glyph
    {
        public char unicode;
        public Curve[] curves;
        public GlyphMetrics metrics;

        public Glyph(GlyphData glyph, char unicode, Head head, Hmtx hmtx)
        {
            this.unicode = unicode;
            curves = GetCurves(glyph.points, glyph.countourEndPoints);
            metrics = new GlyphMetrics()
            {
                UnitsPerEm = head.unitsPerEm,
                AdvanceWidth = hmtx.advanceWidth[glyph.glyphIndex],
                LeftSideBearing = hmtx.leftSideBearing[glyph.glyphIndex],
                bounds = new Rectangle(glyph.xMin, glyph.xMax, glyph.yMax, glyph.yMin)
            };
        }

        /// <summary>
        /// Converts raw points to curves
        /// </summary>
        private static Curve[] GetCurves(GlyphPoint[] points, int[] contourEndPoints)
        {
            List<Curve> result = new List<Curve>();
            foreach (GlyphPoint[] contour in GetContours(points, contourEndPoints))
            {
                GlyphPoint[] impliedContour = GetImplidContour(contour);

                for (int i = 0; i < impliedContour.Length; i++)
                {
                    if (!impliedContour[i].onCurve)
                    {
                        Vector2 a = get(impliedContour[wrap(i - 1)]);
                        Vector2 b = get(impliedContour[i]);
                        Vector2 c = get(impliedContour[wrap(i + 1)]);

                        Curve cur = new Curve(a, b, c);

                        if (cur.isSplitable())
                        {
                            (Curve c1, Curve c2) = cur.Split();
                            result.Add(c1);
                            result.Add(c2);
                        }
                        else
                        {
                            result.Add(cur);
                        }

                        Vector2 get(GlyphPoint p) => new Vector2(p.x, p.y);
                        int wrap(int index) => index < 0 ? impliedContour.Length + index : index % impliedContour.Length;
                    }
                }
            }
            return result.ToArray();
        }

        /// <summary>
        /// Splits the points into seperate contours
        /// </summary>
        private static GlyphPoint[][] GetContours(GlyphPoint[] points, int[] contourEndPoints)
        {
            GlyphPoint[][] contours = new GlyphPoint[contourEndPoints.Length][];

            int startIndex = 0;

            for (int i = 0; i < contours.Length; i++)
            {
                // Get the contour data length
                int length = contourEndPoints[i] - startIndex + 1;

                // Get set the data range
                contours[i] = GetRange(length);

                // Update pointer
                startIndex = contourEndPoints[i] + 1;
            }

            return contours;

            GlyphPoint[] GetRange(int length)
            {
                GlyphPoint[] range = new GlyphPoint[length];
                for (int i = startIndex; i < startIndex + length; i++)
                {
                    range[i - startIndex] = points[i];
                }
                return range;
            }
        }

        /// <summary>
        /// Adds the missing points between points
        /// </summary>
        private static GlyphPoint[] GetImplidContour(GlyphPoint[] contour)
        {
            List<GlyphPoint> result = new List<GlyphPoint>();

            for (int i = 0; i < contour.Length; i++)
            {
                result.Add(contour[i]);

                // If the next point is the same type then add a mid point
                int next = (i + 1) % contour.Length;
                if (contour[i].onCurve == contour[next].onCurve)
                {
                    result.Add(new GlyphPoint()
                    {
                        x = (int)((contour[i].x + contour[next].x) / 2f),
                        y = (int)((contour[i].y + contour[next].y) / 2f),
                        onCurve = !contour[i].onCurve
                    });
                }
            }
            return result.ToArray();
        }
    }
}