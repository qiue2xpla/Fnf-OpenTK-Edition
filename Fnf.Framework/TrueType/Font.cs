using System.Collections.Generic;

namespace Fnf.Framework.TrueType
{
    /// <summary>
    /// Loads glyphs from a truetype font
    /// </summary>
    public sealed class Font
    {
        public Dictionary<char, Glyph> Glyphs => referance.Glyphs;
        public char MissingChar => referance.MissingChar;

        FontBase referance;

        public Font(string fontName)
        {
            // Cache
            // Make sure a pool for fonts exits
            ResourceManager.MakeNewPool<string,FontBase>("Fonts");

            // Get a referance to the pool
            var pool = ResourceManager.GetResourcePool<string, FontBase>("Fonts");

            // Chech if the font is already cached
            if (pool.entries.ContainsKey(fontName))
            {
                // Use the already existing one
                referance = pool.entries[fontName];
            }
            else
            {
                // Make a new one
                using (FontReader reader = new FontReader(fontName))
                using (FontParser parser = new FontParser(reader))
                {
                    referance = new FontBase()
                    {
                        MissingChar = parser.MissingChar,
                        Glyphs = parser.GetGlyphs()
                    };
                    pool.entries.Add(fontName, referance);
                }
            }
        }
    }

    class FontBase
    {
        public Dictionary<char, Glyph> Glyphs;
        public char MissingChar; // TODO: Move the missing glyph to the top of the dictionary

        public override string ToString()
        {
            return $"{Glyphs.Count} Glyphs";
        }
    }
}