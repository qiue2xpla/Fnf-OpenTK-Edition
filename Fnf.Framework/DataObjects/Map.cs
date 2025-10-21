using Fnf.Framework.TrueType.Rasterization;
using OpenTK.Graphics.OpenGL;
using System;
using System.Drawing;

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

        public Bitmap ToBitmap(ColorCalculation<T> action)
        {
            FastBitmap fastBitmap = new FastBitmap(width, height);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    fastBitmap.SetPixel(x, y, action.Invoke(this[x,y]));
                }
            }

            Bitmap result = fastBitmap.bitmap;
            fastBitmap.Dispose();
            return result;
        }

        public delegate Color ColorCalculation<Targ>(Targ a);
    }
}