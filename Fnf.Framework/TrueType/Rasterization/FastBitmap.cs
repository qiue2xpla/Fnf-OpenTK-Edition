using System.Runtime.InteropServices;
using System.Drawing;
using System;

namespace Fnf.Framework.TrueType.Rasterization
{
    /// <summary>
    /// Has the same functionality as a normal Bitmap but aimed
    /// to be a lot faster when setting or getting pixels
    /// </summary>
    public class FastBitmap : IDisposable
    {
        public Bitmap bitmap { get; private set; }
        public readonly int height;
        public readonly int width;

        private GCHandle _bitsHandle;
        private int[] _bits;

        public FastBitmap(int width, int height)
        {
            this.width = width;
            this.height = height;
            _bits = new int[width * height];
            _bitsHandle = GCHandle.Alloc(_bits, GCHandleType.Pinned);
            bitmap = new Bitmap(width, height, width * 4, System.Drawing.Imaging.PixelFormat.Format32bppArgb, _bitsHandle.AddrOfPinnedObject());
        }

        public void SetPixel(int x, int y, int colour)
        {
            _bits[x + y * width] = colour;
        }

        public int GetPixelAsInt(int x, int y)
        {
            return _bits[x + y * width];
        }

        public void SetPixel(int x, int y, Color colour)
        {      
            _bits[x + y * width] = colour.ToArgb();
        }

        public Color GetPixel(int x, int y)
        {
            return new Color(_bits[x + y * width]);
        }

        public bool disposed { get; private set; }
        public void Dispose()
        {
            if (disposed) return;
            _bitsHandle.Free();
            disposed = true;
            bitmap = null;
            _bits = null;
        }
    }
}