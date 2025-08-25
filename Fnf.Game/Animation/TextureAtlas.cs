using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Xml;
using System.IO;
using System;
using Fnf.Framework.Graphics;

namespace Fnf.Game
{
    public static class TextureAtlas
    {
        private struct Atlas
        {
            public SubTexture[] subTextures;
            public Framework.Size textureSize;
            public int texture;

            public Atlas(string atlasName, bool loadTexture = true)
            {
                texture = OpenGL.NULL;

                using (Bitmap bitmap = new Bitmap(atlasName + ".png"))
                {
                    textureSize = new Framework.Size(bitmap.Width, bitmap.Height);

                    if (loadTexture)
                    {
                        texture = Texture.GenerateFromBitmap(bitmap);
                    }
                }

                List<SubTexture> SubTexturesList = new List<SubTexture>(0);

                using (var stringreader = new StringReader(File.ReadAllText(atlasName + ".xml")))
                using (var xmlreader = new XmlTextReader(stringreader))
                {
                    while (xmlreader.Read())
                    {
                        if (xmlreader.Name != "SubTexture") continue;
                        SubTexturesList.Add(new SubTexture(xmlreader));
                    }

                    subTextures = SubTexturesList.ToArray();
                }
            }
        }

        private static Dictionary<string, Atlas> atlases = new Dictionary<string, Atlas>();

        public static void LoadAtlas(string name, string path)
        {
            if (atlases.ContainsKey(name)) return;
            atlases.Add(name, new Atlas(path));
        }

        public static string[] GetAnimationNames(string atlasName)
        {
            Atlas atlas = atlases[atlasName];

            List<string> names = new List<string>();

            for (int i = 0; i < atlas.subTextures.Length; i++)
            {
                string animName = atlas.subTextures[i].name;
                animName = animName.Substring(0, animName.Length - 4);
                if (!names.Contains(animName)) names.Add(animName);
            }

            return names.ToArray();
        }

        public static Animation[] GetAnimations(string atlasName, params string[] animationNames)
        {
            Animation[] animations = new Animation[animationNames.Length];
            for (int i = 0; i < animations.Length; i++) animations[i] = GetAnimation(atlasName, animationNames[i]);
            return animations;
        }

        public static Animation GetAnimation(string atlasName, string animationName)
        {
            return new Animation(GetFramesByName(atlasName, animationName), atlases[atlasName].texture) { name = animationName };
        }

        public static Animation GetAnimation(string atlasName, string animationName, params int[] indices)
        {
            return new Animation(GetFramesByIndices(atlasName, animationName, indices), atlases[atlasName].texture) { name = animationName };
        }

        /*
        public void Dispose()
        {
            texture = OpenGL.NULL;
            subTextures = null;
        }*/

        private static Frame[] GetFramesByName(string atlasName, string name)
       {
            Atlas atlas = atlases[atlasName];

            List<Frame> result = new List<Frame>();

            for (int i = 0; i < atlas.subTextures.Length; i++)
            {
                string subName = atlas.subTextures[i].name.Substring(0, atlas.subTextures[i].name.Length - 4);
                if (subName == name)
                {
                    result.Add(new Frame(atlas.subTextures[i], atlas.textureSize.width, atlas.textureSize.height));
                }
            }

            return result.OrderBy(x => x.name).ToArray();
        }

        private static Frame[] GetFramesByIndices(string atlasName, string name, params int[] indices)
        {
            Atlas atlas = atlases[atlasName];
            Frame[] subs = new Frame[indices.Length];

            for (int i = 0; i < indices.Length; i++)
            {
                string subTextureName = AddNumberToString(name, indices[i]);
                for (int a = 0; a < atlas.  subTextures.Length; a++)
                {
                    if (atlas.subTextures[a].name == subTextureName)
                    {
                        subs[i] = new Frame(atlas.subTextures[a], atlas.textureSize.width, atlas.textureSize.height);
                        break;
                    }
                }

                if (string.IsNullOrEmpty(subs[i].name)) 
                    throw new IndexOutOfRangeException(indices[i].ToString());
            }

            return subs;

            string AddNumberToString(string text, int number)
            {
                for (int i = 0; i < 4 - number.ToString().Length; i++)  text += "0";
                text += number.ToString();
                return text;
            }
        }
    }
}