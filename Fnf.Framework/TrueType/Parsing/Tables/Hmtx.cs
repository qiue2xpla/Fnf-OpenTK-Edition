namespace Fnf.Framework.TrueType.Parsing.Tables
{
    /// <summary>
    /// Contains the horizontal glyph metrics
    /// </summary>
    public class Hmtx
    {
        public int[] advanceWidth;
        public int[] leftSideBearing;

        public Hmtx(FontParser parser, Hhea hhea, Maxp maxp)
        {
            var reader = parser.reader;

            advanceWidth = new int[maxp.numGlyphs];
            leftSideBearing = new int[maxp.numGlyphs];

            reader.GoTo(parser.tableOffsets["hmtx"]);
            for (int i = 0; i < hhea.numberOfHMetrics; i++)
            {
                advanceWidth[i] = reader.ReadUInt16();
                leftSideBearing[i] = reader.ReadInt16();
            }

            int lastAdvanceWidth = advanceWidth[hhea.numberOfHMetrics-1];
            for (int i = hhea.numberOfHMetrics; i < maxp.numGlyphs; i++)
            {
                advanceWidth[i] = lastAdvanceWidth;
                leftSideBearing[i] = reader.ReadInt16();
            }
        }
    }
}