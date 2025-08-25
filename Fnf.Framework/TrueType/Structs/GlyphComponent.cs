namespace Fnf.Framework.TrueType
{
    internal struct GlyphComponent
    {
        public ushort Flags;
        public ushort GlyphIndex;
        public int Argument1;
        public int Argument2;
        public int DestPointIndex;
        public int SrcPointIndex;

        public double A, B, C, D, E, F;
    }
}