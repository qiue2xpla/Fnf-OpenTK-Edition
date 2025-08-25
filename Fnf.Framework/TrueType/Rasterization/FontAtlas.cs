using System.Collections.Generic;
using System.Threading.Tasks;
using System.Drawing;
using System.Linq;
using System;
using System.IO;

namespace Fnf.Framework.TrueType.Rasterization
{
    public class FontAtlas
    {
        public const string UpperCase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        public const string LowerCase = "abcdefghijklmnopqrstuvwxyz";
        public const string Numbers = "1234567890";
        public const string Ponctuals = "!@#$%^&*()_+=-~`'[]{}/\\.,><?:|";
        public const string Space = " ";

        public readonly Dictionary<char,SubAtlas> subAtlasses = new Dictionary<char, SubAtlas>();
        public readonly Bitmap bitmap;

        public readonly string CharSet;
        public readonly char MissingChar;
        public readonly bool IsSignedDistanceFeild;
        public readonly int Spread;
        public readonly int FontSize;
        public readonly int Padding; // The distance between the char and its border used for sdf
        public readonly int Margin; // The minimum distance between all borders

        public FontAtlas(Font font, int fontSize, int padding, int margin, int spread, string customCharSet = null) 
        {
            // Load values
            IsSignedDistanceFeild = spread > 0;
            MissingChar = font.MissingChar;
            FontSize = fontSize;
            Padding = padding;
            Spread = spread;
            Margin = margin;

            // Load glyphs
            Glyph[] glyphs;
            if (customCharSet == null)
            {
                // Load all glyphs in font
                glyphs = font.Glyphs.Values.OrderBy(glyph => glyph.Metrics.Height).Reverse().ToArray();
                CharSet = new string(font.Glyphs.Keys.ToArray());
            }
            else
            {
                // Load glyphs from charset
                if(!customCharSet.Contains(font.MissingChar.ToString()))
                {
                    customCharSet += font.MissingChar;
                }

                CharSet = customCharSet;

                glyphs = new Glyph[customCharSet.Length];
                for (int i = 0; i < customCharSet.Length; i++)
                {
                    glyphs[i] = font.Glyphs[customCharSet[i]];
                }
                glyphs = glyphs.OrderBy(glyph => glyph.Metrics.Height).Reverse().ToArray();
            }

            // Cach
            Size bitmapSize = new Size(GetOptimalBitmapWidth(font.Glyphs.Count, fontSize), 0);
            Point cursor = new Point(margin, margin + padding);
            int highestGlyphInRow = 0;
            
            // Start getting position data
            for (int i = 0; i < glyphs.Length; i++)
            {
                float pixelsPerEm = (float)fontSize / glyphs[i].Metrics.UnitsPerEm;
                int glyphWidth = (int)Math.Ceiling(glyphs[i].Metrics.Width * pixelsPerEm);
                int glyphHeight = (int)Math.Ceiling(glyphs[i].Metrics.Height * pixelsPerEm);

                // If row cant take more glyphs go to a new line
                if (cursor.x + 2*padding + margin + glyphWidth >= bitmapSize.width)
                {
                    cursor.x = margin;
                    cursor.y += highestGlyphInRow + 2*padding + margin;
                    highestGlyphInRow = 0;
                }

                highestGlyphInRow = Math.Max(highestGlyphInRow, glyphHeight);

                cursor.x += padding;

                subAtlasses.Add(glyphs[i].Character, new SubAtlas()
                {
                    glyphMetrics = glyphs[i].Metrics,
                    hasOutline = glyphs[i].Curves.Length != 0,
                    x = cursor.x,
                    y = cursor.y,
                    width = glyphWidth,
                    height = glyphHeight
                });

                cursor.x += glyphWidth + padding + margin;
            }

            bitmapSize.height += cursor.y + highestGlyphInRow + padding + margin;

            // Apply position data to bitmap
            FastBitmap fastBitmap = new FastBitmap(bitmapSize.width,bitmapSize.height);
            Parallel.For(0, glyphs.Length, (i) =>
            {
                SubAtlas sub = subAtlasses[glyphs[i].Character];
                Rasterizer.RasterizeGlyph(fastBitmap, glyphs[i], new Point(sub.x, sub.y), new Size(sub.width, sub.height), Color.White);

                // Apply sdf
                if (IsSignedDistanceFeild)
                {
                    SDF.ApplyToSection(fastBitmap,
                        sub.x - padding,
                        sub.y - padding,
                        sub.width + 2 * padding,
                        sub.height + 2 * padding,
                        spread);
                }
            });

            bitmap = fastBitmap.bitmap;
            fastBitmap.Dispose();
        }

        public void CachAtlas(string cachName)
        {
            throw new NotImplementedException();

            if(!File.Exists($"Cach/{cachName}.png"))
            {

            }
        }

        int GetOptimalBitmapWidth(int glyphCount, int fontSize)
        {
            // Tries to give a value to make the atlas square
            return (int)(Math.Sqrt(glyphCount) * fontSize * 0.7f);
        }
    }

    public struct SubAtlas
    {
        public int x;
        public int y;
        public int width;
        public int height;
        public bool hasOutline;
        public GlyphMetrics glyphMetrics;
    }
}