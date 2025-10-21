namespace Fnf.Framework
{
    public struct Rectangle
    {
        public Point position;
        public Size size;

        public int left => position.x;
        public int right => position.x + size.width - 1;
        public int bottom => position.y;
        public int top => position.y + size.height - 1;

        public Rectangle(Point position, Size size)
        {
            this.position = position;
            this.size = size;
        }

        public Rectangle(int left, int right, int top, int bottom)
        {
            position = new Point(left, bottom);
            size = new Size(right - left + 1, top - bottom + 1);
        }
    }
}