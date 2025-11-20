using Fnf.Framework.TrueType.Rendering;
using Fnf.Framework.TrueType;
using Fnf.Framework.Graphics;

using System.Collections.Generic;
using System.Linq;
using System;
using OpenTK.Graphics.ES11;

namespace Fnf.Framework
{
    /// <summary>
    /// Built-in class for handling text rendering
    /// </summary>
    public class Text : GUI, IRenderable
    {
        #region Instant effect

        /// <summary>
        /// Gets or sets the renderable state of the text
        /// </summary>
        public bool isRenderable { get; set; } = true;

        /// <summary>
        /// The display color of the text
        /// </summary>
        public Color color = Color.White; // TODO: Add fore and back colors

        #endregion

        #region Needs processing

        /// <summary>
        /// The text that is displayed
        /// </summary>
        public string text
        {
            get => _text;
            set
            {
                if (value == null)
                {
                    _text = "";
                }
                else
                {
                    if (value == _text) return;
                    _text = value;
                    mustReSize = true;
                    mustReMesh = true;

                    if (fitContent)
                    {
                        float[] widths = GetLinesWidths();
                        width = widths.Max();
                        height = widths.Length * fontSize; // + line spacing * (widths.Length - 1)
                    }
                }
            }
        }

        /// <summary>
        /// The font atlas used for rendering
        /// </summary>
        public FontAtlas fontAtlas
        {
            get => _fontAtlas;
            set
            {
                if (value != _fontAtlas) ReplaceAtlas(value);
            }
        }

        /// <summary>
        /// Indicates how the text is aligned in the GUI
        /// </summary>
        public TextAlignment textAlignment
        {
            get => _textAlignment;
            set
            {
                if (value != _textAlignment)
                {
                    _textAlignment = value;
                    mustReMesh = true;
                }
            }
        }

        /// <summary>
        /// Displayed text size
        /// </summary>
        public float fontSize
        {
            get => _fontSize;
            set
            {
                if (value == _fontSize) return;
                _fontSize = value;
                mustReMesh = true;
            }
        }

        /// <summary>
        /// Makes the GUI size fit the text automaticaly
        /// </summary>
        public bool fitContent
        {
            get => _fitContent;
            set
            {
                if (value != _fitContent)
                {
                    _fitContent = value;
                    mustReMesh = true;
                }
            }
        }

        #endregion

        FontAtlas _fontAtlas;
        TextAlignment _textAlignment = TextAlignment.Center;
        bool _fitContent;
        string _text = "";
        float _fontSize = 18;

        bool mustReSize = true, mustReMesh = true;
        float previousWidth, previousHeight;
        static int defaultShader, sdfShader;
        int vbo, vao, shader, texture;
        Vector2 atlasSize;

        public int displayedCharCount = 0;
        public int maximumDisplayedCharCount { get; private set; } = 0; 
        int bufferSizeInChars = 50;

        // Constant variables

        /// <summary>
        /// The amount of attributes in each vertex. And they are X, Y, TexCoordX and TexCoordY
        /// </summary>
        const int AttributesPerVertex = 4; // TODO: Add custom/Gradiant coloring

        /// <summary>
        /// Amount of vertices per a displayed character
        /// </summary>
        const int VerticesPerChar = 6; // 2 Triangle

        /// <summary>
        /// Amount of bytes per displayed character
        /// </summary>
        const int BytesPerChar = AttributesPerVertex * VerticesPerChar * sizeof(float); // 4 ( xyuv )

        public Text(FontAtlas fontAtlas = null)
        {
            if (defaultShader == 0) defaultShader = Shader.GenerateShaderFromResource("defaultFont");
            if (sdfShader == 0) sdfShader = Shader.GenerateShaderFromResource("sdfFont");

            this.fontAtlas = fontAtlas;

            vbo = VBO.GenerateVBO();
            vao = VAO.Generate();

            VAO.Bind(vao);
            VBO.Use(vbo);
            VBO.Resize(bufferSizeInChars * BytesPerChar);
            VAO.VertexAttribPointer(0, 2, VAO.VertexAttribPointerType.Float, 4 * sizeof(float), 0);
            VAO.VertexAttribPointer(1, 2, VAO.VertexAttribPointerType.Float, 4 * sizeof(float), 2 * sizeof(float));
            VAO.EnableVertexAttribArray(0);
            VAO.EnableVertexAttribArray(1);
            VAO.Bind(0);
            VBO.Use(0);
        }

        public void Render()
        {
            if (!isRenderable) return;
            if (isRaycastable && IsMouseOverGUI()) RaycastHit();
            if (fontAtlas == null) return;

            if (previousWidth != width || previousHeight != height)
            {
                previousWidth = width;
                previousHeight = height;
                mustReMesh = true;
            }

            if (mustReSize)
            {
                mustReSize = false;
                ResizeBuffer();
            }

            if (mustReMesh)
            {
                mustReMesh = false;
                UpdateBuffer();
            }

            Shader.Bind(shader);
            Shader.Color3(shader, "textColor", color);
            Shader.Uniform1(shader, "fontSize", fontSize);

            Shader.UniformMat(shader, "transform",
                Matrix3.Scale(Window.PixelToViewport(1, 1)) *
                WorldlTransformMatrix() *
                Matrix3.Scale(new Vector2(fontSize)));


            VAO.Bind(vao);
            Texture.Use(texture);

            OpenGL.DrawArrays(DrawMode.Triangles, displayedCharCount * 6);

            Texture.Use(0);
            VAO.Bind(0);

            Shader.Bind(0);
        }

        void ReplaceAtlas(FontAtlas atlas)
        {
            _fontAtlas = atlas;
            mustReSize = true;
            mustReMesh = true;
            atlasSize = new Vector2(atlas.map.width, atlas.map.height);
            shader = (atlas?.isSDF ?? false) ? sdfShader : defaultShader;
            Texture.Destroy(texture);
            texture = Texture.GenerateOneComponentTexture(atlas.map);
        }

        float GetLineWithInEM(string line)
        {
            float lineWidth = 0;

            foreach (char character in line)
            {
                GlyphMetrics mat = fontAtlas.subAtlasses[fontAtlas.subAtlasses.ContainsKey(character) ? character : (char)0xFFFF].metrics;
                lineWidth += (float)mat.AdvanceWidth / mat.UnitsPerEm;
            }

            return lineWidth;
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
                VAO.Bind(vao);

                VBO.Use(vbo);
                VBO.Resize(bufferSizeInChars * BytesPerChar);

                VAO.VertexAttribPointer(0, 2, VAO.VertexAttribPointerType.Float, 4 * sizeof(float), 0);
                VAO.VertexAttribPointer(1, 2, VAO.VertexAttribPointerType.Float, 4 * sizeof(float), 2 * sizeof(float));
                VAO.EnableVertexAttribArray(0);
                VAO.EnableVertexAttribArray(1);

                VAO.Bind(0);
                VBO.Use(0);
            }
        }

        void UpdateBuffer()
        {
            VBO.Use(vbo);
            VBO.SetData(GetMeshInEM());
            VBO.Use(0);
        }

        float[] GetMeshInEM()
        {
            displayedCharCount = 0;
            if (fontAtlas == null) return new float[0];

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
                    case 0b10: cursor.x = width / (2 * fontSize) - lineWidth; break; // Right
                }

                foreach (char character in line)
                {
                    char usable = character;

                    if (!fontAtlas.subAtlasses.ContainsKey(character)) usable = (char)0xFFFF;

                    SubAtlas sub = fontAtlas.subAtlasses[usable];
                    GlyphMetrics mat = sub.metrics;

                    if (sub.hasOutline)
                    {
                        displayedCharCount++;

                        // Load texture data
                        float Left = (float)(sub.x - fontAtlas.padding) / atlasSize.x;
                        float Right = (float)(sub.x + sub.width + fontAtlas.padding) / atlasSize.x;
                        float Top = (float)(sub.y - fontAtlas.padding) / atlasSize.y;
                        float Bottom = (float)(sub.y + sub.height + fontAtlas.padding) / atlasSize.y;

                        // Load character shape data
                        float em = mat.UnitsPerEm;
                        float fs = fontAtlas.fontSize;
                        float glyphWidth = mat.bounds.size.width/ em + fontAtlas.padding * 2 / fs;
                        float glyphHeight = mat.bounds.size.height / em + fontAtlas.padding * 2 / fs;
                        float xOffset = mat.LeftSideBearing / em - fontAtlas.padding / fs;
                        float yOffset = mat.bounds.bottom / em + fontAtlas.padding / fs;

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