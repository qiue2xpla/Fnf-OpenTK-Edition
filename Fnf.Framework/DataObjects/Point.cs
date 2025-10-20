namespace Fnf.Framework
{
    public struct Point
    {
        public int x, y;

        public Point(int v)
        {
            x = v;
            y = v;
        }

        public Point(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public override string ToString()
        {
            return $"({x}, {y})";
        }
        public override int GetHashCode()
        {
            return -x * y;
        }

        public override bool Equals(object obj)
        {
            if (obj is Point v) return v.x == x && v.y == y;
            return false;
        }

        public static Point operator +(Point left, Point right)
        {
            return new Point(left.x + right.x, left.y + right.y);
        }
        public static Point operator -(Point left, Point right)
        {
            return new Point(left.x - right.x, left.y - right.y);
        }

        public static bool operator ==(Point left, Point right)
        {
            return left.Equals(right);
        }
        public static bool operator !=(Point left, Point right)
        {
            return !left.Equals(right);
        }
    }
}