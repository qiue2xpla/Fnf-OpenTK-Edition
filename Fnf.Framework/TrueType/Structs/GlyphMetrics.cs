namespace Fnf.Framework.TrueType
{
    public struct GlyphMetrics
    {
        public int UnitsPerEm;
        public int AdvanceWidth;
        public int AdvanceHeight;
        public int LeftSideBearing;
        public int UpperSideBearing;
        public int MinX;
        public int MaxX;
        public int MinY;
        public int MaxY;

        public int Width => MaxX - MinX;
        public int Height => MaxY - MinY;
    }
}