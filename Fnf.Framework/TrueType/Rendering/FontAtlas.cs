using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System;

using OpenTK.Graphics.OpenGL;
using Fnf.Framework.TrueType.Rendering;

namespace Fnf.Framework.TrueType.Rasterization
{
    public class FontAtlas
    {
        public Dictionary<char,SubAtlas> subAtlasses = new Dictionary<char, SubAtlas>();
        public Map<float> map;

        public string charSet;
        public int fontSize;
        public int padding;
        public int margin;
        public int spread;
        public bool isSDF;

        /// <summary>
        /// Generates a font atlas with the givven font
        /// </summary>
        /// <param name="font">The font for generating the atlas</param>
        /// <param name="fontSize">The font size in the atlas</param>
        /// <param name="padding">The spacing between the glyph and border</param>
        /// <param name="margin">The spacing between glyphs border</param>
        /// <param name="spread">The signed distance spread (0 for none)</param>
        /// <param name="customCharSet">The chars to include in the atlas (null for all chars in font)</param>
        public FontAtlas(Font font, int fontSize, int padding, int margin, int spread, string customCharSet = null) 
        {
            // Set some variables
            this.fontSize = fontSize;
            this.padding = padding;
            this.spread = spread;
            this.margin = margin;
            isSDF = spread > 0;

            // Load the glyphs
            Glyph[] glyphs;
            IEnumerable<Glyph> enumerableGlyphs;
            if (customCharSet == null)
            {
                enumerableGlyphs = font.glyphs.Values;
                charSet = new string(font.glyphs.Keys.ToArray());
            }
            else
            {
                if (!customCharSet.Contains("\uFFFF")) customCharSet += (char)0xFFFF;
                enumerableGlyphs = font.glyphs.Values.Where((glyph) => customCharSet.Contains(glyph.unicode));
                charSet = customCharSet;
            }
            glyphs = enumerableGlyphs.OrderBy(glyph => glyph.metrics.bounds.size.height).Reverse().ToArray();

            // Start making the atlas
            Size mapSize = new Size(GetOptimalBitmapWidth(glyphs.Length, fontSize), 0);
            Point pointer = new Point(margin + padding, margin + padding);
            int tallestGlyphInRow = 0;
            int minMapWidth = 0;

            // Calculate glyph layout
            for (int i = 0; i < glyphs.Length; i++)
            {
                float pixelsPerEm = (float)fontSize / glyphs[i].metrics.UnitsPerEm;
                int glyphWidth = (int)Math.Ceiling(glyphs[i].metrics.bounds.size.width * pixelsPerEm);
                int glyphHeight = (int)Math.Ceiling(glyphs[i].metrics.bounds.size.height * pixelsPerEm);

                // If the row can't take more glyphs the go to the next row
                if (pointer.x + padding + margin + glyphWidth > mapSize.width)
                {
                    if (2 * (margin + padding) + glyphWidth > mapSize.width) throw new Exception($"Someting went wrong with the atlas width '{mapSize.width}'");

                    minMapWidth = Math.Max(pointer.x - padding, minMapWidth);
                    pointer.x = margin + padding;
                    pointer.y += tallestGlyphInRow + 2 * padding + margin;
                    tallestGlyphInRow = 0;
                }

                subAtlasses.Add(glyphs[i].unicode, new SubAtlas(pointer.x, pointer.y, glyphWidth, glyphHeight, glyphs[i].curves.Length != 0));

                tallestGlyphInRow = Math.Max(tallestGlyphInRow, glyphHeight);
                if (glyphs[i].curves.Length != 0) pointer.x += glyphWidth + 2 * padding + margin;
            }

            mapSize.width = minMapWidth;
            mapSize.height = pointer.y + tallestGlyphInRow + padding + margin;

            // Apply layout to map
            map = new Map<float>(mapSize.width, mapSize.height);
            Parallel.For(0, glyphs.Length, RenderGlyph);

            void RenderGlyph(int index)
            {
                Glyph glyph = glyphs[index];
                SubAtlas sub = subAtlasses[glyph.unicode];

                GlyphRenderer.Render(map, glyph, new Rectangle(new Point(sub.x, sub.y), new Size(sub.width, sub.height)));
                //if (isSDF) SDF.ApplyToSection(map, sub.x - padding, sub.y - padding, sub.width + 2 * padding, sub.height + 2 * padding, spread); 
            }
        }

        /// <summary>
        /// Returns a custom charset with easier access
        /// </summary>
        /// <param name="charSets">A string of first char of each set( Lowercase, Uppercase, Numbers, Punctuation, Space )</param>
        public static string GetCustomCharset(string charSets)
        {
            charSets = charSets.ToUpper();

            string result = string.Empty;
            for (int i = 0; i < charSets.Length; i++)
            {
                switch(charSets[i])
                {
                    case 'P': result += "!@#$%^&*()_+=-~`'[]{}/\\.,><?:|"; break;
                    case 'U': result += "ABCDEFGHIJKLMNOPQRSTUVWXYZ"; break;
                    case 'L': result += "abcdefghijklmnopqrstuvwxyz"; break;
                    case 'N': result += "0123456789"; break;
                    case 'S': result += " "; break;
                }
            }
            return result;
        }

        /// <summary>
        /// Returns a side of a square that is optimal for fitting all glyphs in it
        /// </summary>
        int GetOptimalBitmapWidth(int glyphCount, int fontSize)
        {
            return (int)(Math.Sqrt(glyphCount) * fontSize);
        }
    }

    // The size expands in the +x -y direction
    public struct SubAtlas
    {
        public int x;
        public int y;
        public int width;
        public int height;
        public bool hasOutline;

        public SubAtlas(int x, int y, int width, int height, bool hasOutline)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
            this.hasOutline = hasOutline;
        }
    }
}