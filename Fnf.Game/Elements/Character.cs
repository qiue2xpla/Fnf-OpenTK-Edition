using Fnf.Framework;
using System.IO;

namespace Fnf.Game
{
    public class Character : Animator, IUpdatable
    {
        public Character(string configuration)
        {
            string[] anims = File.ReadAllLines($"{GamePaths.CharactersConfigurations}/{configuration}.txt");
            for (int a = 0; a < anims.Length; a++)
            {
                string[] animArgs = StringUtility.SplitValues(anims[a], 0);
                TextureAtlas.LoadAtlas(animArgs[1], $"{GamePaths.Characters}/{animArgs[1]}");
                if (animArgs.Length >= 3)
                {
                    Animation animation = TextureAtlas.GetAnimation(animArgs[1], animArgs[2]);
                    if (animArgs.Length == 5)
                    {
                        animation.offset = new Vector2(float.Parse(animArgs[3]), float.Parse(animArgs[4]));
                    }

                    add(animArgs[0], animation);
                    play("idle");
                }
            }
        }
        //private float _idleCountdown = 0;

        public new void Update()
        {
            /*bool HasFinished = _idleCountdown > 0;
            _idleCountdown -= Time.deltaTime;
            if (_idleCountdown < 0)
            {
                _idleCountdown = 0;
                if (HasFinished) Idle();
            }*/

            base.Update();

            cooldown = MathUtility.Clamp(cooldown - Time.deltaTime, 100, 0);
        }

        float cooldown;

        public void Hit(string anim)
        {
            play(anim);
            cooldown = 0.6f;
        }

        public void Idle()
        {
            if (cooldown > 0) return; 

            play("idle");

        }
    }
}