namespace Fnf.Framework.TrueType
{
    internal struct GlyphPoint
    {
        public int x, y;
        public bool OnCurve;

        public GlyphPoint(int x, int y, bool OnCurve)
        {
            this.x = x;
            this.y = y;
            this.OnCurve = OnCurve;
        }
    }
}