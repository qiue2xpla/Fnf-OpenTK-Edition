using Fnf.Framework;

namespace Fnf.Game
{
    public class Character : Animator, IUpdatable
    {
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
        }

        public void Idle()
        {
            play("idle");
        }
    }
}