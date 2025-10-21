using Fnf.Framework.TrueType.Rasterization;
using Fnf.Framework.TrueType;
using Fnf.Framework.Graphics;
using Fnf.Framework;
using Fnf.Game;
using System;

namespace Fnf
{
    public class Freeplay : Script
    {
        string[] songs = new string[] { "friday night", "yo dead ass", "idk what to type here", "wow song", "idk man", "fuck schools", "vs wheat", "Opposition", "Hell verse"};
        string[] difficulties = new string[] { "EASY", "NORMAL", "HARD"};

        FnfText songText;
        Text scoreText;
        Text difficultyText;

        int selectedDifficulty; 
        float smoothedPositionOffset = 0;
        float distanceBetweenLines = 160;
        int selection = 0;
        int bg;

        int score, smoothedScore;

        Random random = new Random();
        CharacterIcon icon = new CharacterIcon($"{GamePaths.Icons}/bf");

        Panel panel;

        Font font;
        FontAtlas atlas;

        public Freeplay()
        {
            bg = Texture.GenerateFromPath($"{GamePaths.MainMenu}/menuDesat.png");

            songText = new FnfText();
            songText.localScale = new Vector2(1.2f);
            songText.textAlignment = TextAlignment.Left;
            songText.fitContent = true;


            panel = new Panel();
            panel.color = new Color(0, 0, 0, 195);
            panel.height = 85;

            font = new Font("Assets/Fonts/vcr");
            atlas = new FontAtlas(font, 120, 3, 3, 0, FontAtlas.GetCustomCharset("UN") + "()<>:.% ");

            difficultyText = new Text(atlas);
            difficultyText.fontSize = 30;
            difficultyText.fitContent = true;
            difficultyText.parent = panel;
            difficultyText.localPosition = new Vector2(0, -22);

            scoreText = new Text(atlas);
            scoreText.fontSize = 45;
            scoreText.fitContent = true;
            scoreText.parent = panel;
            scoreText.localPosition = new Vector2(0, 20);


        }

        void Update()
        {
            if(Input.GetKeyDown(Key.Down))
            {
                selection++;
                if (selection >= songs.Length) 
                    selection = songs.Length - 1;

                score = random.Next(100000);
            }
            
            if (Input.GetKeyDown(Key.Up))
            {
                selection--;
                if (selection < 0) 
                    selection = 0;

                score = random.Next(1000000000);
            }

            if (Input.GetKeyDown(Key.Right))
            {
                selectedDifficulty++;
                if (selectedDifficulty >= difficulties.Length)
                    selectedDifficulty = difficulties.Length - 1;
            }

            if (Input.GetKeyDown(Key.Left))
            {
                selectedDifficulty--;
                if (selectedDifficulty < 0)
                    selectedDifficulty = 0;
            }

            scoreText.text = "PERSONAL BEST: " + smoothedScore;
            panel.width = Math.Max(scoreText.width, difficultyText.width) + 10;

            panel.localPosition = Window.GridSize.ToVector2() - new Vector2(panel.width / 2, panel.height / 2);

            difficultyText.text = $"< {difficulties[selectedDifficulty]} >";

            smoothedPositionOffset = lerp(smoothedPositionOffset, distanceBetweenLines * selection, Time.deltaTime * 8);

            smoothedScore = (int)lerp(smoothedScore, score, Time.deltaTime * 12);

            if (Math.Abs(smoothedScore - score) < 500) smoothedScore = score;

            SharedGameSystems.VolumeControl.Update();
        }

        void Render()
        {
            Texture.Use(bg);

            OpenGL.BeginDrawing(DrawMode.Quads);

            OpenGL.Color3(1, 1, 1);
            OpenGL.TextureCoord(1, 0);
            OpenGL.Vertex2(1, 1);
            OpenGL.TextureCoord(0, 0);
            OpenGL.Vertex2(-1, 1);
            OpenGL.TextureCoord(0, 1);
            OpenGL.Vertex2(-1, -1);
            OpenGL.TextureCoord(1, 1);
            OpenGL.Vertex2(1, -1);

            OpenGL.EndDrawing();

            Texture.Use(0);

            for (int i = 0; i < songs.Length; i++)
            {
                float forward = 55; // Amount to move each half screen height

                float y = - (i * 160) + smoothedPositionOffset;
                float xoffset = -forward * (y - Window.GridSize.height / 2) * 2 / Window.GridSize.height;

                songText.text = songs[i];
                songText.localPosition = new Vector2(songText.width * songText.localScale.x / 2 + 92 + xoffset, y);

                float iconPos = songText.localPosition.x + songText.width * songText.globalScale.x / 2 + icon.size.x / 2 + 10;

                icon.localPosition = new Vector2(iconPos, y);
                
                songText.Render();
                icon.Render();
            }

            panel.Render();
            scoreText.Render();
            difficultyText.Render();

            SharedGameSystems.VolumeControl.Render();
        }

        float lerp(float a, float b, float t)
        {
            return a + (b - a) * t;
        }
    }
}