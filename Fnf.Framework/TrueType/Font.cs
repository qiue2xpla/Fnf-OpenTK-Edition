using Fnf.Framework.TrueType.Parsing.Tables;
using Fnf.Framework.TrueType.Parsing;
using System.Collections.Generic;

namespace Fnf.Framework.TrueType
{
    /// <summary>
    /// Represents a truetype font file's data
    /// </summary>
    public sealed class Font
    {
        // Cache
        private static Dictionary<string, Font> loadedFonts;

        // Tables
        public Cmap cmap;
        public Glyf glyf;
        public Head head;
        public Hhea hhea;
        public Hmtx hmtx;
        public Loca loca;
        public Maxp maxp;
        public Name name;

        // Font data
        public Dictionary<char, Glyph> glyphs;

        public Font(string fontName, bool lookInWindowsFonts = true)
        {
            if (loadedFonts == null) loadedFonts = new Dictionary<string, Font>();
            string uniqueFontIdentifier = "";

            using (FontReader reader = new FontReader(fontName, lookInWindowsFonts))
            using (FontParser parser = new FontParser(reader))
            {
                name = parser.GetNameTable();

                uniqueFontIdentifier = GetUniqueFontIdentifier();
                if (loadedFonts.ContainsKey(uniqueFontIdentifier))
                {
                    // Use the existing one
                    Font font = loadedFonts[uniqueFontIdentifier];

                    head = font.head;
                    maxp = font.maxp;
                    cmap = font.cmap;
                    hhea = font.hhea;
                    glyf = font.glyf;
                    hmtx = font.hmtx;
                    loca = font.loca;
                    glyphs = font.glyphs;

                    return;
                }

                // Independent tables
                head = parser.GetHeaderTable();
                maxp = parser.GetMaximumProfile();
                cmap = parser.GetCharacterMapping();
                hhea = parser.GetHorizontalHeaderTable();

                // Dependent tables
                hmtx = parser.GetHorizontalMetricsTable(hhea, maxp);
                loca = parser.GetLocationTable(maxp, head);
                glyf = parser.GetGlyphTable(cmap, loca);
            }

            // Cache the font if new
            loadedFonts.Add(uniqueFontIdentifier, this);

            glyphs = glyf.GetGlyphs(head, hmtx);
        }

        /// <summary>
        /// Returns a string that's unique to the font
        /// </summary>
        string GetUniqueFontIdentifier()
        {
            string fontName = name.fullFontName ?? "null";
            string fontFamily = name.fontFamily ?? "null";
            string fontSubFamily = name.fontSubFamily ?? "null";
            string version = name.version ?? "null";
            return $"{fontName}:{fontFamily}:{fontSubFamily}:{version}";
        }
    }
}