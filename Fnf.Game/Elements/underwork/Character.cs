using Fnf.Framework;

namespace Fnf.Game
{
    public class Character
    {
        public CharacterSkin skin;
        public Vector2 position;
        public Vector2 scale = new Vector2(1,1);
        public float rotation;

        private string[] _directions = new string[] { "right", "up", "down", "left" }; 
        private float _idleCountdown = 0;
        private Animator animator;

        public Character(CharacterSkin skin)
        {
            this.skin = skin;
            animator = new Animator();
            animator.add("idle", skin.Idle);
            for (int i = 0; i < 4; i++) 
                animator.add(_directions[i] + "_hit", skin.Hit[i]);
            if (skin.Miss != null)
                for (int i = 0; i < 4; i++) 
                    animator.add(_directions[i] + "_miss", skin.Miss[i]);
            animator.play("idle");
        }

        public void Update()
        {
            bool HasFinished = _idleCountdown > 0;
            _idleCountdown -= Time.deltaTime;
            if(_idleCountdown < 0)
            {
                _idleCountdown = 0;
                if (HasFinished) Idle();
            }
            animator.Update();
        }

        public void Render()
        {
            animator.Render();
        }

        public void Idle()
        {
            animator.play("idle");
        }

        public void Hit(int column, bool missed)
        {
            if (skin.Miss != null && missed)
            {
                animator.play(_directions[column] + "_miss");
            }
            else
            {
                animator.play(_directions[column] + "_hit");
            }

            _idleCountdown = .3f;
        }
    }
}