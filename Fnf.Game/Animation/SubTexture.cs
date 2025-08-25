using System.Xml;

namespace Fnf.Game
{
    internal struct SubTexture
    {
        public string name;
        public int x;
        public int y;
        public int width;
        public int height;

        public int frameX;
        public int frameY;
        public int frameWidth;
        public int frameHeight;

        public SubTexture(string name, int x, int y, int width, int height)
        {
            this.name = name;
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;

            frameX = 0;
            frameY = 0;
            frameWidth = width;
            frameHeight = height;
        }

        public SubTexture(string name, int x, int y, int width, int height, int frameX, int frameY, int frameWidth, int frameHeight)
        {
            this.name = name;
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;

            this.frameX = frameX;
            this.frameY = frameY;
            this.frameWidth = frameWidth;
            this.frameHeight = frameHeight;
        }

        public SubTexture(XmlReader reader)
        {
            name = GetString("name");
            x = GetInt("x");
            y = GetInt("y");
            width = GetInt("width");
            height = GetInt("height");

            if (GetString("frameX") == null)
            {
                // It has not trimming data
                frameX = 0;
                frameY = 0;
                frameWidth = width;
                frameHeight = height;
            }
            else
            {
                frameX = GetInt("frameX");
                frameY = GetInt("frameY");
                frameWidth = GetInt("frameWidth");
                frameHeight = GetInt("frameHeight");
            }

            string GetString(string attribname) => reader.GetAttribute(attribname);
            int GetInt(string attribname) => int.Parse(reader.GetAttribute(attribname));
        }
    }
}