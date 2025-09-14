using Fnf.Framework.TrueType.Rasterization;
using Fnf.Framework.TrueType;
using Fnf.Framework.Graphics;
using Fnf.Framework;
using Fnf.Game;

using System.Collections.Generic;
using System.Linq;
using System.IO;
using System;

namespace Fnf
{
	public class StoryMenu : Script
	{
		// Stage stuff
		Animator gf, bf, opponent;

		// Text stuff
		Text weekScore, weekName, tracks;

		// Song selection stuff
		WeekOption[] options;
		float scrollLerp;
        int ttex; Size tsize;
		int selectedWeek;

		// Difficulty stuff
		Dictionary<string, (int id, Size size)> difficulties;
		Animator leftArrow, rightArrow;
		float leftArrowCooldown, rightArrowCooldown, fallingValue;
		int selectedDifficulty;

        void Start()
		{
			SetupTexts();
			SetupDifficulty();
			SetupWeeks();
            ttex = Texture.GenerateFromPath("Assets/Shared/Menu_Tracks.png", out tsize);

			TextureAtlas.LoadAtlas("menugf", "Assets/Story Menu/Characters/Menu_GF");
            TextureAtlas.LoadAtlas("menubf", "Assets/Story Menu/Characters/Menu_BF");

            Animation gfidle = TextureAtlas.GetAnimation("menugf", "M GF Idle");
			gfidle.looped = true;

            Animation bfidle = TextureAtlas.GetAnimation("menubf", "M BF Idle");
            bfidle.looped = true;

            gf = new();
			gf.localPosition = new Vector2(0, 110);
			gf.localScale = new Vector2(1.2f);
			gf.add("idle", gfidle);
			gf.play("idle");

            bf = new();
            bf.localPosition = new Vector2(440, 80);
            bf.localScale = new Vector2(1.0f);
            bf.add("idle", bfidle);
            bf.play("idle");

        }

		void Update()
		{
			leftArrow.Update();
			rightArrow.Update();
			gf.Update();
			bf.Update();
			SharedGameSystems.VolumeControl.Update();

			if (Input.GetKeyDown(Key.Escape)) Active = new MainMenu();

			// Week selection
			if (Input.GetAnyKeysDown(Key.W, Key.Up)) selectedWeek = MathUtility.WrapClamp(selectedWeek - 1, options.Length - 1, 0);
			if (Input.GetAnyKeysDown(Key.S, Key.Down)) selectedWeek = MathUtility.WrapClamp(selectedWeek + 1, options.Length - 1, 0);
			if (Input.GetKeyDown(Key.Enter))
			{
				Active = new PlayMode("Fuck you!", new string[] { "Epiphany" });
			}

            // Difficulty selection
            if (Input.GetAnyKeysDown(Key.D, Key.Right))
            {
                selectedDifficulty = MathUtility.WrapClamp(selectedDifficulty + 1, options[selectedWeek].difficulties.Length - 1, 0);
                rightArrow.play("pressed");
				rightArrowCooldown = 0.1f;
                fallingValue = .1f;
            }
            if (Input.GetAnyKeysDown(Key.A, Key.Left))
            {
                selectedDifficulty = MathUtility.WrapClamp(selectedDifficulty - 1, options[selectedWeek].difficulties.Length - 1, 0);
                leftArrow.play("pressed");
                leftArrowCooldown = 0.1f;
				fallingValue = .1f;
            }

			if(rightArrowCooldown > 0)
			{
                rightArrowCooldown -= Time.deltaTime;
				if (rightArrowCooldown == 0) rightArrowCooldown = -1f;
			}
            if (leftArrowCooldown > 0)
            {
                leftArrowCooldown -= Time.deltaTime;
                if (leftArrowCooldown == 0) leftArrowCooldown = -1f;
            }

			if (rightArrowCooldown < 0)
			{
                rightArrowCooldown = 0;
				rightArrow.play("idle");
			}
            if (leftArrowCooldown < 0)
            {
                leftArrowCooldown = 0;
                leftArrow.play("idle");
            }

            scrollLerp = MathUtility.Lerp(scrollLerp, selectedWeek * 148, Time.deltaTime * 10);
			fallingValue = MathUtility.Clamp(fallingValue - Time.deltaTime, 1, 0);

			tracks.text = options[selectedWeek].tracks;
			weekName.text = options[selectedWeek].weekName;
		}

		void Render()
		{
			RenderWeeksSprites();
			RenderDifficulty();
			RenderWeekTracks();
			

			OpenGL.Color3(0, 0, 0);
			OpenGL.BeginDrawing(DrawMode.Quads);
			OpenGL.Vertex2(1, 1);
			OpenGL.Vertex2(-1, 1);
			OpenGL.Vertex2(-1, 0);
			OpenGL.Vertex2(1, 0);
			OpenGL.EndDrawing();

			weekScore.Render();
			weekName.Render();

			float top = Window.PixelToViewportVertical(Window.GridSize.height / 2f - 60);
			float bottom = Window.PixelToViewportVertical(Window.GridSize.height / 2f - 60 - 412);

			OpenGL.Color3(1, 1, 1);
			Texture.Use(options[selectedWeek].menuBackground);
			OpenGL.BeginDrawing(DrawMode.Quads);
			OpenGL.TextureCoord(1, 0);
			OpenGL.Vertex2(1, top);
			OpenGL.TextureCoord(0, 0);
			OpenGL.Vertex2(-1, top);
			OpenGL.TextureCoord(0, 1);
			OpenGL.Vertex2(-1, bottom);
			OpenGL.TextureCoord(1, 1);
			OpenGL.Vertex2(1, bottom);
			OpenGL.EndDrawing();
			Texture.Use(OpenGL.NULL);

			gf.Render();
			bf.Render();

            SharedGameSystems.VolumeControl.Render();
        }

		void RenderWeekTracks()
		{

			float w = tsize.width / 2;
			float h = tsize.height / 2;

			float yoff = -140;
			float xoff = -470;

			OpenGL.Color3(1, 1, 1);
			Texture.Use(ttex);
			OpenGL.BeginDrawing(DrawMode.Quads);
			OpenGL.TextureCoord(1, 0);
			OpenGL.Pixel2(w + xoff, h + yoff);
			OpenGL.TextureCoord(0, 0);
			OpenGL.Pixel2(-w + xoff, h + yoff);
			OpenGL.TextureCoord(0, 1);
			OpenGL.Pixel2(-w + xoff, -h + yoff);
			OpenGL.TextureCoord(1, 1);
			OpenGL.Pixel2(w + xoff, -h + yoff);
			OpenGL.EndDrawing();
			Texture.Use(OpenGL.NULL);

			tracks.Render();
		}

		void RenderDifficulty()
		{
			float yoffset = -165 + fallingValue * 100;

			float xoffset = 470;

			Size size = difficulties[options[selectedWeek].difficulties[selectedDifficulty]].size;

			float scale = 1f;
			float w = size.width / 2 * scale;
			float h = size.height / 2 * scale;

			OpenGL.Color4(Color.White, 1 - fallingValue * 6);

			Texture.Use(difficulties[options[selectedWeek].difficulties[selectedDifficulty]].id);
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
			float yoffset = -165 + scrollLerp;

			for (int i = 0; i < options.Length; i++)
			{
				Size size = options[i].weekImageSize;

				float scale = 1.05f;
				float w = size.width / 2 * scale;
				float h = size.height / 2 * scale;

				OpenGL.Color4(Color.White, i == selectedWeek ? 1 : 0.8f);

				Texture.Use(options[i].weekImage);
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

		void SetupTexts()
		{
            // TODO: Fuck this font system. Make a new one bitch.
            Font font = new("Assets/Fonts/vcr");
            FontAtlas atlas = new(font, 100, 3, 2, 0,
                FontAtlas.UpperCase + FontAtlas.LowerCase +
                FontAtlas.Numbers + FontAtlas.Ponctuals + FontAtlas.Space);

            weekScore = new(atlas)
            {
                text = "WEEK SCORE:0",
                textAlignment = TextAlignment.Left,
                height = 60,
                width = 1000,
                fontSize = 37
            };

			weekName = new(atlas)
			{
				text = "",
				textAlignment = TextAlignment.Right,
				height = 60,
				width = 1000,
				fontSize = 37,
				color = new Color(178, 178, 178)
			};

            tracks = new(atlas)
            {
                text = "",
                textAlignment = TextAlignment.Top,
                height = 400,
                width = 1000,
                fontSize = 32,
                color = new Color(229, 87, 119),
                localPosition = new Vector2(-470, -375)
            };

            Anchor.PositionUILocaly(weekName, AnchorType.TopRight, new Vector2(15, 0));
            Anchor.PositionUILocaly(weekScore, AnchorType.TopLeft, new Vector2(15, 0));
        }

		void SetupDifficulty()
		{
			difficulties = new();
			List<string> diffsImagePaths = Directory.GetFileSystemEntries("Assets/Story Menu/Difficulties").ToList();
			for (int i = 0; i < diffsImagePaths.Count; i++)
			{
				string diffName = new FileInfo(diffsImagePaths[i]).Name;
				diffName = diffName.Substring(0, diffName.Length - 4);
				difficulties.Add(diffName, (Texture.GenerateFromPath(diffsImagePaths[i], out Size sizeout), sizeout));
			}

            float maxDifficultyWidth = 0;
			foreach (var difficulty in difficulties)
			{
				float width = difficulty.Value.size.width / 2 + 29;
                maxDifficultyWidth = Math.Max(maxDifficultyWidth, width);
            }

            leftArrow = new();
            rightArrow = new();

            leftArrow.localPosition = new(470 - maxDifficultyWidth, -165);
            rightArrow.localPosition = new(470 + maxDifficultyWidth, -165);

            TextureAtlas.LoadAtlas("cmua", "Assets/campaign_menu_UI_assets");

            leftArrow.add("idle", TextureAtlas.GetAnimation("cmua", "arrow left"));
            leftArrow.add("pressed", TextureAtlas.GetAnimation("cmua", "arrow push left"));
            rightArrow.add("idle", TextureAtlas.GetAnimation("cmua", "arrow right"));
            rightArrow.add("pressed", TextureAtlas.GetAnimation("cmua", "arrow push right"));

            leftArrow.play("idle");
            rightArrow.play("idle");
        }

		void SetupWeeks()
		{
            options = new WeekOption[7];

			options[0] = new WeekOption("Stage", "tutorial")
			{
				tracks = "TUTORIAL",
				weekName = "TEACHING TIME",
				difficulties = new string[3] { "easy", "normal", "hard" }
			};

            options[1] = new WeekOption("Stage", "week1")
            {
                tracks = "Bopeebo\nFresh\nDadBattle",
                weekName = "DADDY DEAREST",
                difficulties = new string[3] { "easy", "normal", "hard" }
            };

            options[2] = new WeekOption("Halloween", "week2")
            {
                tracks = "Spookeez\nSouth\nMonster",
                weekName = "SPOOKY MONTH",
                difficulties = new string[3] { "easy", "normal", "hard" }
            };

            options[3] = new WeekOption("Philly", "week3") 
			{
                tracks = "Pico\nPhilly Nice\nBlammed",
                weekName = "PICO",
                difficulties = new string[3] { "easy", "normal", "hard" } 
			};

            options[4] = new WeekOption("Limo", "week4") 
			{
                tracks = "Satin Panties\nHigh\nM.I.L.F",
                weekName = "MOMMY MUST MURDER",
                difficulties = new string[3] { "easy", "normal", "hard" } 
			};

            options[5] = new WeekOption("Christmas", "week5") 
			{
                tracks = "Cocoa\nEggnog\nWinter HorrorLand",
                weekName = "RED SNOW",
                difficulties = new string[3] { "easy", "normal", "hard" } 
			};

            options[6] = new WeekOption("School", "week6") 
			{
                tracks = "Senpai\nRoses\nThrons",
                weekName = "HATING SIMULATOR FT. MAOWLING",
                difficulties = new string[3] { "easy", "normal", "hard" } 
			};
        }
	}

	class WeekOption
	{
		public string[] difficulties;
		public string tracks;
		public string weekName;

		public int menuBackground;
		public int weekImage;
		public Size weekImageSize;
		 
		public WeekOption(string backgroundImageName, string weekImageName)
		{
			menuBackground = Texture.GenerateFromPath($"Assets/Story Menu/Backgrounds/{backgroundImageName}.png", out _);
			weekImage = Texture.GenerateFromPath($"Assets/Story Menu/Weeks/{weekImageName}.png", out weekImageSize);

			Texture.SetWrap(weekImage, WrapMode.Clamp, WrapMode.Clamp); // Not working
		}
	}
}