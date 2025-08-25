using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.IO;
using Fnf.Framework.Graphics;

namespace Fnf.Framework
{
    public class TextureAtlas
    {
        public Texture texture;
        public SubTexture[] subTextures;

        public TextureAtlas(string atlasName)
        {
            subTextures = GetSubTextures(atlasName);
        }

        public SubTexture[] GetSubTexturesByName(string name)
        {
            List<SubTexture> result = new List<SubTexture>();

            for (int i = 0; i < subTextures.Length; i++)
            {
                if (subTextures[i].name.StartsWith(name))
                { 
                    result.Add(subTextures[i]);
                }
            }

            if (result.Count == 0) return null;

            return result.OrderBy(x => x.name).ToArray();
        }

        public Frame[] GetFramesByName(string name)
        {
            SubTexture[] subs = GetSubTexturesByName(name);
            Frame[] frames = new Frame[subs.Length];

            for (int i = 0; i < frames.Length; i++)
            {
                frames[i] = new Frame(subs[i], texture.width, texture.height);
            }

            return frames;
        }

        public static SubTexture[] GetSubTextures(string xmlpath)
        {
            List<SubTexture> SubTextures = new List<SubTexture>(0);

            using (var stringreader = new StringReader(File.ReadAllText(xmlpath + ".xml")))
            using (var xmlreader = new XmlTextReader(stringreader))
            {
                while (xmlreader.Read())
                {
                    if (xmlreader.Name != "SubTexture") continue;

                    string Name = GetString("name");
                    int x = GetInt("x");
                    int y = GetInt("y");
                    int width = GetInt("width");
                    int height = GetInt("height");

                    if (GetString("frameX") == null) //if not trimmed
                    {
                        SubTextures.Add(new SubTexture(Name, x, y, width, height));
                    }
                    else
                    {
                        int frameX = GetInt("frameX");
                        int frameY = GetInt("frameY");
                        int frameWidth = GetInt("frameWidth");
                        int frameHeight = GetInt("frameHeight");

                        SubTextures.Add(new SubTexture(Name, x, y, width, height, frameX, frameY, frameWidth, frameHeight));
                    }

                }

                string GetString(string attribname)
                {
                    return xmlreader.GetAttribute(attribname);
                }

                int GetInt(string attribname)
                {
                    return int.Parse(xmlreader.GetAttribute(attribname));
                }
            }

            return SubTextures.ToArray();
        }

        /*public static string AddNumberToName(string name, int number)
        {
            if (number > 9999) throw new Exception("The number is too big.");

            for (int i = 0; i < 4 - number.ToString().Length; i++)
            {
                name += "0";
            }

            name += number.ToString();

            return name;
        }*/

        /*public string[] GetAnimationNames()
        {
            List<string> names = new List<string>();

            for (int i = 0; i < frames.Length; i++)
            {
                string name = frames[i].rawname;
                if (!names.Contains(name)) names.Add(name);
            }

            return names.ToArray();
        }*/

        /*public SubTexture[] GetSubTexturesByIndices(string name, params int[] indices)
        {
            SubTexture[] subs = new SubTexture[indices.Length];

            for (int i = 0; i < indices.Length; i++)
            {
                string sn = SubTexture.AddNumberToName(name, indices[i]);
                bool found = false;

                for (int a = 0; a < frames.Length; a++)
                {
                    if (frames[a].name == sn)
                    {
                        subs[i] = frames[a];
                        found = true;
                        break;
                    }
                }

                if (!found) throw new IndexOutOfRangeException(indices[i].ToString());
            }

            return subs;
        }*/

        /*public Frame[] GetFramesByIndices(string name, params int[] indices)
        {
            SubTexture[] main = GetSubTexturesByIndices(name, indices);

            Frame[] frames = new Frame[main.Length];

            for (int i = 0; i < frames.Length; i++)
            {
                frames[i] = new Frame(main[i], texture);
            }

            return frames;
        }*/
    }
}