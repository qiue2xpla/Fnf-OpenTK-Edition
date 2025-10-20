namespace Fnf.Framework.TrueType.Parsing.Tables
{
    /// <summary>
    /// Contains the location of glyphs based on their index
    /// </summary>
    public class Loca
    {
        public uint[] glyphLocations;

        public Loca(FontParser parser, Maxp maxp, Head head)
        {
            // Note: The last glyph location is not a glyph offset. Its for getting the glyph length
            var reader = parser.reader;

            glyphLocations = new uint[maxp.numGlyphs + 1];
            reader.GoTo(parser.tableOffsets["loca"]);
            for (int i = 0; i < glyphLocations.Length; i++)
            {
                glyphLocations[i] = head.indexToLocFormat == 1 ? reader.ReadUInt32() : reader.ReadUInt16() * 2u;
            }
        }
    }
}