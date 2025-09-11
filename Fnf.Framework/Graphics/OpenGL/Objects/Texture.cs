using OpenTK.Graphics.OpenGL;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;

namespace Fnf.Framework.Graphics
{
    public static class Texture
    {
        public static void Destroy(int id)
        {
            if (id <= OpenGL.NULL) return;
            GL.DeleteTexture(id);
        }

        public static int GenerateFromPath(string path, out Size size)
        {
            if(!File.Exists(path)) throw new System.IO.FileNotFoundException(path);

            using (Bitmap bitmap = new Bitmap(path))
            {
                size = new Size(bitmap.Width, bitmap.Height);
                return GenerateFromBitmap(bitmap);
            }
        }

        public static int GenerateFromBitmap(Bitmap bitmap)
        {
            BitmapData bitmapData = bitmap.LockBits(
                new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                ImageLockMode.ReadOnly,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            int id = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, id);
            GL.TexImage2D(
                    TextureTarget.Texture2D, 0,
                    PixelInternalFormat.Rgba,
                    bitmap.Width, bitmap.Height, 0,
                    OpenTK.Graphics.OpenGL.PixelFormat.Bgra,
                    PixelType.UnsignedByte,
                    bitmapData.Scan0);

            bitmap.UnlockBits(bitmapData);

            SetFilter(id, Filter.Linear, Filter.Linear);
            SetWrap(id, WrapMode.Repeat, WrapMode.Repeat);

            GL.BindTexture(TextureTarget.Texture2D, OpenGL.NULL);

            return id;
        }

        public static void SetActive(int id, int index)
        {
            GL.ActiveTexture(TextureUnit.Texture0 + index);
            GL.BindTexture(TextureTarget.Texture2D, id);
        }

        public static void Use(int id)
        {
            GL.BindTexture(TextureTarget.Texture2D, id);
        }

        public static void SetFilter(int id, Filter min, Filter mag)
        {
            GL.TextureParameter(id, TextureParameterName.TextureMinFilter, (int)min);
            GL.TextureParameter(id, TextureParameterName.TextureMagFilter, (int)mag);
        }

        public static void SetWrap(int id, WrapMode horizontal, WrapMode vertical)
        {
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)horizontal);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)vertical);
        }
    }
}