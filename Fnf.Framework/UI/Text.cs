using Fnf.Framework.TrueType.Rasterization;
using Fnf.Framework.TrueType;
using Fnf.Framework.Graphics;
using System.Collections.Generic;
using System;
using System.Linq;

namespace Fnf.Framework
{
    public class Text : UI, IRenderable
    {
        #region Mesh Updating Variables

        private string _text = "";
        public string text
        {
            get => _text;
            set
            {
                if(value == null)
                {
                    _text = "";
                }
                else
                {
                    if (value == _text) return;
                    _text = value;

                    if(fitContent) width = GetLinesWidths().Max();

                    mustReNewBuffer = true;
                    mustReNewMesh = true;
                }
            }
        }

        private bool _fitContent;
        public bool fitContent
        {
            get => _fitContent;
            set
            {
                if (value == _fitContent) return;
                _fitContent = value;
                mustReNewMesh = true;
            }
        }

        private FontAtlas _atlas;
        public FontAtlas atlas
        {
            get => _atlas;
            set
            {
                if (value == _atlas) return;
                _atlas = value;
                mustReNewMesh = true;
                Texture.Destroy(texture);
                texture = Texture.GenerateFromBitmap(value.bitmap, "FontAtlas");
                textureSize = new Size(value.bitmap.Width, value.bitmap.Height);
            }
        }

        TextAlignment _textAlignment = TextAlignment.Center;
        public TextAlignment textAlignment
        {
            get => _textAlignment;
            set
            {
                if (value == _textAlignment) return;
                _textAlignment = value;
                mustReNewMesh = true;
            }
        }

        public bool isRenderable { get; set; } = true;

        #endregion

        #region Whose Effect is Instant Variable

        public float fontSize = 18;

        public Color color = Color.White;

        #endregion

        const int BytesPerChar = 4 * 6 * sizeof(float); // 4 ( xyuv ) * 6 ( 2 triangles ) * float size

        int bufferSizeInChars = 50;
        int charCount = 0;

        // For validating changes
        bool mustReNewBuffer = false;
        bool mustReNewMesh = false;
        float w, h;

        static int DefaultShader, SDFShader;
        int vbo, vao, texture;
        Size textureSize;

        public Text(FontAtlas atlas = null)
        {
            this.atlas = atlas;

            if (DefaultShader == 0)
            {
                DefaultShader = Shader.GenerateShaderFromResource("defaultFont");
                Shader.Use(DefaultShader);
                Shader.Uniform1(DefaultShader, "tex", 0);
                Shader.Use(OpenGL.NULL);
            }

            if (SDFShader == 0)
            {
                SDFShader = Shader.GenerateShaderFromResource("sdfFont");
                Shader.Use(SDFShader);
                Shader.Uniform1(SDFShader, "tex", 0);
                Shader.Use(OpenGL.NULL);
            }

            vbo = VBO.GenerateVBO();
            vao = VAO.GenerateVAO();

            VAO.Use(vao);
            VBO.Use(vbo);
            VBO.Resize(bufferSizeInChars * BytesPerChar);
            VertexAttrib2f2f.UseAttrib();
            VAO.Use(OpenGL.NULL);
            VBO.Use(OpenGL.NULL);
        }

        public void Render()
        {
            if (!isRenderable) return;
            if (raycast && IsInside()) SetTopControl();
            if (atlas == null) return;

            if(w != width || h != height)
            {
                w = width;
                h = height;
                mustReNewMesh = true;
            }

            if (mustReNewBuffer)
            {
                mustReNewBuffer = false;
                ResizeBuffer();
            }

            if (mustReNewMesh)
            {
                mustReNewMesh = false;
                UpdateBuffer();
            }

            if (atlas.IsSignedDistanceFeild)
            {
                Shader.Use(SDFShader);
                Shader.Uniform3(SDFShader, "textColor", color.r / 255f, color.g / 255f, color.b / 255f);
                Shader.Uniform2(SDFShader, "UnitsPerPixel", Window.PixelToViewport(1, 1));
                Shader.Uniform1(SDFShader, "FontSize", fontSize);
                                
                Shader.Uniform2(SDFShader, "position", globalPosition);
                Shader.Uniform2(SDFShader, "scale", globalScale);
                Shader.Uniform1(SDFShader, "rotation", -globalRotation / 180 * (float)Math.PI);
            }
            else
            {
                Shader.Use(DefaultShader);
                Shader.Uniform3(DefaultShader, "textColor", color.r / 255f, color.g / 255f, color.b / 255f);
                Shader.Uniform2(DefaultShader, "UnitsPerPixel", Window.PixelToViewport(1, 1));
                Shader.Uniform1(DefaultShader, "FontSize", fontSize);
                
                Shader.Uniform2(DefaultShader, "position", globalPosition);
                Shader.Uniform2(DefaultShader, "scale", globalScale);
                Shader.Uniform1(DefaultShader, "rotation", -globalRotation / 180 * (float)Math.PI);
            }

            VAO.Use(vao);
            Texture.Use(texture);

            OpenGL.DrawArrays(DrawMode.Triangles, charCount * 6);

            Texture.Use(OpenGL.NULL);
            VAO.Use(OpenGL.NULL);

            Shader.Use(OpenGL.NULL);
        }

        public float GetLineWidth(string line)
        {
            return GetLineWithInEM(line) * fontSize;
        }

        public float[] GetLinesWidths()
        {
            string[] lines = _text.Split('\n');
            float[] widths = new float[lines.Length];

            for (int i = 0; i < widths.Length; i++)
            {
                widths[i] = GetLineWidth(lines[i]);
            }

            return widths;
        }

        void ResizeBuffer()
        {
            bool needsResizing = false;

            // Scale up
            while (_text.Length >= bufferSizeInChars)
            {
                bufferSizeInChars += 500;
                needsResizing = true;
            }

            // Scale down
            while (bufferSizeInChars - 500 > _text.Length)
            {
                bufferSizeInChars -= 500;
                needsResizing = true;
            }

            if (needsResizing)
            {
                VAO.Use(vao);

                VBO.Use(vbo);
                VBO.Resize(bufferSizeInChars * BytesPerChar);
                VertexAttrib2f2f.UseAttrib();

                VAO.Use(-1);
                VBO.Use(-1);
            }
        }

        void UpdateBuffer()
        {
            VBO.Use(vbo);
            VBO.SetData(GetMeshInEM());
            VBO.Use(OpenGL.NULL);
        }

        float[] GetMeshInEM()
        {
            charCount = 0;
            if (atlas == null) return new float[0];

            List<float> buffer = new List<float>();
            string[] lines = SegmentIntoLines(_text);
            Vector2 cursor = new Vector2(0);

            switch ((int)_textAlignment & 0b11)
            {
                case 0b00: cursor.y = height / (2 * fontSize) - 1; break;                   // Top
                case 0b01: cursor.y = (lines.Length - 2f) / 2; break;                        // Center
                case 0b10: cursor.y = -height / (2 * fontSize) + (lines.Length - 1); break; // Down
            }

            foreach (string line in lines)
            {
                float lineWidth = GetLineWithInEM(line);

                switch ((int)_textAlignment >> 2 & 0b11)
                {
                    case 0b00: cursor.x = -width / (2 * fontSize); break;           // Left
                    case 0b01: cursor.x = -lineWidth / 2; break;                    // Center
                    case 0b10: cursor.x = width / (2 * fontSize) -lineWidth; break; // Right
                }

                foreach (char character in line)
                {
                    char usable = character;

                    if (!atlas.subAtlasses.ContainsKey(character))
                    {
                        usable = atlas.MissingChar;
                    }

                    SubAtlas sub = atlas.subAtlasses[usable];
                    GlyphMetrics mat = sub.glyphMetrics;

                    if (sub.hasOutline)
                    {
                        charCount++;

                        // Load texture data
                        float Left = (float)(sub.x - atlas.Padding) / textureSize.width;
                        float Right = (float)(sub.x + sub.width + atlas.Padding) / textureSize.width;
                        float Top = (float)(sub.y - atlas.Padding) / textureSize.height;
                        float Bottom = (float)(sub.y + sub.height + atlas.Padding) / textureSize.height;

                        // Load character shape data
                        float em = mat.UnitsPerEm;
                        float fs = atlas.FontSize;
                        float glyphWidth = mat.Width / em + atlas.Padding * 2 / fs;
                        float glyphHeight = mat.Height / em + atlas.Padding * 2 / fs;
                        float xOffset = mat.LeftSideBearing / em - atlas.Padding / fs;
                        float yOffset = mat.MinY / em + atlas.Padding / fs;

                        // Add vertex data to the buffer
                        add(glyphWidth, glyphHeight, Right, Top); // Topright  vertex
                        add(0, glyphHeight, Left, Top);           // Topleft   vertex
                        add(0, 0, Left, Bottom);                  // Downleft  vertex
                        add(0, 0, Left, Bottom);                  // Downleft  vertex
                        add(glyphWidth, 0, Right, Bottom);        // Downright vertex
                        add(glyphWidth, glyphHeight, Right, Top); // Topright  vertex

                        void add(float x, float y, float tx, float ty)
                        {
                            buffer.Add(x + cursor.x + xOffset);
                            buffer.Add(y + cursor.y + yOffset);
                            buffer.Add(tx);
                            buffer.Add(ty);
                        }
                    }

                    // Add horizontal advance to cursor
                    cursor.x += (float)mat.AdvanceWidth / mat.UnitsPerEm;
                }

                cursor.y -= 1;
            }

            return buffer.ToArray();
        }

        float GetLineWithInEM(string line)
        {
            float lineWidth = 0;

            foreach (char character in line)
            {
                GlyphMetrics mat = atlas.subAtlasses[
                    atlas.subAtlasses.ContainsKey(character) ? character : atlas.MissingChar].glyphMetrics;
                
                lineWidth += (float)mat.AdvanceWidth / mat.UnitsPerEm;
            }

            return lineWidth;
        }

        string[] SegmentIntoLines(string text)
        {
            List<string> newLines = new List<string>();
            string currentLine = "";
            float currentWidth = 0;
            float spaceWidth = GetLineWithInEM(" ");

            foreach (var line in text.Split('\n'))
            {
                foreach (var word in line.Split(' '))
                {
                    currentWidth += GetLineWithInEM(word) + spaceWidth;
                    currentLine += word + " ";

                    if(currentWidth - spaceWidth > width)
                    {
                        // New line
                        newLines.Add(currentLine.Substring(0, currentLine.Length-1));
                        currentWidth = 0;
                        currentLine = "";
                    }
                }

                if(!string.IsNullOrEmpty(currentLine)) newLines.Add(currentLine.Substring(0, currentLine.Length - 1));
                currentWidth = 0;
                currentLine = "";
            }

            return newLines.ToArray();
        }
    }
}