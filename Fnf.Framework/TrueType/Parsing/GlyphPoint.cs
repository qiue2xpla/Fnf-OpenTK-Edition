namespace Fnf.Framework.TrueType.Parsing
{
    public struct GlyphPoint
    {
        public int x, y;
        public bool onCurve;

        public GlyphPoint(int x, int y, bool onCurve)
        {
            this.x = x;
            this.y = y;
            this.onCurve = onCurve;
        }
    }
}