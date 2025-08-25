using Fnf.Framework;
using System;

namespace Fnf.Game
{
    public class SlideValueEffect : Effect, IUpdatable
    {
        public bool isUpdatable { get; set; } = true;

        float from, to;
        Action<float> action;

        public SlideValueEffect(float cooldown, float duration, float from, float to, Action<float> action)
        {
            this.remaining = duration;
            this.duration = duration;
            this.cooldown = cooldown;
            this.action = action;
            this.from = from;
            this.to = to;
        }

        public void Update()
        {
            if (!isUpdatable) return;

            if (remaining > 0)
            {
                action?.Invoke(Lerp(from, to, 1 - (remaining / duration)));
            }
            else
            {
                return;
            }

            if (cooldown > 0)
            {
                cooldown -= Time.deltaTime;
                return;
            }

            float tem = remaining;
            remaining -= Time.deltaTime;

            if (tem > 0 && remaining <= 0)
            {
                action?.Invoke(to);
            }
        }

        float Lerp(float a, float b, float t)
        {
            return a + (b - a) * t;
        }
    }
}