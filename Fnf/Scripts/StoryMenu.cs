using Fnf.Framework;
using Fnf.Framework.Graphics;
using Fnf.Framework.TrueType;
using Fnf.Framework.TrueType.Rasterization;
using System.Collections.Generic;

namespace Fnf
{
    public class StoryMenu : Script
    {
        Text weekScore;
        Text weekName;

        Dictionary<string, int> bgs = new();

        int currentBg;

        void Start()
        {
            Font font = new("Assets/Fonts/vcr");
            FontAtlas atlas = new(font, 100, 3, 2, 0, FontAtlas.UpperCase + FontAtlas.LowerCase + 
                FontAtlas.Numbers + FontAtlas.Ponctuals + FontAtlas.Space);

            bgs.Add("stage", Texture.GenerateFromPath("Assets/MenuBackgrounds/menu_stage.png", out _));
            currentBg = bgs["stage"];

            weekScore = new Text(atlas);
            weekScore.text = "WEEK SCORE:0";
            weekScore.textAlignment = TextAlignment.Left;
            weekScore.height = 60;
            weekScore.width = 1000;
            weekScore.fontSize = 37;

            weekName = new(atlas);
            weekName.text = "CUTIE";
            weekName.textAlignment = TextAlignment.Right;
            weekName.height = 60;
            weekName.width = 1000;
            weekName.fontSize = 37;
            weekName.color = new Color(178, 178, 178);

            Anchor.PositionUILocaly(weekScore, AnchorType.TopLeft, new Vector2(15, 0));
            Anchor.PositionUILocaly(weekName, AnchorType.TopRight, new Vector2(15, 0));

        }

        void Update ()
        {
            //text.localPosition = Input.GetGridMousePosition();
        }

        void Render()
        {
            weekScore.Render();
            weekName.Render();

            float top = Window.PixelToViewportVertical(Window.GridSize.height / 2f - 60);
            float bottom = Window.PixelToViewportVertical(Window.GridSize.height / 2f - 60 - 412);

            Texture.Use(currentBg);
            OpenGL.BeginDrawing(DrawMode.Quads);
            OpenGL.Color3(1, 1, 1);
            OpenGL.TextureCoord(1, 0);
            OpenGL.Vertex2( 1, top);
            OpenGL.TextureCoord(0, 0);
            OpenGL.Vertex2(-1, top);
            OpenGL.TextureCoord(0, 1);
            OpenGL.Vertex2(-1, bottom);
            OpenGL.TextureCoord(1, 1);
            OpenGL.Vertex2( 1, bottom);
            OpenGL.EndDrawing();
            Texture.Use(OpenGL.NULL);
        }
    }
}