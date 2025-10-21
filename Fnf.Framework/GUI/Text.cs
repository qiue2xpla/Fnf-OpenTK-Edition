using Fnf.Framework.TrueType.Rasterization;
using Fnf.Framework.TrueType;
using Fnf.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Fnf.Framework
{
    /// <summary>
    /// Used to display text on the screen with ease
    /// </summary>
    public class Text : GUI, IRenderable
    {
        // These variables require the mesh buffer to be remade

        /// <summary>
        /// The text that is displayed
        /// </summary>
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

        /// <summary>
        /// Make the GUI width fit the text
        /// </summary>
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

        /// <summary>
        /// The atlas that the text uses
        /// </summary>
        public FontAtlas atlas
        {
            get => _atlas;
            set
            {
                //if (value == _atlas) return;
                //_atlas = value;
                //mustReNewMesh = true;
                //mustReNewBuffer = true;
                //Texture.Destroy(texture);
                //texture = Texture.GenerateFromBitmap(value.bitmap, "FontAtlas");
                //atlasSize = new Vector2(value.bitmap.Width, value.bitmap.Height);
                //shader = (value?.IsSignedDistanceFeild??false) ? sdfShader : defaultShader; 
            }
        }

        /// <summary>
        /// Displayed text font size
        /// </summary>
        public float fontSize
        {
            get => _fontSize;
            set
            {
                if (value == _fontSize) return;
                _fontSize = value;
                mustReNewMesh = true;
            }
        }

        /// <summary>
        /// How the text is aligned in the GUI
        /// </summary>
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

        // These variables doesn't require the mesh buffer to be remade
        // Mostly handled by the Text or the shader
         
        /// <summary>
        /// The display color of the text
        /// </summary>
        public Color color = Color.White; // TODO: Add fore and back colors

        /// <summary>
        /// Gets or sets the renderable state of the text
        /// </summary>
        public bool isRenderable { get; set; } = true;

        // Private variables

        TextAlignment _textAlignment;
        FontAtlas _atlas;
        bool _fitContent;
        string _text;
        float _fontSize;

        float previousWidth, previousHeight;
        bool mustReNewBuffer = false;
        bool mustReNewMesh = false;
        int displayedCharCount = 0; // TODO: Non character triangles may be added later, so this isn't reliable
        int bufferSizeInChars = 50;
        int vbo, vao, shader, texture;
        Vector2 atlasSize;

        static int defaultShader, sdfShader; // More than one shader copy is not needed

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

        public Text(FontAtlas atlas = null)
        {
            _textAlignment = TextAlignment.Center;
            _text = "";
            _fontSize = 18;
            
            if (defaultShader == 0)
            {
                defaultShader = Shader.GenerateShaderFromResource("defaultFont");
                Shader.Bind(defaultShader);
                Shader.Uniform1(defaultShader, "tex", 0);
                Shader.Bind(0);
            }

            if (sdfShader == 0)
            {
                sdfShader = Shader.GenerateShaderFromResource("sdfFont");
                Shader.Bind(sdfShader);
                Shader.Uniform1(sdfShader, "tex", 0);
                Shader.Bind(0);
            }

            this.atlas = atlas;
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
            if (isRaycastable && IsOverGUI()) RaycastHit();
            if (atlas == null) return;

            if (previousWidth != width || previousHeight != height)
            {
                previousWidth = width;
                previousHeight = height;
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

                    if (!atlas.subAtlasses.ContainsKey(character)) usable = (char)0xFFFF;

                    SubAtlas sub = atlas.subAtlasses[usable];
                    //GlyphMetrics mat = sub.glyphMetrics;

                    if (sub.hasOutline)
                    {
                        displayedCharCount++;/*

                        // Load texture data
                        float Left = (float)(sub.x - atlas.Padding) / atlasSize.x;
                        float Right = (float)(sub.x + sub.width + atlas.Padding) / atlasSize.x;
                        float Top = (float)(sub.y - atlas.Padding) / atlasSize.y;
                        float Bottom = (float)(sub.y + sub.height + atlas.Padding) / atlasSize.y;

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
                        }*/
                    }

                    // Add horizontal advance to cursor
                    //cursor.x += (float)mat.AdvanceWidth / mat.UnitsPerEm;
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
                //GlyphMetrics mat = atlas.subAtlasses[
                    //atlas.subAtlasses.ContainsKey(character) ? character : atlas.MissingChar].glyphMetrics;
                
                //lineWidth += (float)mat.AdvanceWidth / mat.UnitsPerEm;
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