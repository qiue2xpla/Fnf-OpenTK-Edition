using Fnf.Framework;
using System;

namespace Fnf.Game
{
    public class FlickerRenderEffect : Effect, IUpdatable
    {
        public bool isUpdatable { get; set; } = true;

        float passed;
        float timesPersecond;
        IRenderable target;
        event Action onEndCallback;

        public FlickerRenderEffect(float cooldown, float duration, float timesPerSecond, IRenderable target, Action callback = null)
        {
            this.target = target;
            this.cooldown = cooldown;
            this.duration = duration;

            onEndCallback = callback;
            remaining = duration;
            passed = 0;
            timesPersecond = timesPerSecond;
        }

        public void Update()
        {
            if (!isUpdatable) return;

            if (cooldown > 0)
            {
                cooldown -= Time.deltaTime;
                return;
            }

            if(remaining > 0)
            {
                passed += Time.deltaTime;
                if(passed > 1f / timesPersecond)
                {
                    passed -= 1 / timesPersecond;
                    target.isRenderable = !target.isRenderable;
                }
            }

            float tem = remaining;
            remaining -= Time.deltaTime;

            if (tem > 0 && remaining <= 0)
            {
                target.isRenderable = true;
                onEndCallback?.Invoke();
            }
        }

        public override bool isFinished()
        {
            return remaining <= 0;
        }
    }
}