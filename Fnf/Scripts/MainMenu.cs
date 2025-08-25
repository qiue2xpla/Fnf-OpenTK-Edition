using Fnf.Framework.Graphics;
using Fnf.Framework.Audio;
using Fnf.Framework;
using Fnf.Game;

namespace Fnf
{
    public class MainMenu : Script
    {
        Animator[] menuItems;
        EffectsLayer effectsLayer;

        int bg = Texture.GenerateFromPath("Assets/Shared/menuBG.png", out _);

        string[] Options = { "story mode", "freeplay", "options" };
        static int CurrentSelected = 0;
        bool selectedSomethin = false;
        float CurrentBGPosition = 0;
        float TargetBGPosition = 0;
        float size = 1.1f;

        void Start()
        {
            effectsLayer = new EffectsLayer();

            if (Music.Instruments == null)
            {
                Music.LoadMenuMusic(0.7f);
            }

            menuItems = new Animator[Options.Length];

            TextureAtlas.LoadAtlas("menuitems", "Assets/Shared/MainMenuAssets");

            for (int i = 0; i < Options.Length; i++)
            {
                menuItems[i] = new Animator();
                menuItems[i].globalPosition = new Vector2(0, 160 + (i * -160));

                Animation idle = TextureAtlas.GetAnimation("menuitems", Options[i] + " basic");
                Animation selected = TextureAtlas.GetAnimation("menuitems", Options[i] + " white");
                idle.looped = true;
                selected.looped = true;


                menuItems[i].add("idle", idle);
                menuItems[i].add("selected", selected);
                menuItems[i].play("idle");
            }


            changeItem();

            effectsLayer.Add(new TransitionOutEffect(0, 0.5f, Color.Black));
        }

        void Update()
        {
            for (int i = 0; i < menuItems.Length; i++)
            {
                menuItems[i].Update();
            }

            CurrentBGPosition = Lerp(CurrentBGPosition, TargetBGPosition, Time.deltaTime * 4);

            GlobalSystems.VolumeControl.Update();

            effectsLayer.Update();

            if (selectedSomethin) return;

            if (Input.GetKeyDown(Key.Escape))
            {
                effectsLayer.Add(new TransitionInEffect(0, 0.5f, Color.Black));
                selectedSomethin = true;
                return;
            }

            if (Input.GetKeyDown(Key.Up))
            {
                new Clip("Assets/Shared/scrollMenu.ogg") { endAction = EndAction.Dispose }.play();
                changeItem(-1);
            }

            if (Input.GetKeyDown(Key.Down))
            {
                new Clip("Assets/Shared/scrollMenu.ogg") { endAction = EndAction.Dispose }.play();
                changeItem(1);
            }

            if (Input.GetAnyKeysDown(Key.Space, Key.Enter, Key.KeypadEnter))
            {
                selectedSomethin = true;

                new Clip("Assets/Shared/confirm.ogg") { endAction = EndAction.Dispose }.play();

                //flickerRenderEffect.Start(0, 1.1f, 5, bg);

                effectsLayer.Add(new SlideValueEffect(0, 0.4f, 255, 0, (a) =>
                {
                    for (int i = 0; i < menuItems.Length; i++)
                    {
                        if (CurrentSelected != i)
                        {
                            menuItems[i].color.a = (byte)a;
                        }
                    }
                }));

                effectsLayer.Add(new FlickerRenderEffect(0, 1, 16, menuItems[CurrentSelected]));

                for (int i = 0; i < menuItems.Length; i++)
                {
                    if (CurrentSelected == i)
                    {
                        string daChoice = Options[CurrentSelected];

                        switch (daChoice)
                        {
                            case "story mode":
                                effectsLayer.Add(new TransitionInEffect(0.7f, 0.5f, Color.Black, delegate { Active = new StoryMenu(); }));
                                break;

                            case "freeplay":
                                effectsLayer.Add(new TransitionInEffect(0.7f, 0.5f, Color.Black, delegate { Active = new Freeplay(); }));
                                break;

                            case "options":
                                effectsLayer.Add(new TransitionInEffect(0.7f, 0.5f, Color.Black, delegate { Active = new Test(); }));
                                break;
                        }
                    }
                }
            }

        }

        void Render()
        {
            Texture.Use(bg);
            OpenGL.BeginDrawing(DrawMode.Quads);
            OpenGL.Color3(1f, 1f, 1f);

            OpenGL.TextureCoord(1, 0);
            OpenGL.Vertex2( size,  size + CurrentBGPosition);

            OpenGL.TextureCoord(0, 0);
            OpenGL.Vertex2(-size,  size + CurrentBGPosition);

            OpenGL.TextureCoord(0, 1);
            OpenGL.Vertex2(-size, -size + CurrentBGPosition);

            OpenGL.TextureCoord(1, 1);
            OpenGL.Vertex2( size, -size + CurrentBGPosition);

            OpenGL.EndDrawing();

            for (int i = 0; i < menuItems.Length; i++)
            {
                menuItems[i].Render();
            }
            effectsLayer.Render();
            GlobalSystems.VolumeControl.Render();
        }

        void changeItem(int huh = 0)
        {
            CurrentSelected += huh;

            if (CurrentSelected >= menuItems.Length)
            {
                CurrentSelected = 0;
            }

            if (CurrentSelected < 0)
            {
                CurrentSelected = menuItems.Length - 1;
            }

            float con = (size - 1) / 2;
            TargetBGPosition = Lerp(-con, con, (float)CurrentSelected / (menuItems.Length - 1));
            
            for (int i = 0; i < menuItems.Length; i++)
            {
                if (i == CurrentSelected)
                {
                    menuItems[i].play("selected");
                }
                else
                {
                    menuItems[i].play("idle");
                }
            }
        }

        float Lerp(float a, float b, float t)
        {
            return a + (b - a) * t;
        }
    }
}