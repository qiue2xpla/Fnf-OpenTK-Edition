using Fnf.Framework.Audio;
using Fnf.Framework;
using Fnf.Game;

namespace Fnf
{
    public class Intro : Script
    {
        // States
        static bool introShown = false;
        bool transitioning = false;
        bool skippedIntro = false;
        bool danceLeft;

        Animator Logo, GF, PETB;
        IntroText introText;

        public Intro()
        {
            #region Setup Animations

            // Load atlases
            TextureAtlas.LoadAtlas("logo", "Assets/Shared/Intro/logo");
            TextureAtlas.LoadAtlas("gf", "Assets/Shared/Intro/GfIntro");
            TextureAtlas.LoadAtlas("petb", "Assets/Shared/Intro/titleEnter");

            // Make the animators
            GF = new Animator();
            Logo = new Animator();
            PETB = new Animator();

            // Position the animators
            GF.globalPosition = new Vector2(318, -45);
            Logo.globalPosition = new Vector2(-298, 119);
            PETB.globalPosition = new Vector2(243, -303);

            // Some animations need to be looped
            Animation idle = TextureAtlas.GetAnimation("petb", "Press Enter to Begin"); idle.looped = true;
            Animation pressed = TextureAtlas.GetAnimation("petb", "ENTER PRESSED"); pressed.looped = true;

            // Add animations to the animators
            Logo.add("logo", TextureAtlas.GetAnimation("logo", "logo"));
            GF.add("left", TextureAtlas.GetAnimation("gf", "gfDance", 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14));
            GF.add("right", TextureAtlas.GetAnimation("gf", "gfDance", 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29));
            PETB.add("pressed", pressed);
            PETB.add("idle", idle);

            PETB.play("idle");

            #endregion

            ShowAnimators(false);

            introText = new IntroText();

            // Setup beat system
            Music.BeatsPerMinute = 102;
            Music.OnBeatHit += OnBeatHit;

            if (introShown) skipIntro();
            else
            {
                introShown = true;
                Music.LoadMenuMusic();
                Music.Play();
                Music.Instruments.slideVolume(0, 0.1f, 4);
            }
        }

        void Update()
        {
            GF.Update();
            Logo.Update();
            PETB.Update();

            Music.Update();
            GlobalSystems.VolumeControl.Update();
            EffectsManager.Update();

            if (Input.GetAnyKeysDown(Key.Enter, Key.KeypadEnter, Key.Space))
            {
                if (!transitioning && skippedIntro)
                {
                    PETB.play("pressed");

                    new Clip("Assets/Shared/Confirm.ogg") { volume = 0.7f, endAction = EndAction.Dispose }.play(); 

                    transitioning = true;

                    new FlashEffect(0, 1, Color.White);
                    new TransitionInEffect(1, 1, Color.Black, delegate { Active = new MainMenu(); });
                }

                if (!skippedIntro)
                {
                    new Clip("Assets/Shared/Confirm.ogg") { volume = 0.7f, endAction = EndAction.Dispose }.play();
                    skipIntro();
                }
            }
        }

        void Render()
        {
            GF.Render();
            Logo.Render();
            PETB.Render();
            introText.Render();

            GlobalSystems.VolumeControl.Render();

            EffectsManager.Render();
        }

        void OnBeatHit(int beat)
        {
            danceLeft = !danceLeft;
            if (danceLeft) GF.play("right");
            else GF.play("left");

            Logo.play("logo");

            if (skippedIntro) return;
            switch (Music.PositionInBeats)
            {
                case 1: introText.Clear(2); introText.AddRandom(); break;
                case 3: introText.AddRandom(); break;
                case 4: introText.Clear(2); break;
                case 5: introText.AddRandom(); break;
                case 7: introText.AddRandom(); break;
                case 8: introText.Clear(2); break;
                case 9: introText.AddRandom(); break;
                case 11: introText.AddRandom(); break;
                case 12: introText.Clear(4); break; // IDK
                case 13: introText.AddText("Friday"); break;
                case 14: introText.AddText("Night"); break;
                case 15: introText.AddText("Funkin"); break;
                case 16: skipIntro(); break;
            }
        }

        void skipIntro()
        {
            if (skippedIntro) return;

            skippedIntro = true;
            introShown = true;

            new FlashEffect(0, 4, Color.White);

            ShowAnimators(true);

            introText.Clear(0);
        }

        void ShowAnimators(bool state)
        {
            GF.isRenderable = state;
            PETB.isRenderable = state;
            Logo.isRenderable = state;
        }
    }
}