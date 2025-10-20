namespace Fnf.Framework
{
    public struct SizeF
    {
        public float width, height;
        public float slobe => (float)height / width;

        public SizeF(int width, int height)
        {
            this.width = width;
            this.height = height;
        }

        public override int GetHashCode()
        {
            return -(int)(width * height * 100);
        }

        public override bool Equals(object obj)
        {
            if (obj is SizeF v)
            {
                return v.width == width && v.height == height;
            }

            return false;
        }

        public static SizeF operator *(SizeF left, float right)
        {
            left.width = left.width * right;
            left.height = left.height * right;
            return left;
        }
        public static SizeF operator /(SizeF left, float right)
        {
            left.width = left.width / right;
            left.height = left.height / right;
            return left;
        }
        public static bool operator ==(SizeF left, SizeF right)
        {
            return left.Equals(right);
        }
        public static bool operator !=(SizeF left, SizeF right)
        {
            return !left.Equals(right);
        }

        public Vector2 ToVector2()
        {
            return new Vector2(width, height);
        }
    }
}