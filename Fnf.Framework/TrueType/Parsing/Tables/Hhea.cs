namespace Fnf.Framework.TrueType.Parsing.Tables
{
    /// <summary>
    /// Contains data for loading horizontal metrics
    /// </summary>
    public class Hhea
    {
        public int numberOfHMetrics;

        public Hhea(FontParser parser)
        {
            var reader = parser.reader;
            reader.GoTo(parser.tableOffsets["hhea"]);
            reader.Skip(4); // version
            reader.Skip(2); // ascender
            reader.Skip(2); // decender
            reader.Skip(2); // lineGap
            reader.Skip(2); // advanceWidthMax
            reader.Skip(2); // minLeftSideBearing
            reader.Skip(2); // minRightSideBearing
            reader.Skip(2); // xMaxExtent
            reader.Skip(2); // caretSlopeRise
            reader.Skip(2); // caretSlopeRun
            reader.Skip(2); // caretOffset
            reader.Skip(8); // reserved
            reader.Skip(2); // metricDataFormat
            numberOfHMetrics = reader.ReadUInt16();
        }
    }
}