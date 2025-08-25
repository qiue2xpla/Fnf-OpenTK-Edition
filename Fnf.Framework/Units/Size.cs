namespace Fnf.Framework
{
    /// <summary>
    /// Used to store a width and a hight intigers and can provide a slobe
    /// </summary>
    public struct Size
    {
        public int width, height;
        public float slobe => (float)height / width;

        public Size(int width, int height)
        {
            this.width = width;
            this.height = height;
        }

        public override int GetHashCode()
        {
            return -width * height;
        }

        public override bool Equals(object obj)
        {
            if (obj is Size v)
            {
                return v.width == width && v.height == height;
            }

            return false;
        }

        public static Size operator *(Size left, float right)
        {
            left.width = (int)(left.width * right);
            left.height = (int)(left.height * right);
            return left;
        }
        public static Size operator /(Size left, float right)
        {
            left.width = (int)(left.width / right);
            left.height = (int)(left.height / right);
            return left;
        }
        public static bool operator ==(Size left, Size right)
        {
            return left.Equals(right);
        }
        public static bool operator !=(Size left, Size right)
        {
            return !left.Equals(right);
        }

        public Vector2 ToVector2()
        {
            return new Vector2(width, height);
        }
    }
}