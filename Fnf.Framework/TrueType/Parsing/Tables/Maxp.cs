namespace Fnf.Framework.TrueType.Parsing.Tables
{
    /// <summary>
    /// Contains the maximum expected values for validation and reducing memory usage
    /// </summary>
    public class Maxp
    {
        // int version;
        public int numGlyphs;
        // ushort maxPoints;
        // ushort maxContours;
        // ushort maxCompositePoints;
        // ushort maxCompositeContours;
        // ushort maxZones;
        // ushort maxTwilightPoints;
        // ushort maxStorage;
        // ushort maxFunctionDefs;
        // ushort maxInstructionDefs;
        // ushort maxStackElements;
        // ushort maxSizeOfInstructions;
        // ushort maxComponentElements;
        // ushort maxComponentDepth;

        public Maxp(FontParser parser)
        {
            var reader = parser.reader;

            reader.GoTo(parser.tableOffsets["maxp"]);

            reader.Skip(sizeof(int)); // version
            numGlyphs = reader.ReadUInt16();
        }
    }
}