using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using System.Linq;
using OpenTK.Graphics.OpenGL;

namespace Fnf.Framework.Graphics
{
    public static class Texture
    {
        public static Dictionary<string, int> LoadedTextures = new Dictionary<string, int>();

        public static void Destroy(int id)
        {
            if (id <= 0) return;
            if(LoadedTextures.ContainsValue(id))
            {
                foreach (var item in LoadedTextures.Where(kvp => kvp.Value == id).ToList())
                {
                    LoadedTextures.Remove(item.Key);
                }
            };
            GL.DeleteTexture(id);
        }

        public static int GenerateFromPath(string path, bool newInstance = true)
        {
            if(!File.Exists(path)) throw new FileNotFoundException(path);

            int id;

            if (newInstance)
            {
                id = GL.GenTexture();
                string textureName = $"New{id}-{path}";
                LoadedTextures.Add(textureName, id);
                LoadImage();
                return id;
            }
            else
            {
                if (LoadedTextures.ContainsKey(path))
                {
                    return LoadedTextures[path];
                }
                else
                {
                    id = GL.GenTexture();
                    LoadedTextures.Add(path, id);
                    LoadImage();
                    return id;
                }
            }

            void LoadImage()
            {
                Bitmap bitmap = new Bitmap(path);
                BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                GL.BindTexture(TextureTarget.Texture2D, id);
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bitmap.Width, bitmap.Height, 0,
                    OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, bitmapData.Scan0);

                bitmap.UnlockBits(bitmapData);
                bitmap.Dispose();

                SetFilter(id, Filter.Linear, Filter.Linear);
                SetWrap(id, WrapMode.Repeat, WrapMode.Repeat);
                Use(0);
            }
        }

        public static int GenerateFromBitmap(Bitmap bitmap, string optionalName = null)
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

            GL.BindTexture(TextureTarget.Texture2D, 0);
            LoadedTextures.Add("Bitmap" + id + (optionalName == null ? "-" + optionalName : ""), id);

            return id;
        }

        public static Size GetTextureSize(int id)
        {
            Use(id);
            GL.GetTextureLevelParameter(id, 0, GetTextureParameter.TextureWidth, out int width);
            GL.GetTextureLevelParameter(id, 0, GetTextureParameter.TextureHeight, out int height);
            Use(0);
            return new Size(width, height);
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