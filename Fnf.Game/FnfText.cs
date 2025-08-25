using System.Collections.Generic;
using Fnf.Framework;
using System;

namespace Fnf.Game
{
    // Only supports capital characters for now
    public class FnfText : UI, IRenderable
    {
        const string CharactersToLoad = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        public bool isRenderable { get; set; } = true;
        public bool fitContent;

        string _text = "";
        public string text
        {
            get => _text;
            set
            {
                if (value == null || text == value) return;

                _text = value;

                if (fitContent)
                {
                    width = 0;

                    float[] cwl = GetLinesWidths();
                    for (int i = 0; i < cwl.Length; i++)
                        width = Math.Max(width, cwl[i]);
                }
            }
        }
        public float lineHeight = 70;
        public float lineSpacing = 0;
        public float charSpacing = 0;
        public float spaceWidth = 30;

        public TextAlignment textAlignment = TextAlignment.Center;

        static List<Animation> animations;
        public Vector2 min, max;
        Vector2 cursor;

        public FnfText()
        {
            if (animations != null) return;

            TextureAtlas.LoadAtlas("fnftext", "Assets/Shared/alphabet");

            animations = new List<Animation>();
            for (int i = 0; i < CharactersToLoad.Length; i++)
            {
                animations.Add(TextureAtlas.GetAnimation("fnftext", CharactersToLoad[i] + " bold"));
            }
        }

        public void Render()
        {
            if (!isRenderable) return;

            string[] lines = text.ToUpper().Split('\n');

            switch ((int)textAlignment & 0b0011)
            {
                case 0: cursor.y = (height - lineHeight) / 2; break;
                case 1: cursor.y = (lines.Length - 1) * (lineHeight + lineSpacing) / 2; break;
                case 2: cursor.y = (lineHeight - height) / 2; break;
            }

            foreach (string line in lines)
            {
                float lineLength = GetLineWidth(line);

                switch (((int)textAlignment & 0b1100) >> 2)
                {
                    case 0: cursor.x = -width / 2; break;
                    case 1: cursor.x = -lineLength / 2; break;
                    case 2: cursor.x = width / 2 - lineLength; break;
                }

                if(line.Length > 0 && line[0] != ' ')
                {
                    Vector2[] verts = animations[CharactersToLoad.IndexOf(line[0])].frames[0].verts;
                    float cw = verts[0].x - verts[1].x;

                    cursor.x += cw / 2;
                }

                foreach (char c in line)
                {
                    int index = CharactersToLoad.IndexOf(c);

                    if (index == -1)
                    {
                        cursor.x += spaceWidth;
                        continue;
                    }

                    Frame frame = animations[index].frames[(int)(Time.time * 30 % animations[index].frames.Length)];
                    Animator.RenderFrame(frame, animations[index].texture, cursor, globalPosition, globalScale, globalRotation);
        
                    Vector2[] verts = animations[index].frames[0].verts;
                    float cw = verts[0].x - verts[1].x;
                    cursor.x += cw + charSpacing;
                }

                cursor.y -= lineHeight + lineSpacing;
            }
        }

        public float[] GetLinesWidths()
        {
            string[] lines = text.Split('\n');
            float[] widths = new float[lines.Length];

            for (int i = 0; i < widths.Length; i++)
                widths[i] = GetLineWidth(lines[i]);

            return widths;
        }

        public float GetLineWidth(string line)
        {
            float width = 0;

            foreach (char c in line.ToUpper()) width += GetCharWidth(c);
            width += (line.Length - 1) * charSpacing;
            if (width < 0) width = 0;

            return width;
        }

        float GetCharWidth(char c)
        {
            int index = CharactersToLoad.IndexOf(c);

            if (index == -1)
            {
                return spaceWidth;
            }
            else
            {
                Vector2[] verts = animations[index].frames[0].verts;
                return verts[0].x - verts[1].x;
            }
        }
    }
}