namespace Fnf.Framework.TrueType.Parsing
{
    public struct GlyphData
    {
        public int[] countourEndPoints;
        public GlyphPoint[] points;
        public byte[] flags;

        public int numberOfContours;
        public int glyphIndex;
        public int xMin;
        public int yMin;
        public int xMax;
        public int yMax;
    }
}