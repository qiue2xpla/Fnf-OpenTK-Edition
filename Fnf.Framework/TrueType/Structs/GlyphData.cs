namespace Fnf.Framework.TrueType
{
    internal struct GlyphData
    {
        public ushort[] CountourEndPoints;
        public GlyphPoint[] points;

        public int xMin;
        public int yMin;
        public int xMax;
        public int yMax;
    }
}