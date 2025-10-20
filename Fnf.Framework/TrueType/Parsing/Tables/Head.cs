namespace Fnf.Framework.TrueType.Parsing.Tables
{
    /// <summary>
    /// Contains some information about the font
    /// </summary>
    public class Head
    {
        public int unitsPerEm;
        public int indexToLocFormat;

        public Head(FontParser parser)
        {
            var reader = parser.reader;

            reader.GoTo(parser.tableOffsets["head"]);

            reader.Skip(4); // version
            reader.Skip(4); // fontRevision
            reader.Skip(sizeof(uint)); // chechSumAdjustment
            reader.Skip(sizeof(uint)); // macigNumber
            reader.Skip(sizeof(ushort)); // flags
            unitsPerEm = reader.ReadUInt16();
            reader.Skip(sizeof(long)); // created
            reader.Skip(sizeof(long)); // modified
            reader.Skip(sizeof(short)); // xMin
            reader.Skip(sizeof(short)); // yMin
            reader.Skip(sizeof(short)); // xMax
            reader.Skip(sizeof(short)); // yMax
            reader.Skip(sizeof(ushort)); // macStyle
            reader.Skip(sizeof(ushort)); // lowestRecPPEM
            reader.Skip(sizeof(short)); // fontDirectionHint
            indexToLocFormat = reader.ReadInt16();
            reader.Skip(sizeof(short)); // glyphDataFormat (usually 0)
        }
    }
}