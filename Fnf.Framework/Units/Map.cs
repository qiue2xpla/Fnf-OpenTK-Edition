namespace Fnf.Framework
{
    public class Map<T>
    {
        public readonly int width;
        public readonly int height;
        public readonly int size;

        public T this[int x, int y]
        {
            get => values[y * width + x];
            set => values[y * width + x] = value;
        }
        public T this[int i]
        {
            get => values[i];
            set => values[i] = value;
        }

        T[] values;

        public Map(int width, int height, T defaultValue = default)
        {
            this.width = width;
            this.height = height;
            size = width * height;
            values = new T[width * height];

            for (int i = 0; i < width * height; i++)
            {
                values[i] = defaultValue;
            }
        }
    }
}