using System.Collections.Generic;

namespace Fnf.Framework.TrueType
{
    public sealed class Font
    {
        public readonly Dictionary<char, Glyph> Glyphs = new Dictionary<char, Glyph>();
        public readonly char MissingChar;
        public readonly int UnitsPerEm;

        public Font(string fontName)
        {
            using (FontReader reader = new FontReader(fontName))
            using (FontParser parser = new FontParser(reader))
            {
                MissingChar = parser.MissingChar;
                UnitsPerEm = parser.UnitsPerEm;
                Glyphs = parser.GetGlyphs();
            }
        }
    }
}