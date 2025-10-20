namespace Fnf.Framework
{
    /// <summary>
    /// A grid of data that can be anything
    /// </summary>
    public class Map<T>
    {
        public readonly int width;
        public readonly int height;
        public readonly int size;
        T[] values;

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

        public Map(int width, int height, T defaultValue = default)
        {
            this.width = width;
            this.height = height;
            values = new T[width * height];
            size = values.Length;
            for (int i = 0; i < size; i++) values[i] = defaultValue;
        }
    }
}