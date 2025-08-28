using Fnf.Framework.TrueType.Rasterization;
using Fnf.Framework.TrueType;
using Fnf.Framework.Graphics;
using Fnf.Framework;
using Fnf.Game;

using System.Collections.Generic;
using System;

namespace Fnf
{
    public class StoryMenu : Script
    {
        Text weekScore;
        Text weekName;

        Dictionary<string, int> bgs = new();
        List<(int, Size)> storyw = new();

        List<(int, Size)> difs = new();

        int currentBg;

        int currentWeek;

        Animator leftArrow, rightArrow;

        void Start()
        {
            Font font = new("Assets/Fonts/vcr");
            FontAtlas atlas = new(font, 100, 3, 2, 0, FontAtlas.UpperCase + FontAtlas.LowerCase + 
                FontAtlas.Numbers + FontAtlas.Ponctuals + FontAtlas.Space);

            bgs.Add("stage", Texture.GenerateFromPath("Assets/MenuBackgrounds/menu_stage.png", out _));
            currentBg = bgs["stage"];

            storyw.Add((Texture.GenerateFromPath("Assets/StoryWeeks/tutorial.png", out Size size), size));

            for (int i = 0; i < 6; i++)
            {
                storyw.Add((Texture.GenerateFromPath($"Assets/StoryWeeks/week{i+1}.png", out Size sizee), sizee));
            }

            difs.Add((Texture.GenerateFromPath("Assets/Difficulties/normal.png", out Size size3), size3));

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


            leftArrow = new();
            rightArrow = new();

            TextureAtlas.LoadAtlas("cmua", "Assets/campaign_menu_UI_assets");

            leftArrow.add("idle", TextureAtlas.GetAnimation("cmua", "arrow left"));
            leftArrow.add("pressed", TextureAtlas.GetAnimation("cmua", "arrow push left"));
            rightArrow.add("idle", TextureAtlas.GetAnimation("cmua", "arrow right"));
            rightArrow.add("pressed", TextureAtlas.GetAnimation("cmua", "arrow push right"));

            leftArrow.play("idle");
            rightArrow.play("idle");

            float maxw = 0;

            for (int i = 0; i < difs.Count; i++)
            {
                maxw = Math.Max(difs[i].Item2.width, maxw);
            }

            //maxw *= 1.2f;
            maxw /= 2;
            maxw += 29;

            leftArrow.localPosition = new(470 - maxw, -165);
            rightArrow.localPosition = new(470 + maxw, -165);
        }

        void Update ()
        {
            //text.localPosition = Input.GetGridMousePosition();

            leftArrow.Update();
            rightArrow.Update();
        }

        void Render()
        {
            weekScore.Render();
            weekName.Render();

            float top = Window.PixelToViewportVertical(Window.GridSize.height / 2f - 60);
            float bottom = Window.PixelToViewportVertical(Window.GridSize.height / 2f - 60 - 412);

            OpenGL.Color3(1, 1, 1);
            Texture.Use(currentBg);
            OpenGL.BeginDrawing(DrawMode.Quads);
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

            RenderWeeksSprites();
            RenderDifficulty();
        }

        void RenderDifficulty()
        {
            float yoffset = -165;

            float xoffset = 470;

            Size size = difs[0].Item2;

            float scale = 1f;
            float w = size.width / 2 * scale;
            float h = size.height / 2 * scale;

            OpenGL.Color4(Color.White);

            Texture.Use(difs[0].Item1);
            OpenGL.BeginDrawing(DrawMode.Quads);
            OpenGL.TextureCoord(1, 0);
            OpenGL.Pixel2(w + xoffset, h + yoffset);
            OpenGL.TextureCoord(0, 0);
            OpenGL.Pixel2(-w + xoffset, h + yoffset);
            OpenGL.TextureCoord(0, 1);
            OpenGL.Pixel2(-w + xoffset, -h + yoffset);
            OpenGL.TextureCoord(1, 1);
            OpenGL.Pixel2(w + xoffset, -h + yoffset);
            OpenGL.EndDrawing();
            Texture.Use(OpenGL.NULL);

            leftArrow.Render();
            rightArrow.Render();
        }

        void RenderWeeksSprites()
        {
            float yoffset = -165;



            RenderWeekSprite(0, 1);
            RenderWeekSprite(1, 1);
            RenderWeekSprite(2, 1);
            RenderWeekSprite(3, 1);

            void RenderWeekSprite(int week, float alpha)
            {
                Size size = storyw[week].Item2;

                float scale = 1.05f;
                float w = size.width / 2 * scale;
                float h = size.height / 2 * scale;

                OpenGL.Color4(Color.White, alpha);

                Texture.Use(storyw[week].Item1);
                OpenGL.BeginDrawing(DrawMode.Quads);
                OpenGL.TextureCoord(1, 0);
                OpenGL.Pixel2(w, h + yoffset);
                OpenGL.TextureCoord(0, 0);
                OpenGL.Pixel2(-w, h + yoffset);
                OpenGL.TextureCoord(0, 1);
                OpenGL.Pixel2(-w, -h + yoffset);
                OpenGL.TextureCoord(1, 1);
                OpenGL.Pixel2(w, -h + yoffset);
                OpenGL.EndDrawing();
                Texture.Use(OpenGL.NULL);

                yoffset -= 148;
            }
        }
    }
}